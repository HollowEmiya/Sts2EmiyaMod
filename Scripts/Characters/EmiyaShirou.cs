
using Godot;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Models;
using STS2RitsuLib.Scaffolding.Visuals.StateMachine;
using STS2RitsuLib.Scaffolding.Visuals.StateMachine.Backends;
using MegaCrit.Sts2.Core.Entities.Characters;
using MegaCrit.Sts2.Core.Models.Relics;
using MegaCrit.Sts2.Core.Nodes.Combat;
using STS2RitsuLib.Data.Models;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Characters;
using STS2RitsuLib.Scaffolding.Godot;
namespace Sts2EmiyaMod.Scripts;

[RegisterCharacter]
public class EmiyaShirouCharacter : 
    ModCharacterTemplate<EmiyaShirouCardPool,
        EmiyaShirouRelicPool, EmiyaShirouPotionPool>
{
    /// <summary>
    /// 人物名称颜色
    /// #a43c00
    /// </summary>
    public override Color NameColor => new Color("a43c00");

    /// <summary>
    /// 能量图标轮廓颜色
    /// #AD3318
    /// </summary>
    public override Color EnergyLabelOutlineColor => new Color("AD3318");

    /// <summary>
    /// 地图绘制颜色
    /// #222222
    /// </summary>
    public override Color MapDrawingColor => new Color("222222");

    public override CharacterGender Gender => CharacterGender.Masculine;

    /// <summary>
    /// 人物起始血量
    /// </summary>
    public override int StartingHp => 80;

    /// <summary>
    /// 初始金币
    /// </summary>
    public override int StartingGold => 99;

    public override CharacterAssetProfile AssetProfile => CharacterAssetProfiles.Merge(
        CharacterAssetProfiles.Ironclad(),
        new(
            Scenes: new(
                // 人物模型tscn路径。
                VisualsPath:
                    "res://Resources/EmiyaShirou/Scenes/EmiyaShirou.tscn",
                // 能量表盘tscn路径。
                EnergyCounterPath:
                    "res://Resources/EmiyaShirou/Scenes/EmiyaShirou_Energy_Counter.tscn",
                // 商店人物场景。
                MerchantAnimPath: 
                    "res://Resources/EmiyaShirou/Scenes/EmiyaShirou_Merchant.tscn",
                // 篝火休息场景。
                RestSiteAnimPath: 
                    "res://Resources/EmiyaShirou/Scenes/EmiyaShirou_Rest_Site.tscn"
            ),
            Ui: new(
                // 对于图片，只要是godot支持的格式都可以，例如png,jpg,svg等等，之后不再说明
                // 人物头像路径。自适应大小。
                IconTexturePath:
                    "res://Resources/EmiyaShirou/Images/EmiyaShirouIcon.png",
                IconOutlineTexturePath:
                    "res://Resources/EmiyaShirou/Images/EmiyaShirouIconOutline.png",
                // 游戏左上角头像、角色统计页头像、每日挑战角色头像。这个是场景而不是图片。参考下方附赠资源搭建。
                IconPath: 
                    "res://Resources/EmiyaShirou/Scenes/EmiyaShirou_Icon.tscn",
                // 人物选择背景。
                // 人物选择界面背景
                CharacterSelectBgPath:
                    "res://Resources/EmiyaShirou/Scenes/EmiyaShirou_Bg.tscn",
                // 人物选择图标。
                CharacterSelectIconPath: 
                    "res://Resources/EmiyaShirou/Images/EmiyaShirouSelected.png",
                // 人物选择图标-锁定状态。
                CharacterSelectLockedIconPath:
                    "res://Resources/EmiyaShirou/Images/EmiyaShirouSelectedLocked.png",
                // 人物选择过渡动画。
                CharacterSelectTransitionPath:
                    "res://Resources/EmiyaShirou/Scenes/EmiyaShirou_Transition.tres",
                // 地图上的角色标记图标、表情轮盘上的角色头像。
                MapMarkerPath: 
                    "res://Resources/EmiyaShirou/Images/EmiyaShirouIcon.png"
            ),
            Vfx: new(
                // 卡牌拖尾场景。
                // TrailPath: "res://scenes/vfx/card_trail_ironclad.tscn"
            ),
            Audio: new(
                // 攻击音效
                AttackSfx:  
                    "event:/Sts2EmiyaMod/EmiyaShirou/sfx/EmiyaShirouAttack",
                // 施法音效
                CastSfx: 
                    "event:/Sts2EmiyaMod/EmiyaShirou/sfx/EmiyaShirouCast",
                // 死亡音效
                DeathSfx: 
                    "event:/Sts2EmiyaMod/EmiyaShirou/sfx/EmiyaShirouDie",
                // 角色选择音效
                CharacterSelectSfx: 
                    "event:/Sts2EmiyaMod/EmiyaShirou/sfx/EmiyaShirouSelect"
                // 过渡音效
                // CharacterTransitionSfx: "event:/sfx/ui/wipe_ironclad"
            ),
            Multiplayer: new(
                // 多人模式-手指。
                ArmPointingTexturePath: 
                    "res://Resources/EmiyaShirou/Images/EmiyaShirouPoint.png",
                // 多人模式剪刀石头布-石头。
                ArmRockTexturePath: 
                    "res://Resources/EmiyaShirou/Images/EmiyaShirouRock.png",
                // 多人模式剪刀石头布-布。
                ArmPaperTexturePath:
                    "res://Resources/EmiyaShirou/Images/EmiyaShirouPaper.png",
                // 多人模式剪刀石头布-剪刀。
                ArmScissorsTexturePath:
                    "res://Resources/EmiyaShirou/Images/EmiyaShirouScissors.png"
            ),
            // 其余如果有需要自行取消注释使用
            // Spine: null,
            // VisualCues: null, // 帧动画静态图人物使用，查看角色动画一章
            // WorldProceduralVisuals: null,
            // 以下为让遗物根据你的人物展现不同的图像资源，在列表里添加即可
            // VanillaCardVisualOverrides: [],
            VanillaRelicVisualOverrides: [
                new (CharacterOwnedVanillaRelicModelId.YummyCookie, new("Sts2EmiyaMod/Images/Relics/EmiyaShirou/EmiyaShirouCookie.png")) // 美味饼干覆盖
            ]
            // VanillaPotionVisualOverrides: []
        )
    );

    // 攻击和施法动画延迟，以对齐动画
    public override float AttackAnimDelay => 0f;
    public override float CastAnimDelay => 0f;

    // 如果你的人物不需要时间线小故事，加上这句。
    public override bool RequiresEpochAndTimeline => false;

    // 自动转换人物场景，让你不需要手动挂脚本。复制即可。
    protected override NCreatureVisuals? TryCreateCreatureVisuals() =>
         RitsuGodotNodeFactories.CreateFromScenePath<NCreatureVisuals>
            (AssetProfile.Scenes!.VisualsPath!);

    // AnimatedSprite2D 播完一次不会自动切换动画；由状态机安排后继状态。
    protected override ModAnimStateMachine? SetupCustomCombatAnimationStateMachine(
        Node visualsRoot, CharacterModel character)
    {
        var sprite = visualsRoot as AnimatedSprite2D
            ?? visualsRoot.GetNodeOrNull<AnimatedSprite2D>("%Visuals");
        if (sprite == null)
            throw new InvalidOperationException("卫宫战斗场景缺少 AnimatedSprite2D Visuals 节点。");

        var builder = ModAnimStateMachineBuilder.Create()
            .AddState("Idle", loop: true).AsInitial().Done()
            .AddState("Attack").WithNext("Idle").Done()
            .AddState("Cast").WithNext("Idle").Done()
            .AddState("Hit").WithNext("Idle").Done()
            .AddState("Die").Done();

        builder.AddAnyState("Idle", "Idle");
        builder.AddAnyState("Attack", "Attack");
        builder.AddAnyState("Cast", "Cast");
        builder.AddAnyState("Hit", "Hit");
        builder.AddAnyState("Dead", "Die");
        builder.AddAnyState("Relaxed", "Idle");
        // 本体的死亡语音位于 Spine 分支；序列帧人物需要显式接入。
        // AnimationChanged 仅在动画名称改变时触发，避免每一帧重复播放。
        sprite.AnimationChanged += () =>
        {
            if (sprite.Animation == "Die")
                SfxCmd.Play(character.DeathSfx);
        };
        return builder.Build(new AnimatedSprite2DBackend(sprite));
    }
    // 攻击建筑师的攻击特效列表
    public override List<string> GetArchitectAttackVfx() => [
        "vfx/vfx_attack_blunt",
        "vfx/vfx_heavy_blunt",
        "vfx/vfx_attack_slash",
        "vfx/vfx_bloody_impact",
        "vfx/vfx_rock_shatter"
    ];
}