using HarmonyLib;
using System;
using System.Reflection;
using EFT.InventoryLogic;

namespace GamePanelHUDCore.Utils
{
    public class GetMag
    {
        private static Func<Weapon, object> RefGetCurrentMagazine;

        public static void Init()
        {
            RefGetCurrentMagazine = AccessTools.MethodDelegate<Func<Weapon, object>>(typeof(Weapon).GetMethod("GetCurrentMagazine", BindingFlags.Public | BindingFlags.Instance));
        }

        public static object GetCurrentMagazine(Weapon weapon)
        {
            return RefGetCurrentMagazine(weapon);
        }
    }
}
