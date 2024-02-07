using System;
using UnityEngine;
using UnityEngine.UI;
#if !UNITY_EDITOR
using EFTUtils;
using GamePanelHUDCore.Models;
using GamePanelHUDCompass.Models;
using SettingsModel = GamePanelHUDCompass.Models.SettingsModel;

#endif

namespace GamePanelHUDCompass.Views
{
    public class CompassStaticUIView : MonoBehaviour
#if !UNITY_EDITOR
        , IUpdate
#endif
    {
#if !UNITY_EDITOR

        internal StaticModel.Type InfoType;

#endif

        public bool Work { get; private set; }

        public float XDiff { get; private set; }

        public bool HasRequirements { get; private set; }

        public bool InZone { get; private set; }

        public Vector3 where;

        public string[] target;

        public string nameKey;

        public string descriptionKey;

        public string zoneId;

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

        private float IconXRightRight => _iconX + 2880 * 2;

#if !UNITY_EDITOR
        private void Awake()
        {
            _canvasGroup = GetComponent<CanvasGroup>();

            _realRect = real.GetComponent<RectTransform>();
            _virtualRect = @virtual.GetComponent<RectTransform>();
            _virtual2Rect = virtual2.GetComponent<RectTransform>();
            _virtual3Rect = virtual3.GetComponent<RectTransform>();
        }

        private void Start()
        {
            switch (InfoType)
            {
                case StaticModel.Type.Airdrop:
                    SetSizeDelta(new Vector2(32, 24));
                    break;
                case StaticModel.Type.Exfiltration:
                case StaticModel.Type.Switch:
                    SetNativeSize();
                    break;
                case StaticModel.Type.ConditionLeaveItemAtLocation:
                case StaticModel.Type.ConditionPlaceBeacon:
                case StaticModel.Type.ConditionFindItem:
                case StaticModel.Type.ConditionVisitPlace:
                case StaticModel.Type.ConditionInZone:
                    SetSizeDelta(new Vector2(32, 32));
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(InfoType), InfoType, null);
            }

            HUDCoreModel.Instance.UpdateManger.Register(this);
        }

        private void OnEnable()
        {
            Work = true;
            HUDCoreModel.Instance.UpdateManger.Run(this);
        }

        private void OnDisable()
        {
            Work = false;
            HUDCoreModel.Instance.UpdateManger.Stop(this);
        }

        public void CustomUpdate()
        {
            var compassHUDModel = CompassHUDModel.Instance;
            var compassFireHUDModel = CompassFireHUDModel.Instance;
            var compassStaticHUDModel = CompassStaticHUDModel.Instance;
            var settingsModel = SettingsModel.Instance;

            var lhs = where - compassFireHUDModel.CompassFire.CameraPosition;

            var angle = compassFireHUDModel.CompassFire.GetToAngle(lhs);

            _iconX = -(angle / 15 * 120);

            var iconXLeft = IconXLeft;
            var iconXRight = IconXRight;
            var iconXRightRight = IconXRightRight;

            //Center always is Virtual2
            XDiff = -iconXRight - compassHUDModel.Compass.CompassX;

            var height = settingsModel.KeyCompassStaticHeight.Value;
            _realRect.anchoredPosition = new Vector2(_iconX, height);
            _virtualRect.anchoredPosition = new Vector2(iconXLeft, height);
            _virtual2Rect.anchoredPosition = new Vector2(iconXRight, height);
            _virtual3Rect.anchoredPosition = new Vector2(iconXRightRight, height);

            switch (InfoType)
            {
                case StaticModel.Type.Exfiltration:
                    Exfiltration();
                    break;
                case StaticModel.Type.Switch:
                    Switch();
                    break;
                case StaticModel.Type.ConditionFindItem:
                    if (settingsModel.KeyConditionFindItem.Value)
                    {
                        FindItem();
                    }
                    else
                    {
                        Enabled(false);
                    }

                    break;
                case StaticModel.Type.ConditionLeaveItemAtLocation:
                    if (settingsModel.KeyConditionLeaveItemAtLocation.Value)
                    {
                        PlaceItem();
                    }
                    else
                    {
                        Enabled(false);
                    }

                    break;
                case StaticModel.Type.ConditionPlaceBeacon:
                    if (settingsModel.KeyConditionPlaceBeacon.Value)
                    {
                        PlaceItem();
                    }
                    else
                    {
                        Enabled(false);
                    }

                    break;
                case StaticModel.Type.ConditionVisitPlace:
                    if (settingsModel.KeyConditionVisitPlace.Value)
                    {
                        Other();
                    }
                    else
                    {
                        Enabled(false);
                    }

                    break;
                case StaticModel.Type.ConditionInZone:
                    if (settingsModel.KeyConditionInZone.Value)
                    {
                        InZone = compassStaticHUDModel.CompassStatic.TriggerZones.Contains(zoneId);

                        Other();
                    }
                    else
                    {
                        Enabled(false);
                    }

                    break;
                case StaticModel.Type.Airdrop:
                    Airdrop();
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(InfoType), InfoType, null);
            }
        }

        private void Exfiltration()
        {
            var notPresent = Requirements[0]();
            var hasRequirements = Requirements[1]();

            HasRequirements = hasRequirements;

            if (SettingsModel.Instance.KeyCompassStaticHideRequirements.Value)
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

            if (SettingsModel.Instance.KeyCompassStaticHideRequirements.Value)
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
            var compassStaticHUDModel = CompassStaticHUDModel.Instance;
            var settingsModel = SettingsModel.Instance;

            var hasItems = true;
            if (compassStaticHUDModel.CompassStatic.HasEquipmentAndQuestRaidItems)
            {
                foreach (var id in target)
                {
                    if (!compassStaticHUDModel.CompassStatic.EquipmentAndQuestRaidItems.Contains(id))
                    {
                        hasItems = false;
                    }
                }
            }

            HasRequirements = !hasItems;

            var needHideRequirements = settingsModel.KeyCompassStaticHideRequirements.Value && HasRequirements;

            var needHideOptional = settingsModel.KeyCompassStaticHideOptional.Value && isNotNecessary;

            if (needHideRequirements || needHideOptional)
            {
                Enabled(false);
            }
            else
            {
                Enabled(true);
            }
        }

        private void FindItem()
        {
            var compassStaticHUDModel = CompassStaticHUDModel.Instance;
            var settingsModel = SettingsModel.Instance;

            var hasItems = true;
            if (compassStaticHUDModel.CompassStatic.HasEquipmentAndQuestRaidItems)
            {
                foreach (var id in target)
                {
                    if (!compassStaticHUDModel.CompassStatic.EquipmentAndQuestRaidItems.Contains(id))
                    {
                        hasItems = false;
                    }
                }
            }

            if (settingsModel.KeyCompassStaticHideOptional.Value)
            {
                Enabled(!isNotNecessary && !hasItems);
            }
            else
            {
                Enabled(!hasItems);
            }
        }

        private void Other()
        {
            if (SettingsModel.Instance.KeyCompassStaticHideOptional.Value)
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
            if (SettingsModel.Instance.KeyCompassStaticHideSearchedAirdrop.Value)
            {
                Enabled(!Requirements[0]());
            }
            else
            {
                Enabled(true);
            }
        }

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
            HUDCoreModel.Instance.UpdateManger.Remove(this);
            Destroy(gameObject);
        }

#endif
    }
}