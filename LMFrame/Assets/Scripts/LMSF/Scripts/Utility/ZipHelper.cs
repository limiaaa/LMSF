using ICSharpCode.SharpZipLib.Checksums;
using ICSharpCode.SharpZipLib.Zip;
using System;
using System.IO;
using UnityEngine;

public class ZipHelper
{
    static string rootPath = "";

    /// <summary>
    /// 压缩文件
    /// </summary>
    /// <param name="sourceFilePath"></param>
    /// <param name="destinationZipFilePath"></param>
    public static void CreateZip(string sourceFilePath, string destinationZipFilePath)
    {
        // DebugUtils.Log(sourceFilePath);
        if (sourceFilePath[sourceFilePath.Length - 1] != System.IO.Path.DirectorySeparatorChar)
            sourceFilePath += System.IO.Path.DirectorySeparatorChar;
        rootPath = sourceFilePath;
        ZipOutputStream zipStream = new ZipOutputStream(File.Create(destinationZipFilePath));
        zipStream.SetLevel(9); // 压缩级别 0-9
        CreateZipFiles(sourceFilePath, zipStream);
        zipStream.Finish();
        zipStream.Close();
    }


    public static void UnzipFile(string srcDirPath, string destDirPath)
    {
        ZipInputStream zipIn = null;
        FileStream streamWriter = null;

        try
        {
            Directory.CreateDirectory(Path.GetDirectoryName(destDirPath));

            zipIn = new ZipInputStream(File.OpenRead(srcDirPath));
            ZipEntry entry;

            while ((entry = zipIn.GetNextEntry()) != null)
            {
                string dirPath = Path.GetDirectoryName(destDirPath + entry.Name);

                if (!Directory.Exists(dirPath))
                {
                    Directory.CreateDirectory(dirPath);
                }

                if (!entry.IsDirectory)
                {
                    streamWriter = File.Create(destDirPath + entry.Name);
                    int size = 2048;
                    byte[] buffer = new byte[size];

                    while ((size = zipIn.Read(buffer, 0, buffer.Length)) > 0)
                    {
                        streamWriter.Write(buffer, 0, size);
                    }
                }

                streamWriter.Close();
            }
        }
        catch (System.Threading.ThreadAbortException lException)
        {
            // do nothing
        }
        catch (Exception ex)
        {
            throw (ex);
        }
        finally
        {
            if (zipIn != null)
            {
                zipIn.Close();
            }

            if (streamWriter != null)
            {
                streamWriter.Close();
            }
        }
    }

    /// <summary>
    /// 递归压缩文件
    /// </summary>
    /// <param name="sourceFilePath">待压缩的文件或文件夹路径</param>
    /// <param name="zipStream">打包结果的zip文件路径（类似 D:\WorkSpace\a.zip）,全路径包括文件名和.zip扩展名</param>
    /// <param name="staticFile"></param>
    private static void CreateZipFiles(string sourceFilePath, ZipOutputStream zipStream)
    {
        Crc32 crc = new Crc32();
        string[] filesArray = Directory.GetFileSystemEntries(sourceFilePath);
        foreach (string file in filesArray)
        {
            if (Directory.Exists(file)) //如果当前是文件夹，递归
            {
                CreateZipFiles(file, zipStream);
            }
            else //如果是文件，开始压缩
            {
                FileStream fileStream = File.OpenRead(file);
                byte[] buffer = new byte[fileStream.Length];
                fileStream.Read(buffer, 0, buffer.Length);
                string tempFile = file.Replace(rootPath, "").Replace('\\', '/');
                ZipEntry entry = new ZipEntry(tempFile);
                entry.DateTime = DateTime.Now;
                entry.Size = fileStream.Length;
                fileStream.Close();
                crc.Reset();
                crc.Update(buffer);
                entry.Crc = crc.Value;
                zipStream.PutNextEntry(entry);
                zipStream.Write(buffer, 0, buffer.Length);
            }
        }
    }
}