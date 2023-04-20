#if !UNITY_EDITOR
using Aki.Reflection.Patching;
using System;
using System.Reflection;
using System.Threading.Tasks;
using EFT;

//using Aki.SinglePlayer.Patches.Healing.PlayerPatch

namespace GamePanelHUDCore.Patches
{
    public class PlayerPatch : ModulePatch
    {
        private static readonly bool Is231Up = GamePanelHUDCorePlugin.HUDCoreClass.GameVersion > new Version("0.12.12.17349");

        protected override MethodBase GetTargetMethod()
        {
            return typeof(Player).GetMethod("Init", BindingFlags.NonPublic | BindingFlags.Instance);
        }

        [PatchPostfix]
        private static async void PatchPostfix(Task __result, Player __instance)
        {
            await __result;

            bool isYouPlayer;

            if (Is231Up)
            {
                isYouPlayer = __instance.IsYourPlayer;
            }
            else
            {
                isYouPlayer = __instance.Id == 1;
            }

            if (isYouPlayer)
            {
                GamePanelHUDCorePlugin.YourPlayer = __instance;
            }
        }
    }
}
#endif
