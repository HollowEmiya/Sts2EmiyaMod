using MegaCrit.Sts2.Core.Entities.Cards;
using STS2RitsuLib.Content;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Keywords;

namespace Sts2EmiyaMod.Scripts;

// 原本的关键字都是影响卡牌自己本身的功能。
// 如果有新的关键字继续在这里添加
[RegisterOwnedCardKeyword(nameof(Trace),
    CardDescriptionPlacement = ModKeywordCardDescriptionPlacement.BeforeCardDescription)]
public class EmiyaKeywords
{
    /// <summary>
    /// Trace需要描述,还真得使用关键字
    /// </summary>
    public static readonly CardKeyword Trace = ModContentRegistry.GetQualifiedKeywordId(
        Entry.ModId, nameof(Trace)).GetModCardKeyword();
}