#if !UNITY_EDITOR
using Aki.Reflection.Patching;
using Aki.Reflection.Utils;
using HarmonyLib;
using System;
using System.Linq;
using System.Reflection;
using GamePanelHUDCore.Utils;

namespace GamePanelHUDCore.Patches
{
    public class MainApplicationPatch : ModulePatch
    {
        private static readonly bool Is330Up = GamePanelHUDCorePlugin.GameVersion > new Version("0.12.12.20243");

        private static readonly MethodBase MainApplicationBase;

        static MainApplicationPatch()
        {
            if (Is330Up)
            {
                MainApplicationBase = PatchConstants.EftTypes.Single(x => x.Name == "TarkovApplication").GetMethods(BindingFlags.DeclaredOnly | BindingFlags.NonPublic | BindingFlags.Instance).Single(x => x.IsAssembly);
            }
            else
            {
                MainApplicationBase = PatchConstants.EftTypes.Single(x => x.Name == "MainApplication").GetMethods(BindingFlags.DeclaredOnly | BindingFlags.NonPublic | BindingFlags.Instance).Single(x => x.IsAssembly);
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
