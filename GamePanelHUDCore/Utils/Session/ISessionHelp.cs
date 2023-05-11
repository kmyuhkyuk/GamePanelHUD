#if !UNITY_EDITOR
using HarmonyLib;

namespace GamePanelHUDCore.Utils.Session
{
    public static class ISessionHelp
    {
        private static ISession Session;

        private static object BackEndConfig;

        public static void Init(ISession session)
        {
            Session = session;

            BackEndConfig = Traverse.Create(Session).Property("BackEndConfig").GetValue<object>();

            ExperienceHelp.Init(BackEndConfig);
            TradersAvatar.Init(Session);
        }
    }
}
#endif
