#if !UNITY_EDITOR
using Aki.Reflection.Patching;
using System.Reflection;
using EFT;

namespace GamePanelHUDCompass.Patches
{
    public class OnBeenKilledByAggressorPatch : ModulePatch
    {
        protected override MethodBase GetTargetMethod()
        {
            return typeof(Player).GetMethod("OnBeenKilledByAggressor", BindingFlags.NonPublic | BindingFlags.Instance);
        }

        [PatchPostfix]
        private static void PatchPostfix(Player __instance)
        {
            GamePanelHUDCompassPlugin.DestroyFire(__instance.Id);
        }
    }
}
#endif
