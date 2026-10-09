using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using STS2RitsuLib.Interop.AutoRegistration;

namespace Sts2EmiyaMod.Scripts;

/// <summary>
/// 大扫除：1费技能，选择任意数量的手牌丢弃，然后抽取等量的牌，消耗。
/// 升级后移除消耗。
/// </summary>
[RegisterCard(typeof(EmiyaShirouCardPool))]
public class HouseCleaning : EmiyaCardNode
{
    public const int energyCost = 1;
    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];

    public HouseCleaning() : base(energyCost, CardType.Skill,
        CardRarity.Uncommon, TargetType.Self)
    {
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        var handCount = PileType.Hand.GetPile(Owner).Cards.Count;
        if (handCount == 0)
            return;

        var selectedCards = (await CardSelectCmd.FromHandForDiscard(choiceContext, Owner,
            new CardSelectorPrefs(CardSelectorPrefs.DiscardSelectionPrompt, 0, handCount),
            null, this)).ToList();
        if (selectedCards.Count > 0)
            await CardCmd.DiscardAndDraw(choiceContext, selectedCards, selectedCards.Count);
    }

    protected override void OnUpgrade()
    {
        RemoveKeyword(CardKeyword.Exhaust);
    }
}
