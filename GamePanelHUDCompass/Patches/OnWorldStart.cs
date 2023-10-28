#if !UNITY_EDITOR

using EFT;
using GamePanelHUDCompass.Models;

namespace GamePanelHUDCompass
{
    public partial class GamePanelHUDCompassPlugin
    {
        private static void OnWorldStart(GameWorld __instance)
        {
            CompassStaticHUDModel.Instance.CompassStaticCacheBool = true;
        }
    }
}

#endif