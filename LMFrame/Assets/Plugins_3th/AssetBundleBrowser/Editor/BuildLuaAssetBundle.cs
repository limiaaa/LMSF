using UnityEngine;
using UnityEditor;
using System.IO;
using System.Collections.Generic;
using System;
namespace AssetBundleBrowser
{
	public class BuildLuaAssetBundle
	{
		static string lua = "lua";
		static string Extension = "*.bytes";
		//lua路径文件路径
		static string luatxtPath = Application.dataPath + "/LuaPath/LuaPath.txt";
		public static void BuildLuaResource(string outPath)
		{
			BuildLuaAssetResource(outPath,EditorUserBuildSettings.activeBuildTarget);
		}
		public static void BuildLuaAssetResource(string outPath, BuildTarget target)
		{
			BuildLuaFiles(outPath,target);
		}
		private static void InitTempFolder()
		{
			string streamDir = TempDir;
			if (Directory.Exists(streamDir))
			{
				Directory.Delete(streamDir, true);
			}
			Directory.CreateDirectory(streamDir);
		}
		private static string TempDir
		{
			get
			{
				string luaDir = Application.dataPath + "/" + AppConst.TempDir;
				return luaDir;
			}
		}
		private const string PROGRESS_TITLE = "打包进度:";
		private static void OnProgress(string title, string content, float progress)
		{
			EditorUtility.DisplayProgressBar(title + "(" + (progress * 100f).ToString("F2") + "%)", content, progress);
		}
		public static void BuildLuaFiles(string outPath, BuildTarget target)
		{
			InitTempFolder();
			string targetPath = outPath + "/";
			List<BuildVo> buildList = new List<BuildVo>();
			OnProgress(PROGRESS_TITLE, "准备打包", 0f);
			//获取资源包列表
			AddBundleList(buildList, targetPath);
			AssetDatabase.Refresh();
			//打包所有AssetBundle
			Build(buildList, target);
			OnProgress(PROGRESS_TITLE, "生成配置文件", Convert.ToSingle(buildList.Count) / Convert.ToSingle(buildList.Count + 1));

			AssetDatabase.Refresh();

			buildList.Clear();
			AssetDatabase.Refresh();
			DeleteLuaTempDir();

			EditorUtility.ClearProgressBar();

			//EditorUtility.DisplayDialog(string.Empty, "打包完成!", "确定");
		}
		private static void Build(List<BuildVo> buildList, BuildTarget target)
		{
			BuildAssetBundleOptions options = BuildAssetBundleOptions.DeterministicAssetBundle;
			for (int i = 0; i < buildList.Count; i++)
			{
				OnProgress(PROGRESS_TITLE, buildList[i].progressName, Convert.ToSingle(i) / Convert.ToSingle(buildList.Count + 1));

				if (!Directory.Exists(buildList[i].targetPath))
				{
					Directory.CreateDirectory(buildList[i].targetPath);
				}
				else
				{
					Directory.Delete(buildList[i].targetPath, true);
					Directory.CreateDirectory(buildList[i].targetPath);
				}
				//if (AppConst.EncryptABBol)
				//{
				//	var buildManifest = BuildPipeline.BuildAssetBundles(buildList[i].targetPath, buildList[i].assetBundleBuildList.ToArray(), options, target);
				//	foreach (var assetBundleName in buildManifest.GetAllAssetBundles())
				//	{
				//		StreamEncryption.EcryptAssetBundle(Path.Combine(buildList[i].targetPath, assetBundleName));					
				//	}
				//}
				//else
				//{
					BuildPipeline.BuildAssetBundles(buildList[i].targetPath, buildList[i].assetBundleBuildList.ToArray(), options, target);
				//}			

			}
		}
		private static void DeleteLuaTempDir()
		{
			string streamDir = TempDir;
			if (Directory.Exists(streamDir))
			{
				Directory.Delete(streamDir, true);
			}
		}
		private class BuildVo
		{
			public string progressName;
			public string targetPath;
			public string targetName;
			public List<AssetBundleBuild> assetBundleBuildList;
		}
		static void AddBundleList(List<BuildVo> buildList, string targetPath)
		{
			string filePath = luatxtPath;
			string subTargetPath = targetPath + lua + "/";
			if (!Directory.Exists(subTargetPath)) 
				Directory.CreateDirectory(subTargetPath);
			AddPackageList(buildList, subTargetPath, lua, filePath);
		}
		private static void AddPackageList(List<BuildVo> buildList, string targetPath, string fileName, string packageConfigPath)
		{
			string tmpFileName = "";
			tmpFileName = packageConfigPath.Replace('\\', '/');

			List<AssetBundleBuild> abBuildList = null;
			if (File.Exists(tmpFileName))
			{
				abBuildList = new List<AssetBundleBuild>();

				string content = File.ReadAllText(tmpFileName);

				if (!string.IsNullOrEmpty(content))
				{
					string[] contents = content.Split(new char[] { '\r','\n' }, System.StringSplitOptions.RemoveEmptyEntries);
					for (int i = 0; i < contents.Length; i++)
					{
						if (!string.IsNullOrEmpty(contents[i]))
						{
							CopyFiles(contents[i], TempDir);
						}
					}
					AddEveryFolderBuildList(abBuildList, fileName, TempDir, Application.dataPath);
				}
			}
			BuildVo vo = new BuildVo();
			vo.targetPath = targetPath;
			vo.progressName = "开始打包 " + fileName;
			vo.assetBundleBuildList = abBuildList;
			vo.targetName = fileName;
			buildList.Add(vo);
		}
		public static void CopyFiles(string sourceDir, string destDir)
		{
			if (!Directory.Exists(sourceDir))
			{
				return;
			}

			string[] files = Directory.GetFiles(sourceDir, "*.lua", SearchOption.AllDirectories);
			int len = sourceDir.Length;

			if (sourceDir[len - 1] == '/' || sourceDir[len - 1] == '\\')
			{
				--len;
			}

			for (int i = 0; i < files.Length; i++)
			{
				string str = files[i].Remove(0, len);
				string dest = destDir + "/" + str;
				dest += ".bytes";
				string dir = Path.GetDirectoryName(dest);
				Directory.CreateDirectory(dir);
				File.Copy(files[i], dest, true);
			}
		}
		private static void AddEveryFolderBuildList(List<AssetBundleBuild> abBuildList, string configBundleName, string tempDir, string dataPath)
		{
			string[] dirs = Directory.GetDirectories(tempDir, "*", SearchOption.AllDirectories);
			for (int i = 0; i < dirs.Length; i++)
			{
				string bundleName = dirs[i].Replace(tempDir, string.Empty);
				bundleName = bundleName.Replace('\\', '_').Replace('/', '_');
				bundleName = bundleName.ToLower() + AppConst.ExtName;

				string path = "Assets" + dirs[i].Replace(dataPath, string.Empty);
				if (!string.IsNullOrEmpty(Extension))
				{
					AddBuildList(abBuildList, bundleName, Extension, path);
				}
			}

			string path1 = "Assets" + tempDir.Replace(dataPath, string.Empty);
			if (!string.IsNullOrEmpty(Extension))
			{
				AddBuildList(abBuildList, configBundleName + AppConst.ExtName, Extension, path1);
			}
		}
		private static void AddBuildList(List<AssetBundleBuild> abBuildList, string bundleName, string pattern, string fromFolderPath)
		{
			string extension = Path.GetExtension(fromFolderPath);
			if (!string.IsNullOrEmpty(extension))
			{
				fromFolderPath = Path.GetDirectoryName(fromFolderPath);
			}

			string[] files = Directory.GetFiles(fromFolderPath, Extension);
			if (files.Length == 0) return;

			for (int i = 0; i < files.Length; i++)
			{
				files[i] = files[i].Replace('\\', '/');
			}

			AssetBundleBuild build = new AssetBundleBuild();
			build.assetBundleName = bundleName;
			build.assetNames = files;
			abBuildList.Add(build);
		}
	}
}
