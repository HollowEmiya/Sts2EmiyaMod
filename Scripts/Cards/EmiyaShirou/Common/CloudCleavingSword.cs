using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;

namespace Sts2EmiyaMod.Scripts;
/// <summary>
/// 云劈剑：1费，攻击，造成9点伤害，从抽牌堆中选择一张牌放到抽牌堆顶。
/// 升级后造成12点伤害。
/// </summary>
[RegisterCard(typeof(EmiyaShirouCardPool))]
public class CloudCleavingSword : EmiyaCardNode
{
    public const int energyCost = 1;

    protected override HashSet<CardTag> CanonicalTags =>
        new HashSet<CardTag> {
            CardTag.Strike,
            EmiyaTags.GanJiangMoYe
        };

    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new DamageVar(9, ValueProp.Move)
    ];

    public CloudCleavingSword() : base(energyCost, CardType.Attack,
        CardRarity.Common, TargetType.AnyEnemy)
    {
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        ArgumentNullException.ThrowIfNull(cardPlay.Target, "cardPlay.Target");
        await DamageCmd.Attack(DynamicVars.Damage.BaseValue)
            .FromCard(this, cardPlay)
            .Targeting(cardPlay.Target)
            .WithHitFx("vfx/vfx_dramatic_stab", null, "blunt_attack.mp3")
            .Execute(choiceContext);

        CardModel? selectedCard = (await CardSelectCmd.FromCombatPile(
            context: choiceContext, pile: PileType.Draw.GetPile(Owner), player: Owner,
            prefs: new CardSelectorPrefs(SelectionScreenPrompt, 1))).FirstOrDefault();
        if (selectedCard != null)
            await CardPileCmd.Add(selectedCard, PileType.Draw, CardPilePosition.Top);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(3m);
    }
}
