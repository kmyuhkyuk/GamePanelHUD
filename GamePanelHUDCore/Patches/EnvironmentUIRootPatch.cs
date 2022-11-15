#if !UNITY_EDITOR
using Aki.Reflection.Patching;
using System.Reflection;
using EFT.UI;

namespace GamePanelHUDCore.Patches
{
    public class EnvironmentUIRootPatch : ModulePatch
    {
        protected override MethodBase GetTargetMethod()
        {
            return typeof(EnvironmentUIRoot).GetMethod("Init", BindingFlags.Public | BindingFlags.Instance);
        }

        [PatchPostfix]
        private static void PatchPostfix(EnvironmentUIRoot __instance)
        {
            GamePanelHUDCorePlugin.EnvironmentUIRoot = __instance.gameObject;
        }
    }
}
#endif
