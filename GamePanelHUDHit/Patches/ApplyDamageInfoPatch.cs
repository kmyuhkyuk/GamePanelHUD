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
            if (damageInfo.Player == GamePanelHUDCorePlugin.HUDCore.IsYourPlayer)
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

                GamePanelHUDHitPlugin.HitInfo info = new GamePanelHUDHitPlugin.HitInfo();

                info.Damage = damageInfo.DidBodyDamage;
                info.DamagePart = bodyPartType;
                info.HitPoint = damageInfo.HitPoint;
                info.ArmorDamage = armorDamage;

                info.HasArmorHit = hasArmorHit;

                if (__instance.HealthController.IsAlive)
                {
                    info.HitType = hasArmorHit ? GamePanelHUDHitPlugin.HitInfo.Hit.HasArmorHit : GamePanelHUDHitPlugin.HitInfo.Hit.OnlyHp;
                }
                else
                {
                    info.HitType = GamePanelHUDHitPlugin.HitInfo.Hit.Dead;
                }

                info.HitDirection = damageInfo.Direction;

                GamePanelHUDHitPlugin.ShowHit(info);
            }
        }
    }
}
#endif
