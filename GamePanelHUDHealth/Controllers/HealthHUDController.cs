﻿using UnityEngine;
#if !UNITY_EDITOR
using KmyTarkovApi;
using GamePanelHUDCore.Models;
using GamePanelHUDHealth.Models;
using SettingsModel = GamePanelHUDHealth.Models.SettingsModel;
using KmyTarkovUtils;
using static KmyTarkovApi.EFTHelpers;

#endif

namespace GamePanelHUDHealth.Controllers
{
    public class HealthHUDController : MonoBehaviour
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
            var healthHUDModel = HealthHUDModel.Instance;
            var settingsModel = SettingsModel.Instance;

            var hasPlayer = hudCoreModel.HasPlayer;

            healthHUDModel.HealthHUDSw = hudCoreModel.AllHUDSw && healthHUDModel.HealthController != null &&
                                         hasPlayer &&
                                         settingsModel.KeyHealthHUDSw.Value;

            if (hasPlayer)
            {
                healthHUDModel.HealthController = EFTGlobal.HealthController;
            }

            var healthController = healthHUDModel.HealthController;

            if (healthController == null)
                return;

            var health = healthHUDModel.Health;

            health.Head =
                _HealthControllerHelper.GetBodyPartHealth(healthController, EBodyPart.Head);
            health.Chest =
                _HealthControllerHelper.GetBodyPartHealth(healthController, EBodyPart.Chest);
            health.Stomach =
                _HealthControllerHelper.GetBodyPartHealth(healthController, EBodyPart.Stomach);
            health.LeftArm =
                _HealthControllerHelper.GetBodyPartHealth(healthController, EBodyPart.LeftArm);
            health.RightArm =
                _HealthControllerHelper.GetBodyPartHealth(healthController, EBodyPart.RightArm);
            health.LeftLeg =
                _HealthControllerHelper.GetBodyPartHealth(healthController, EBodyPart.LeftLeg);
            health.RightLeg =
                _HealthControllerHelper.GetBodyPartHealth(healthController, EBodyPart.RightLeg);
            health.Common =
                _HealthControllerHelper.GetBodyPartHealth(healthController, EBodyPart.Common);

            health.Hydration = healthController.Hydration;
            health.Energy = healthController.Energy;

            health.HealthRate = healthController.HealthRate;
            health.HydrationRate = healthController.HydrationRate;
            health.EnergyRate = healthController.EnergyRate;
        }

#endif
    }
}