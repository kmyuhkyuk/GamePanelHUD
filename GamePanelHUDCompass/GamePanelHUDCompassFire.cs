using EFT;
using EFT.UI;
using GamePanelHUDCore;
using GamePanelHUDCore.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace GamePanelHUDCompass
{
    public class GamePanelHUDCompassFire : UIElement
#if !UNITY_EDITOR
        , IUpdate
#endif
    {
#if !UNITY_EDITOR
        private GamePanelHUDCorePlugin.HUDClass<GamePanelHUDCompassPlugin.CompassInfo, GamePanelHUDCompassPlugin.SettingsData> HUD
        {
            get
            {
                return GamePanelHUDCompassPlugin.CompassFireHUD;
            }
        }
#endif

        private Dictionary<int, GamePanelHUDCompassFireUI> CompassFires = new Dictionary<int, GamePanelHUDCompassFireUI>();

        [SerializeField]
        private RectTransform _Azimuths;

        [SerializeField]
        private Transform _Fires;

        private CanvasGroup FiresGroup;

        internal static Action<int> Remove;

#if !UNITY_EDITOR
        void Start()
        {
            FiresGroup = _Fires.GetComponent<CanvasGroup>();

            Remove = RemoveFireUI;

            GamePanelHUDCompassPlugin.ShowFire = ShowFire;

            GamePanelHUDCorePlugin.UpdateManger.Register(this);
        }

        public void IUpdate()
        {
            RectTransform.anchoredPosition = HUD.SettingsData.KeyAnchoredPosition.Value;
            RectTransform.sizeDelta = HUD.SettingsData.KeySizeDelta.Value;
            RectTransform.localScale = HUD.SettingsData.KeyLocalScale.Value;

            if (_Azimuths != null)
            {
                if (_Fires != null)
                {
                    FiresGroup.alpha = HUD.SettingsData.KeyCompassFireHUDSW.Value ? 1 : 0;
                }

                _Azimuths.anchoredPosition = new Vector2(HUD.Info.CompassX, 0);
            }
        }

        void ShowFire(GamePanelHUDCompassPlugin.CompassFireInfo fireinfo)
        {
            if (_Fires != null)
            {
                if (!fireinfo.IsSilenced || !HUD.SettingsData.KeyCompassFireSilenced.Value)
                {
                    if (CompassFires.TryGetValue(fireinfo.Who, out var fireui))
                    {
                        fireui.Where = fireinfo.Where;

                        fireui.Fire();
                    }
                    else
                    {
                        GameObject fire = Instantiate(GamePanelHUDCompassPlugin.FirePrefab, _Fires);

                        GamePanelHUDCompassFireUI _fire = fire.GetComponent<GamePanelHUDCompassFireUI>();

                        _fire.Who = fireinfo.Who;

                        _fire.Where = fireinfo.Where;

                        _fire.Active = true;

                        CompassFires.Add(fireinfo.Who, _fire);
                    }
                }
            }
        }

        void RemoveFireUI(int id)
        {
            CompassFires.Remove(id);
        }
#endif
    }
}
