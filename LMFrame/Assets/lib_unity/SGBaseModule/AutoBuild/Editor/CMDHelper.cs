using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace SG.AutoBuild
{
    public class CMDHelper
    {
        public static void RunFileCmd(string scriptName, List<string> args = null , bool isShowWin = false , string workDir = null)
        {
            string arg = scriptName;
            if (args != null)
            {
                for (int j = 0; j < args.Count; j++)
                {
                    arg = arg + " " + args[j];
                }
            }
            RunStringCmd(arg ,isShowWin , workDir );
        }
    
        public static string RunStringCmd(string cmdContent , bool showWin = false ,string workDir = null)
        {
            string exeName = "";
            string pref = "";
    #if UNITY_EDITOR_WIN
            pref = "/C";
            exeName = "cmd.exe";
    #elif UNITY_EDITOR_OSX
            pref = "";
            exeName = "sh";
    #endif
            string arg = cmdContent;
            Process proc = new Process
            {
                StartInfo =
                {
                    FileName = exeName,
                    Arguments = pref+arg,
                    CreateNoWindow = showWin,
                    WindowStyle = ProcessWindowStyle.Normal,
                    UseShellExecute = false,
                    ErrorDialog =  true,
                    RedirectStandardInput = false ,
                    RedirectStandardOutput =  false,
                    RedirectStandardError = true,
                    WorkingDirectory =  workDir,
                    StandardErrorEncoding = System.Text.Encoding.GetEncoding("gb2312")
                }
            };
    
            proc.Start();
            proc.WaitForExit();
            var sr = proc.StandardError;
            string errorMsg = sr.ReadToEnd();
            UnityEngine.Debug.Log(errorMsg);
            proc.Close();

            return errorMsg;
        }
    }
}
