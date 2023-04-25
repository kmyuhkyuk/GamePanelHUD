using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using EFT;
using EFT.UI;
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
        private GamePanelHUDCorePlugin.HUDClass<GamePanelHUDCompassPlugin.CompassStaticData, GamePanelHUDCompassPlugin.SettingsData> HUD
        {
            get
            {
                return GamePanelHUDCompassPlugin.CompassStaticHUD;
            }
        }
#endif

        private readonly Dictionary<string, List<GamePanelHUDCompassStaticUI>> CompassStatics = new Dictionary<string, List<GamePanelHUDCompassStaticUI>>();

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

        private CanvasGroup CompassStaticGroup;

        private CanvasGroup AirdropsGroup;

        private CanvasGroup QuestsGroup;

        private CanvasGroup ExfiltrationGroup;

        internal static Action<string> Remove;

#if !UNITY_EDITOR
        void Start()
        {
            CompassStaticGroup = _CompassStatic.GetComponent<CanvasGroup>();
            AirdropsGroup = _Airdrops.GetComponent<CanvasGroup>();
            QuestsGroup = _Quests.GetComponent<CanvasGroup>();
            ExfiltrationGroup = _Exfiltrations.GetComponent<CanvasGroup>();

            InfoPanel = _NameValue.transform.parent.parent;
            InfoPanelRect = InfoPanel.GetComponent<RectTransform>();

            DistancePanel = _DistanceValue.transform.parent;

            Remove = RemoveStatic;
            GamePanelHUDCompassPlugin.ShowStatic = ShowStatic;
            GamePanelHUDCompassPlugin.DestroyStatic = DestroyStatic;
            GamePanelHUDCorePlugin.HUDCoreClass.WorldDispose += DestroyAll;

            GamePanelHUDCorePlugin.UpdateManger.Register(this);
        }

        public void IUpdate()
        {
            CompassStaticHUD();
        }

        void CompassStaticHUD()
        {
            RectTransform.anchoredPosition = HUD.SettingsData.KeyAnchoredPosition.Value;
            RectTransform.sizeDelta = new Vector2(HUD.Info.SizeDelta.x, HUD.Info.SizeDelta.y * 1.5f);
            RectTransform.localScale = HUD.SettingsData.KeyLocalScale.Value;

            if (_CompassStatic != null)
            {
                CompassStaticGroup.alpha = HUD.HUDSW ? 1 : 0;
                AirdropsGroup.alpha = HUD.SettingsData.KeyCompassStaticAirdrop.Value ? 1 : 0;
                ExfiltrationGroup.alpha = HUD.SettingsData.KeyCompassStaticExfiltration.Value ? 1 : 0;
                QuestsGroup.alpha = HUD.SettingsData.KeyCompassStaticQuest.Value ? 1 : 0;

                _Azimuths.anchoredPosition = new Vector2(HUD.Info.CompassX, 0);

                InfoPanelRect.anchoredPosition = HUD.SettingsData.KeyCompasStaticInfoAnchoredPosition.Value;
                InfoPanelRect.localScale = HUD.SettingsData.KeyCompassStaticInfoScale.Value;

                bool isCenter = false;
                if (CompassStatics.Count > 0)
                {
                    GamePanelHUDCompassStaticUI[] workUIs = CompassStatics.Values.SelectMany(x => x).Where(x => x.Work).ToArray();

                    if (workUIs.Length > 0)
                    {
                        float[] xDiffs = workUIs.Select(x => x.XDiff).ToArray();

                        //Closest to 0
                        float xDiff = xDiffs.Aggregate((current, next) => Math.Abs(current) < Math.Abs(next) ? current : next);

                        GamePanelHUDCompassStaticUI ui = workUIs[Array.IndexOf(xDiffs, xDiff)];

                        int range = HUD.SettingsData.KeyCompassStaticCenterPointRange.Value;

                        if (xDiff < range && xDiff > -range)
                        {
                            switch (ui.InfoType)
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

                            ui.transform.SetAsLastSibling();

                            string necessaryText = ui.IsNotNecessary ? LocalizedHelp.Localized("(optional)", EStringCase.None) : "";
                            string requirementsText = ui.HasRequirements ? LocalizedHelp.Localized("hideout/Requirements are not fulfilled", EStringCase.None) : "";

                            FontStyles nameStyles = HUD.SettingsData.KeyCompassStaticNameStyles.Value;
                            Color nameColor = HUD.SettingsData.KeyCompassStaticNameColor.Value;

                            _NameValue.fontStyle = nameStyles;
                            _NameValue.color = nameColor;
                            _NameValue.text = LocalizedHelp.Localized(ui.NameKey, EStringCase.None);

                            _NecessaryValue.gameObject.SetActive(ui.IsNotNecessary);
                            _RequirementsValue.gameObject.SetActive(ui.HasRequirements);

                            _NecessaryValue.fontStyle = nameStyles;
                            _NecessaryValue.text = necessaryText;
                            _NecessaryValue.color = nameColor;

                            _RequirementsValue.fontStyle = nameStyles;
                            _RequirementsValue.color = nameColor;
                            _RequirementsValue.text = requirementsText;

                            _DescriptionValue.fontStyle = HUD.SettingsData.KeyCompassStaticDescriptionStyles.Value;
                            _DescriptionValue.color = HUD.SettingsData.KeyCompassStaticDescriptionColor.Value;
                            _DescriptionValue.text = LocalizedHelp.Localized(ui.DescriptionKey, EStringCase.None);

                            FontStyles distanceStyles = HUD.SettingsData.KeyCompassStaticDistanceStyles.Value;
                            Color distanceColor = HUD.SettingsData.KeyCompassStaticDistanceColor.Value;

                            string distance = Vector3.Distance(ui.Where, HUD.Info.PlayerPosition).ToString("F0");

                            _DistanceValue.fontStyle = distanceStyles;
                            _DistanceValue.color = distanceColor;
                            _DistanceValue.text = distance.ToString();

                            _DistanceSignValue.fontStyle = distanceStyles;
                            _DistanceSignValue.color = distanceColor;

                            isCenter = true;
                        }
                    }
                }

                if (!isCenter)
                {
                    _Quests.SetSiblingIndex(0);
                    _Exfiltrations.SetSiblingIndex(1);
                    _Airdrops.SetSiblingIndex(2);
                }

                InfoPanel.gameObject.SetActive(isCenter && HUD.SettingsData.KeyCompassStaticInfoHUDSW.Value);

                DistancePanel.gameObject.SetActive(HUD.SettingsData.KeyCompassStaticDistanceHUDSW.Value);
            }
        }

        void ShowStatic(GamePanelHUDCompassPlugin.CompassStaticInfo staticinfo)
        {
            Transform root;
            switch (staticinfo.InfoType)
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

            switch (staticinfo.InfoType)
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
                    TradersAvatar.GetAvatar(staticinfo.TraderId, _static.BindIcon);
                    break;
            }

            _static.Id = staticinfo.Id;
            _static.Where = staticinfo.Where;
            _static.ZoneId = staticinfo.ZoneId;
            _static.Target = staticinfo.Target;
            _static.NameKey = staticinfo.NameKey;
            _static.TraderId = staticinfo.TraderId;
            _static.IsNotNecessary = staticinfo.IsNotNecessary;
            _static.DescriptionKey = staticinfo.DescriptionKey;
            _static.ExIndex = staticinfo.ExIndex;
            _static.ExIndex2 = staticinfo.ExIndex2;
            _static.InfoType = staticinfo.InfoType;

            if (CompassStatics.TryGetValue(staticinfo.Id, out List<GamePanelHUDCompassStaticUI> list))
            {
                list.Add(_static);
            }
            else
            {
                CompassStatics.Add(staticinfo.Id, new List<GamePanelHUDCompassStaticUI>() { _static });
            }
        }

        void DestroyAll(GameWorld world)
        {
            foreach (var ui in CompassStatics.Values.SelectMany(x => x))
            {
                ui.ToDestroy = true;
            }
        }
#endif

        void DestroyStatic(string id)
        {
            if (CompassStatics.TryGetValue(id, out List<GamePanelHUDCompassStaticUI> list))
            {
                foreach (GamePanelHUDCompassStaticUI ui in list)
                {
                    ui.ToDestroy = true;
                }
            }
        }

        void RemoveStatic(string id)
        {
            CompassStatics.Remove(id);
        }
    }
}
