using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.CardPools;
using MegaCrit.Sts2.Core.ValueProps;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;

namespace Sts2EmiyaMod.Scripts;


// TODO:需要给生成的牌加Tag
[RegisterCard(typeof(ColorlessCardPool))]
public class TraceMagecraft : ModCardTemplate
{
    public override bool GainsBlock => true;
    private const int energyCost = 1;
    private const CardType type = CardType.Skill;
    private const CardRarity rarity = CardRarity.Common;
    private const TargetType targetType = TargetType.Self;
    private const bool shouldShowInCardLibrary = true;

    public override CardAssetProfile AssetProfile => new(
        PortraitPath: $"res://Sts2EmiyaMod/Images/Cards/EmiyaShirou/{GetType().Name}.png"
    );

    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new BlockVar(5, ValueProp.Move)
    ];

    public TraceMagecraft() : base(energyCost, type, rarity, targetType, shouldShowInCardLibrary)
    {
        
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await CreatureCmd.GainBlock(base.Owner.Creature, base.DynamicVars.Block, cardPlay);
        CardModel selection = (await CardSelectCmd.FromHand(prefs: new CardSelectorPrefs(base.SelectionScreenPrompt, 1), context: choiceContext, player: base.Owner, filter: delegate(CardModel c)
		{
			CardType type = c.Type;
			return (type == CardType.Attack || type == CardType.Power || type == CardType.Skill) ? true : false;
		}, source: this)).FirstOrDefault();
        if (selection != null)
        {
            CardModel card = selection.CreateClone();
            ApplyCopiedCardStatReduction(card);
            CardCmd.ApplyKeyword(card, TraceKeyword.Trace);
            card.EnergyCost.AddThisCombat(-1);
            await CardPileCmd.AddGeneratedCardToCombat(card, PileType.Hand, base.Owner);
        }
    }

    private static void ApplyCopiedCardStatReduction(CardModel card)
    {
        if (card.DynamicVars.TryGetValue("Damage", out DynamicVar? damageVar))
        {
            damageVar.BaseValue = Math.Max(0m, damageVar.BaseValue - 2m);
        }

        if (card.DynamicVars.TryGetValue("Block", out DynamicVar? blockVar))
        {
            blockVar.BaseValue = Math.Max(0m, blockVar.BaseValue - 2m);
        }
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Block.UpgradeValueBy(3);
    }
}