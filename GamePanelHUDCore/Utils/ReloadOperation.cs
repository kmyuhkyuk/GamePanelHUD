using HarmonyLib;
using System;
using System.Reflection;
using EFT;

namespace GamePanelHUDCore.Utils
{
    public class ReloadOperation
    {
        private static Func<Player.FirearmController, bool> RefReloadOperation;

        public static void Init()
        {
            RefReloadOperation = AccessTools.MethodDelegate<Func<Player.FirearmController, bool>>(typeof(Player.FirearmController).GetMethod("IsInReloadOperation", BindingFlags.Public | BindingFlags.Instance));
        }

        public static bool IsInReloadOperation(Player.FirearmController firearmcontroller)
        {
            return RefReloadOperation(firearmcontroller);
        }
    }
}
