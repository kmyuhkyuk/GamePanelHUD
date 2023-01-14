#if !UNITY_EDITOR
using Aki.Reflection.Patching;
using System.Reflection;
using System.Collections.Generic;
using UnityEngine;
using EFT;
using GamePanelHUDCore;

namespace GamePanelHUDCompass.Patches
{
    public class FirePatch : ModulePatch
    {
        public static List<GamePanelHUDCompassPlugin.CompassFireInfo> Test = new List<GamePanelHUDCompassPlugin.CompassFireInfo>();

        protected override MethodBase GetTargetMethod()
        {
            return typeof(Player.FirearmController).GetMethod("InitiateShot", BindingFlags.NonPublic | BindingFlags.Instance);
        }

        [PatchPostfix]
        private static void PatchPostfix(Player.FirearmController __instance, Player ____player)
        {
            Player yourPlayer = GamePanelHUDCorePlugin.HUDCore.YourPlayer;

            if (____player != yourPlayer)
            {
                Vector3 weaponPosition = __instance.WeaponRoot.position;

                GamePanelHUDCompassPlugin.CompassFireInfo fireInfo = new GamePanelHUDCompassPlugin.CompassFireInfo()
                {
                    Who = ____player.Id,
                    Where = weaponPosition,
                    IsSilenced = __instance.IsSilenced,
                    Distance = Vector3.Distance(weaponPosition, yourPlayer.Position)
                };

                Test.Add(fireInfo);

                GamePanelHUDCompassPlugin.ShowFire(fireInfo);
            }
        }
    }
}
#endif
