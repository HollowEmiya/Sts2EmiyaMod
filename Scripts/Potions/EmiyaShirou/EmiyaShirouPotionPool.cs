using STS2RitsuLib.Scaffolding.Content;

namespace Sts2EmiyaMod.Scripts;

public class EmiyaShirouPotionPool : TypeListPotionPoolModel
{
    public override string? TextEnergyIconPath =>
        "res://Sts2EmiyaMod/Images/Cards/Energy/EmiyaShirou/SmallEnergy.png";

    public override string? BigEnergyIconPath =>
        "res://Sts2EmiyaMod/Images/Cards/Energy/EmiyaShirou/BigEnergy.png";

    public override string EnergyColorName => "EmiyaShirouEnergyColor";
}