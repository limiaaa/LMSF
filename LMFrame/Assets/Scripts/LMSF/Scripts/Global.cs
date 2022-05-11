using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Global 
{

    public static string FlyCoinPath = "Assets/LMSF/Module/FlyCoin/Resource/Prefeb/FlyCoin.prefab";
    /// <summary>
    /// 多语言映射表
    /// </summary>
    /// 
    public static readonly string[][] LANGUAGE_VALUES = new string[][]{
        new string[] {"en","English"},
        new string[] {"zh","简体中文"},
    };

    /// <summary>
    /// 游戏支持的语言
    /// </summary>
    public enum GameLanguage
    {
        Default = -1,
        en,
        zh,
    }
    // <summary>
    /// 游戏运行时全局数据
    /// </summary>
    public static class Runtime
    {
        public static GameLanguage mRuntimeLanguage
        {
            get { return LocalJsonDataUtils.Instance.gameData.mCurrentLanguage; }
            set
            {
                LocalJsonDataUtils.Instance.gameData.mCurrentLanguage = value;
                LocalJsonDataUtils.Instance.SavegameData();
            }
        }
    }
    public enum SWITCH_UI_PAGE
    {
        SETTING,
        GAME_PAUSE,
    }
    public enum SWITCH_TYPE
    {
        SOUND,
        MUSIC,
        VIBRATION
    }
}
