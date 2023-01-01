#if !UNITY_EDITOR
using HarmonyLib;

namespace GamePanelHUDCore.Utils
{
    public class ISessionHelp
    {
        private static object Session;

        private static object BackEndConfig;

        //Only Init Once
        public static void Init(object session)
        {
            Session = session;

            BackEndConfig = Traverse.Create(Session).Property("BackEndConfig").GetValue<object>();

            ExperienceHelp.Init(BackEndConfig);
        }
    }
}
#endif
