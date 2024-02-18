using System;
using System.Collections;
using System.Collections.Generic;
using EFT.Interactive;
using EFT.Quests;
using EFTApi;
using EFTReflection;
using JetBrains.Annotations;
using static EFTApi.EFTHelpers;

namespace GamePanelHUDCompass.Models
{
    internal class ReflectionModel
    {
        private static readonly Lazy<ReflectionModel> Lazy = new Lazy<ReflectionModel>(() => new ReflectionModel());

        public static ReflectionModel Instance => Lazy.Value;

        public readonly RefHelper.IRef<object, object> RefQuests;

        public readonly RefHelper.IRef<object, object> RefConditions;

        public readonly RefHelper.FieldRef<object, IList> RefQuestsList;

        public readonly RefHelper.FieldRef<object, List<LootItem>> RefLootList;

        public readonly RefHelper.FieldRef<object, string> RefLocationId;

        [CanBeNull] public readonly RefHelper.FieldRef<object, int> RefPlayerGroup;

        public readonly RefHelper.FieldRef<object, string> RefTraderId;

        public readonly RefHelper.FieldRef<object, IList> RefAvailableForFinishConditionsList;

        public readonly RefHelper.FieldRef<ConditionCounterCreator, object> RefCounter;

        public readonly RefHelper.FieldRef<object, IList> RefConditionsList;

        public readonly RefHelper.PropertyRef<object, EQuestStatus> RefQuestStatus;

        public readonly RefHelper.PropertyRef<object, object> RefTemplate;

        public readonly RefHelper.PropertyRef<object, string> RefNameLocaleKey;

        public readonly RefHelper.PropertyRef<object, object> RefAvailableForFinishConditions;

        private ReflectionModel()
        {
            if (EFTVersion.AkiVersion > EFTVersion.Parse("3.7.6"))
            {
                RefQuests = RefHelper.PropertyRef<object, object>.Create(_PlayerHelper.RefQuestController.FieldType,
                    "Quests");
            }
            else
            {
                RefQuests = RefHelper.FieldRef<object, object>.Create(_PlayerHelper.RefQuestController.FieldType,
                    "Quests");
            }

            RefQuestsList = RefHelper.FieldRef<object, IList>.Create(RefQuests.RefType, "list_0");

            var questDataType = RefQuestsList.FieldType.GetGenericArguments()[0];

            RefQuestStatus = RefHelper.PropertyRef<object, EQuestStatus>.Create(questDataType, "QuestStatus");
            RefTemplate = RefHelper.PropertyRef<object, object>.Create(questDataType, "Template");

            if (EFTVersion.AkiVersion > EFTVersion.Parse("2.3.1"))
            {
                RefPlayerGroup = RefHelper.FieldRef<object, int>.Create(RefTemplate.PropertyType, "PlayerGroup");
            }

            RefLocationId = RefHelper.FieldRef<object, string>.Create(RefTemplate.PropertyType, "LocationId");

            RefNameLocaleKey = RefHelper.PropertyRef<object, string>.Create(RefTemplate.PropertyType, "NameLocaleKey");
            RefTraderId = RefHelper.FieldRef<object, string>.Create(RefTemplate.PropertyType, "TraderId");
            RefAvailableForFinishConditions =
                RefHelper.PropertyRef<object, object>.Create(questDataType, "AvailableForFinishConditions");
            RefAvailableForFinishConditionsList =
                RefHelper.FieldRef<object, IList>.Create(RefAvailableForFinishConditions.PropertyType, "list_0");

            if (EFTVersion.AkiVersion > EFTVersion.Parse("3.7.6"))
            {
                RefCounter = RefHelper.FieldRef<ConditionCounterCreator, object>.Create("_templateConditions");
                RefConditions = RefHelper.FieldRef<object, object>.Create(RefCounter.FieldType, "Conditions");
            }
            else
            {
                RefCounter = RefHelper.FieldRef<ConditionCounterCreator, object>.Create("counter");
                RefConditions = RefHelper.PropertyRef<object, object>.Create(RefCounter.FieldType, "conditions");
            }

            RefConditionsList = RefHelper.FieldRef<object, IList>.Create(RefConditions.RefType, "list_0");

            RefLootList =
                RefHelper.FieldRef<object, List<LootItem>>.Create(_GameWorldHelper.RefLootItems.FieldType, "list_0");
        }
    }
}