using UnityEngine;
using System;
using System.Collections;
using System.IO;
using SevenZip.Compression.LZMA;
using ICSharpCode.SharpZipLib.Zip;
using ICSharpCode.SharpZipLib.Checksums;

/// <summary>
/// 文件压缩逻辑
/// </summary>
public static class ZipUtils
    {
        /// <summary>
        /// 打包后的文件后缀名
        /// </summary>
        public const string EXTENSION = ".7z";

        /// <summary>
        /// 是否是压缩包
        /// </summary>
        public static bool IsCompressFile(string file_name)
        {
            return file_name.Contains(EXTENSION);
        }

        /// <summary>
        /// 获得文件的压缩包名
        /// </summary>
        public static string GetCompressFileName(string file_name)
        {
            return file_name + EXTENSION;
        }

        /// <summary>
        /// 获得默认文件名
        /// </summary>
        public static string GetDefaultFileName(string compress_file_name)
        {
            return compress_file_name.Replace(EXTENSION, "");
        }

        /// <summary>
        /// 压缩文件
        /// </summary>
        public static bool CompressFile(string in_file, string out_file = null)
        {
            if (out_file == null)
                out_file = GetCompressFileName(in_file);

            return CompressFileLZMA(in_file, out_file);
        }

        /// <summary>
        /// 解压文件
        /// </summary>
        public static bool DecompressFile(string in_file, string out_file = null)
        {
            if (out_file == null)
                out_file = GetDefaultFileName(in_file);

            return DecompressFileLZMA(in_file, out_file);
        }

        /// <summary>
        /// 使用LZMA算法压缩文件  
        /// </summary>
        static bool CompressFileLZMA(string inFile, string outFile)
        {
            try
            {
                if (!File.Exists(inFile))
                    return false;
                FileStream input = new FileStream(inFile, FileMode.Open);
                FileStream output = new FileStream(outFile, FileMode.OpenOrCreate);

                Encoder coder = new Encoder();
                coder.WriteCoderProperties(output);

                byte[] data = BitConverter.GetBytes(input.Length);

                output.Write(data, 0, data.Length);

                coder.Code(input, output, input.Length, -1, null);
                output.Flush();
                output.Close();
                input.Close();

                return true;
            }
            catch (System.Exception ex)
            {
                Debug.LogError(ex.Message);
            }

            return false;
        }

        /// <summary>
        /// 使用LZMA算法解压文件  
        /// </summary>
        static bool DecompressFileLZMA(string inFile, string outFile)
        {
            try
            {
                if (!File.Exists(inFile))
                    return false;

                FileStream input = new FileStream(inFile, FileMode.Open);
                FileStream output = new FileStream(outFile, FileMode.OpenOrCreate);

                byte[] properties = new byte[5];
                input.Read(properties, 0, 5);

                byte[] fileLengthBytes = new byte[8];
                input.Read(fileLengthBytes, 0, 8);
                long fileLength = BitConverter.ToInt64(fileLengthBytes, 0);

                Decoder coder = new Decoder();
                coder.SetDecoderProperties(properties);
                coder.Code(input, output, input.Length, fileLength, null);
                output.Flush();
                output.Close();
                input.Close();
            }
            catch (System.Exception ex)
            {
                Debug.LogError(ex.Message);
            }
            return false;
        }
        public static bool CompressDirectory(string dir, string outFile)
        {
            return false;
        }
    //**************************另外一套
    #region 另外一套压缩解压
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
    #endregion
}
