using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;

namespace Sts2EmiyaMod.Scripts;

/// <summary>
/// 保留原版计算伤害和附魔预览逻辑，但以 DamageVar 作为基础伤害。
/// </summary>
public class DamageBasedCalculatedDamageVar : CalculatedDamageVar
{
    public DamageBasedCalculatedDamageVar(ValueProp props) : base(props)
    {
    }

    protected override DynamicVar GetBaseVar()
    {
        if (_owner is not CardModel card)
            throw new InvalidOperationException("计算伤害变量必须绑定到卡牌。");
        return card.DynamicVars.Damage;
    }
}
