using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Factories;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using Sts2EmiyaMod.Scripts;
using STS2RitsuLib.Interop.AutoRegistration;

/// <summary>
/// 紧急投影：1费，*技能*，获得从3张随机牌选择一张，并进行投影-2。  
/// 升级后从5张升级过的随机牌中选择一张。
/// </summary>
[RegisterCard(typeof(EmiyaShirouCardPool))]
public class EmergencyProjection : EmiyaCardNode
{
    public override bool GainsBlock => false;
    
    /// <summary>
    /// 基础耗能    
    /// </summary>;
    public const int energyCost = 1;

    private const CardType type = CardType.Skill;

    private const CardRarity rarity = CardRarity.Uncommon;

    private const TargetType targetType = TargetType.Self;

    // 是否在卡牌图鉴中显示
    private const bool shouldShowInCardLibrary = true;
    
    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new IntVar("ProjectionDefect", 2m),
        new CardsVar(3)
    ];

    public EmergencyProjection() :
        base(energyCost, type, rarity, targetType, shouldShowInCardLibrary)
    {
        
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        IEnumerable<CardModel> cards = EmiyaCardUtils.GetRandomProjectionPool(Owner);
        List<CardModel> list2 = CardFactory.GetDistinctForCombat(
            base.Owner, cards, (int)base.DynamicVars.Cards.BaseValue,
            base.Owner.RunState.Rng.CombatCardGeneration).ToList();
        if (base.IsUpgraded)
        {
            foreach (CardModel item in list2)
            {
                CardCmd.Upgrade(item);
            }
        }
        CardModel? cardModel = await CardSelectCmd.FromChooseACardScreen(
            choiceContext, list2, base.Owner, canSkip: true);
        if (cardModel != null)
		{
            EmiyaCardUtils.ConvertStarsToEnergy(cardModel);
            EmiyaCardUtils.ApplyCopiedCardDamageAndBlockReduction(
                cardModel, DynamicVars["ProjectionDefect"].IntValue);
            CardCmd.ApplyKeyword(cardModel, EmiyaKeywords.Projection);
            cardModel.EnergyCost.AddThisCombat(-1);
			cardModel.SetToFreeThisTurn();
			await CardPileCmd.AddGeneratedCardToCombat(cardModel, PileType.Hand, base.Owner);
		}
    }

    protected override void OnUpgrade()
	{
		base.DynamicVars.Cards.UpgradeValueBy(2m);
	}
}