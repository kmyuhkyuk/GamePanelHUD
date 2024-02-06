using System;
using EFTReflection;

namespace GamePanelHUDHealth.Models
{
    internal class ReflectionModel
    {
        private static readonly Lazy<ReflectionModel> Lazy = new Lazy<ReflectionModel>(() => new ReflectionModel());

        public static ReflectionModel Instance => Lazy.Value;

        public readonly RefHelper.PropertyRef<MainMenuController, object> RefHealthController;

        private ReflectionModel()
        {
            RefHealthController = RefHelper.PropertyRef<MainMenuController, object>.Create("HealthController");
        }
    }
}