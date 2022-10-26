using Aki.Reflection.Patching;
using Aki.Reflection.Utils;
using System.Reflection;
using EFT.UI;

namespace GamePanelHUDCore.Patches
{
    public class InventoryScreenPatch : ModulePatch
    {
        protected override MethodBase GetTargetMethod()
        {
            return typeof(InventoryScreen).GetMethod("Awake", PatchConstants.PrivateFlags);
        }

        [PatchPostfix]
        private static void PatchPostfix(InventoryScreen __instance)
        {
            GamePanelHUDCorePlugin.InventoryScreen = __instance.gameObject;
        }
    }
}
