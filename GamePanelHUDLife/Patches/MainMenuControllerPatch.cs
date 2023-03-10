#if !UNITY_EDITOR
using Aki.Reflection.Patching;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace GamePanelHUDLife.Patches
{
    public class MainMenuControllerPatch : ModulePatch
    {
        private static MethodInfo fesf = typeof(MainMenuController).GetMethods(BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly).Where(x => x.ReturnType == typeof(Task)).ElementAt(0);

        protected override MethodBase GetTargetMethod()
        {
            return typeof(MainMenuController).GetMethods(BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly).Where(x => x.ReturnType == typeof(Task)).ElementAt(1);
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
