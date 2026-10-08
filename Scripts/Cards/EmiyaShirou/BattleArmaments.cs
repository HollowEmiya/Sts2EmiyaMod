using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Combat.History.Entries;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.CardPools;
using MegaCrit.Sts2.Core.Models.Cards;
using MegaCrit.Sts2.Core.ValueProps;
using STS2RitsuLib.Cards.DynamicVars;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Keywords;
using STS2RitsuLib.Scaffolding.Content;
namespace Sts2EmiyaMod.Scripts;

/// ToDo:要给锻造后的牌加魔力武器
// 注册卡牌到指定池（这里是无色）。如果要写自定义池看添加人物的开头
/// <summary>
/// 战斗武装:造成8点伤害，选择一张牌进行升级并锻造
/// </summary>
[RegisterCard(typeof(EmiyaShirouCardPool))]
public class BattleArmaments : EmiyaCardNode
{
    // 基础耗能
    private const int energyCost = 1;
    // 卡牌类型
    private const CardType type = CardType.Attack;
    // 卡牌稀有度
    private const CardRarity rarity = CardRarity.Common;
    // 目标类型（AnyEnemy表示任意敌人）
    private const TargetType targetType = TargetType.AnyEnemy;
    // 是否在卡牌图鉴中显示
    private const bool shouldShowInCardLibrary = true;

    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new DamageVar(8, ValueProp.Move)
    ];

    public BattleArmaments()  :
        base(energyCost, type, rarity, targetType, shouldShowInCardLibrary)
    {
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
	{
		ArgumentNullException.ThrowIfNull(cardPlay.Target, "cardPlay.Target");
		await DamageCmd.Attack(DynamicVars.Damage.BaseValue)
            .FromCard(this, cardPlay)
            .Targeting(cardPlay.Target!)
			.WithHitFx("vfx/vfx_dramatic_stab", null, "blunt_attack.mp3")
			.Execute(choiceContext);

        // if (base.IsUpgraded)
		// {
		// 	foreach (CardModel item in PileType.Hand.GetPile(base.Owner).Cards.Where((CardModel c) => c.IsUpgradable))
		// 	{
		// 		CardCmd.Upgrade(item);
        //         CardCmd.Enchant<Reinforcement>(item, 2);
		// 	}
		// 	return;
		// }
		CardModel cardModel = await EmiyaCardSelectCmd.FromHandForUpgradeAndEnchant(
            choiceContext, base.Owner, this);
		if (cardModel != null)
		{
			CardCmd.Upgrade(cardModel);
            CardCmd.Enchant<Reinforcement>(cardModel, 2);
		}
	}

	protected override void OnUpgrade()
	{
		base.DynamicVars.Damage.UpgradeValueBy(3m);
	}
}