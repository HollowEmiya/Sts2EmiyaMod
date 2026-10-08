using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;

namespace Sts2EmiyaMod.Scripts;

public static class EmiyaCardUtils
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="card"></param>
    public static void ApplyCopiedCardDamageAndBlockReduction(CardModel card, Decimal acount)
    {
        if (card.DynamicVars.TryGetValue("Damage", out DynamicVar? damageVar))
        {
            damageVar.BaseValue = Math.Max(0m, damageVar.BaseValue - acount);
        }

        if (card.DynamicVars.TryGetValue("Block", out DynamicVar? blockVar))
        {
            blockVar.BaseValue = Math.Max(0m, blockVar.BaseValue - acount);
        }
    }
}