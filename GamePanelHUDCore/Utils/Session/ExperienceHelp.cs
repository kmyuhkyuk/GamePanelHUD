﻿#if !UNITY_EDITOR
using HarmonyLib;
using System;
using System.Reflection;
using EFT;

namespace GamePanelHUDCore.Utils
{
    public class ExperienceHelp
    {
        private static object Config;

        private static object Experience;

        private static object Kill;

        private static Func<object, int, int> RefKillingBonusPercent;

        private static int VictimLevelExp;

        private static int VictimBotLevelExp;

        private static float HeadShotMult;

        public static bool CanWork { get; private set; }

        static ExperienceHelp()
        {
            BindingFlags flags = BindingFlags.Public | BindingFlags.Instance;

            RefKillingBonusPercent = RefHelp.ObjectMethodDelegate<Func<object, int, int>>(RefHelp.GetEftMethod(x => x.GetMethod("GetKillingBonusPercent") != null, flags, x => x.Name == "GetKillingBonusPercent"));
        }

        public static void Init(object backendconfig)
        {
            Config = Traverse.Create(backendconfig).Field("Config").GetValue<object>();

            Experience = Traverse.Create(Config).Field("Experience").GetValue<object>();

            Kill = Traverse.Create(Experience).Field("Kill").GetValue<object>();

            VictimLevelExp = Traverse.Create(Kill).Field("VictimLevelExp").GetValue<int>();

            VictimBotLevelExp = Traverse.Create(Kill).Field("VictimBotLevelExp").GetValue<int>();

            HeadShotMult = Traverse.Create(Kill).Field("HeadShotMult").GetValue<float>();

            CanWork = true;
        }

        public static int GetBaseExp(int exp, EPlayerSide side)
        {
            switch (side)
            {
                case EPlayerSide.Usec:
                    return VictimLevelExp;
                case EPlayerSide.Bear:
                    return VictimLevelExp;
                case EPlayerSide.Savage:
                    if (exp < 0)
                        return VictimBotLevelExp;
                    else
                        return exp;
            }

            return 0;
        }

        public static int GetHeadExp(int exp, EPlayerSide side)
        {
            return (int)((float)GetBaseExp(exp, side) * HeadShotMult);
        }

        public static int GetStreakExp(int exp, EPlayerSide side, int kills)
        {
            return (int)((float)GetBaseExp(exp, side) * ((float)GetKillingBonusPercent(kills) / 100f));
        }

        public static int GetKillingBonusPercent(int killed)
        {
            return RefKillingBonusPercent(Kill, killed);
        }
    }
}
#endif
