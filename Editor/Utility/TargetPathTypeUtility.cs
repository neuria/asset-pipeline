using System.IO;
using Daihenka.AssetPipeline.Processors;
using UnityEditor;

namespace Daihenka.AssetPipeline
{
    internal static class TargetPathTypeUtility
    {
        public static string GetFolderPath(this TargetPathType pathType, string source, string destination, DefaultAsset targetFolder = null)
        {
            if (pathType == TargetPathType.TargetFolder && targetFolder)
            {
                return AssetDatabase.GetAssetPath(targetFolder);
            }

            if (pathType == TargetPathType.Absolute) {
                return destination.FixPathSeparators();
            }

            var destinationPath = File.Exists(source) ? Path.GetDirectoryName(source) : source;
            if (pathType == TargetPathType.Relative)
            {
                destinationPath = Path.Combine(destinationPath, destination).FixPathSeparators();
            }
            else if (pathType == TargetPathType.ParentFolder)
            {
                destinationPath = Path.Combine(destinationPath, "..").FixPathSeparators();
            }

            return destinationPath;
        }
    }
}