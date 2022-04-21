using System.Collections.Generic;
using System.IO;
using System.Linq;
using SG.ContainerCoreMain;
using UnityEngine;

namespace SG.AssetBundleBrowser
{
    public class AssetBundleManifestAnalyzer
    {
        private AssetBundleManifest mManifest;
        private Tree tree;

        public AssetBundleManifestAnalyzer(AssetBundleManifest manifest)
        {
            mManifest = manifest;
        }

        public void Analyze()
        {
            var assetBundles = mManifest.GetAllAssetBundles();

            tree = new Tree(assetBundles);
            tree.CalculateGeneration(mManifest);
            tree.LogTree();
        }


        public int GetGeneration(string assetbundleName, string parentPath)
        {
            var filePath = parentPath + assetbundleName;
          
            if (File.Exists(filePath))
            {
                var ab = AssetBundle.LoadFromFile(filePath);

                if (ab.GetAllAssetNames().Length <= 0 && ab.GetAllScenePaths().Length <= 0)
                {
                    Debug.LogError(assetbundleName + " is Empty, please check!");
                    ab.Unload(true);
                    return -1;
                }

                ab.Unload(true);
            }
            else
            {
                Debug.LogError(assetbundleName + " file is NOT existed");
                return -1;
            }

            var node = tree.GetNode(assetbundleName);
            if (node == null)
            {
                return -1;
            }

            return node.generation;
        }


        public class TreeNode
        {
            public string assetBundleName;
            public int generation;
            public List<TreeNode> children;
            public List<TreeNode> parent;
        }

        public class Tree
        {
            public Dictionary<string, TreeNode> nodes;

            public TreeNode GetNode(string assetName)
            {
                if (nodes.ContainsKey(assetName))
                {
                    return nodes[assetName];
                }

                return null;
            }

            public Tree(string[] assetBundles)
            {
                nodes = new Dictionary<string, TreeNode>();
                foreach (var assetBundle in assetBundles)
                {
                    nodes[assetBundle] = new TreeNode()
                    {
                        assetBundleName = assetBundle,
                        children = new List<TreeNode>(),
                        parent = new List<TreeNode>(),
                        generation = -1
                    };
                }
            }

            public void CalculateGeneration(AssetBundleManifest manifest)
            {
                foreach (TreeNode treeNode in nodes.Values)
                {
                    var dependencies = manifest.GetAllDependencies(treeNode.assetBundleName);
                    if (dependencies.IsEmpty())
                    {
                        treeNode.generation = 0;
                    }
                    else
                    {
                        foreach (var dependency in dependencies)
                        {
                            if (nodes.ContainsKey(dependency))
                            {
                                var parent = nodes[dependency];
                                if (parent.generation == -1)
                                {
                                    continue;
                                }

                                treeNode.generation = parent.generation + 1;
                                if (!parent.children.Contains(treeNode))
                                {
                                    parent.children.Add(treeNode);
                                }

                                if (!treeNode.parent.Contains(parent))
                                {
                                    treeNode.parent.Add(parent);
                                }
                            }
                            else
                            {
                                Debug.LogError("---> " + dependency);
                            }
                        }
                    }
                }

                foreach (var nodesValue in nodes.Values)
                {
                    if (nodesValue.generation == -1)
                    {
                        CalculateGeneration(manifest);
                        break;
                    }
                }
            }


            public void LogTree()
            {
            }
        }
    }
}