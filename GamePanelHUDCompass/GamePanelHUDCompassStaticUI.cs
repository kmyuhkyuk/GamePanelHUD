using System;
using UnityEngine;
using UnityEngine.UI;
#if !UNITY_EDITOR
using EFTUtils;
using GamePanelHUDCore;

#endif

namespace GamePanelHUDCompass
{
    public class GamePanelHUDCompassStaticUI : MonoBehaviour
#if !UNITY_EDITOR
        , IUpdate
#endif
    {
#if !UNITY_EDITOR

        private static GamePanelHUDCorePlugin.HUDCoreClass HUDCore => GamePanelHUDCorePlugin.HUDCore;

        private static
            GamePanelHUDCorePlugin.HUDClass<GamePanelHUDCompassPlugin.CompassStaticData,
                GamePanelHUDCompassPlugin.SettingsData> HUD => GamePanelHUDCompassPlugin.CompassStaticHUD;

        public GamePanelHUDCompassPlugin.CompassStaticInfo.Type infoType;

#endif
        public bool Work { get; private set; }

        public float XDiff { get; private set; }

        public bool HasRequirements { get; private set; }

        public Vector3 where;

        public string[] target;

        public string nameKey;

        public string descriptionKey;

        public bool isNotNecessary;

        public Func<bool>[] Requirements;

        [SerializeField] private Image real;

        [SerializeField] private Image @virtual;

        [SerializeField] private Image virtual2;

        [SerializeField] private Image virtual3;

        private Sprite _icon;

        private RectTransform _realRect;

        private RectTransform _virtualRect;

        private RectTransform _virtual2Rect;

        private RectTransform _virtual3Rect;

        private CanvasGroup _canvasGroup;

        private float _iconX;

        private float IconXLeft => _iconX - 2880;

        private float IconXRight => _iconX + 2880;

        private float IconXRightRight => _iconX + 5760; //2880 * 2

#if !UNITY_EDITOR

        private void Start()
        {
            _canvasGroup = GetComponent<CanvasGroup>();

            _realRect = real.GetComponent<RectTransform>();
            _virtualRect = @virtual.GetComponent<RectTransform>();
            _virtual2Rect = virtual2.GetComponent<RectTransform>();
            _virtual3Rect = virtual3.GetComponent<RectTransform>();

            switch (infoType)
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
                default:
                    throw new ArgumentOutOfRangeException(nameof(infoType), infoType, null);
            }

            HUDCore.UpdateManger.Register(this);
        }

        private void OnEnable()
        {
            Work = true;
            HUDCore.UpdateManger.Run(this);
        }

        private void OnDisable()
        {
            Work = false;
            HUDCore.UpdateManger.Stop(this);
        }

        public void CustomUpdate()
#endif
#if UNITY_EDITOR
        void Update()
#endif
        {
            CompassStaticUI();
        }

        private void CompassStaticUI()
        {
#if !UNITY_EDITOR

            var lhs = where - HUD.Info.CameraPosition;

            var angle = HUD.Info.GetToAngle(lhs);

            _iconX = -(angle / 15 * 120);

            var iconXLeft = IconXLeft;
            var iconXRight = IconXRight;
            var iconXRightRight = IconXRightRight;

            //Center always is Virtual2
            XDiff = -iconXRight - HUD.Info.CompassX;

            var height = HUD.SetData.KeyCompassStaticHeight.Value;
            _realRect.anchoredPosition = new Vector2(_iconX, height);
            _virtualRect.anchoredPosition = new Vector2(iconXLeft, height);
            _virtual2Rect.anchoredPosition = new Vector2(iconXRight, height);
            _virtual3Rect.anchoredPosition = new Vector2(iconXRightRight, height);

            switch (infoType)
            {
                case GamePanelHUDCompassPlugin.CompassStaticInfo.Type.Exfiltration:
                    Exfiltration();
                    break;
                case GamePanelHUDCompassPlugin.CompassStaticInfo.Type.Switch:
                    Switch();
                    break;
                case GamePanelHUDCompassPlugin.CompassStaticInfo.Type.ConditionFindItem:
                    if (HUD.SetData.KeyConditionFindItem.Value)
                    {
                        FindItem();
                    }
                    else
                    {
                        Enabled(false);
                    }

                    break;
                case GamePanelHUDCompassPlugin.CompassStaticInfo.Type.ConditionLeaveItemAtLocation:
                    if (HUD.SetData.KeyConditionLeaveItemAtLocation.Value)
                    {
                        PlaceItem();
                    }
                    else
                    {
                        Enabled(false);
                    }

                    break;
                case GamePanelHUDCompassPlugin.CompassStaticInfo.Type.ConditionPlaceBeacon:
                    if (HUD.SetData.KeyConditionPlaceBeacon.Value)
                    {
                        PlaceItem();
                    }
                    else
                    {
                        Enabled(false);
                    }

                    break;
                case GamePanelHUDCompassPlugin.CompassStaticInfo.Type.ConditionVisitPlace:
                    if (HUD.SetData.KeyConditionVisitPlace.Value)
                    {
                        Other();
                    }
                    else
                    {
                        Enabled(false);
                    }

                    break;
                case GamePanelHUDCompassPlugin.CompassStaticInfo.Type.ConditionInZone:
                    if (HUD.SetData.KeyConditionInZone.Value)
                    {
                        Other();
                    }
                    else
                    {
                        Enabled(false);
                    }

                    break;
                case GamePanelHUDCompassPlugin.CompassStaticInfo.Type.Airdrop:
                    Airdrop();
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(infoType), infoType, null);
            }

#endif
        }

#if !UNITY_EDITOR

        private void Exfiltration()
        {
            var notPresent = Requirements[0]();
            var hasRequirements = Requirements[1]();

            HasRequirements = hasRequirements;

            if (HUD.SetData.KeyCompassStaticHideRequirements.Value)
            {
                Enabled(!hasRequirements && !notPresent);
            }
            else
            {
                Enabled(!notPresent);
            }
        }

        private void Switch()
        {
            var hasRequirements = Requirements[1]();
            var open = Requirements[2]();

            if (HUD.SetData.KeyCompassStaticHideRequirements.Value)
            {
                Enabled(!hasRequirements && !open);
            }
            else
            {
                Enabled(!open);
            }
        }

        private void PlaceItem()
        {
            var hasItems = true;
            if (HUD.Info.HasEquipmentAndQuestRaidItems)
            {
                foreach (var id in target)
                {
                    if (!HUD.Info.EquipmentAndQuestRaidItems.Contains(id))
                    {
                        hasItems = false;
                    }
                }
            }

            HasRequirements = !hasItems;

            if (HUD.SetData.KeyCompassStaticHideRequirements.Value)
            {
                Enabled(!HasRequirements);
            }
            else if (HUD.SetData.KeyCompassStaticHideOptional.Value)
            {
                Enabled(!isNotNecessary);
            }
            else
            {
                Enabled(true);
            }
        }

        private void FindItem()
        {
            var hasItems = true;
            if (HUD.Info.HasEquipmentAndQuestRaidItems)
            {
                foreach (var id in target)
                {
                    if (!HUD.Info.EquipmentAndQuestRaidItems.Contains(id))
                    {
                        hasItems = false;
                    }
                }
            }

            if (HUD.SetData.KeyCompassStaticHideOptional.Value)
            {
                Enabled(!isNotNecessary);
            }
            else
            {
                Enabled(!hasItems);
            }
        }

        private void Other()
        {
            if (HUD.SetData.KeyCompassStaticHideOptional.Value)
            {
                Enabled(!isNotNecessary);
            }
            else
            {
                Enabled(true);
            }
        }

        private void Airdrop()
        {
            if (HUD.SetData.KeyCompassStaticHideSearchedAirdrop.Value)
            {
                Enabled(!Requirements[0]());
            }
            else
            {
                Enabled(true);
            }
        }

#endif

        public void BindIcon(Sprite sprite)
        {
            _icon = sprite;

            real.sprite = _icon;
            @virtual.sprite = _icon;
            virtual2.sprite = _icon;
            virtual3.sprite = _icon;
        }

        private void Enabled(bool sw)
        {
            Work = sw;
            _canvasGroup.alpha = sw ? 1 : 0;
        }

        private void SetSizeDelta(Vector2 size)
        {
            _realRect.sizeDelta = size;
            _virtualRect.sizeDelta = size;
            _virtual2Rect.sizeDelta = size;
            _virtual3Rect.sizeDelta = size;
        }

        private void SetNativeSize()
        {
            real.SetNativeSize();
            @virtual.SetNativeSize();
            virtual2.SetNativeSize();
            virtual3.SetNativeSize();
        }

        public void Destroy()
        {
#if !UNITY_EDITOR

            HUDCore.UpdateManger.Remove(this);
            Destroy(gameObject);

#endif
        }
    }
}