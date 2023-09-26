#if !UNITY_EDITOR

using System.Threading.Tasks;

namespace GamePanelHUDLife
{
    public partial class GamePanelHUDLifePlugin
    {
        private static async void Execute(Task<MainMenuController> __result)
        {
            _healthController = (await __result).HealthController;
        }
    }
}

#endif