using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditor.U2D;
using UnityEngine;
using UnityEngine.U2D;
using Object = UnityEngine.Object;

public class TextureSetting : AssetPostprocessor
{
    static List<TextureImporterType> TextureTypeList = new List<TextureImporterType>();
    static string saveDir = Application.dataPath + "/BatchModifyTexture";
    static DirectoryInfo saveDirInfo;
    static DirectoryInfo oldDirInfo;
    [MenuItem("Tool/ChangeTextureAndPng")]
    static private void ChangeTextureAndPng()
    {
        //获取Project视图中的选中目录下的所有图片
        var objects = Selection.GetFiltered(typeof(Object), SelectionMode.Unfiltered);
        string dir = Application.dataPath.Substring(0, Application.dataPath.LastIndexOf("/")) + "/" + AssetDatabase.GetAssetPath(objects[0]);
        string[] paths = Directory.GetFiles(dir, "*.png", SearchOption.AllDirectories);
        //创建存放目录
        if (!Directory.Exists(saveDir))
        {
            Directory.CreateDirectory(saveDir);
        }

        string[] FloderPaths = Directory.GetDirectories(dir, "*", SearchOption.AllDirectories);
        //创建文件夹
        saveDirInfo = new DirectoryInfo(saveDir);
        oldDirInfo = new DirectoryInfo(dir);
        CopyOnlyDirectory(oldDirInfo, saveDirInfo);
        AssetDatabase.Refresh();
        DisposeTexture(paths);


    }
    public static void CopyAll(DirectoryInfo source, DirectoryInfo target)
    {
        if (source.FullName.ToLower() == target.FullName.ToLower())
        {
            return;
        }
        if (Directory.Exists(target.FullName) == false)
        {
            Directory.CreateDirectory(target.FullName);
        }
        foreach (DirectoryInfo diSourceSubDir in source.GetDirectories())
        {
            DirectoryInfo nextTargetSubDir = target.CreateSubdirectory(diSourceSubDir.Name);
            CopyAll(diSourceSubDir, nextTargetSubDir);
        }
        foreach (FileInfo fi in source.GetFiles())
        {
            fi.CopyTo(Path.Combine(target.ToString(), fi.Name));
        }
    }
    public static void CopyOnlyDirectory(DirectoryInfo source, DirectoryInfo target)
    {
        if (source.FullName.ToLower() == target.FullName.ToLower())
        {
            return;
        }
        if (Directory.Exists(target.FullName) == false)
        {
            Directory.CreateDirectory(target.FullName);
        }
        foreach (DirectoryInfo diSourceSubDir in source.GetDirectories())
        {
            DirectoryInfo nextTargetSubDir = target.CreateSubdirectory(diSourceSubDir.Name);
            CopyOnlyDirectory(diSourceSubDir, nextTargetSubDir);
        }
    }

    public static void DisposeTexture(string[]paths)
    {
        //图片处理
        for (int i = 0; i < paths.Length; i++)
        {
            string path = paths[i];
            string assetPath = path.Substring(path.IndexOf("Assets/"));
            string name = assetPath.Substring(assetPath.LastIndexOf("\\") + 1);
            Debug.Log(assetPath + "___" + name);
            //设置成可读
            TextureImporter textureImporter = AssetImporter.GetAtPath(assetPath) as TextureImporter;
            TextureTypeList.Add(textureImporter.textureType);
            textureImporter.isReadable = true;
            AssetDatabase.ImportAsset(assetPath);
            //创建新Texture，修改图集;
            Texture2D tex = AssetDatabase.LoadAssetAtPath<Texture2D>(assetPath);
            Color[] colors = tex.GetPixels(0, 0, tex.width, tex.height);
            Texture2D texTemp = new Texture2D(tex.width, tex.height);
            Color[] colorsTemp = new Color[colors.Length];
            for (int j = 0; j < colors.Length; j++)
            {
                colorsTemp[j] = colors[j] * 0.999f;
            }
            texTemp.SetPixels(0, 0, tex.width, tex.height, colorsTemp);
            texTemp.Apply();
            byte[] bytes = texTemp.EncodeToPNG();
            //保存至新文件夹
            DirectoryInfo[] dirs = saveDirInfo.GetDirectories();
            DirectoryInfo dir = new DirectoryInfo(path);
            string parentPath = "";
            foreach(var item in dirs)
            {
                if (dir.Parent.Name == item.Name)
                {
                    File.WriteAllBytes(item.FullName + "/" + name, bytes);
                    break;
                }
            }
            if(dir.Parent.Name== "TestImgFloder")
            {
                File.WriteAllBytes(saveDir + "/" + name, bytes);
            }

        }
        AssetDatabase.Refresh();
        //恢复图片状态
        string[] pathsBatched = Directory.GetFiles(saveDir, "*.png", SearchOption.AllDirectories);
        for (int i = 0; i < pathsBatched.Length; i++)
        {
            string path = pathsBatched[i];
            string assetPath = path.Substring(path.IndexOf("Assets/"));
            //设置成可读
            TextureImporter textureImporter = AssetImporter.GetAtPath(assetPath) as TextureImporter;
            textureImporter.textureType = TextureTypeList[i];
            textureImporter.isReadable = true;
            AssetDatabase.ImportAsset(assetPath);
        }
        AssetDatabase.Refresh();
    }
    [MenuItem("Tool/Test")]
    static void Test()
    {
        //获取Project视图中的选中目录下的所有图片
        var objects = Selection.GetFiltered(typeof(Object), SelectionMode.Unfiltered);
        string dir = Application.dataPath.Substring(0, Application.dataPath.LastIndexOf("/")) + "/" + AssetDatabase.GetAssetPath(objects[0]);
        DirectoryInfo d = new DirectoryInfo(dir);
        foreach(var item in d.GetDirectories("*",SearchOption.AllDirectories))
        {
            Debug.Log(item.Name);
        }
    }
}