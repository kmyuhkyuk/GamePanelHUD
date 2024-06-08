using System;
using UnityEngine;
#if !UNITY_EDITOR
using EFTUtils;
using GamePanelHUDCore.Models;
using GamePanelHUDHit.Models;
using SettingsModel = GamePanelHUDHit.Models.SettingsModel;

#endif

namespace GamePanelHUDHit.Views
{
    public class HitHUDView : MonoBehaviour
#if !UNITY_EDITOR
        , IUpdate
#endif
    {
        [SerializeField] private HitUIView hitUIView;

        private HitUIView _testHitUIView;

        private CanvasGroup _hitGroup;

#if !UNITY_EDITOR

        private void Awake()
        {
            _testHitUIView = Instantiate(hitUIView, transform);

            _hitGroup = hitUIView.GetComponent<CanvasGroup>();

            HitHUDModel.Instance.ShowHit = ShowHit;
        }

        private void Start()
        {
            HUDCoreModel.Instance.UpdateManger.Register(this);
        }

        public void CustomUpdate()
        {
            _hitGroup.alpha = HitHUDModel.Instance.HitHUDSw ? 1 : 0;

            HitSet(hitUIView);
            HitSet(_testHitUIView);
        }

        private static void HitSet(HitUIView hitUIView)
        {
            var settingsModel = SettingsModel.Instance;

            hitUIView.damageHUDSw = settingsModel.KeyHitDamageHUDSw.Value;

            hitUIView.hitAnchoredPosition = settingsModel.KeyHitAnchoredPosition.Value;
            hitUIView.hitLocalRotation = settingsModel.KeyHitLocalRotation.Value;
            hitUIView.hitSizeDelta = settingsModel.KeyHitSizeDelta.Value;
            hitUIView.hitLocalScale = settingsModel.KeyHitLocalScale.Value;
            hitUIView.hitHeadSizeDelta = settingsModel.KeyHitHeadSizeDelta.Value;

            hitUIView.hitDamageAnchoredPosition = settingsModel.KeyHitDamageAnchoredPosition.Value;
            hitUIView.hitDamageSizeDelta = settingsModel.KeyHitDamageSizeDelta.Value;
            hitUIView.hitDamageLocalScale = settingsModel.KeyHitDamageLocalScale.Value;

            hitUIView.activeSpeed = settingsModel.KeyHitActiveSpeed.Value;
            hitUIView.endSpeed = settingsModel.KeyHitEndSpeed.Value;
            hitUIView.deadSpeed = settingsModel.KeyHitDeadSpeed.Value;

            hitUIView.damageColor = settingsModel.KeyHitDamageColor.Value;
            hitUIView.armorDamageColor = settingsModel.KeyHitArmorDamageColor.Value;
            hitUIView.deadColor = settingsModel.KeyHitDeadColor.Value;
            hitUIView.headColor = settingsModel.KeyHitHeadColor.Value;
            hitUIView.damageInfoColor = settingsModel.KeyHitDamageInfoColor.Value;
            hitUIView.armorDamageInfoColor = settingsModel.KeyHitArmorDamageInfoColor.Value;

            hitUIView.damageStyles = settingsModel.KeyHitDamageStyles.Value;
            hitUIView.armorDamageStyles = settingsModel.KeyHitArmorDamageStyles.Value;
        }

        private void ShowHit(HitModel hitModel)
        {
            var hitHUDModel = HitHUDModel.Instance;
            var settingsModel = SettingsModel.Instance;

            HitModel.Direction direction;
            if (hitModel.HitDirection.x < settingsModel.KeyHitDirectionLeft.Value)
            {
                direction = HitModel.Direction.Left;
            }
            else if (hitModel.HitDirection.x > settingsModel.KeyHitDirectionRight.Value)
            {
                direction = HitModel.Direction.Right;
            }
            else
            {
                direction = HitModel.Direction.Center;
            }

            if (!hitModel.IsTest)
            {
                var cam = Camera.main;

                if (cam != null)
                {
                    var pos = cam.WorldToScreenPoint(hitModel.HitPoint);

                    var screenPos = new Vector2(pos.x - cam.pixelWidth * 0.5f, pos.y - cam.pixelHeight * 0.5f);

                    hitUIView.transform.localPosition =
                        new Vector2((int)Math.Round(screenPos.x), (int)Math.Round(screenPos.y));
                }

                Hit(hitModel, direction, settingsModel, hitUIView);
            }
            else
            {
                _testHitUIView.transform.localPosition = new Vector2(hitHUDModel.ScreenRect.sizeDelta.x / 3.2f, 0);

                Hit(hitModel, direction, settingsModel, _testHitUIView);
            }
        }

        private static void Hit(HitModel hitModel,
            HitModel.Direction direction, SettingsModel settingsModel,
            HitUIView hitUIView)
        {
            hitUIView.damage = hitModel.Damage;
            hitUIView.armorDamage = hitModel.ArmorDamage;

            var isHead = hitModel.DamagePart == EBodyPart.Head;

            if (settingsModel.KeyHitHasHead.Value && isHead && hitModel.HitType != HitModel.Hit.Dead)
            {
                hitModel.HitType = HitModel.Hit.Head;
            }

            hitUIView.hasArmorHit = hitModel.HasArmorHit;

            hitUIView.CustomUpdate();

            if (hitModel.HitType == HitModel.Hit.Dead)
            {
                hitUIView.HitDeadTrigger();
            }

            if (!settingsModel.KeyHitHasDirection.Value)
            {
                direction = HitModel.Direction.Center;
            }

            hitUIView.HitTrigger(isHead, hitModel, direction);
        }

#endif
    }
}