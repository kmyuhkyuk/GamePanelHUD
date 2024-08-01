using UnityEngine;
#if !UNITY_EDITOR
using System.Collections;
using EFTApi;
using EFTUtils;
using GamePanelHUDCore.Models;
using GamePanelHUDCompass.Models;
using SettingsModel = GamePanelHUDCompass.Models.SettingsModel;

#endif

namespace GamePanelHUDCompass.Controllers
{
    public class CompassHUDController : MonoBehaviour
#if !UNITY_EDITOR
        , IUpdate
#endif
    {
#if !UNITY_EDITOR

        private Coroutine _compassStateCoroutine;

        private void Awake()
        {
            CompassHUDModel.Instance.SetCompassState = SetCompassState;
        }

        private void Start()
        {
            HUDCoreModel.Instance.UpdateManger.Register(this);
        }

        public void CustomUpdate()
        {
            var hudCoreModel = HUDCoreModel.Instance;
            var compassHUDModel = CompassHUDModel.Instance;
            var settingsModel = SettingsModel.Instance;

            var hasPlayer = hudCoreModel.HasPlayer;

            compassHUDModel.CompassHUDSw = hudCoreModel.AllHUDSw && compassHUDModel.CamTransform != null &&
                                           hasPlayer &&
                                           settingsModel.KeyCompassHUDSw.Value;

            compassHUDModel.Compass.SizeDelta = settingsModel.KeyCompassAutoSizeDelta.Value
                ? new Vector2(
                    compassHUDModel.ScreenRect.sizeDelta.x * ((float)settingsModel.KeyAutoSizeDeltaRate.Value / 100),
                    settingsModel.KeySizeDelta.Value.y)
                : settingsModel.KeySizeDelta.Value;

            if (!hasPlayer)
                return;

            compassHUDModel.CamTransform = hudCoreModel.YourPlayer.CameraPosition;

            compassHUDModel.Compass.Angle = GetAngle(compassHUDModel.CamTransform.eulerAngles,
                EFTGlobal.LevelSettings.NorthDirection);
        }

        private void SetCompassState(bool isVisible)
        {
            if (_compassStateCoroutine != null)
            {
                StopCoroutine(_compassStateCoroutine);
            }

            _compassStateCoroutine =
                StartCoroutine(ChangeCompassState(isVisible,
                    SettingsModel.Instance.KeyImmersiveCompassWaitTime.Value));
        }

        private static IEnumerator ChangeCompassState(bool isVisible, float waitTime)
        {
            if (isVisible)
            {
                yield return new WaitForSeconds(waitTime);
            }

            CompassHUDModel.Instance.Compass.CompassState = isVisible;
        }

        private static float GetAngle(Vector3 eulerAngles, float northDirection)
        {
            var num = eulerAngles.y - northDirection;

            if (num >= 0)
                return num;

            return num + 360;
        }

#endif
    }
}