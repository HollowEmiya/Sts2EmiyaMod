using System.Runtime.CompilerServices;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Models;

namespace Sts2EmiyaMod.Scripts;

// 以战斗状态为生命周期，避免跨战斗残留；每名玩家单独累计。
internal static class EmiyaTechniqueHistory
{
    private static readonly ConditionalWeakTable<ICombatState, Dictionary<Player, int>> Counts = new();

    public static void Record(ICombatState combat, Player player)
    {
        var counts = Counts.GetOrCreateValue(combat);
        counts[player] = counts.GetValueOrDefault(player) + 1;
        // 只响应当前成功操作，不把此前次数补发给后来生成的牌。
        if (player.PlayerCombatState is { } playerCombat)
        {
            foreach (var card in playerCombat.AllCards.OfType<ContinuousProjection>().ToArray())
                card.AfterTechniquePerformed();
        }
        player.PlayerCombatState?.RecalculateCardValues();
    }

    public static int GetCount(CardModel card)
    {
        if (card.CombatState is not { } combat || !Counts.TryGetValue(combat, out var counts))
            return 0;
        return counts.GetValueOrDefault(card.Owner);
    }
}
