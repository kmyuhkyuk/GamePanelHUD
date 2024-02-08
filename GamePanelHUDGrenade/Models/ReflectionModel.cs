#if !UNITY_EDITOR

using System;
using EFT.InventoryLogic;
using EFTReflection;

namespace GamePanelHUDGrenade.Models
{
    internal class ReflectionModel
    {
        private static readonly Lazy<ReflectionModel> Lazy = new Lazy<ReflectionModel>(() => new ReflectionModel());

        public static ReflectionModel Instance => Lazy.Value;

        public readonly RefHelper.PropertyRef<object, ThrowWeapType> RefThrowType;

        private ReflectionModel()
        {
            RefThrowType = RefHelper.PropertyRef<object, ThrowWeapType>.Create(
                RefTool.GetEftType(x => x.GetProperty("ThrowType", RefTool.Public) != null), "ThrowType");
        }
    }
}

#endif