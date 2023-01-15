#if !UNITY_EDITOR
using Aki.Reflection.Patching;
using System.Reflection;
using System.Collections.Generic;
using UnityEngine;
using EFT;
using GamePanelHUDCore;
using GamePanelHUDCore.Utils;

namespace GamePanelHUDCompass.Patches
{
    public class FirePatch : ModulePatch
    {
        private static ReflectionData ReflectionDatas = new ReflectionData();

        public static List<GamePanelHUDCompassPlugin.CompassFireInfo> Test = new List<GamePanelHUDCompassPlugin.CompassFireInfo>();

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

            //if (____player != yourPlayer)

            GamePanelHUDCompassPlugin.CompassFireInfo fireInfo = new GamePanelHUDCompassPlugin.CompassFireInfo()
            {
                Who = ____player.Id,
                Where = weaponPosition,
                Role = ReflectionDatas.RefRole.GetValue(ReflectionDatas.RefSettings.GetValue(____player.Profile.Info)),
                IsSilenced = __instance.IsSilenced && !__instance.IsInLauncherMode(),
                Distance = Vector3.Distance(weaponPosition, yourPlayer.Position)
            };

            Test.Add(fireInfo);

            GamePanelHUDCompassPlugin.ShowFire(fireInfo);
        }

        public class ReflectionData
        {
            public RefHelp.FieldRef<InfoClass, object> RefSettings;
            public RefHelp.FieldRef<object, WildSpawnType> RefRole;
        }
    }
}
#endif
