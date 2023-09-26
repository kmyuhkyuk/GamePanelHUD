#if !UNITY_EDITOR

using EFT;
using UnityEngine;
using static EFTApi.EFTHelpers;

namespace GamePanelHUDCompass
{
    public partial class GamePanelHUDCompassPlugin
    {
        private static void InitiateShot(Player.FirearmController __instance, Player ____player, Vector3 shotPosition)
        {
            if (____player != HUDCore.YourPlayer)
            {
                var fireInfo = new CompassFireInfo
                {
                    Who = ____player.ProfileId,
                    Where = shotPosition,
                    Role = _PlayerHelper.RefRole.GetValue(_PlayerHelper.RefSettings.GetValue(____player.Profile.Info)),
                    IsSilenced = __instance.IsSilenced && !__instance.IsInLauncherMode(),
                    Distance = Vector3.Distance(shotPosition, HUDCore.YourPlayer.CameraPosition.position)
                };

                ShowFire(fireInfo);
            }
        }
    }
}

#endif