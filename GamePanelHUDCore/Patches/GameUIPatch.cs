using Aki.Reflection.Patching;
using System.Reflection;
using EFT.UI;

namespace GamePanelHUDCore.Patches
{
    public class GameUIPatch : ModulePatch
    {
        protected override MethodBase GetTargetMethod()
        {
            return typeof(GameUI).GetMethod("Awake", BindingFlags.Public | BindingFlags.Instance);
        }

        [PatchPostfix]
        private static void PatchPostfix(GameUI __instance, BattleUIScreen ___BattleUiScreen)
        {
            GamePanelHUDCorePlugin.BattleUiScreen = ___BattleUiScreen.gameObject;
        }
    }
}
