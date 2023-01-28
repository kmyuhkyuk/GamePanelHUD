using Aki.Reflection.Patching;
using System.Reflection;
using System.Collections.Generic;
using EFT.Interactive;


namespace GamePanelHUDCompass.Patches
{
    public class ExperienceTriggerPatch : ModulePatch
    {
        private static List<ExperienceTrigger> Test = new List<ExperienceTrigger>();

        protected override MethodBase GetTargetMethod()
        {
            return typeof(ExperienceTrigger).GetMethod("Awake", BindingFlags.NonPublic | BindingFlags.Instance);
        }

        [PatchPostfix]
        private static void PatchPostfix(ExperienceTrigger __instance)
        {
            GamePanelHUDCompassPlugin.AddTrigger(__instance);
            Test.Add(__instance);
        }
    }
}
