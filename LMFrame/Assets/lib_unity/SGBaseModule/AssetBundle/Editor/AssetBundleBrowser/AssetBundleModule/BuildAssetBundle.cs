/***************************************************************
 * Copyright 2016 By Zhang Minglin
 * Author: Zhang Minglin
 * Create: 2016/11/30
 * Note  : 打包AssetBundle
***************************************************************/

using System;
using UnityEditor;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using ICSharpCode.SharpZipLib.Checksums;

namespace SG.AssetBundleBrowser.AssetBundlePacker
{
    /// <summary>
    /// 
    /// </summary>
    public static class BuildAssetBundle
    {
        /// <summary>
        ///   打包AssetBundle
        /// </summary>
        public static void BuildAllAssetBundlesToTarget(BuildTarget target
            , BuildAssetBundleOptions options)
        {
            string manifest_file = EditorCommon.BUILD_PATH + "/" + Common.MAIN_MANIFEST_FILE_NAME;
            AssetBundleManifest old_manifest = Common.LoadMainManifestByPath(manifest_file);

            if (!Directory.Exists(EditorCommon.BUILD_PATH))
                Directory.CreateDirectory(EditorCommon.BUILD_PATH);
            BuildPipeline.BuildAssetBundles(EditorCommon.BUILD_PATH, options, target);
            AssetDatabase.Refresh();

            AssetBundleManifest new_manifest = Common.LoadMainManifestByPath(manifest_file);
            ComparisonAssetBundleManifest(old_manifest, new_manifest);
            ExportResourcesManifestFile(new_manifest);

            string resoures_manifest_file = EditorCommon.BUILD_PATH + "/" + Common.RESOURCES_MANIFEST_FILE_NAME;
            ResourcesManifest resoureces_manifest = Common.LoadResourcesManifestByPath(resoures_manifest_file);
            CompressAssetBundles(resoureces_manifest, ref resoureces_manifest);
            resoureces_manifest.Save(resoures_manifest_file);
            CopyNativeAssetBundleToStreamingAssets(resoureces_manifest, false);
            EditorUtility.ClearProgressBar();
            AssetDatabase.Refresh();
        }

        /// <summary>
        /// 压缩AssetBundle
        /// </summary>
        public static bool CompressAssetBundles(ResourcesManifest old_resources_manifest,
            ref ResourcesManifest resources_manifest)
        {
            if (resources_manifest == null)
                return false;
            if (resources_manifest.RawData == null)
                return false;
            if (resources_manifest.RawData.AssetBundles == null)
                return false;

            // 通过记录新旧版本中压缩标记
            // 判定资源是否需要压缩、删除压缩包
            Dictionary<string, int> dic = new Dictionary<string, int>();
            int old_version_bit = 0x1; // 旧版本中压缩
            int new_version_bit = 0x2; // 新版本中压缩
            if (old_resources_manifest.RawData != null && old_resources_manifest.RawData.AssetBundles != null)
            {
                var itr = old_resources_manifest.RawData.AssetBundles.GetEnumerator();
                while (itr.MoveNext())
                {
                    if (itr.Current.Value.IsCompress)
                    {
                        string name = itr.Current.Value.AssetBundleName;
                        if (!dic.ContainsKey(name))
                            dic.Add(name, old_version_bit);
                        else
                            dic[name] |= old_version_bit;
                    }
                }
            }

            {
                var itr = resources_manifest.RawData.AssetBundles.GetEnumerator();
                while (itr.MoveNext())
                {
                    if (itr.Current.Value.IsCompress)
                    {
                        string name = itr.Current.Value.AssetBundleName;
                        if (!dic.ContainsKey(name))
                            dic.Add(name, new_version_bit);
                        else
                            dic[name] |= new_version_bit;
                    }
                }
            }

            float current = 0f;
            float total = resources_manifest.RawData.AssetBundles.Count;
            var itr1 = dic.GetEnumerator();
            while (itr1.MoveNext())
            {
                string name = itr1.Current.Key;
                int mask = itr1.Current.Value;

                //过滤主AssetBundle文件
                if (name == Common.MAIN_MANIFEST_FILE_NAME)
                    continue;

                string action;
                string file_name = EditorCommon.BUILD_PATH + "/" + name;
                if ((mask & old_version_bit) > 0
                    && (mask & new_version_bit) == 0)
                {
                    // 旧版本中存在，新版本不存在
                    // 删除压缩包
                    string compress_file = Compress.GetCompressFileName(file_name);
                    File.Delete(compress_file);
                    File.Delete(compress_file + Common.NATIVE_MANIFEST_EXTENSION);

                    //重写ResourcesManifest数据
                    var ab = resources_manifest.RawData.AssetBundles[name];
                    ab.CompressSize = 0;

                    action = "Delete Compress";
                }
                else if ((mask & new_version_bit) > 0)
                {
                    //新版本中存在，压缩文件
                    Compress.CompressFile(file_name);

                    //重写ResourcesManifest数据
                    var ab = resources_manifest.RawData.AssetBundles[name];
                    ab.CompressSize =
                        AssetBundleBrowser.FileHelper.GetFileSize(Compress.GetCompressFileName(file_name));

                    action = "Compress";
                }
                else
                {
                    action = "Ignore";
                }

                //更新进度条
                if (ShowProgressBar("", action + " " + name, current / total))
                {
                    EditorUtility.ClearProgressBar();
                    return false;
                }
            }

            EditorUtility.ClearProgressBar();
            return true;
        }

        /// <summary>
        /// 拷贝资源包文件
        /// </summary>
        public static bool CopyResourcesPackageFileToStreamingAssets()
        {
            string file = Common.RESOURCES_PACKAGE_FILE_NAME;
            string src_file_name = EditorCommon.BUILD_PATH + "/" + file;
            string dest_file_name = Common.INITIAL_PATH + "/" + file;
            bool result = AssetBundleBrowser.FileHelper.CopyFile(src_file_name, dest_file_name, true);
            if (result)
                AssetDatabase.Refresh();

            return result;
        }

        /// <summary>
        /// 拷贝本地AssetBunle至StreamingAssets目录
        /// </summary>
        public static bool CopyNativeAssetBundleToStreamingAssets(ResourcesManifest resources_manifest,
            bool isStrictMode)
        {
            string folderName = "mainapp";
            bool succeed = false;
            if (!SG.SettingReader.ScriptableObject.isSingleGameApp)
            {
                succeed = DoCopyNativeAssetBundleToStreamingAssets(resources_manifest, folderName, isStrictMode);
                var folders = SG.SettingReader.ScriptableObject.nativeInstallGame;
                if (folders == null || folders.Count == 0)
                {
                    EditorUtility.DisplayDialog("Warning", "No sub-app is native installed", "OK");
                }
                else
                {
                    for (int i = 0; i < folders.Count; i++)
                    {
                        var folder = folders[i];
                        EditorUtility.DisplayDialog("Hint", folder + " is native installed", "OK");
                        succeed &= DoCopyNativeAssetBundleToStreamingAssets(resources_manifest, folder, isStrictMode);
                    }
                }
            }
            else
            {
                var gameName = SG.SettingReader.ScriptableObject.GameName;
                if (string.IsNullOrEmpty(gameName))
                {
                    EditorUtility.DisplayDialog("Error", "No game name in (SubGame)SettingAsset", "OK");
                    return false;
                }


                gameName = gameName.ToLower();
                succeed = DoCopyNativeAssetBundleToStreamingAssets(resources_manifest, folderName, isStrictMode) &&
                          DoCopyNativeAssetBundleToStreamingAssets(resources_manifest, gameName, isStrictMode);

                if (succeed)
                {
                    EditorUtility.DisplayDialog("Hint", gameName + " and mainapp are native installed", "OK");
                }
            }

            return succeed;
        }

        public static bool DoCopyNativeAssetBundleToStreamingAssets(ResourcesManifest resources_manifest,
            string folderName = "mainapp", bool isStrictMode = true)
        {
            if (resources_manifest == null)
                return false;

            try
            {
                //清空本地资源目录
                if (ShowProgressBar("", "清空本地资源目录", 0f))
                {
                    EditorUtility.ClearProgressBar();
                    return false;
                }


                if (!Directory.Exists(Common.INITIAL_PATH + "/" + folderName))
                    Directory.CreateDirectory(Common.INITIAL_PATH + "/" + folderName);
                else
                    AssetBundleBrowser.FileHelper.DeleteAllChild(Common.INITIAL_PATH + "/" + folderName
                        , FileAttributes.Hidden | FileAttributes.System);

                //拷贝所有配置文件
                for (int i = 0; i < Common.MAIN_CONFIG_NAME_ARRAY.Length; ++i)
                {
                    string file = Common.MAIN_CONFIG_NAME_ARRAY[i];
                    string src_file_name = EditorCommon.BUILD_PATH + "/" + file;
                    string dest_file_name = Common.INITIAL_PATH + "/" + folderName + "/" + file;
                    float progress = (float) (i + 1) / (float) Common.MAIN_CONFIG_NAME_ARRAY.Length;
                    if (ShowProgressBar("", "Copy " + file, progress))
                    {
                        EditorUtility.ClearProgressBar();
                        return false;
                    }

                    AssetBundleBrowser.FileHelper.CopyFile(src_file_name, dest_file_name, true);
                }

                //拷贝AssetBundle文件
                if (resources_manifest.RawData != null && resources_manifest.RawData.AssetBundles != null)
                {
                    float current = 0f;
                    float total = resources_manifest.RawData.AssetBundles.Count;
                    float sizeLimit = 20;
                    foreach (var desc in resources_manifest.RawData.AssetBundles.Values)
                    {
                        current += 1f;

                        if (isStrictMode && desc.Size > sizeLimit * 1024 * 1024)
                        {
                            throw new TooLargeAssetBundleException(
                                "AssetBundle size larger than " + sizeLimit + "MB and stopped copying");
                        }
                        /// 目前的情况就是为了方便打包和降低流程复杂程度，这里全部自动复制！

                        if (desc.AssetBundleName == Common.MAIN_MANIFEST_FILE_NAME ||
                            !desc.AssetBundleName.ToLower().StartsWith(folderName.ToLower()))
                            continue;
//
                    if (desc.IsGeneral)
                        {
                            AssetBundleBrowser.FileHelper.CopyFile(EditorCommon.BUILD_PATH + "/" + desc.AssetBundleName
                                , Common.INITIAL_PATH + "/" + folderName + "/" + desc.AssetBundleName, true);
                        }

                        //更新进度条
                        if (ShowProgressBar("", "Copy " + desc.AssetBundleName, current / total))
                        {
                            EditorUtility.ClearProgressBar();
                            return false;
                        }
                    }

                    if (folderName != "mainapp")
                    {
                        FileHelper.DeleteAllChild(Common.INITIAL_PATH + "/" + folderName + "/" + "mainapp",
                            FileAttributes.Hidden | FileAttributes.System | FileAttributes.Normal);
                        FileHelper.DeleteAllChild(Common.INITIAL_PATH + "/mainapp/" + folderName,
                            FileAttributes.Hidden | FileAttributes.System | FileAttributes.Normal);

                        if (Directory.Exists(Common.INITIAL_PATH + "/" + folderName + "/" + "mainapp"))
                        {
                            Directory.Delete(Common.INITIAL_PATH + "/" + folderName + "/" + "mainapp", true);
                        }

                        if (Directory.Exists(Common.INITIAL_PATH + "/mainapp/" + folderName))
                        {
                            Directory.Delete(Common.INITIAL_PATH + "/mainapp/" + folderName, true);
                        }
                    }
                }

                EditorUtility.ClearProgressBar();
                AssetDatabase.Refresh();
                return true;
            }
            catch (TooLargeAssetBundleException e)
            {
                EditorUtility.DisplayDialog("Error",
                    e.Message, "OK");
                EditorUtility.ClearProgressBar();
                return false;
            }
            catch (Exception e)
            {
                Debug.LogError(e.Message + "\n" + e.StackTrace);
                EditorUtility.ClearProgressBar();
                return false;
            }
        }

        class TooLargeAssetBundleException : Exception
        {
            public TooLargeAssetBundleException(string message) : base(message)
            {
            }
        }

        /// <summary>
        /// 根据AssetBundle导出ResourcesManifest文件
        /// </summary>
        public static void ExportResourcesManifestFile(AssetBundleManifest manifest)
        {
            ResourcesManifest info = new ResourcesManifest();

            //读取所有AssetBundle
            string root_dir = EditorCommon.BUILD_PATH + "/";

            var analyzer = new AssetBundleManifestAnalyzer(manifest);
            analyzer.Analyze();

            List<string> scenes = new List<string>();
            if (manifest != null)
            {
                uint mainCrc = 0;
                BuildPipeline.GetCRCForAssetBundle(root_dir + Common.MAIN_MANIFEST_FILE_NAME, out mainCrc);
                //读取主AssetBundle
                ResourcesManifestRawData.AssetBundle desc = new ResourcesManifestRawData.AssetBundle
                {
                    LoadOrder = analyzer.GetGeneration(Common.MAIN_MANIFEST_FILE_NAME, root_dir),
                    AssetBundleName = Common.MAIN_MANIFEST_FILE_NAME,
                    Size = AssetBundleBrowser.FileHelper.GetFileSize(root_dir + Common.MAIN_MANIFEST_FILE_NAME),
                    Crc = mainCrc,
                    IsGeneral = true
                };
                info.RawData.AssetBundles.Add(Common.MAIN_MANIFEST_FILE_NAME, desc);

                //读取其它AssetBundle
                foreach (var name in manifest.GetAllAssetBundles())
                {
                    uint crc = 0;
                    BuildPipeline.GetCRCForAssetBundle(root_dir + name, out crc);
                    desc = new ResourcesManifestRawData.AssetBundle
                    {
                        AssetBundleName = name,
                        LoadOrder = analyzer.GetGeneration(name, root_dir),
                        Size = AssetBundleBrowser.FileHelper.GetFileSize(root_dir + name),
                        Crc = crc,
                    };
                    AssetBundle ab = null;
                    var loadedAssetBundles = AssetBundle.GetAllLoadedAssetBundles();
                    foreach (var loadedAssetBundle in loadedAssetBundles)
                    {
                        if (loadedAssetBundle.name == name)
                        {
                            ab = loadedAssetBundle;
                        }
                    }

                    if (ab == null)
                    {
                        ab = AssetBundle.LoadFromFile(root_dir + name);
                    }

                    foreach (var asset in ab.GetAllAssetNames())
                    {
                        desc.Assets.Add(asset);
                    }

                    foreach (var scene in ab.GetAllScenePaths())
                    {
                        desc.Scenes.Add(scene);
                        scenes.Add(scene);
                    }

                    ab.Unload(false);

                    info.RawData.AssetBundles.Add(name, desc);
                }
            }

            //读取所有Scene信息
            for (int i = 0; i < scenes.Count; ++i)
            {
                ResourcesManifestRawData.Scene scene_desc = new ResourcesManifestRawData.Scene();
                scene_desc.SceneLevelName = Path.GetFileNameWithoutExtension(scenes[i]);
                scene_desc.ScenePath = scenes[i];
                scene_desc.SceneConfigPath = SceneConfig.GetSceneConfigPath(scenes[i]);
                info.RawData.Scenes.Add(scene_desc.SceneLevelName, scene_desc);
            }

            //读取旧的ResourcesInfo，同步其它额外的数据
            // ResourcesManifest old_info = new ResourcesManifest();
            // old_info.Load(EditorCommon.RESOURCES_MANIFEST_FILE_PATH);
            // if (old_info.Data != null && old_info.Data.AssetBundles.Count > 0)
            // {
            //     foreach (var desc in old_info.Data.AssetBundles.Values)
            //     {
            //         if (info.Data.AssetBundles.ContainsKey(desc.AssetBundleName))
            //         {
            //             // info.Data.AssetBundles[desc.AssetBundleName].IsNative = desc.IsNative;
            //             // info.Data.AssetBundles[desc.AssetBundleName].IsPermanent = desc.IsPermanent;
            //         }
            //     }
            //
            //     // 同步旧的版本号
            //     info.Data.Version = old_info.Data.Version;
            // }

            //保存ResourcesInfo
            string resources_manifest_file = EditorCommon.BUILD_PATH + "/" + Common.RESOURCES_MANIFEST_FILE_NAME;
            info.Save(resources_manifest_file);

            AssetDatabase.Refresh();
        }

        /// <summary>
        /// 比对AssetBundleManifest, 删除冗余的AssetBundle
        /// </summary>
        public static void ComparisonAssetBundleManifest(AssetBundleManifest old_manifest
            , AssetBundleManifest new_manifest)
        {
            if (old_manifest == null || new_manifest == null)
                return;
            //删除冗余
            string root_dir = EditorCommon.BUILD_PATH + "/";
            string[] new_abs = new_manifest.GetAllAssetBundles();
            HashSet<string> new_ab_table = new HashSet<string>(new_abs);
            string[] old_abs = old_manifest.GetAllAssetBundles();
            for (int i = 0; i < old_abs.Length; ++i)
            {
                if (!new_ab_table.Contains(old_abs[i]))
                {
                    //删除AssetBundle与压缩包
                    File.Delete(root_dir + old_abs[i]);
                    File.Delete(root_dir + old_abs[i] + Common.NATIVE_MANIFEST_EXTENSION);
                    File.Delete(root_dir + old_abs[i] + Compress.EXTENSION);
                    File.Delete(root_dir + old_abs[i] + Compress.EXTENSION + Common.NATIVE_MANIFEST_EXTENSION);
                }
            }
        }

        /// <summary>
        /// 检查AssetBundleManifest, 删除非本subapp AssetBundle
        /// </summary>
        public static void RemoveOtherSubappRes(ref ResourcesManifest manifest
            , string gameName)
        {
            if (manifest == null || gameName == null)
                return;
            var assetBundles = manifest.RawData.AssetBundles.Keys.ToArray();
            for (int i = 0; i < assetBundles.Length; ++i)
            {
                if (!assetBundles[i].Contains(gameName))
                {
                    manifest.RawData.AssetBundles.Remove(assetBundles[i]);
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        static bool ShowProgressBar(string title, string operating, float progress)
        {
//            DebugUtils.Log(title + " -> " + operating);
            return EditorUtility.DisplayCancelableProgressBar(title, operating, progress);
        }
    }
}