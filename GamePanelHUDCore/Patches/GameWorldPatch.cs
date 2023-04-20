#if !UNITY_EDITOR
using Aki.Reflection.Patching;
using System.Reflection;
using EFT;

namespace GamePanelHUDCore.Patches
{
    public class GameWorldAwakePatch : ModulePatch
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

    public class GameWorldOnGameStartedPatch : ModulePatch
    {
        protected override MethodBase GetTargetMethod()
        {
            return typeof(GameWorld).GetMethod("OnGameStarted", BindingFlags.Public | BindingFlags.Instance);
        }

        [PatchPostfix]
        private static void PatchPostfix(GameWorld __instance)
        {
            GamePanelHUDCorePlugin.HUDCoreClass.GameWorldStart(__instance);
        }
    }

    public class GameWorldDisposePatch : ModulePatch
    {
        protected override MethodBase GetTargetMethod()
        {
            return typeof(GameWorld).GetMethod("Dispose", BindingFlags.Public | BindingFlags.Instance);
        }

        [PatchPostfix]
        private static void PatchPostfix(GameWorld __instance)
        {
            GamePanelHUDCorePlugin.HUDCoreClass.GameWorldDispose(__instance);
        }
    }
}
#endif
