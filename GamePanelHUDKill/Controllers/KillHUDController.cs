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
                hudCoreModel.AllHUDSw && hudCoreModel.HasPlayer && settingsModel.KeyKillHUDSw.Value;
            killHUDModel.ExpHUDSw = hudCoreModel.AllHUDSw && hudCoreModel.HasPlayer && settingsModel.KeyExpHUDSw.Value;

            if (!hudCoreModel.HasPlayer)
            {
                killHUDModel.KillCount = 0;
            }
        }
#endif
    }
}