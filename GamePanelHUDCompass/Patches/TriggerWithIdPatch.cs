#if !UNITY_EDITOR
using Aki.Reflection.Patching;
using EFT.Interactive;
using System.Reflection;
using System.Collections.Generic;

namespace GamePanelHUDCompass.Patches
{
    public class TriggerWithIdPatch : ModulePatch
    {
        private static List<TriggerWithId> Test = new List<TriggerWithId>();

        protected override MethodBase GetTargetMethod()
        {
            return typeof(TriggerWithId).GetMethod("Awake", BindingFlags.NonPublic | BindingFlags.Instance);
        }

        [PatchPostfix]
        private static void PatchPostfix(TriggerWithId __instance)
        {
            GamePanelHUDCompassPlugin.AddTrigger(__instance);

            Test.Add(__instance);
        }
    }
}
#endif
