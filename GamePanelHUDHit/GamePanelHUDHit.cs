using System;
using UnityEngine;
using EFT.UI;
#if !UNITY_EDITOR
using GamePanelHUDCore;
#endif
using GamePanelHUDCore.Utils;

namespace GamePanelHUDHit
{
    public class GamePanelHUDHit : UIElement
#if !UNITY_EDITOR
        , IUpdate
#endif
    {
#if !UNITY_EDITOR
        private GamePanelHUDCorePlugin.HUDClass<RectTransform, GamePanelHUDHitPlugin.SettingsData> HUD => GamePanelHUDHitPlugin.HitHUD;
#endif

        [SerializeField]
        private GamePanelHUDHitUI _Hit;

        private GamePanelHUDHitUI _TestHit;

        private CanvasGroup HitGroup;

#if !UNITY_EDITOR
        void Start()
        {
            if (_Hit != null)
            {
                _TestHit = Instantiate(_Hit, transform);

                HitGroup = _Hit.GetComponent<CanvasGroup>();
            }

            GamePanelHUDHitPlugin.ShowHit = ShowHit;

            GamePanelHUDCorePlugin.UpdateManger.Register(this);
        }

        public void IUpdate()
        {
            HitHUD();
        }

        void HitHUD()
        {
            if (_Hit != null)
            {
                HitGroup.alpha = HUD.HUDSw ? 1 : 0;

                HitSet(_Hit);
                HitSet(_TestHit);
            }
        }

        void HitSet(GamePanelHUDHitUI _hit)
        {
            _hit.DamageHUDSw = HUD.SetData.KeyHitDamageHUDSw.Value;

            _hit.HitAnchoredPosition = HUD.SetData.KeyHitAnchoredPosition.Value;
            _hit.HitLocalRotation = HUD.SetData.KeyHitLocalRotation.Value;
            _hit.HitSizeDelta = HUD.SetData.KeyHitSizeDelta.Value;
            _hit.HitLocalScale = HUD.SetData.KeyHitLocalScale.Value;
            _hit.HitHeadSizeDelta = HUD.SetData.KeyHitHeadSizeDelta.Value;

            _hit.HitDamageAnchoredPosition = HUD.SetData.KeyHitDamageAnchoredPosition.Value;
            _hit.HitDamageSizeDelta = HUD.SetData.KeyHitDamageSizeDelta.Value;
            _hit.HitDamageLocalScale = HUD.SetData.KeyHitDamageLocalScale.Value;

            _hit.ActiveSpeed = HUD.SetData.KeyHitActiveSpeed.Value;
            _hit.EndSpeed = HUD.SetData.KeyHitEndSpeed.Value;
            _hit.DeadSpeed = HUD.SetData.KeyHitDeadSpeed.Value;

            _hit.DamageColor = HUD.SetData.KeyHitDamageColor.Value;
            _hit.ArmorDamageColor = HUD.SetData.KeyHitArmorDamageColor.Value;
            _hit.DeadColor = HUD.SetData.KeyHitDeadColor.Value;
            _hit.HeadColor = HUD.SetData.KeyHitHeadColor.Value;
            _hit.DamageInfoColor = HUD.SetData.KeyHitDamageInfoColor.Value;
            _hit.ArmorDamageInfoColor = HUD.SetData.KeyHitArmorDamageInfoColor.Value;

            _hit.DamageStyles = HUD.SetData.KeyHitDamageStyles.Value;
            _hit.ArmorDamageStyles = HUD.SetData.KeyHitArmorDamageStyles.Value;
        }

        void ShowHit(GamePanelHUDHitPlugin.HitInfo hitInfo)
        {
            if (_Hit != null)
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
                    Camera cam = Camera.main;

                    if (cam != null)
                    {
                        Vector3 pos = cam.WorldToScreenPoint(hitInfo.HitPoint);

                        Vector2 screenPos = new Vector2(pos.x - (cam.pixelWidth * 0.5f), pos.y - (cam.pixelHeight * 0.5f));

                        _Hit.transform.localPosition = new Vector2((int)Math.Round(screenPos.x), (int)Math.Round(screenPos.y));
                    }

                    Hit(hitInfo, direction, HUD.SetData, _Hit);
                }
                else
                {
                    _TestHit.transform.localPosition = new Vector2(HUD.Info.sizeDelta.x / 3.2f, 0);

                    Hit(hitInfo, direction, HUD.SetData, _TestHit);
                }
            }
        }

        void Hit(GamePanelHUDHitPlugin.HitInfo hitInfo, GamePanelHUDHitPlugin.HitInfo.Direction direction, GamePanelHUDHitPlugin.SettingsData setData, GamePanelHUDHitUI _hit)
        {
            _hit.Damage = hitInfo.Damage;
            _hit.ArmorDamage = hitInfo.ArmorDamage;

            bool isHead = hitInfo.DamagePart == EBodyPart.Head;

            if (setData.KeyHitHasHead.Value && isHead && hitInfo.HitType != GamePanelHUDHitPlugin.HitInfo.Hit.Dead)
            {
                hitInfo.HitType = GamePanelHUDHitPlugin.HitInfo.Hit.Head;
            }

            _hit.HasArmorHit = hitInfo.HasArmorHit;

            _hit.IUpdate();

            if (hitInfo.HitType == GamePanelHUDHitPlugin.HitInfo.Hit.Dead)
            {
                _hit.HitDeadTrigger();
            }

            if (!setData.KeyHitHasDirection.Value)
            {
                direction = GamePanelHUDHitPlugin.HitInfo.Direction.Center;
            }

            _hit.HitTrigger(isHead, hitInfo, direction);
        }
#endif
    }
}
