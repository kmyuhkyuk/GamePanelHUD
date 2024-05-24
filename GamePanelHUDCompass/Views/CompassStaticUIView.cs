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

        public float XLeftDiff { get; private set; }

        public float XRightDiff { get; private set; }

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

        [SerializeField] private Image virtualLeft;

        [SerializeField] private Image virtualRight;

        private Sprite _icon;

        private RectTransform _realRect;

        private RectTransform _virtualLeftRect;

        private RectTransform _virtualRightRect;

        private CanvasGroup _canvasGroup;

        private float _angle;

        private float IconX => -_angle * 8f + 8 * 360;

        private float IconXLeft => IconX - 8 * 360;

        private float IconXRight => IconX + 8 * 360;

#if !UNITY_EDITOR
        private void Awake()
        {
            _canvasGroup = GetComponent<CanvasGroup>();

            _realRect = real.GetComponent<RectTransform>();
            _virtualLeftRect = virtualLeft.GetComponent<RectTransform>();
            _virtualRightRect = virtualRight.GetComponent<RectTransform>();
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

            _angle = compassFireHUDModel.CompassFire.GetToAngle(lhs);

            var iconX = IconX;
            var iconXLeft = IconXLeft;
            var iconXRight = IconXRight;

            XDiff = -iconX - compassHUDModel.Compass.CompassX;
            XLeftDiff = -iconXLeft - compassHUDModel.Compass.CompassX;
            XRightDiff = -iconXRight - compassHUDModel.Compass.CompassX;

            var height = settingsModel.KeyCompassStaticHeight.Value;
            _realRect.anchoredPosition = new Vector2(iconX, height);
            _virtualLeftRect.anchoredPosition = new Vector2(iconXLeft, height);
            _virtualRightRect.anchoredPosition = new Vector2(iconXRight, height);

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
            if (compassStaticHUDModel.CompassStatic.HasEquipmentAndQuestRaidItemHashSet)
            {
                foreach (var id in target)
                {
                    if (compassStaticHUDModel.CompassStatic.EquipmentAndQuestRaidItemHashSet.Contains(id)) 
                        continue;

                    hasItems = false;

                    break;
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
            if (compassStaticHUDModel.CompassStatic.HasEquipmentAndQuestRaidItemHashSet)
            {
                foreach (var id in target)
                {
                    if (!compassStaticHUDModel.CompassStatic.EquipmentAndQuestRaidItemHashSet.Contains(id))
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
            virtualLeft.sprite = _icon;
            virtualRight.sprite = _icon;
        }

        private void Enabled(bool sw)
        {
            Work = sw;
            _canvasGroup.alpha = sw ? 1 : 0;
        }

        private void SetSizeDelta(Vector2 size)
        {
            _realRect.sizeDelta = size;
            _virtualLeftRect.sizeDelta = size;
            _virtualRightRect.sizeDelta = size;
        }

        private void SetNativeSize()
        {
            real.SetNativeSize();
            virtualLeft.SetNativeSize();
            virtualRight.SetNativeSize();
        }

        public void Destroy()
        {
            HUDCoreModel.Instance.UpdateManger.Remove(this);
            Destroy(gameObject);
        }

#endif
    }
}