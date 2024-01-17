using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
#if !UNITY_EDITOR
using static EFTApi.EFTHelpers;
using EFTUtils;
using GamePanelHUDCore.Models;
using GamePanelHUDCompass.Models;
using SettingsModel = GamePanelHUDCompass.Models.SettingsModel;

#endif

namespace GamePanelHUDCompass.Views
{
    public class CompassFireHUDView : MonoBehaviour
#if !UNITY_EDITOR
        , IUpdate
#endif
    {
        private readonly ConcurrentDictionary<string, CompassFireUIView> _compassFires =
            new ConcurrentDictionary<string, CompassFireUIView>();

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
        private void Awake()
        {
            _rectTransform = GetComponent<RectTransform>();

            _compassFireGroup = compassFireRoot.GetComponent<CanvasGroup>();

            _fireLeftRect = fireLeft.GetComponent<RectTransform>();
            _fireRightRect = fireRight.GetComponent<RectTransform>();

            Remove = RemoveFireUI;

            var compassFireHUDModel = CompassFireHUDModel.Instance;

            compassFireHUDModel.ShowFire = ShowFire;
            compassFireHUDModel.DestroyFire = DestroyFireUI;
        }

        private void Start()
        {
            HUDCoreModel.Instance.UpdateManger.Register(this);
        }

        public void CustomUpdate()
        {
            var compassHUDModel = CompassHUDModel.Instance;
            var compassFireHUDModel = CompassFireHUDModel.Instance;
            var settingsModel = SettingsModel.Instance;

            _rectTransform.anchoredPosition = settingsModel.KeyAnchoredPosition.Value;
            _rectTransform.sizeDelta = compassHUDModel.Compass.SizeDelta;
            _rectTransform.localScale = settingsModel.KeyLocalScale.Value;

            _compassFireGroup.alpha = compassFireHUDModel.CompassFireHUDSw ? 1 : 0;

            azimuthsRoot.anchoredPosition = new Vector2(compassHUDModel.Compass.CompassX, 0);

            var directionPosition = settingsModel.KeyCompassFireDirectionAnchoredPosition.Value;

            _fireLeftRect.anchoredPosition = new Vector2(-directionPosition.x, directionPosition.y);
            _fireLeftRect.localScale = settingsModel.KeyCompassFireDirectionScale.Value;
            var leftDirectionColor = settingsModel.KeyCompassFireColor.Value;

            _fireRightRect.anchoredPosition = directionPosition;
            _fireRightRect.localScale = settingsModel.KeyCompassFireDirectionScale.Value;
            var rightDirectionColor = settingsModel.KeyCompassFireColor.Value;

            var left = false;
            var right = false;
            if (_compassFires.Count > 0)
            {
                if (_removes.Count > 0)
                {
                    for (var i = 0; i < _removes.Count; i++)
                    {
                        var remove = _removes[i];

                        if (!_compassFires.TryRemove(remove, out var ui))
                            continue;

                        ui.Destroy();

                        _removes.RemoveAt(i);
                    }
                }

                foreach (var fireUI in _compassFires.Values)
                {
                    var isBoos = fireUI.isBoss;

                    var isFollower = fireUI.isFollower;

                    switch (fireUI.Direction)
                    {
                        case 1:
                            left = true;

                            if (isBoos)
                            {
                                leftDirectionColor = settingsModel.KeyCompassFireBossColor.Value;
                            }
                            else if (isFollower)
                            {
                                leftDirectionColor = settingsModel.KeyCompassFireFollowerColor.Value;
                            }

                            break;
                        case -1:
                            right = true;

                            if (isBoos)
                            {
                                rightDirectionColor = settingsModel.KeyCompassFireBossColor.Value;
                            }
                            else if (isFollower)
                            {
                                rightDirectionColor = settingsModel.KeyCompassFireFollowerColor.Value;
                            }

                            break;
                    }
                }
            }

            fireLeft.gameObject.SetActive(left && settingsModel.KeyCompassFireDirectionHUDSw.Value);
            fireRight.gameObject.SetActive(right && settingsModel.KeyCompassFireDirectionHUDSw.Value);

            fireLeft.fontStyle = settingsModel.KeyCompassFireDirectionStyles.Value;
            fireRight.fontStyle = settingsModel.KeyCompassFireDirectionStyles.Value;

            fireLeft.color = leftDirectionColor;
            fireRight.color = rightDirectionColor;
        }

        private void ShowFire(FireModel fireModel)
        {
            var settingsModel = SettingsModel.Instance;
            var compassFireHUDModel = CompassFireHUDModel.Instance;

            if ((!fireModel.IsSilenced || !settingsModel.KeyCompassFireSilenced.Value) &&
                fireModel.Distance <= settingsModel.KeyCompassFireDistance.Value)
            {
                _compassFires.AddOrUpdate(fireModel.Who, key =>
                {
                    var fireUI = Instantiate(compassFireHUDModel.FirePrefab, firesRoot)
                        .GetComponent<CompassFireUIView>();

                    fireUI.who = fireModel.Who;

                    fireUI.where = fireModel.Where;

                    var isBoss = _PlayerHelper.RoleHelper.IsBoss(fireModel.Role);
                    var isFollower = _PlayerHelper.RoleHelper.IsFollower(fireModel.Role);

                    fireUI.isBoss = isBoss;

                    fireUI.isFollower = isFollower;

                    if (isBoss)
                    {
                        fireUI.fireColor = settingsModel.KeyCompassFireBossColor.Value;
                        fireUI.outlineColor = settingsModel.KeyCompassFireBossOutlineColor.Value;
                    }
                    else if (isFollower)
                    {
                        fireUI.fireColor = settingsModel.KeyCompassFireFollowerColor.Value;
                        fireUI.outlineColor = settingsModel.KeyCompassFireFollowerOutlineColor.Value;
                    }
                    else
                    {
                        fireUI.fireColor = settingsModel.KeyCompassFireColor.Value;
                        fireUI.outlineColor = settingsModel.KeyCompassFireOutlineColor.Value;
                    }

                    fireUI.fireSizeDelta = settingsModel.KeyCompassFireSizeDelta.Value;

                    fireUI.outlineSizeDelta = settingsModel.KeyCompassFireOutlineSizeDelta.Value;

                    fireUI.active = true;

                    return fireUI;
                }, (key, value) =>
                {
                    if (value == null)
                        return null;

                    value.where = fireModel.Where;
                    value.Fire();

                    return value;
                });
            }
        }

        private void DestroyFireUI(string id)
        {
            if (SettingsModel.Instance.KeyCompassFireDeadDestroy.Value && _compassFires.ContainsKey(id))
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