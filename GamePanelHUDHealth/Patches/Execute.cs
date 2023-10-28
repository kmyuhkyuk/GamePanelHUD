#if !UNITY_EDITOR

using System.Threading.Tasks;
using GamePanelHUDHealth.Models;
using HarmonyLib;

namespace GamePanelHUDHealth
{
    public partial class GamePanelHUDHealthPlugin
    {
        private static async void Execute(Task<MainMenuController> __result)
        {
            HealthHUDModel.Instance.HealthController =
                Traverse.Create(await __result).Property("HealthController").GetValue<object>();
        }
    }
}

#endif