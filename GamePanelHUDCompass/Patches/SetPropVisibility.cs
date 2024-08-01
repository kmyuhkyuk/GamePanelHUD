#if !UNITY_EDITOR

using EFT;
using GamePanelHUDCompass.Models;
using static EFTApi.EFTHelpers;

namespace GamePanelHUDCompass
{
    public partial class GamePanelHUDCompassPlugin
    {
        private static void SetPropVisibility(Player __instance, bool isVisible)
        {
            if (_PlayerHelper.IsYourPlayer(__instance))
            {
                CompassHUDModel.Instance.SetCompassState(isVisible);
            }
        }
    }
}

#endif