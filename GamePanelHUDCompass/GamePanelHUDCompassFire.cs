using System;
using System.Text;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using EFT.UI;
#if !UNITY_EDITOR
using GamePanelHUDCore;
using GamePanelHUDCore.Utils;
#endif

namespace GamePanelHUDCompass
{
    public class GamePanelHUDCompassFire : UIElement
#if !UNITY_EDITOR
        , IUpdate
#endif
    {
#if !UNITY_EDITOR
        private GamePanelHUDCorePlugin.HUDClass<GamePanelHUDCompassPlugin.CompassFireData, GamePanelHUDCompassPlugin.SettingsData> HUD
        {
            get
            {
                return GamePanelHUDCompassPlugin.CompassFireHUD;
            }
        }
#endif

        private readonly Dictionary<int, GamePanelHUDCompassFireUI> CompassFires = new Dictionary<int, GamePanelHUDCompassFireUI>();

        [SerializeField]
        private Transform _CompassFire;

        [SerializeField]
        private RectTransform _Azimuths;

        [SerializeField]
        private Transform _Fires;

        [SerializeField]
        private TMP_Text _FireLeft;

        [SerializeField]
        private TMP_Text _FireRight;

        private CanvasGroup CompassFireGroup;

        private RectTransform FireLeftRect;

        private RectTransform FireRightRect;

        private readonly StringBuilderData StringBuilderDatas = new StringBuilderData();

        internal static Action<int> Remove;

#if !UNITY_EDITOR
        void Start()
        {
            CompassFireGroup = _CompassFire.GetComponent<CanvasGroup>();

            FireLeftRect = _FireLeft.GetComponent<RectTransform>();
            FireRightRect = _FireRight.GetComponent<RectTransform>();

            Remove = RemoveFireUI;

            GamePanelHUDCompassPlugin.ShowFire = ShowFire;
            GamePanelHUDCompassPlugin.DestroyFire = DestroyFireUI;

            GamePanelHUDCorePlugin.UpdateManger.Register(this);
        }

        public void IUpdate()
        {
            CompassFrieHUD();
        }

        void CompassFrieHUD()
        {
            RectTransform.anchoredPosition = HUD.SettingsData.KeyAnchoredPosition.Value;
            RectTransform.sizeDelta = HUD.Info.SizeDelta;
            RectTransform.localScale = HUD.SettingsData.KeyLocalScale.Value;

            if (_CompassFire != null)
            {
                CompassFireGroup.alpha = HUD.HUDSW ? 1 : 0;

                _Azimuths.anchoredPosition = new Vector2(HUD.Info.CompassX, 0);

                Vector2 directionPosition = HUD.SettingsData.KeyCompassFireDirectionAnchoredPosition.Value;

                FireLeftRect.anchoredPosition = new Vector2(-directionPosition.x, directionPosition.y);
                FireRightRect.anchoredPosition = directionPosition;

                _FireLeft.fontStyle = HUD.SettingsData.KeyCompassFireDirectionStyles.Value;
                _FireRight.fontStyle = HUD.SettingsData.KeyCompassFireDirectionStyles.Value;

                string leftDirectionColor = HUD.SettingsData.KeyCompassFireColor.Value.ColorToHtml();
                string rightDirectionColor = HUD.SettingsData.KeyCompassFireColor.Value.ColorToHtml();

                bool left = false;
                bool right = false;

                if (CompassFires.Count > 0)
                {
                    foreach (GamePanelHUDCompassFireUI fire in CompassFires.Values)
                    {
                        bool? isLeft = fire.IsLeft;

                        if (isLeft.HasValue)
                        {
                            bool isBoos = fire.IsBoss;

                            bool isFollower = fire.IsFollower;

                            if ((bool)isLeft)
                            {
                                left = true;

                                if (isBoos)
                                {
                                    leftDirectionColor = HUD.SettingsData.KeyCompassFireBossColor.Value.ColorToHtml();
                                }
                                else if (isFollower)
                                {
                                    leftDirectionColor = HUD.SettingsData.KeyCompassFireFollowerColor.Value.ColorToHtml();
                                }
                            }
                            else
                            {
                                right = true;

                                if (isBoos)
                                {
                                    rightDirectionColor = HUD.SettingsData.KeyCompassFireBossColor.Value.ColorToHtml();
                                }
                                else if (isFollower)
                                {
                                    rightDirectionColor = HUD.SettingsData.KeyCompassFireFollowerColor.Value.ColorToHtml();
                                }
                            }
                        }
                    }
                }

                _FireLeft.text = StringBuilderDatas._FireLeft.StringConcat("<color=", leftDirectionColor, ">", "<", "</color>");
                _FireRight.text = StringBuilderDatas._FireLeft.StringConcat("<color=", rightDirectionColor, ">", ">", "</color>");

                _FireLeft.gameObject.SetActive(left && HUD.SettingsData.KeyCompassFireDirectionHUDSW.Value);
                _FireRight.gameObject.SetActive(right && HUD.SettingsData.KeyCompassFireDirectionHUDSW.Value);
            }
        }

        void ShowFire(GamePanelHUDCompassPlugin.CompassFireInfo fireinfo)
        {
            if (_Fires != null)
            {
                if ((!fireinfo.IsSilenced || !HUD.SettingsData.KeyCompassFireSilenced.Value) && fireinfo.Distance <= HUD.SettingsData.KeyCompassFireDistance.Value)
                {
                    if (CompassFires.TryGetValue(fireinfo.Who, out var fireui) && !fireui.DeadDestroy)
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

                        bool isBoss = RoleHelp.IsBoss(fireinfo.Role);
                        bool isFollower = RoleHelp.IsFollower(fireinfo.Role);

                        _fire.IsBoss = isBoss;

                        _fire.IsFollower = isFollower;

                        if (isBoss)
                        {
                            _fire.FireColor = HUD.SettingsData.KeyCompassFireBossColor.Value;
                            _fire.OutlineColor = HUD.SettingsData.KeyCompassFireBossOutlineColor.Value;
                        }
                        else if (isFollower)
                        {
                            _fire.FireColor = HUD.SettingsData.KeyCompassFireFollowerColor.Value;
                            _fire.OutlineColor = HUD.SettingsData.KeyCompassFireFollowerOutlineColor.Value;
                        }
                        else
                        {
                            _fire.FireColor = HUD.SettingsData.KeyCompassFireColor.Value;
                            _fire.OutlineColor = HUD.SettingsData.KeyCompassFireOutlineColor.Value;
                        }

                        _fire.FireSizeDelta = HUD.SettingsData.KeyCompassFireSizeDelta.Value;

                        _fire.OutlineSizeDelta = HUD.SettingsData.KeyCompassFireOutlineSizeDelta.Value;

                        _fire.Active = true;

                        CompassFires.Add(fireinfo.Who, _fire);
                    }
                }
            }
        }

        void DestroyFireUI(int id)
        {
            if (CompassFires.TryGetValue(id, out var fireui) && HUD.SettingsData.KeyCompassFireDeadDestroy.Value)
            {
                fireui.DeadDestroy = true;
            }
        }

        void RemoveFireUI(int id)
        {
            CompassFires.Remove(id);
        }
#endif

        public class StringBuilderData
        {
            public StringBuilder _FireLeft = new StringBuilder(128);
            public StringBuilder _FireRight = new StringBuilder(128);
        }
    }
}
