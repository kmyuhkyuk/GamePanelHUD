#if !UNITY_EDITOR

using System.Threading.Tasks;
using GamePanelHUDHealth.Models;

namespace GamePanelHUDHealth
{
    public partial class GamePanelHUDHealthPlugin
    {
        private static async void Execute(Task<MainMenuController> __result)
        {
            HealthHUDModel.Instance.HealthController =
                ReflectionModel.Instance.RefHealthController.GetValue(await __result);
        }
    }
}

#endif