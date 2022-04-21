using System;
using UnityEngine;
using UnityEditor;
using UnityEngine.Assertions;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor.IMGUI.Controls;
using SG.AssetBundleBrowser.AssetBundlePacker;
using SG.Utils;

namespace SG.AssetBundleBrowser.AssetBundleDataSource
{
    internal class AssetDatabaseABDataSource : ABDataSource
    {
        public static List<ABDataSource> CreateDataSources()
        {
            var op = new AssetDatabaseABDataSource();
            var retList = new List<ABDataSource>();
            retList.Add(op);
            return retList;
        }

        public string Name
        {
            get { return "Default"; }
        }

        public string ProviderName
        {
            get { return "Built-in"; }
        }

        public string[] GetAssetPathsFromAssetBundle(string assetBundleName)
        {
            return AssetDatabase.GetAssetPathsFromAssetBundle(assetBundleName);
        }

        public string GetAssetBundleName(string assetPath)
        {
            var importer = AssetImporter.GetAtPath(assetPath);
            if (importer == null)
            {
                return string.Empty;
            }

            var bundleName = importer.assetBundleName;
            if (importer.assetBundleVariant.Length > 0)
            {
                bundleName = bundleName + "." + importer.assetBundleVariant;
            }

            return bundleName;
        }

        public string GetImplicitAssetBundleName(string assetPath)
        {
            return AssetDatabase.GetImplicitAssetBundleName(assetPath);
        }

        public string[] GetAllAssetBundleNames()
        {
            var bundles = AssetDatabase.GetAllAssetBundleNames();
            var bundleList = bundles.ToList();
            foreach (string bundle in bundles)
            {
                var assets = AssetDatabase.GetAssetPathsFromAssetBundle(bundle);
                foreach (string asset in assets)
                {
                    if (asset.ToLower().StartsWith("package"))
                    {
                        bundleList.Remove(bundle);
                        break;
                    }
                }
            }

            return bundleList.ToArray();
        }

        public bool IsReadOnly()
        {
            return false;
        }

        public void SetAssetBundleNameAndVariant(string assetPath, string bundleName, string variantName)
        {
            AssetImporter.GetAtPath(assetPath).SetAssetBundleNameAndVariant(bundleName, variantName);
        }

        public void RemoveUnusedAssetBundleNames()
        {
            AssetDatabase.RemoveUnusedAssetBundleNames();
        }

        public bool CanSpecifyBuildTarget
        {
            get { return true; }
        }

        public bool CanSpecifyBuildOutputDirectory
        {
            get { return true; }
        }

        public bool CanSpecifyBuildOptions
        {
            get { return true; }
        }

        public bool BuildAssetBundles(ABBuildInfo info)
        {
            if (info == null)
            {
                DebugUtils.Log("Error in build");
                return false;
            }

            AssetBundleManifest old_manifest = null;
            string manifest_file = EditorCommon.BUILD_PATH + "/" + AssetBundlePacker.Common.MAIN_MANIFEST_FILE_NAME;
            if (File.Exists(manifest_file))
            {
                old_manifest = AssetBundlePacker.Common.LoadMainManifestByPath(manifest_file);
            }

            var buildManifest = BuildPipeline.BuildAssetBundles(info.outputDirectory, info.options, info.buildTarget);
            if (buildManifest == null)
            {
                DebugUtils.Log("Error in build");
                return false;
            }

            foreach (var assetBundleName in buildManifest.GetAllAssetBundles())
            {
                if (info.onBuild != null)
                {
                    info.onBuild(assetBundleName);
                }
            }

            AssetBundleManifest new_manifest = AssetBundlePacker.Common.LoadMainManifestByPath(manifest_file);

            if (new_manifest == null)
            {
                Debug.LogError("Load Fail:" + manifest_file);
            }

            //  检查依赖关系，防止mainapp依赖subapp资源
            var allAssetBundles = new_manifest.GetAllAssetBundles();
            foreach (var assetbundlename in allAssetBundles)
            {
                string[] deps = new_manifest.GetAllDependencies(assetbundlename);
                foreach (var dep in deps)
                {
                    if (assetbundlename.Contains("mainapp") && !dep.Contains("mainapp"))
                    {
                        EditorUtility.DisplayDialog("Error Dependency!!!",
                            assetbundlename + "\n依赖于\n" + dep + "\n请修改!!! ",
                            "OK");
                        Debug.LogError(assetbundlename + " --> " + dep);
                    }
                }
            }

            //zcode.AssetBundlePacker.BuildAssetBundle.CompressAssetBundles()
            BuildAssetBundle.ComparisonAssetBundleManifest(old_manifest, new_manifest);
            BuildAssetBundle.ExportResourcesManifestFile(buildManifest);
            return true;
        }
    }
}