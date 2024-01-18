﻿using System;
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

            const string modPath =
                @"R:\Battlestate Games\Client.0.13.5.3.26535\BepInEx\plugins\kmyuhkyuk-GamePanelHUD";

            var previewName = $"{new DirectoryInfo(modPath).Name}-(Preview).7z";

            var releasePreview = new[]
            {
                "Release",
                "Preview"
            };

            Copy.CopyAssembly(arg, releasePreview, baseDirectory, Path.Combine(modPath, "core"), new[]
            {
                "GamePanelHUDCore"
            });

            Copy.CopyAssembly(arg, releasePreview, baseDirectory, modPath, new[]
            {
                "GamePanelHUDCompass",
                "GamePanelHUDGrenade",
                "GamePanelHUDHit",
                "GamePanelHUDKill",
                "GamePanelHUDHealth",
                "GamePanelHUDWeapon"
            });

            Copy.GenerateSevenZip(arg, "Release", modPath, null, @"BepInEx\plugins", Array.Empty<string>(),
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
    }
}