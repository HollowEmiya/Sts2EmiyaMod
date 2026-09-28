using MegaCrit.Sts2.Core.Entities.Cards;
using STS2RitsuLib.Content;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Keywords;

namespace Sts2EmiyaMod.Scripts;

[RegisterOwnedCardKeyword(nameof(Trace),
    CardDescriptionPlacement = ModKeywordCardDescriptionPlacement.BeforeCardDescription)]
public class TraceKeyword
{
    public static readonly CardKeyword Trace = ModContentRegistry.GetQualifiedKeywordId(
        Entry.ModId, nameof(Trace)).GetModCardKeyword();
}