using Aki.Reflection.Patching;
using Aki.Reflection.Utils;
using System.Reflection;
using EFT.Hideout;

namespace GamePanelHUDCore.Patches
{
    public class HideoutScreenOverlayPatch : ModulePatch
    {
        protected override MethodBase GetTargetMethod()
        {
            return typeof(HideoutScreenOverlay).GetMethod("Awake", PatchConstants.PrivateFlags);
        }

        [PatchPostfix]
        private static void PatchPostfix(HideoutScreenOverlay __instance)
        {
            GamePanelHUDCorePlugin.HideoutScreenOverlay = __instance.gameObject;
        }
    }
}
