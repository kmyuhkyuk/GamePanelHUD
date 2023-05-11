#if !UNITY_EDITOR
using BepInEx.Logging;
using System;
using System.Threading.Tasks;
using UnityEngine;

namespace GamePanelHUDCore.Utils
{
    public static class BundleHelp
    {
        public static AssetBundle LoadBundle(ManualLogSource logSource, string bundlePath)
        {
            AssetBundle assetBundle = AssetBundle.LoadFromFile(bundlePath);

            if (assetBundle == null)
            {
                logSource.LogError("Failed to load AssetBundle!");

                return null;
            }
            else
            {
                return assetBundle;
            }
        }

        public static async Task<AssetBundle> LoadAsyncBundle(ManualLogSource logSource, string bundlePath)
        {
            var www = AssetBundle.LoadFromFileAsync(bundlePath);

            while (!www.isDone)
                await Task.Yield();

            if (www.assetBundle == null)
            {
                logSource.LogError("Failed to load AssetBundle!");

                return null;
            }
            else
            {
                return www.assetBundle;
            }
        }

        public static async Task<T[]> LoadAsyncAllAsset<T>(AssetBundle assetBundle) where T : UnityEngine.Object
        {
            if (assetBundle != null)
            {
                var www = assetBundle.LoadAllAssetsAsync<T>();

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
    }
}
#endif
