#if !UNITY_EDITOR

using System;
using System.Diagnostics.CodeAnalysis;
using BepInEx.Configuration;
using EFT;
using GamePanelHUDCore.Attributes;
using TMPro;
using UnityEngine;

namespace GamePanelHUDKill.Models
{
    internal class SettingsModel
    {
        public static SettingsModel Instance { get; private set; }

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

        public readonly ConfigEntry<Vector2> KeyKillAnchoredPosition;
        public readonly ConfigEntry<Vector2> KeyKillSizeDelta;
        public readonly ConfigEntry<Vector2> KeyKillLocalScale;
        public readonly ConfigEntry<Vector2> KeyKillExpAnchoredPosition;
        public readonly ConfigEntry<Vector2> KeyKillExpSizeDelta;
        public readonly ConfigEntry<Vector2> KeyKillExpLocalScale;

        public readonly ConfigEntry<float> KeyKillWriteWaitTime;
        public readonly ConfigEntry<float> KeyKillWrite2WaitTime;
        public readonly ConfigEntry<float> KeyKillWaitTime;

        public readonly ConfigEntry<float> KeyKillDistance;
        public readonly ConfigEntry<float> KeyKillWaitSpeed;
        public readonly ConfigEntry<float> KeyExpWaitSpeed;

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

        public readonly ConfigEntry<FontStyles> KeyKillInfoStyles;
        public readonly ConfigEntry<FontStyles> KeyKillDistanceStyles;
        public readonly ConfigEntry<FontStyles> KeyKillStreakStyles;
        public readonly ConfigEntry<FontStyles> KeyKillOtherStyles;
        public readonly ConfigEntry<FontStyles> KeyKillXpStyles;
        public readonly ConfigEntry<FontStyles> KeyExpStyles;

        [SuppressMessage("ReSharper", "RedundantTypeArgumentsOfMethod")]
        private SettingsModel(ConfigFile configFile)
        {
            const string mainSettings = "Main Settings";
            const string prsSettings = "Position Rotation Scale Settings";
            const string killColorSettings = "Kill Color Settings";
            const string fontStylesSettings = "Font Styles Settings";
            const string speedSettings = "Animation Speed Settings";
            const string distanceSettings = "Distance Settings";

            configFile.Bind(mainSettings, "Draw Test Kill", string.Empty,
                new ConfigDescription(string.Empty, null,
                    new ConfigurationManagerAttributes
                    {
                        Order = 1,
                        HideDefaultButton = true,
                        CustomDrawer = DrawTestKill,
                        HideSettingName = true
                    },
                    new EFTConfigurationAttributes { HideSetting = true }));

            configFile.Bind(mainSettings, "Test Kill", string.Empty,
                new ConfigDescription(string.Empty, null, new ConfigurationManagerAttributes { Browsable = false },
                    new EFTConfigurationAttributes { ButtonAction = TestKill }));

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

            KeyKillWaitSpeed = configFile.Bind<float>(speedSettings, "Kill Text Wait Speed", 1,
                new ConfigDescription(string.Empty, new AcceptableValueRange<float>(0, 10)));
            KeyExpWaitSpeed = configFile.Bind<float>(speedSettings, "Exp Wait Speed", 1,
                new ConfigDescription(string.Empty, new AcceptableValueRange<float>(0, 10)));

            KeyKillWriteWaitTime = configFile.Bind<float>(speedSettings, "Kill Text Input Speed", 0.01f,
                new ConfigDescription("Single character input speed (unit s)",
                    new AcceptableValueRange<float>(0, 10)));
            KeyKillWrite2WaitTime = configFile.Bind<float>(speedSettings, "Kill Text 2 Input Speed", 0.01f,
                new ConfigDescription("Single character input speed (unit s)",
                    new AcceptableValueRange<float>(0, 10)));
            KeyKillWaitTime = configFile.Bind<float>(speedSettings, "Kill Text 2 Wait Time", 0.5f,
                new ConfigDescription("Play Text to Text2 ago Wait Time (unit s)",
                    new AcceptableValueRange<float>(0, 10)));

            KeyKillDistance = configFile.Bind<float>(distanceSettings, "Kill Distance display", 100,
                new ConfigDescription("When Kill distance >= How many meters display",
                    new AcceptableValueRange<float>(0, 1000)));

            KeyKillInfoStyles = configFile.Bind<FontStyles>(fontStylesSettings, "Kill Info", FontStyles.Bold);
            KeyKillDistanceStyles =
                configFile.Bind<FontStyles>(fontStylesSettings, "Kill Distance", FontStyles.Bold);
            KeyKillStreakStyles =
                configFile.Bind<FontStyles>(fontStylesSettings, "Kill Streak", FontStyles.Normal);
            KeyKillOtherStyles =
                configFile.Bind<FontStyles>(fontStylesSettings, "Kill Other", FontStyles.Normal);
            KeyKillXpStyles = configFile.Bind<FontStyles>(fontStylesSettings, "Kill Xp", FontStyles.Normal);
            KeyExpStyles = configFile.Bind<FontStyles>(fontStylesSettings, "Exp", FontStyles.Normal);

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

        // ReSharper disable once UnusedMethodReturnValue.Global
        public static SettingsModel Create(ConfigFile configFile)
        {
            if (Instance != null)
                return Instance;

            return Instance = new SettingsModel(configFile);
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
                allowRole.IsBossOrFollower();
            }
            catch
            {
                allowRole = (WildSpawnType)role.GetValue(0);
            }

            var killModel = new KillModel
            {
                PlayerName = TestName[UnityEngine.Random.Range(0, TestName.Length)],
                WeaponName = TestWeaponName[UnityEngine.Random.Range(0, TestWeaponName.Length)],
                Part = (EBodyPart)part.GetValue(UnityEngine.Random.Range(0, part.Length)),
                Distance = UnityEngine.Random.Range(0f, 100f),
                Level = UnityEngine.Random.Range(1, 79),
                Side = (EPlayerSide)side.GetValue(UnityEngine.Random.Range(0, side.Length)),
                Exp = UnityEngine.Random.Range(100, 1001),
                Role = allowRole,
                KillCount = UnityEngine.Random.Range(0, 11),
                ScavKillExpPenalty = 0.5f,
                HasMarkOfUnknown = false,
                IsAI = true,
                IsTest = true
            };

            KillHUDModel.Instance.ShowKill(killModel);
        }
    }
}

#endif