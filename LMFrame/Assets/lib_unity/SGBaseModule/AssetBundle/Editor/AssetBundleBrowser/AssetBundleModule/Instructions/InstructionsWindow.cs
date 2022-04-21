﻿/***************************************************************
 * Copyright 2016 By Zhang Minglin
 * Author: Zhang Minglin
 * Create: 2016/12/19
 * Note  : 使用说明
***************************************************************/
using UnityEditor;
using UnityEngine;
using System.Collections;

public class InstructionsWindow : EditorWindow
{
    static string Instructions =
        "本插件通过使用菜单（AssetBundle->Step1~3）的三个步骤实现打包AssetBundle\n\n"
      + "打包详细说明\n"
      + "1. Step1 - 打包AssetBundle\n"
      + "  #负责设置打包平台，配置需要打包的资源与场景，下列为支持的打包目录\n"
      + "      资源（Assets）\n"
      + "      场景（Assets/Scenes）\n"
      + "  #支持配置资源的打包规则\n"
      + "      None:        不打包\n"
      + "      SignleFile:  打包此资源文件（资源设置有效）\n"
      + "      Folder:      打包此文件夹\n"
      + "      Ignore:      忽略此文件夹下所有资源或忽略此资源\n"
      + "  #资源粒度表示当前资源被多少个AssetBundle打包，可以根据粒度值调整打包策略（频繁使用的AssetBundle应该保证粒度尽量为1）\n\n"
      + "2. Step2 - 配置AssetBundle\n"
      + "  负责AssetBundle的压缩、是否跟随安装包打包、游戏加载时是否常驻于内存中等配置信息的设置\n"
      + "  AssetBundle压缩采用LZMA压缩算法，执行会压缩勾选的资源，当下载AssetBundle后会自动解压到对应的目录，并且删除此压缩包（AssetBundleDownloader）。\n"
      + "  游戏中初始使用到的、常用的、必要的AssetBundle可以考虑勾选打包至安装包。\n"
      + "  勾选常驻内存常常用于游戏中频繁使用到的AssetBundle，可以避免多次加载消耗。\n\n"
      + "3. Step3 - 配置AssetBundle资源包\n"
      + "  负责资源包的创建，修改，删除等操作。资源包可用于游戏运行中临时下载更新的AssetBundle。\n"
      + "  资源包配置文件会自动打包至安装包中，或者通过Updater下载更新资源包配置文件。\n"
      + "  插件提供PackageDownloader供游戏中下载指定资源包。\n\n"
      + "插件需手动配置的事项\n"
      + "1. 增加命名为\"" + SG.AssetBundleBrowser.AssetBundlePacker.SceneConfigTool.SERIALIZE_SCENE_OBJECT_TAG + "\"的GameObject's Tag（Edit->Project Settings->Tags and Layers）\n\n\n\n"
      + "FAQ\n"
      + "1. 游戏中如何使用打包后的AssetBundle?\n"
      + "      插件自带的例子包含详细的使用方试\n"
      + "      Example - 0: 展示AssetBundlePacker插件如何启动、AssetBundle如何加载使用等\n"
      + "      Example - 1: 展示AssetBundle如何更新（例子如需完整运行要按例子的需求配置AssetBundle下载的远程服务器）\n"
      + "      Example - 2: 展示资源包的使用方式，包含下载、更新、使用等（例子如需完整运行要按例子的需求配置AssetBundle下载的远程服务器）\n";


    /// <summary>
    /// 
    /// </summary>
    void OnGUI()
    {
        GUILayout.Label(Instructions);
    }

    //[MenuItem("AssetBundle/Instructions", false, 1)]
    public static void Open()
    {
        EditorWindow.GetWindow<InstructionsWindow>(true, "使用说明", true).Show();
    }
}
