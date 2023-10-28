using UnityEngine;
#if !UNITY_EDITOR
using EFTApi;
using EFTUtils;
using GamePanelHUDCore.Models;
using GamePanelHUDCompass.Models;
using SettingsModel = GamePanelHUDCompass.Models.SettingsModel;
#endif

namespace GamePanelHUDCompass.Controllers
{
    public class CompassFireHUDController : MonoBehaviour
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
            var compassHUDModel = CompassHUDModel.Instance;
            var compassFireHUDModel = CompassFireHUDModel.Instance;
            var settingsModel = SettingsModel.Instance;

            compassFireHUDModel.CompassFireHUDSw =
                compassHUDModel.CompassHUDSw && settingsModel.KeyCompassFireHUDSw.Value;

            if (hudCoreModel.HasPlayer)
            {
                compassFireHUDModel.CompassFire.NorthVector = EFTGlobal.LevelSettings.NorthVector;

                compassFireHUDModel.CompassFire.CameraPosition = compassHUDModel.CamTransform.position;

                compassFireHUDModel.CompassFire.PlayerRight = compassHUDModel.CamTransform.right;
            }
        }

#endif
    }
}