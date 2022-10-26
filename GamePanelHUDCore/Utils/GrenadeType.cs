using System;
using System.Reflection;

namespace GamePanelHUDCore.Utils
{
    public class GrenadeType
    {
        public static Type GrenadeItemType { get; private set; }

        public static void Init()
        {
            BindingFlags flags = BindingFlags.Public | BindingFlags.Instance;

            GrenadeItemType = RefHelp.GetEftType(x => x.GetMethod("CreateFragment", flags) != null && x.GetProperty("GetExplDelay", flags) != null);
        }
    }
}
