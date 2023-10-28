#if !UNITY_EDITOR

using EFT;
using GamePanelHUDCompass.Models;
using GamePanelHUDCore.Models;

namespace GamePanelHUDCompass
{
    public partial class GamePanelHUDCompassPlugin
    {
        private static void OnDead(Player __instance)
        {
            if (__instance != HUDCoreModel.Instance.YourPlayer)
            {
                CompassFireHUDModel.Instance.DestroyFire(__instance.ProfileId);
            }
        }
    }
}

#endif