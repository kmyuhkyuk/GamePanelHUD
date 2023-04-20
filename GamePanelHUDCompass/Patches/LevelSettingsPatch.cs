#if !UNITY_EDITOR
using Aki.Reflection.Patching;
using System.Reflection;

namespace GamePanelHUDCompass.Patches
{
    public class LevelSettingsPatch : ModulePatch
    {
        protected override MethodBase GetTargetMethod()
        {
            return typeof(LevelSettings).GetMethod("Awake", BindingFlags.NonPublic | BindingFlags.Instance);
        }

        [PatchPostfix]
        private static void PatchPostfix(LevelSettings __instance)
        {
            GamePanelHUDCompassPlugin.NorthDirection = __instance.NorthDirection;
            GamePanelHUDCompassPlugin.NorthVector = __instance.NorthVector; 
        }
    }
}
#endif
