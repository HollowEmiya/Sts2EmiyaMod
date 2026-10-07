using MegaCrit.Sts2.Core.Entities.Relics;
using STS2RitsuLib.Scaffolding.Content;

namespace Sts2EmiyaMod.Scripts;

public class EmiyaShirouRelicNode : ModRelicTemplate
{
    public override RelicRarity Rarity => RelicRarity.Common;

    public override RelicAssetProfile AssetProfile => new(
        // 85x85
        IconPath: $"res://Sts2EmiyaMod/Images/Relics/EmiyaShirou/{GetType().Name}.png",
        // outline 85x85
        IconOutlinePath: $"res://res://Sts2EmiyaMod/Images/Relics/EmiyaShirou/Outline/{GetType().Name}.png",
        // 256x256
        BigIconPath:$"res://res://Sts2EmiyaMod/Images/Relics/EmiyaShirou/Big/{GetType().Name}.png"
    );
}