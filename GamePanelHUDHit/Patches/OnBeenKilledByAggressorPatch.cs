#if !UNITY_EDITOR
using Aki.Reflection.Patching;
using System.Reflection;
using UnityEngine;
using EFT;
using GamePanelHUDCore;
using GamePanelHUDCore.Utils;

namespace GamePanelHUDHit.Patches
{
    public class OnBeenKilledByAggressorPatch : ModulePatch
    {
        private static readonly ReflectionData ReflectionDatas = new ReflectionData();

        static OnBeenKilledByAggressorPatch()
        {
            ReflectionDatas.RefSettings = RefHelp.FieldRef<InfoClass, object>.Create(typeof(InfoClass), "Settings");
            ReflectionDatas.RefExperience = RefHelp.FieldRef<object, int>.Create(ReflectionDatas.RefSettings.FieldType, "Experience" );
            ReflectionDatas.RefRole = RefHelp.FieldRef<object, WildSpawnType>.Create(ReflectionDatas.RefSettings.FieldType, "Role");
        }

        protected override MethodBase GetTargetMethod()
        {
            return typeof(Player).GetMethod("OnBeenKilledByAggressor", BindingFlags.NonPublic | BindingFlags.Instance);
        }

        [PatchPostfix]
        private static void PatchPostfix(Player __instance, Player aggressor, DamageInfo damageInfo, EBodyPart bodyPart)
        {
            if (aggressor == GamePanelHUDCorePlugin.HUDCore.IsYourPlayer)
            {
                GamePanelHUDHitPlugin.KillInfo info = new GamePanelHUDHitPlugin.KillInfo();

                info.PlayerName = __instance.Profile.Nickname;
                info.WeaponName = damageInfo.Weapon.ShortName;
                info.Part = bodyPart;
                info.Distance = Vector3.Distance(aggressor.Position, __instance.Position);
                info.Level = __instance.Profile.Info.Level;
                info.Side = __instance.Profile.Info.Side;

                object settings = ReflectionDatas.RefSettings.GetValue(__instance.Profile.Info);

                info.Exp = ReflectionDatas.RefExperience.GetValue(settings);
                info.Role = ReflectionDatas.RefRole.GetValue(settings);

                info.Kills = GamePanelHUDHitPlugin.Kills++;

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
#endif
