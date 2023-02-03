using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using EFT.UI;
using GamePanelHUDCore;
using GamePanelHUDCore.Utils;

namespace GamePanelHUDCompass
{
    public class GamePanelHUDCompassStatic : UIElement
#if !UNITY_EDITOR
        , IUpdate
#endif
    {
#if !UNITY_EDITOR
        private GamePanelHUDCorePlugin.HUDClass<GamePanelHUDCompassPlugin.CompassInfo, GamePanelHUDCompassPlugin.SettingsData> HUD
        {
            get
            {
                return GamePanelHUDCompassPlugin.CompassStaticHUD;
            }
        }
#endif

        [SerializeField]
        private Transform _CompassStatic;

        [SerializeField]
        private RectTransform _Azimuths;

        [SerializeField]
        private Transform _Airdrops;

        [SerializeField]
        private Transform _Quests;

        [SerializeField]
        private Sprite Airdrop;

        private CanvasGroup CompassStaticGroup;

        private CanvasGroup AirdropsGroup;

        private CanvasGroup QuestsGroup;

#if !UNITY_EDITOR
        void Start()
        {
            CompassStaticGroup = _CompassStatic.GetComponent<CanvasGroup>();
            AirdropsGroup = _Airdrops.GetComponent<CanvasGroup>();
            QuestsGroup = _Quests.GetComponent<CanvasGroup>();

            GamePanelHUDCorePlugin.UpdateManger.Register(this);
        }

        public void IUpdate()
        {
            CompassStaticHUD();
        }

        void CompassStaticHUD()
        {
            RectTransform.anchoredPosition = HUD.SettingsData.KeyAnchoredPosition.Value;
            RectTransform.sizeDelta = HUD.SettingsData.KeySizeDelta.Value;
            RectTransform.localScale = HUD.SettingsData.KeyLocalScale.Value;

            if (_CompassStatic != null)
            {
                CompassStaticGroup.alpha = HUD.HUDSW ? 1 : 0;

                _Azimuths.anchoredPosition = new Vector2(HUD.Info.CompassX, 0);
            }
        }

        void ShowStatic(GamePanelHUDCompassPlugin.CompassStaticInfo staticinfo)
        {
            GameObject staticG = Instantiate(GamePanelHUDCompassPlugin.StaticPrefab, staticinfo.IsAirdrop ? _Airdrops : _Quests);

            GamePanelHUDCompassStaticUI _static = staticG.GetComponent<GamePanelHUDCompassStaticUI>();

            _static.Where = staticinfo.Where;
        }
#endif
    }
}
