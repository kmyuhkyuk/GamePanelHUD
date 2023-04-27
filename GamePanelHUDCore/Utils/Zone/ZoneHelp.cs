#if !UNITY_EDITOR
using System.Linq;
using System.Collections.Generic;
using EFT.Interactive;

namespace GamePanelHUDCore.Utils.Zone
{
    public static class ZoneHelp
    {
        internal static readonly List<TriggerWithId> TriggerPoints = new List<TriggerWithId>();

        public static IEnumerable<ExperienceTrigger> ExperienceTriggers
        {
            get
            {
                return TriggerPoints.Where(x => x is ExperienceTrigger).Cast<ExperienceTrigger>();
            }
        }

        public static IEnumerable<PlaceItemTrigger> PlaceItemTriggers
        {
            get
            {
                return TriggerPoints.Where(x => x is PlaceItemTrigger).Cast<PlaceItemTrigger>();
            }
        }

        public static IEnumerable<QuestTrigger> QuestTriggers
        {
            get
            {
                return TriggerPoints.Where(x => x is QuestTrigger).Cast<QuestTrigger>();
            }
        }

        static ZoneHelp()
        {
            GamePanelHUDCorePlugin.HUDCoreClass.WorldDispose += (GameWorld) => TriggerPoints.Clear();
        }

        public static bool TryGetValues<T>(string id, out IEnumerable<T> triggers) where T : TriggerWithId
        {
            if (typeof(T) == typeof(ExperienceTrigger))
            {
                triggers = ExperienceTriggers.Where(x => x.Id == id) as IEnumerable<T>;
            }
            else if (typeof(T) == typeof(PlaceItemTrigger))
            {
                triggers = PlaceItemTriggers.Where(x => x.Id == id) as IEnumerable<T>;
            }
            else if (typeof(T) == typeof(QuestTrigger))
            {
                triggers = QuestTriggers.Where(x => x.Id == id) as IEnumerable<T>;
            }
            else
            {
                triggers = null;
                return false;
            }

            return triggers.Any();
        }
    }
}
#endif
