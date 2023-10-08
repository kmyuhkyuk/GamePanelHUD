using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Globalization;
using EFT;
using TMPro;
using UnityEngine;
#if !UNITY_EDITOR
using EFTUtils;
using GamePanelHUDCore;
using static EFTApi.EFTHelpers;

#endif

namespace GamePanelHUDCompass
{
    public class GamePanelHUDCompassStatic : MonoBehaviour
#if !UNITY_EDITOR
        , IUpdate
#endif
    {
#if !UNITY_EDITOR

        private static GamePanelHUDCorePlugin.HUDCoreClass HUDCore => GamePanelHUDCorePlugin.HUDCore;

        private static
            GamePanelHUDCorePlugin.HUDClass<GamePanelHUDCompassPlugin.CompassStaticData,
                GamePanelHUDCompassPlugin.SettingsData> HUD => GamePanelHUDCompassPlugin.CompassStaticHUD;

#endif

        private static readonly ConcurrentDictionary<string, List<GamePanelHUDCompassStaticUI>> CompassStatics =
            new ConcurrentDictionary<string, List<GamePanelHUDCompassStaticUI>>();

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

        private Transform _infoPanelTransform;

        private Transform _distancePanelTransform;

        private RectTransform _rectTransform;

        private RectTransform _infoPanelRect;

#if !UNITY_EDITOR

        private void Start()
        {
            _rectTransform = GetComponent<RectTransform>();

            _infoPanelTransform = nameValue.transform.parent.parent;
            _infoPanelRect = _infoPanelTransform.GetComponent<RectTransform>();

            _distancePanelTransform = distanceValue.transform.parent;

            GamePanelHUDCompassPlugin.ShowStatic = ShowStatic;
            GamePanelHUDCompassPlugin.DestroyStatic = DestroyStaticUI;
            HUDCore.WorldDispose += DestroyAll;

            HUDCore.UpdateManger.Register(this);
        }

        public void CustomUpdate()
        {
            CompassStaticHUD();
        }

        private void CompassStaticHUD()
        {
            _rectTransform.anchoredPosition = HUD.SetData.KeyAnchoredPosition.Value;
            _rectTransform.sizeDelta = new Vector2(HUD.Info.SizeDelta.x, HUD.Info.SizeDelta.y * 1.5f);
            _rectTransform.localScale = HUD.SetData.KeyLocalScale.Value;

            compassStaticRoot.gameObject.SetActive(HUD.HUDSw);

            airdropsRoot.gameObject.SetActive(HUD.SetData.KeyCompassStaticAirdrop.Value);
            exfiltrationsRoot.gameObject.SetActive(HUD.SetData.KeyCompassStaticExfiltration.Value);
            questsRoot.gameObject.SetActive(HUD.SetData.KeyCompassStaticQuest.Value);

            azimuthsRoot.anchoredPosition = new Vector2(HUD.Info.CompassX, 0);

            _infoPanelRect.anchoredPosition = HUD.SetData.KeyCompassStaticInfoAnchoredPosition.Value;
            _infoPanelRect.localScale = HUD.SetData.KeyCompassStaticInfoScale.Value;

            var isCenter = false;

            if (CompassStatics.Count > 0 && Removes.Count > 0)
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

            if (CompassStatics.Count > 0)
            {
                var range = HUD.SetData.KeyCompassStaticCenterPointRange.Value;

                List<(float XDiff, GamePanelHUDCompassStaticUI StaticUI)> allDiff =
                    new List<(float, GamePanelHUDCompassStaticUI)>();

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
                    GamePanelHUDCompassStaticUI minUI = null;

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
                        switch (minUI.infoType)
                        {
                            case GamePanelHUDCompassPlugin.CompassStaticInfo.Type.Airdrop:
                                airdropsRoot.SetAsLastSibling();
                                break;
                            case GamePanelHUDCompassPlugin.CompassStaticInfo.Type.Exfiltration:
                            case GamePanelHUDCompassPlugin.CompassStaticInfo.Type.Switch:
                                exfiltrationsRoot.SetAsLastSibling();
                                break;
                            case GamePanelHUDCompassPlugin.CompassStaticInfo.Type.ConditionLeaveItemAtLocation:
                            case GamePanelHUDCompassPlugin.CompassStaticInfo.Type.ConditionPlaceBeacon:
                            case GamePanelHUDCompassPlugin.CompassStaticInfo.Type.ConditionFindItem:
                            case GamePanelHUDCompassPlugin.CompassStaticInfo.Type.ConditionVisitPlace:
                            case GamePanelHUDCompassPlugin.CompassStaticInfo.Type.ConditionInZone:
                                questsRoot.SetAsLastSibling();
                                break;
                            default:
                                throw new ArgumentOutOfRangeException(nameof(minUI.infoType), minUI.infoType, null);
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

                        var nameStyles = HUD.SetData.KeyCompassStaticNameStyles.Value;

                        nameValue.fontStyle = nameStyles;
                        nameValue.color = HUD.SetData.KeyCompassStaticNameColor.Value;
                        nameValue.text = _LocalizedHelper.Localized(minUI.nameKey);

                        necessaryValue.gameObject.SetActive(minUI.isNotNecessary);
                        requirementsValue.gameObject.SetActive(minUI.HasRequirements);

                        necessaryValue.fontStyle = nameStyles;
                        necessaryValue.text = necessaryText;
                        necessaryValue.color = HUD.SetData.KeyCompassStaticNecessaryColor.Value;

                        requirementsValue.fontStyle = nameStyles;
                        requirementsValue.color = HUD.SetData.KeyCompassStaticRequirementsColor.Value;
                        requirementsValue.text = requirementsText;

                        descriptionValue.fontStyle = HUD.SetData.KeyCompassStaticDescriptionStyles.Value;
                        descriptionValue.color = HUD.SetData.KeyCompassStaticDescriptionColor.Value;
                        descriptionValue.text = _LocalizedHelper.Localized(minUI.descriptionKey);

                        var distanceStyles = HUD.SetData.KeyCompassStaticDistanceStyles.Value;

                        var distance = Vector3.Distance(minUI.where, HUD.Info.CameraPosition).ToString("F0");

                        distanceValue.fontStyle = distanceStyles;
                        distanceValue.color = HUD.SetData.KeyCompassStaticDistanceColor.Value;
                        distanceValue.text = distance;

                        distanceSignValue.fontStyle = distanceStyles;
                        distanceSignValue.color = HUD.SetData.KeyCompassStaticMetersColor.Value;
                    }
                }
            }

            if (!isCenter)
            {
                questsRoot.SetSiblingIndex(0);
                exfiltrationsRoot.SetSiblingIndex(1);
                airdropsRoot.SetSiblingIndex(2);
            }

            _infoPanelTransform.gameObject.SetActive(isCenter && HUD.SetData.KeyCompassStaticInfoHUDSw.Value);

            _distancePanelTransform.gameObject.SetActive(HUD.SetData.KeyCompassStaticDistanceHUDSw.Value);
        }

        private void ShowStatic(GamePanelHUDCompassPlugin.CompassStaticInfo staticInfo)
        {
            Transform root;
            switch (staticInfo.InfoType)
            {
                case GamePanelHUDCompassPlugin.CompassStaticInfo.Type.Airdrop:
                    root = airdropsRoot;
                    break;
                case GamePanelHUDCompassPlugin.CompassStaticInfo.Type.Exfiltration:
                case GamePanelHUDCompassPlugin.CompassStaticInfo.Type.Switch:
                    root = exfiltrationsRoot;
                    break;
                case GamePanelHUDCompassPlugin.CompassStaticInfo.Type.ConditionLeaveItemAtLocation:
                case GamePanelHUDCompassPlugin.CompassStaticInfo.Type.ConditionPlaceBeacon:
                case GamePanelHUDCompassPlugin.CompassStaticInfo.Type.ConditionFindItem:
                case GamePanelHUDCompassPlugin.CompassStaticInfo.Type.ConditionVisitPlace:
                case GamePanelHUDCompassPlugin.CompassStaticInfo.Type.ConditionInZone:
                    root = questsRoot;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(staticInfo.InfoType), staticInfo.InfoType, null);
            }

            var staticUI = Instantiate(GamePanelHUDCompassPlugin.StaticPrefab, root)
                .GetComponent<GamePanelHUDCompassStaticUI>();

            switch (staticInfo.InfoType)
            {
                case GamePanelHUDCompassPlugin.CompassStaticInfo.Type.Airdrop:
                    staticUI.BindIcon(airdrop);
                    break;
                case GamePanelHUDCompassPlugin.CompassStaticInfo.Type.Exfiltration:
                case GamePanelHUDCompassPlugin.CompassStaticInfo.Type.Switch:
                    staticUI.BindIcon(exfiltration);
                    break;
                case GamePanelHUDCompassPlugin.CompassStaticInfo.Type.ConditionLeaveItemAtLocation:
                case GamePanelHUDCompassPlugin.CompassStaticInfo.Type.ConditionPlaceBeacon:
                case GamePanelHUDCompassPlugin.CompassStaticInfo.Type.ConditionFindItem:
                case GamePanelHUDCompassPlugin.CompassStaticInfo.Type.ConditionVisitPlace:
                case GamePanelHUDCompassPlugin.CompassStaticInfo.Type.ConditionInZone:
                    _SessionHelper.TradersHelper.TradersAvatarData.GetAvatar(staticInfo.TraderId, staticUI.BindIcon);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(staticInfo.InfoType), staticInfo.InfoType, null);
            }

            staticUI.where = staticInfo.Where;
            staticUI.target = staticInfo.Target;
            staticUI.nameKey = staticInfo.NameKey;
            staticUI.isNotNecessary = staticInfo.IsNotNecessary;
            staticUI.descriptionKey = staticInfo.DescriptionKey;
            staticUI.infoType = staticInfo.InfoType;
            staticUI.Requirements = staticInfo.Requirements;

            CompassStatics.AddOrUpdate(staticInfo.Id, key => new List<GamePanelHUDCompassStaticUI> { staticUI },
                (key, value) =>
                {
                    value.Add(staticUI);

                    return value;
                });
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