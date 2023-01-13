#if !UNITY_EDITOR
using Aki.Reflection.Patching;
using HarmonyLib;
using System;
using System.Reflection;
using GamePanelHUDCore.Utils;

namespace GamePanelHUDCore.Patches
{
    public class MainApplicationPatch : ModulePatch
    {
        private static readonly bool Is330Up = GamePanelHUDCorePlugin.HUDCoreClass.GameVersion > new Version("0.12.12.20243");

        private static readonly MethodBase MainApplicationBase;

        static MainApplicationPatch()
        {
            BindingFlags flags = BindingFlags.DeclaredOnly | BindingFlags.NonPublic | BindingFlags.Instance;

            if (Is330Up)
            {
                MainApplicationBase = RefHelp.GetEftMethod(x => x.Name == "TarkovApplication", flags, x => x.IsAssembly);
            }
            else
            {
                MainApplicationBase = RefHelp.GetEftMethod(x => x.Name == "MainApplication", flags, x => x.IsAssembly);
            }
        }

        protected override MethodBase GetTargetMethod()
        {
            return MainApplicationBase;
        }

        [PatchPostfix]
        private static void PatchPostfix(object __instance)
        {
            object backEnd;

            if (Is330Up)
            {
                backEnd = Traverse.Create(__instance).Field("ClientBackEnd").GetValue<object>();
            }
            else
            {
                backEnd = Traverse.Create(__instance).Field("_backEnd").GetValue<object>();
            }

            ISessionHelp.Init(Traverse.Create(backEnd).Property("Session").GetValue<ISession>());
        }
    }
}
#endif
