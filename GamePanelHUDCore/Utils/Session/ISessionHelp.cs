using HarmonyLib;

namespace GamePanelHUDCore.Utils
{
    public class ISessionHelp
    {
        private static ISession Session;

        private static object BackEndConfig;

        //Only Init Once
        public static void Init(ISession session)
        {
            Session = session;

            BackEndConfig = Traverse.Create(Session).Property("BackEndConfig").GetValue<object>();

            ExperienceHelp.Init(BackEndConfig);
        }
    }
}
