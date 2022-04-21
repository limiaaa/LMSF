/***************************************************************
 * Copyright 2016 By Zhang Minglin
 * Author: Zhang Minglin
 * Create: 2016/01/18
 * Note  : 资源描述数据
***************************************************************/

using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace SG.AssetBundleBrowser.AssetBundlePacker
{
    public class ResourcesManifest
    {
        public ResourcesManifestData Data;
        public ResourcesManifestRawData RawData;

        public ResourcesManifest()
        {
            Data = new ResourcesManifestData();
            RawData = new ResourcesManifestRawData();
        }

        public static ResourcesManifest Load(string fileName)
        {
            var manifest = new ResourcesManifest
            {
                RawData = ResourcesManifestRawData.Load(fileName)
            };
            if (manifest.RawData != null)
                manifest.Data = manifest.RawData.Build();
            else
            {
                return null;
            }

            return manifest;
        }

        public static ResourcesManifest LoadContent(string content)
        {
            var manifest = new ResourcesManifest
            {
                RawData = ResourcesManifestRawData.LoadContent(content)
            };
            if (manifest.RawData != null)
                manifest.Data = manifest.RawData.Build();
            else
            {
                return null;
            }

            return manifest;
        }

        public bool Save(string filePath)
        {
            try
            {
                RawData.Save(filePath);
            }
            catch (Exception e)
            {
                return false;
            }

            return true;
        }

        /// <summary>
        ///   找到一个AssetBundleDescribe
        /// </summary>
        public ResourcesManifestRawData.AssetBundle Find(string assetbundlename)
        {
            if (RawData == null)
                return null;
            if (RawData.AssetBundles == null)
                return null;
            if (RawData.AssetBundles.Count == 0)
                return null;
            if (!RawData.AssetBundles.ContainsKey(assetbundlename))
                return null;

            return RawData.AssetBundles[assetbundlename];
        }

        /// <summary>
        ///   找到一个AssetBundleDescribe
        /// </summary>
        public ResourcesManifestRawData.Scene FindScene(string scene_name)
        {
            if (RawData == null)
            {
                Debug.LogError("RawData is null");
                return null;
            }

            if (RawData.Scenes == null)
            {
                Debug.LogError("RawData.Scenes is null");
                return null;
            }

            if (RawData.Scenes.Count == 0)
            {
                Debug.LogError("RawData.Scenes.Count == 0");
                return null;
            }

            if (!RawData.Scenes.ContainsKey(scene_name))
            {
                Debug.LogError("RawData.Scenes.ContainsKey false:" + scene_name);
                return null;
            }

            return RawData.Scenes[scene_name];
        }

        public uint GetAssetBundleCrc(string assetbundleName)
        {
            if (!CheckRawData(assetbundleName)) return 0;

            return RawData.AssetBundles[assetbundleName].Crc;
        }

        private bool CheckRawData(string assetbundleName = null)
        {
            if (RawData == null || RawData.AssetBundles==null|| RawData.AssetBundles.Count<=0)
            {
                Debug.LogError("ResourcesManifest RawData is Empty");
                return false;
            }

            if (!string.IsNullOrEmpty(assetbundleName) && !RawData.AssetBundles.ContainsKey(assetbundleName))
            {
                Debug.LogError("ResourcesManifest RawData not contains: " + assetbundleName);
                return false;
            }

            return true;
        }

        /// <summary>
        ///   获得包含某个资源的所有AssetBundle
        /// </summary>
        public string[] GetAllAssetBundleName(string asset)
        {
            if (Data.AssetTable == null)
            {
                Debug.LogError("ResourceManifest AssetTable is null, can not load:" + asset);
                return null;
            }

            if (!Data.AssetTable.ContainsKey(asset))
            {
//                Debug.LogWarning("ResourceManifest AssetTable does NOT contain this asset, can not load:" + asset);
                return null;
            }

            // DebugUtils.Log("ResourcesManifestData GetAllAssetBundleName: " + AssetTable[asset].Count);
            return Data.AssetTable[asset].ToArray();
        }

        /// <summary>
        ///   获得场景的AssetBundleName
        /// </summary>
        public string GetAssetBundleNameByScene(string scene_path)
        {
            if (Data.SceneTable == null)
            {
                Debug.LogError("Data.SceneTable == null");
                return null;
            }

            if (!Data.SceneTable.ContainsKey(scene_path))
            {
                Debug.LogError("Data.SceneTable.ContainsKey false:" + scene_path);
                return null;
            }

            return Data.SceneTable[scene_path];
        }

        /// <summary>
        ///   获得场景的AssetBundleName
        /// </summary>
        public string GetAssetBundleNameBySceneLevelName(string scene_name)
        {
            ResourcesManifestRawData.Scene desc = FindScene(scene_name);
            if (desc == null)
                return null;
            return GetAssetBundleNameByScene(desc.ScenePath);
        }

        /// <summary>
        ///   判断一个AssetBundle是否常驻内存资源
        /// </summary>
        public bool IsPermanent(string assetbundlename)
        {
            if (RawData.AssetBundles == null)
                return false;
            // if (Data.AssetBundles.ContainsKey(assetbundlename))
            //     return Data.AssetBundles[assetbundlename].IsPermanent;

            return false;
        }

        /// <summary>
        ///   获得AssetBundle的大小
        /// </summary>
        public long GetAssetBundleSize(string assetbunlename)
        {
            ResourcesManifestRawData.AssetBundle desc = Find(assetbunlename);
            if (desc != null)
                return desc.Size;

            return 0;
        }

        /// <summary>
        ///   获得AssetBundle的大小
        /// </summary>
        public long GetAssetBundleCompressSize(string assetbunlename)
        {
            ResourcesManifestRawData.AssetBundle desc = Find(assetbunlename);
            if (desc != null)
                return desc.CompressSize;

            return 0;
        }

        public string[] GetSameLoadOrderAssetBundles(int loadOrder, bool isGeneral)
        {
            var assetBundles = new List<string>();
            if (CheckRawData())
            {
                foreach (var assetBundle in RawData.AssetBundles.Values)
                {
                    if (isGeneral && assetBundle.IsGeneral)
                    {
                        assetBundles.Add(assetBundle.AssetBundleName);
                    }
                    if (assetBundle.LoadOrder == loadOrder && assetBundle.IsGeneral == isGeneral)
                    {
                        assetBundles.Add(assetBundle.AssetBundleName);
                    }
                }
            }


            return assetBundles.ToArray();
        }
    }

    /// <summary>
    /// 资源清单
    /// </summary>
    public class ResourcesManifestRawData
    {
        /// <summary>
        ///   场景描述信息
        /// </summary>
        public class Scene
        {
            public string SceneLevelName; // 场景名称
            public string ScenePath; // 场景路径
            public string SceneConfigPath; // 场景配置文件路径
        }

        /// <summary>
        ///   AssetBundle描述信息
        /// </summary>
        public class AssetBundle
        {
            public string AssetBundleName; // AssetBundleName

            /// <summary>
            /// 资源列表
            /// </summary>
            public List<string> Assets = new List<string>(); // 

            /// <summary>
            /// 场景列表
            /// </summary>
            public List<string> Scenes = new List<string>(); // 

            /// <summary>
            /// AssetBundle大小
            /// </summary>
            public long Size;

            /// <summary>
            /// 下载/复制到App可用状态的顺序
            /// </summary>
            public int LoadOrder;

            /// <summary>
            /// 
            /// </summary>
            public bool IsGeneral;

            /// <summary>
            /// 压缩包大小 
            /// </summary>
            public long CompressSize; // 压缩包大小 

            /// <summary>
            /// 是否压缩
            /// </summary>
            public bool IsCompress = false;

            public uint Crc;

            // public bool IsNative = false;                       // 是否打包到安装包中（原始资源）
            // public bool IsPermanent = false;                    // 是否常驻内存
        }

        public uint Version;

        //Key：AssetBundleName Value: Describe
        public Dictionary<string, AssetBundle> AssetBundles = new Dictionary<string, AssetBundle>();

        //Key：SceneLevelName Value: SceneDescribe
        public Dictionary<string, Scene> Scenes = new Dictionary<string, Scene>();

        /// <summary>
        ///   组建数据，建立资源查询表
        /// </summary>
        public ResourcesManifestData Build()
        {
            var data = new ResourcesManifestData
            {
                AssetTable = new Dictionary<string, List<string>>(),
                SceneTable = new Dictionary<string, string>()
            };
            if (AssetBundles != null)
            {
                var itr = AssetBundles.Values.GetEnumerator();
                while (itr.MoveNext())
                {
                    List<string> list = itr.Current.Assets;
                    for (int i = 0; i < list.Count; ++i)
                    {
                        if (!data.AssetTable.ContainsKey(list[i]))
                        {
                            data.AssetTable.Add(list[i], new List<string>());
                        }

                        data.AssetTable[list[i]].Add(itr.Current.AssetBundleName);
                    }

                    List<string> scenes = itr.Current.Scenes;
                    for (int i = 0; i < scenes.Count; ++i)
                    {
                        if (!data.SceneTable.ContainsKey(scenes[i]))
                            data.SceneTable.Add(scenes[i], itr.Current.AssetBundleName);
                    }
                }

                itr.Dispose();
            }

            return data;
        }

        /// <summary>
        /// 
        /// </summary>
        public static ResourcesManifestRawData Load(string file_name)
        {
            var result = SimpleJsonReader.ReadFromFile<ResourcesManifestRawData>(file_name);

            return result;
        }

        public static ResourcesManifestRawData LoadContent(string content)
        {
            content = content.Trim();
            var result = SimpleJsonReader.ReadFromString<ResourcesManifestRawData>(content);
            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        public bool Save(string file_name)
        {
            return SimpleJsonWriter.WriteToFile(this, file_name);
        }
    }

    public class ResourcesManifestData
    {
        /// <summary>
        ///   资源查询表
        ///   Key： Asset
        ///   Value： AssetBundleName's list
        /// </summary>
        public Dictionary<string, List<string>> AssetTable;

        /// <summary>
        ///   场景查询表(场景强制打包为一个AssetBundle)
        ///   Key： SceneLevelName
        ///   Value： AssetBundleName
        /// </summary>
        public Dictionary<string, string> SceneTable;

        /// <summary>
        ///   
        /// </summary>
        public ResourcesManifestData()
        {
        }
    }
}