using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
#if !UNITY_EDITOR
using EFTUtils;
using GamePanelHUDCore;
using static EFTApi.EFTHelpers;

#endif

namespace GamePanelHUDCompass
{
    public class GamePanelHUDCompassFire : MonoBehaviour
#if !UNITY_EDITOR
        , IUpdate
#endif
    {
#if !UNITY_EDITOR

        private static GamePanelHUDCorePlugin.HUDCoreClass HUDCore => GamePanelHUDCorePlugin.HUDCore;

        private static
            GamePanelHUDCorePlugin.HUDClass<GamePanelHUDCompassPlugin.CompassFireData,
                GamePanelHUDCompassPlugin.SettingsData> HUD => GamePanelHUDCompassPlugin.CompassFireHUD;

#endif
        private readonly ConcurrentDictionary<string, GamePanelHUDCompassFireUI> _compassFires =
            new ConcurrentDictionary<string, GamePanelHUDCompassFireUI>();

        private readonly List<string> _removes = new List<string>();

        [SerializeField] private Transform compassFireRoot;

        [SerializeField] private RectTransform azimuthsRoot;

        [SerializeField] private Transform firesRoot;

        [SerializeField] private TMP_Text fireLeft;

        [SerializeField] private TMP_Text fireRight;

        private CanvasGroup _compassFireGroup;

        private RectTransform _rectTransform;

        private RectTransform _fireLeftRect;

        private RectTransform _fireRightRect;

        internal static Action<string> Remove;

#if !UNITY_EDITOR

        private void Start()
        {
            _rectTransform = GetComponent<RectTransform>();

            _compassFireGroup = compassFireRoot.GetComponent<CanvasGroup>();

            _fireLeftRect = fireLeft.GetComponent<RectTransform>();
            _fireRightRect = fireRight.GetComponent<RectTransform>();

            Remove = RemoveFireUI;

            GamePanelHUDCompassPlugin.ShowFire = ShowFire;
            GamePanelHUDCompassPlugin.DestroyFire = DestroyFireUI;

            HUDCore.UpdateManger.Register(this);
        }

        public void CustomUpdate()
        {
            CompassFireHUD();
        }

        private void CompassFireHUD()
        {
            _rectTransform.anchoredPosition = HUD.SetData.KeyAnchoredPosition.Value;
            _rectTransform.sizeDelta = HUD.Info.SizeDelta;
            _rectTransform.localScale = HUD.SetData.KeyLocalScale.Value;

            _compassFireGroup.alpha = HUD.HUDSw ? 1 : 0;

            azimuthsRoot.anchoredPosition = new Vector2(HUD.Info.CompassX, 0);

            var directionPosition = HUD.SetData.KeyCompassFireDirectionAnchoredPosition.Value;

            _fireLeftRect.anchoredPosition = new Vector2(-directionPosition.x, directionPosition.y);
            _fireLeftRect.localScale = HUD.SetData.KeyCompassFireDirectionScale.Value;
            var leftDirectionColor = HUD.SetData.KeyCompassFireColor.Value;
            var left = false;

            _fireRightRect.anchoredPosition = directionPosition;
            _fireRightRect.localScale = HUD.SetData.KeyCompassFireDirectionScale.Value;
            var rightDirectionColor = HUD.SetData.KeyCompassFireColor.Value;
            var right = false;

            if (_compassFires.Count > 0 && _removes.Count > 0)
            {
                for (var i = 0; i < _removes.Count; i++)
                {
                    var remove = _removes[i];

                    if (_compassFires.TryRemove(remove, out var ui))
                    {
                        ui.Destroy();

                        _removes.RemoveAt(i);
                    }
                }
            }

            if (_compassFires.Count > 0)
            {
                foreach (var fireUI in _compassFires.Values)
                {
                    var isLeft = fireUI.IsLeft;

                    if (!isLeft.HasValue)
                        continue;

                    var isBoos = fireUI.isBoss;

                    var isFollower = fireUI.isFollower;

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

            fireLeft.gameObject.SetActive(left && HUD.SetData.KeyCompassFireDirectionHUDSw.Value);
            fireRight.gameObject.SetActive(right && HUD.SetData.KeyCompassFireDirectionHUDSw.Value);

            fireLeft.fontStyle = HUD.SetData.KeyCompassFireDirectionStyles.Value;
            fireRight.fontStyle = HUD.SetData.KeyCompassFireDirectionStyles.Value;

            fireLeft.color = leftDirectionColor;
            fireRight.color = rightDirectionColor;
        }

        private void ShowFire(GamePanelHUDCompassPlugin.CompassFireInfo fireInfo)
        {
            if ((!fireInfo.IsSilenced || !HUD.SetData.KeyCompassFireSilenced.Value) &&
                fireInfo.Distance <= HUD.SetData.KeyCompassFireDistance.Value)
            {
                _compassFires.AddOrUpdate(fireInfo.Who, key =>
                {
                    var fireUI = Instantiate(GamePanelHUDCompassPlugin.FirePrefab, firesRoot)
                        .GetComponent<GamePanelHUDCompassFireUI>();

                    fireUI.who = fireInfo.Who;

                    fireUI.where = fireInfo.Where;

                    var isBoss = _PlayerHelper.RoleHelper.IsBoss(fireInfo.Role);
                    var isFollower = _PlayerHelper.RoleHelper.IsFollower(fireInfo.Role);

                    fireUI.isBoss = isBoss;

                    fireUI.isFollower = isFollower;

                    if (isBoss)
                    {
                        fireUI.fireColor = HUD.SetData.KeyCompassFireBossColor.Value;
                        fireUI.outlineColor = HUD.SetData.KeyCompassFireBossOutlineColor.Value;
                    }
                    else if (isFollower)
                    {
                        fireUI.fireColor = HUD.SetData.KeyCompassFireFollowerColor.Value;
                        fireUI.outlineColor = HUD.SetData.KeyCompassFireFollowerOutlineColor.Value;
                    }
                    else
                    {
                        fireUI.fireColor = HUD.SetData.KeyCompassFireColor.Value;
                        fireUI.outlineColor = HUD.SetData.KeyCompassFireOutlineColor.Value;
                    }

                    fireUI.fireSizeDelta = HUD.SetData.KeyCompassFireSizeDelta.Value;

                    fireUI.outlineSizeDelta = HUD.SetData.KeyCompassFireOutlineSizeDelta.Value;

                    fireUI.active = true;

                    return fireUI;
                }, (key, value) =>
                {
                    if (value == null)
                        return null;

                    value.where = fireInfo.Where;
                    value.Fire();

                    return value;
                });
            }
        }

        private void DestroyFireUI(string id)
        {
            if (HUD.SetData.KeyCompassFireDeadDestroy.Value && _compassFires.ContainsKey(id))
            {
                RemoveFireUI(id);
            }
        }

        private void RemoveFireUI(string id)
        {
            if (!_removes.Contains(id))
            {
                _removes.Add(id);
            }
        }

#endif
    }
}