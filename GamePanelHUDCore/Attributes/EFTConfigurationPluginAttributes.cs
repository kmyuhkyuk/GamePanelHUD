namespace GamePanelHUDCore.Attributes
{
    // ReSharper disable InvalidXmlDocComment
    // ReSharper disable FieldCanBeMadeReadOnly.Global
    // ReSharper disable NotAccessedField.Global
    /// <summary>
    ///     You can copy this file to any project and bind Attribute to Plugin
    /// </summary>
    /// <remarks>
    ///     <see cref="EFTConfigurationPlugin" /> will auto search Attribute by name and fields
    /// </remarks>
    [System.AttributeUsage(System.AttributeTargets.Class)]
    public sealed class EFTConfigurationPluginAttributes : System.Attribute
    {
        /// <summary>
        ///     Your Mod Url From Aki Hub Mod page
        /// </summary>
        public string ModUrl;

        /// <summary>
        ///     Never displayed Plugin
        /// </summary>
        /// <remarks>
        ///     Copy <see cref="BrowsableAttribute" />
        /// </remarks>
        public bool HidePlugin;

        /// <summary>
        ///     Keep plugins displayed with not any setting
        /// </summary>
        /// <remarks>
        ///     It priority lower than <see cref="HidePlugin" />
        /// </remarks>
        public bool AlwaysDisplay;

        /// <summary>
        ///     Localized folder, Combine Path from bind attribute plugin dll directory
        /// </summary>
        public string LocalizedPath;

        public EFTConfigurationPluginAttributes(string modUrl, string localizedPath = "localized",
            bool hidePlugin = false, bool alwaysDisplay = false)
        {
            ModUrl = modUrl;
            LocalizedPath = localizedPath;
            HidePlugin = hidePlugin;
            AlwaysDisplay = alwaysDisplay;
        }
    }
}