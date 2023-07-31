using EFT;

namespace GamePanelHUDHit
{
    public partial class GamePanelHUDHitPlugin
    {
        private static void ApplyDamageInfo(Player __instance, DamageInfo damageInfo, EBodyPart bodyPartType)
        {
            if (damageInfo.Player == HUDCore.YourPlayer)
            {
                float armorDamage;
                bool hasArmorHit;

                if (Armor.Activate)
                {
                    armorDamage = Armor.Damage;
                    hasArmorHit = true;

                    Armor.Rest();
                }
                else
                {
                    armorDamage = 0;
                    hasArmorHit = false;
                }

                HitInfo.Hit hitType;

                if (__instance.HealthController.IsAlive)
                {
                    hitType = hasArmorHit ? HitInfo.Hit.HasArmorHit : HitInfo.Hit.OnlyHp;
                }
                else
                {
                    hitType = HitInfo.Hit.Dead;
                }

                var info = new HitInfo
                {
                    Damage = damageInfo.DidBodyDamage,
                    DamagePart = bodyPartType,
                    HitPoint = damageInfo.HitPoint,
                    ArmorDamage = armorDamage,
                    HasArmorHit = hasArmorHit,
                    HitType = hitType,
                    HitDirection = damageInfo.Direction
                };

                ShowHit(info);
            }
        }
    }
}