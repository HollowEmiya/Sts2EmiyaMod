using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;
using STS2RitsuLib.Interop.AutoRegistration;

namespace Sts2EmiyaMod.Scripts;

/// <summary>
/// 强化连携：1费，<u>攻击</u>，造成6点伤害，选择手牌中一张带有强化附魔的牌打出。  
/// 升级变成0费。
/// 强化附魔是Reinforcement
/// </summary>
[RegisterCard(typeof(EmiyaShirouCardPool))]
public class ReinforcementCombo : EmiyaCardNode
{
    public const int energyCost = 1;

    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new DamageVar(6, ValueProp.Move)
    ];

    protected override IEnumerable<IHoverTip> AdditionalHoverTips => [
        new HoverTip(
            new LocString("enchantments", "STS2_EMIYA_MOD_ENCHANTMENT_REINFORCEMENT.title"),
            new LocString("enchantments", "STS2_EMIYA_MOD_ENCHANTMENT_REINFORCEMENT.generalDescription"))
    ];

    public ReinforcementCombo() : base(energyCost, CardType.Attack,
        CardRarity.Common, TargetType.AnyEnemy)
    {
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        ArgumentNullException.ThrowIfNull(cardPlay.Target, "cardPlay.Target");
        await DamageCmd.Attack(DynamicVars.Damage.BaseValue)
            .FromCard(this, cardPlay)
            .Targeting(cardPlay.Target)
            .WithHitFx("vfx/vfx_dramatic_stab", null, "blunt_attack.mp3")
            .Execute(choiceContext);

        CardModel? selectedCard = (await CardSelectCmd.FromHand(
            prefs: new CardSelectorPrefs(SelectionScreenPrompt, 1),
            context: choiceContext, player: Owner,
            filter: card => card.Enchantment is Reinforcement
                && !card.Keywords.Contains(CardKeyword.Unplayable),
            source: this)).FirstOrDefault();
        if (selectedCard == null)
            return;

        // 单体敌方牌优先连击原目标；其他目标类型交由自动打出处理。
        var target = selectedCard.TargetType == TargetType.AnyEnemy
            && cardPlay.Target.IsAlive ? cardPlay.Target : null;
        await CardCmd.AutoPlay(choiceContext, selectedCard, target);
    }

    protected override void OnUpgrade()
    {
        EnergyCost.UpgradeBy(-1);
    }
}
