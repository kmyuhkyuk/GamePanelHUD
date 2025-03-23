#if !UNITY_EDITOR

using EFT;
using GamePanelHUDCompass.Models;

namespace GamePanelHUDCompass
{
    public partial class GamePanelHUDCompassPlugin
    {
        private static void SetPropVisibility(Player __instance, bool isVisible)
        {
            if (__instance.IsYourPlayer)
            {
                CompassHUDModel.Instance.SetCompassState(isVisible);
            }
        }
    }
}

#endif