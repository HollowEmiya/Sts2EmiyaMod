/// <summary>
/// 双刀流：1费，<u>攻击</u>，造成8点伤害，手牌中每有1张牌，伤害增加1点。  
/// 升级，造成10点伤害，每有1张牌，伤害增加2点。
/// </summary>
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;
using STS2RitsuLib.Cards.DynamicVars;
using STS2RitsuLib.Interop.AutoRegistration;

namespace Sts2EmiyaMod.Scripts;

[RegisterCard(typeof(EmiyaShirouCardPool))]
public class DualWielding : EmiyaCardNode
{
    public const int energyCost = 1;

    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new DamageVar(8, ValueProp.Move),
        new IntVar("DamagePerCard", 1m)
    ];

    public DualWielding() : base(energyCost, CardType.Attack,
        CardRarity.Common, TargetType.AnyEnemy)
    {
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        ArgumentNullException.ThrowIfNull(cardPlay.Target, "cardPlay.Target");
        int handCount = PileType.Hand.GetPile(Owner).Cards.Count;
        decimal damage = DynamicVars.Damage.BaseValue
            + handCount * DynamicVars["DamagePerCard"].BaseValue;
        await DamageCmd.Attack(damage)
            .FromCard(this, cardPlay)
            .Targeting(cardPlay.Target)
            .WithHitFx("vfx/vfx_dramatic_stab", null, "blunt_attack.mp3")
            .Execute(choiceContext);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(2m);
        DynamicVars["DamagePerCard"].UpgradeValueBy(1m);
    }
}
