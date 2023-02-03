#if !UNITY_EDITOR
using HarmonyLib;
using System;
using System.Reflection;
using EFT;

namespace GamePanelHUDCore.Utils
{
    public class LocalizedHelp
    {
        private static Func<string, EStringCase, string> RefLocalized;

        private static Func<string, string, string> RefLocalizedPrefix;

        public static void Init()
        {
            BindingFlags flags = BindingFlags.Public | BindingFlags.Static | BindingFlags.Instance;

            Type type = RefHelp.GetEftType(x => x.GetMethod("ParseLocalization", flags) != null);

            RefLocalized = AccessTools.MethodDelegate<Func<string, EStringCase, string>>(RefHelp.GetEftMethod(type, flags,
                x => x.Name == "Localized"
                && x.GetParameters().Length == 2
                && x.GetParameters()[0].ParameterType == typeof(string)
                && x.GetParameters()[1].ParameterType == typeof(EStringCase)));

            RefLocalizedPrefix = AccessTools.MethodDelegate<Func<string, string, string>>(RefHelp.GetEftMethod(type, flags,
                x => x.Name == "Localized"
                && x.GetParameters().Length == 2
                && x.GetParameters()[0].ParameterType == typeof(string)
                && x.GetParameters()[1].ParameterType == typeof(string)));
        }

        public static string Localized(string id, EStringCase @case)
        {
            return RefLocalized(id, @case);
        }

        public static string Localized(string id, string prefix = null)
        {
            return RefLocalizedPrefix(id, prefix);
        }
    }
}
#endif
