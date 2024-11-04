#if !UNITY_EDITOR

using BepInEx;
using EFTUtils;
using GamePanelHUDCore.Attributes;
using GamePanelHUDCore.Models;
using GamePanelHUDHit.Models;
using HarmonyLib;
using UnityEngine;
using static EFTApi.EFTHelpers;
using SettingsModel = GamePanelHUDHit.Models.SettingsModel;

namespace GamePanelHUDHit
{
    [BepInPlugin("com.kmyuhkyuk.GamePanelHUDHit", "GamePanelHUDHit", "3.1.1")]
    [BepInDependency("com.kmyuhkyuk.GamePanelHUDCore", "3.1.1")]
    [EFTConfigurationPluginAttributes("https://hub.sp-tarkov.com/files/file/652-game-panel-hud", @"localized\hit")]
    public partial class GamePanelHUDHitPlugin : BaseUnityPlugin
    {
        private void Awake()
        {
            SettingsModel.Create(Config);

            var hudCoreModel = HUDCoreModel.Instance;

            foreach (var value in hudCoreModel.LoadHUD("gamepanelhithud.bundle", "GamePanelHitHUD").Init.Values)
            {
                value.ReplaceAllFont(EFTFontHelper.BenderNormal);
            }

            HitHUDModel.Instance.ScreenRect = hudCoreModel.GamePanelHUDPublic.GetComponent<RectTransform>();
        }

        private void Start()
        {
            _PlayerHelper.ApplyDamageInfo.Add(this, nameof(ApplyDamageInfo));
            _ArmorComponentHelper.ApplyDamage.Add(this, nameof(ApplyDamage),
                HarmonyPatchType.ILManipulator);

            //Coop
            _PlayerHelper.ObservedCoopApplyShot?.Add(this, nameof(CoopApplyShot));
        }

        private static void BaseApplyDamageInfo(object damageInfo, EBodyPart bodyPartType, object healthController)
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
            if (_HealthControllerHelper.RefIsAlive.GetValue(healthController))
            {
                hitType = hasArmorHit ? HitModel.Hit.HasArmorHit : HitModel.Hit.OnlyHp;
            }
            else
            {
                hitType = HitModel.Hit.Dead;
            }

            var info = new HitModel
            {
                Damage = _DamageInfoHelper.RefDamage.GetValue(damageInfo),
                DamagePart = bodyPartType,
                HitPoint = _DamageInfoHelper.RefHitPoint.GetValue(damageInfo),
                ArmorDamage = armorDamage,
                HasArmorHit = hasArmorHit,
                HitType = hitType,
                HitDirection = _DamageInfoHelper.RefDirection.GetValue(damageInfo)
            };

            HitHUDModel.Instance.ShowHit(info);
        }
    }
}

#endif