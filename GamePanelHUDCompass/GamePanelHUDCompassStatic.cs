using System;
using System.Text;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using EFT;
using EFT.UI;
using GamePanelHUDCore;
using GamePanelHUDCore.Utils;
#if !UNITY_EDITOR
using GamePanelHUDCore.Utils.Session;
#endif

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
        private TMP_Text _Name;

        [SerializeField]
        private TMP_Text _Description;

        private Transform InfoPanel;

        private RectTransform InfoPanelRect;

        private CanvasGroup CompassStaticGroup;

        private CanvasGroup AirdropsGroup;

        private CanvasGroup QuestsGroup;

        private CanvasGroup ExfiltrationGroup;

        private readonly StringBuilderData StringBuilderDatas = new StringBuilderData();

        internal static Action<string> Remove;

#if !UNITY_EDITOR
        void Start()
        {
            CompassStaticGroup = _CompassStatic.GetComponent<CanvasGroup>();
            AirdropsGroup = _Airdrops.GetComponent<CanvasGroup>();
            QuestsGroup = _Quests.GetComponent<CanvasGroup>();
            ExfiltrationGroup = _Exfiltrations.GetComponent<CanvasGroup>();

            InfoPanel = _Name.transform.parent;
            InfoPanelRect = InfoPanel.GetComponent<RectTransform>();    

            Remove = RemoveStatic;
            GamePanelHUDCompassPlugin.ShowStatic = ShowStatic;
            GamePanelHUDCompassPlugin.DestroyStatic = DestroyStatic;
            GamePanelHUDCorePlugin.HUDCoreClass.WorldDispose += DestroyAll;

            GamePanelHUDCorePlugin.UpdateManger.Register(this);
        }

        public void IUpdate()
#endif
#if UNITY_EDITOR
        void Update()
#endif
        {
            CompassStaticHUD();
        }

        void CompassStaticHUD()
        {
#if !UNITY_EDITOR
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
                    GamePanelHUDCompassStaticUI[] workUI = CompassStatics.Values.SelectMany(x => x).Where(x => x.Work).ToArray();

                    if (workUI.Length > 0)
                    {
                        float[] xDiffs = workUI.Select(x => x.XDiff).ToArray();

                        float xDiff = xDiffs.Aggregate((current, next) => Math.Abs(current) < Math.Abs(next) ? current : next);

                        GamePanelHUDCompassStaticUI ui = workUI[Array.IndexOf(xDiffs, xDiff)];

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
                                case GamePanelHUDCompassPlugin.CompassStaticInfo.Type.ConditionFindItem:
                                case GamePanelHUDCompassPlugin.CompassStaticInfo.Type.ConditionVisitPlace:
                                case GamePanelHUDCompassPlugin.CompassStaticInfo.Type.ConditionInZone:
                                    _Quests.SetAsLastSibling();
                                    break;
                            }

                            ui.transform.SetAsLastSibling();

                            string necessary = ui.IsNotNecessary ? LocalizedHelp.Localized("(optional)") : "";

                            _Name.fontStyle = HUD.SettingsData.KeyCompassStaticNameStyles.Value;
                            if (ui.HasRequirement)
                            {
                                _Name.text = StringBuilderDatas._Name.StringConcat("<color=", HUD.SettingsData.KeyCompassStaticNameColor.Value.ColorToHtml(), ">", LocalizedHelp.Localized(ui.NameKey), necessary, "(", LocalizedHelp.Localized("hideout/Requirements are not fulfilled"), ")", "</color>");
                            }
                            else
                            {
                                _Name.text = StringBuilderDatas._Name.StringConcat("<color=", HUD.SettingsData.KeyCompassStaticNameColor.Value.ColorToHtml(), ">", LocalizedHelp.Localized(ui.NameKey), necessary, "</color>");
                            }

                            _Description.fontStyle = HUD.SettingsData.KeyCompassStaticDescriptionStyles.Value;
                            _Description.text = StringBuilderDatas._Description.StringConcat("<color=", HUD.SettingsData.KeyCompassStaticDescriptionColor.Value.ColorToHtml(), ">", LocalizedHelp.Localized(ui.DescriptionKey), "</color>");

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

                InfoPanel.gameObject.SetActive(HUD.SettingsData.KeyCompassStaticInfoHUDSW.Value && isCenter);
            }
#endif
        }

#if !UNITY_EDITOR
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

        public class StringBuilderData
        {
            public StringBuilder _Name = new StringBuilder(128);
            public StringBuilder _Description = new StringBuilder(128);
        }
    }
}
