using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Globalization;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
#if !UNITY_EDITOR
using EFT;
using EFTUtils;
using static EFTApi.EFTHelpers;
using GamePanelHUDCore.Models;
using GamePanelHUDCompass.Models;
using SettingsModel = GamePanelHUDCompass.Models.SettingsModel;

#endif

namespace GamePanelHUDCompass.Views
{
    public class CompassStaticHUDView : MonoBehaviour
#if !UNITY_EDITOR
        , IUpdate
#endif
    {
        private static readonly ConcurrentDictionary<string, List<CompassStaticUIView>> CompassStatics =
            new ConcurrentDictionary<string, List<CompassStaticUIView>>();

        private static readonly List<string> Removes = new List<string>();

        [SerializeField] private Transform compassStaticRoot;

        [SerializeField] private RectTransform azimuthsRoot;

        [SerializeField] private Transform airdropsRoot;

        [SerializeField] private Transform questsRoot;

        [SerializeField] private Transform exfiltrationsRoot;

        [SerializeField] private Sprite airdrop;

        [SerializeField] private Sprite exfiltration;

        [SerializeField] private TMP_Text necessaryValue;

        [SerializeField] private TMP_Text requirementsValue;

        [SerializeField] private TMP_Text nameValue;

        [SerializeField] private TMP_Text descriptionValue;

        [SerializeField] private TMP_Text distanceValue;

        [SerializeField] private TMP_Text distanceSignValue;

        [SerializeField] private Image inZone;

        private Transform _infoPanelTransform;

        private Transform _distancePanelTransform;

        private RectTransform _rectTransform;

        private RectTransform _infoPanelRect;

#if !UNITY_EDITOR
        private void Awake()
        {
            _rectTransform = GetComponent<RectTransform>();

            _infoPanelTransform = nameValue.transform.parent.parent;
            _infoPanelRect = _infoPanelTransform.GetComponent<RectTransform>();

            _distancePanelTransform = distanceValue.transform.parent;

            var compassStaticHUDModel = CompassStaticHUDModel.Instance;

            compassStaticHUDModel.ShowStatic = ShowStatic;
            compassStaticHUDModel.DestroyStatic = DestroyStaticUI;
        }

        private void Start()
        {
            HUDCoreModel.Instance.WorldDispose += DestroyAll;

            HUDCoreModel.Instance.UpdateManger.Register(this);
        }

        public void CustomUpdate()
        {
            var compassHUDModel = CompassHUDModel.Instance;
            var compassFireHUDModel = CompassFireHUDModel.Instance;
            var compassStaticHUDModel = CompassStaticHUDModel.Instance;
            var settingsModel = SettingsModel.Instance;

            _rectTransform.anchoredPosition = settingsModel.KeyAnchoredPosition.Value;
            _rectTransform.sizeDelta = new Vector2(compassHUDModel.Compass.SizeDelta.x,
                compassHUDModel.Compass.SizeDelta.y * 1.5f);
            _rectTransform.localScale = settingsModel.KeyLocalScale.Value;

            compassStaticRoot.gameObject.SetActive(compassStaticHUDModel.CompassStaticHUDSw);

            airdropsRoot.gameObject.SetActive(settingsModel.KeyCompassStaticAirdrop.Value);
            exfiltrationsRoot.gameObject.SetActive(settingsModel.KeyCompassStaticExfiltration.Value);
            questsRoot.gameObject.SetActive(settingsModel.KeyCompassStaticQuest.Value);

            azimuthsRoot.anchoredPosition = new Vector2(compassHUDModel.Compass.CompassX, 0);

            _infoPanelRect.anchoredPosition = settingsModel.KeyCompassStaticInfoAnchoredPosition.Value;
            _infoPanelRect.localScale = settingsModel.KeyCompassStaticInfoScale.Value;

            var isCenter = false;
            if (CompassStatics.Count > 0)
            {
                if (Removes.Count > 0)
                {
                    for (var i = 0; i < Removes.Count; i++)
                    {
                        var remove = Removes[i];

                        if (CompassStatics.TryRemove(remove, out var list))
                        {
                            foreach (var ui in list)
                            {
                                ui.Destroy();
                            }

                            Removes.RemoveAt(i);
                        }
                    }
                }

                var range = settingsModel.KeyCompassStaticCenterPointRange.Value;

                List<(float XDiff, CompassStaticUIView StaticUI)> allDiff =
                    new List<(float, CompassStaticUIView)>();
                foreach (var uis in CompassStatics.Values)
                {
                    foreach (var ui in uis)
                    {
                        if (!ui.Work)
                            continue;

                        var xDiff = ui.XDiff;

                        if (xDiff < range && xDiff > -range)
                        {
                            allDiff.Add((xDiff, ui));
                            isCenter = true;
                        }
                    }
                }

                if (isCenter)
                {
                    float minXDiff = range;
                    CompassStaticUIView minUI = null;

                    foreach (var diff in allDiff)
                    {
                        if (Math.Abs(diff.XDiff) < minXDiff)
                        {
                            minXDiff = diff.XDiff;

                            minUI = diff.StaticUI;
                        }
                    }

                    if (minUI != null)
                    {
                        switch (minUI.InfoType)
                        {
                            case StaticModel.Type.Airdrop:
                                airdropsRoot.SetAsLastSibling();
                                break;
                            case StaticModel.Type.Exfiltration:
                            case StaticModel.Type.Switch:
                                exfiltrationsRoot.SetAsLastSibling();
                                break;
                            case StaticModel.Type.ConditionLeaveItemAtLocation:
                            case StaticModel.Type.ConditionPlaceBeacon:
                            case StaticModel.Type.ConditionFindItem:
                            case StaticModel.Type.ConditionVisitPlace:
                            case StaticModel.Type.ConditionInZone:
                                questsRoot.SetAsLastSibling();
                                break;
                            default:
                                throw new ArgumentOutOfRangeException(nameof(minUI.InfoType), minUI.InfoType, null);
                        }

                        minUI.transform.SetAsLastSibling();
                        var necessaryText = minUI.isNotNecessary
                            ? _LocalizedHelper.Localized("(optional)")
                            : string.Empty;

                        var textInfo = CultureInfo.CurrentCulture.TextInfo;
                        var requirementsText = minUI.HasRequirements
                            ? textInfo.ToTitleCase(_LocalizedHelper.Localized("ragfair/REQUIREMENTS",
                                EStringCase.Lower))
                            : string.Empty;

                        var nameStyles = settingsModel.KeyCompassStaticNameStyles.Value;

                        nameValue.fontStyle = nameStyles;
                        nameValue.color = settingsModel.KeyCompassStaticNameColor.Value;
                        nameValue.text = _LocalizedHelper.Localized(minUI.nameKey);

                        necessaryValue.gameObject.SetActive(minUI.isNotNecessary);
                        requirementsValue.gameObject.SetActive(minUI.HasRequirements);

                        necessaryValue.fontStyle = nameStyles;
                        necessaryValue.text = necessaryText;
                        necessaryValue.color = settingsModel.KeyCompassStaticNecessaryColor.Value;

                        requirementsValue.fontStyle = nameStyles;
                        requirementsValue.color = settingsModel.KeyCompassStaticRequirementsColor.Value;
                        requirementsValue.text = requirementsText;

                        descriptionValue.fontStyle = settingsModel.KeyCompassStaticDescriptionStyles.Value;
                        descriptionValue.color = settingsModel.KeyCompassStaticDescriptionColor.Value;
                        descriptionValue.text = _LocalizedHelper.Localized(minUI.descriptionKey);

                        var distanceStyles = settingsModel.KeyCompassStaticDistanceStyles.Value;

                        var distance = Vector3.Distance(minUI.where, compassFireHUDModel.CompassFire.CameraPosition)
                            .ToString("F0");

                        distanceValue.fontStyle = distanceStyles;
                        distanceValue.color = settingsModel.KeyCompassStaticDistanceColor.Value;
                        distanceValue.text = distance;

                        distanceSignValue.fontStyle = distanceStyles;
                        distanceSignValue.color = settingsModel.KeyCompassStaticMetersColor.Value;

                        inZone.gameObject.SetActive(settingsModel.KeyCompassStaticInZoneHUDSw.Value && minUI.InZone);
                        inZone.color = settingsModel.KeyCompassStaticInZoneColor.Value;
                    }
                }
            }

            if (!isCenter)
            {
                questsRoot.SetSiblingIndex(0);
                exfiltrationsRoot.SetSiblingIndex(1);
                airdropsRoot.SetSiblingIndex(2);
            }

            _infoPanelTransform.gameObject.SetActive(isCenter && settingsModel.KeyCompassStaticInfoHUDSw.Value);

            _distancePanelTransform.gameObject.SetActive(settingsModel.KeyCompassStaticDistanceHUDSw.Value);
        }

        private void ShowStatic(StaticModel staticModel)
        {
            CompassStatics.AddOrUpdate(staticModel.Id,
                key => new List<CompassStaticUIView> { CreateStatic(staticModel) },
                (key, value) =>
                {
                    if (value == null)
                        return null;

                    value.Add(CreateStatic(staticModel));

                    return value;
                });
        }

        private CompassStaticUIView CreateStatic(StaticModel staticModel)
        {
            Transform root;
            switch (staticModel.InfoType)
            {
                case StaticModel.Type.Airdrop:
                    root = airdropsRoot;
                    break;
                case StaticModel.Type.Exfiltration:
                case StaticModel.Type.Switch:
                    root = exfiltrationsRoot;
                    break;
                case StaticModel.Type.ConditionLeaveItemAtLocation:
                case StaticModel.Type.ConditionPlaceBeacon:
                case StaticModel.Type.ConditionFindItem:
                case StaticModel.Type.ConditionVisitPlace:
                case StaticModel.Type.ConditionInZone:
                    root = questsRoot;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(staticModel.InfoType), staticModel.InfoType, null);
            }

            var staticUI = Instantiate(CompassStaticHUDModel.Instance.StaticPrefab, root)
                .GetComponent<CompassStaticUIView>();

            switch (staticModel.InfoType)
            {
                case StaticModel.Type.Airdrop:
                    staticUI.BindIcon(airdrop);
                    break;
                case StaticModel.Type.Exfiltration:
                case StaticModel.Type.Switch:
                    staticUI.BindIcon(exfiltration);
                    break;
                case StaticModel.Type.ConditionLeaveItemAtLocation:
                case StaticModel.Type.ConditionPlaceBeacon:
                case StaticModel.Type.ConditionFindItem:
                case StaticModel.Type.ConditionVisitPlace:
                case StaticModel.Type.ConditionInZone:
                    _SessionHelper.TradersHelper.TradersAvatarData.GetAvatar(staticModel.TraderId, staticUI.BindIcon);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(staticModel.InfoType), staticModel.InfoType, null);
            }

            staticUI.where = staticModel.Where;
            staticUI.target = staticModel.Target;
            staticUI.nameKey = staticModel.NameKey;
            staticUI.isNotNecessary = staticModel.IsNotNecessary;
            staticUI.descriptionKey = staticModel.DescriptionKey;
            staticUI.InfoType = staticModel.InfoType;
            staticUI.Requirements = staticModel.Requirements;
            staticUI.zoneId = staticModel.ZoneId;

            return staticUI;
        }

        private static void DestroyAll(GameWorld __instance)
        {
            foreach (var id in CompassStatics.Keys)
            {
                RemoveStaticUI(id);
            }
        }

        private static void DestroyStaticUI(string id)
        {
            if (CompassStatics.ContainsKey(id))
            {
                RemoveStaticUI(id);
            }
        }

        private static void RemoveStaticUI(string id)
        {
            if (!Removes.Contains(id))
            {
                Removes.Add(id);
            }
        }

#endif
    }
}