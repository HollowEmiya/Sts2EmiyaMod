using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.Nodes.Rooms;
using MegaCrit.Sts2.Core.Nodes.Vfx;
using MegaCrit.Sts2.Core.ValueProps;
using STS2RitsuLib.Interop.AutoRegistration;

namespace Sts2EmiyaMod.Scripts;

/// <summary>
/// 圣杯：2费，每回合获得2点能量，获得3点力量，3点敏捷。
/// </summary>
[RegisterCard(typeof(EmiyaShirouCardPool))]
public class HolyGrail : EmiyaCardNode
{
    public override bool GainsBlock => false;
    
    /// <summary>
    /// 基础耗能    
    /// </summary>;
    public const int energyCost = 2;

    private const CardType type = CardType.Power;

    private const CardRarity rarity = CardRarity.Ancient;

    private const TargetType targetType = TargetType.Self;

    // 是否在卡牌图鉴中显示
    private const bool shouldShowInCardLibrary = true;

    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new PowerVar<DexterityPower>(3m),
        new PowerVar<StrengthPower>(3m),
        new EnergyVar(2)
    ];

    protected override IEnumerable<IHoverTip> AdditionalHoverTips => [
        HoverTipFactory.FromPower<DexterityPower>(),
        HoverTipFactory.FromPower<StrengthPower>(),
        base.EnergyHoverTip
    ];

    public HolyGrail() :
        base(energyCost, type, rarity, targetType, shouldShowInCardLibrary)
    {
        
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
	{
		NPowerUpVfx.CreateNormal(base.Owner.Creature);
		await PowerCmd.Apply<DexterityPower>(
            choiceContext, base.Owner.Creature,
            DynamicVars.Dexterity.BaseValue, base.Owner.Creature, this);

        await PowerCmd.Apply<StrengthPower>(
            choiceContext, base.Owner.Creature,
            DynamicVars.Strength.BaseValue, base.Owner.Creature, this);

        await PowerCmd.Apply<PyrePower>(
            choiceContext, base.Owner.Creature, 
            base.DynamicVars.Energy.BaseValue, base.Owner.Creature, this);
	}

	public override async Task OnEnqueuePlayVfx(Creature? target)
	{
        // 力量
		NCombatRoom.Instance?.CombatVfxContainer.
            AddChildSafely(NGroundFireVfx.Create(base.Owner.Creature));
		await CreatureCmd.TriggerAnim(base.Owner.Creature, 
            "Cast", base.Owner.Character.CastAnimDelay);
	}

	protected override void OnUpgrade()
	{
		base.DynamicVars.Strength.UpgradeValueBy(1m);
		base.DynamicVars.Dexterity.UpgradeValueBy(1m);
	}
}