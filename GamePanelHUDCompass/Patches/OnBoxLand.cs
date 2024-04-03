#if !UNITY_EDITOR

using EFT.Interactive;
using UnityEngine;

namespace GamePanelHUDCompass
{
    public partial class GamePanelHUDCompassPlugin
    {
        // ReSharper disable once SuggestBaseTypeForParameter
        private static void OnBoxLand(MonoBehaviour __instance, object ___boxSync, LootableContainer ___container)
        {
            GetNameDescriptionKey(___boxSync, out var nameKey, out var descriptionKey);

            ShowAirdrop(__instance.transform.position, nameKey, descriptionKey, ___container);
        }
    }
}

#endif