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
/// 钢铁为身：2费，*技能*，获得14点格挡，这回合每受到1次攻击，对攻击者造成2点伤害。  
/// 升级获得16格挡，造成4点。
/// 能力用来对攻击者造成伤害，名字：SteelThornsPower
/// </summary>
[RegisterCard(typeof(EmiyaShirouCardPool))]
public class SteelBody : EmiyaCardNode
{
    public override bool GainsBlock => true;
    
    /// <summary>
    /// 基础耗能    
    /// </summary>;
    public const int energyCost = 2;

    private const CardType type = CardType.Skill;

    private const CardRarity rarity = CardRarity.Common;

    private const TargetType targetType = TargetType.Self;

    // 是否在卡牌图鉴中显示
    private const bool shouldShowInCardLibrary = true;

    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new BlockVar(14, ValueProp.Move),
		new DynamicVar("DamageBack", 2m)
    ];

    protected override IEnumerable<IHoverTip> AdditionalHoverTips => [
        HoverTipFactory.FromPower<SteelThornsPower>()
    ];

    public SteelBody() : base(energyCost, type, rarity, targetType, shouldShowInCardLibrary)
    {
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await CreatureCmd.GainBlock(Owner.Creature, DynamicVars.Block, cardPlay);
        await PowerCmd.Apply<SteelThornsPower>(choiceContext, Owner.Creature,
            DynamicVars["DamageBack"].BaseValue, Owner.Creature, this);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Block.UpgradeValueBy(2m);
        DynamicVars["DamageBack"].UpgradeValueBy(2m);
    }
}
