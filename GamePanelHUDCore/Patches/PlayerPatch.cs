#if !UNITY_EDITOR
using Aki.Reflection.Patching;
using Aki.Reflection.Utils;
using System;
using System.Reflection;
using System.Threading.Tasks;
using Comfort.Common;
using EFT;

//using Aki.SinglePlayer.Patches.Healing.PlayerPatch

namespace GamePanelHUDCore.Patches
{
    public class PlayerPatch : ModulePatch
    {
        private static readonly bool Is231Up = GamePanelHUDCorePlugin.GameVersion > new Version("0.12.12.17349");

        protected override MethodBase GetTargetMethod()
        {
            return typeof(Player).GetMethod("Init", PatchConstants.PrivateFlags);
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
                isYouPlayer = __instance == Singleton<GameWorld>.Instance.AllPlayers[0];
            }

            if (isYouPlayer)
            {
                GamePanelHUDCorePlugin.IsYourPlayer = __instance;
            }
        }
    }
}
#endif
