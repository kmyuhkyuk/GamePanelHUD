#if !UNITY_EDITOR

using UnityEngine;
using static EFTApi.EFTHelpers;

namespace GamePanelHUDCompass
{
    public partial class GamePanelHUDCompassPlugin
    {
        // ReSharper disable once SuggestBaseTypeForParameter
        // ReSharper disable once InconsistentNaming
        private static void CoopOnBoxLand(MonoBehaviour __instance, object ___boxSync)
        {
            GetNameDescriptionKey(___boxSync, out var nameKey, out var descriptionKey);

            ShowAirdrop(__instance.transform.position, nameKey, descriptionKey,
                _AirdropBoxHelper.RefCoopContainer?.GetValue(__instance));
        }
    }
}

#endif