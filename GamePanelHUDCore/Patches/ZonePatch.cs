#if !UNITY_EDITOR
using Aki.Reflection.Patching;
using System.Reflection;
using System.Collections.Generic;
using EFT.Interactive;
using GamePanelHUDCore.Utils.Zone;

namespace GamePanelHUDCore.Patches
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
            ZoneHelp.AddPoint(__instance);
            Test.Add(__instance);
        }
    }

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
            ZoneHelp.AddPoint(__instance);

            Test.Add(__instance);
        }
    }
}
#endif
