using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;
using STS2RitsuLib.Cards.DynamicVars;
using STS2RitsuLib.Interop.AutoRegistration;

namespace Sts2EmiyaMod.Scripts;

/// <summary>
/// 杜兰德尔：1费攻击，造成9点伤害，力量发挥3倍效果。
/// 升级后造成10点伤害，力量发挥5倍效果。
/// </summary>
[RegisterCard(typeof(EmiyaShirouCardPool))]
public class Durandal : EmiyaCardNode
{
    public const int energyCost = 1;

    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new DamageVar(9, ValueProp.Move),
        new IntVar("StrengthMultiplier", 3m)
    ];

    protected override IEnumerable<IHoverTip> AdditionalHoverTips => [
        HoverTipFactory.FromPower<StrengthPower>()
    ];

    public Durandal() : base(energyCost, CardType.Attack,
        CardRarity.Uncommon, TargetType.AnyEnemy)
    {
    }

    public override decimal ModifyDamageAdditive(Creature? target, decimal amount,
        ValueProp props, Creature? dealer, CardModel? cardSource, CardPlay? cardPlay)
    {
        if (cardSource != this || dealer != Owner.Creature || !props.IsPoweredAttack())
            return 0m;

        // 原版力量已贡献1倍，此处仅补足额外倍数；伤害预览也使用同一钩子。
        return Owner.Creature.GetPowerAmount<StrengthPower>()
            * (DynamicVars["StrengthMultiplier"].BaseValue - 1m);
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        ArgumentNullException.ThrowIfNull(cardPlay.Target, "cardPlay.Target");
        await DamageCmd.Attack(DynamicVars.Damage.BaseValue)
            .FromCard(this, cardPlay)
            .Targeting(cardPlay.Target)
            .WithHitFx("vfx/vfx_dramatic_stab", null, "blunt_attack.mp3")
            .Execute(choiceContext);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(1m);
        DynamicVars["StrengthMultiplier"].UpgradeValueBy(2m);
    }
}
