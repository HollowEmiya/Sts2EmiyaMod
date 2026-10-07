using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;
using STS2RitsuLib.Interop.AutoRegistration;

namespace Sts2EmiyaMod.Scripts;

[RegisterCard(typeof(EmiyaShirouCardPool))]
/// <summary>
/// 赤原猎犬：0费，破除所有格挡，造成14伤害，施加2层易伤。  
/// 升级后，造成20点伤害，施加3层易伤。
/// </summary>
public class Hrunting : EmiyaCardNode
{
    public override bool GainsBlock => false;
    
    /// <summary>
    /// 基础耗能    
    /// </summary>;
    public const int energyCost = 0;

    private const CardType type = CardType.Attack;

    private const CardRarity rarity = CardRarity.Ancient;

    private const TargetType targetType = TargetType.AnyEnemy;

    // 是否在卡牌图鉴中显示
    private const bool shouldShowInCardLibrary = true;

    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new DamageVar(14, ValueProp.Move),
        new PowerVar<VulnerablePower>(2m)
    ];

    protected override IEnumerable<IHoverTip> AdditionalHoverTips => [
        HoverTipFactory.FromPower<VulnerablePower>()
    ];

    public Hrunting() :
        base(energyCost, type, rarity, targetType, shouldShowInCardLibrary)
    {
        
    }

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        
		ArgumentNullException.ThrowIfNull(cardPlay.Target, "cardPlay.Target");
        await CreatureCmd.LoseBlock(choiceContext, cardPlay.Target, 
            cardPlay.Target.Block,
            base.Owner.Creature);
            
        await DamageCmd.Attack(DynamicVars.Damage.BaseValue)
            .FromCard(this, cardPlay)
            .Targeting(cardPlay.Target!)
            .WithHitFx("vfx/vfx_dramatic_stab", null, "blunt_attack.mp3")
            .Execute(choiceContext);

        await PowerCmd.Apply<VulnerablePower>(choiceContext, cardPlay.Target, 
            base.DynamicVars.Vulnerable.BaseValue, base.Owner.Creature, this);
    }
    
    protected override void OnUpgrade()
	{
		base.DynamicVars.Damage.UpgradeValueBy(6m);
		base.DynamicVars.Vulnerable.UpgradeValueBy(1m);
	}
}