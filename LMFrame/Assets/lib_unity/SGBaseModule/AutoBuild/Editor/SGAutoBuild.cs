using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Newtonsoft.Json;
using SG.AssetBundleBrowser;
using SG.UI;
using SG.Utils;
using UnityEditor;
using UnityEngine;

namespace SG.AutoBuild.Editor
{
    public enum AutoAndroidBuildType
    {
        Debug_APK,
        Release_APK,
        Release_AAB,
        Release_AAB_APK
    }

    public class SGAutoBuild : UnityEditor.Editor
    {
        private static string GameName = "";

        private static string projectPath = System.IO.Directory.GetCurrentDirectory();
        private static string OutPutPath = projectPath + "/BuildTools/OutPut/";
        private static string AndroidProjectPath = string.Format("{0}{1}_AndroidProject", OutPutPath, GameName);

        [InitializeOnLoad]
        public class Startup
        {
            static Startup()
            {
                Debug.Log("启动");
            }
        }

        [MenuItem("SG/AutoBuild/安卓AB")]
        public static void Build_AB_Android()
        {
            EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTarget.Android);
            DebugUtils.Log("开始打【安卓】AssetBundle包.....");
            AssetBundleBrowserMain mgr = ScriptableObject.CreateInstance<AssetBundleBrowserMain>();
            mgr.m_BuildTab.m_UserData.m_BuildTarget = AssetBundleBuildTab.ValidBuildTarget.Android;
            mgr.OnEnable();

            mgr.m_BuildTab.StartBuild();
            mgr.m_BuildTab.onBuildEnd = delegate
            {
                mgr.m_ConfigTab.SaveData();
                mgr.m_ConfigTab.LoadData();
                DebugUtils.Log("assetbundle包完毕.....");
                DebugUtils.Log("assetbundle拷贝到安装目录下");
                mgr.m_ConfigTab.isStrictMode = false;
                mgr.m_ConfigTab.ExecuteModified();
                mgr.m_BuildTab.onBuildEnd = null;
            };
        }

        private static string startmsg = "[keyword:{0}]开始打包：{1},\n版本号{2},bundleCode:{3}\n包名：{4}\n{5},\n打包设备：{6}";
        private static string endmsg = "[keyword:{0}]打包结束：{1},\n版本号{2},bundleCode:{3}\n包名：{4}\n{5},\n打包设备：{6},\n下载地址：{7}";

        private static string errormsg =
            "[keyword:{0}]打包参数没有设置，请在Unity编辑器中通过 SG->AutoBuild->生成安卓打包参数模板 菜单生成配置文件，然后在文件【{1}】中填好对应参数";

        [MenuItem("SG/AutoBuild/打包：Android Debug")]
        public static void AutomationBuild()
        {
            BuildAndroid(AutoAndroidBuildType.Debug_APK);
        }

        [MenuItem("SG/AutoBuild/打包：Android Release")]
        public static void AutomationBuild_Android_Release()
        {
            BuildAndroid(AutoAndroidBuildType.Release_APK);
        }

        [MenuItem("SG/AutoBuild/打包：Android Release .aab")]
        public static void AutomationBuild_Android_ReleaseAAB()
        {
            BuildAndroid(AutoAndroidBuildType.Release_AAB);
        }

        private static void BuildAndroid(AutoAndroidBuildType buildType)
        {
            BuildTarget mBuildTarget = EditorUserBuildSettings.activeBuildTarget;
            if (mBuildTarget != BuildTarget.Android)
            {
                EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTargetGroup.Android, BuildTarget.Android);
            }

            DingTalkData talkParam = DingTalkTemplateGenerator.GetDingTalkData();
            AndroidBuildData data = BuildParamGenerator.GetAndroidBuildData();
            if (data == null)
            {
                DingDingTalk.ErrorBuildDingTalk(talkParam.token, string.Format(errormsg,
                    talkParam.keyWord, BuildParamGeneratorEnv.GetAndroidParamFilePath()));
            }
            else
            {
                string path = GameName;
                string msg = "";
                bool isDevelopment = false;
                string file_path = "debug/";
                string endpx = ".apk";
                GameName = PlayerSettings.productName.Replace(" ", "_") + "_" + data.mVersion + "_" +
                           data.mBundleVersionCode + "_" + DateTime.Now.ToString("yyyyMMddHHmm");
                string ApkPath = string.Format("{0}{1}.apk", OutPutPath, GameName);
                string AABPath = string.Format("{0}{1}.aab", OutPutPath, GameName);
                EditorUserBuildSettings.exportAsGoogleAndroidProject = false;
                EditorUserBuildSettings.buildAppBundle = false;
                switch (buildType)
                {
                    case AutoAndroidBuildType.Debug_APK:
                        path = ApkPath;
                        file_path = "debug/";
                        msg = "安卓Debug--apk";
                        SettingReader.ScriptableObject.IsLogEnabled = true;
                        SettingReader.ScriptableObject.isDebug = true;
                        isDevelopment = true;
                        break;
                    case AutoAndroidBuildType.Release_APK:
                        path = ApkPath;
                        file_path = "release/";
                        msg = "安卓release--apk";
                        isDevelopment = false;
                        SettingReader.ScriptableObject.IsLogEnabled = false;
                        SettingReader.ScriptableObject.isDebug = false;
                        break;
                    case AutoAndroidBuildType.Release_AAB:
                        path = AABPath;
                        endpx = ".aab";
                        file_path = "release/";
                        msg = "安卓release--aab";
                        isDevelopment = false;
                        SettingReader.ScriptableObject.IsLogEnabled = false;
                        SettingReader.ScriptableObject.isDebug = false;
                        EditorUserBuildSettings.buildAppBundle = true;
                        break;
                    case AutoAndroidBuildType.Release_AAB_APK:
                        SettingReader.ScriptableObject.IsLogEnabled = false;
                        SettingReader.ScriptableObject.isDebug = false;
                        isDevelopment = false;
                        path = AABPath;
                        endpx = ".aab";
                        file_path = "release/";
                        msg = "安卓release--aab->apk";
                        break;
                }

                SettingReader.ScriptableObject.isLoadFromAssetBundle = true;
                if (SettingReader.ScriptableObject.IsBuildNotify)
                {
                    DingDingTalk.StartBuildDingTalk(talkParam.token,
                        string.Format(startmsg, talkParam.keyWord, data.mAppName, data.mVersion,
                            data.mBundleVersionCode, data.mPackageName, msg , SystemInfo.deviceName));
                }

                PlayerSettings.productName = data.mAppName;
                PlayerSettings.bundleVersion = data.mVersion;
                PlayerSettings.applicationIdentifier = data.mPackageName;
                PlayerSettings.Android.bundleVersionCode = data.mBundleVersionCode;
                PlayerSettings.Android.keystoreName = data.mKeystorePath; // 路径
                PlayerSettings.Android.keystorePass = data.mKeystorePassword; // 密钥密码
                PlayerSettings.Android.keyaliasName = data.mKeystoreAliasName; // 密钥别名
                PlayerSettings.Android.keyaliasPass = data.mKeyaliasPass;
                BuildOptions buildOption = BuildOptions.None;
                if (isDevelopment)
                    buildOption |= BuildOptions.Development;
                else
                    buildOption &= BuildOptions.Development;

                BuildPipeline.BuildPlayer(EditorBuildSettings.scenes, path, BuildTarget.Android, buildOption);
                DebugUtils.Log("------------- 结束 BuildAPK -------------");
                DebugUtils.Log("Build目录：" + ApkPath);
                if (SettingReader.ScriptableObject.IsBuildNotify)
                {
                    DingDingTalk.EndBuildDingTalk(file_path + GameName + endpx,
                        string.Format(endmsg, talkParam.keyWord, data.mAppName, data.mVersion, data.mBundleVersionCode,
                            data.mPackageName, msg, SystemInfo.deviceName , (talkParam.url+file_path + GameName + endpx)), talkParam.url, talkParam.token);
                }

                if (buildType == AutoAndroidBuildType.Release_AAB || buildType == AutoAndroidBuildType.Release_AAB_APK)
                {
                    DingDingTalk.StartBuildDingTalk(talkParam.token , talkParam.keyWord+"开始校验aab安装包合理性："+GameName+endpx);
                    //验证abb
                    string errormsg = DoAABValidityShell(path , data);
                    if (errormsg.Equals(""))
                    {
                        DingDingTalk.StartBuildDingTalk(talkParam.token, talkParam.keyWord+"校验成功！！！！！" + path+".apks");
                    }
                    else
                    {
                        DingDingTalk.StartBuildDingTalk(talkParam.token, talkParam.keyWord+"校验失败！！日志：" + errormsg);
                    }
                }
            }
        }

        [MenuItem("SG/AutoBuild/打包：测试函数")]
        public static void TestFunc()
        {
            RunShell("java");
        }

        /// <summary>
        /// 执行安装包转apks
        /// </summary>
        /// <returns></returns>
        public static string DoAABValidityShell(string aabFile, AndroidBuildData buildData)
        {
            //string cmd1 = "cd "+projectPath+"/tools/ \n";
            string cmd = "java -jar "+projectPath+"/tools/bundletool.jar build-apks --bundle={0} --output={0}.apks --ks={1} --ks-pass=pass:{2} --ks-key-alias={3} --key-pass=pass:{4}";
            string shellCmd = string.Format(cmd, aabFile,projectPath+"/"+buildData.mKeystorePath, buildData.mKeystorePassword,
                buildData.mKeystoreAliasName, buildData.mKeyaliasPass);
            
            string filePath = projectPath+"/tools/checkaab.bat";
#if UNITY_EDITOR_WIN
            filePath = projectPath+"/tools/checkaab.bat";
#elif UNITY_EDITOR_OSX
            filePath = projectPath+"/tools/checkaab.sh";
#endif
            
            FileStream fs = new FileStream(filePath, FileMode.OpenOrCreate, FileAccess.Write);
            if (fs != null)
            {
                fs.Seek(0, SeekOrigin.Begin);
                fs.SetLength(0);
                byte[] bytes = Encoding.Default.GetBytes(shellCmd);

                fs.Write(bytes, 0, bytes.Length);
            }
            fs.Close();
            return RunShell(filePath);
        }
        
        public static string RunShell(string shellcmd)
        {
            // 这里不开线程的话，就会阻塞住unity的主线程，当然如果你需要阻塞的效果的话可以不开
           // Thread newThread = new Thread(new ThreadStart(RunShellThreadStart));
           // newThread.Start();
          return CMDHelper.RunStringCmd(shellcmd);
        }
    }
}

