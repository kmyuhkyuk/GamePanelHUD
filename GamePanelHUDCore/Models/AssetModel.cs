#if !UNITY_EDITOR

using System.Collections.Generic;

namespace GamePanelHUDCore.Models
{
    public class AssetModel<T>
    {
        public readonly IReadOnlyDictionary<string, T> Asset;

        public readonly IReadOnlyDictionary<string, T> Init;

        public AssetModel(IReadOnlyDictionary<string, T> asset, IReadOnlyDictionary<string, T> init)
        {
            Asset = asset;
            Init = init;
        }
    }
}

#endif