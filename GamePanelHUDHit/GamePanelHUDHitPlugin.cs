#if !UNITY_EDITOR

using BepInEx;
using EFT.HealthSystem;
using GamePanelHUDCore.Attributes;
using GamePanelHUDCore.Models;
using GamePanelHUDHit.Models;
using HarmonyLib;
using KmyTarkovUtils;
using UnityEngine;
using static KmyTarkovApi.EFTHelpers;
using SettingsModel = GamePanelHUDHit.Models.SettingsModel;

namespace GamePanelHUDHit
{
    [BepInPlugin("com.kmyuhkyuk.GamePanelHUDHit", "GamePanelHUDHit", "3.4.0")]
    [BepInDependency("com.kmyuhkyuk.GamePanelHUDCore", "3.4.0")]
    [EFTConfigurationPluginAttributes("https://hub.sp-tarkov.com/files/file/652-game-panel-hud", @"localized\hit")]
    public partial class GamePanelHUDHitPlugin : BaseUnityPlugin
    {
        private void Awake()
        {
            SettingsModel.Create(Config);
        }

        private void Start()
        {
            var hudCoreModel = HUDCoreModel.Instance;

            foreach (var value in hudCoreModel.LoadHUD("gamepanelhithud.bundle", "GamePanelHitHUD").Init.Values)
            {
                value.ReplaceAllFont(EFTFontHelper.BenderNormal);
            }

            HitHUDModel.Instance.ScreenRect = hudCoreModel.GamePanelHUDPublic.GetComponent<RectTransform>();

            _PlayerHelper.ApplyDamageInfo.Add(this, nameof(ApplyDamageInfo));
            _ArmorComponentHelper.ApplyDamage.Add(this, nameof(ApplyDamage),
                HarmonyPatchType.ILManipulator);

            //Coop
            _PlayerHelper.ObservedCoopApplyShot?.Add(this, nameof(CoopApplyShot));
        }

        private static void BaseApplyDamageInfo(DamageInfoStruct damageInfo, EBodyPart bodyPartType,
            IHealthController healthController)
        {
            var armorModel = ArmorModel.Instance;

            float armorDamage;
            bool hasArmorHit;
            if (armorModel.Activate)
            {
                armorDamage = armorModel.Damage;
                hasArmorHit = true;

                armorModel.Reset();
            }
            else
            {
                armorDamage = 0;
                hasArmorHit = false;
            }

            HitModel.Hit hitType;
            if (healthController.IsAlive)
            {
                hitType = hasArmorHit ? HitModel.Hit.HasArmorHit : HitModel.Hit.OnlyHp;
            }
            else
            {
                hitType = HitModel.Hit.Dead;
            }

            var info = new HitModel
            {
                Damage = damageInfo.Damage,
                DamagePart = bodyPartType,
                HitPoint = damageInfo.HitPoint,
                ArmorDamage = armorDamage,
                HasArmorHit = hasArmorHit,
                HitType = hitType,
                HitDirection = damageInfo.Direction
            };

            HitHUDModel.Instance.ShowHit(info);
        }
    }
}

#endif