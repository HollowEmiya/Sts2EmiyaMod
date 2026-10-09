using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;
using STS2RitsuLib.Scaffolding.Content.Patches;

namespace Sts2EmiyaMod.Scripts;

[RegisterPower]
public class CircuitConnectPower : TemporaryStrengthPower, IModPowerAssetOverrides
{
    public override AbstractModel OriginModel => ModelDb.Card<CircuitConnect>();
    protected override bool IsPositive => true;

    // 使用肌肉药水效果的小图标和大图，标题仍取回路链接。
    public PowerAssetProfile AssetProfile => new(
        IconPath: ModelDb.Power<FlexPotionPower>().PackedIconPath,
        BigIconPath: ModelDb.Power<FlexPotionPower>().ResolvedBigIconPath);

    public string CustomIconPath => AssetProfile.IconPath!;
    public string CustomBigIconPath => AssetProfile.BigIconPath!;
}
