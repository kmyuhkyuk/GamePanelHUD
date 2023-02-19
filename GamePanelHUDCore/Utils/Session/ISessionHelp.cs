#if !UNITY_EDITOR
using HarmonyLib;
using GamePanelHUDCore.Utils.Session;

namespace GamePanelHUDCore.Utils
{
    public class ISessionHelp
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
