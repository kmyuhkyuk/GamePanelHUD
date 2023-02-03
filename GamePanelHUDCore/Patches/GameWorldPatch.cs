using Aki.Reflection.Patching;
using System.Reflection;
using EFT;

namespace GamePanelHUDCore.Patches
{
    public class GameWorldPatch : ModulePatch
    {
        protected override MethodBase GetTargetMethod()
        {
            return typeof(GameWorld).GetMethod("Awake", BindingFlags.NonPublic | BindingFlags.Instance);
        }

        [PatchPostfix]
        private static void PatchPostfix(GameWorld __instance)
        {
            GamePanelHUDCorePlugin.TheWorld = __instance;
        }
    }
}
