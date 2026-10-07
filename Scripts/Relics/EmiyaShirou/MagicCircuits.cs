using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Rooms;
using STS2RitsuLib.Interop.AutoRegistration;

namespace Sts2EmiyaMod.Scripts;

[RegisterRelic(typeof(EmiyaShirouRelicPool))]
[RegisterCharacterStarterRelic(typeof(EmiyaShirouCharacter))]
/// <summary>
/// 魔术回路：开局获得1点能量，战斗结束回复3点生命。
/// </summary>
public class MagicCircuits : EmiyaShirouRelicNode
{
    public override RelicRarity Rarity => RelicRarity.Starter;

    protected override IEnumerable<DynamicVar> CanonicalVars => 
        [
            new HealVar(3),
            new EnergyVar(1)
        ];

    // public override RelicAssetProfile AssetProfile => new(
    //     // 85x85
    //     IconPath: $"res://Sts2EmiyaMod/Images/Relics/EmiyaShirou/{GetType().Name}.png",
    //     // outline 85x85
    //     IconOutlinePath: $"res://res://Sts2EmiyaMod/Images/Relics/EmiyaShirou/Outline/{GetType().Name}.png",
    //     // 256x256
    //     BigIconPath:$"res://res://Sts2EmiyaMod/Images/Relics/EmiyaShirou/Big/{GetType().Name}.png"
    // );

    public override async Task AfterSideTurnStart(
        CombatSide side, IReadOnlyList<Creature> participants, ICombatState combatState)
    {
        if(side == base.Owner.Creature.Side &&
            combatState.RoundNumber <= 1)
        {
            Flash();
            await PlayerCmd.GainEnergy(base.DynamicVars.Energy.BaseValue,
                base.Owner);
        }
    }

    public override async Task AfterCombatVictory(CombatRoom room)
    {
        if (!base.Owner.Creature.IsDead)
		{
			Flash();
			await CreatureCmd.Heal(base.Owner.Creature, base.DynamicVars.Heal.BaseValue);
		}
    }
}