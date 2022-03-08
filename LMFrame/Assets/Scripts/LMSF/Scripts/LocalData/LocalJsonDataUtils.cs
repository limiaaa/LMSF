using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LMSF.Utils;

public class LocalJsonDataUtils:MonoSingleton<LocalJsonDataUtils> 
{
    public GameData gameData;
    public GlobalData globalData;
    
    int SaveCD = 3;
    public void LoadAll()
    {
        LoadgameData();
        LoadglobalData();

        SaveAll();
    }
    public void SaveAll()
    {
        SavegameData();
        SaveglobalData();
    }
    public void LoadgameData()
    {
        gameData = LocalFileUtils.LoadFromFile<GameData>();
        if (gameData == null)
        {

        }
    }
    public void SavegameData()
    {
        LocalFileUtils.SaveToFile(gameData);
    }
    public void LoadglobalData()
    {
        globalData = LocalFileUtils.LoadFromFile<GlobalData>();
        if (globalData == null)
        {

        }
    }
    public void SaveglobalData()
    {
        LocalFileUtils.SaveToFile(globalData);
    }

}

