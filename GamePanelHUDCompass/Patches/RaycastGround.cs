#if !UNITY_EDITOR

using EFT.Interactive;
using EFTApi;
using UnityEngine;
using static EFTApi.EFTHelpers;

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
                switch (_AirdropSynchronizableObjectHelper.RefAirdropType?.GetValue(___airdropSynchronizableObject_0))
                {
                    case 0:
                        nameKey = "6223349b3136504a544d1608 Name";
                        descriptionKey = "6223349b3136504a544d1608 Description";
                        break;
                    case 1:
                        nameKey = "622334fa3136504a544d160c Name";
                        descriptionKey = "622334fa3136504a544d160c Description";
                        break;
                    case 2:
                        nameKey = "622334c873090231d904a9fc Name";
                        descriptionKey = "622334c873090231d904a9fc Description";
                        break;
                    case 3:
                        nameKey = "6223351bb5d97a7b2c635ca7 Name";
                        descriptionKey = "6223351bb5d97a7b2c635ca7 Description";
                        break;
                    default:
                        nameKey = "Unknown";
                        descriptionKey = "Unknown";
                        break;
                }
            }
            else
            {
                nameKey = "61a89e5445a2672acf66c877 Name";
                descriptionKey = "61a89e5445a2672acf66c877 Description";
            }

            ShowAirdrop(___airdropSynchronizableObject_0.transform.position, nameKey, descriptionKey,
                ___airdropSynchronizableObject_0.transform.GetComponentInChildren<LootableContainer>());
        }
    }
}

#endif