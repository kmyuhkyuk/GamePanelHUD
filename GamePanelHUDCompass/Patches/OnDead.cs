using EFT;

namespace GamePanelHUDCompass
{
    public partial class GamePanelHUDCompassPlugin
    {
        private static void OnDead(Player __instance)
        {
            if (__instance != HUDCore.YourPlayer)
            {
                DestroyFire(__instance.ProfileId);
            }
        }
    }
}