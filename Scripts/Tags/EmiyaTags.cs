using MegaCrit.Sts2.Core.Entities.Cards;
using Sts2EmiyaMod.Scripts;
using STS2RitsuLib.CardTags;
using STS2RitsuLib.Content;
using STS2RitsuLib.Interop.AutoRegistration;

namespace Sts2EmiyaMod.Scripts;

[RegisterOwnedCardTag(nameof(Projection))]
[RegisterOwnedCardTag(nameof(GanJiangMoYe))]
// [RegisterOwnedCardTag(nameof(Heavy2))] // 添加更多就新加这个特性
public class EmiyaTags
{
    public static readonly CardTag Projection =
        ModContentRegistry.GetQualifiedCardTagId(
            Entry.ModId, nameof(Projection)).GetModCardTag();

    public static readonly CardTag GanJiangMoYe = 
        ModContentRegistry.GetQualifiedCardTagId(
            Entry.ModId, nameof(GanJiangMoYe)).GetModCardTag();

    // public static readonly CardTag Heavy2 = ModContentRegistry.GetQualifiedCardTagId(Entry.ModId, nameof(Heavy2)).GetModCardTag();
}