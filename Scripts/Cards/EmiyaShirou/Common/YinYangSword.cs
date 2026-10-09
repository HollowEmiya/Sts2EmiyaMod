using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;

namespace Sts2EmiyaMod.Scripts;

/// <summary>
/// 阴阳剑：0费，<u>攻击</u>，阳：造成3点伤害施加1层易伤，打出后切换为阴：造成3点伤害施加1层虚弱。
///   升级后造成6点，2层。
/// </summary>
[RegisterCard(typeof(EmiyaShirouCardPool))]
public class YinYangSword : EmiyaCardNode
{
    public const int energyCost = 0;

    private const CardType type = CardType.Attack;

    private const CardRarity rarity = CardRarity.Common;

    private const TargetType targetType = TargetType.AnyEnemy;

    // 是否在卡牌图鉴中显示
    private const bool shouldShowInCardLibrary = true;

    private const string YangPortraitPath =
        "res://Sts2EmiyaMod/Images/Cards/EmiyaShirou/EmiyaYangSword.png";
    private const string YinPortraitPath =
        "res://Sts2EmiyaMod/Images/Cards/EmiyaShirou/EmiyaYinSword.png";

    public override CardAssetProfile AssetProfile => new(
        PortraitPath: Yang ? YangPortraitPath : YinPortraitPath);

    public override IEnumerable<string> AllPortraitPaths => [YangPortraitPath, YinPortraitPath];

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

    protected override void AddExtraArgsToDescription(LocString description)
    {
        base.AddExtraArgsToDescription(description);
        // 每次生成描述时读取当前形态，供本地化文本的布尔条件使用。
        description.Add("Yang", Yang);
    }

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
