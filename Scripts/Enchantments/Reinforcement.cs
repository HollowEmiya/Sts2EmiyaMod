using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Enchantments;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;

namespace Sts2EmiyaMod.Scripts;

[RegisterEnchantment]
public class Reinforcement : ModEnchantmentTemplate
{
    // 是否在卡牌上显示数值
    public override bool ShowAmount => true;

    // 是否会添加额外的卡牌描述文本
    public override bool HasExtraCardText => true;

    // 图标位置。大小1:1就行，原版是64x64
    public override EnchantmentAssetProfile AssetProfile => new(
        IconPath: $"res://Sts2EmiyaMod/Images/Enchantments/{GetType().Name}.png"
    );

    public override bool CanEnchantCardType(CardType cardType)
	{
		return cardType == CardType.Attack || 
        cardType == CardType.Skill;
	}

    public override decimal EnchantDamageAdditive(decimal originalDamage,
     ValueProp props)
	{
		if (!props.IsPoweredAttack())
		{
			return 0m;
		}
		return base.Amount;
	}

    // 修改卡牌获得的格挡值，返回增加的改变量。
    public override decimal EnchantBlockAdditive(decimal originalBlock)
    {
        // 获得格挡额外增加Amount数量。这个数量是你给予附魔时指定的。
        return Amount;
    }
}
