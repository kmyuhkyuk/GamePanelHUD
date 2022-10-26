using HarmonyLib;
using System;
using System.Reflection;
using EFT;

namespace GamePanelHUDCore.Utils
{
    public class RoleHelp
    {
        private static Type RoleHelpType;

        private static Func<WildSpawnType, bool> RefIsBoss;

        private static Func<WildSpawnType, bool> RefIsFollower;

        private static Func<WildSpawnType, bool> RefIsBossOrFollower;

        private static Func<WildSpawnType, string> RefGetScavRoleKey;

        public static void Init()
        {
            BindingFlags flags = BindingFlags.Public | BindingFlags.Static | BindingFlags.Instance;

            RoleHelpType = RefHelp.GetEftType(x => x.GetMethod("IsBoss", flags) != null && x.GetMethod("Init", flags) != null);

            RefIsBoss = AccessTools.MethodDelegate<Func<WildSpawnType, bool>>(RoleHelpType.GetMethod("IsBoss", flags));

            RefIsFollower = AccessTools.MethodDelegate<Func<WildSpawnType, bool>>(RoleHelpType.GetMethod("IsFollower", flags));

            RefIsBossOrFollower = AccessTools.MethodDelegate<Func<WildSpawnType, bool>>(RoleHelpType.GetMethod("IsBossOrFollower", flags));

            RefGetScavRoleKey = AccessTools.MethodDelegate<Func<WildSpawnType, string>>(RoleHelpType.GetMethod("GetScavRoleKey", flags));
        }

        public static bool IsBoss(WildSpawnType role)
        {
            return RefIsBoss(role);
        }

        public static bool IsFollower(WildSpawnType role)
        {
            return RefIsFollower.Invoke(role);
        }

        public static bool IsBossOrFollower(WildSpawnType role)
        {
            return RefIsBossOrFollower(role);
        }

        public static string GetScavRoleKey(WildSpawnType role)
        {
            return RefGetScavRoleKey(role);
        }
    }
}
