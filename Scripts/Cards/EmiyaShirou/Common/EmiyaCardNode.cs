using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Models.CardPools;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;

namespace Sts2EmiyaMod.Scripts;

[RegisterCard(typeof(EmiyaShirouCardPool), Inherit = true)]
public abstract class EmiyaCardNode : ModCardTemplate
{
    // 卡图资源
    public override CardAssetProfile AssetProfile => new(
        PortraitPath: $"res://Sts2EmiyaMod/Images/Cards/EmiyaShirou/{GetType().Name}.png");

    public EmiyaCardNode(int baseCost, CardType type,
     CardRarity rarity, TargetType target, bool showInCardLibrary = true) :
      base(baseCost, type, rarity, target, showInCardLibrary)
    {
    }
}