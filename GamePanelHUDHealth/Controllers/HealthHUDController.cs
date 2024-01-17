using UnityEngine;
#if !UNITY_EDITOR
using GamePanelHUDCore.Models;
using GamePanelHUDHealth.Models;
using SettingsModel = GamePanelHUDHealth.Models.SettingsModel;
using EFTUtils;
using static EFTApi.EFTHelpers;

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

            healthHUDModel.HealthHUDSw = hudCoreModel.AllHUDSw && healthHUDModel.HealthController != null &&
                                         hudCoreModel.HasPlayer &&
                                         settingsModel.KeyHealthHUDSw.Value;

            if (hudCoreModel.HasPlayer)
            {
                healthHUDModel.HealthController = _PlayerHelper.HealthControllerHelper.HealthController;
            }

            var healthController = healthHUDModel.HealthController;

            if (healthController == null)
                return;

            var health = healthHUDModel.Health;

            health.Head =
                _PlayerHelper.HealthControllerHelper.GetBodyPartHealth(healthController, EBodyPart.Head);
            health.Chest =
                _PlayerHelper.HealthControllerHelper.GetBodyPartHealth(healthController, EBodyPart.Chest);
            health.Stomach =
                _PlayerHelper.HealthControllerHelper.GetBodyPartHealth(healthController, EBodyPart.Stomach);
            health.LeftArm =
                _PlayerHelper.HealthControllerHelper.GetBodyPartHealth(healthController, EBodyPart.LeftArm);
            health.RightArm =
                _PlayerHelper.HealthControllerHelper.GetBodyPartHealth(healthController, EBodyPart.RightArm);
            health.LeftLeg =
                _PlayerHelper.HealthControllerHelper.GetBodyPartHealth(healthController, EBodyPart.LeftLeg);
            health.RightLeg =
                _PlayerHelper.HealthControllerHelper.GetBodyPartHealth(healthController, EBodyPart.RightLeg);
            health.Common =
                _PlayerHelper.HealthControllerHelper.GetBodyPartHealth(healthController, EBodyPart.Common);

            health.Hydration = _PlayerHelper.HealthControllerHelper.RefHydration.GetValue(healthController);
            health.Energy = _PlayerHelper.HealthControllerHelper.RefEnergy.GetValue(healthController);

            health.HealthRate = _PlayerHelper.HealthControllerHelper.RefHealthRate.GetValue(healthController);
            health.HydrationRate = _PlayerHelper.HealthControllerHelper.RefHydrationRate.GetValue(healthController);
            health.EnergyRate = _PlayerHelper.HealthControllerHelper.RefEnergyRate.GetValue(healthController);
        }

#endif
    }
}