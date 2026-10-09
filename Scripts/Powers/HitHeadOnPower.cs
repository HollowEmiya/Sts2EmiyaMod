using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using STS2RitsuLib.Interop.AutoRegistration;

namespace Sts2EmiyaMod.Scripts;

// 沿用原版尖啸的临时力量逻辑，包括叠加、人工制品和回合结束恢复。
[RegisterPower]
public class HitHeadOnPower : TemporaryStrengthPower
{
    public override AbstractModel OriginModel => ModelDb.Card<HitHeadOn>();
    protected override bool IsPositive => false;
}
