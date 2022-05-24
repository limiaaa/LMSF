using System.Collections;
using System.IO;

public class EncryptionStream : FileStream {
    public byte [] EncryKey {
        get; private set;
    }

    public static byte [] UInt32ToByte4 (uint vl) {
        byte [] ret = new byte [4];
        ret [0] = (byte) (vl >> 0);
        ret [1] = (byte) (vl >> 8);
        ret [2] = (byte) (vl >> 16);
        ret [3] = (byte) (vl >> 24);
        return ret;
    }
    public EncryptionStream (string path, FileMode mode, uint encryKey) : base (path, mode) { EncryKey = UInt32ToByte4 (encryKey); }
    public EncryptionStream (string path, FileMode mode, FileAccess access, uint encryKey) : base (path, mode, access) { EncryKey = UInt32ToByte4 (encryKey); }
    public EncryptionStream (string path, FileMode mode, FileAccess access, FileShare share, int bufferSize, uint encryKey) : base (path, mode, access, share, bufferSize) { EncryKey = UInt32ToByte4 (encryKey); }
    public EncryptionStream (string path, FileMode mode, FileAccess access, FileShare share, int bufferSize, bool useAsync, uint encryKey) : base (path, mode, access, share, bufferSize, useAsync) { EncryKey = UInt32ToByte4 (encryKey); }

    public override bool CanSeek => true;

    public override int Read (byte [] array, int offset, int count) {
        int ret = base.Read (array, offset, count);

        for (int i = 0;i < ret;++i) {
            array [i] ^= EncryKey [i % 4];
        }
        return ret;
    }
}
public static class StreamEncryption {
    //16进制key值
    public static uint encryKey = 0x6F7A81F7;
	//加密AssetBundle
	public static void EncryptAssetBundle (string abPath) {
        byte [] EncryKey = EncryptionStream.UInt32ToByte4 (encryKey);
        byte [] bytes = File.ReadAllBytes (abPath);
        for (int i = 0;i < bytes.Length;++i) {
            bytes [i] ^= EncryKey [i % 4];
        }
        File.WriteAllBytes (abPath, bytes);
    }
	//解密AssetBundle
	public static void DecryptAssetBundle (byte [] data) {
        byte [] EncryKey = EncryptionStream.UInt32ToByte4 (encryKey);
        for (int i = 0;i < data.Length;++i) {
            data [i] ^= EncryKey [i % 4];
        }
    }

    public static IEnumerator DecryptAssetBundleAsync (byte [] data) {
        byte [] EncryKey = EncryptionStream.UInt32ToByte4 (encryKey);
        for (int i = 0;i < data.Length;++i) {
            data [i] ^= EncryKey [i % 4];

            if (i % 2048 == 0) {
                yield return new UnityEngine.WaitForEndOfFrame ();
            }
        }
    }
}