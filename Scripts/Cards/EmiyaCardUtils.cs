using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Models.CardPools;

namespace Sts2EmiyaMod.Scripts;

public static class EmiyaCardUtils
{
    public static IEnumerable<CardModel> GetRandomProjectionPool(Player player) =>
        player.UnlockState.CharacterCardPools
            .Append(ModelDb.CardPool<ColorlessCardPool>())
            .Distinct()
            .SelectMany(pool => pool.GetUnlockedCards(player.UnlockState,
                player.RunState.CardMultiplayerConstraint))
            .Where(card => !card.Tags.Contains(CardTag.OstyAttack) && !card.HasStarCostX)
            .DistinctBy(card => card.Id);

    public static void ConvertStarsToEnergy(CardModel card)
    {
        if (card.CanonicalStarCost > 0)
        {
            card.SetStarCostThisCombat(0);
            card.EnergyCost.AddThisCombat(card.CanonicalStarCost / 2);
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="card"></param>
    public static void ApplyCopiedCardDamageAndBlockReduction(CardModel card, Decimal acount)
    {
        if (card.DynamicVars.TryGetValue("Damage", out DynamicVar? damageVar))
        {
            damageVar.BaseValue = Math.Max(1m, damageVar.BaseValue - acount);
        }

        if (card.DynamicVars.TryGetValue("Block", out DynamicVar? blockVar))
        {
            blockVar.BaseValue = Math.Max(1m, blockVar.BaseValue - acount);
        }
    }

    public static void ApplyCopiedCardDamageAndBlockPercentage(CardModel card, Decimal percentage)
    {
        if (card.DynamicVars.TryGetValue("Damage", out DynamicVar? damageVar))
        {
            damageVar.BaseValue = Math.Max(1m, damageVar.BaseValue * (1m - percentage));
        }

        if (card.DynamicVars.TryGetValue("Block", out DynamicVar? blockVar))
        {
            blockVar.BaseValue = Math.Max(1m, blockVar.BaseValue * (1m - percentage));
        }
    }
}
