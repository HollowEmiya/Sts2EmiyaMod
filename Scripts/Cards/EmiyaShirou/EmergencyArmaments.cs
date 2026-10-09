using HarmonyLib;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Factories;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;
using Sts2EmiyaMod.Scripts;
using STS2RitsuLib.Interop.AutoRegistration;

/// <summary>
/// 紧急武装：1费，*技能*，获得5点格挡，消耗一张牌，投影1张随机牌。
/// CardFactory.GetDistinctForCombat  
/// 升级，获得7点格挡，升级后的牌。
/// </summary>
[RegisterCard(typeof(EmiyaShirouCardPool))]
public class EmergencyArmaments : EmiyaCardNode
{
    public override bool GainsBlock => true;
    
    /// <summary>
    /// 基础耗能    
    /// </summary>;
    public const int energyCost = 1;

    private const CardType type = CardType.Skill;

    private const CardRarity rarity = CardRarity.Common;

    private const TargetType targetType = TargetType.Self;

    // 是否在卡牌图鉴中显示
    private const bool shouldShowInCardLibrary = true;
    
    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new BlockVar(5, ValueProp.Move),
        new IntVar("ProjectionDefect", 2m)
    ];

    public EmergencyArmaments() :
        base(energyCost, type, rarity, targetType, shouldShowInCardLibrary)
    {
        
    }

     protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await CreatureCmd.GainBlock(base.Owner.Creature, base.DynamicVars.Block, cardPlay);

        // 角色List
        List<CardPoolModel> list = base.Owner.UnlockState.CharacterCardPools.ToList();
		// 所有卡
        IEnumerable<CardModel> cards = from c in list.SelectMany((CardPoolModel c) => c.GetUnlockedCards(base.Owner.UnlockState, base.Owner.RunState.CardMultiplayerConstraint))
            where !c.Tags.Contains(CardTag.OstyAttack) && !c.HasStarCostX
            select c;
        CardModel? card = CardFactory.GetDistinctForCombat(base.Owner, cards, 1,
            base.Owner.RunState.Rng.CombatCardGeneration).FirstOrDefault();
        
        if(card != null)
        {
            if (base.IsUpgraded)
            {
                CardCmd.Upgrade(card);
            }
            if(card.CanonicalStarCost > 0)
            {
                card.SetStarCostThisCombat(0);
                card.EnergyCost.AddThisCombat(card.CanonicalStarCost / 2);
            }
            EmiyaCardUtils.ApplyCopiedCardDamageAndBlockReduction(
                card, DynamicVars["ProjectionDefect"].IntValue);
            CardCmd.ApplyKeyword(card, EmiyaKeywords.Projection);
            card.Tags.AddItem(EmiyaTags.Projection);
            card.EnergyCost.AddThisCombat(-1);
            await CardPileCmd.AddGeneratedCardToCombat(card, PileType.Hand, base.Owner);
        }
    }

     protected override void OnUpgrade()
	{
		base.DynamicVars.Block.UpgradeValueBy(2m);
	}
}