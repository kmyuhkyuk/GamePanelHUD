#if !UNITY_EDITOR
using Aki.Reflection.Patching;
using Aki.Reflection.Utils;
using System.Reflection;

namespace GamePanelHUDCompass.Patches
{
    public class LevelSettingsPatch : ModulePatch
    {
        protected override MethodBase GetTargetMethod()
        {
            return typeof(LevelSettings).GetMethod("Awake", PatchConstants.PrivateFlags);
        }

        [PatchPostfix]
        private static void PatchPostfix(LevelSettings __instance)
        {
            GamePanelHUDCompassPlugin.NorthDirection = __instance.NorthDirection;
        }
    }
}
#endif
