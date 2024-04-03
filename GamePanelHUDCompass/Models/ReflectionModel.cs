using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using EFT.Interactive;
using EFT.Quests;
using EFTApi;
using EFTReflection;
using JetBrains.Annotations;
using Newtonsoft.Json;
using static EFTApi.EFTHelpers;

namespace GamePanelHUDCompass.Models
{
    internal class ReflectionModel
    {
        private static readonly Lazy<ReflectionModel> Lazy = new Lazy<ReflectionModel>(() => new ReflectionModel());

        public static ReflectionModel Instance => Lazy.Value;

        public readonly RefHelper.IRef<object, IEnumerable> RefQuests;

        public readonly RefHelper.IRef<object, IEnumerable> RefConditions;

        public readonly RefHelper.FieldRef<object, List<LootItem>> RefLootList;

        public readonly RefHelper.IRef<object, string> RefLocationId;

        [CanBeNull] public readonly RefHelper.IRef<object, int> RefPlayerGroup;

        public readonly RefHelper.IRef<object, string> RefTraderId;

        public readonly RefHelper.FieldRef<ConditionCounterCreator, object> RefCounter;

        public readonly RefHelper.PropertyRef<object, EQuestStatus> RefQuestStatus;

        public readonly RefHelper.PropertyRef<object, object> RefTemplate;

        public readonly RefHelper.FieldRef<object, string> RefId;

        public readonly RefHelper.FieldRef<object, string> RefName;

        public readonly RefHelper.PropertyRef<object, IEnumerable> RefAvailableForFinishConditions;

        private ReflectionModel()
        {
            if (EFTVersion.AkiVersion > EFTVersion.Parse("3.7.6"))
            {
                RefQuests = RefHelper.PropertyRef<object, IEnumerable>.Create(
                    _QuestControllerHelper.RefQuestController.FieldType,
                    "Quests");
            }
            else
            {
                RefQuests = RefHelper.FieldRef<object, IEnumerable>.Create(
                    _QuestControllerHelper.RefQuestController.FieldType,
                    "Quests");
            }

            var questDataType = RefQuests.RefType.BaseType?.GetGenericArguments()[0];

            RefQuestStatus = RefHelper.PropertyRef<object, EQuestStatus>.Create(questDataType, "QuestStatus");
            RefTemplate = RefHelper.PropertyRef<object, object>.Create(questDataType, "Template");

            if (EFTVersion.AkiVersion > EFTVersion.Parse("3.7.6"))
            {
                RefPlayerGroup = RefHelper.PropertyRef<object, int>.Create(RefTemplate.PropertyType, "PlayerGroup");
            }
            else if (EFTVersion.AkiVersion > EFTVersion.Parse("3.0.0"))
            {
                RefPlayerGroup = RefHelper.FieldRef<object, int>.Create(RefTemplate.PropertyType, "PlayerGroup");
            }

            if (EFTVersion.AkiVersion > EFTVersion.Parse("3.7.6"))
            {
                RefLocationId = RefHelper.PropertyRef<object, string>.Create(RefTemplate.PropertyType, "LocationId");
                RefTraderId = RefHelper.PropertyRef<object, string>.Create(RefTemplate.PropertyType, "TraderId");
            }
            else
            {
                RefLocationId = RefHelper.FieldRef<object, string>.Create(RefTemplate.PropertyType, "LocationId");
                RefTraderId = RefHelper.FieldRef<object, string>.Create(RefTemplate.PropertyType, "TraderId");
            }

            RefId = RefHelper.FieldRef<object, string>.Create(RefTemplate.PropertyType, "Id");
            RefName = RefHelper.FieldRef<object, string>.Create(RefTemplate.PropertyType,
                x => x.GetCustomAttribute<JsonPropertyAttribute>()?.PropertyName == "name");
            RefAvailableForFinishConditions =
                RefHelper.PropertyRef<object, IEnumerable>.Create(questDataType, "AvailableForFinishConditions");

            if (EFTVersion.AkiVersion > EFTVersion.Parse("3.7.6"))
            {
                RefCounter = RefHelper.FieldRef<ConditionCounterCreator, object>.Create("_templateConditions");
                RefConditions = RefHelper.FieldRef<object, IEnumerable>.Create(RefCounter.FieldType, "Conditions");
            }
            else
            {
                RefCounter = RefHelper.FieldRef<ConditionCounterCreator, object>.Create("counter");
                RefConditions = RefHelper.PropertyRef<object, IEnumerable>.Create(RefCounter.FieldType, "conditions");
            }

            RefLootList =
                RefHelper.FieldRef<object, List<LootItem>>.Create(_GameWorldHelper.RefLootItems.FieldType, "list_0");
        }
    }
}