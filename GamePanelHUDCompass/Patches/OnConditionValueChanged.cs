#if !UNITY_EDITOR

using EFT.Quests;
using GamePanelHUDCompass.Models;
using static EFTApi.EFTHelpers;

namespace GamePanelHUDCompass
{
    public partial class GamePanelHUDCompassPlugin
    {
        private static void OnConditionValueChanged(EQuestStatus status,
            Condition condition)
        {
            if (status != EQuestStatus.Started)
            {
                CompassStaticHUDModel.Instance.DestroyStatic(_ConditionMongoIDHelper.RefId.GetValue(condition)
                    .ToString());
            }
        }
    }
}

#endif