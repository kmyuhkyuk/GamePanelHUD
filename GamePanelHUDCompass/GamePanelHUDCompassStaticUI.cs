using UnityEngine;
using UnityEngine.UI;
#if !UNITY_EDITOR
using GamePanelHUDCore;
#endif
using GamePanelHUDCore.Utils;

namespace GamePanelHUDCompass
{
    public class GamePanelHUDCompassStaticUI : MonoBehaviour
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

        public GamePanelHUDCompassPlugin.CompassStaticInfo.Type InfoType;
#endif

        public bool Work { get; private set; }

        public float XDiff { get; private set; }

        public bool HasRequirements { get; private set; }

        public string Id;

        public Vector3 Where;

        public string ZoneId;

        public string[] Target;

        public string NameKey;

        public string DescriptionKey;

        public string TraderId;

        public bool IsNotNecessary;

        public int ExIndex;

        public int ExIndex2;

        public bool ToDestroy;

        [SerializeField]
        private Image _Real;

        [SerializeField]
        private Image _Virtual;

        [SerializeField]
        private Image _Virtual2;

        [SerializeField]
        private Image _Virtual3;

        private Sprite Icon;

        private RectTransform RealRect;

        private RectTransform VirtualRect;

        private RectTransform Virtual2Rect;

        private RectTransform Virtual3Rect;

        private CanvasGroup CanvasGroup;

        private float IconX;

        private float IconXLeft
        {
            get
            {
                return IconX - 2880;
            }
        }

        private float IconXRight
        {
            get
            {
                return IconX + 2880;
            }
        }

        private float IconXRightRight
        {
            get
            {
                return IconX + 5760; //2880 * 2
            }
        }

#if !UNITY_EDITOR
        void Start()
        {
            CanvasGroup = GetComponent<CanvasGroup>();

            RealRect = _Real.GetComponent<RectTransform>();
            VirtualRect = _Virtual.GetComponent<RectTransform>();
            Virtual2Rect = _Virtual2.GetComponent<RectTransform>();
            Virtual3Rect = _Virtual3.GetComponent<RectTransform>();

            switch (InfoType)
            {
                case GamePanelHUDCompassPlugin.CompassStaticInfo.Type.Airdrop:
                    SetSizeDelta(new Vector2(32, 24));
                    break;
                case GamePanelHUDCompassPlugin.CompassStaticInfo.Type.Exfiltration:
                case GamePanelHUDCompassPlugin.CompassStaticInfo.Type.Switch:
                    SetNativeSize();
                    break;
                case GamePanelHUDCompassPlugin.CompassStaticInfo.Type.ConditionLeaveItemAtLocation:
                case GamePanelHUDCompassPlugin.CompassStaticInfo.Type.ConditionPlaceBeacon:
                case GamePanelHUDCompassPlugin.CompassStaticInfo.Type.ConditionFindItem:
                case GamePanelHUDCompassPlugin.CompassStaticInfo.Type.ConditionVisitPlace:
                case GamePanelHUDCompassPlugin.CompassStaticInfo.Type.ConditionInZone:
                    SetSizeDelta(new Vector2(32, 32));
                    break;
            }

            GamePanelHUDCorePlugin.UpdateManger.Register(this);
        }

        public void IUpdate()
#endif
#if UNITY_EDITOR
        void Update()
#endif
        {
            CompassStaticUI();
        }

        void CompassStaticUI()
        {
#if !UNITY_EDITOR
            Vector3 lhs = Where - HUD.Info.PlayerPosition;

            float angle = HUD.Info.GetToAngle(lhs);

            IconX = -(angle / 15 * 120);

            float iconXLeft = IconXLeft;
            float iconXRight = IconXRight;
            float iconXRightRight = IconXRightRight;

            //Center always is Virtual2
            XDiff = -iconXRight - HUD.Info.CompassX;

            float height = HUD.SettingsData.KeyCompassStaticHeight.Value;
            RealRect.anchoredPosition = new Vector2(IconX, height);
            VirtualRect.anchoredPosition = new Vector2(iconXLeft, height);
            Virtual2Rect.anchoredPosition = new Vector2(iconXRight, height);
            Virtual3Rect.anchoredPosition = new Vector2(iconXRightRight, height);

            switch (InfoType)
            {
                case GamePanelHUDCompassPlugin.CompassStaticInfo.Type.Exfiltration:
                    HUD.Info.ExfiltrationGetStatus(ExIndex, out bool notPresent, out bool hasRequirements);
                    HasRequirements = hasRequirements;
                    if (HUD.SettingsData.KeyCompassStaticHideRequirements.Value)
                    {
                        Enabled(!hasRequirements && !notPresent);
                    }
                    else
                    {
                        Enabled(!notPresent);
                    }
                    break;
                case GamePanelHUDCompassPlugin.CompassStaticInfo.Type.Switch:
                    HUD.Info.ExfiltrationGetStatus(ExIndex, out notPresent, out hasRequirements);
                    HUD.Info.ExfiltrationGetSwitch(ExIndex, ExIndex2, out bool open);
                    if (HUD.SettingsData.KeyCompassStaticHideRequirements.Value)
                    {
                        Enabled(!hasRequirements && !open);
                    }
                    else
                    {
                        Enabled(!open);
                    }
                    break;
                case GamePanelHUDCompassPlugin.CompassStaticInfo.Type.ConditionLeaveItemAtLocation:
                case GamePanelHUDCompassPlugin.CompassStaticInfo.Type.ConditionPlaceBeacon:
                    if (HUD.Info.HasEquipmentAndQuestRaidItems)
                    {
                        bool hasItems = true;
                        foreach (string id in Target)
                        {
                            if (!HUD.Info.EquipmentAndQuestRaidItems.Contains(id))
                            {
                                hasItems = false;
                            }
                        }
                        HasRequirements = !hasItems;
                    }
                    if (HUD.SettingsData.KeyCompassStaticHideRequirements.Value)
                    {
                        Enabled(!HasRequirements);
                    }
                    else if (HUD.SettingsData.KeyCompassStaticHideOptional.Value)
                    {
                        Enabled(!IsNotNecessary);
                    }
                    else
                    {
                        Enabled(true);
                    }
                    break;
                case GamePanelHUDCompassPlugin.CompassStaticInfo.Type.ConditionFindItem:
                case GamePanelHUDCompassPlugin.CompassStaticInfo.Type.ConditionVisitPlace:
                case GamePanelHUDCompassPlugin.CompassStaticInfo.Type.ConditionInZone:
                    if (HUD.SettingsData.KeyCompassStaticHideOptional.Value)
                    {
                        Enabled(!IsNotNecessary);
                    }
                    else
                    {
                        Enabled(true);
                    }
                    break;
                case GamePanelHUDCompassPlugin.CompassStaticInfo.Type.Airdrop:
                    Enabled(true);
                    break;
            }

            if (ToDestroy)
            {
                Destroy();
            }
#endif
        }

        public void BindIcon(Sprite sprite)
        {
            Icon = sprite;

            _Real.sprite = Icon;
            _Virtual.sprite = Icon;
            _Virtual2.sprite = Icon;
            _Virtual3.sprite = Icon;
        }

        void Enabled(bool sw)
        {
            Work = sw;
            CanvasGroup.alpha = sw ? 1 : 0;
        }

        void SetSizeDelta(Vector2 size)
        {
            RealRect.sizeDelta = size;
            VirtualRect.sizeDelta = size;
            Virtual2Rect.sizeDelta = size;
            Virtual3Rect.sizeDelta = size;
        }

        void SetNativeSize()
        {
            _Real.SetNativeSize();
            _Virtual.SetNativeSize();
            _Virtual2.SetNativeSize();
            _Virtual3.SetNativeSize();
        }

        void Destroy()
        {
#if !UNITY_EDITOR
            GamePanelHUDCompassStatic.Remove(Id);
            GamePanelHUDCorePlugin.UpdateManger.Remove(this);
            Destroy(gameObject);
#endif
        }
    }
}
