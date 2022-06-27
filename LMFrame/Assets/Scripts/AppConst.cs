
using System.Collections.Generic;
using UnityEngine;

public static class AppConst
{
#if DEBUG_TEST
    public const bool DebugMode = true;                       //调试模式-用于内部测试
#else
	public const bool DebugMode = false;                       //调试模式-用于内部测试
#endif
	/// <summary>
	/// 如果想删掉框架自带的例子，那这个例子模式必须要
	/// 关闭，否则会出现一些错误。
	/// </summary>
	public const bool ExampleMode = false;                       //例子模式 
	public static bool UpdateMode = true;                       //更新模式-默认关闭 
	public static bool LuaByteMode = false;                       //Lua字节码模式-默认关闭 
	public static bool LuaBundleMode = false;                    //Lua代码AssetBundle模式
	public const int TimerInterval = 1;
	public const int GameFrameRate = 60;                        //游戏帧频
	public const string AppName = "Main";               //应用程序名称
	public const string TempDir = "TempFolder/";                    //临时目录
	public const string AppPrefix = AppName + "_";              //应用程序前缀
	public static string ExtName = ".lo";                   //素材扩展名
	public static string AssetDir = "StreamingAssets";           //素材目录 
	public static string AESEncryptKey = "TWf7g1Gt701h0.#0";
	public static string AESEncryptIV = "YgnHV16#8HQFc&16";
	public static string MainAbName = "abundleinfo";
	public static string MainMainfestName = "AssetBundleManifest";
	

	public static bool EncryptABBol = false;
	//机器码
	public static string EquipmentCode = string.Empty;
	public static string SocketAddress = "";
	public static int SocketPort = 0;
	public static string FrameworkRoot
	{
		get
		{
			return Application.dataPath + "/" + AppName;
		}
	}










}
