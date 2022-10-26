#if !UNITY_EDITOR
using Aki.Reflection.Patching;
using Aki.Reflection.Utils;
using System.Reflection;
using System.Threading.Tasks;

namespace GamePanelHUDLife.Patches
{
    public class MainMenuControllerPatch : ModulePatch
    {
        protected override MethodBase GetTargetMethod()
        {
            return typeof(MainMenuController).GetMethod("method_4", PatchConstants.PrivateFlags);
        }

        [PatchPostfix]
        private static async void PatchPostfix(Task __result, MainMenuController __instance)
        {
            await __result;

            GamePanelHUDLifePlugin.HealthController = __instance.HealthController;
        }
    }
}
#endif
