#if !UNITY_EDITOR

using System.Diagnostics.CodeAnalysis;
using BepInEx.Configuration;
using TMPro;
using UnityEngine;

namespace GamePanelHUDCompass.Models
{
    public class SettingsModel
    {
        public static SettingsModel Instance { get; private set; }

        public readonly ConfigEntry<bool> KeyCompassHUDSw;
        public readonly ConfigEntry<bool> KeyAngleHUDSw;
        public readonly ConfigEntry<bool> KeyCompassFireHUDSw;
        public readonly ConfigEntry<bool> KeyCompassFireDirectionHUDSw;
        public readonly ConfigEntry<bool> KeyCompassFireSilenced;
        public readonly ConfigEntry<bool> KeyCompassFireDeadDestroy;
        public readonly ConfigEntry<bool> KeyCompassStaticHUDSw;
        public readonly ConfigEntry<bool> KeyCompassStaticAirdrop;
        public readonly ConfigEntry<bool> KeyCompassStaticExfiltration;
        public readonly ConfigEntry<bool> KeyCompassStaticQuest;
        public readonly ConfigEntry<bool> KeyCompassStaticInfoHUDSw;
        public readonly ConfigEntry<bool> KeyCompassStaticDistanceHUDSw;
        public readonly ConfigEntry<bool> KeyCompassStaticInZoneHUDSw;
        public readonly ConfigEntry<bool> KeyCompassStaticHideRequirements;
        public readonly ConfigEntry<bool> KeyCompassStaticHideOptional;
        public readonly ConfigEntry<bool> KeyCompassStaticHideSearchedAirdrop;
        public readonly ConfigEntry<bool> KeyCompassAutoSizeDelta;
        public readonly ConfigEntry<bool> KeyCompassStaticInfoAutoSizeDelta;

        public readonly ConfigEntry<bool> KeyConditionFindItem;
        public readonly ConfigEntry<bool> KeyConditionLeaveItemAtLocation;
        public readonly ConfigEntry<bool> KeyConditionPlaceBeacon;
        public readonly ConfigEntry<bool> KeyConditionVisitPlace;
        public readonly ConfigEntry<bool> KeyConditionInZone;

        public readonly ConfigEntry<Vector2> KeyAnchoredPosition;
        public readonly ConfigEntry<Vector2> KeySizeDelta;
        public readonly ConfigEntry<Vector2> KeyLocalScale;
        public readonly ConfigEntry<Vector2> KeyCompassFireSizeDelta;
        public readonly ConfigEntry<Vector2> KeyCompassFireOutlineSizeDelta;
        public readonly ConfigEntry<Vector2> KeyCompassFireDirectionAnchoredPosition;
        public readonly ConfigEntry<Vector2> KeyCompassFireDirectionScale;
        public readonly ConfigEntry<Vector2> KeyCompassStaticInfoAnchoredPosition;
        public readonly ConfigEntry<Vector2> KeyCompassStaticInfoSizeDelta;
        public readonly ConfigEntry<Vector2> KeyCompassStaticInfoScale;

        public readonly ConfigEntry<float> KeyCompassFireHeight;
        public readonly ConfigEntry<float> KeyCompassFireDistance;
        public readonly ConfigEntry<float> KeyCompassFireActiveSpeed;
        public readonly ConfigEntry<float> KeyCompassFireWaitSpeed;
        public readonly ConfigEntry<float> KeyCompassFireToSmallSpeed;
        public readonly ConfigEntry<float> KeyCompassFireSmallWaitSpeed;
        public readonly ConfigEntry<float> KeyCompassStaticHeight;

        public readonly ConfigEntry<int> KeyAutoSizeDeltaRate;
        public readonly ConfigEntry<int> KeyCompassStaticCenterPointRange;

        public readonly ConfigEntry<Color> KeyArrowColor;
        public readonly ConfigEntry<Color> KeyAzimuthsColor;
        public readonly ConfigEntry<Color> KeyAzimuthsAngleColor;
        public readonly ConfigEntry<Color> KeyDirectionColor;
        public readonly ConfigEntry<Color> KeyAngleColor;
        public readonly ConfigEntry<Color> KeyCompassFireColor;
        public readonly ConfigEntry<Color> KeyCompassFireOutlineColor;
        public readonly ConfigEntry<Color> KeyCompassFireBossColor;
        public readonly ConfigEntry<Color> KeyCompassFireBossOutlineColor;
        public readonly ConfigEntry<Color> KeyCompassFireFollowerColor;
        public readonly ConfigEntry<Color> KeyCompassFireFollowerOutlineColor;
        public readonly ConfigEntry<Color> KeyCompassStaticNameColor;
        public readonly ConfigEntry<Color> KeyCompassStaticDescriptionColor;
        public readonly ConfigEntry<Color> KeyCompassStaticNecessaryColor;
        public readonly ConfigEntry<Color> KeyCompassStaticRequirementsColor;
        public readonly ConfigEntry<Color> KeyCompassStaticDistanceColor;
        public readonly ConfigEntry<Color> KeyCompassStaticMetersColor;
        public readonly ConfigEntry<Color> KeyCompassStaticInZoneColor;

        public readonly ConfigEntry<FontStyles> KeyAzimuthsAngleStyles;
        public readonly ConfigEntry<FontStyles> KeyDirectionStyles;
        public readonly ConfigEntry<FontStyles> KeyAngleStyles;
        public readonly ConfigEntry<FontStyles> KeyCompassFireDirectionStyles;
        public readonly ConfigEntry<FontStyles> KeyCompassStaticNameStyles;
        public readonly ConfigEntry<FontStyles> KeyCompassStaticDescriptionStyles;
        public readonly ConfigEntry<FontStyles> KeyCompassStaticDistanceStyles;

        [SuppressMessage("ReSharper", "RedundantTypeArgumentsOfMethod")]
        private SettingsModel(ConfigFile configFile)
        {
            const string mainSettings = "Main Settings";
            const string questSettings = "Quest Display Settings";
            const string positionScaleSettings = "Position Scale Settings";
            const string colorSettings = "Color Settings";
            const string fontStylesSettings = "Font Styles Settings";
            const string speedSettings = "Animation Speed Settings";
            const string rateSettings = "Rate Settings";
            const string otherSettings = "Other Settings";

            KeyCompassHUDSw = configFile.Bind<bool>(mainSettings, "Compass HUD display", true);
            KeyAngleHUDSw = configFile.Bind<bool>(mainSettings, "Compass Angle HUD display", true);
            KeyCompassFireHUDSw = configFile.Bind<bool>(mainSettings, "Compass Fire HUD display", true);
            KeyCompassFireDirectionHUDSw =
                configFile.Bind<bool>(mainSettings, "Compass Fire Direction HUD display", true);
            KeyCompassFireSilenced =
                configFile.Bind<bool>(mainSettings, "Compass Fire Hide Silenced", true);
            KeyCompassFireDeadDestroy =
                configFile.Bind<bool>(mainSettings, "Compass Fire Dead Destroy", true);
            KeyCompassStaticHUDSw =
                configFile.Bind<bool>(mainSettings, "Compass Static HUD display", true);
            KeyCompassStaticAirdrop =
                configFile.Bind<bool>(mainSettings, "Compass Static Airdrop display", true);
            KeyCompassStaticExfiltration =
                configFile.Bind<bool>(mainSettings, "Compass Static Exfiltration display", true);
            KeyCompassStaticQuest =
                configFile.Bind<bool>(mainSettings, "Compass Static Quest display", true);
            KeyCompassStaticInfoHUDSw =
                configFile.Bind<bool>(mainSettings, "Compass Static Info HUD display", true);
            KeyCompassStaticDistanceHUDSw =
                configFile.Bind<bool>(mainSettings, "Compass Static Distance HUD display", true);
            KeyCompassStaticInZoneHUDSw =
                configFile.Bind<bool>(mainSettings, "Compass Static InZone HUD display", true);
            KeyCompassStaticHideRequirements =
                configFile.Bind<bool>(mainSettings, "Compass Static Hide Requirements", false);
            KeyCompassStaticHideOptional =
                configFile.Bind<bool>(mainSettings, "Compass Static Hide Optional", false);
            KeyCompassStaticHideSearchedAirdrop = configFile.Bind<bool>(mainSettings,
                "Compass Static Hide Already Searched Airdrop", true);
            KeyCompassAutoSizeDelta = configFile.Bind<bool>(mainSettings, "Compass Auto Size Delta", true);
            KeyCompassStaticInfoAutoSizeDelta =
                configFile.Bind<bool>(mainSettings, "Compass Static Info Auto Size Delta", true);

            KeyConditionFindItem = configFile.Bind<bool>(questSettings, "FindItem", true);
            KeyConditionLeaveItemAtLocation = configFile.Bind<bool>(questSettings, "LeaveItemAtLocation", true);
            KeyConditionPlaceBeacon = configFile.Bind<bool>(questSettings, "PlaceBeacon", true);
            KeyConditionVisitPlace = configFile.Bind<bool>(questSettings, "VisitPlace", true);
            KeyConditionInZone = configFile.Bind<bool>(questSettings, "InZone", true);

            KeyAnchoredPosition =
                configFile.Bind<Vector2>(positionScaleSettings, "Anchored Position", Vector2.zero);
            KeySizeDelta =
                configFile.Bind<Vector2>(positionScaleSettings, "Size Delta", new Vector2(600, 90));
            KeyLocalScale =
                configFile.Bind<Vector2>(positionScaleSettings, "Local Scale", new Vector2(1, 1));
            KeyCompassFireSizeDelta = configFile.Bind<Vector2>(positionScaleSettings,
                "Compass Fire Size Delta", new Vector2(25, 25));
            KeyCompassFireOutlineSizeDelta = configFile.Bind<Vector2>(positionScaleSettings,
                "Compass Fire Outline Size Delta", new Vector2(26, 26));
            KeyCompassFireDirectionAnchoredPosition = configFile.Bind<Vector2>(positionScaleSettings,
                "Compass Fire Direction Anchored Position", new Vector2(15, -63));
            KeyCompassFireDirectionScale = configFile.Bind<Vector2>(positionScaleSettings,
                "Compass Fire Direction Local Scale", new Vector2(1, 1));
            KeyCompassStaticInfoAnchoredPosition = configFile.Bind<Vector2>(positionScaleSettings,
                "Compass Static Info Anchored Position", new Vector2(0, -15));
            KeyCompassStaticInfoSizeDelta = configFile.Bind<Vector2>(positionScaleSettings,
                "Compass Static Info Size Delta", new Vector2(240, 70));
            KeyCompassStaticInfoScale = configFile.Bind<Vector2>(positionScaleSettings,
                "Compass Static Info Local Scale", new Vector2(1, 1));

            KeyCompassFireActiveSpeed = configFile.Bind<float>(speedSettings, "Compass Fire Active Speed",
                1, new ConfigDescription(string.Empty, new AcceptableValueRange<float>(0, 10)));
            KeyCompassFireWaitSpeed = configFile.Bind<float>(speedSettings, "Compass Fire Wait Speed", 1,
                new ConfigDescription(string.Empty, new AcceptableValueRange<float>(0, 10)));
            KeyCompassFireToSmallSpeed = configFile.Bind<float>(speedSettings,
                "Compass Fire To Small Speed", 1,
                new ConfigDescription(string.Empty, new AcceptableValueRange<float>(0, 10)));
            KeyCompassFireSmallWaitSpeed = configFile.Bind<float>(speedSettings,
                "Compass Fire Small Wait Speed", 1,
                new ConfigDescription(string.Empty, new AcceptableValueRange<float>(0, 10)));

            KeyCompassFireHeight = configFile.Bind<float>(positionScaleSettings, "Compass Fire Height", 8);
            KeyCompassStaticHeight =
                configFile.Bind<float>(positionScaleSettings, "Compass Static Height", 5);

            KeyCompassFireDistance = configFile.Bind<float>(otherSettings, "Compass Fire Max Distance",
                50,
                new ConfigDescription("Fire distance <= How many meters display",
                    new AcceptableValueRange<float>(0, 1000)));
            KeyCompassStaticCenterPointRange =
                configFile.Bind<int>(otherSettings, "Compass Static Center Point Range", 20);

            KeyAutoSizeDeltaRate = configFile.Bind<int>(rateSettings, "Auto Size Delta Rate", 30,
                new ConfigDescription("Screen percentage", new AcceptableValueRange<int>(0, 100)));

            KeyArrowColor = configFile.Bind<Color>(colorSettings, "Arrow", new Color(1f, 1f, 1f));
            KeyAzimuthsColor = configFile.Bind<Color>(colorSettings, "Azimuths",
                new Color(0.8901961f, 0.8901961f, 0.8392157f));
            KeyAzimuthsAngleColor = configFile.Bind<Color>(colorSettings, "Azimuths Angle",
                new Color(0.8901961f, 0.8901961f, 0.8392157f));
            KeyDirectionColor = configFile.Bind<Color>(colorSettings, "Direction",
                new Color(0.8901961f, 0.8901961f, 0.8392157f));
            KeyAngleColor =
                configFile.Bind<Color>(colorSettings, "Angle", new Color(0.8901961f, 0.8901961f, 0.8392157f));

            KeyCompassFireColor =
                configFile.Bind<Color>(colorSettings, "Compass Fire", new Color(1f, 0f, 0f));
            KeyCompassFireOutlineColor =
                configFile.Bind<Color>(colorSettings, "Compass Fire Outline", new Color(0.5f, 0f, 0f));
            KeyCompassFireBossColor =
                configFile.Bind<Color>(colorSettings, "Compass Boss Fire", new Color(1f, 0.5f, 0f));
            KeyCompassFireBossOutlineColor = configFile.Bind<Color>(colorSettings,
                "Compass Boss Fire Outline", new Color(1f, 0.3f, 0f));
            KeyCompassFireFollowerColor =
                configFile.Bind<Color>(colorSettings, "Compass Follower Fire", new Color(0f, 1f, 1f));
            KeyCompassFireFollowerOutlineColor = configFile.Bind<Color>(colorSettings,
                "Compass Follower Outline", new Color(0f, 0.7f, 1f));
            KeyCompassStaticNameColor = configFile.Bind<Color>(colorSettings, "Compass Static Name",
                new Color(0.8901961f, 0.8901961f, 0.8392157f));
            KeyCompassStaticDescriptionColor = configFile.Bind<Color>(colorSettings,
                "Compass Static Description", new Color(0.8901961f, 0.8901961f, 0.8392157f));
            KeyCompassStaticNecessaryColor = configFile.Bind<Color>(colorSettings,
                "Compass Static Optional", new Color(0.8901961f, 0.8901961f, 0.8392157f));
            KeyCompassStaticRequirementsColor = configFile.Bind<Color>(colorSettings,
                "Compass Static Requirements", new Color(0.8901961f, 0.8901961f, 0.8392157f));
            KeyCompassStaticDistanceColor = configFile.Bind<Color>(colorSettings, "Compass Static Distance",
                new Color(0.8901961f, 0.8901961f, 0.8392157f));
            KeyCompassStaticMetersColor = configFile.Bind<Color>(colorSettings, "Compass Static Meters",
                new Color(0.8901961f, 0.8901961f, 0.8392157f));
            KeyCompassStaticInZoneColor = configFile.Bind<Color>(colorSettings, "Compass Static InZone",
                new Color(1f, 0.5f, 0f));

            KeyAzimuthsAngleStyles =
                configFile.Bind<FontStyles>(fontStylesSettings, "Azimuths Angle", FontStyles.Normal);
            KeyDirectionStyles = configFile.Bind<FontStyles>(fontStylesSettings, "Direction", FontStyles.Bold);
            KeyAngleStyles = configFile.Bind<FontStyles>(fontStylesSettings, "Angle", FontStyles.Bold);
            KeyCompassFireDirectionStyles = configFile.Bind<FontStyles>(fontStylesSettings,
                "Compass Fire Direction", FontStyles.Normal);
            KeyCompassStaticNameStyles =
                configFile.Bind<FontStyles>(fontStylesSettings, "Compass Static Name", FontStyles.Bold);
            KeyCompassStaticDescriptionStyles = configFile.Bind<FontStyles>(fontStylesSettings,
                "Compass Static Description", FontStyles.Normal);
            KeyCompassStaticDistanceStyles = configFile.Bind<FontStyles>(fontStylesSettings,
                "Compass Static Distance", FontStyles.Bold);
        }

        // ReSharper disable once UnusedMethodReturnValue.Global
        public static SettingsModel Create(ConfigFile configFile)
        {
            if (Instance != null)
                return Instance;

            return Instance = new SettingsModel(configFile);
        }
    }
}

#endif