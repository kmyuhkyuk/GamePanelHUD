#if !UNITY_EDITOR
using Aki.Reflection.Patching;
using System.Reflection;
using System.Collections.Generic;
using EFT.Interactive;
using GamePanelHUDCore.Utils.Zone;
using GamePanelHUDCore.Patches.Ex;

namespace GamePanelHUDCore.Patches
{
    public class TriggerWithIdPatch : ModulePatchs
    {
        protected override IEnumerable<MethodBase> GetTargetMethods()
        {
            yield return typeof(TriggerWithId).GetMethod("Awake", BindingFlags.NonPublic | BindingFlags.Instance);
            yield return typeof(ExperienceTrigger).GetMethod("Awake", BindingFlags.NonPublic | BindingFlags.Instance);
        }

        [PatchPostfix]
        private static void PatchPostfix(TriggerWithId __instance)
        {
            ZoneHelp.TriggerPoints.Add(__instance);
        }
    }
}
#endif
