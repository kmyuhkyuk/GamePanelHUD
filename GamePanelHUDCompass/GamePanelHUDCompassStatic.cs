using System;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using EFT;
using EFT.UI;
using System.Globalization;
#if !UNITY_EDITOR
using GamePanelHUDCore;
using GamePanelHUDCore.Utils.Session;
#endif
using GamePanelHUDCore.Utils;

namespace GamePanelHUDCompass
{
    public class GamePanelHUDCompassStatic : UIElement
#if !UNITY_EDITOR
        , IUpdate
#endif
    {
#if !UNITY_EDITOR
        private GamePanelHUDCorePlugin.HUDClass<GamePanelHUDCompassPlugin.CompassStaticData, GamePanelHUDCompassPlugin.SettingsData> HUD => GamePanelHUDCompassPlugin.CompassStaticHUD;
#endif

        private readonly Dictionary<string, List<GamePanelHUDCompassStaticUI>> CompassStatics = new Dictionary<string, List<GamePanelHUDCompassStaticUI>>();

        private readonly List<string> Removes = new List<string>();

        [SerializeField]
        private Transform _CompassStatic;

        [SerializeField]
        private RectTransform _Azimuths;

        [SerializeField]
        private Transform _Airdrops;

        [SerializeField]
        private Transform _Quests;

        [SerializeField]
        private Transform _Exfiltrations;

        [SerializeField]
        private Sprite Airdrop;

        [SerializeField]
        private Sprite Exfiltration;

        [SerializeField]
        private TMP_Text _NecessaryValue;

        [SerializeField]
        private TMP_Text _RequirementsValue;

        [SerializeField]
        private TMP_Text _NameValue;

        [SerializeField]
        private TMP_Text _DescriptionValue;

        [SerializeField]
        private TMP_Text _DistanceValue;

        [SerializeField]
        private TMP_Text _DistanceSignValue;

        private Transform InfoPanel;

        private Transform DistancePanel;

        private RectTransform InfoPanelRect;

#if !UNITY_EDITOR
        void Start()
        {
            InfoPanel = _NameValue.transform.parent.parent;
            InfoPanelRect = InfoPanel.GetComponent<RectTransform>();

            DistancePanel = _DistanceValue.transform.parent;

            GamePanelHUDCompassPlugin.ShowStatic = ShowStatic;
            GamePanelHUDCompassPlugin.DestroyStatic = DestroyStaticUI;
            GamePanelHUDCorePlugin.HUDCoreClass.WorldDispose += DestroyAll;

            GamePanelHUDCorePlugin.UpdateManger.Register(this);
        }

        public void IUpdate()
        {
            CompassStaticHUD();
        }

        void CompassStaticHUD()
        {
            RectTransform.anchoredPosition = HUD.SetData.KeyAnchoredPosition.Value;
            RectTransform.sizeDelta = new Vector2(HUD.Info.SizeDelta.x, HUD.Info.SizeDelta.y * 1.5f);
            RectTransform.localScale = HUD.SetData.KeyLocalScale.Value;

            if (_CompassStatic != null)
            {
                _CompassStatic.gameObject.SetActive(HUD.HUDSw);

                _Airdrops.gameObject.SetActive(HUD.SetData.KeyCompassStaticAirdrop.Value);
                _Exfiltrations.gameObject.SetActive(HUD.SetData.KeyCompassStaticExfiltration.Value);
                _Quests.gameObject.SetActive(HUD.SetData.KeyCompassStaticQuest.Value);

                _Azimuths.anchoredPosition = new Vector2(HUD.Info.CompassX, 0);

                InfoPanelRect.anchoredPosition = HUD.SetData.KeyCompassStaticInfoAnchoredPosition.Value;
                InfoPanelRect.localScale = HUD.SetData.KeyCompassStaticInfoScale.Value;

                bool isCenter = false;

                if (CompassStatics.Count > 0 && Removes.Count > 0)
                {
                    for (int i = 0; i < Removes.Count; i++)
                    {
                        string remove = Removes[i];

                        foreach (GamePanelHUDCompassStaticUI ui in CompassStatics[remove])
                        {
                            ui.Destroy();
                        }

                        CompassStatics.Remove(remove);

                        Removes.RemoveAt(i);
                    }
                }

                if (CompassStatics.Count > 0)
                {
                    int range = HUD.SetData.KeyCompassStaticCenterPointRange.Value;

                    List<Tuple<float, GamePanelHUDCompassStaticUI>> allXDiff = new List<Tuple<float, GamePanelHUDCompassStaticUI>>();

                    foreach (List<GamePanelHUDCompassStaticUI> uis in CompassStatics.Values)
                    {
                        foreach (GamePanelHUDCompassStaticUI ui in uis)
                        {
                            if (!ui.Work)
                                continue;

                            float xDiff = ui.XDiff;

                            if (xDiff < range && xDiff > -range)
                            {
                                allXDiff.Add(new Tuple<float, GamePanelHUDCompassStaticUI>(xDiff, ui));
                                isCenter = true;
                            }
                        }
                    }

                    if (isCenter)
                    {
                        float minXDiff = range;
                        GamePanelHUDCompassStaticUI minUI = default;

                        foreach (var xDiff in allXDiff)
                        {
                            if (Math.Abs(xDiff.Item1) < minXDiff)
                            {
                                minXDiff = xDiff.Item1;

                                minUI = xDiff.Item2;
                            }
                        }

                        if (minUI != null)
                        {
                            switch (minUI.InfoType)
                            {
                                case GamePanelHUDCompassPlugin.CompassStaticInfo.Type.Airdrop:
                                    _Airdrops.SetAsLastSibling();
                                    break;
                                case GamePanelHUDCompassPlugin.CompassStaticInfo.Type.Exfiltration:
                                case GamePanelHUDCompassPlugin.CompassStaticInfo.Type.Switch:
                                    _Exfiltrations.SetAsLastSibling();
                                    break;
                                case GamePanelHUDCompassPlugin.CompassStaticInfo.Type.ConditionLeaveItemAtLocation:
                                case GamePanelHUDCompassPlugin.CompassStaticInfo.Type.ConditionPlaceBeacon:
                                case GamePanelHUDCompassPlugin.CompassStaticInfo.Type.ConditionFindItem:
                                case GamePanelHUDCompassPlugin.CompassStaticInfo.Type.ConditionVisitPlace:
                                case GamePanelHUDCompassPlugin.CompassStaticInfo.Type.ConditionInZone:
                                    _Quests.SetAsLastSibling();
                                    break;
                            }

                            minUI.transform.SetAsLastSibling();

                            string necessaryText = minUI.IsNotNecessary ? LocalizedHelp.Localized("(optional)", EStringCase.None) : "";

                            TextInfo textInfo = CultureInfo.CurrentCulture.TextInfo;
                            string requirementsText = minUI.HasRequirements ? textInfo.ToTitleCase(LocalizedHelp.Localized("ragfair/REQUIREMENTS", EStringCase.Lower)) : "";

                            FontStyles nameStyles = HUD.SetData.KeyCompassStaticNameStyles.Value;
                            Color nameColor = HUD.SetData.KeyCompassStaticNameColor.Value;

                            _NameValue.fontStyle = nameStyles;
                            _NameValue.color = nameColor;
                            _NameValue.text = LocalizedHelp.Localized(minUI.NameKey, EStringCase.None);

                            _NecessaryValue.gameObject.SetActive(minUI.IsNotNecessary);
                            _RequirementsValue.gameObject.SetActive(minUI.HasRequirements);

                            _NecessaryValue.fontStyle = nameStyles;
                            _NecessaryValue.text = necessaryText;
                            _NecessaryValue.color = nameColor;

                            _RequirementsValue.fontStyle = nameStyles;
                            _RequirementsValue.color = nameColor;
                            _RequirementsValue.text = requirementsText;

                            _DescriptionValue.fontStyle = HUD.SetData.KeyCompassStaticDescriptionStyles.Value;
                            _DescriptionValue.color = HUD.SetData.KeyCompassStaticDescriptionColor.Value;
                            _DescriptionValue.text = LocalizedHelp.Localized(minUI.DescriptionKey, EStringCase.None);

                            FontStyles distanceStyles = HUD.SetData.KeyCompassStaticDistanceStyles.Value;
                            Color distanceColor = HUD.SetData.KeyCompassStaticDistanceColor.Value;

                            string distance = Vector3.Distance(minUI.Where, HUD.Info.PlayerPosition).ToString("F0");

                            _DistanceValue.fontStyle = distanceStyles;
                            _DistanceValue.color = distanceColor;
                            _DistanceValue.text = distance;

                            _DistanceSignValue.fontStyle = distanceStyles;
                            _DistanceSignValue.color = distanceColor;
                        }
                    }
                }

                if (!isCenter)
                {
                    _Quests.SetSiblingIndex(0);
                    _Exfiltrations.SetSiblingIndex(1);
                    _Airdrops.SetSiblingIndex(2);
                }

                InfoPanel.gameObject.SetActive(isCenter && HUD.SetData.KeyCompassStaticInfoHUDSw.Value);

                DistancePanel.gameObject.SetActive(HUD.SetData.KeyCompassStaticDistanceHUDSw.Value);
            }
        }

        void ShowStatic(GamePanelHUDCompassPlugin.CompassStaticInfo staticInfo)
        {
            Transform root;
            switch (staticInfo.InfoType)
            {
                case GamePanelHUDCompassPlugin.CompassStaticInfo.Type.Airdrop:
                    root = _Airdrops;
                    break;
                case GamePanelHUDCompassPlugin.CompassStaticInfo.Type.Exfiltration:
                case GamePanelHUDCompassPlugin.CompassStaticInfo.Type.Switch:
                    root = _Exfiltrations;
                    break;
                case GamePanelHUDCompassPlugin.CompassStaticInfo.Type.ConditionLeaveItemAtLocation:
                case GamePanelHUDCompassPlugin.CompassStaticInfo.Type.ConditionPlaceBeacon:
                case GamePanelHUDCompassPlugin.CompassStaticInfo.Type.ConditionFindItem:
                case GamePanelHUDCompassPlugin.CompassStaticInfo.Type.ConditionVisitPlace:
                case GamePanelHUDCompassPlugin.CompassStaticInfo.Type.ConditionInZone:
                    root = _Quests;
                    break;
                default:
                    return;
            }
            GameObject newInfo = Instantiate(GamePanelHUDCompassPlugin.StaticPrefab, root);

            GamePanelHUDCompassStaticUI _static = newInfo.GetComponent<GamePanelHUDCompassStaticUI>();

            switch (staticInfo.InfoType)
            {
                case GamePanelHUDCompassPlugin.CompassStaticInfo.Type.Airdrop:
                    _static.BindIcon(Airdrop);
                    break;
                case GamePanelHUDCompassPlugin.CompassStaticInfo.Type.Exfiltration:
                case GamePanelHUDCompassPlugin.CompassStaticInfo.Type.Switch:
                    _static.BindIcon(Exfiltration);
                    break;
                case GamePanelHUDCompassPlugin.CompassStaticInfo.Type.ConditionLeaveItemAtLocation:
                case GamePanelHUDCompassPlugin.CompassStaticInfo.Type.ConditionPlaceBeacon:
                case GamePanelHUDCompassPlugin.CompassStaticInfo.Type.ConditionFindItem:
                case GamePanelHUDCompassPlugin.CompassStaticInfo.Type.ConditionVisitPlace:
                case GamePanelHUDCompassPlugin.CompassStaticInfo.Type.ConditionInZone:
                    TradersAvatar.GetAvatar(staticInfo.TraderId, _static.BindIcon);
                    break;
            }

            _static.Where = staticInfo.Where;
            _static.ZoneId = staticInfo.ZoneId;
            _static.Target = staticInfo.Target;
            _static.NameKey = staticInfo.NameKey;
            _static.TraderId = staticInfo.TraderId;
            _static.IsNotNecessary = staticInfo.IsNotNecessary;
            _static.DescriptionKey = staticInfo.DescriptionKey;
            _static.ExIndex = staticInfo.ExIndex;
            _static.ExIndex2 = staticInfo.ExIndex2;
            _static.InfoType = staticInfo.InfoType;

            if (CompassStatics.TryGetValue(staticInfo.Id, out List<GamePanelHUDCompassStaticUI> list))
            {
                list.Add(_static);
            }
            else
            {
                CompassStatics.Add(staticInfo.Id, new List<GamePanelHUDCompassStaticUI>() { _static });
            }
        }

        void DestroyAll(GameWorld world)
        {
            foreach (string id in CompassStatics.Keys)
            {
                RemoveStaticUI(id);
            }
        }

        void DestroyStaticUI(string id)
        {
            if (CompassStatics.ContainsKey(id))
            {
                RemoveStaticUI(id);
            }
        }

        void RemoveStaticUI(string id)
        {
            if (!Removes.Contains(id))
            {
                Removes.Add(id);
            }
        }
#endif
    }
}
