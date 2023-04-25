﻿#if !UNITY_EDITOR
using HarmonyLib;
using System;
using System.Reflection;
using EFT;

namespace GamePanelHUDCore.Utils
{
    public static class LocalizedHelp
    {
        private static Func<string, EStringCase, string> RefLocalized;

        public static void Init()
        {
            BindingFlags flags = BindingFlags.Public | BindingFlags.Static | BindingFlags.Instance;

            Type type = RefHelp.GetEftType(x => x.GetMethod("ParseLocalization", flags) != null);

            RefLocalized = AccessTools.MethodDelegate<Func<string, EStringCase, string>>(RefHelp.GetEftMethod(type, flags,
                x => x.Name == "Localized"
                && x.GetParameters().Length == 2
                && x.GetParameters()[0].ParameterType == typeof(string)
                && x.GetParameters()[1].ParameterType == typeof(EStringCase)));
        }

        public static string Localized(string id, EStringCase @case)
        {
            return RefLocalized(id, @case);
        }
    }
}
#endif
