#if !UNITY_EDITOR

using EFT.Interactive;
using EFTApi;
using UnityEngine;

namespace GamePanelHUDCompass
{
    public partial class GamePanelHUDCompassPlugin
    {
        // ReSharper disable once SuggestBaseTypeForParameter
        // ReSharper disable once InconsistentNaming
        private static void RaycastGround(MonoBehaviour ___airdropSynchronizableObject_0)
        {
            string nameKey;
            string descriptionKey;
            if (EFTVersion.AkiVersion > EFTVersion.Parse("3.0.0"))
            {
                GetNameDescriptionKey(___airdropSynchronizableObject_0, out nameKey, out descriptionKey);
            }
            else
            {
                nameKey = "61a89e5445a2672acf66c877 Name";
                descriptionKey = "61a89e5445a2672acf66c877 Description";
            }

            ShowAirdrop(___airdropSynchronizableObject_0.transform.position, nameKey, descriptionKey,
                ___airdropSynchronizableObject_0.GetComponentInChildren<LootableContainer>());
        }
    }
}

#endif