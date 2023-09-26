using System;
using UnityEngine;
#if !UNITY_EDITOR
using EFTUtils;
using GamePanelHUDCore;
#endif

namespace GamePanelHUDHit
{
    public class GamePanelHUDHit : MonoBehaviour
#if !UNITY_EDITOR
        , IUpdate
#endif
    {
#if !UNITY_EDITOR
        private static GamePanelHUDCorePlugin.HUDCoreClass HUDCore => GamePanelHUDCorePlugin.HUDCore;
        private static GamePanelHUDCorePlugin.HUDClass<RectTransform, GamePanelHUDHitPlugin.SettingsData> HUD =>
            GamePanelHUDHitPlugin.HitHUD;
#endif

        [SerializeField] private GamePanelHUDHitUI hit;

        private GamePanelHUDHitUI _testHit;

        private CanvasGroup _hitGroup;

#if !UNITY_EDITOR
        private void Start()
        {
            _testHit = Instantiate(hit, transform);

            _hitGroup = hit.GetComponent<CanvasGroup>();

            GamePanelHUDHitPlugin.ShowHit = ShowHit;

            HUDCore.UpdateManger.Register(this);
        }

        public void CustomUpdate()
        {
            HitHUD();
        }

        private void HitHUD()
        {
            _hitGroup.alpha = HUD.HUDSw ? 1 : 0;

            HitSet(hit);
            HitSet(_testHit);
        }

        private static void HitSet(GamePanelHUDHitUI hit)
        {
            hit.damageHUDSw = HUD.SetData.KeyHitDamageHUDSw.Value;

            hit.hitAnchoredPosition = HUD.SetData.KeyHitAnchoredPosition.Value;
            hit.hitLocalRotation = HUD.SetData.KeyHitLocalRotation.Value;
            hit.hitSizeDelta = HUD.SetData.KeyHitSizeDelta.Value;
            hit.hitLocalScale = HUD.SetData.KeyHitLocalScale.Value;
            hit.hitHeadSizeDelta = HUD.SetData.KeyHitHeadSizeDelta.Value;

            hit.hitDamageAnchoredPosition = HUD.SetData.KeyHitDamageAnchoredPosition.Value;
            hit.hitDamageSizeDelta = HUD.SetData.KeyHitDamageSizeDelta.Value;
            hit.hitDamageLocalScale = HUD.SetData.KeyHitDamageLocalScale.Value;

            hit.activeSpeed = HUD.SetData.KeyHitActiveSpeed.Value;
            hit.endSpeed = HUD.SetData.KeyHitEndSpeed.Value;
            hit.deadSpeed = HUD.SetData.KeyHitDeadSpeed.Value;

            hit.damageColor = HUD.SetData.KeyHitDamageColor.Value;
            hit.armorDamageColor = HUD.SetData.KeyHitArmorDamageColor.Value;
            hit.deadColor = HUD.SetData.KeyHitDeadColor.Value;
            hit.headColor = HUD.SetData.KeyHitHeadColor.Value;
            hit.damageInfoColor = HUD.SetData.KeyHitDamageInfoColor.Value;
            hit.armorDamageInfoColor = HUD.SetData.KeyHitArmorDamageInfoColor.Value;

            hit.damageStyles = HUD.SetData.KeyHitDamageStyles.Value;
            hit.armorDamageStyles = HUD.SetData.KeyHitArmorDamageStyles.Value;
        }

        private void ShowHit(GamePanelHUDHitPlugin.HitInfo hitInfo)
        {
            GamePanelHUDHitPlugin.HitInfo.Direction direction;

            if (hitInfo.HitDirection.x < HUD.SetData.KeyHitDirectionLeft.Value)
            {
                direction = GamePanelHUDHitPlugin.HitInfo.Direction.Left;
            }
            else if (hitInfo.HitDirection.x > HUD.SetData.KeyHitDirectionRight.Value)
            {
                direction = GamePanelHUDHitPlugin.HitInfo.Direction.Right;
            }
            else
            {
                direction = GamePanelHUDHitPlugin.HitInfo.Direction.Center;
            }

            if (!hitInfo.IsTest)
            {
                var cam = Camera.main;

                if (cam != null)
                {
                    var pos = cam.WorldToScreenPoint(hitInfo.HitPoint);

                    var screenPos = new Vector2(pos.x - cam.pixelWidth * 0.5f, pos.y - cam.pixelHeight * 0.5f);

                    hit.transform.localPosition =
                        new Vector2((int)Math.Round(screenPos.x), (int)Math.Round(screenPos.y));
                }

                Hit(hitInfo, direction, HUD.SetData, hit);
            }
            else
            {
                _testHit.transform.localPosition = new Vector2(HUD.Info.sizeDelta.x / 3.2f, 0);

                Hit(hitInfo, direction, HUD.SetData, _testHit);
            }
        }

        private static void Hit(GamePanelHUDHitPlugin.HitInfo hitInfo,
            GamePanelHUDHitPlugin.HitInfo.Direction direction, GamePanelHUDHitPlugin.SettingsData setData,
            GamePanelHUDHitUI hit)
        {
            hit.damage = hitInfo.Damage;
            hit.armorDamage = hitInfo.ArmorDamage;

            var isHead = hitInfo.DamagePart == EBodyPart.Head;

            if (setData.KeyHitHasHead.Value && isHead && hitInfo.HitType != GamePanelHUDHitPlugin.HitInfo.Hit.Dead)
            {
                hitInfo.HitType = GamePanelHUDHitPlugin.HitInfo.Hit.Head;
            }

            hit.hasArmorHit = hitInfo.HasArmorHit;

            hit.CustomUpdate();

            if (hitInfo.HitType == GamePanelHUDHitPlugin.HitInfo.Hit.Dead)
            {
                hit.HitDeadTrigger();
            }

            if (!setData.KeyHitHasDirection.Value)
            {
                direction = GamePanelHUDHitPlugin.HitInfo.Direction.Center;
            }

            hit.HitTrigger(isHead, hitInfo, direction);
        }
#endif
    }
}