﻿using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using CopyBuildAssembly;

// ReSharper disable ClassNeverInstantiated.Global

namespace Build
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            var baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
            var arg = args.ElementAtOrDefault(0);
            var sha = Copy.GetTipSha(args.ElementAtOrDefault(1));

            const string modPath =
                @"R:\Battlestate Games\Client.0.16.1.3.35392\BepInEx\plugins\kmyuhkyuk-GamePanelHUD";

            var modDirectoryName = new DirectoryInfo(modPath).Name;

            const string versionName = "3.3.0";

            var releaseName = $"{modDirectoryName}(Release_{versionName}).7z";

            var previewName = $"{modDirectoryName}(Preview).7z";

            var releasePreview = new[]
            {
                "Release",
                "Preview"
            };

            try
            {
                Copy.CopyFolder(arg, releasePreview, Path.Combine(baseDirectory, "localized"),
                    Path.Combine(modPath, "localized"));

                Copy.CopyAssembly(arg, releasePreview, baseDirectory, Path.Combine(modPath, "core"), new[]
                {
                    "GamePanelHUDCore"
                }, sha);

                Copy.CopyAssembly(arg, releasePreview, baseDirectory, modPath, new[]
                {
                    "GamePanelHUDCompass",
                    "GamePanelHUDGrenade",
                    "GamePanelHUDHit",
                    "GamePanelHUDKill",
                    "GamePanelHUDHealth",
                    "GamePanelHUDWeapon"
                }, sha);

                Copy.GenerateSevenZip(arg, "Release", modPath, releaseName, @"BepInEx\plugins", Array.Empty<string>(),
                    Array.Empty<string>(), new[] { Path.Combine(baseDirectory, "ReadMe.txt") }, Array.Empty<string>());

                Copy.GenerateSevenZip(arg, "Preview", modPath, previewName, @"BepInEx\plugins", Array.Empty<string>(),
                    Array.Empty<string>(), new[] { Path.Combine(baseDirectory, "ReadMe.txt") }, Array.Empty<string>());

                //Unity

                const string unityEditorPath = @"C:\Users\24516\Documents\GamePanelHUD\Assets\Managed";

                Copy.CopyAssembly(arg, "UNITY_EDITOR", baseDirectory, unityEditorPath, new[]
                {
                    "GamePanelHUDCore",
                    "GamePanelHUDCompass",
                    "GamePanelHUDGrenade",
                    "GamePanelHUDHit",
                    "GamePanelHUDKill",
                    "GamePanelHUDHealth",
                    "GamePanelHUDWeapon"
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);

                Console.ReadKey();

                Process.GetCurrentProcess().Kill();
            }
        }
    }
}