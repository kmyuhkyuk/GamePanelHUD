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

            /*if (____player != yourPlayer))
            {
                GamePanelHUDCompassPlugin.CompassFireInfo fireInfo = new GamePanelHUDCompassPlugin.CompassFireInfo()
                {
                    WhoFire = ____player,
                    WhereFire = __instance.WeaponRoot.position
                };

                Test.Add(fireInfo);
            }*/

            Vector3 weaponPosition = __instance.WeaponRoot.position;

            GamePanelHUDCompassPlugin.CompassFireInfo fireInfo = new GamePanelHUDCompassPlugin.CompassFireInfo()
            {
                Who = ____player,
                Where = weaponPosition,
                IsSilenced = __instance.IsSilenced,
                Distance = Vector3.Distance(weaponPosition, yourPlayer.Position)
            };

            Test.Add(fireInfo);
        }
    }
}
