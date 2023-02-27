using Aki.Reflection.Patching;
using System;
using System.Reflection;
using UnityEngine;
using EFT;
using GamePanelHUDCore;
using GamePanelHUDCore.Utils;

namespace GamePanelHUDHit.Patches
{
    public class PlayerApplyDamageInfoPatch : ModulePatch
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

    public class PlayerKillPatch : ModulePatch
    {
        private static readonly ReflectionData ReflectionDatas = new ReflectionData();

        static PlayerKillPatch()
        {
            ReflectionDatas.RefSettings = RefHelp.FieldRef<InfoClass, object>.Create(typeof(InfoClass), "Settings");

            Type settingsType = ReflectionDatas.RefSettings.FieldType;

            ReflectionDatas.RefExperience = RefHelp.FieldRef<object, int>.Create(settingsType, "Experience");
            ReflectionDatas.RefRole = RefHelp.FieldRef<object, WildSpawnType>.Create(settingsType, "Role");
        }

        protected override MethodBase GetTargetMethod()
        {
            return typeof(Player).GetMethod("OnBeenKilledByAggressor", BindingFlags.NonPublic | BindingFlags.Instance);
        }

        [PatchPostfix]
        private static void PatchPostfix(Player __instance, Player aggressor, DamageInfo damageInfo, EBodyPart bodyPart)
        {
            if (aggressor == GamePanelHUDCorePlugin.HUDCore.YourPlayer)
            {
                GamePanelHUDHitPlugin.Kills++;

                object settings = ReflectionDatas.RefSettings.GetValue(__instance.Profile.Info);

                GamePanelHUDHitPlugin.KillInfo info = new GamePanelHUDHitPlugin.KillInfo()
                {
                    PlayerName = __instance.Profile.Nickname,
                    WeaponName = damageInfo.Weapon.ShortName,
                    Part = bodyPart,
                    Distance = Vector3.Distance(aggressor.Position, __instance.Position),
                    Level = __instance.Profile.Info.Level,
                    Side = __instance.Profile.Info.Side,
                    Exp = ReflectionDatas.RefExperience.GetValue(settings),
                    Role = ReflectionDatas.RefRole.GetValue(settings),
                    Kills = GamePanelHUDHitPlugin.Kills
                };

                GamePanelHUDHitPlugin.ShowKill(info);
            }
        }

        public class ReflectionData
        {
            public RefHelp.FieldRef<InfoClass, object> RefSettings;
            public RefHelp.FieldRef<object, int> RefExperience;
            public RefHelp.FieldRef<object, WildSpawnType> RefRole;
        }
    }
}
