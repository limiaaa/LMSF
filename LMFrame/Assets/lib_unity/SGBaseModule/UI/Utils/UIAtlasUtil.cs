using System.Collections;
using System.Collections.Generic;
using SG.AssetBundleBrowser.AssetBundlePacker;
using UnityEngine;
using UnityEngine.U2D;

namespace SG.UI
{
    public class UIAtlasUtil
    {
        private static Dictionary<string, SpriteAtlas> mSpriteAtlasCache = new Dictionary<string, SpriteAtlas>();
        
        /// <summary>
        /// 加载Atlas
        /// </summary>
        /// <param name="path">资源全路径</param>
        /// <returns></returns>
        public static SpriteAtlas LoadAtlas(string path)
        {
            if (mSpriteAtlasCache.ContainsKey(path))
                return mSpriteAtlasCache[path];
            SpriteAtlas atlas = ResourcesManager.Load<SpriteAtlas>(path);
            if(atlas==null)
                Debug.LogError("图集加载错误 ["+path+"]请检查资源路径是否正确~！！！");
            mSpriteAtlasCache.Add(path, atlas);
            return atlas;
        }
        
        /// <summary>
        /// 通过图集获取Sprite
        /// </summary>
        /// <param name="atlasPath">图集路径</param>
        /// <param name="spriteName">sprite名称</param>
        /// <returns></returns>
        public static Sprite GetSpriteByAtlas(string atlasPath , string spriteName)
        {
            SpriteAtlas atlas = LoadAtlas(atlasPath);
            if (atlas != null)
            {
                return atlas.GetSprite(spriteName); 
            }

            return null;
        }

        /// <summary>
        /// 已知图集中加载Sprite
        /// </summary>
        /// <param name="atlas">图集</param>
        /// <param name="spriteName">sprite名称</param>
        /// <returns></returns>
        public static Sprite GetSpriteByAtlas(SpriteAtlas atlas , string spriteName)
        {
            if (atlas == null) return null;
            return atlas.GetSprite(spriteName); 
        }

        public static void ClearCache()
        {
            mSpriteAtlasCache = new Dictionary<string, SpriteAtlas>();
        }
    }
}

