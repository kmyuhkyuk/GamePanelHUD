#if !UNITY_EDITOR
using Aki.Reflection.Patching;
using HarmonyLib;
using System.Linq;
using System.Reflection;
using EFT;
using GamePanelHUDCore.Utils;
using Aki.Reflection.Utils;
using System;

namespace GamePanelHUDCore.Patches
{
    public class MainApplicationPatch : ModulePatch
    {
        private static readonly bool Is330Up;

        private static readonly MethodBase MainApplication;

        static MainApplicationPatch()
        {
            Type mainApp = PatchConstants.EftTypes.SingleOrDefault(x => x.Name == "MainApplication");

            Is330Up = mainApp == null;

            if (Is330Up)
            {
                MainApplication = typeof(TarkovApplication).GetMethods(BindingFlags.DeclaredOnly | BindingFlags.NonPublic | BindingFlags.Instance).Single(x => x.IsAssembly);
            }
            else
            {
                MainApplication = mainApp.GetMethods(BindingFlags.DeclaredOnly | BindingFlags.NonPublic | BindingFlags.Instance).Single(x => x.IsAssembly);
            }
        }

        protected override MethodBase GetTargetMethod()
        {
            return MainApplication;
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
