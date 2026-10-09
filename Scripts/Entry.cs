using System.Reflection;
using HarmonyLib;
using MegaCrit.Sts2.Core.Logging;
using MegaCrit.Sts2.Core.Modding;
using MegaCrit.Sts2.Core.Models.Cards;
using STS2RitsuLib;
using STS2RitsuLib.Audio;
using STS2RitsuLib.Interop;

namespace Sts2EmiyaMod.Scripts;

[ModInitializer(nameof(Init))]
public class Entry
{
    // 你的modid
    public const string ModId = "Sts2EmiyaMod";
    public static readonly Logger Logger = RitsuLibFramework.CreateLogger(ModId);

    public static void Init()
    {
        // harmony可用，但是最好用ritsu的封装patch，见补丁系统一章
        // var harmony = new Harmony("com.example.testmod");
        // harmony.PatchAll();
        var assembly = Assembly.GetExecutingAssembly();
        RitsuLibFramework.
            EnsureGodotScriptsRegistered(assembly, Logger);
        // 自动注册内容
        ModTypeDiscoveryHub.
            RegisterModAssembly(ModId, assembly);

        var techniqueHistoryHarmony = new Harmony(ModId + ".TechniqueHistory");
        techniqueHistoryHarmony.CreateClassProcessor(typeof(ReinforcementHistoryPatch)).Patch();
        techniqueHistoryHarmony.CreateClassProcessor(typeof(ProjectionHistoryPatch)).Patch();
        
        RitsuLibFramework.
            RegisterArchaicToothTranscendenceMapping<Kyudo, Hrunting>();

        RitsuLibFramework.
            RegisterTouchOfOrobasRefinementMapping<MagicCircuits, MagicCircuitsEX>();

        FmodStudioDeferredBankRegistration.RegisterBank
        (
            "res://Resources/EmiyaShirou/Audios/EmiyaShirou.bank"
        );

        FmodStudioDeferredBankRegistration.RegisterStudioGuidMappings
        (
            "res://Resources/EmiyaShirou/Audios/GUIDs.txt"
        );
    }
}
