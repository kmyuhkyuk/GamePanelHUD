#if !UNITY_EDITOR

using EFT.Interactive;
using EFT.SynchronizableObjects;

namespace GamePanelHUDCompass
{
    public partial class GamePanelHUDCompassPlugin
    {
        // ReSharper disable once SuggestBaseTypeForParameter
        // ReSharper disable once InconsistentNaming
        private static void RaycastGround(AirdropSynchronizableObject ___airdropSynchronizableObject_0)
        {
            GetNameDescriptionKey(___airdropSynchronizableObject_0, out var nameKey, out var descriptionKey);

            ShowAirdrop(___airdropSynchronizableObject_0.transform.position, nameKey, descriptionKey,
                ___airdropSynchronizableObject_0.GetComponentInChildren<LootableContainer>());
        }
    }
}

#endif