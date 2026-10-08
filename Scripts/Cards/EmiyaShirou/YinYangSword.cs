using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;
using STS2RitsuLib.Interop.AutoRegistration;

namespace Sts2EmiyaMod.Scripts;

/// <summary>
/// 阴阳剑：0费，<u>攻击</u>，阳：造成3点伤害施加1层虚弱，打出后切换为阴：造成3点伤害施加1层易伤。  
///   升级后造成6点，2层。
/// </summary>
[RegisterCard(typeof(EmiyaShirouCardPool))]
public class YinYangSword : EmiyaCardNode
{
    public const int energyCost = 0;

    private const CardType type = CardType.Attack;

    private const CardRarity rarity = CardRarity.Basic;

    private const TargetType targetType = TargetType.AnyEnemy;

    // 是否在卡牌图鉴中显示
    private const bool shouldShowInCardLibrary = true;

    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new DamageVar(3, ValueProp.Move),
        new PowerVar<VulnerablePower>(1m),
		new PowerVar<WeakPower>(1m)
    ];

    protected override IEnumerable<IHoverTip> AdditionalHoverTips => [
        HoverTipFactory.FromPower<VulnerablePower>(),
        HoverTipFactory.FromPower<WeakPower>(),
    ];

    public bool Yang = true;

    public YinYangSword() :
        base(energyCost, type, rarity, targetType, shouldShowInCardLibrary)
    {
        
    }

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
		ArgumentNullException.ThrowIfNull(cardPlay.Target, "cardPlay.Target");
        await DamageCmd.Attack(DynamicVars.Damage.BaseValue)
            .FromCard(this, cardPlay)
            .Targeting(cardPlay.Target!)
            .WithHitFx("vfx/vfx_dramatic_stab", null, "blunt_attack.mp3")
            .Execute(choiceContext);
        if(Yang)
        {
            await PowerCmd.Apply<VulnerablePower>(choiceContext, cardPlay.Target, 
                base.DynamicVars.Vulnerable.BaseValue, base.Owner.Creature, this);
        }
        else
        {
            await PowerCmd.Apply<WeakPower>(choiceContext, cardPlay.Target, 
                base.DynamicVars.Weak.BaseValue, base.Owner.Creature, this);
        }
        Yang = !Yang;
    }
    
    protected override void OnUpgrade()
	{
		base.DynamicVars.Damage.UpgradeValueBy(3m);
		base.DynamicVars.Vulnerable.UpgradeValueBy(1m);
		base.DynamicVars.Weak.UpgradeValueBy(1m);
	}
}