#if !UNITY_EDITOR

using System.Threading.Tasks;
using static EFTApi.EFTHelpers;

namespace GamePanelHUDHealth
{
    public partial class GamePanelHUDHealthPlugin
    {
        private static async void Execute(Task<MainMenuController> __result)
        {
            _MainMenuControllerHelper.RefHealthController.GetValue(await __result);
        }
    }
}

#endif