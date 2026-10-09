using HarmonyLib;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;
using STS2RitsuLib.Interop.AutoRegistration;

namespace Sts2EmiyaMod.Scripts;

/// <summary>
/// 1费，获得6点格挡，选择一张牌,\n如果是攻击、技能、能力牌进行投影,并丢弃。投影出的牌附带消耗词条,费用-1。  
///   升级后获得9点格挡。
/// </summary>
[RegisterCard(typeof(EmiyaShirouCardPool))]
// 注册成人物起始卡，后面是数量。不需要删除即可。
[RegisterCharacterStarterCard(typeof(EmiyaShirouCharacter), 1)]
public class Projection : EmiyaCardNode
{
    public override bool GainsBlock => true;
    
    /// <summary>
    /// 基础耗能    
    /// </summary>;
    public const int energyCost = 1;

    private const CardType type = CardType.Skill;

    private const CardRarity rarity = CardRarity.Basic;

    private const TargetType targetType = TargetType.Self;

    // 是否在卡牌图鉴中显示
    private const bool shouldShowInCardLibrary = true;

    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new BlockVar(5, ValueProp.Move),
        new IntVar("ProjectionDefect", 2m)
    ];

    protected override IEnumerable<IHoverTip> AdditionalHoverTips => [
        HoverTipFactory.FromKeyword(EmiyaKeywords.Projection)
    ];

    public Projection() :
        base(energyCost, type, rarity, targetType, shouldShowInCardLibrary)
    {
        
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
	{
		await CreatureCmd.GainBlock(base.Owner.Creature, base.DynamicVars.Block, cardPlay);
        CardModel? cardModel = (await CardSelectCmd.
            FromHandForDiscard(choiceContext, base.Owner,
            new CardSelectorPrefs(
                CardSelectorPrefs.DiscardSelectionPrompt, 1), null, this)).FirstOrDefault();
		if (cardModel != null)
		{
            if(cardModel.Type == CardType.Attack ||
                cardModel.Type == CardType.Skill ||
                cardModel.Type == CardType.Power)
            {
                CardModel card = cardModel.CreateClone();
                EmiyaCardUtils.
                    ApplyCopiedCardDamageAndBlockReduction(
                        card, DynamicVars["ProjectionDefect"].IntValue);
                CardCmd.ApplyKeyword(card, EmiyaKeywords.Projection);
                CardCmd.ApplyKeyword(card, CardKeyword.Exhaust);
                card.EnergyCost.AddThisCombat(-1);
                await CardPileCmd.AddGeneratedCardToCombat(card, PileType.Hand, base.Owner);
            }
			await CardCmd.Discard(choiceContext, cardModel);
		}
	}

    protected override void OnUpgrade()
    {
        DynamicVars.Block.UpgradeValueBy(3);
    }
}
