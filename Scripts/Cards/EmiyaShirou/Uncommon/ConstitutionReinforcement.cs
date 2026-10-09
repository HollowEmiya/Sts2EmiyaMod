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
/// 构成补强：1费，*技能*，获得8点格挡，选择手中一张牌进行强化。  
/// 升级获得11点格挡。
/// </summary>
[RegisterCard(typeof(EmiyaShirouCardPool))]
public class ConstitutionReinforcement : EmiyaCardNode
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
        new BlockVar(8, ValueProp.Move)
    ];

    protected override IEnumerable<IHoverTip> AdditionalHoverTips =>
        HoverTipFactory.FromEnchantment<Reinforcement>(2);

    public ConstitutionReinforcement() :
        base(energyCost, type, rarity, targetType, shouldShowInCardLibrary)
    {
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
	{
        await CreatureCmd.GainBlock(base.Owner.Creature, base.DynamicVars.Block, cardPlay);

        CardModel? cardModel = 
            (await CardSelectCmd.FromHand(
                prefs: new CardSelectorPrefs(SelectionScreenPrompt, 1),
                context: choiceContext, player: Owner,
                filter: card => card.Enchantment == null
                    && (card.Type == CardType.Skill || card.Type == CardType.Attack) &&
                    Reinforcement.CardCanEnchant(card),
                source: this)).FirstOrDefault();
        if (cardModel != null)
		{
            CardCmd.Enchant<Reinforcement>(cardModel, 2);
		}
    }

    protected override void OnUpgrade()
	{
		base.DynamicVars.Block.UpgradeValueBy(3m);
	}
}