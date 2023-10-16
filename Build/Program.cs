﻿using System;
using System.Collections.Generic;
using System.IO;
using SevenZip;

namespace Build
{
    // ReSharper disable once ClassNeverInstantiated.Global
    internal class Program
    {
        private static readonly string BaseDirectory = AppDomain.CurrentDomain.BaseDirectory;

        private static void Main(string[] args)
        {
            var hasArgs = args.Length > 0;

            var configurationName = hasArgs ? args[0] : "Release";

            switch (configurationName)
            {
                case "Release":
                    const string releasePath =
                        "R:\\Battlestate Games\\Client.0.13.5.3.26535\\BepInEx\\plugins\\kmyuhkyuk-GamePanelHUD";

                    Copy(Path.Combine(releasePath, "core"), new[]
                    {
                        "GamePanelHUDCore"
                    });

                    Copy(releasePath, new[]
                    {
                        "GamePanelHUDCompass",
                        "GamePanelHUDGrenade",
                        "GamePanelHUDHit",
                        "GamePanelHUDLife",
                        "GamePanelHUDMag"
                    });

                    SevenZip(releasePath);
                    break;
                case "UNITY_EDITOR":
                    const string unityEditorPath = "C:\\Users\\24516\\Documents\\GamePanelHUD\\Assets\\Managed";

                    Copy(unityEditorPath, new[]
                    {
                        "GamePanelHUDCore",
                        "GamePanelHUDCompass",
                        "GamePanelHUDGrenade",
                        "GamePanelHUDHit",
                        "GamePanelHUDLife",
                        "GamePanelHUDMag"
                    });
                    break;
            }

            if (!hasArgs)
            {
                Console.WriteLine("\nPress any key to close console app...");
                Console.ReadKey();
            }
        }

        private static void SevenZip(string path)
        {
            var directory = new DirectoryInfo(path);

            if (directory.Parent == null)
            {
                throw new ArgumentNullException(nameof(directory.Parent));
            }

            SevenZipBase.SetLibraryPath(
                $@"{Environment.CurrentDirectory}\{(IntPtr.Size == 4 ? "x86" : "x64")}\7z.dll");

            var compressor = new SevenZipCompressor();

            var filesDictionary = new Dictionary<string, string>();
            foreach (var fileInfo in directory.GetFiles("*", SearchOption.AllDirectories))
            {
                filesDictionary.Add(
                    fileInfo.FullName.Replace(directory.Parent.FullName, "BepInEx\\plugins"),
                    fileInfo.FullName);
            }

            compressor.CompressFileDictionary(filesDictionary,
                File.Create(Path.Combine(directory.Parent.FullName, $"{directory.Name}.7z")));
        }

        private static void Copy(string toPath, string[] dllNames)
        {
            foreach (var dllName in dllNames)
            {
                var dllFullName = $"{dllName}.dll";

                var dllPath = Path.Combine(BaseDirectory, dllFullName);

                var toDLLPath = Path.Combine(toPath, dllFullName);

                Console.WriteLine($"Copy {Path.GetFileName(dllFullName)} to \n{toDLLPath}");

                File.Copy(dllPath, toDLLPath, true);
            }
        }
    }
}