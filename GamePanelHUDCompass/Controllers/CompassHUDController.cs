﻿using UnityEngine;
#if !UNITY_EDITOR
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

        private void Start()
        {
            HUDCoreModel.Instance.UpdateManger.Register(this);
        }

        public void CustomUpdate()
        {
            var hudCoreModel = HUDCoreModel.Instance;
            var compassHUDModel = CompassHUDModel.Instance;
            var settingsModel = SettingsModel.Instance;

            compassHUDModel.CompassHUDSw = hudCoreModel.AllHUDSw && compassHUDModel.CamTransform != null &&
                                           hudCoreModel.HasPlayer &&
                                           settingsModel.KeyCompassHUDSw.Value;

            compassHUDModel.Compass.SizeDelta = settingsModel.KeyAutoSizeDelta.Value
                ? new Vector2(
                    compassHUDModel.ScreenRect.sizeDelta.x * ((float)settingsModel.KeyAutoSizeDeltaRate.Value / 100),
                    settingsModel.KeySizeDelta.Value.y)
                : settingsModel.KeySizeDelta.Value;

            if (hudCoreModel.HasPlayer)
            {
                compassHUDModel.CamTransform = hudCoreModel.YourPlayer.CameraPosition;

                compassHUDModel.Compass.Angle = GetAngle(compassHUDModel.CamTransform.eulerAngles,
                    EFTGlobal.LevelSettings.NorthDirection);
            }
        }

        private static float GetAngle(Vector3 eulerAngles, float northDirection)
        {
            var num = eulerAngles.y - northDirection;

            if (num >= 0)
                return num;
            else
                return num + 360;
        }

#endif
    }
}