using Godot;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.RelicPools;
using MegaCrit.Sts2.Core.Rooms;
using MegaCrit.Sts2.Core.Saves.Runs;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;

namespace Sts2EmiyaMod.Scripts;

[RegisterRelic(typeof(EmiyaShirouRelicPool))]
/// <summary>
/// 魔术回路EX：开局获得3点能量，战斗结束回复6点生命。
/// </summary>
public class MagicCircuitsEX : EmiyaShirouRelicNode
{
    public override RelicRarity Rarity => RelicRarity.Ancient;

    protected override IEnumerable<DynamicVar> CanonicalVars => 
        [
            new HealVar(6),
            new EnergyVar(3)
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