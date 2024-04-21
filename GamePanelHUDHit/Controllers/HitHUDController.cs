using UnityEngine;
#if !UNITY_EDITOR
using EFTUtils;
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

            hitHUDModel.HitHUDSw = hudCoreModel.AllHUDSw && settingsModel.KeyHitHUDSw.Value && hudCoreModel.HasPlayer;

            if (!hudCoreModel.HasPlayer)
            {
                ArmorModel.Instance.Reset();
            }
        }

#endif
    }
}