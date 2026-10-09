using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;
using Sts2EmiyaMod.Scripts;
using STS2RitsuLib.Interop.AutoRegistration;

/// <summary>
/// 白纸格挡：1费，*技能*，获得8点格挡，如果此牌拥有附魔额外获得3点格挡。  
/// 升级11点格挡,额外获得4点格挡。
/// </summary>
[RegisterCard(typeof(EmiyaShirouCardPool))]
public class PaperBlock : EmiyaCardNode
{
    public override bool GainsBlock => true;
    
    /// <summary>
    /// 基础耗能    
    /// </summary>;
    public const int energyCost = 1;

    private const CardType type = CardType.Skill;

    private const CardRarity rarity = CardRarity.Common;

    private const TargetType targetType = TargetType.Self;

    // 是否在卡牌图鉴中显示
    private const bool shouldShowInCardLibrary = true;

    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new BlockVar(8, ValueProp.Move),
        new BlockVar("ExtraBlock",3, ValueProp.Move)
    ];

    public PaperBlock() :
        base(energyCost, type, rarity, targetType, shouldShowInCardLibrary)
    {
        
    }
    

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
	{
        await CreatureCmd.GainBlock(base.Owner.Creature, base.DynamicVars.Block, cardPlay);
        if(this.Enchantment != null)
        {
            await CreatureCmd.GainBlock(base.Owner.Creature, 
                (BlockVar)base.DynamicVars["ExtraBlock"],
                cardPlay);
        }
    }

    protected override void OnUpgrade()
	{
		base.DynamicVars.Block.UpgradeValueBy(3m);
		base.DynamicVars["ExtraBlock"].UpgradeValueBy(1m);
	}
}