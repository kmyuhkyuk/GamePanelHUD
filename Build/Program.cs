using System;
using System.IO;
using System.Linq;
using CopyBuildAssembly;

namespace Build
{
    // ReSharper disable once ClassNeverInstantiated.Global
    internal class Program
    {
        private static void Main(string[] args)
        {
            var baseDirectory = AppDomain.CurrentDomain.BaseDirectory;

            const string releasePath =
                "R:\\Battlestate Games\\Client.0.13.5.3.26535\\BepInEx\\plugins\\kmyuhkyuk-GamePanelHUD";

            Copy.CopyAssembly(args.ElementAtOrDefault(0), "Release", baseDirectory, Path.Combine(releasePath, "core"), new[]
            {
                "GamePanelHUDCore"
            });

            Copy.CopyAssembly(args.ElementAtOrDefault(0), "Release", baseDirectory, releasePath, new[]
            {
                "GamePanelHUDCompass",
                "GamePanelHUDGrenade",
                "GamePanelHUDHit",
                "GamePanelHUDKill",
                "GamePanelHUDHealth",
                "GamePanelHUDWeapon"
            });

            const string unityEditorPath = "C:\\Users\\24516\\Documents\\GamePanelHUD\\Assets\\Managed";

            Copy.CopyAssembly(args.ElementAtOrDefault(0), "UNITY_EDITOR", baseDirectory, unityEditorPath,
                new[]
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