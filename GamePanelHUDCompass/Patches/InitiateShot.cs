#if !UNITY_EDITOR

using EFT;
using GamePanelHUDCompass.Models;
using GamePanelHUDCore.Models;
using UnityEngine;

namespace GamePanelHUDCompass
{
    public partial class GamePanelHUDCompassPlugin
    {
        private static void InitiateShot(Player.FirearmController __instance, Player ____player, Vector3 shotPosition)
        {
            var hudCoreModel = HUDCoreModel.Instance;

            if (____player == hudCoreModel.YourPlayer)
                return;

            var settings = ____player.Profile.Info.Settings;

            var fireModel = new FireModel
            {
                Who = ____player.ProfileId,
                Where = shotPosition,
                Role = settings.Role,
                IsSilenced = __instance.IsSilenced && !__instance.IsInLauncherMode(),
                Distance = Vector3.Distance(shotPosition, hudCoreModel.YourPlayer.CameraPosition.position)
            };

            CompassFireHUDModel.Instance.ShowFire(fireModel);
        }
    }
}

#endif