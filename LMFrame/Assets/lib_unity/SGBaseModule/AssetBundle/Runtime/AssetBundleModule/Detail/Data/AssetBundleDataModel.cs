using System;

namespace SG.AssetBundleBrowser.AssetBundlePacker
{
    [Serializable]
    public class HotfixInfo
    {
        public string url;
        public string luaCmd;
    }

    [Serializable]
    public class ExtendResInfo
    {
        public enum UpdateType
        {
            /// <summary>
            /// 
            /// </summary>
            None = 0,

            /// <summary>
            /// 
            /// </summary>
            Delete = 1,

            /// <summary>
            /// 
            /// </summary>
            Update = 2
        }

        public string url;
        public string localPath;
        public long version;
        public string md5;


        public UpdateType updateType;
    }
}