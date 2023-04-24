using BepInEx;

namespace GamePanelHUDDebug
{
    [BepInPlugin("com.kmyuhkyuk.GamePanelHUDDebug", "kmyuhkyuk-GamePanelHUDDebug", "2.5.3")]
    [BepInDependency("com.kmyuhkyuk.GamePanelHUDCore")]
    public class GamePanelHUDDebugPlugin : BaseUnityPlugin
    {
        private void Start()
        {
            Logger.LogInfo("Loaded: kmyuhkyuk-GamePanelHUDDebug");
        }
    }
}
