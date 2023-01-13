#if !UNITY_EDITOR
using Aki.Reflection.Patching;
using System.Reflection;
using EFT;
using GamePanelHUDCore;

namespace GamePanelHUDHit.Patches
{
    public class ApplyDamageInfoPatch : ModulePatch
    {
        protected override MethodBase GetTargetMethod()
        {
            return typeof(Player).GetMethod("ApplyDamageInfo", BindingFlags.Public | BindingFlags.Instance);
        }

        [PatchPostfix]
        private static void PatchPostfix(Player __instance, DamageInfo damageInfo, EBodyPart bodyPartType)
        {
            if (damageInfo.Player == GamePanelHUDCorePlugin.HUDCore.YourPlayer)
            {
                float armorDamage;

                bool hasArmorHit;

                if (GamePanelHUDHitPlugin.Armor.Activa)
                {
                    armorDamage = GamePanelHUDHitPlugin.Armor.ArmorDamage;
                    hasArmorHit = true;

                    GamePanelHUDHitPlugin.Armor.Rest();
                }
                else
                {
                    armorDamage = 0;
                    hasArmorHit = false;
                }

                GamePanelHUDHitPlugin.HitInfo.Hit hitType;

                if (__instance.HealthController.IsAlive)
                {
                    hitType = hasArmorHit ? GamePanelHUDHitPlugin.HitInfo.Hit.HasArmorHit : GamePanelHUDHitPlugin.HitInfo.Hit.OnlyHp;
                }
                else
                {
                    hitType = GamePanelHUDHitPlugin.HitInfo.Hit.Dead;
                }

                GamePanelHUDHitPlugin.HitInfo info = new GamePanelHUDHitPlugin.HitInfo()
                {
                    Damage = damageInfo.DidBodyDamage,
                    DamagePart = bodyPartType,
                    HitPoint = damageInfo.HitPoint,
                    ArmorDamage = armorDamage,
                    HasArmorHit = hasArmorHit,
                    HitType = hitType,
                    HitDirection = damageInfo.Direction
                };

                GamePanelHUDHitPlugin.ShowHit(info);
            }
        }
    }
}
#endif
