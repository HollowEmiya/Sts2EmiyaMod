using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;
using STS2RitsuLib.Interop.AutoRegistration;

namespace Sts2EmiyaMod.Scripts;

/// <summary>
/// 连携攻击：3费，对所有敌人造成12点伤害；本回合每打出一张牌，费用降低1点。
/// 升级后造成15点伤害。
/// </summary>
[RegisterCard(typeof(EmiyaShirouCardPool))]
public class CoordinatedAttack : EmiyaCardNode
{
    public const int energyCost = 3;

    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new DamageVar(12, ValueProp.Move)
    ];

    public CoordinatedAttack() : base(energyCost, CardType.Attack,
        CardRarity.Uncommon, TargetType.AllEnemies)
    {
    }

    public override bool TryModifyEnergyCostInCombat(CardModel card,
        decimal originalCost, out decimal modifiedCost)
    {
        modifiedCost = originalCost;
        if (card != this || CombatState == null)
            return false;

        int playedCount = CombatManager.Instance.History.CardPlaysFinished
            .Count(entry => entry.CardPlay.Card.Owner == Owner && entry.HappenedThisTurn(CombatState));
        modifiedCost = Math.Max(0m, originalCost - playedCount);
        return playedCount > 0;
    }

    public override Task AfterCardPlayed(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        if (cardPlay.Card.Owner == Owner)
            InvokeEnergyCostChanged();
        return Task.CompletedTask;
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        var combatState = CombatState;
        ArgumentNullException.ThrowIfNull(combatState);
        await DamageCmd.Attack(DynamicVars.Damage.BaseValue)
            .FromCard(this, cardPlay)
            .TargetingAllOpponents(combatState)
            .WithHitFx("vfx/vfx_dramatic_stab", null, "blunt_attack.mp3")
            .Execute(choiceContext);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(3m);
    }
}
