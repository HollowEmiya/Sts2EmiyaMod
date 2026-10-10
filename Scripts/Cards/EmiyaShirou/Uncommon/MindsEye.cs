using System.Reflection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Logging;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.MonsterMoves.MonsterMoveStateMachine;
using MegaCrit.Sts2.Core.ValueProps;
using Sts2EmiyaMod.Scripts;
using STS2RitsuLib.Interop.AutoRegistration;

/// <summary>
/// 心眼真：1费，*技能*，如果敌人意图为攻击，获得8点格挡，施加2层虚弱，抽一张牌，非攻击则施加2层易伤，抽1张牌。  
/// 升级获得11点格挡。
/// </summary>
[RegisterCard(typeof(EmiyaShirouCardPool))]
public class MindsEye : EmiyaCardNode
{
    public override bool GainsBlock => true;
    
    /// <summary>
    /// 基础耗能    
    /// </summary>;
    public const int energyCost = 1;

    private const CardType type = CardType.Skill;

    private const CardRarity rarity = CardRarity.Uncommon;

    private const TargetType targetType = TargetType.AnyEnemy;

    // 是否在卡牌图鉴中显示
    private const bool shouldShowInCardLibrary = true;

    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new BlockVar(8, ValueProp.Move),
        new PowerVar<VulnerablePower>(2m),
		new PowerVar<WeakPower>(2m),
		new CardsVar(1)
    ];

    public MindsEye() :
        base(energyCost, type, rarity, targetType, shouldShowInCardLibrary)
    {
        
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        ArgumentNullException.ThrowIfNull(cardPlay.Target, "cardPlay.Target");
        if(cardPlay.Target.IsEnemy && cardPlay.Target.Monster != null
            && cardPlay.Target.Monster.IntendsToAttack)
        {
            await CreatureCmd.GainBlock(base.Owner.Creature,
                base.DynamicVars.Block, cardPlay);
        
            await PowerCmd.Apply<WeakPower>(choiceContext, cardPlay.Target, 
                base.DynamicVars.Weak.BaseValue, base.Owner.Creature, this);
        }
        else
        {
            await PowerCmd.Apply<VulnerablePower>(choiceContext, cardPlay.Target, 
                base.DynamicVars.Vulnerable.BaseValue, base.Owner.Creature, this);
        }
        await CardPileCmd.Draw(choiceContext, base.DynamicVars.Cards.BaseValue, base.Owner);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Block.UpgradeValueBy(3);
    }
}