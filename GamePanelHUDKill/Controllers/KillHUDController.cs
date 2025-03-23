using UnityEngine;
#if !UNITY_EDITOR
using KmyTarkovUtils;
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

            var hasPlayer = hudCoreModel.HasPlayer;

            killHUDModel.KillHUDSw =
                hudCoreModel.AllHUDSw && hasPlayer && settingsModel.KeyKillHUDSw.Value;
            killHUDModel.ExpHUDSw = hudCoreModel.AllHUDSw && hasPlayer && settingsModel.KeyExpHUDSw.Value;

            if (!hasPlayer)
            {
                killHUDModel.KillCount = 0;
            }
        }

#endif
    }
}