#if !UNITY_EDITOR
using BepInEx;
using BepInEx.Configuration;
using System;
using UnityEngine;
using TMPro;
using EFT;
using EFT.InventoryLogic;
using GamePanelHUDCore;
using GamePanelHUDCore.Utils;
using GamePanelHUDHit.Patches;

namespace GamePanelHUDHit
{
    [BepInPlugin("com.kmyuhkyuk.GamePanelHUDHit", "kmyuhkyuk-GamePanelHUDHit", "2.4.3")]
    [BepInDependency("com.kmyuhkyuk.GamePanelHUDCore")]
    public class GamePanelHUDHitPlugin : BaseUnityPlugin, IUpdate
    {
        private GamePanelHUDCorePlugin.HUDCoreClass HUDCore
        {
            get
            {
                return GamePanelHUDCorePlugin.HUDCore;
            }
        }

        internal static readonly GamePanelHUDCorePlugin.HUDClass<RectTransform, SettingsData> HitHUD = new GamePanelHUDCorePlugin.HUDClass<RectTransform, SettingsData>();

        internal static readonly KillHUDClass<RectTransform, SettingsData> KillHUD = new KillHUDClass<RectTransform, SettingsData>();

        private bool HitHUDSW;

        private bool KillHUDSW;

        private bool ExpHUDSW;

        private readonly SettingsData SettingsDatas = new SettingsData();

        private RectTransform ScreenRect;

        private static readonly string[] TestName = new string[] { "Player", "Me", "My Life", "Water Chip", "Camper", "Profile", "The City", "Escape from Tarkov", "C Disk", "Windows", "Display", "Controller", "Ram", "Cpu", "Wifi", "Phone", "Computer", "Bed", "Air Conditioner", "Discord", "Graphics Card", "Power", "Home", "World", "Sol3", "Moon", "Joker", "Galaxy", "My Heart", "WO_TM_CALL_110", "Kenny", "Abby", "Big Smoke", "Adam Smasher", "Robert Edwin House", "Joseph Seed", "Saburo Arasaka", "Handsome Jack", "Dracula", "Darth Sidious", "Lance Vance", "Dimitri Rascalov", "Mikhail Faustin", "Wei Cheng", "Steve Haines", "Darth Vader", "Devin Weston", "Steven Armstrong", "Aron Keener", "Agent Smith", "Yuri", "Volodymyr Makarov", "Victor Zakhaev", "Tyran", "General Shepherd", "Measurehead", "Cacodemon", "Zombie", "Ghoul", "Cyberpsychosis", "Trauma Team", "Max Tac", "Whirlpool Gang", "Golden Path", "Game Panel HUD", "Game Program", "Aki Server", "Bsg Server", "TypeScript", "NoBody", "AnyOne", "AllText", "TheTestText", "Who?", "Unity", "VSCode", "VisualStudio", "OtherName" };

        private static readonly string[] TestWeaponName = new string[] { "F-1", "M18", "Zarya", "MPL-50", "C100", "SP-81", "AS-VAL", "MPX", "Vector .45ACP", "M4A1", "HK-416", "AR-15", "AK-104", "SV-98", "ST-AR-15", "MCX", "MDR", "FN40GL", "MSGL", "P90", "ASh-12", "Glock 18C", "USP .45 Expert", "MP5", "M1911", "MK-14", "AK-47", "AS-50", "AR-57", "MAG-7", "Mosin", "M1903", "Gew98", "Ro-635", "UMP 9", "M16", "G11", "UMP 45", "AK-12", "RPG-7V2", "AEK-971", "L115A3", "CS/LR-4", "L96A1", "M200", "R90", "725", "AA-12", "Guts", "BFG-9000", "R8", "AWP", "DesertEagle", "Gravity Gun", "Portal Gun", "Freeze Gun", "Sentry Gun", "Hand", "M42-MIRV", "Martini-Henry", "CAR", "R-101", "R-97", "Alternator", "Charge Rifle", "Alligator", "Disturbia", "B3 Wingman Elite", "Smart Pistol MK5", "GM6", "Kolibri", "Cow Gun", "Shark Gun", "Test Gun", "MailBox", "Mantis Blades", "Mono Wire", "Gorilla Arms", "Malorian Arms 3516", "Sandevistan", "Cyber Skeleton", "Militech Basilisk", "Militech Behemoth", "Floating Car", "Light Rail", "Soulkiller", "Water", "Short Circuit", "Giant Shark", "Velociraptor", "Air", "Fall", "BTR-80", "Rorsch Mk-1", "Rebel", "Hypoxia", "99A", "A-10", "F-35", "AC-130", "Tsar Bomba", "Recycle Bin", "Joy", "Elevator", "Tactical Nuclear", "Task Manager", "Black Hole", "Time", "Space Radiation", "Ender Dragon", "Creeper", "Wooden Sword", "TNT", "Void", "Truncheon", "Recurve Bow", "Burn", "Sun", "Light", "Cybertruck", "Tornado", "Liquid Nitrogen", "KeyBoard", "Unicorn", "Talk", "Anim", "Auto-9", "Mercury Hg", "Formaldehyde", "Gamma Ray", "X-ray", "Snake", "Bug", "Code", "Blaster", "Mouth", "Money", "Work", "Armored Train", "Rainbow Cat", "Butter Cat", "Medkit", "Pacemaker", "Blowtorch", "Ambulance", "Bus", "Game Leak", "Pencil", "Lightning", "Skill Issue", "C4", "M134", "Dance", "GP-30", "M79", "Big Sally", "Neuralyzer", "Noisy Cricket", "Death Star", "Star Destroyer", "Millennium Falcon", "42", "Gatling Laser", "Supernova Explosion", "Bathtub", "Gas Tank", "Game", "Video", "UFO", "Lava", "Other Gun" };

        internal static GameObject KillPrefab { get; private set; }

        internal static readonly ArmorInfo Armor = new ArmorInfo();

        internal static int Kills;

        internal static Action<HitInfo> ShowHit;

        internal static Action<KillInfo> ShowKill;

        private void Start()
        {
            Logger.LogInfo("Loaded: kmyuhkyuk-GamePanelHUDHit");

            ModUpdateCheck.DrawNeedUpdate(Config, Info.Metadata.Version);

            const string mainSettings = "主设置 Main Settings";
            const string prsSettings = "位置旋转大小设置 Position Rotation Scale Settings";
            const string hitColorSettings = "击中颜色设置 Hit Color Settings";
            const string killColorSettings = "击中颜色设置 Kill Color Settings";
            const string fontStylesSettings = "字体样式设置 Font Styles Settings";
            const string speedSettings = "动画速度设置 Animation Speed Settings";
            const string distanceSettings = "距离设置 Distance Settings";
            const string directionRateSettings = "方向率设置 Direction Rate Settings";

            Config.Bind(mainSettings, "Test Hit", "", new ConfigDescription("", null, new ConfigurationManagerAttributes { Order = 2, HideDefaultButton = true, CustomDrawer = TestHit, HideSettingName = true }));

            Config.Bind(mainSettings, "Test Kill", "", new ConfigDescription("", null, new ConfigurationManagerAttributes { Order = 1, HideDefaultButton = true, CustomDrawer = TestKill, HideSettingName = true }));

            SettingsDatas.KeyHitHUDSW = Config.Bind<bool>(mainSettings, "命中显示 Hit HUD display", true);
            SettingsDatas.KeyHitDamageHUDSW = Config.Bind<bool>(mainSettings, "命中伤害显示 Hit Damage HUD display", false);
            SettingsDatas.KeyHitHasHead = Config.Bind<bool>(mainSettings, "命中头部单独颜色 Hit Head Separate Color", false);
            SettingsDatas.KeyHitHasDirection = Config.Bind<bool>(mainSettings, "命中方向 Hit Direction", true);
            SettingsDatas.KeyKillHUDSW = Config.Bind<bool>(mainSettings, "击杀信息显示 Kill HUD display", true);
            SettingsDatas.KeyKillWaitBottom = Config.Bind<bool>(mainSettings, "等待底部击杀销毁 Wait Bottom Kill Destroy", true);
            SettingsDatas.KeyKillHasDistance = Config.Bind<bool>(mainSettings, "击杀距离显示 Kill Distance display", true);
            SettingsDatas.KeyKillHasOther = Config.Bind<bool>(mainSettings, "击杀其他显示 Kill Other display", true);
            SettingsDatas.KeyKillHasStreak = Config.Bind<bool>(mainSettings, "击杀连杀显示 Kill Streak display", true);
            SettingsDatas.KeyKillHasXp = Config.Bind<bool>(mainSettings, "击杀经验显示 Kill Xp display", true);
            SettingsDatas.KeyKillHasLevel = Config.Bind<bool>(mainSettings, "击杀等级显示 Kill Level display", false);
            SettingsDatas.KeyKillHasPart = Config.Bind<bool>(mainSettings, "击杀部位显示 Kill Part display", false);
            SettingsDatas.KeyKillHasSide = Config.Bind<bool>(mainSettings, "击杀派系显示 Kill Faction display", false);
            SettingsDatas.KeyKillHasWeapon = Config.Bind<bool>(mainSettings, "击杀武器显示 Kill Weapon display", true);
            SettingsDatas.KeyKillScavEn = Config.Bind<bool>(mainSettings, "击杀Scav名字转英文 Kill Scav Name To En", true);
            SettingsDatas.KeyExpHUDSW = Config.Bind<bool>(mainSettings, "经验值显示 Exp HUD display", true);

            SettingsDatas.KeyHitAnchoredPosition = Config.Bind<Vector2>(prsSettings, "命中位置 Hit Anchored Position", new Vector2(12, 12));
            SettingsDatas.KeyHitSizeDelta = Config.Bind<Vector2>(prsSettings, "命中高度 Hit Size Delta", new Vector2(3, 12));
            SettingsDatas.KeyHitLocalScale = Config.Bind<Vector2>(prsSettings, "命中大小 Hit Local Scale", new Vector2(1, 1));
            SettingsDatas.KeyHitHeadSizeDelta = Config.Bind<Vector2>(prsSettings, "命中头部高度 Hit Head Size Delta", new Vector2(3, 16));
            SettingsDatas.KeyHitDamageAnchoredPosition = Config.Bind<Vector2>(prsSettings, "命中伤害位置 Hit Damage Anchored Position", new Vector2(30, 0));
            SettingsDatas.KeyHitDamageSizeDelta = Config.Bind<Vector2>(prsSettings, "命中伤害高度 Hit Damage Size Delta", new Vector2(60, 34));
            SettingsDatas.KeyHitDamageLocalScale = Config.Bind<Vector2>(prsSettings, "命中伤害大小 Hit Damage Local Scale", new Vector2(1, 1));
            SettingsDatas.KeyKillAnchoredPosition = Config.Bind<Vector2>(prsSettings, "击杀位置 Kill Anchored Position", new Vector2(80, -110));
            SettingsDatas.KeyKillSizeDelta = Config.Bind<Vector2>(prsSettings, "击杀高度 Kill Size Delta", new Vector2(500, 180));
            SettingsDatas.KeyKillLocalScale = Config.Bind<Vector2>(prsSettings, "击杀大小 Kill Local Scale", new Vector2(1, 1));
            SettingsDatas.KeyKillExpAnchoredPosition = Config.Bind<Vector2>(prsSettings, "经验值位置 Exp Anchored Position", new Vector2(100, -105));
            SettingsDatas.KeyKillExpSizeDelta = Config.Bind<Vector2>(prsSettings, "经验值高度 Exp Size Delta", new Vector2(80, 30));
            SettingsDatas.KeyKillExpLocalScale = Config.Bind<Vector2>(prsSettings, "经验值大小 Exp Local Scale", new Vector2(1, 1));

            SettingsDatas.KeyHitLocalRotation = Config.Bind<Vector3>(prsSettings, "命中旋转 Hit Rotation", new Vector3(0, 0, 45));

            SettingsDatas.KeyHitDirectionLeft = Config.Bind<float>(directionRateSettings, "命中左侧 Hit Left", -0.5f, new ConfigDescription("When Hit Direction < -0.5 Active Hit Left", new AcceptableValueRange<float>(-1, 1)));
            SettingsDatas.KeyHitDirectionRight = Config.Bind<float>(directionRateSettings, "命中右侧 Hit Right", 0.5f, new ConfigDescription("When Hit Direction > 0.5 Active Hit Right", new AcceptableValueRange<float>(-1, 1)));

            SettingsDatas.KeyHitActiveSpeed = Config.Bind<float>(speedSettings, "命中激活速度 Hit Active Speed", 1, new ConfigDescription("", new AcceptableValueRange<float>(0, 10)));
            SettingsDatas.KeyHitEndSpeed = Config.Bind<float>(speedSettings, "命中结束等待速度 Hit End Wait Speed", 1, new ConfigDescription("", new AcceptableValueRange<float>(0, 10)));
            SettingsDatas.KeyHitDeadSpeed = Config.Bind<float>(speedSettings, "命中死亡等待速度 Hit Dead Wait Speed", 1, new ConfigDescription("", new AcceptableValueRange<float>(0, 10)));
            SettingsDatas.KeyKillWaitSpeed = Config.Bind<float>(speedSettings, "击杀等待速度 Kill Text Wait Speed", 1, new ConfigDescription("", new AcceptableValueRange<float>(0, 10)));
            SettingsDatas.KeyExpWaitSpeed = Config.Bind<float>(speedSettings, "经验值等待速度 Exp Wait Speed", 1, new ConfigDescription("", new AcceptableValueRange<float>(0, 10)));

            SettingsDatas.KeyKillWriteSpeed = Config.Bind<int>(speedSettings, "击杀文本显示速度 Kill Text display Speed", 10, new ConfigDescription("Single character input speed (unit ms)", new AcceptableValueRange<int>(0, 1000)));
            SettingsDatas.KeyKillWrite2Speed = Config.Bind<int>(speedSettings, "击杀文本显示速度 Kill Text 2 display Speed", 10, new ConfigDescription("Single character input speed (unit ms)", new AcceptableValueRange<int>(0, 1000)));
            SettingsDatas.KeyKillWaitTime = Config.Bind<int>(speedSettings, "击杀文本显示速度 Kill Text 2 Wait Time", 500, new ConfigDescription("Play Text to Text2 ago (unit ms)", new AcceptableValueRange<int>(0, 1000)));

            SettingsDatas.KeyKillDistance = Config.Bind<float>(distanceSettings, "击杀距离显示 Kill Distance display", 50, new ConfigDescription("Kill distance >= How many meters display", new AcceptableValueRange<float>(0, 1000)));

            SettingsDatas.KeyHitDamageStyles = Config.Bind<FontStyles>(fontStylesSettings, "伤害 Damage", FontStyles.Normal);
            SettingsDatas.KeyHitArmorDamageStyles = Config.Bind<FontStyles>(fontStylesSettings, "护甲伤害 Armor Damage", FontStyles.Normal);
            SettingsDatas.KeyKillInfoStyles = Config.Bind<FontStyles>(fontStylesSettings, "击杀信息 Kill Info", FontStyles.Bold);
            SettingsDatas.KeyKillDistanceStyles = Config.Bind<FontStyles>(fontStylesSettings, "击杀距离 Kill Distance", FontStyles.Bold);
            SettingsDatas.KeyKillStreakStyles = Config.Bind<FontStyles>(fontStylesSettings, "击杀连杀 Kill Streak", FontStyles.Normal);
            SettingsDatas.KeyKillOtherStyles = Config.Bind<FontStyles>(fontStylesSettings, "击杀其他 Kill Other", FontStyles.Normal);
            SettingsDatas.KeyKillXpStyles = Config.Bind<FontStyles>(fontStylesSettings, "击杀经验 Kill Xp", FontStyles.Normal);
            SettingsDatas.KeyExpStyles = Config.Bind<FontStyles>(fontStylesSettings, "经验值 Exp", FontStyles.Normal);

            SettingsDatas.KeyHitDamageColor = Config.Bind<Color>(hitColorSettings, "伤害 Damage", new Color(1f, 1f, 1f));
            SettingsDatas.KeyHitArmorDamageColor = Config.Bind<Color>(hitColorSettings, "护甲伤害 Armor Damage", new Color(0, 0.5f, 0.8f));
            SettingsDatas.KeyHitDeadColor = Config.Bind<Color>(hitColorSettings, "死亡 Dead", new Color(1f, 0f, 0f));
            SettingsDatas.KeyHitHeadColor = Config.Bind<Color>(hitColorSettings, "头部 Head", new Color(1f, 0.3f, 0f));
            SettingsDatas.KeyHitDamageInfoColor = Config.Bind<Color>(hitColorSettings, "伤害信息 Damage Info", new Color(1f, 0f, 0f));
            SettingsDatas.KeyHitArmorDamageInfoColor = Config.Bind<Color>(hitColorSettings, "护甲伤害信息 Armor Damage Info", new Color(0, 0.5f, 0.8f));

            SettingsDatas.KeyKillNameColor = Config.Bind<Color>(killColorSettings, "名字 Name", new Color(1f, 0.2f, 0.2f));
            SettingsDatas.KeyKillWeaponColor = Config.Bind<Color>(killColorSettings, "武器 Weapon", new Color(0.8901961f, 0.8901961f, 0.8392157f));
            SettingsDatas.KeyKillDistanceColor = Config.Bind<Color>(killColorSettings, "距离 Distance", new Color(1f, 1f, 0f));
            SettingsDatas.KeyKillLevelColor = Config.Bind<Color>(killColorSettings, "等级 Level", new Color(0.8901961f, 0.8901961f, 0.8392157f));
            SettingsDatas.KeyKillStreakColor = Config.Bind<Color>(killColorSettings, "连杀 Streak", new Color(1f, 1f, 0f));
            SettingsDatas.KeyKillPartColor = Config.Bind<Color>(killColorSettings, "身体部位 Body Part", new Color(0.6039216f, 0.827451f, 0.1372549f));
            SettingsDatas.KeyKillMetersColor = Config.Bind<Color>(killColorSettings, "米 Meters", new Color(0.8901961f, 0.8901961f, 0.8392157f));
            SettingsDatas.KeyKillLvlColor = Config.Bind<Color>(killColorSettings, "等级 Lvl", new Color(0.8901961f, 0.8901961f, 0.8392157f));
            SettingsDatas.KeyKillXpColor = Config.Bind<Color>(killColorSettings, "经验值 Xp", new Color(0.8901961f, 0.8901961f, 0.8392157f));
            SettingsDatas.KeyKillStatsStreakColor = Config.Bind<Color>(killColorSettings, "连杀奖励 Stats Streak", new Color(0.8901961f, 0.8901961f, 0.8392157f));
            SettingsDatas.KeyKillOtherColor = Config.Bind<Color>(killColorSettings, "其他 Other", new Color(0.8901961f, 0.8901961f, 0.8392157f));
            SettingsDatas.KeyKillEnemyDownColor = Config.Bind<Color>(killColorSettings, "敌人倒下 Enemy Down", new Color(0.8901961f, 0.8901961f, 0.8392157f));
            SettingsDatas.KeyKillUsecColor = Config.Bind<Color>(killColorSettings, "击杀者 Kill Usec", new Color(0f, 0.8f, 1f));
            SettingsDatas.KeyKillBearColor = Config.Bind<Color>(killColorSettings, "击杀者 Kill Bear", new Color(1f, 0.5f, 0f));
            SettingsDatas.KeyKillScavColor = Config.Bind<Color>(killColorSettings, "击杀者 Kill Scav", new Color(1f, 0.8f, 0f));
            SettingsDatas.KeyKillBossColor = Config.Bind<Color>(killColorSettings, "击杀者 Kill Boss", new Color(1f, 0f, 0f));
            SettingsDatas.KeyKillFollowerColor = Config.Bind<Color>(killColorSettings, "击杀者 Kill Follower", new Color(0.8f, 0f, 0f));
            SettingsDatas.KeyKillBracketColor = Config.Bind<Color>(killColorSettings, "括号 Bracket", new Color(0.8901961f, 0.8901961f, 0.8392157f));
            SettingsDatas.KeyExpColor = Config.Bind<Color>(killColorSettings, "经验值 Exp", new Color(0.8901961f, 0.8901961f, 0.8392157f));

            new ApplyDamagePatch().Enable();
            new ApplyDurabilityDamagePatch().Enable();
            new PlayerApplyDamageInfoPatch().Enable();
            new PlayerKillPatch().Enable();

            GamePanelHUDCorePlugin.UpdateManger.Register(this);
        }

        private void Awake()
        {
            BundleHelp.AssetData<GameObject> prefabs = GamePanelHUDCorePlugin.HUDCoreClass.LoadHUD("gamepanlhithud.bundle", "gamepanlhithud");

            KillPrefab = prefabs.Asset["kill"];

            ScreenRect = GamePanelHUDCorePlugin.HUDCoreClass.GamePanlHUDPublic.GetComponent<RectTransform>();
        }

        public void IUpdate()
        {
            HitPlugin();
        }

        void HitPlugin()
        {
            HitHUDSW = HUDCore.AllHUDSW && HUDCore.HasPlayer && SettingsDatas.KeyHitHUDSW.Value;
            KillHUDSW = HUDCore.AllHUDSW && HUDCore.HasPlayer && SettingsDatas.KeyKillHUDSW.Value;
            ExpHUDSW = HUDCore.AllHUDSW && HUDCore.HasPlayer && SettingsDatas.KeyExpHUDSW.Value;

            HitHUD.Set(ScreenRect, SettingsDatas, HitHUDSW);
            KillHUD.Set(ScreenRect, SettingsDatas, KillHUDSW, ExpHUDSW, SettingsDatas.KeyExpHUDSW.Value);

            if (!HUDCore.HasPlayer)
            {
                Armor.Rest();
                Kills = 0;
            }
        }

        void TestHit(ConfigEntryBase entry)
        {
            if (GUILayout.Button("Test Hit", GUILayout.ExpandWidth(true)))
            {
                var hitType = Enum.GetValues(typeof(HitInfo.Hit));

                var part = Enum.GetValues(typeof(EBodyPart));

                var type = (HitInfo.Hit)hitType.GetValue(UnityEngine.Random.Range(0, hitType.Length));

                var hitInfo = new HitInfo()
                {
                    Damage = UnityEngine.Random.Range(0, 101),
                    ArmorDamage = UnityEngine.Random.Range(0f, 100f),
                    HasArmorHit = type == HitInfo.Hit.HasArmorHit,
                    DamagePart = (EBodyPart)part.GetValue(UnityEngine.Random.Range(0, part.Length)),
                    HitType = type,
                    HitDirection = new Vector3(UnityEngine.Random.Range(-1f, 1f), 0, 0),
                    HitPoint = Vector3.zero,
                    IsTest = true
                };

                ShowHit(hitInfo);
            }
        }

        void TestKill(ConfigEntryBase entry)
        {
            if (GUILayout.Button("Test Kill", GUILayout.ExpandWidth(true)) && ExperienceHelp.CanWork)
            {
                var role = Enum.GetValues(typeof(WildSpawnType));

                var side = Enum.GetValues(typeof(EPlayerSide));

                var part = Enum.GetValues(typeof(EBodyPart));

                var killInfo = new KillInfo()
                {
                    WeaponName = TestWeaponName[UnityEngine.Random.Range(0, TestWeaponName.Length)],
                    PlayerName = TestName[UnityEngine.Random.Range(0, TestName.Length)],
                    Role = (WildSpawnType)role.GetValue(UnityEngine.Random.Range(0, role.Length)),
                    Distance = UnityEngine.Random.Range(0f, 100f),
                    Level = UnityEngine.Random.Range(1, 79),
                    Exp = UnityEngine.Random.Range(100, 1001),
                    Kills = UnityEngine.Random.Range(0, 11),
                    Part = (EBodyPart)part.GetValue(UnityEngine.Random.Range(0, part.Length)),
                    Side = (EPlayerSide)side.GetValue(UnityEngine.Random.Range(0, side.Length)),
                    IsTest = true
                };

                ShowKill(killInfo);
            }
        }

        public class KillHUDClass<T, V> : GamePanelHUDCorePlugin.HUDClass<T, V>
        {
            public bool HUDSW2;

            public bool HUDSW3;

            public void Set(T info, V SettingsDatas, bool hudsw, bool hudsw2, bool hudsw3)
            {
                Set(info, SettingsDatas, hudsw);

                HUDSW2 = hudsw2;
                HUDSW3 = hudsw3;
            }
        }

        public struct HitInfo
        {
            public float Damage;

            public EBodyPart DamagePart;

            public Vector3 HitPoint;

            public Vector3 HitDirection;

            public Hit HitType;

            public bool HasArmorHit;

            public float ArmorDamage;

            public bool IsTest;

            public enum Hit
            {
                OnlyHp,
                HasArmorHit,
                Dead,
                Head
            }

            public enum Direction
            {
                Center,
                Left,
                Right
            }
        }

        public struct KillInfo
        {
            public string PlayerName;

            public string WeaponName;

            public float Distance;

            public int Level;

            public int Exp;

            public int Kills;

            public EPlayerSide Side;

            public EBodyPart Part;

            public WildSpawnType Role;

            public bool IsTest;
        }

        public class ArmorInfo
        {
            public ArmorComponent Component;

            public float ArmorDamage;

            public bool Activa;

            public void SetComponent(ArmorComponent component)
            {
                Component = component;
            }

            public void SetActiva(float damage)
            {
                ArmorDamage = damage;
                Activa = true;
            }

            public void Rest()
            {
                Component = null;
                ArmorDamage = 0;
                Activa = false;
            }
        }

        public class SettingsData
        {
            public ConfigEntry<bool> KeyHitHUDSW;
            public ConfigEntry<bool> KeyHitDamageHUDSW;
            public ConfigEntry<bool> KeyHitHasHead;
            public ConfigEntry<bool> KeyHitHasDirection;
            public ConfigEntry<bool> KeyKillHUDSW;
            public ConfigEntry<bool> KeyKillWaitBottom;
            public ConfigEntry<bool> KeyKillHasDistance;
            public ConfigEntry<bool> KeyKillHasStreak;
            public ConfigEntry<bool> KeyKillHasOther;
            public ConfigEntry<bool> KeyKillHasXp;
            public ConfigEntry<bool> KeyKillHasLevel;
            public ConfigEntry<bool> KeyKillHasPart;
            public ConfigEntry<bool> KeyKillHasWeapon;
            public ConfigEntry<bool> KeyKillHasSide;
            public ConfigEntry<bool> KeyKillScavEn;
            public ConfigEntry<bool> KeyExpHUDSW;

            public ConfigEntry<Vector2> KeyHitAnchoredPosition;
            public ConfigEntry<Vector2> KeyHitSizeDelta;
            public ConfigEntry<Vector2> KeyHitLocalScale;
            public ConfigEntry<Vector2> KeyHitHeadSizeDelta;
            public ConfigEntry<Vector2> KeyHitDamageAnchoredPosition;
            public ConfigEntry<Vector2> KeyHitDamageSizeDelta;
            public ConfigEntry<Vector2> KeyHitDamageLocalScale;
            public ConfigEntry<Vector2> KeyKillAnchoredPosition;
            public ConfigEntry<Vector2> KeyKillSizeDelta;
            public ConfigEntry<Vector2> KeyKillLocalScale;
            public ConfigEntry<Vector2> KeyKillExpAnchoredPosition;
            public ConfigEntry<Vector2> KeyKillExpSizeDelta;
            public ConfigEntry<Vector2> KeyKillExpLocalScale;

            public ConfigEntry<Vector3> KeyHitLocalRotation;

            public ConfigEntry<int> KeyKillWriteSpeed;
            public ConfigEntry<int> KeyKillWrite2Speed;
            public ConfigEntry<int> KeyKillWaitTime;

            public ConfigEntry<float> KeyHitDirectionLeft;
            public ConfigEntry<float> KeyHitDirectionRight;

            public ConfigEntry<float> KeyHitActiveSpeed;
            public ConfigEntry<float> KeyHitEndSpeed;
            public ConfigEntry<float> KeyHitDeadSpeed;
            public ConfigEntry<float> KeyKillDistance;
            public ConfigEntry<float> KeyKillWaitSpeed;
            public ConfigEntry<float> KeyExpWaitSpeed;

            public ConfigEntry<Color> KeyHitDamageColor;
            public ConfigEntry<Color> KeyHitArmorDamageColor;
            public ConfigEntry<Color> KeyHitDeadColor;
            public ConfigEntry<Color> KeyHitHeadColor;
            public ConfigEntry<Color> KeyHitDamageInfoColor;
            public ConfigEntry<Color> KeyHitArmorDamageInfoColor;
            public ConfigEntry<Color> KeyKillNameColor;
            public ConfigEntry<Color> KeyKillWeaponColor;
            public ConfigEntry<Color> KeyKillDistanceColor;
            public ConfigEntry<Color> KeyKillLevelColor;
            public ConfigEntry<Color> KeyKillXpColor;
            public ConfigEntry<Color> KeyKillPartColor;
            public ConfigEntry<Color> KeyKillMetersColor;
            public ConfigEntry<Color> KeyKillStreakColor;
            public ConfigEntry<Color> KeyKillLvlColor;
            public ConfigEntry<Color> KeyKillStatsStreakColor;
            public ConfigEntry<Color> KeyKillOtherColor;
            public ConfigEntry<Color> KeyKillEnemyDownColor;
            public ConfigEntry<Color> KeyKillBearColor;
            public ConfigEntry<Color> KeyKillUsecColor;
            public ConfigEntry<Color> KeyKillScavColor;
            public ConfigEntry<Color> KeyKillBossColor;
            public ConfigEntry<Color> KeyKillFollowerColor;
            public ConfigEntry<Color> KeyKillBracketColor;
            public ConfigEntry<Color> KeyExpColor;

            public ConfigEntry<FontStyles> KeyHitDamageStyles;
            public ConfigEntry<FontStyles> KeyHitArmorDamageStyles;
            public ConfigEntry<FontStyles> KeyKillInfoStyles;
            public ConfigEntry<FontStyles> KeyKillDistanceStyles;
            public ConfigEntry<FontStyles> KeyKillStreakStyles;
            public ConfigEntry<FontStyles> KeyKillOtherStyles;
            public ConfigEntry<FontStyles> KeyKillXpStyles;
            public ConfigEntry<FontStyles> KeyExpStyles;
        }
    }
}
#endif
