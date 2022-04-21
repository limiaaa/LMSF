namespace SG.LevelEditor { 
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.IO;
    using UnityEngine;

    public static class EditorTool
    {

        public static Color ColorFromRGBString(string rgbstr)
        {
            int ivalue;
            try
            {
                ivalue = Convert.ToInt32(rgbstr, 16);
            }
            catch (Exception)
            {

                throw;
            }

            byte a = 0xff;// (byte)((ivalue >> 24) & 0xff);
            byte r = (byte)((ivalue >> 16) & 0xff);
            byte g = (byte)((ivalue >> 8) & 0xff);
            byte b = (byte)((ivalue >> 0) & 0xff);

            Color32 c32 = new Color32(r, g, b, a);
            return c32;
        }


        public static string ColorToRGBString(Color c)
        {
            Color32 c32 = c;
            var s = string.Format("{0:x2}{1:x2}{2:x2}", c32.r, c32.g, c32.b);
            return s;
        }


        public static string LoadJson(string path)
        {
            if (!File.Exists(path))
            {
                return null;
            }

            StreamReader sr = new StreamReader(path);
            string str = sr.ReadToEnd();
            sr.Close();
            return str;
        }

        public static void SaveJson(string path, string jsonStr)
        {
            if (File.Exists(path))
            {
                File.Delete(path);
            }

            StreamWriter sw = new StreamWriter(path, true);
            sw.Write(jsonStr);
            sw.Flush();
            sw.Close();
        }


        public static FileInfo LoadFile(string path)
        {
            if (!File.Exists(path)) { return null; }

            FileInfo file = new FileInfo(path);

            return file;
        }


        public static string TimeString()
        {
            return TimeString(DateTime.Now);
        }
        public static string TimeString(DateTime t)
        {
            if (t == null)
            {
                t = DateTime.Now;
            }

            string s = String.Format("{0:D4}.{1:D2}{2:D2}.{3:D2}{4:D2}{5:D2}",
                t.Year,
                t.Month,
                t.Day,
                t.Hour,
                t.Minute,
                t.Second
                );
            return s;
        }


    }
}