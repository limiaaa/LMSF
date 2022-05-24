using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
namespace AssetBundleBrowser
{
	public class XConfigList
	{
		/// <summary>
		/// 单例模式
		/// </summary>
		private static XConfigList mInstance;
		private static readonly object locker = new object();
		private XConfigList() 
		{

		}
		public static XConfigList GetInstance() 
		{
			if (mInstance == null)
			{
				lock (locker) 
				{
					if (mInstance == null)
					{
						mInstance = new XConfigList();
					}
				}
			}
			return mInstance;
		}

		public class ConfigVo
		{
			public string configPath;
			public string configName;
			public List<string> contentList;
		}
		private const string CONFIG_EXTENTION = ".txt";
		public static string[] ADD_TO_MAIN_LIST = { "lua", "assetbundleinfo" };
		private const string CONFIG_INFO = "abundleinfo";
		public ConfigVo CreateMainConfigVo(string targetPath, List<ConfigVo> configList)
		{

			ConfigVo mainVo = new ConfigVo();
			mainVo.contentList = new List<string>();
			mainVo.configName = "files" + CONFIG_EXTENTION;
			mainVo.configPath = targetPath + Path.DirectorySeparatorChar + "files" + CONFIG_EXTENTION;

			List<ConfigVo> singleList = new List<ConfigVo>();

			for (int i = 0; i < configList.Count; i++)
			{
				bool find = false;
				//根据ADD_TO_MAIN_LIST把对应的资源包配置再加一份到files.txt
				for (int j = 0; j < ADD_TO_MAIN_LIST.Length; j++)
				{
					if (configList[i].configName.IndexOf(ADD_TO_MAIN_LIST[j]) >= 0)
					{
						for (int k = 0; k < configList[i].contentList.Count; k++)
						{
							mainVo.contentList.Add(configList[i].configName + "/" + configList[i].contentList[k]);
						}
						find = true;
					}
				}

				if (!find)
				{
					//添加各个资源包配置文件的md5码
					mainVo.contentList.Add(CreateConfigLine(configList[i].configPath, targetPath + Path.DirectorySeparatorChar));
				}
			}
			mainVo.contentList.Add(CreateConfigLine(targetPath + Path.DirectorySeparatorChar + CONFIG_INFO, targetPath + Path.DirectorySeparatorChar));
			return mainVo;
		}

		public List<ConfigVo> CreateConfigList(string[] buildList)
		{
			List<ConfigVo> configList = new List<ConfigVo>();

			for (int i = 0; i < buildList.Length; i++)
			{
				ConfigVo vo = new ConfigVo();
				string fileName = buildList[i];
				int index = fileName.LastIndexOf(Path.DirectorySeparatorChar) + Path.DirectorySeparatorChar.ToString().Length;
				fileName = fileName.Substring(index);
				vo.configPath = buildList[i] + Path.DirectorySeparatorChar + fileName + CONFIG_EXTENTION;
				if (Directory.Exists(buildList[i]))
				{
					vo.configName = fileName;
					vo.contentList = CreateConfigContent(buildList[i] + Path.DirectorySeparatorChar);
					if (vo.contentList.Count > 0)
					{
						configList.Add(vo);
					}
				}
			}
			return configList;
		}
		public void SaveConfigList(List<ConfigVo> configList)
		{
			for (int i = 0; i < configList.Count; i++)
			{
				string newFilePath = configList[i].configPath;

				if (File.Exists(newFilePath))
				{
					File.Delete(newFilePath);
				}
				else
				{
					if (!Directory.Exists(Path.GetDirectoryName(newFilePath)))
					{
						continue;
					}
				}

				FileStream fs = new FileStream(newFilePath, FileMode.CreateNew);
				StreamWriter sw = new StreamWriter(fs);

				for (int j = 0; j < configList[i].contentList.Count; j++)
				{
					sw.WriteLine(configList[i].contentList[j]);
				}

				sw.Close();
				fs.Close();
			}
		}

		private List<string> CreateConfigContent(string targetPath)
		{
			List<string> contentList = new List<string>();

			List<string> fileList = new List<string>();

			AddNormalFileList(fileList, targetPath);

			for (int i = 0; i < fileList.Count; i++)
			{
				string line = CreateConfigLine(fileList[i], targetPath);
				if (!string.IsNullOrEmpty(line))
				{
					contentList.Add(line);
				}
			}
			return contentList;
		}

		private void AddNormalFileList(List<string> fileList, string path)
		{
			AddFileList(fileList, path, false, ".meta", ".proto", ".manifest",".txt");
		}
		private void AddFileList(List<string> fileList, string path, bool includeExtension, params string[] extensionList)
		{
			if (Directory.Exists(path))
			{
				string[] files = Directory.GetFiles(path);
				for (int i = 0; i < files.Length; i++)
				{
					string fileName = files[i];
					string ext = Path.GetExtension(fileName);
					if (extensionList != null)
					{
						bool needContinue = false;
						for (int j = 0; j < extensionList.Length; j++)
						{
							if (includeExtension)
							{
								if (ext.EndsWith(extensionList[j]))
								{
									needContinue = false;
									break;
								}
								else
								{
									needContinue = true;
									break;
								}
							}
							else
							{
								if (ext.EndsWith(extensionList[j]))
								{
									needContinue = true;
									break;
								}
							}
						}

						if (needContinue)
						{
							continue;
						}
					}

					fileList.Add(fileName.Replace('\\', '/'));
				}

				string[] dirs = Directory.GetDirectories(path);
				foreach (string dir in dirs)
				{
					AddFileList(fileList, dir, includeExtension, extensionList);
				}
			}
		}
		private string CreateConfigLine(string filePath, string targetPath)
		{
			if (filePath.EndsWith(".meta") || filePath.Contains(".DS_Store") || filePath.Contains(".manifest") ||
				filePath.Contains(".suo")) return string.Empty;

			long size = 0;
			string md5 = Util.md5file(filePath, out size);
			targetPath = targetPath.Replace('\\', '/');
			filePath = filePath.Replace('\\', '/');
			string value = filePath.Replace(targetPath, string.Empty);
			string line = value + "|" + md5 + "|" + size;
			return line;
		}

		
	}
}
