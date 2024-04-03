#if !UNITY_EDITOR

using EFT.Interactive;
using UnityEngine;

namespace GamePanelHUDCompass
{
    public partial class GamePanelHUDCompassPlugin
    {
        // ReSharper disable once SuggestBaseTypeForParameter
        // ReSharper disable once InconsistentNaming
        private static void CoopOnBoxLand(MonoBehaviour __instance, object ___boxSync, LootableContainer ___Container)
        {
            BaseOnBoxLand(__instance.transform.position, ___boxSync, ___Container);
        }
    }
}

#endif