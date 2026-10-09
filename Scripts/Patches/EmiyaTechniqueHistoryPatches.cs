using HarmonyLib;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Combat.History;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Models;

namespace Sts2EmiyaMod.Scripts;

[HarmonyPatch(typeof(CardCmd), nameof(CardCmd.Enchant),
    new[] { typeof(EnchantmentModel), typeof(CardModel), typeof(decimal) })]
internal static class ReinforcementHistoryPatch
{
    [HarmonyPostfix]
    private static void Postfix(CardModel card, EnchantmentModel? __result)
    {
        // 只记录成功的命令；克隆、读档和预览不会经过这里。
        if (__result is Reinforcement && card.IsInCombat && card.CombatState is { } combat)
            EmiyaTechniqueHistory.Record(combat, card.Owner);
    }
}

[HarmonyPatch(typeof(CombatHistory), nameof(CombatHistory.CardGenerated))]
internal static class ProjectionHistoryPatch
{
    [HarmonyPostfix]
    private static void Postfix(ICombatState combatState, CardModel card, Player? creator)
    {
        // 投影关键词由所有投影入口实际写入；Tags.AddItem 不会修改原集合。
        if (creator != null && card.Keywords.Contains(EmiyaKeywords.Projection))
            EmiyaTechniqueHistory.Record(combatState, creator);
    }
}
