#if !UNITY_EDITOR

using EFT;
using GamePanelHUDCore.Models;
using GamePanelHUDHit.Models;
using static EFTApi.EFTHelpers;

namespace GamePanelHUDHit
{
    public partial class GamePanelHUDHitPlugin
    {
        private static void ApplyDamageInfo(Player __instance, DamageInfo damageInfo, EBodyPart bodyPartType)
        {
            if (_PlayerHelper.DamageInfoHelper.GetPlayer(damageInfo) != HUDCoreModel.Instance.YourPlayer)
                return;

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
            if (_PlayerHelper.HealthControllerHelper.RefIsAlive.GetValue(
                    _PlayerHelper.HealthControllerHelper.RefHealthController.GetValue(__instance)))
            {
                hitType = hasArmorHit ? HitModel.Hit.HasArmorHit : HitModel.Hit.OnlyHp;
            }
            else
            {
                hitType = HitModel.Hit.Dead;
            }

            var info = new HitModel
            {
                Damage = damageInfo.DidBodyDamage,
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