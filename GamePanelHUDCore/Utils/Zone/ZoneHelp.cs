#if !UNITY_EDITOR
using System.Collections.Generic;
using UnityEngine;
using EFT;
using EFT.Interactive;
using System.Linq;

namespace GamePanelHUDCore.Utils.Zone
{
    public static class ZoneHelp
    {
        internal static readonly List<TriggerWithId> TriggerPoints = new List<TriggerWithId>();

        public static IEnumerable<ExperienceTrigger> ExperienceTriggers
        {
            get
            {
                return TriggerPoints.Where(x => x is ExperienceTrigger).Select(x => (ExperienceTrigger)x);
            }
        }

        public static IEnumerable<PlaceItemTrigger> PlaceItemTriggers
        {
            get
            {
                return TriggerPoints.Where(x => x is PlaceItemTrigger).Select(x => (PlaceItemTrigger)x);
            }
        }

        public static IEnumerable<QuestTrigger> QuestTriggers
        {
            get
            {
                return TriggerPoints.Where(x => x is QuestTrigger).Select(x => (QuestTrigger)x);
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

            return triggers.Count() > 0 ? true : false;
        }
    }
}
#endif
