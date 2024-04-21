using UnityEngine;
#if !UNITY_EDITOR
using EFTUtils;
using GamePanelHUDCore.Models;
using GamePanelHUDKill.Models;
using SettingsModel = GamePanelHUDKill.Models.SettingsModel;

#endif

namespace GamePanelHUDKill.Controllers
{
    public class KillHUDController : MonoBehaviour
#if !UNITY_EDITOR
        , IUpdate
#endif
    {
#if !UNITY_EDITOR
        private void Start()
        {
            HUDCoreModel.Instance.UpdateManger.Register(this);
        }

        public void CustomUpdate()
        {
            var hudCoreModel = HUDCoreModel.Instance;
            var killHUDModel = KillHUDModel.Instance;
            var settingsModel = SettingsModel.Instance;

            killHUDModel.KillHUDSw =
                hudCoreModel.AllHUDSw && settingsModel.KeyKillHUDSw.Value && hudCoreModel.HasPlayer;
            killHUDModel.ExpHUDSw = hudCoreModel.AllHUDSw && settingsModel.KeyExpHUDSw.Value && hudCoreModel.HasPlayer;

            if (!hudCoreModel.HasPlayer)
            {
                killHUDModel.KillCount = 0;
            }
        }
#endif
    }
}