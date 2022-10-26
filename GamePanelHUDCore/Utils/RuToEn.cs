using HarmonyLib;
using System;
using System.Linq;
using System.Reflection;

namespace GamePanelHUDCore.Utils
{
    public class RuToEn
    {
        private static Func<string, string> RefTransliterate;

        public static void Init()
        {
            BindingFlags flags = BindingFlags.Public | BindingFlags.Static | BindingFlags.Instance;

            RefTransliterate = AccessTools.MethodDelegate<Func<string, string>>(RefHelp.GetEftMethod(x => x.GetMethods(flags).Any(t => t.Name == "Transliterate"), flags, 
                x => x.Name == "Transliterate"
                && x.GetParameters().Length == 1));
        }

        public static string Transliterate(string text)
        {
            return RefTransliterate(text);
        }
    }
}
