#if !UNITY_EDITOR

using EFT.Interactive;
using EFT.SynchronizableObjects;

namespace GamePanelHUDCompass
{
    public partial class GamePanelHUDCompassPlugin
    {
        // ReSharper disable once SuggestBaseTypeForParameter
        // ReSharper disable once InconsistentNaming
        private static void RaycastGround(AirdropSynchronizableObject ____syncObject)
        {
            GetNameDescriptionKey(____syncObject, out var nameKey, out var descriptionKey);

            ShowAirdrop(____syncObject.transform.position, nameKey, descriptionKey,
                ____syncObject.GetComponentInChildren<LootableContainer>());
        }
    }
}

#endif