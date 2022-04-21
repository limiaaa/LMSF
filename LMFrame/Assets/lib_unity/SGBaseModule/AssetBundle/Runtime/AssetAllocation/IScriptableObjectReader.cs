#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

namespace SG
{
    public abstract class IScriptableObjectReader<READER_T, ASSET_T>
        where READER_T : IScriptableObjectReader<READER_T, ASSET_T>, new()
        where ASSET_T : ScriptableObject
    {
        protected IScriptableObjectReader()
        {
        }

        private static READER_T s_reader = new READER_T();
        private static ASSET_T s_asset = null;

        protected abstract string ScriptableObjectAssetNameInResources();

        protected abstract string SubGameScriptableObjectAssetNameInResources();

        public static ASSET_T ScriptableObject
        {
            get
            {
                // 先查找子游戏的配置
                if (s_asset == null)
                {
                    string s = s_reader.SubGameScriptableObjectAssetNameInResources();
                    s_asset = Resources.Load<ASSET_T>(s);
                }

                // 再查找mainApp的配置
                if (s_asset == null)
                {
                    string s = s_reader.ScriptableObjectAssetNameInResources();
                    s_asset = Resources.Load<ASSET_T>(s);
                    if (s_asset == null)
                    {
                        Debug.LogError("Resources 资源目录中无法找到 配置文件 -> " + s);
#if UNITY_EDITOR
                        Debug.LogError("Resources 资源目录下已经创建SettingAsset ");

                        s_asset = UnityEngine.ScriptableObject.CreateInstance<ASSET_T>();
                        if (!AssetDatabase.IsValidFolder("Assets/Resources"))
                            AssetDatabase.CreateFolder("Assets", "Resources");
                        AssetDatabase.CreateAsset(s_asset,
                            "Assets/Resources/SettingAsset.asset");
#endif
                    }
                }

                return s_asset;
            }
        }
    }
}