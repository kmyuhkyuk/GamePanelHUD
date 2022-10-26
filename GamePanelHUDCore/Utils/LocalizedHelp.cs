using HarmonyLib;
using System;
using System.Reflection;
using EFT;

namespace GamePanelHUDCore.Utils
{
    public class LocalizedHelp
    {
        private static Func<string, EStringCase, string> RefLocalized;

        public static void Init()
        {
            BindingFlags flags = BindingFlags.Public | BindingFlags.Static | BindingFlags.Instance;

            RefLocalized = AccessTools.MethodDelegate<Func<string, EStringCase, string>>(RefHelp.GetEftMethod(x => x.GetMethod("ParseLocalization", flags) != null, flags,
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
