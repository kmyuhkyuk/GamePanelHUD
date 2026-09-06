#if !UNITY_EDITOR

using System.Threading.Tasks;
using EFT;
using GamePanelHUDHealth.Models;

namespace GamePanelHUDHealth
{
    public partial class GamePanelHUDHealthPlugin
    {
        private static async void Execute(Task<MainMenuShowOperation> __result)
        {
            var mainMenuControllerClass = await __result;

            HealthHUDModel.Instance.HealthController = mainMenuControllerClass.HealthController;
        }
    }
}

#endif
