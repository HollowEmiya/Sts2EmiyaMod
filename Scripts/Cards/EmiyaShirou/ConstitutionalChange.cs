using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;
using STS2RitsuLib.Interop.AutoRegistration;

namespace Sts2EmiyaMod.Scripts;

/// <summary>
/// 构成变更：1费，<u>攻击</u>，造成9点伤害，选择一张牌消耗，如果消耗的牌为攻击牌，额外造成9点伤害。  
/// 升级造成11点。
/// </summary>
[RegisterCard(typeof(EmiyaShirouCardPool))]
public class ConstitutionalChange : EmiyaCardNode
{
    public const int energyCost = 1;

    private const CardType type = CardType.Attack;

    private const CardRarity rarity = CardRarity.Common;

    private const TargetType targetType = TargetType.AnyEnemy;

    // 是否在卡牌图鉴中显示
    private const bool shouldShowInCardLibrary = true;

    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new DamageVar(9, ValueProp.Move)
    ];

    protected override IEnumerable<IHoverTip> AdditionalHoverTips => [
        HoverTipFactory.FromKeyword(CardKeyword.Exhaust)
    ];

    public ConstitutionalChange() :
        base(energyCost, type, rarity, targetType, shouldShowInCardLibrary)
    {
        
    }

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
		ArgumentNullException.ThrowIfNull(cardPlay.Target, "cardPlay.Target");
        await DamageCmd.Attack(DynamicVars.Damage.BaseValue)
            .FromCard(this, cardPlay)
            .Targeting(cardPlay.Target!)
            .WithHitFx("vfx/vfx_dramatic_stab", null, "blunt_attack.mp3")
            .Execute(choiceContext);
        CardModel? cardModel = (await CardSelectCmd.FromHand(
            prefs: new CardSelectorPrefs(
                CardSelectorPrefs.ExhaustSelectionPrompt, 1),
                context: choiceContext, player: base.Owner,
                filter: null, source: this)).FirstOrDefault();
        // 没有可选手牌时，只结算第一次伤害。
        if (cardModel == null)
            return;

        bool exhaustedAttack = cardModel.Type == CardType.Attack;
        await CardCmd.Exhaust(choiceContext, cardModel);
        if (exhaustedAttack)
        {
            await DamageCmd.Attack(DynamicVars.Damage.BaseValue)
                .FromCard(this, cardPlay)
                .Targeting(cardPlay.Target!)
                .WithHitFx("vfx/vfx_dramatic_stab", null, "blunt_attack.mp3")
                .Execute(choiceContext);
        }
    }

    protected override void OnUpgrade()
	{
		base.DynamicVars.Damage.UpgradeValueBy(2m);
	}
}
