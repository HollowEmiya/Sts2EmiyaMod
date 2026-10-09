// using MegaCrit.Sts2.Core.CardSelection;
// using MegaCrit.Sts2.Core.Commands;
// using MegaCrit.Sts2.Core.Entities.Cards;
// using MegaCrit.Sts2.Core.GameActions.Multiplayer;
// using MegaCrit.Sts2.Core.HoverTips;
// using MegaCrit.Sts2.Core.Localization.DynamicVars;
// using MegaCrit.Sts2.Core.Models;
// using MegaCrit.Sts2.Core.ValueProps;
// using STS2RitsuLib.Interop.AutoRegistration;

// namespace Sts2EmiyaMod.Scripts;

// /// <summary>
// /// 钢铁为身：2费，*技能*，获得14点格挡，这回合每受到1次攻击，对攻击者造成2点伤害。  
// /// 升级获得16格挡，造成4点。
// /// TODO,要做能力和能力Icon
// /// </summary>
// [RegisterCard(typeof(EmiyaShirouCardPool))]
// public class SteelBody : EmiyaCardNode
// {
//     public override bool GainsBlock => true;
    
//     /// <summary>
//     /// 基础耗能    
//     /// </summary>;
//     public const int energyCost = 1;

//     private const CardType type = CardType.Skill;

//     private const CardRarity rarity = CardRarity.Common;

//     private const TargetType targetType = TargetType.Self;

//     // 是否在卡牌图鉴中显示
//     private const bool shouldShowInCardLibrary = true;

//     protected override IEnumerable<DynamicVar> CanonicalVars => [
//         new BlockVar(14, ValueProp.Move),
// 		new DynamicVar("DamageBack", 2m)
//     ];
// }