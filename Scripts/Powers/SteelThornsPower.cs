using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;

namespace Sts2EmiyaMod.Scripts;

[RegisterPower]
public class SteelThornsPower : ModPowerTemplate
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;

    public override PowerAssetProfile AssetProfile => new(
        IconPath: "res://Sts2EmiyaMod/Images/Powers/SteelThornsPower.png",
        BigIconPath: "res://Sts2EmiyaMod/Images/Powers/SteelThornsPower.png");

    public override async Task AfterDamageReceived(PlayerChoiceContext choiceContext,
        Creature target, DamageResult result, ValueProp props, Creature? dealer, CardModel? card)
    {
        // 与火焰屏障一致：每段攻击均触发，非攻击伤害和反伤不会触发。
        if (target == Owner && dealer != null && props.IsPoweredAttack())
            await CreatureCmd.Damage(choiceContext, dealer, Amount, ValueProp.Unpowered, Owner);
    }

    public override async Task AfterSideTurnEnd(PlayerChoiceContext choiceContext,
        CombatSide side, IEnumerable<Creature> participants)
    {
        // 保留到敌方回合结束，才能抵挡并反击敌人的攻击。
        if (Owner.Side != side)
            await PowerCmd.Remove(this);
    }
}
