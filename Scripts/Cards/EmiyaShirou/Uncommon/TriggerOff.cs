using HarmonyLib;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Factories;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using STS2RitsuLib.Interop.AutoRegistration;

namespace Sts2EmiyaMod.Scripts;

[RegisterCard(typeof(EmiyaShirouCardPool))]
public class TriggerOff : EmiyaCardNode
{
    public const int energyCost = 1;

    protected override IEnumerable<IHoverTip> AdditionalHoverTips => [
        HoverTipFactory.FromKeyword(CardKeyword.Exhaust),
        HoverTipFactory.FromKeyword(EmiyaKeywords.Projection)
    ];

    public TriggerOff() : base(energyCost, CardType.Skill,
        CardRarity.Uncommon, TargetType.Self)
    {
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        var exhaustedCard = (await CardSelectCmd.FromHand(choiceContext, Owner,
            new CardSelectorPrefs(CardSelectorPrefs.ExhaustSelectionPrompt, 1), null, this))
            .FirstOrDefault();
        if (exhaustedCard != null)
            await CardCmd.Exhaust(choiceContext, exhaustedCard);

        var choices = CardFactory.GetDistinctForCombat(Owner,
            EmiyaCardUtils.GetRandomProjectionPool(Owner), 3,
            Owner.RunState.Rng.CombatCardGeneration).ToList();
        foreach (var card in choices)
        {
            if (IsUpgraded)
                CardCmd.Upgrade(card);
            EmiyaCardUtils.ConvertStarsToEnergy(card);
            CardCmd.ApplyKeyword(card, EmiyaKeywords.Projection);
        }
        if (choices.Count == 0)
            return;

        var selectedCard = await CardSelectCmd.FromChooseACardScreen(
            choiceContext, choices, Owner, canSkip: false);
        if (selectedCard != null)
            await CardPileCmd.AddGeneratedCardToCombat(selectedCard, PileType.Hand, Owner);
    }
}
