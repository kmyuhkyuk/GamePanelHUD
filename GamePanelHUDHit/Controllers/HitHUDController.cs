using UnityEngine;
#if !UNITY_EDITOR
using KmyTarkovUtils;
using GamePanelHUDCore.Models;
using GamePanelHUDHit.Models;
using SettingsModel = GamePanelHUDHit.Models.SettingsModel;

#endif

namespace GamePanelHUDHit.Controllers
{
    public class HitHUDController : MonoBehaviour
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
            var hitHUDModel = HitHUDModel.Instance;
            var settingsModel = SettingsModel.Instance;

            var hasPlayer = hudCoreModel.HasPlayer;

            hitHUDModel.HitHUDSw = hudCoreModel.AllHUDSw && hasPlayer && settingsModel.KeyHitHUDSw.Value;

            if (!hasPlayer)
            {
                ArmorModel.Instance.Reset();
            }
        }

#endif
    }
}