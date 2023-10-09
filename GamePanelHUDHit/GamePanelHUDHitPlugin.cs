#if !UNITY_EDITOR

using System;
using BepInEx;
using BepInEx.Configuration;
using EFT;
using EFTUtils;
using GamePanelHUDCore;
using GamePanelHUDCore.Attributes;
using HarmonyLib;
using TMPro;
using UnityEngine;
using static EFTApi.EFTHelpers;

namespace GamePanelHUDHit
{
    [BepInPlugin("com.kmyuhkyuk.GamePanelHUDHit", "kmyuhkyuk-GamePanelHUDHit", "2.7.7")]
    [BepInDependency("com.kmyuhkyuk.GamePanelHUDCore", "2.7.7")]
    [EFTConfigurationPluginAttributes("https://hub.sp-tarkov.com/files/file/652-game-panel-hud", "localized/hit")]
    public partial class GamePanelHUDHitPlugin : BaseUnityPlugin, IUpdate
    {
        private static GamePanelHUDCorePlugin.HUDCoreClass HUDCore => GamePanelHUDCorePlugin.HUDCore;

        internal static readonly GamePanelHUDCorePlugin.HUDClass<RectTransform, SettingsData> HitHUD =
            new GamePanelHUDCorePlugin.HUDClass<RectTransform, SettingsData>();

        internal static readonly KillHUDClass<RectTransform, SettingsData> KillHUD =
            new KillHUDClass<RectTransform, SettingsData>();

        private bool _hitHUDSw;

        private bool _killHUDSw;

        private bool _expHUDSw;

        private readonly SettingsData _setData;

        private static readonly ArmorInfo Armor = new ArmorInfo();

        private RectTransform _screenRect;

        private static readonly string[] TestName =
        {
            "Player", "Me", "My Life", "Water Chip", "Camper", "Profile", "The City", "Escape from Tarkov", "C Disk",
            "Windows", "Display", "Controller", "Ram", "Cpu", "Wifi", "Phone", "Computer", "Bed", "Air Conditioner",
            "Discord", "Graphics Card", "Power", "Home", "World", "Sol3", "Moon", "Joker", "Galaxy", "My Heart",
            "WO_TM_CALL_110", "Kenny", "Abby", "Big Smoke", "Adam Smasher", "Robert Edwin House", "Joseph Seed",
            "Saburo Arasaka", "Handsome Jack", "Dracula", "Darth Sidious", "Lance Vance", "Dimitri Rascalov",
            "Mikhail Faustin", "Wei Cheng", "Steve Haines", "Darth Vader", "Devin Weston", "Steven Armstrong",
            "Aron Keener", "Agent Smith", "Yuri", "Volodymyr Makarov", "Victor Zakhaev", "Tyran", "General Shepherd",
            "Measurehead", "Cacodemon", "Zombie", "Ghoul", "Cyberpsychosis", "Trauma Team", "Max Tac", "Whirlpool Gang",
            "Golden Path", "Game Panel HUD", "Game Program", "Aki Server", "Bsg Server", "TypeScript", "NoBody",
            "AnyOne", "AllText", "TheTestText", "Who?", "Unity", "VSCode", "VisualStudio", "OtherName"
        };

        private static readonly string[] TestWeaponName =
        {
            "F-1", "M18", "Zarya", "MPL-50", "C100", "SP-81", "AS-VAL", "MPX", "Vector .45ACP", "M4A1", "HK-416",
            "AR-15", "AK-104", "SV-98", "ST-AR-15", "MCX", "MDR", "FN40GL", "MSGL", "P90", "ASh-12", "Glock 18C",
            "USP .45 Expert", "MP5", "M1911", "MK-14", "AK-47", "AS-50", "AR-57", "MAG-7", "Mosin", "M1903", "Gew98",
            "Ro-635", "UMP 9", "M16", "G11", "UMP 45", "AK-12", "RPG-7V2", "AEK-971", "L115A3", "CS/LR-4", "L96A1",
            "M200", "R90", "725", "AA-12", "Guts", "BFG-9000", "R8", "AWP", "DesertEagle", "Gravity Gun", "Portal Gun",
            "Freeze Gun", "Sentry Gun", "Hand", "M42-MIRV", "Martini-Henry", "CAR", "R-101", "R-97", "Alternator",
            "Charge Rifle", "Alligator", "Disturbia", "B3 Wingman Elite", "Smart Pistol MK5", "GM6", "Kolibri",
            "Cow Gun", "Shark Gun", "Test Gun", "MailBox", "Mantis Blades", "Mono Wire", "Gorilla Arms",
            "Malorian Arms 3516", "Sandevistan", "Cyber Skeleton", "Militech Basilisk", "Militech Behemoth",
            "Floating Car", "Light Rail", "Soulkiller", "Water", "Short Circuit", "Giant Shark", "Velociraptor", "Air",
            "Fall", "BTR-80", "Rorsch Mk-1", "Rebel", "Hypoxia", "99A", "A-10", "F-35", "AC-130", "Tsar Bomba",
            "Recycle Bin", "Joy", "Elevator", "Tactical Nuclear", "Task Manager", "Black Hole", "Time",
            "Space Radiation", "Ender Dragon", "Creeper", "Wooden Sword", "TNT", "Void", "Truncheon", "Recurve Bow",
            "Burn", "Sun", "Light", "Cybertruck", "Tornado", "Liquid Nitrogen", "KeyBoard", "Unicorn", "Talk", "Anim",
            "Auto-9", "Mercury Hg", "Formaldehyde", "Gamma Ray", "X-ray", "Snake", "Bug", "Code", "Blaster", "Mouth",
            "Money", "Work", "Armored Train", "Rainbow Cat", "Butter Cat", "Medkit", "Pacemaker", "Blowtorch",
            "Ambulance", "Bus", "Game Leak", "Pencil", "Lightning", "Skill Issue", "C4", "M134", "Dance", "GP-30",
            "M79", "Big Sally", "Neuralyzer", "Noisy Cricket", "Death Star", "Star Destroyer", "Millennium Falcon",
            "42", "Gatling Laser", "Supernova Explosion", "Bathtub", "Gas Tank", "Game", "Video", "UFO", "Lava",
            "Other Gun"
        };

        internal static GameObject KillPrefab { get; private set; }

        private static int _kills;

        internal static Action<HitInfo> ShowHit;

        internal static Action<KillInfo> ShowKill;

        public GamePanelHUDHitPlugin()
        {
            _setData = new SettingsData(Config);
        }

        private void Start()
        {
            _PlayerHelper.ApplyDamageInfo.Add(this, nameof(ApplyDamageInfo));
            _PlayerHelper.OnBeenKilledByAggressor.Add(this, nameof(OnBeenKilledByAggressor));
            _PlayerHelper.ArmorComponentHelper.ApplyDamage.Add(this, nameof(ApplyDamage),
                HarmonyPatchType.ILManipulator);

            HUDCore.UpdateManger.Register(this);
        }

        private void Awake()
        {
            var prefabs = HUDCore.LoadHUD("gamepanelhithud.bundle", "GamePanelHitHUD");

            KillPrefab = prefabs.Asset["Kill"];

            _screenRect = HUDCore.GamePanelHUDPublic.GetComponent<RectTransform>();
        }

        public void CustomUpdate()
        {
            HitPlugin();
        }

        private void HitPlugin()
        {
            _hitHUDSw = HUDCore.AllHUDSw && HUDCore.HasPlayer && _setData.KeyHitHUDSw.Value;
            _killHUDSw = HUDCore.AllHUDSw && HUDCore.HasPlayer && _setData.KeyKillHUDSw.Value;
            _expHUDSw = HUDCore.AllHUDSw && HUDCore.HasPlayer && _setData.KeyExpHUDSw.Value;

            HitHUD.Set(_screenRect, _setData, _hitHUDSw);
            KillHUD.Set(_screenRect, _setData, _killHUDSw, _expHUDSw, _setData.KeyExpHUDSw.Value);

            if (!HUDCore.HasPlayer)
            {
                _kills = 0;

                Armor.Reset();
            }
        }

        private static void DrawTestHit(ConfigEntryBase entry)
        {
            if (GUILayout.Button("Test Hit", GUILayout.ExpandWidth(true)))
            {
                TestHit();
            }
        }

        private static void TestHit()
        {
            var hitType = Enum.GetValues(typeof(HitInfo.Hit));

            var part = Enum.GetValues(typeof(EBodyPart));

            var type = (HitInfo.Hit)hitType.GetValue(UnityEngine.Random.Range(0, hitType.Length));

            var hitInfo = new HitInfo
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

        private static void DrawTestKill(ConfigEntryBase entry)
        {
            if (GUILayout.Button("Test Kill", GUILayout.ExpandWidth(true)))
            {
                TestKill();
            }
        }

        private static void TestKill()
        {
            var role = Enum.GetValues(typeof(WildSpawnType));

            var side = Enum.GetValues(typeof(EPlayerSide));

            var part = Enum.GetValues(typeof(EBodyPart));

            var allowRole = (WildSpawnType)role.GetValue(UnityEngine.Random.Range(0, role.Length));
            try
            {
                _PlayerHelper.RoleHelper.IsBossOrFollower(allowRole);
            }
            catch
            {
                allowRole = (WildSpawnType)role.GetValue(0);
            }

            var killInfo = new KillInfo
            {
                WeaponName = TestWeaponName[UnityEngine.Random.Range(0, TestWeaponName.Length)],
                PlayerName = TestName[UnityEngine.Random.Range(0, TestName.Length)],
                Role = allowRole,
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

        public class KillHUDClass<T, TV> : GamePanelHUDCorePlugin.HUDClass<T, TV>
        {
            public bool HUDSw2;

            public bool HUDSw3;

            public void Set(T info, TV setData, bool hudSw, bool hudSw2, bool hudSw3)
            {
                Set(info, setData, hudSw);

                HUDSw2 = hudSw2;
                HUDSw3 = hudSw3;
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
            public bool Activate;

            public float Damage;

            public void Set(DamageInfo damageInfo, float armorDamage)
            {
                if (_PlayerHelper.DamageInfoHelper.GetPlayer(damageInfo) == HUDCore.YourPlayer)
                {
                    Damage = armorDamage;
                    Activate = true;
                }
            }

            public void Reset()
            {
                Damage = 0;
                Activate = false;
            }
        }

        public class SettingsData
        {
            public readonly ConfigEntry<bool> KeyHitHUDSw;
            public readonly ConfigEntry<bool> KeyHitDamageHUDSw;
            public readonly ConfigEntry<bool> KeyHitHasHead;
            public readonly ConfigEntry<bool> KeyHitHasDirection;
            public readonly ConfigEntry<bool> KeyKillHUDSw;
            public readonly ConfigEntry<bool> KeyKillWaitBottom;
            public readonly ConfigEntry<bool> KeyKillHasDistance;
            public readonly ConfigEntry<bool> KeyKillHasStreak;
            public readonly ConfigEntry<bool> KeyKillHasOther;
            public readonly ConfigEntry<bool> KeyKillHasXp;
            public readonly ConfigEntry<bool> KeyKillHasLevel;
            public readonly ConfigEntry<bool> KeyKillHasPart;
            public readonly ConfigEntry<bool> KeyKillHasWeapon;
            public readonly ConfigEntry<bool> KeyKillHasSide;
            public readonly ConfigEntry<bool> KeyKillScavEn;
            public readonly ConfigEntry<bool> KeyExpHUDSw;

            public readonly ConfigEntry<Vector2> KeyHitAnchoredPosition;
            public readonly ConfigEntry<Vector2> KeyHitSizeDelta;
            public readonly ConfigEntry<Vector2> KeyHitLocalScale;
            public readonly ConfigEntry<Vector2> KeyHitHeadSizeDelta;
            public readonly ConfigEntry<Vector2> KeyHitDamageAnchoredPosition;
            public readonly ConfigEntry<Vector2> KeyHitDamageSizeDelta;
            public readonly ConfigEntry<Vector2> KeyHitDamageLocalScale;
            public readonly ConfigEntry<Vector2> KeyKillAnchoredPosition;
            public readonly ConfigEntry<Vector2> KeyKillSizeDelta;
            public readonly ConfigEntry<Vector2> KeyKillLocalScale;
            public readonly ConfigEntry<Vector2> KeyKillExpAnchoredPosition;
            public readonly ConfigEntry<Vector2> KeyKillExpSizeDelta;
            public readonly ConfigEntry<Vector2> KeyKillExpLocalScale;

            public readonly ConfigEntry<Vector3> KeyHitLocalRotation;

            public readonly ConfigEntry<int> KeyKillWriteSpeed;
            public readonly ConfigEntry<int> KeyKillWrite2Speed;
            public readonly ConfigEntry<int> KeyKillWaitTime;

            public readonly ConfigEntry<float> KeyHitDirectionLeft;
            public readonly ConfigEntry<float> KeyHitDirectionRight;

            public readonly ConfigEntry<float> KeyHitActiveSpeed;
            public readonly ConfigEntry<float> KeyHitEndSpeed;
            public readonly ConfigEntry<float> KeyHitDeadSpeed;
            public readonly ConfigEntry<float> KeyKillDistance;
            public readonly ConfigEntry<float> KeyKillWaitSpeed;
            public readonly ConfigEntry<float> KeyExpWaitSpeed;

            public readonly ConfigEntry<Color> KeyHitDamageColor;
            public readonly ConfigEntry<Color> KeyHitArmorDamageColor;
            public readonly ConfigEntry<Color> KeyHitDeadColor;
            public readonly ConfigEntry<Color> KeyHitHeadColor;
            public readonly ConfigEntry<Color> KeyHitDamageInfoColor;
            public readonly ConfigEntry<Color> KeyHitArmorDamageInfoColor;
            public readonly ConfigEntry<Color> KeyKillNameColor;
            public readonly ConfigEntry<Color> KeyKillWeaponColor;
            public readonly ConfigEntry<Color> KeyKillDistanceColor;
            public readonly ConfigEntry<Color> KeyKillLevelColor;
            public readonly ConfigEntry<Color> KeyKillXpColor;
            public readonly ConfigEntry<Color> KeyKillPartColor;
            public readonly ConfigEntry<Color> KeyKillMetersColor;
            public readonly ConfigEntry<Color> KeyKillStreakColor;
            public readonly ConfigEntry<Color> KeyKillLvlColor;
            public readonly ConfigEntry<Color> KeyKillStatsStreakColor;
            public readonly ConfigEntry<Color> KeyKillOtherColor;
            public readonly ConfigEntry<Color> KeyKillEnemyDownColor;
            public readonly ConfigEntry<Color> KeyKillBearColor;
            public readonly ConfigEntry<Color> KeyKillUsecColor;
            public readonly ConfigEntry<Color> KeyKillScavColor;
            public readonly ConfigEntry<Color> KeyKillBossColor;
            public readonly ConfigEntry<Color> KeyKillFollowerColor;
            public readonly ConfigEntry<Color> KeyKillBracketColor;
            public readonly ConfigEntry<Color> KeyExpColor;

            public readonly ConfigEntry<FontStyles> KeyHitDamageStyles;
            public readonly ConfigEntry<FontStyles> KeyHitArmorDamageStyles;
            public readonly ConfigEntry<FontStyles> KeyKillInfoStyles;
            public readonly ConfigEntry<FontStyles> KeyKillDistanceStyles;
            public readonly ConfigEntry<FontStyles> KeyKillStreakStyles;
            public readonly ConfigEntry<FontStyles> KeyKillOtherStyles;
            public readonly ConfigEntry<FontStyles> KeyKillXpStyles;
            public readonly ConfigEntry<FontStyles> KeyExpStyles;

            public SettingsData(ConfigFile configFile)
            {
                const string mainSettings = "Main Settings";
                const string prsSettings = "Position Rotation Scale Settings";
                const string hitColorSettings = "Hit Color Settings";
                const string killColorSettings = "Kill Color Settings";
                const string fontStylesSettings = "Font Styles Settings";
                const string speedSettings = "Animation Speed Settings";
                const string distanceSettings = "Distance Settings";
                const string directionRateSettings = "Direction Rate Settings";

                configFile.Bind(mainSettings, "Draw Test Hit", string.Empty,
                    new ConfigDescription(string.Empty, null,
                        new ConfigurationManagerAttributes
                            { Order = 2, HideDefaultButton = true, CustomDrawer = DrawTestHit, HideSettingName = true },
                        new EFTConfigurationAttributes { HideSetting = true }));

                configFile.Bind(mainSettings, "Draw Test Kill", string.Empty,
                    new ConfigDescription(string.Empty, null,
                        new ConfigurationManagerAttributes
                        {
                            Order = 1, HideDefaultButton = true, CustomDrawer = DrawTestKill, HideSettingName = true
                        },
                        new EFTConfigurationAttributes { HideSetting = true }));

                configFile.Bind(mainSettings, "Test Hit", string.Empty,
                    new ConfigDescription(string.Empty, null, new ConfigurationManagerAttributes { Browsable = false },
                        new EFTConfigurationAttributes { ButtonAction = TestHit }));

                configFile.Bind(mainSettings, "Test Kill", string.Empty,
                    new ConfigDescription(string.Empty, null, new ConfigurationManagerAttributes { Browsable = false },
                        new EFTConfigurationAttributes { ButtonAction = TestKill }));

                KeyHitHUDSw = configFile.Bind<bool>(mainSettings, "Hit HUD display", true);
                KeyHitDamageHUDSw = configFile.Bind<bool>(mainSettings, "Hit Damage HUD display", false);
                KeyHitHasHead = configFile.Bind<bool>(mainSettings, "Hit Head Separate Color", false);
                KeyHitHasDirection = configFile.Bind<bool>(mainSettings, "Hit Direction", true);
                KeyKillHUDSw = configFile.Bind<bool>(mainSettings, "Kill HUD display", true);
                KeyKillWaitBottom = configFile.Bind<bool>(mainSettings, "Wait Bottom Kill Destroy", true);
                KeyKillHasDistance = configFile.Bind<bool>(mainSettings, "Kill Distance display", true);
                KeyKillHasOther = configFile.Bind<bool>(mainSettings, "Kill Other display", true);
                KeyKillHasStreak = configFile.Bind<bool>(mainSettings, "Kill Streak display", true);
                KeyKillHasXp = configFile.Bind<bool>(mainSettings, "Kill Xp display", true);
                KeyKillHasLevel = configFile.Bind<bool>(mainSettings, "Kill Level display", false);
                KeyKillHasPart = configFile.Bind<bool>(mainSettings, "Kill Part display", false);
                KeyKillHasSide = configFile.Bind<bool>(mainSettings, "Kill Faction display", false);
                KeyKillHasWeapon = configFile.Bind<bool>(mainSettings, "Kill Weapon display", true);
                KeyKillScavEn = configFile.Bind<bool>(mainSettings, "Kill Scav Name To En", true);
                KeyExpHUDSw = configFile.Bind<bool>(mainSettings, "Exp HUD display", true);

                KeyHitAnchoredPosition =
                    configFile.Bind<Vector2>(prsSettings, "Hit Anchored Position", new Vector2(12, 12));
                KeyHitSizeDelta = configFile.Bind<Vector2>(prsSettings, "Hit Size Delta", new Vector2(3, 12));
                KeyHitLocalScale = configFile.Bind<Vector2>(prsSettings, "Hit Local Scale", new Vector2(1, 1));
                KeyHitHeadSizeDelta =
                    configFile.Bind<Vector2>(prsSettings, "Hit Head Size Delta", new Vector2(3, 16));
                KeyHitDamageAnchoredPosition = configFile.Bind<Vector2>(prsSettings,
                    "Hit Damage Anchored Position", new Vector2(30, 0));
                KeyHitDamageSizeDelta =
                    configFile.Bind<Vector2>(prsSettings, "Hit Damage Size Delta", new Vector2(60, 34));
                KeyHitDamageLocalScale =
                    configFile.Bind<Vector2>(prsSettings, "Hit Damage Local Scale", new Vector2(1, 1));
                KeyKillAnchoredPosition =
                    configFile.Bind<Vector2>(prsSettings, "Kill Anchored Position", new Vector2(80, -110));
                KeyKillSizeDelta =
                    configFile.Bind<Vector2>(prsSettings, "Kill Size Delta", new Vector2(500, 180));
                KeyKillLocalScale = configFile.Bind<Vector2>(prsSettings, "Kill Local Scale", new Vector2(1, 1));
                KeyKillExpAnchoredPosition =
                    configFile.Bind<Vector2>(prsSettings, "Exp Anchored Position", new Vector2(100, -105));
                KeyKillExpSizeDelta =
                    configFile.Bind<Vector2>(prsSettings, "Exp Size Delta", new Vector2(80, 30));
                KeyKillExpLocalScale =
                    configFile.Bind<Vector2>(prsSettings, "Exp Local Scale", new Vector2(1, 1));

                KeyHitLocalRotation =
                    configFile.Bind<Vector3>(prsSettings, "Hit Rotation", new Vector3(0, 0, 45));

                KeyHitDirectionLeft = configFile.Bind<float>(directionRateSettings, "Hit Left", -0.5f,
                    new ConfigDescription("When Hit Direction < -0.5 Active Hit Left",
                        new AcceptableValueRange<float>(-1, 1)));
                KeyHitDirectionRight = configFile.Bind<float>(directionRateSettings, "Hit Right", 0.5f,
                    new ConfigDescription("When Hit Direction > 0.5 Active Hit Right",
                        new AcceptableValueRange<float>(-1, 1)));

                KeyHitActiveSpeed = configFile.Bind<float>(speedSettings, "Hit Active Speed", 1,
                    new ConfigDescription(string.Empty, new AcceptableValueRange<float>(0, 10)));
                KeyHitEndSpeed = configFile.Bind<float>(speedSettings, "Hit End Wait Speed", 1,
                    new ConfigDescription(string.Empty, new AcceptableValueRange<float>(0, 10)));
                KeyHitDeadSpeed = configFile.Bind<float>(speedSettings, "Hit Dead Wait Speed", 1,
                    new ConfigDescription(string.Empty, new AcceptableValueRange<float>(0, 10)));
                KeyKillWaitSpeed = configFile.Bind<float>(speedSettings, "Kill Text Wait Speed", 1,
                    new ConfigDescription(string.Empty, new AcceptableValueRange<float>(0, 10)));
                KeyExpWaitSpeed = configFile.Bind<float>(speedSettings, "Exp Wait Speed", 1,
                    new ConfigDescription(string.Empty, new AcceptableValueRange<float>(0, 10)));

                KeyKillWriteSpeed = configFile.Bind<int>(speedSettings, "Kill Text display Speed", 10,
                    new ConfigDescription("Single character input speed (unit ms)",
                        new AcceptableValueRange<int>(0, 1000)));
                KeyKillWrite2Speed = configFile.Bind<int>(speedSettings, "Kill Text 2 display Speed", 10,
                    new ConfigDescription("Single character input speed (unit ms)",
                        new AcceptableValueRange<int>(0, 1000)));
                KeyKillWaitTime = configFile.Bind<int>(speedSettings, "Kill Text 2 Wait Time", 500,
                    new ConfigDescription("Play Text to Text2 ago Wait Time (unit ms)",
                        new AcceptableValueRange<int>(0, 1000)));

                KeyKillDistance = configFile.Bind<float>(distanceSettings, "Kill Distance display", 50,
                    new ConfigDescription("When Kill distance >= How many meters display",
                        new AcceptableValueRange<float>(0, 1000)));

                KeyHitDamageStyles = configFile.Bind<FontStyles>(fontStylesSettings, "Damage", FontStyles.Normal);
                KeyHitArmorDamageStyles =
                    configFile.Bind<FontStyles>(fontStylesSettings, "Armor Damage", FontStyles.Normal);
                KeyKillInfoStyles = configFile.Bind<FontStyles>(fontStylesSettings, "Kill Info", FontStyles.Bold);
                KeyKillDistanceStyles =
                    configFile.Bind<FontStyles>(fontStylesSettings, "Kill Distance", FontStyles.Bold);
                KeyKillStreakStyles =
                    configFile.Bind<FontStyles>(fontStylesSettings, "Kill Streak", FontStyles.Normal);
                KeyKillOtherStyles =
                    configFile.Bind<FontStyles>(fontStylesSettings, "Kill Other", FontStyles.Normal);
                KeyKillXpStyles = configFile.Bind<FontStyles>(fontStylesSettings, "Kill Xp", FontStyles.Normal);
                KeyExpStyles = configFile.Bind<FontStyles>(fontStylesSettings, "Exp", FontStyles.Normal);

                KeyHitDamageColor = configFile.Bind<Color>(hitColorSettings, "Damage", new Color(1f, 1f, 1f));
                KeyHitArmorDamageColor =
                    configFile.Bind<Color>(hitColorSettings, "Armor Damage", new Color(0, 0.5f, 0.8f));
                KeyHitDeadColor = configFile.Bind<Color>(hitColorSettings, "Dead", new Color(1f, 0f, 0f));
                KeyHitHeadColor = configFile.Bind<Color>(hitColorSettings, "Head", new Color(1f, 0.3f, 0f));
                KeyHitDamageInfoColor =
                    configFile.Bind<Color>(hitColorSettings, "Damage Info", new Color(1f, 0f, 0f));
                KeyHitArmorDamageInfoColor = configFile.Bind<Color>(hitColorSettings, "Armor Damage Info",
                    new Color(0, 0.5f, 0.8f));

                KeyKillNameColor = configFile.Bind<Color>(killColorSettings, "Name", new Color(1f, 0.2f, 0.2f));
                KeyKillWeaponColor = configFile.Bind<Color>(killColorSettings, "Weapon",
                    new Color(0.8901961f, 0.8901961f, 0.8392157f));
                KeyKillDistanceColor = configFile.Bind<Color>(killColorSettings, "Distance", new Color(1f, 1f, 0f));
                KeyKillLevelColor = configFile.Bind<Color>(killColorSettings, "Level",
                    new Color(0.8901961f, 0.8901961f, 0.8392157f));
                KeyKillStreakColor = configFile.Bind<Color>(killColorSettings, "Streak", new Color(1f, 1f, 0f));
                KeyKillPartColor = configFile.Bind<Color>(killColorSettings, "Body Part",
                    new Color(0.6039216f, 0.827451f, 0.1372549f));
                KeyKillMetersColor = configFile.Bind<Color>(killColorSettings, "Meters",
                    new Color(0.8901961f, 0.8901961f, 0.8392157f));
                KeyKillLvlColor =
                    configFile.Bind<Color>(killColorSettings, "Lvl", new Color(0.8901961f, 0.8901961f, 0.8392157f));
                KeyKillXpColor =
                    configFile.Bind<Color>(killColorSettings, "Xp", new Color(0.8901961f, 0.8901961f, 0.8392157f));
                KeyKillStatsStreakColor = configFile.Bind<Color>(killColorSettings, "Stats Streak",
                    new Color(0.8901961f, 0.8901961f, 0.8392157f));
                KeyKillOtherColor = configFile.Bind<Color>(killColorSettings, "Other",
                    new Color(0.8901961f, 0.8901961f, 0.8392157f));
                KeyKillEnemyDownColor = configFile.Bind<Color>(killColorSettings, "Enemy Down",
                    new Color(0.8901961f, 0.8901961f, 0.8392157f));
                KeyKillUsecColor = configFile.Bind<Color>(killColorSettings, "Kill Usec", new Color(0f, 0.8f, 1f));
                KeyKillBearColor = configFile.Bind<Color>(killColorSettings, "Kill Bear", new Color(1f, 0.5f, 0f));
                KeyKillScavColor = configFile.Bind<Color>(killColorSettings, "Kill Scav", new Color(1f, 0.8f, 0f));
                KeyKillBossColor = configFile.Bind<Color>(killColorSettings, "Kill Boss", new Color(1f, 0f, 0f));
                KeyKillFollowerColor =
                    configFile.Bind<Color>(killColorSettings, "Kill Follower", new Color(0.8f, 0f, 0f));
                KeyKillBracketColor = configFile.Bind<Color>(killColorSettings, "Bracket",
                    new Color(0.8901961f, 0.8901961f, 0.8392157f));
                KeyExpColor = configFile.Bind<Color>(killColorSettings, "Exp",
                    new Color(0.8901961f, 0.8901961f, 0.8392157f));
            }
        }
    }
}

#endif