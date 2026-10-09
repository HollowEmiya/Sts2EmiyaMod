using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;
using Sts2EmiyaMod.Scripts;
using STS2RitsuLib.Interop.AutoRegistration;

/// <summary>
/// 剑之壁垒：2费，技能，获得12点格挡，手中每有一张牌获得2点格挡。  
/// 升级获得14点格挡，每有一张获得3点格挡。
/// </summary>
[RegisterCard(typeof(EmiyaShirouCardPool))]
public class SwordBarricade : EmiyaCardNode
{
    public override bool GainsBlock => true;
    
    /// <summary>
    /// 基础耗能    
    /// </summary>;
    public const int energyCost = 2;

    private const CardType type = CardType.Skill;

    private const CardRarity rarity = CardRarity.Common;

    private const TargetType targetType = TargetType.Self;

    // 是否在卡牌图鉴中显示
    private const bool shouldShowInCardLibrary = true;

    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new BlockVar(12, ValueProp.Move),
        new BlockVar("CardBlock", 2, ValueProp.Move),
    ];

    public SwordBarricade() :
        base(energyCost, type, rarity, targetType, shouldShowInCardLibrary)
    {
        
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
	{
        await CreatureCmd.GainBlock(base.Owner.Creature, base.DynamicVars.Block, cardPlay);
        foreach (CardModel item in
            PileType.Hand.GetPile(base.Owner).Cards.ToList())
        {
			await CreatureCmd.GainBlock(
                base.Owner.Creature, (BlockVar)base.DynamicVars["CardBlock"], cardPlay);
        }
    }

    
    protected override void OnUpgrade()
	{
		base.DynamicVars.Block.UpgradeValueBy(2m);
		base.DynamicVars["CardBlock"].UpgradeValueBy(1m);
	}
}