using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using TMPro;
using EFT;
using EFT.UI;
using GamePanelHUDCore;
using GamePanelHUDCore.Utils;

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

        [SerializeField]
        private TMP_Text _FireLeft;

        [SerializeField]
        private TMP_Text _FireRight;

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
                FiresGroup.alpha = HUD.SettingsData.KeyCompassFireHUDSW.Value ? 1 : 0;

                _Azimuths.anchoredPosition = new Vector2(HUD.Info.CompassX, 0);

                IEnumerable<GamePanelHUDCompassPlugin.CompassFireInfo.HideDirection> directions = CompassFires.Values.Select(x => x.GetDirection());

                _FireLeft.gameObject.SetActive(directions.Contains(GamePanelHUDCompassPlugin.CompassFireInfo.HideDirection.Left));
                _FireRight.gameObject.SetActive(directions.Contains(GamePanelHUDCompassPlugin.CompassFireInfo.HideDirection.Right));
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
