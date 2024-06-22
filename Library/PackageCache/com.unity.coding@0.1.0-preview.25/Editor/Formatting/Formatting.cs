using System.Collections.Generic;
using System;
using UnityEditor.Compilation;
using UnityEditor.PackageManager;
using UnityEngine;

namespace Unity.Coding.Editor.Formatting
{
    /// <summary>
    /// Methods for invoking and validating code formatting
    /// </summary>
    public static class Formatting
    {
        /// <summary>
        /// Format files in your project based on .editorconfig rules.
        ///
        /// Note: Only files under "Assets/" and "Packages/" will be considered for formating.
        /// Please see full documentation for detail on formatters and EditorConfig.
        /// </summary>
        /// <returns>Collection of files that were formatted</returns>
        public static ICollection<string> Format()
        {
            var pathsToFormat = new List<string>();
            Utility.CollectUnityAssetPathsRecursively("Assets", pathsToFormat);
            Utility.CollectUnityAssetPathsRecursively("Packages", pathsToFormat);

            var formatted = Utility.Format(pathsToFormat);

            return formatted.Keys;
        }

        /// <summary>
        /// Recursively format files in a specific project folder based on .editorconfig rules.
        ///
        /// Note: Only files under "Assets/" and "Packages/" will be considered for formating.
        /// Please see full documentation for detail on formatters and EditorConfig.
        /// </summary>
        /// <param name="path">Path to file or folder to format.</param>
        /// <returns>Collection of files that were formatted</returns>
        public static ICollection<string> Format(string path)
        {
            var pathsToFormat = new List<string>();
            Utility.CollectUnityAssetPathsRecursively(path, pathsToFormat);
            var formatted = Utility.Format(pathsToFormat);

            return formatted.Keys;
        }

        /// <summary>
        /// Recursively validates if files in a specific project folder are correctly formatted based on .editorconfig rules.
        ///
        /// Note: Only files under "Assets/" and "Packages/" will be considered for formating.
        /// Please see full documentation for detail on formatters and EditorConfig.
        /// </summary>
        /// <param name="path">Path to the file or directory that should be validated</param>
        /// <param name="failedFileList">List of files that have failed validation.</param>
        /// <returns>True if no files have failed validation, false otherwise</returns>
        public static bool ValidateAllFilesFormatted(string path, List<string> failedFileList = null)
        {
            var pathsToFormat = new List<string>();
            Utility.CollectUnityAssetPathsRecursively(path, pathsToFormat);
            var formatted = Utility.Format(pathsToFormat, Utility.FormatUtilityOptions.Default | Utility.FormatUtilityOptions.DryRun);

            if (failedFileList != null)
                failedFileList.AddRange(formatted.Keys);

            return formatted.Count == 0;
        }

        static void ValidateAllProjectFilesFormatted()
        {
            var pathsToFormat = new List<string>();
            var localFolders = new HashSet<string>();
            Utility.CollectUnityAssetPathsRecursively("Assets", pathsToFormat);
            foreach (var assembly in CompilationPipeline.GetAssemblies())
            {
                var asmDefPath = CompilationPipeline.GetAssemblyDefinitionFilePathFromAssemblyName(assembly.name);
                if (asmDefPath == null)
                    continue;
                var packageInfo = UnityEditor.PackageManager.PackageInfo.FindForAssetPath(asmDefPath);
                if (packageInfo == null || packageInfo.source != PackageSource.Embedded && packageInfo.source != PackageSource.Local)
                {
                    continue;
                }
                if (localFolders.Add(packageInfo.assetPath))
                    Utility.CollectUnityAssetPathsRecursively(packageInfo.assetPath, pathsToFormat);
            }
            var formatted = Utility.Format(pathsToFormat, Utility.FormatUtilityOptions.Default | Utility.FormatUtilityOptions.DryRun);
            if (formatted.Count > 0)
            {
                throw new Exception("Following files require formatting:\n" + string.Join("\n", formatted.Keys));
            }
        }
    }
}
