using AssetBundleBrowser;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class AssetBundleEncrypt : Editor
{
    // // [MenuItem("AssetBuild/Encrypt AssetBundles")]
    // public static void EncryptAssetBundles()
    // {
    //     string sourceuri = string.Empty;
    //     sourceuri = Application.dataPath + "/AssetBundles/abundleinfo";
    //     string desturi = string.Empty;
    //     desturi = Application.dataPath + "/AssetBundles/encryptabundleinfo";
    //     string sourceDirName = sourceuri.Replace("Assets/", string.Empty);
    //     string destDirName = desturi.Replace("Assets/", string.Empty);
    //     DirectoryCopy(sourceDirName, destDirName);
    //     EncryptEachFileInThisFolder(destDirName);
    //     CreateAssetBundleConfig.CreateConfig(destDirName);
    //     EditorUtility.DisplayDialog(string.Empty, "加密完成!", "确定");
    // }

    public static void IncrementalEncryptEachFileInThisFolder(string folderPath,
        Dictionary<string, string> backupBundleMD5)
    {
        _backupBundleMD5 = backupBundleMD5;
        EncryptEachFileInThisFolder(folderPath);
    }

    public static void EncryptEachFileInThisFolder(string folderPath)
    {
        DirectoryInfo directoryInfo = new DirectoryInfo(folderPath);
        FileSystemInfo[] fileSystemInfo = directoryInfo.GetFileSystemInfos();
        int fileSystemInfoLength = fileSystemInfo.Length;
        for (int i = 0; i < fileSystemInfoLength; i++)
        {
            if (fileSystemInfo[i] is DirectoryInfo)
            {
                EncryptEachFileInThisFolder(fileSystemInfo[i].FullName);
            }
            else
            {
                EncryptFile(fileSystemInfo[i].FullName);
            }
        }
    }

    private static Dictionary<string, string> _backupBundleMD5 = new Dictionary<string, string>();

    private static void EncryptFile(string filePath)
    {
        if (Path.GetExtension(filePath) == ".lo")
        {
            var fileName = filePath.Split('\\').Last();
            if (!_backupBundleMD5.ContainsKey(fileName))
            {
                Debug.LogWarning($"new build===>{fileName}");
                StreamEncryption.EncryptAssetBundle(filePath);
                return;
            }

            var newMD5 = Util.md5file(filePath);
            var oldMD5 = _backupBundleMD5[fileName];
            if (newMD5 == oldMD5) return;

            Debug.LogWarning($"re_encrypt===>{filePath} oldMD5==>{oldMD5} newMD5===>{newMD5}");
            StreamEncryption.EncryptAssetBundle(filePath);
        }
    }

    private static void DirectoryCopy(string sourceDirName, string destDirName)
    {
        // If the destination directory doesn't exist, create it.
        if (!Directory.Exists(destDirName))
        {
            Directory.CreateDirectory(destDirName);
        }

        foreach (string folderPath in Directory.GetDirectories(sourceDirName, "*", SearchOption.AllDirectories))
        {
            if (!Directory.Exists(folderPath.Replace(sourceDirName, destDirName)))
                Directory.CreateDirectory(folderPath.Replace(sourceDirName, destDirName));
        }

        foreach (string filePath in Directory.GetFiles(sourceDirName, "*.*", SearchOption.AllDirectories))
        {
            var fileDirName = Path.GetDirectoryName(filePath).Replace("\\", "/");
            var fileName = Path.GetFileName(filePath);
            string newFilePath = Path.Combine(fileDirName.Replace(sourceDirName, destDirName), fileName);
            File.Copy(filePath, newFilePath, true);
        }
    }
}