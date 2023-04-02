#if !UNITY_EDITOR
using BepInEx.Logging;
using System;
using System.Threading.Tasks;
using UnityEngine;

namespace GamePanelHUDCore.Utils
{
    public static class BundleHelp
    {
        public static AssetBundle LoadBundle(ManualLogSource logsource, string bundlepath)
        {
            AssetBundle assetBundle = AssetBundle.LoadFromFile(bundlepath);

            if (assetBundle == null)
            {
                logsource.LogError("Failed to load AssetBundle!");

                return null;
            }
            else
            {
                return assetBundle;
            }
        }

        public static async Task<AssetBundle> LoadAsyncBundle(ManualLogSource logsource, string bundlepath)
        {
            var www = AssetBundle.LoadFromFileAsync(bundlepath);

            while (!www.isDone)
                await Task.Yield();

            if (www.assetBundle == null)
            {
                logsource.LogError("Failed to load AssetBundle!");

                return null;
            }
            else
            {
                return www.assetBundle;
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
    }
}
#endif
