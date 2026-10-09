using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;
using STS2RitsuLib.Interop.AutoRegistration;

namespace Sts2EmiyaMod.Scripts;

/// <summary>
/// 强化暴走：1费，<u>攻击</u>，对所有敌人造成6点伤害，消耗一张牌。  
/// 本场战斗每投影或者强化一次，伤害额外增加4点。  
/// 升级造成10点伤害，伤害额外增加6点。
/// </summary>
[RegisterCard(typeof(EmiyaShirouCardPool))]
public class ReinforcementRampage : EmiyaCardNode
{
    public const int energyCost = 1;

    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new DamageVar(6m, ValueProp.Move),
        new ExtraDamageVar(4m),
        new DamageBasedCalculatedDamageVar(ValueProp.Move).WithMultiplier(
            static (card, _) => EmiyaTechniqueHistory.GetCount(card))
    ];

    protected override IEnumerable<IHoverTip> AdditionalHoverTips => [
        HoverTipFactory.FromKeyword(CardKeyword.Exhaust),
        HoverTipFactory.FromKeyword(EmiyaKeywords.Projection)
    ];

    public ReinforcementRampage() : base(energyCost, CardType.Attack,
        CardRarity.Uncommon, TargetType.AllEnemies)
    {
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        var combatState = CombatState;
        ArgumentNullException.ThrowIfNull(combatState);
        await DamageCmd.Attack(DynamicVars.CalculatedDamage)
            .FromCard(this, cardPlay)
            .TargetingAllOpponents(combatState)
            .WithHitFx("vfx/vfx_dramatic_stab", null, "blunt_attack.mp3")
            .Execute(choiceContext);

        var selectedCard = (await CardSelectCmd.FromHand(choiceContext, Owner,
            new CardSelectorPrefs(CardSelectorPrefs.ExhaustSelectionPrompt, 1), null, this))
            .FirstOrDefault();
        if (selectedCard != null)
            await CardCmd.Exhaust(choiceContext, selectedCard);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(4m);
        DynamicVars.ExtraDamage.UpgradeValueBy(2m);
    }
}
