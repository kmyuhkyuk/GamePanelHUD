using System;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using EFT.UI;
#if !UNITY_EDITOR
using GamePanelHUDCore;
#endif
using GamePanelHUDCore.Utils;

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

        private readonly Dictionary<string, GamePanelHUDCompassFireUI> CompassFires = new Dictionary<string, GamePanelHUDCompassFireUI>();

        private readonly List<string> Removes = new List<string>();

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

        internal static Action<string> Remove;

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

                Color leftDirectionColor = HUD.SettingsData.KeyCompassFireColor.Value;
                Color rightDirectionColor = HUD.SettingsData.KeyCompassFireColor.Value;

                bool left = false;
                bool right = false;

                if (CompassFires.Count > 0)
                {
                    if (Removes.Count > 0)
                    {
                        for (int i = 0; i < Removes.Count;i++)
                        {
                            string remove = Removes[i];

                            CompassFires[remove].Destroy();

                            CompassFires.Remove(remove);

                            Removes.RemoveAt(i);
                        }
                    }

                    foreach (GamePanelHUDCompassFireUI fire in CompassFires.Values)
                    {
                        bool? isLeft = fire.IsLeft;

                        if (!isLeft.HasValue)
                            continue;

                        bool isBoos = fire.IsBoss;

                        bool isFollower = fire.IsFollower;

                        if ((bool)isLeft)
                        {
                            left = true;

                            if (isBoos)
                            {
                                leftDirectionColor = HUD.SettingsData.KeyCompassFireBossColor.Value;
                            }
                            else if (isFollower)
                            {
                                leftDirectionColor = HUD.SettingsData.KeyCompassFireFollowerColor.Value;
                            }
                        }
                        else
                        {
                            right = true;

                            if (isBoos)
                            {
                                rightDirectionColor = HUD.SettingsData.KeyCompassFireBossColor.Value;
                            }
                            else if (isFollower)
                            {
                                rightDirectionColor = HUD.SettingsData.KeyCompassFireFollowerColor.Value;
                            }
                        }
                    }
                }

                _FireLeft.gameObject.SetActive(left && HUD.SettingsData.KeyCompassFireDirectionHUDSW.Value);
                _FireRight.gameObject.SetActive(right && HUD.SettingsData.KeyCompassFireDirectionHUDSW.Value);

                _FireLeft.fontStyle = HUD.SettingsData.KeyCompassFireDirectionStyles.Value;
                _FireRight.fontStyle = HUD.SettingsData.KeyCompassFireDirectionStyles.Value;

                _FireLeft.color = leftDirectionColor;
                _FireRight.color = rightDirectionColor;
            }
        }

        void ShowFire(GamePanelHUDCompassPlugin.CompassFireInfo fireinfo)
        {
            if (_Fires != null)
            {
                if ((!fireinfo.IsSilenced || !HUD.SettingsData.KeyCompassFireSilenced.Value) && fireinfo.Distance <= HUD.SettingsData.KeyCompassFireDistance.Value)
                {
                    if (CompassFires.TryGetValue(fireinfo.Who, out GamePanelHUDCompassFireUI fireui))
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

        void DestroyFireUI(string id)
        {
            if (HUD.SettingsData.KeyCompassFireDeadDestroy.Value && CompassFires.ContainsKey(id))
            {
                RemoveFireUI(id);
            }
        }

        void RemoveFireUI(string id)
        {
            if (!Removes.Contains(id))
            {
                Removes.Add(id);
            }
        }
#endif
    }
}
