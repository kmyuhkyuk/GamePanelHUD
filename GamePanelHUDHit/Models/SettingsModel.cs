#if !UNITY_EDITOR

using System;
using BepInEx.Configuration;
using GamePanelHUDCore.Attributes;
using TMPro;
using UnityEngine;

namespace GamePanelHUDHit.Models
{
    public class SettingsModel
    {
        public static SettingsModel Instance { get; private set; }

        public readonly ConfigEntry<bool> KeyHitHUDSw;
        public readonly ConfigEntry<bool> KeyHitDamageHUDSw;
        public readonly ConfigEntry<bool> KeyHitHasHead;
        public readonly ConfigEntry<bool> KeyHitHasDirection;

        public readonly ConfigEntry<Vector2> KeyHitAnchoredPosition;
        public readonly ConfigEntry<Vector2> KeyHitSizeDelta;
        public readonly ConfigEntry<Vector2> KeyHitLocalScale;
        public readonly ConfigEntry<Vector2> KeyHitHeadSizeDelta;
        public readonly ConfigEntry<Vector2> KeyHitDamageAnchoredPosition;
        public readonly ConfigEntry<Vector2> KeyHitDamageSizeDelta;
        public readonly ConfigEntry<Vector2> KeyHitDamageLocalScale;

        public readonly ConfigEntry<Vector3> KeyHitLocalRotation;

        public readonly ConfigEntry<float> KeyHitDirectionLeft;
        public readonly ConfigEntry<float> KeyHitDirectionRight;

        public readonly ConfigEntry<float> KeyHitActiveSpeed;
        public readonly ConfigEntry<float> KeyHitEndSpeed;
        public readonly ConfigEntry<float> KeyHitDeadSpeed;

        public readonly ConfigEntry<Color> KeyHitDamageColor;
        public readonly ConfigEntry<Color> KeyHitArmorDamageColor;
        public readonly ConfigEntry<Color> KeyHitDeadColor;
        public readonly ConfigEntry<Color> KeyHitHeadColor;
        public readonly ConfigEntry<Color> KeyHitDamageInfoColor;
        public readonly ConfigEntry<Color> KeyHitArmorDamageInfoColor;

        public readonly ConfigEntry<FontStyles> KeyHitDamageStyles;
        public readonly ConfigEntry<FontStyles> KeyHitArmorDamageStyles;

        private SettingsModel(ConfigFile configFile)
        {
            const string mainSettings = "Main Settings";
            const string prsSettings = "Position Rotation Scale Settings";
            const string hitColorSettings = "Hit Color Settings";
            const string fontStylesSettings = "Font Styles Settings";
            const string speedSettings = "Animation Speed Settings";
            const string directionRateSettings = "Direction Rate Settings";

            configFile.Bind(mainSettings, "Draw Test Hit", string.Empty,
                new ConfigDescription(string.Empty, null,
                    new ConfigurationManagerAttributes
                        { Order = 2, HideDefaultButton = true, CustomDrawer = DrawTestHit, HideSettingName = true },
                    new EFTConfigurationAttributes { HideSetting = true }));

            configFile.Bind(mainSettings, "Test Hit", string.Empty,
                new ConfigDescription(string.Empty, null, new ConfigurationManagerAttributes { Browsable = false },
                    new EFTConfigurationAttributes { ButtonAction = TestHit }));

            KeyHitHUDSw = configFile.Bind<bool>(mainSettings, "Hit HUD display", true);
            KeyHitDamageHUDSw = configFile.Bind<bool>(mainSettings, "Hit Damage HUD display", false);
            KeyHitHasHead = configFile.Bind<bool>(mainSettings, "Hit Head Separate Color", false);
            KeyHitHasDirection = configFile.Bind<bool>(mainSettings, "Hit Direction", true);

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

            KeyHitDamageStyles = configFile.Bind<FontStyles>(fontStylesSettings, "Damage", FontStyles.Normal);
            KeyHitArmorDamageStyles =
                configFile.Bind<FontStyles>(fontStylesSettings, "Armor Damage", FontStyles.Normal);

            KeyHitDamageColor = configFile.Bind<Color>(hitColorSettings, "Damage", new Color(1f, 1f, 1f));
            KeyHitArmorDamageColor =
                configFile.Bind<Color>(hitColorSettings, "Armor Damage", new Color(0, 0.5f, 0.8f));
            KeyHitDeadColor = configFile.Bind<Color>(hitColorSettings, "Dead", new Color(1f, 0f, 0f));
            KeyHitHeadColor = configFile.Bind<Color>(hitColorSettings, "Head", new Color(1f, 0.3f, 0f));
            KeyHitDamageInfoColor =
                configFile.Bind<Color>(hitColorSettings, "Damage Info", new Color(1f, 0f, 0f));
            KeyHitArmorDamageInfoColor = configFile.Bind<Color>(hitColorSettings, "Armor Damage Info",
                new Color(0, 0.5f, 0.8f));
        }

        public static SettingsModel Create(ConfigFile configFile)
        {
            if (Instance != null)
                return Instance;

            return Instance = new SettingsModel(configFile);
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
            var hitType = Enum.GetValues(typeof(HitModel.Hit));

            var part = Enum.GetValues(typeof(EBodyPart));

            var type = (HitModel.Hit)hitType.GetValue(UnityEngine.Random.Range(0, hitType.Length));

            var hitInfo = new HitModel
            {
                Damage = UnityEngine.Random.Range(0, 101),
                ArmorDamage = UnityEngine.Random.Range(0f, 100f),
                HasArmorHit = type == HitModel.Hit.HasArmorHit,
                DamagePart = (EBodyPart)part.GetValue(UnityEngine.Random.Range(0, part.Length)),
                HitType = type,
                HitDirection = new Vector3(UnityEngine.Random.Range(-1f, 1f), 0, 0),
                HitPoint = Vector3.zero,
                IsTest = true
            };

            HitHUDModel.Instance.ShowHit?.Invoke(hitInfo);
        }
    }
}

#endif