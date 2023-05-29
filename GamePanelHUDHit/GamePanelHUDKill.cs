using System;
using System.Collections.Generic;
using System.Linq;
using EFT;
using EFTUtils;
using GamePanelHUDCore.Utils;
using UnityEngine;
#if !UNITY_EDITOR
using GamePanelHUDCore;
using static EFTApi.EFTHelpers;
#endif

namespace GamePanelHUDHit
{
    public class GamePanelHUDKill : MonoBehaviour
#if !UNITY_EDITOR
        , IUpdate
#endif
    {
#if !UNITY_EDITOR
        private static GamePanelHUDCorePlugin.HUDCoreClass HUDCore => GamePanelHUDCorePlugin.HUDCore;
        private static GamePanelHUDHitPlugin.KillHUDClass<RectTransform, GamePanelHUDHitPlugin.SettingsData> HUD =>
            GamePanelHUDHitPlugin.KillHUD;
#endif

        private int _currentHasInfo;

        private int _waitInfo;

        private int _lastXp;

        private GamePanelHUDKillUI _previous;

        [SerializeField] private Transform killsRoot;

        private Transform _testKillsTransform;

        [SerializeField] private GamePanelHUDExpUI exp;

        private GamePanelHUDExpUI _testExp;

        private RectTransform _killsRect;

        private RectTransform _expRect;

        private RectTransform _testKillsRect;

        private RectTransform _testExpRect;

        private CanvasGroup _killGroup;

        private CanvasGroup _expGroup;

        private CanvasGroup _testExpGroup;

        internal static Action HasInfoMinus;

        internal static Action HasWaitInfoMinus;

#if !UNITY_EDITOR
        private void Start()
        {
            if (killsRoot != null)
            {
                _testKillsTransform = Instantiate(killsRoot.gameObject, killsRoot.transform.parent).transform;
                _testKillsRect = _testKillsTransform.GetComponent<RectTransform>();

                _killGroup = killsRoot.GetComponent<CanvasGroup>();

                if (exp != null)
                {
                    _testExp = Instantiate(exp.gameObject, exp.transform.parent).GetComponent<GamePanelHUDExpUI>();
                    _testExpRect = _testExp.GetComponent<RectTransform>();

                    _expRect = exp.GetComponent<RectTransform>();
                    _killsRect = killsRoot.GetComponent<RectTransform>();

                    _expGroup = exp.GetComponent<CanvasGroup>();
                    _testExpGroup = _testExp.GetComponent<CanvasGroup>();
                }
            }

            HasInfoMinus = InfoMinus;
            HasWaitInfoMinus = WaitInfoMinus;

            GamePanelHUDHitPlugin.ShowKill = ShowKill;

            HUDCore.UpdateManger.Register(this);
        }

        public void CustomUpdate()
        {
            KillHUD();
        }

        private void KillHUD()
        {
            if (killsRoot != null)
            {
                _killGroup.alpha = HUD.HUDSw ? 1 : 0;

                _killsRect.anchoredPosition = HUD.SetData.KeyKillAnchoredPosition.Value;
                _killsRect.sizeDelta = HUD.SetData.KeyKillSizeDelta.Value;
                _killsRect.localScale = HUD.SetData.KeyKillLocalScale.Value;

                _testKillsRect.anchoredPosition = HUD.SetData.KeyKillAnchoredPosition.Value +
                                                  new Vector2(HUD.Info.sizeDelta.x / 3.2f, 0);
                _testKillsRect.sizeDelta = HUD.SetData.KeyKillSizeDelta.Value;
                _testKillsRect.localScale = HUD.SetData.KeyKillLocalScale.Value;
            }

            if (exp != null)
            {
                _expGroup.alpha = HUD.HUDSw2 ? 1 : 0;

                _expRect.anchoredPosition = HUD.SetData.KeyKillExpAnchoredPosition.Value;
                _expRect.sizeDelta = HUD.SetData.KeyKillExpSizeDelta.Value;
                _expRect.localScale = HUD.SetData.KeyKillExpLocalScale.Value;

                exp.xpColor = HUD.SetData.KeyExpColor.Value;
                exp.xpStyles = HUD.SetData.KeyExpStyles.Value;
                exp.xpWaitSpeed = HUD.SetData.KeyExpWaitSpeed.Value;

                _testExpGroup.alpha = HUD.HUDSw3 ? 1 : 0;

                _testExpRect.anchoredPosition = HUD.SetData.KeyKillExpAnchoredPosition.Value +
                                                new Vector2(HUD.Info.sizeDelta.x / 3.2f, 0);
                _testExpRect.sizeDelta = HUD.SetData.KeyKillExpSizeDelta.Value;
                _testExpRect.localScale = HUD.SetData.KeyKillExpLocalScale.Value;

                _testExp.xpColor = HUD.SetData.KeyExpColor.Value;
                _testExp.xpStyles = HUD.SetData.KeyExpStyles.Value;
                _testExp.xpWaitSpeed = HUD.SetData.KeyExpWaitSpeed.Value;
            }
        }

        private void ShowKill(GamePanelHUDHitPlugin.KillInfo killInfo)
        {
            if (killsRoot != null)
            {
                var killRoot = killInfo.IsTest ? _testKillsTransform : killsRoot;

                var expRoot = killInfo.IsTest ? _testExp : exp;

                var baseExp = _SessionHelper.ExperienceHelper.GetBaseExp(killInfo.Exp, killInfo.Side);

                var hasExp = baseExp;

                var hasInfo = 1;

                var hasKills = new List<GamePanelHUDKillUI>();

                if (killInfo.Distance >= HUD.SetData.KeyKillDistance.Value && HUD.SetData.KeyKillHasDistance.Value)
                {
                    hasKills.Add(AddDistanceInfo(killInfo, HUD.SetData, killRoot));
                }

                if (killInfo.Kills > 1 && HUD.SetData.KeyKillHasStreak.Value)
                {
                    var streakXp =
                        _SessionHelper.ExperienceHelper.GetStreakExp(killInfo.Exp, killInfo.Side, killInfo.Kills);

                    hasKills.Add(AddStreakInfo(killInfo, HUD.SetData, killRoot, streakXp));

                    hasExp += streakXp;

                    hasInfo++;
                }

                if (killInfo.Part == EBodyPart.Head && HUD.SetData.KeyKillHasOther.Value)
                {
                    var headXp = _SessionHelper.ExperienceHelper.GetHeadExp(killInfo.Exp, killInfo.Side);

                    hasKills.Add(AddOtherInfo(HUD.SetData, killRoot, _LocalizedHelper.Localized("StatsHeadshot"),
                        headXp, true));

                    hasExp += headXp;

                    hasInfo++;
                }

                hasKills.Add(AddKillInfo(killInfo, HUD.SetData, killRoot, baseExp));

                var hasKillsCount = hasKills.Count;

                _currentHasInfo += hasKillsCount;
                _waitInfo += hasKillsCount;

                var count = hasKillsCount - 1;

                if (!HUD.SetData.KeyKillWaitBottom.Value)
                {
                    if (_previous != null)
                    {
                        _previous.after = hasKills.Last();

                        count -= 1;
                    }

                    hasKills.First().canDestroy = true;
                }
                else
                {
                    if (_previous != null)
                    {
                        _previous.after = hasKills.First();

                        hasKills.Last().after = _previous;
                    }
                    else
                    {
                        hasKills.First().canDestroy = true;
                    }
                }

                _previous = hasKills.Last();

                if (hasKillsCount > 1)
                {
                    for (var i = 0; i < count; i++)
                    {
                        hasKills[i].after = hasKills[i + 1];
                    }
                }

                var isAnim = expRoot.IsAnim;

                if (_currentHasInfo > 0 && !isAnim)
                {
                    expRoot.XpUp(hasExp, _lastXp);
                }
                else if (isAnim || hasInfo > 1)
                {
                    expRoot.XpUp(hasExp, 0);
                }
                else
                {
                    _lastXp = hasExp;
                }
            }
        }

        private static GamePanelHUDKillUI AddKillInfo(GamePanelHUDHitPlugin.KillInfo killInfo,
            GamePanelHUDHitPlugin.SettingsData setData, Transform killsRoot, int exp)
        {
            var killUI = Instantiate(GamePanelHUDHitPlugin.KillPrefab, killsRoot).GetComponent<GamePanelHUDKillUI>();

            var isScav = killInfo.Side == EPlayerSide.Savage;

            string playerName;

            if (isScav && setData.KeyKillScavEn.Value)
            {
                playerName = _LocalizedHelper.Transliterate(killInfo.PlayerName);
            }
            else
            {
                playerName = killInfo.PlayerName;
            }

            var weaponName = _LocalizedHelper.Localized(killInfo.WeaponName);

            string sideKey;
            if (isScav && _PlayerHelper.RoleHelper.IsBossOrFollower(killInfo.Role))
            {
                sideKey = _PlayerHelper.RoleHelper.GetScavRoleKey(killInfo.Role);
            }
            else
            {
                sideKey = killInfo.Side.ToString();
            }

            var sideName = _LocalizedHelper.Localized(sideKey);

            var nameColor = setData.KeyKillNameColor.Value.ColorToHtml();
            var partColor = setData.KeyKillPartColor.Value.ColorToHtml();
            var weaponColor = setData.KeyKillWeaponColor.Value.ColorToHtml();
            var lvlColor = setData.KeyKillLvlColor.Value.ColorToHtml();
            var levelColor = setData.KeyKillLevelColor.Value.ColorToHtml();
            var bracketColor = setData.KeyKillBracketColor.Value.ColorToHtml();
            var enemyDownColor = setData.KeyKillEnemyDownColor.Value.ColorToHtml();

            string sideColor;
            switch (isScav)
            {
                case true when _PlayerHelper.RoleHelper.IsBoss(killInfo.Role):
                    sideColor = setData.KeyKillBossColor.Value.ColorToHtml();
                    break;
                case true when _PlayerHelper.RoleHelper.IsFollower(killInfo.Role):
                    sideColor = setData.KeyKillFollowerColor.Value.ColorToHtml();
                    break;
                default:
                    switch (killInfo.Side)
                    {
                        case EPlayerSide.Usec:
                            sideColor = setData.KeyKillUsecColor.Value.ColorToHtml();
                            break;
                        case EPlayerSide.Bear:
                            sideColor = setData.KeyKillBearColor.Value.ColorToHtml();
                            break;
                        case EPlayerSide.Savage:
                            sideColor = setData.KeyKillScavColor.Value.ColorToHtml();
                            break;
                        default:
                            sideColor = Color.black.ColorToHtml();
                            break;
                    }

                    break;
            }

            killUI.xp = exp;

            var part = setData.KeyKillHasPart.Value
                ? string.Concat("<color=", bracketColor, ">", "[", "</color>", "<color=", partColor, ">",
                    _LocalizedHelper.Localized(killInfo.Part.ToString()), "</color>", "<color=", bracketColor, ">", "]",
                    "</color>")
                : string.Empty;
            var side = setData.KeyKillHasSide.Value
                ? string.Concat("<color=", bracketColor, ">", "[", "</color>", "<color=", sideColor, ">", sideName,
                    "</color>", "<color=", bracketColor, ">", "]", "</color>")
                : string.Empty;
            var weapon = setData.KeyKillHasWeapon.Value
                ? string.Concat("<color=", bracketColor, ">", "[", "</color>", "<color=", weaponColor, ">", weaponName,
                    "</color>", "<color=", bracketColor, ">", "]", "</color>")
                : string.Empty;
            var level = !isScav && setData.KeyKillHasLevel.Value
                ? string.Concat("<color=", bracketColor, ">", "[", "</color>", "<color=", lvlColor, ">",
                    _LocalizedHelper.Localized("LVLKILLLIST"), ".", "</color>", "<color=", levelColor, ">",
                    killInfo.Level, "</color>", "<color=", bracketColor, ">", "]", "</color>")
                : string.Empty;

            killUI.text = string.Concat("<color=", enemyDownColor, ">", _LocalizedHelper.Localized("ENEMYDOWN"),
                "</color>");
            killUI.text2 = string.Concat(weapon, side, " ", "<color=", nameColor, ">", playerName, "</color>", " ",
                level, part);

            killUI.textFontStyles = setData.KeyKillInfoStyles.Value;

            killUI.isKillInfo = true;

            killUI.hasXp = setData.KeyKillHasXp.Value;

            killUI.xpColor = setData.KeyKillXpColor.Value;

            killUI.transform.SetAsFirstSibling();

            killUI.active = true;

            return killUI;
        }

        private static GamePanelHUDKillUI AddDistanceInfo(GamePanelHUDHitPlugin.KillInfo killInfo,
            GamePanelHUDHitPlugin.SettingsData setData, Transform killsRoot)
        {
            var killUI = Instantiate(GamePanelHUDHitPlugin.KillPrefab, killsRoot).GetComponent<GamePanelHUDKillUI>();

            var meters = _LocalizedHelper.Localized("meters");

            var metersColor = setData.KeyKillMetersColor.Value.ColorToHtml();

            var distanceColor = setData.KeyKillDistanceColor.Value.ColorToHtml();

            var distanceText = killInfo.Distance.ToString("F2");

            killUI.text = string.Concat("<color=", distanceColor, ">", distanceText, "</color>", " ", "<color=",
                metersColor, ">", meters, "</color>");
            killUI.textFontStyles = setData.KeyKillDistanceStyles.Value;

            killUI.transform.SetAsFirstSibling();

            killUI.active = true;

            return killUI;
        }

        private static GamePanelHUDKillUI AddStreakInfo(GamePanelHUDHitPlugin.KillInfo killInfo,
            GamePanelHUDHitPlugin.SettingsData setData, Transform killsRoot, int exp)
        {
            var killUI = Instantiate(GamePanelHUDHitPlugin.KillPrefab, killsRoot).GetComponent<GamePanelHUDKillUI>();

            var statsStreakColor = setData.KeyKillStatsStreakColor.Value.ColorToHtml();
            var streakColor = setData.KeyKillStreakColor.Value.ColorToHtml();
            var bracketColor = setData.KeyKillBracketColor.Value.ColorToHtml();

            killUI.text = string.Concat("<color=", statsStreakColor, ">", _LocalizedHelper.Localized("StatsStreak"),
                "</color>", " ", "<color=", bracketColor, ">", "[", "</color>", "<color=", streakColor, ">",
                killInfo.Kills, "</color>", "<color=", bracketColor, ">", "]", "</color>");
            killUI.textFontStyles = setData.KeyKillStreakStyles.Value;

            killUI.xp = exp;

            killUI.hasXp = setData.KeyKillHasXp.Value;

            killUI.xpColor = setData.KeyKillXpColor.Value;

            killUI.transform.SetAsFirstSibling();

            killUI.active = true;

            return killUI;
        }

        private static GamePanelHUDKillUI AddOtherInfo(GamePanelHUDHitPlugin.SettingsData setData, Transform killsRoot,
            string text, int exp, bool hasXp)
        {
            var killUI = Instantiate(GamePanelHUDHitPlugin.KillPrefab, killsRoot).GetComponent<GamePanelHUDKillUI>();

            var otherColor = setData.KeyKillOtherColor.Value.ColorToHtml();

            killUI.text = string.Concat("<color=", otherColor, ">", text, "</color>");
            killUI.textFontStyles = setData.KeyKillOtherStyles.Value;

            killUI.xp = exp;

            killUI.hasXp = hasXp && setData.KeyKillHasXp.Value;

            killUI.xpColor = setData.KeyKillXpColor.Value;

            killUI.transform.SetAsFirstSibling();

            killUI.active = true;

            return killUI;
        }

        private void InfoMinus()
        {
            _currentHasInfo -= 1;
        }

        private void WaitInfoMinus()
        {
            _waitInfo -= 1;

            if (_waitInfo == 0)
            {
                exp.XpComplete();
                _testExp.XpComplete();
            }
        }
#endif
    }
}