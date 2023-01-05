using HarmonyLib;
using System;
using System.Reflection;
using EFT;

namespace GamePanelHUDCore.Utils
{
    public class LauncherMode
    {
        private static Func<Player.FirearmController, bool> RefIsInLauncherMode;

        public static void Init()
        {
            RefIsInLauncherMode = AccessTools.MethodDelegate<Func<Player.FirearmController, bool>>(typeof(Player.FirearmController).GetMethod("IsInLauncherMode", BindingFlags.Public | BindingFlags.Instance));
        }

        public static bool IsInLauncherMode(Player.FirearmController firearmcontroller)
        {
            return RefIsInLauncherMode(firearmcontroller);
        }
    }
}
