#if !UNITY_EDITOR

using System.Threading.Tasks;
using HarmonyLib;

namespace GamePanelHUDLife
{
    public partial class GamePanelHUDLifePlugin
    {
        private static async void Execute(Task<MainMenuController> __result)
        {
            _healthController = Traverse.Create(await __result).Property("HealthController").GetValue<object>();
        }
    }
}

#endif