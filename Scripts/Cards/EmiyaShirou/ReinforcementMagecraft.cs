using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;
using Sts2EmiyaMod.Scripts;
using STS2RitsuLib.Interop.AutoRegistration;

/// <summary>
/// 强化魔术：1费，*技能*，抽2牌，选择一张牌进行强化。
/// 升级抽3
/// </summary>
[RegisterCard(typeof(EmiyaShirouCardPool))]
public class ReinforcementMagecraft : EmiyaCardNode
{
    public override bool GainsBlock => false;
    
    /// <summary>
    /// 基础耗能    
    /// </summary>;
    public const int energyCost = 1;

    private const CardType type = CardType.Skill;

    private const CardRarity rarity = CardRarity.Common;

    private const TargetType targetType = TargetType.Self;

    // 是否在卡牌图鉴中显示
    private const bool shouldShowInCardLibrary = true;

    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new CardsVar(2)
    ];

    public ReinforcementMagecraft() :
        base(energyCost, type, rarity, targetType, shouldShowInCardLibrary)
    {
        
    }
    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
	{
		await CardPileCmd.Draw(choiceContext, base.DynamicVars.Cards.BaseValue, base.Owner);
		
        CardModel? cardModel = (await CardSelectCmd.FromHand(
            prefs: new CardSelectorPrefs(SelectionScreenPrompt, 1),
            context: choiceContext, player: base.Owner,
            filter: card => card.Enchantment == null
                && Reinforcement.CardCanEnchant(card),
            source: this)).FirstOrDefault();
		if (cardModel != null)
		{
            CardCmd.Enchant<Reinforcement>(cardModel, 2);
		}
	}

    protected override void OnUpgrade()
	{
		base.DynamicVars.Cards.UpgradeValueBy(1m);
	}
}