using UnityEngine;
#if !UNITY_EDITOR
using GamePanelHUDCore.Models;

#endif

namespace GamePanelHUDCore.Controllers
{
    public class HUDCoreController : MonoBehaviour
    {
#if !UNITY_EDITOR

        public void Update()
        {
            var hudCoreModel = HUDCoreModel.Instance;

            bool allHUDSw;
            //All HUD always display 
            if (SettingsModel.Instance.KeyAllHUDAlways.Value)
            {
                allHUDSw = true;
            }
            //All HUD display 
            else if (hudCoreModel.HasPlayer)
            {
                allHUDSw = hudCoreModel.YourEftBattleUIScreen.gameObject.activeSelf;
            }
            else
            {
                allHUDSw = false;
            }

            hudCoreModel.AllHUDSw = allHUDSw;

            hudCoreModel.UpdateManger.Update();
        }

#endif
    }
}