using EFT.Quests;

namespace GamePanelHUDCompass
{
    public partial class GamePanelHUDCompassPlugin
    {
        private static void OnConditionValueChanged(EQuestStatus status,
            Condition condition)
        {
            if (status != EQuestStatus.Started)
            {
                DestroyStatic(condition.id);
            }
        }
    }
}