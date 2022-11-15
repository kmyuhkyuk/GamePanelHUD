#if !UNITY_EDITOR
using Aki.Reflection.Patching;
using HarmonyLib;
using System.Linq;
using System.Reflection;
using EFT;
using GamePanelHUDCore.Utils;

namespace GamePanelHUDCore.Patches
{
    public class MainApplicationPatch : ModulePatch
    {
        protected override MethodBase GetTargetMethod()
        {
            return typeof(MainApplication).GetMethods(BindingFlags.DeclaredOnly | BindingFlags.NonPublic | BindingFlags.Instance).Single(x => x.IsAssembly);
        }

        [PatchPostfix]
        private static void PatchPostfix(object ____backEnd)
        {
            ISessionHelp.Init(Traverse.Create(____backEnd).Property("Session").GetValue<ISession>());
        }
    }
}
#endif
