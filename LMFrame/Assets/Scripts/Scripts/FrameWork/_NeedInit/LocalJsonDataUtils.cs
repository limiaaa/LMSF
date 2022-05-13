using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LMSF.Utils;

public static class LocalJsonDataUtils
{
    static GameData gameData;
    static GlobalData globalData;

    static int SaveCD = 3;
    public static void LoadAll()
    {
        LoadgameData();
        LoadglobalData();
        SaveAll();
    }
    public static void SaveAll()
    {
        SavegameData();
        SaveglobalData();
    }
    public static void LoadgameData()
    {
        gameData = LocalFileUtils.LoadFromFile<GameData>();
        if (gameData == null)
        {
        }
    }
    public static void SavegameData()
    {
        LocalFileUtils.SaveToFile(gameData);
    }
    public static GameData GetGameData()
    {
        return gameData;
    }
    public static void LoadglobalData()
    {
        globalData = LocalFileUtils.LoadFromFile<GlobalData>();
        if (globalData == null)
        {

        }
    }
    public static void SaveglobalData()
    {
        LocalFileUtils.SaveToFile(globalData);
    }
    public static GlobalData GetGlobalData()
    {
        return globalData;
    }
}
