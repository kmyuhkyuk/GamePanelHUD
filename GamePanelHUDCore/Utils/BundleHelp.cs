using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine;

namespace GamePanelHUDCore.Utils
{
    public class BundleHelp
    {
        public static AssetBundle LoadBundle(string bundlepath)
        {
            AssetBundle assetBundle = AssetBundle.LoadFromFile(bundlepath);

            if (assetBundle == null)
            {
                GamePanelHUDCorePlugin.LogLogger.LogError("Failed to load AssetBundle!");

                return null;
            }
            else
            {
                return assetBundle;
            }
        }

        public static async Task<AssetBundle> LoadAsyncBundle(string bundlepath)
        {
            var www = AssetBundle.LoadFromFileAsync(bundlepath);

            while (!www.isDone)
                await Task.Yield();

            if (www.assetBundle == null)
            {
                GamePanelHUDCorePlugin.LogLogger.LogError("Failed to load AssetBundle!");

                return null;
            }
            else
            {
                return www.assetBundle;
            }
        }

        public static T[] LoadAllAsset<T>(AssetBundle assetbundle) where T : UnityEngine.Object
        {
            if (assetbundle != null)
            {
                return assetbundle.LoadAllAssets<T>();
            }
            else
            {
                return null;
            }
        }

        public static async Task<T[]> LoadAsyncAllAsset<T>(AssetBundle assetbundle) where T : UnityEngine.Object
        {
            if (assetbundle != null)
            {
                var www = assetbundle.LoadAllAssetsAsync<T>();

                while (!www.isDone)
                    await Task.Yield();

                if (www.allAssets != null)
                {
                    return Array.ConvertAll(www.allAssets, x => (T)x);
                }
                else
                {
                    return null;
                }
            }
            else
            {
                return null;
            }
        }

        public static GameObject InitAsset(GameObject prefab, Transform transform)
        {
            if (prefab != null)
            {
                return GameObject.Instantiate(prefab, transform);
            }
            else
            {
                return null;
            }
        }

        public class AssetData<T>
        {
            public Dictionary<string, T> Asset;

            public Dictionary<string, T> Init;

            public AssetData(Dictionary<string, T> asset, Dictionary<string, T> init)
            {
                Asset = asset;
                Init = init;
            }
        }
    }
}
