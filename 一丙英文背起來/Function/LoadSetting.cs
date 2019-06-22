using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;

namespace 一丙英文背起來
{
    public class LoadSetting
    {
        public static SetupIniIP ini = new SetupIniIP();
        public static string IniFileName = "UserSetting.ini";
        public static void Init_Setting()
        {
            if (File.Exists($"{App.Root}\\{IniFileName}"))
            {
                ReadSetting();
            }
            else
            {
                SetDefault();
                ReadSetting();
            }
        }

        private static void SetDefault()
        {
            ini.IniWriteValue("Setting", "rb_Answer_Eng", "True", IniFileName);
            ini.IniWriteValue("Setting", "rb_Order", "True", IniFileName);
        }

        private static void ReadSetting()
        {
            App.Set_rb_Answer_Eng = ini.IniReadValue("Setting", "rb_Answer_Eng", IniFileName);
            App.Set_rb_Order = ini.IniReadValue("Setting", "rb_Order", IniFileName);
        }

        public static void SaveSetting()
        {
            ini.IniWriteValue("Setting", "rb_Answer_Eng", App.Set_rb_Answer_Eng, IniFileName);
            ini.IniWriteValue("Setting", "rb_Order", App.Set_rb_Order, IniFileName);
        }

        public class SetupIniIP
        {
            public string path;

            [DllImport("kernel32", CharSet = CharSet.Unicode)]
            private static extern long WritePrivateProfileString(string section, string key, string val, string filePath);

            [DllImport("kernel32", CharSet = CharSet.Unicode)]
            private static extern int GetPrivateProfileString(string section, string key, string def, StringBuilder retVal, int size, string filePath);

            public void IniWriteValue(string Section, string Key, string Value, string inipath)
            {
                WritePrivateProfileString(Section, Key, Value, App.Root + "\\" + inipath);
            }

            public string IniReadValue(string Section, string Key, string inipath)
            {
                StringBuilder temp = new StringBuilder(255);
                int i = GetPrivateProfileString(Section, Key, "", temp, 255, App.Root + "\\" + inipath);
                return temp.ToString();
            }
        }
    }
}