using Godot;
using STS2RitsuLib.Scaffolding.Characters;
using STS2RitsuLib.Scaffolding.Content;
using STS2RitsuLib.Utils;

namespace Sts2EmiyaMod.Scripts;

public class EmiyaShirouCardPool : TypeListCardPoolModel, IModColorfulPhilosophersCardPool
{
    /// <summary>
    /// 卡池的ID，唯一
    /// </summary>
    public override string Title => "EmiyaShirouCardPool";
    public override string EnergyColorName =>
        "EmiyaShirouEnergyColor";

    /// <summary>
    /// 描述中使用的能量图标，size 24x24
    /// </summary>
    public override string? TextEnergyIconPath =>
        "res://Sts2EmiyaMod/Images/Energy/EmiyaShirou/SmallEnergy.png";

    /// <summary>
    /// 74x74
    /// </summary>
    public override string? BigEnergyIconPath =>
        "res://Sts2EmiyaMod/Images/Energy/EmiyaShirou/BigEnergy.png";

    /// <summary>
    /// 卡池主题颜色
    /// </summary>
    /// #ff3300
    public override Color DeckEntryCardColor => new Color("FF3300");

    /// <summary>
    /// 能量盘文字轮廓颜色
    /// #860000
    /// </summary>
    public override Color EnergyOutlineColor => new Color("860000");

    
    // 根据你使用的卡框决定使用哪个Material
    private static readonly Material? _poolFrameMaterial =
        MaterialUtils.CreateReplaceHueShaderMaterial(0.5f, 0.5f, 1f);
         // 如果你使用原版卡框，使用这个直接替换色调。
    
    // private static readonly Material? _poolFrameMaterial =
    //  MaterialUtils.CreateRgbShaderMaterial(0.5f, 0.5f, 1f); 
    // 使用原版卡框替换色调。
    // 除非你的版本没有CreateReplaceHueShaderMaterial函数，否则应使用上面那种
    
    // private static readonly Material? _poolFrameMaterial =
    // MaterialUtils.CreateUnmodulatedHsvShaderMaterial(); 
    // 如果你是自定义卡框，使用这个
    
    public override Material? PoolFrameMaterial => _poolFrameMaterial;
    
    // 卡池是否是无色。例如事件、状态等卡池就是无色的。
    public override bool IsColorless => false;
}