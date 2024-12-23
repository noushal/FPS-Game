namespace Unity.Multiplayer.Tools.Common
{
    internal static class StringConstants
    {
        public const string k_SettingsMenuLabel   = "Multiplayer Tools";
        public const string k_PackageName         = "com.unity.multiplayer.tools";
        public const string k_PackagePath         = "Packages/" + k_PackageName + "/";
        public const string k_PackageEditorPath   = k_PackagePath       + "Editor/";
        public const string k_PackageSettingsPath = k_PackageEditorPath + "Settings/";

        public const string k_ResourcePrefix            = "UnityMpTools";
        public const string k_ResourcePrefixRnsm        = k_ResourcePrefix + "Rnsm";
        public const string k_ResourcePrefixRnsmDefault = k_ResourcePrefixRnsm + "Default";
    }
}