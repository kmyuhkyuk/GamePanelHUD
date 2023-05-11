using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using EFT;
using EFT.UI;
#if !UNITY_EDITOR
using GamePanelHUDCore;
using GamePanelHUDCore.Utils.Session;
#endif
using GamePanelHUDCore.Utils;

namespace GamePanelHUDHit
{
    public class GamePanelHUDKill : UIElement
#if !UNITY_EDITOR
        , IUpdate
#endif
    {
#if !UNITY_EDITOR
        private GamePanelHUDHitPlugin.KillHUDClass<RectTransform, GamePanelHUDHitPlugin.SettingsData> HUD => GamePanelHUDHitPlugin.KillHUD;
#endif

        private int NowHasInfo;

        private int WaitInfo;

        private int LastXp;

        private GamePanelHUDKillUI Previous;

        [SerializeField]
        private Transform _Kills;

        private Transform _TestKills;

        [SerializeField]
        private GamePanelHUDExpUI _Exp;

        private GamePanelHUDExpUI _TestExp;

        private RectTransform KillsRect;

        private RectTransform ExpRect;

        private RectTransform TestKillsRect;

        private RectTransform TestExpRect;

        private CanvasGroup KillGroup;

        private CanvasGroup ExpGroup;

        private CanvasGroup TestExpGroup;

        internal static Action HasInfoMinus;

        internal static Action HasWaitInfoMinus;

#if !UNITY_EDITOR
        void Start()
        {
            if (_Kills != null)
            {
                _TestKills = Instantiate(_Kills.gameObject, _Kills.transform.parent).transform;
                TestKillsRect = _TestKills.GetComponent<RectTransform>();

                KillGroup = _Kills.GetComponent<CanvasGroup>();

                if (_Exp != null)
                {
                    _TestExp = Instantiate(_Exp.gameObject, _Exp.transform.parent).GetComponent<GamePanelHUDExpUI>();
                    TestExpRect = _TestExp.GetComponent<RectTransform>();

                    ExpRect = _Exp.GetComponent<RectTransform>();
                    KillsRect = _Kills.GetComponent<RectTransform>();

                    ExpGroup = _Exp.GetComponent<CanvasGroup>();
                    TestExpGroup = _TestExp.GetComponent<CanvasGroup>();
                }
            }

            HasInfoMinus = InfoMinus;
            HasWaitInfoMinus = WaitInfoMinus;

            GamePanelHUDHitPlugin.ShowKill = ShowKill;

            GamePanelHUDCorePlugin.UpdateManger.Register(this);
        }

        public void IUpdate()
        {
            KillHUD();
        }

        void KillHUD()
        {
            if (_Kills != null)
            {
                KillGroup.alpha = HUD.HUDSw ? 1 : 0;

                KillsRect.anchoredPosition = HUD.SetData.KeyKillAnchoredPosition.Value;
                KillsRect.sizeDelta = HUD.SetData.KeyKillSizeDelta.Value;
                KillsRect.localScale = HUD.SetData.KeyKillLocalScale.Value;

                TestKillsRect.anchoredPosition = HUD.SetData.KeyKillAnchoredPosition.Value + new Vector2(HUD.Info.sizeDelta.x / 3.2f, 0);
                TestKillsRect.sizeDelta = HUD.SetData.KeyKillSizeDelta.Value;
                TestKillsRect.localScale = HUD.SetData.KeyKillLocalScale.Value;
            }
            if (_Exp != null)
            {
                ExpGroup.alpha = HUD.HUDSw2 ? 1 : 0;

                ExpRect.anchoredPosition = HUD.SetData.KeyKillExpAnchoredPosition.Value;
                ExpRect.sizeDelta = HUD.SetData.KeyKillExpSizeDelta.Value;
                ExpRect.localScale = HUD.SetData.KeyKillExpLocalScale.Value;

                _Exp.XPColor = HUD.SetData.KeyExpColor.Value;
                _Exp.XPStyles = HUD.SetData.KeyExpStyles.Value;
                _Exp.XPWaitSpeed = HUD.SetData.KeyExpWaitSpeed.Value;

                TestExpGroup.alpha = HUD.HUDSw3 ? 1 : 0;

                TestExpRect.anchoredPosition = HUD.SetData.KeyKillExpAnchoredPosition.Value + new Vector2(HUD.Info.sizeDelta.x / 3.2f, 0);
                TestExpRect.sizeDelta = HUD.SetData.KeyKillExpSizeDelta.Value;
                TestExpRect.localScale = HUD.SetData.KeyKillExpLocalScale.Value;

                _TestExp.XPColor = HUD.SetData.KeyExpColor.Value;
                _TestExp.XPStyles = HUD.SetData.KeyExpStyles.Value;
                _TestExp.XPWaitSpeed = HUD.SetData.KeyExpWaitSpeed.Value;
            }
        }

        void ShowKill(GamePanelHUDHitPlugin.KillInfo killInfo)
        {
            if (_Kills != null)
            {
                Transform kills = killInfo.IsTest ? _TestKills : _Kills;

                GamePanelHUDExpUI exp = killInfo.IsTest ? _TestExp : _Exp;

                int baseExp = ExperienceHelp.GetBaseExp(killInfo.Exp, killInfo.Side);

                int hasExp = baseExp;

                int hasInfo = 1;

                List<GamePanelHUDKillUI> hasKills = new List<GamePanelHUDKillUI>();

                if (killInfo.Distance >= HUD.SetData.KeyKillDistance.Value && HUD.SetData.KeyKillHasDistance.Value)
                {
                    hasKills.Add(AddDistanceInfo(killInfo, HUD.SetData, kills));
                }

                if (killInfo.Kills > 1 && HUD.SetData.KeyKillHasStreak.Value)
                {
                    int streakXp = ExperienceHelp.GetStreakExp(killInfo.Exp, killInfo.Side, killInfo.Kills);

                    hasKills.Add(AddStreakInfo(killInfo, HUD.SetData, kills, streakXp));

                    hasExp += streakXp;

                    hasInfo++;
                }

                if (killInfo.Part == EBodyPart.Head && HUD.SetData.KeyKillHasOther.Value)
                {
                    int headXp = ExperienceHelp.GetHeadExp(killInfo.Exp, killInfo.Side);

                    hasKills.Add(AddOtherInfo(HUD.SetData, kills, LocalizedHelp.Localized("StatsHeadshot", EStringCase.None), headXp, true));

                    hasExp += headXp;

                    hasInfo++;
                }

                hasKills.Add(AddKillInfo(killInfo, HUD.SetData, kills, baseExp));

                int hasKillsCount = hasKills.Count;

                NowHasInfo += hasKillsCount;
                WaitInfo += hasKillsCount;

                int count = hasKillsCount - 1;

                if (!HUD.SetData.KeyKillWaitBottom.Value)
                {
                    if (Previous != null)
                    {
                        Previous.After = hasKills.Last();

                        count -= 1;
                    }

                    hasKills.First().CanDestroy = true;
                }
                else
                {
                    if (Previous != null)
                    {
                        Previous.After = hasKills.First();

                        hasKills.Last().After = Previous;
                    }
                    else
                    {
                        hasKills.First().CanDestroy = true;
                    }
                }

                Previous = hasKills.Last();

                if (hasKillsCount > 1)
                {
                    for (int i = 0; i < count; i++)
                    {
                        hasKills[i].After = hasKills[i + 1];
                    }
                }

                if (NowHasInfo > 0 && !exp.IsAnim())
                {
                    exp.XpUp(hasExp, LastXp);
                }
                else if (exp.IsAnim() || hasInfo > 1)
                {
                    exp.XpUp(hasExp, 0);
                }
                else
                {
                    LastXp = hasExp;
                }
            }
        }

        GamePanelHUDKillUI AddKillInfo(GamePanelHUDHitPlugin.KillInfo killInfo, GamePanelHUDHitPlugin.SettingsData setData, Transform _kills, int exp)
        {
            GameObject kill = Instantiate(GamePanelHUDHitPlugin.KillPrefab, _kills);

            GamePanelHUDKillUI _kill = kill.GetComponent<GamePanelHUDKillUI>();

            bool isScav = killInfo.Side == EPlayerSide.Savage;

            string playerName;

            if (isScav && setData.KeyKillScavEn.Value)
            {
                playerName = RuToEn.Transliterate(killInfo.PlayerName);
            }
            else
            {
                playerName = killInfo.PlayerName;
            }

            var weaponName = LocalizedHelp.Localized(killInfo.WeaponName, EStringCase.None);

            string sideKey;
            if (isScav && RoleHelp.IsBossOrFollower(killInfo.Role))
            {
                sideKey = RoleHelp.GetScavRoleKey(killInfo.Role);
            }
            else
            {
                sideKey = killInfo.Side.ToString();
            }

            var sideName = LocalizedHelp.Localized(sideKey, EStringCase.None);

            string nameColor = setData.KeyKillNameColor.Value.ColorToHtml();
            string partColor = setData.KeyKillPartColor.Value.ColorToHtml();
            string weaponColor = setData.KeyKillWeaponColor.Value.ColorToHtml();
            string lvlColor = setData.KeyKillLvlColor.Value.ColorToHtml();
            string levelColor = setData.KeyKillLevelColor.Value.ColorToHtml();
            string bracketColor = setData.KeyKillBracketColor.Value.ColorToHtml();
            string enemyDownColor = setData.KeyKillEnemyDownColor.Value.ColorToHtml();

            string sideColor;
            if (isScav && RoleHelp.IsBoss(killInfo.Role))
            {
                sideColor = setData.KeyKillBossColor.Value.ColorToHtml();
            }
            else if (isScav && RoleHelp.IsFollower(killInfo.Role))
            {
                sideColor = setData.KeyKillFollowerColor.Value.ColorToHtml();
            }
            else
            {
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
            }

            _kill.Xp = exp;

            string part = setData.KeyKillHasPart.Value ? string.Concat("<color=", bracketColor, ">", "[", "</color>", "<color=", partColor, ">", LocalizedHelp.Localized(killInfo.Part.ToString(), EStringCase.None), "</color>", "<color=", bracketColor, ">", "]", "</color>") : "";
            string side = setData.KeyKillHasSide.Value ? string.Concat("<color=", bracketColor, ">", "[", "</color>", "<color=", sideColor, ">", sideName, "</color>", "<color=", bracketColor, ">", "]", "</color>") : "";
            string weapon = setData.KeyKillHasWeapon.Value ? string.Concat("<color=", bracketColor, ">", "[", "</color>", "<color=", weaponColor, ">", weaponName, "</color>", "<color=", bracketColor, ">", "]", "</color>") : "";
            string level = !isScav && setData.KeyKillHasLevel.Value ? string.Concat("<color=", bracketColor, ">", "[", "</color>", "<color=", lvlColor, ">", LocalizedHelp.Localized("LVLKILLLIST", EStringCase.None), ".", "</color>", "<color=", levelColor, ">", killInfo.Level, "</color>", "<color=", bracketColor, ">", "]", "</color>") : "";

            _kill.Text = string.Concat("<color=", enemyDownColor, ">", LocalizedHelp.Localized("ENEMYDOWN", EStringCase.None), "</color>");
            _kill.Text2 = string.Concat(weapon, side, " ", "<color=", nameColor, ">", playerName, "</color>", " ", level, part);

            _kill.TextFontStyles = setData.KeyKillInfoStyles.Value;

            _kill.IsKillInfo = true;

            _kill.HasXp = setData.KeyKillHasXp.Value;

            _kill.XpColor = setData.KeyKillXpColor.Value;

            _kill.transform.SetAsFirstSibling();

            _kill.Active = true;

            return _kill;
        }

        GamePanelHUDKillUI AddDistanceInfo(GamePanelHUDHitPlugin.KillInfo killInfo, GamePanelHUDHitPlugin.SettingsData setData, Transform _kills)
        {
            GameObject kill = Instantiate(GamePanelHUDHitPlugin.KillPrefab, _kills);

            GamePanelHUDKillUI _kill = kill.GetComponent<GamePanelHUDKillUI>();

            string meters = LocalizedHelp.Localized("meters", EStringCase.None);

            string metersColor = setData.KeyKillMetersColor.Value.ColorToHtml();

            string distanceColor = setData.KeyKillDistanceColor.Value.ColorToHtml();

            string distanceText = killInfo.Distance.ToString("F2");

            _kill.Text = string.Concat("<color=", distanceColor, ">", distanceText, "</color>", " ", "<color=", metersColor, ">", meters, "</color>");
            _kill.TextFontStyles = setData.KeyKillDistanceStyles.Value;

            _kill.transform.SetAsFirstSibling();

            _kill.Active = true;

            return _kill;
        }

        GamePanelHUDKillUI AddStreakInfo(GamePanelHUDHitPlugin.KillInfo killInfo, GamePanelHUDHitPlugin.SettingsData setData, Transform _kills, int exp)
        {
            GameObject kill = Instantiate(GamePanelHUDHitPlugin.KillPrefab, _kills);

            GamePanelHUDKillUI _kill = kill.GetComponent<GamePanelHUDKillUI>();

            string statsStreakColor = setData.KeyKillStatsStreakColor.Value.ColorToHtml();
            string streakColor = setData.KeyKillStreakColor.Value.ColorToHtml();
            string bracketColor = setData.KeyKillBracketColor.Value.ColorToHtml();

            _kill.Text = string.Concat("<color=", statsStreakColor, ">", LocalizedHelp.Localized("StatsStreak", EStringCase.None), "</color>", " ", "<color=", bracketColor, ">", "[", "</color>", "<color=", streakColor, ">", killInfo.Kills, "</color>", "<color=", bracketColor, ">", "]", "</color>");
            _kill.TextFontStyles = setData.KeyKillStreakStyles.Value;

            _kill.Xp = exp;

            _kill.HasXp = setData.KeyKillHasXp.Value;

            _kill.XpColor = setData.KeyKillXpColor.Value;

            _kill.transform.SetAsFirstSibling();

            _kill.Active = true;

            return _kill;
        }

        GamePanelHUDKillUI AddOtherInfo(GamePanelHUDHitPlugin.SettingsData setData, Transform _kills, string text, int exp, bool hasXp)
        {
            GameObject kill = Instantiate(GamePanelHUDHitPlugin.KillPrefab, _kills);

            GamePanelHUDKillUI _kill = kill.GetComponent<GamePanelHUDKillUI>();

            string otherColor = setData.KeyKillOtherColor.Value.ColorToHtml();

            _kill.Text = string.Concat("<color=", otherColor, ">", text, "</color>");
            _kill.TextFontStyles = setData.KeyKillOtherStyles.Value;

            _kill.Xp = exp;

            _kill.HasXp = hasXp && setData.KeyKillHasXp.Value;

            _kill.XpColor = setData.KeyKillXpColor.Value;

            _kill.transform.SetAsFirstSibling();

            _kill.Active = true;

            return _kill;
        }

        private void InfoMinus()
        {
            NowHasInfo -= 1;
        }

        void WaitInfoMinus()
        {
            WaitInfo -= 1;

            if (WaitInfo == 0)
            {
                _Exp.XpComplete();
                _TestExp.XpComplete();
            }
        }
#endif
    }
}
