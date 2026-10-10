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
/// 鹤翼三连：1费攻击，破除目标全部格挡；若原本有格挡，施加1层易伤，造成3点伤害3次。
/// 升级后施加2层易伤，每次造成4点伤害。
/// </summary>
[RegisterCard(typeof(EmiyaShirouCardPool))]
public class TripleCraneWings : EmiyaCardNode
{
    public const int energyCost = 1;

    protected override HashSet<CardTag> CanonicalTags =>
            new HashSet<CardTag> {
                CardTag.Strike,
                EmiyaTags.GanJiangMoYe
            };

    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new DamageVar(3, ValueProp.Move),
        new PowerVar<VulnerablePower>(1m)
    ];

    protected override IEnumerable<IHoverTip> AdditionalHoverTips => [
        HoverTipFactory.FromPower<VulnerablePower>()
    ];

    public TripleCraneWings() : base(energyCost, CardType.Attack,
        CardRarity.Uncommon, TargetType.AnyEnemy)
    {
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        ArgumentNullException.ThrowIfNull(cardPlay.Target, "cardPlay.Target");
        var target = cardPlay.Target;
        int originalBlock = target.Block;
        if (originalBlock > 0)
        {
            await CreatureCmd.LoseBlock(choiceContext, target, originalBlock, Owner.Creature);
            await PowerCmd.Apply<VulnerablePower>(choiceContext, target,
                DynamicVars.Vulnerable.BaseValue, Owner.Creature, this);
        }

        await DamageCmd.Attack(DynamicVars.Damage.BaseValue)
            .WithHitCount(3)
            .FromCard(this, cardPlay)
            .Targeting(target)
            .WithHitFx("vfx/vfx_dramatic_stab", null, "blunt_attack.mp3")
            .Execute(choiceContext);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(1m);
        DynamicVars.Vulnerable.UpgradeValueBy(1m);
    }
}
