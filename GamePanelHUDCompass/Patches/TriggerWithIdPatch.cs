using Aki.Reflection.Patching;
using EFT;
using EFT.Interactive;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

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
