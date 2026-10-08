using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Context;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Multiplayer;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Logging;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Multiplayer.Game;
using MegaCrit.Sts2.Core.Nodes.Combat;
using MegaCrit.Sts2.Core.Nodes.Rooms;
using MegaCrit.Sts2.Core.Runs;

namespace Sts2EmiyaMod.Scripts;

public static class EmiyaCardSelectCmd
{
    private static bool ShouldSelectLocalCard(Player player)
    {
        if (LocalContext.IsMe(player))
        {
            return RunManager.Instance.NetService.Type !=
                NetGameType.Replay;
        }

        return false;
    }

    private static void UndoEndTurnIfNecessary(Player player)
    {
        if (CombatManager.Instance.IsPlayerReadyToEndTurn(player) && player.Creature.CombatState != null && player.Creature.CombatState.CurrentSide == CombatSide.Player)
        {
            CombatManager.Instance.UndoReadyToEndTurn(player);
        }
    }
    
    private static void LogChoice(Player player, IEnumerable<CardModel?> cards)
    {
        string value = string.Join(",", from c in cards.OfType<CardModel>()
                                        select c.Id.Entry);
        Log.Info($"Player {player.NetId} chose cards [{value}]");
    }

    public static async Task<CardModel?> FromHandForUpgradeAndEnchant(PlayerChoiceContext context, Player player, AbstractModel source)
    {
        if (CombatManager.Instance.IsOverOrEnding)
        {
            return null;
        }

        if (ShouldSelectLocalCard(player))
        {
            NPlayerHand.Instance?.CancelAllCardPlay();
        }

        UndoEndTurnIfNecessary(player);
        uint? choiceId = null;
        if (CardSelectCmd.Selector == null)
        {
            choiceId = RunManager.Instance.PlayerChoiceSynchronizer.ReserveChoiceId(player);
            await context.SignalPlayerChoiceBegun(player, PlayerChoiceOptions.CancelPlayCardActions);
        }

        List<CardModel> list = 
            PileType.Hand.GetPile(player).Cards.
                Where((CardModel c) => c.IsUpgradable && c.Enchantment == null).ToList();
        CardModel result;
        if (list.Count <= 1)
        {
            result = list.FirstOrDefault();
        }
        else if (CardSelectCmd.Selector != null)
        {
            result = (await CardSelectCmd.Selector.GetSelectedCards(list, 1, 1)).FirstOrDefault();
        }
        else if (ShouldSelectLocalCard(player))
        {
            if (CardSelectCmd.LocalSelector != null)
            {
                result = (await CardSelectCmd.LocalSelector.GetSelectedCards(list, 1, 1)).FirstOrDefault();
            }
            else
            {
                result = (await NCombatRoom.Instance.Ui.Hand.SelectCards(new CardSelectorPrefs(new LocString("gameplay_ui", "CHOOSE_CARD_UPGRADE_HEADER"), 1), (CardModel c) => c.IsUpgradable, source, NPlayerHand.Mode.UpgradeSelect)).FirstOrDefault();
                RunManager.Instance.PlayerChoiceSynchronizer.SyncLocalChoice(player, choiceId.Value, PlayerChoiceResult.FromMutableCombatCard(result));
            }
        }
        else
        {
            result = (await RunManager.Instance.PlayerChoiceSynchronizer.WaitForRemoteChoice(player, choiceId.Value)).AsCombatCards().FirstOrDefault();
        }

        if (choiceId.HasValue)
        {
            await context.SignalPlayerChoiceEnded();
        }

        LogChoice(player, new []{result});
        return result;
    }
}