using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using 一丙英文背起來;

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
        ini.IniWriteValue("Setting", "btn_1", "1", IniFileName);
    }

    private static void ReadSetting()
    {
        //App.Set1 = ini.IniReadValue("Setting", "btn_1", IniFileName);
    }

    public static void SaveSetting()
    {
        //ini.IniWriteValue("Setting", "btn_1", App.Set1, IniFileName);
    }

    public class SetupIniIP
    {
        public string path;
        [DllImport("kernel32", CharSet = CharSet.Unicode)]
        private static extern long WritePrivateProfileString(string section,
        string key, string val, string filePath);
        [DllImport("kernel32", CharSet = CharSet.Unicode)]
        private static extern int GetPrivateProfileString(string section,
        string key, string def, StringBuilder retVal,
        int size, string filePath);
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