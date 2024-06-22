namespace Unity.Coding.Editor.ApiScraping
{
    internal static class Utilities
    {
#if UNITY_2023_1_OR_NEWER
        public static bool IsFolderImmutable(string path)
        {
            UnityEditor.AssetDatabase.TryGetAssetFolderInfo(path, out var isRoot, out var isImmutable);
            return isImmutable;
        }

#endif
    }
}
