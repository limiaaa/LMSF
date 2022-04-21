using System;
using System.Text;
using UnityEngine;
using System.Security.Cryptography;
using System.IO;
using System.Threading;
using System.Collections.Generic;

namespace SG
{
    public sealed class CryptUtils
    {
        #region 校验值

        #region MD5

        public static string Md5FromFile(string filePath)
        {
            return Md5(File.ReadAllBytes(filePath));
        }

        public static string Md5(string text)
        {
            return Md5(Encoding.Default.GetBytes(text));
        }

        public static string Md5(string text, Encoding encoding /*Encoding.Default*/)
        {
            return Md5(encoding.GetBytes(text));
        }

        public static string Md5(byte[] bytes)
        {
            byte[] hash;
            hash = (new MD5CryptoServiceProvider()).ComputeHash(bytes);
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < hash.Length; i++)
                sb.Append(hash[i].ToString("X2"));
            return sb.ToString();
        }

        public static void Md5FromFileAsyn(string filePath, ED_1Param<string> onfinished)
        {
            Md5Asyn(File.ReadAllBytes(filePath), onfinished);
        }

        public static void Md5Asyn(byte[] bytes, ED_1Param<string> onfinished)
        {
            MD5Thread.Create(new Thread(OnMd5Process), bytes, onfinished);
        }

        #region MD5值计算线程
        [ExecuteInEditMode]
        public class MD5Thread : MonoBehaviour
        {
            Thread            thread;
            ED_1Param<string> onFinished;
            byte[]            bytes = null;
            public static void Create(Thread thread, byte[] bytes, ED_1Param<string> onfinished)
            {
                MD5Thread md5t;
                md5t            = (new GameObject("__MD5Thread__")).AddComponent<MD5Thread>();
                md5t.thread     = thread;
                md5t.onFinished = onfinished;
                md5t.bytes      = bytes;
            }

            void Start()
            {
                thread.Start(bytes);
            }

            void Update()
            {
                if (!thread.IsAlive)
                {
                    if (onFinished != null)
                        onFinished(lastMd5Value);
                    DestroyImmediate(gameObject);
                }
            }
        }
        static string lastMd5Value = "";
        static void OnMd5Process(object obj)
        {
            byte[] bytes = (byte[])obj;
            lastMd5Value = Md5(bytes);
        }
        #endregion

        #endregion

        #region CRC-32

        #region CRC Table.

        static uint[] crcTable = 
        {
             0x0, 0x77073096, 0xee0e612c, 0x990951ba, 0x76dc419, 0x706af48f, 0xe963a535, 0x9e6495a3,
             0xedb8832, 0x79dcb8a4, 0xe0d5e91e, 0x97d2d988, 0x9b64c2b, 0x7eb17cbd, 0xe7b82d07, 0x90bf1d91,
             0x1db71064, 0x6ab020f2, 0xf3b97148, 0x84be41de, 0x1adad47d, 0x6ddde4eb, 0xf4d4b551, 0x83d385c7,
             0x136c9856, 0x646ba8c0, 0xfd62f97a, 0x8a65c9ec, 0x14015c4f, 0x63066cd9, 0xfa0f3d63, 0x8d080df5,
             0x3b6e20c8, 0x4c69105e, 0xd56041e4, 0xa2677172, 0x3c03e4d1, 0x4b04d447, 0xd20d85fd, 0xa50ab56b,
             0x35b5a8fa, 0x42b2986c, 0xdbbbc9d6, 0xacbcf940, 0x32d86ce3, 0x45df5c75, 0xdcd60dcf, 0xabd13d59,
             0x26d930ac, 0x51de003a, 0xc8d75180, 0xbfd06116, 0x21b4f4b5, 0x56b3c423, 0xcfba9599, 0xb8bda50f,
             0x2802b89e, 0x5f058808, 0xc60cd9b2, 0xb10be924, 0x2f6f7c87, 0x58684c11, 0xc1611dab, 0xb6662d3d,
             0x76dc4190, 0x1db7106, 0x98d220bc, 0xefd5102a, 0x71b18589, 0x6b6b51f, 0x9fbfe4a5, 0xe8b8d433,
             0x7807c9a2, 0xf00f934, 0x9609a88e, 0xe10e9818, 0x7f6a0dbb, 0x86d3d2d, 0x91646c97, 0xe6635c01,
             0x6b6b51f4, 0x1c6c6162, 0x856530d8, 0xf262004e, 0x6c0695ed, 0x1b01a57b, 0x8208f4c1, 0xf50fc457,
             0x65b0d9c6, 0x12b7e950, 0x8bbeb8ea, 0xfcb9887c, 0x62dd1ddf, 0x15da2d49, 0x8cd37cf3, 0xfbd44c65,
             0x4db26158, 0x3ab551ce, 0xa3bc0074, 0xd4bb30e2, 0x4adfa541, 0x3dd895d7, 0xa4d1c46d, 0xd3d6f4fb,
             0x4369e96a, 0x346ed9fc, 0xad678846, 0xda60b8d0, 0x44042d73, 0x33031de5, 0xaa0a4c5f, 0xdd0d7cc9,
             0x5005713c, 0x270241aa, 0xbe0b1010, 0xc90c2086, 0x5768b525, 0x206f85b3, 0xb966d409, 0xce61e49f,
             0x5edef90e, 0x29d9c998, 0xb0d09822, 0xc7d7a8b4, 0x59b33d17, 0x2eb40d81, 0xb7bd5c3b, 0xc0ba6cad,
             0xedb88320, 0x9abfb3b6, 0x3b6e20c, 0x74b1d29a, 0xead54739, 0x9dd277af, 0x4db2615, 0x73dc1683,
             0xe3630b12, 0x94643b84, 0xd6d6a3e, 0x7a6a5aa8, 0xe40ecf0b, 0x9309ff9d, 0xa00ae27, 0x7d079eb1,
             0xf00f9344, 0x8708a3d2, 0x1e01f268, 0x6906c2fe, 0xf762575d, 0x806567cb, 0x196c3671, 0x6e6b06e7,
             0xfed41b76, 0x89d32be0, 0x10da7a5a, 0x67dd4acc, 0xf9b9df6f, 0x8ebeeff9, 0x17b7be43, 0x60b08ed5,
             0xd6d6a3e8, 0xa1d1937e, 0x38d8c2c4, 0x4fdff252, 0xd1bb67f1, 0xa6bc5767, 0x3fb506dd, 0x48b2364b,
             0xd80d2bda, 0xaf0a1b4c, 0x36034af6, 0x41047a60, 0xdf60efc3, 0xa867df55, 0x316e8eef, 0x4669be79,
             0xcb61b38c, 0xbc66831a, 0x256fd2a0, 0x5268e236, 0xcc0c7795, 0xbb0b4703, 0x220216b9, 0x5505262f,
             0xc5ba3bbe, 0xb2bd0b28, 0x2bb45a92, 0x5cb36a04, 0xc2d7ffa7, 0xb5d0cf31, 0x2cd99e8b, 0x5bdeae1d,
             0x9b64c2b0, 0xec63f226, 0x756aa39c, 0x26d930a, 0x9c0906a9, 0xeb0e363f, 0x72076785, 0x5005713,
             0x95bf4a82, 0xe2b87a14, 0x7bb12bae, 0xcb61b38, 0x92d28e9b, 0xe5d5be0d, 0x7cdcefb7, 0xbdbdf21,
             0x86d3d2d4, 0xf1d4e242, 0x68ddb3f8, 0x1fda836e, 0x81be16cd, 0xf6b9265b, 0x6fb077e1, 0x18b74777,
             0x88085ae6, 0xff0f6a70, 0x66063bca, 0x11010b5c, 0x8f659eff, 0xf862ae69, 0x616bffd3, 0x166ccf45,
             0xa00ae278, 0xd70dd2ee, 0x4e048354, 0x3903b3c2, 0xa7672661, 0xd06016f7, 0x4969474d, 0x3e6e77db,
             0xaed16a4a, 0xd9d65adc, 0x40df0b66, 0x37d83bf0, 0xa9bcae53, 0xdebb9ec5, 0x47b2cf7f, 0x30b5ffe9,
             0xbdbdf21c, 0xcabac28a, 0x53b39330, 0x24b4a3a6, 0xbad03605, 0xcdd70693, 0x54de5729, 0x23d967bf,
             0xb3667a2e, 0xc4614ab8, 0x5d681b02, 0x2a6f2b94, 0xb40bbe37, 0xc30c8ea1, 0x5a05df1b, 0x2d02ef8d,
        };

        #endregion

        public static string CRC32Base64FromFile(string filePath)
        {
            return Convert.ToBase64String(System.BitConverter.GetBytes(CRC32FromFile(filePath)));
        }

        public static string CRC32Base64(string text)
        {
            return Convert.ToBase64String(System.BitConverter.GetBytes(CRC32(text)));
        }

        public static string CRC32Base64(string text, Encoding encoding /*Encoding.Default*/)
        {
            return Convert.ToBase64String(System.BitConverter.GetBytes(CRC32(text, encoding)));
        }

        public static string CRC32Base64(byte[] bytes)
        {
            return Convert.ToBase64String(System.BitConverter.GetBytes(CRC32(bytes)));
        }
       
        public static int CRC32FromFile(string filePath)
        {
            return CRC32(File.ReadAllBytes(filePath));
        }

        public static int CRC32(string text)
        {
            return CRC32(Encoding.Default.GetBytes(text));
        }

        public static int CRC32(string text, Encoding encoding /*Encoding.Default*/)
        {
            return CRC32(encoding.GetBytes(text));
        }

        public static int CRC32(byte[] bytes)
        {    
            uint crc;
            int  count;
            count = bytes.Length;
            crc   = 0xFFFFFFFF;
            for (int i = 0; i < count; i++)
                crc = ((crc >> 8) & 0x00FFFFFF) ^ crcTable[(crc ^ bytes[i]) & 0xFF];
            return (int)(crc ^ 0xFFFFFFFF);
        }

        #endregion

        #endregion

        #region 压缩-解压缩

        #region LZMA
        [ExecuteInEditMode]
        public class LZMAThread : MonoBehaviour
        {
            string            mName;
            Thread            mThread;
            ED_1Param<string> onFinished;
            string[]          mFilePaths = { "", "" };

            public static void Create(string name, Thread thread, ED_1Param<string> onfinished, string inFile, string outFile)
            {
                LZMAThread lzmat;
                lzmat               = (new GameObject("__LZMAThread__")).AddComponent<LZMAThread>();
                lzmat.mName         = name;
                lzmat.mThread       = thread;
                lzmat.onFinished    = onfinished;
                lzmat.mFilePaths[0] = inFile;
                lzmat.mFilePaths[1] = outFile;
            }

            void Start()
            { 
                mThread.Start(mFilePaths); 
            }

            void Update()
            {
                if (!mThread.IsAlive)
                {
                    if (onFinished != null)
                        onFinished(mName);
                    DestroyImmediate(gameObject);
                }
            }
        }

        static void OnLZMACompress(object obj)
        {
            string[] filePaths = (string[])obj;
            CompressFileLZMA(filePaths[0], filePaths[1]);

        }

        static void OnLZMADecompress(object obj)
        {
            string[] filePaths = (string[])obj;
            DecompressFileLZMA(filePaths[0], filePaths[1]);
        }

        public static void CompressFileLZMAAsyn(string inFile, string outFile, string key, ED_1Param<string> onFinished)
        {
            LZMAThread.Create(key, new Thread(OnLZMACompress), onFinished, inFile, outFile);
        }

        public static void DecompressFileLZMAAsyn(string inFile, string outFile, string key, ED_1Param<string> onFinished)
        {
            LZMAThread.Create(key, new Thread(OnLZMADecompress), onFinished, inFile, outFile);
        }

        public static void CompressFileLZMA(string inFile, string outFile)
        {
            SevenZip.Compression.LZMA.Encoder coder;
            FileStream input, output;
            coder  = new SevenZip.Compression.LZMA.Encoder();
            input  = new FileStream(inFile, FileMode.Open);
            output = new FileStream(outFile, FileMode.Create);

            //写入头信息
            coder.WriteCoderProperties(output);
            output.Write(BitConverter.GetBytes(input.Length), 0, 8);

            //压缩
            coder.Code(input, output, input.Length, -1, null);

            output.Flush();
            output.Close();
            input.Close();
        }

        public static void DecompressFileLZMA(string inFile, string outFile)
        {
            SevenZip.Compression.LZMA.Decoder coder;
            FileStream input, output;
            coder  = new SevenZip.Compression.LZMA.Decoder();
            input  = new FileStream(inFile, FileMode.Open);
            output = new FileStream(outFile, FileMode.Create);

            // 读取头信息
            byte[] properties = new byte[5];
            byte[] fileLengthBytes = new byte[8];

            input.Read(properties, 0, 5);
            input.Read(fileLengthBytes, 0, 8);

            long fileLength = BitConverter.ToInt64(fileLengthBytes, 0);

            coder.SetDecoderProperties(properties);
            coder.Code(input, output, input.Length, fileLength, null);

            output.Flush();
            output.Close();
            input.Close();
        }

        public static bool IsLZMACompressFile(string filePath)
        {
            FileStream input = File.Open(filePath, FileMode.Open);
            byte[] head4bytes = new byte[4];
            input.Read(head4bytes, 0, 4);
            input.Close();
            return BitConverter.ToUInt32(head4bytes, 0) == 0x4000005d;
        }
       
        #endregion

        #endregion

        #region Libii 加密解密

        #region C

        private static string EncryptC(string content, string pw, string iv)
        {
            byte[] data;
            data = Encoding.UTF8.GetBytes(content);

            // 将byte数组的奇数字节异或0xA	
            for (int i = 0; i < data.Length; i += 2)
                data[i] ^= 0xA;
            // 将byte数组的偶数字节异或0xC	
            for (int i = 1; i < data.Length; i += 2)
                data[i] ^= 0xC;

            // 对这个byte数组用3DES算法加密	
            data = TripleDESEncrypt(data, pw, iv);

            // 	将加密后的byte数组用Base64编码并返回	
            return Convert.ToBase64String(data);
        }

        private static string DecryptC(string content, string pw, string iv)
        {
            // 将data解码成byte数组	
            byte[] data = Convert.FromBase64String(content);

            // 对这个byte数组用3DES算法解密	
            data = TripleDESDecrypt(data, pw, iv);

            // 将byte数组的奇数字节异或0xA	
            for (int i = 0; i < data.Length; i += 2)
                data[i] ^= 0xA;
            // 将byte数组的偶数字节异或0xC	
            for (int i = 1; i < data.Length; i += 2)
                data[i] ^= 0xC;

            return Encoding.UTF8.GetString(data);
        }
   
        #endregion

        #region T

        private static string EncryptT(string content, string pw, string iv)
        {
            byte[] data;
            data = Encoding.UTF8.GetBytes(content);

            // 将byte数组的奇数字节异或0x9	
            for (int i = 0; i < data.Length; i += 2)
                data[i] ^= 0x9;
            // 将byte数组的偶数字节异或0x7	
            for (int i = 1; i < data.Length; i += 2)
                data[i] ^= 0x7;

            // 对这个byte数组用AES算法加密	
            data = AESEncrypt(data, pw, iv);

            // 	将加密后的byte数组用Base64编码并返回	
            return Convert.ToBase64String(data);
        }

        private static string DecryptT(string content, string pw, string iv)
        {
            // 将data解码成byte数组	
            byte[] data = Convert.FromBase64String(content);

            // 对这个byte数组用AES算法解密	
            data = AESDecrypt(data, pw, iv);

            // 将byte数组的奇数字节异或0x9	
            for (int i = 0; i < data.Length; i += 2)
                data[i] ^= 0x9;
            // 将byte数组的偶数字节异或0x7	
            for (int i = 1; i < data.Length; i += 2)
                data[i] ^= 0x7;

            return Encoding.UTF8.GetString(data);
        }

        #endregion

        public enum EncryptType
        {
            C = 1,  //效验类型 C (3DES+BASE64)
            T = 2,  //效验类型 T (AES+BASE64)
            N = 3,  //不加解密 N 
        }

        public static string Encrypt(string content, string pw, string iv, string encryptType)
        {
            EncryptType t = (EncryptType)Enum.Parse(typeof(EncryptType), encryptType);
            return Encrypt(content, pw, iv, t);
        }

        public static string Decrypt(string content, string pw, string iv, string decryptType)
        {
            EncryptType t = (EncryptType)Enum.Parse(typeof(EncryptType), decryptType);
            return Decrypt(content, pw, iv, t);
        }

        public static string Encrypt(string content, string pw = "tripledesencryptionkey12", string iv = "12345678", EncryptType type = EncryptType.C)
        {
            string ret = "";
            switch (type)
            {
                case EncryptType.C:
                    ret = EncryptC(content, pw, iv);
                    break;
                case EncryptType.T:
                    ret = EncryptT(content, pw, iv);
                    break;
                case EncryptType.N:
                    ret = content;
                    break;
            }
            return ret;
        }

        public static string Decrypt(string content, string pw = "tripledesencryptionkey12", string iv = "12345678", EncryptType type = EncryptType.C)
        {
            string ret = "";
            switch (type)
            {
                case EncryptType.C:
                    ret = DecryptC(content, pw, iv);
                    break;
                case EncryptType.T:
                    ret = DecryptT(content, pw, iv);
                    break;
                case EncryptType.N:
                    ret = content;
                    break;
            }
            return ret;
        }

        #endregion

        #region 3DES 加密解密

        private static byte[] TripleDESEncrypt(byte[] data, string key, string iv)
        {
            TripleDESCryptoServiceProvider des;
            des         = new TripleDESCryptoServiceProvider();
            des.Key     = Encoding.ASCII.GetBytes(key);
            des.IV      = Encoding.ASCII.GetBytes(iv);
            des.Mode    = CipherMode.CBC;
            des.Padding = PaddingMode.PKCS7;
            return des.CreateEncryptor().TransformFinalBlock(data, 0, data.Length);
        }

        private static byte[] TripleDESDecrypt(byte[] data, string key, string iv)
        {
            TripleDESCryptoServiceProvider des;
            des         = new TripleDESCryptoServiceProvider();
            des.Key     = Encoding.ASCII.GetBytes(key);
            des.IV      = Encoding.ASCII.GetBytes(iv);
            des.Mode    = CipherMode.CBC;
            des.Padding = PaddingMode.PKCS7;
            return des.CreateDecryptor().TransformFinalBlock(data, 0, data.Length);
        }

        #endregion

        #region AES 加密解密

        private static byte[] AESEncrypt(byte[] data, string key, string iv)
        {
            Rijndael aes = Rijndael.Create();
            aes.Key     = Encoding.ASCII.GetBytes(key);
            aes.IV      = Encoding.ASCII.GetBytes(iv);
            aes.Mode    = CipherMode.CBC;
            aes.Padding = PaddingMode.PKCS7;
            return aes.CreateEncryptor().TransformFinalBlock(data, 0, data.Length);
        }

        private static byte[] AESDecrypt(byte[] data, string key, string iv)
        {
            Rijndael aes = Rijndael.Create();
            aes.Key     = Encoding.ASCII.GetBytes(key);
            aes.IV      = Encoding.ASCII.GetBytes(iv);
            aes.Mode    = CipherMode.CBC;
            aes.Padding = PaddingMode.PKCS7;
            return aes.CreateDecryptor().TransformFinalBlock(data, 0, data.Length);
        }

        #endregion
    }
}

