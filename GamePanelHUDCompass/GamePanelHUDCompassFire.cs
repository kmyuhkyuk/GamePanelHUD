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
        private GamePanelHUDCorePlugin.HUDClass<GamePanelHUDCompassPlugin.CompassFireData, GamePanelHUDCompassPlugin.SettingsData> HUD => GamePanelHUDCompassPlugin.CompassFireHUD;
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
            CompassFireHUD();
        }

        void CompassFireHUD()
        {
            RectTransform.anchoredPosition = HUD.SetData.KeyAnchoredPosition.Value;
            RectTransform.sizeDelta = HUD.Info.SizeDelta;
            RectTransform.localScale = HUD.SetData.KeyLocalScale.Value;

            if (_CompassFire != null)
            {
                CompassFireGroup.alpha = HUD.HUDSw ? 1 : 0;

                _Azimuths.anchoredPosition = new Vector2(HUD.Info.CompassX, 0);

                Vector2 directionPosition = HUD.SetData.KeyCompassFireDirectionAnchoredPosition.Value;

                FireLeftRect.anchoredPosition = new Vector2(-directionPosition.x, directionPosition.y);
                FireRightRect.anchoredPosition = directionPosition;

                Color leftDirectionColor = HUD.SetData.KeyCompassFireColor.Value;
                Color rightDirectionColor = HUD.SetData.KeyCompassFireColor.Value;

                bool left = false;
                bool right = false;

                if (CompassFires.Count > 0 && Removes.Count > 0)
                {
                    for (int i = 0; i < Removes.Count; i++)
                    {
                        string remove = Removes[i];

                        CompassFires[remove].Destroy();

                        CompassFires.Remove(remove);

                        Removes.RemoveAt(i);
                    }
                }

                if (CompassFires.Count > 0)
                {
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
                                leftDirectionColor = HUD.SetData.KeyCompassFireBossColor.Value;
                            }
                            else if (isFollower)
                            {
                                leftDirectionColor = HUD.SetData.KeyCompassFireFollowerColor.Value;
                            }
                        }
                        else
                        {
                            right = true;

                            if (isBoos)
                            {
                                rightDirectionColor = HUD.SetData.KeyCompassFireBossColor.Value;
                            }
                            else if (isFollower)
                            {
                                rightDirectionColor = HUD.SetData.KeyCompassFireFollowerColor.Value;
                            }
                        }
                    }
                }

                _FireLeft.gameObject.SetActive(left && HUD.SetData.KeyCompassFireDirectionHUDSw.Value);
                _FireRight.gameObject.SetActive(right && HUD.SetData.KeyCompassFireDirectionHUDSw.Value);

                _FireLeft.fontStyle = HUD.SetData.KeyCompassFireDirectionStyles.Value;
                _FireRight.fontStyle = HUD.SetData.KeyCompassFireDirectionStyles.Value;

                _FireLeft.color = leftDirectionColor;
                _FireRight.color = rightDirectionColor;
            }
        }

        void ShowFire(GamePanelHUDCompassPlugin.CompassFireInfo fireInfo)
        {
            if (_Fires != null)
            {
                if ((!fireInfo.IsSilenced || !HUD.SetData.KeyCompassFireSilenced.Value) && fireInfo.Distance <= HUD.SetData.KeyCompassFireDistance.Value)
                {
                    if (CompassFires.TryGetValue(fireInfo.Who, out GamePanelHUDCompassFireUI fireUI))
                    {
                        fireUI.Where = fireInfo.Where;

                        fireUI.Fire();
                    }
                    else
                    {
                        GameObject fire = Instantiate(GamePanelHUDCompassPlugin.FirePrefab, _Fires);

                        GamePanelHUDCompassFireUI _fire = fire.GetComponent<GamePanelHUDCompassFireUI>();

                        _fire.Who = fireInfo.Who;

                        _fire.Where = fireInfo.Where;

                        bool isBoss = RoleHelp.IsBoss(fireInfo.Role);
                        bool isFollower = RoleHelp.IsFollower(fireInfo.Role);

                        _fire.IsBoss = isBoss;

                        _fire.IsFollower = isFollower;

                        if (isBoss)
                        {
                            _fire.FireColor = HUD.SetData.KeyCompassFireBossColor.Value;
                            _fire.OutlineColor = HUD.SetData.KeyCompassFireBossOutlineColor.Value;
                        }
                        else if (isFollower)
                        {
                            _fire.FireColor = HUD.SetData.KeyCompassFireFollowerColor.Value;
                            _fire.OutlineColor = HUD.SetData.KeyCompassFireFollowerOutlineColor.Value;
                        }
                        else
                        {
                            _fire.FireColor = HUD.SetData.KeyCompassFireColor.Value;
                            _fire.OutlineColor = HUD.SetData.KeyCompassFireOutlineColor.Value;
                        }

                        _fire.FireSizeDelta = HUD.SetData.KeyCompassFireSizeDelta.Value;

                        _fire.OutlineSizeDelta = HUD.SetData.KeyCompassFireOutlineSizeDelta.Value;

                        _fire.Active = true;

                        CompassFires.Add(fireInfo.Who, _fire);
                    }
                }
            }
        }

        void DestroyFireUI(string id)
        {
            if (HUD.SetData.KeyCompassFireDeadDestroy.Value && CompassFires.ContainsKey(id))
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
