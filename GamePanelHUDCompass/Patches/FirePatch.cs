#if !UNITY_EDITOR
using Aki.Reflection.Patching;
using System.Reflection;
using UnityEngine;
using EFT;
using GamePanelHUDCore;
using GamePanelHUDCore.Utils;

namespace GamePanelHUDCompass.Patches
{
    public class FirePatch : ModulePatch
    {
        private static ReflectionData ReflectionDatas = new ReflectionData();

        static FirePatch()
        {
            ReflectionDatas.RefSettings = RefHelp.FieldRef<InfoClass, object>.Create(typeof(InfoClass), "Settings");
            ReflectionDatas.RefRole = RefHelp.FieldRef<object, WildSpawnType>.Create(ReflectionDatas.RefSettings.FieldType, "Role");
        }

        protected override MethodBase GetTargetMethod()
        {
            return typeof(Player.FirearmController).GetMethod("InitiateShot", BindingFlags.NonPublic | BindingFlags.Instance);
        }

        [PatchPostfix]
        private static void PatchPostfix(Player.FirearmController __instance, Player ____player)
        {
            Player yourPlayer = GamePanelHUDCorePlugin.HUDCore.YourPlayer;

            Vector3 weaponPosition = __instance.WeaponRoot.position;

            if (____player != yourPlayer)
            {
                GamePanelHUDCompassPlugin.CompassFireInfo fireInfo = new GamePanelHUDCompassPlugin.CompassFireInfo()
                {
                    Who = ____player.Id,
                    Where = weaponPosition,
                    Role = ReflectionDatas.RefRole.GetValue(ReflectionDatas.RefSettings.GetValue(____player.Profile.Info)),
                    IsSilenced = __instance.IsSilenced && !__instance.IsInLauncherMode(),
                    Distance = Vector3.Distance(weaponPosition, yourPlayer.Position)
                };

                GamePanelHUDCompassPlugin.ShowFire(fireInfo);
            }
        }

        public class ReflectionData
        {
            public RefHelp.FieldRef<InfoClass, object> RefSettings;
            public RefHelp.FieldRef<object, WildSpawnType> RefRole;
        }
    }
}
#endif
