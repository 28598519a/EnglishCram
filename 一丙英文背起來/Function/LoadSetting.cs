using System.IO;
using System.Runtime.InteropServices;
using System.Security;
using System.Text;

namespace 一丙英文背起來
{
    public class LoadSetting
    {
        private static SetupIniIP ini = new SetupIniIP();
        public static string IniFileName = "UserSetting.ini";

        /// <summary>
        /// 讀取設定檔
        /// </summary>
        public static void Init_Setting()
        {
            if (File.Exists(Path.Combine(App.Root, IniFileName)))
            {
                ReadSetting();
            }
            else
            {
                SetDefault();
                ReadSetting();
            }
        }

        /// <summary>
        /// 設定 設定檔的初值
        /// </summary>
        private static void SetDefault()
        {
            ini.IniWriteValue("Setting", "rb_Answer_Eng", "True", IniFileName);
            ini.IniWriteValue("Setting", "rb_Order", "False", IniFileName);
            ini.IniWriteValue("Setting", "rb_FileCovertExt", ".db", IniFileName);
            ini.IniWriteValue("Setting", "tb_Again_times", "3", IniFileName);
            ini.IniWriteValue("Setting", "tb_pfclim", "3", IniFileName);
            ini.IniWriteValue("Setting", "cb_AllowEnter", "True", IniFileName);
            ini.IniWriteValue("Setting", "cb_pfclim", "True", IniFileName);
        }

        /// <summary>
        /// 讀取設定檔的值
        /// </summary>
        private static void ReadSetting()
        {
            App.Set_rb_Answer_Eng = ini.IniReadValue("Setting", "rb_Answer_Eng", IniFileName);
            App.Set_rb_Order = ini.IniReadValue("Setting", "rb_Order", IniFileName);
            App.Set_rb_FileCovertExt = ini.IniReadValue("Setting", "rb_FileCovertExt", IniFileName);
            App.Set_tb_Again_times = ini.IniReadValue("Setting", "tb_Again_times", IniFileName);
            App.Set_tb_pfclim = ini.IniReadValue("Setting", "tb_pfclim", IniFileName);
            App.Set_cb_AllowEnter = ini.IniReadValue("Setting", "cb_AllowEnter", IniFileName);
            App.Set_cb_pfclim = ini.IniReadValue("Setting", "cb_pfclim", IniFileName);
        }

        /// <summary>
        /// 保存值進設定檔
        /// </summary>
        public static void SaveSetting()
        {
            ini.IniWriteValue("Setting", "rb_Answer_Eng", App.Set_rb_Answer_Eng, IniFileName);
            ini.IniWriteValue("Setting", "rb_Order", App.Set_rb_Order, IniFileName);
            ini.IniWriteValue("Setting", "rb_FileCovertExt", App.Set_rb_FileCovertExt, IniFileName);
            ini.IniWriteValue("Setting", "tb_Again_times", App.Set_tb_Again_times, IniFileName);
            ini.IniWriteValue("Setting", "tb_pfclim", App.Set_tb_pfclim, IniFileName);
            ini.IniWriteValue("Setting", "cb_AllowEnter", App.Set_cb_AllowEnter, IniFileName);
            ini.IniWriteValue("Setting", "cb_pfclim", App.Set_cb_pfclim, IniFileName);
        }

        private class SetupIniIP
        {
            [SuppressUnmanagedCodeSecurity]
            internal static class SafeNativeMethods
            {
                [DllImport("kernel32", CharSet = CharSet.Unicode)]
                internal static extern bool WritePrivateProfileString(string section, string key, string val, string filePath);


                [DllImport("kernel32", CharSet = CharSet.Unicode)]
                internal static extern int GetPrivateProfileString(string section, string key, string def, StringBuilder retVal, int size, string filePath);

            }

            /// <summary>
            /// ini寫值
            /// </summary>
            /// <param name="Section">集合</param>
            /// <param name="Key">索引</param>
            /// <param name="Value">值</param>
            /// <param name="inipath">ini路徑</param>
            public void IniWriteValue(string Section, string Key, string Value, string inipath)
            {
                SafeNativeMethods.WritePrivateProfileString(Section, Key, Value, Path.Combine(App.Root, inipath));
            }

            /// <summary>
            /// ini讀值
            /// </summary>
            /// <param name="Section">集合</param>
            /// <param name="Key">索引</param>
            /// <param name="inipath">ini路徑</param>
            /// <returns>ini值</returns>
            public string IniReadValue(string Section, string Key, string inipath)
            {
                StringBuilder temp = new StringBuilder(255);
                int i = SafeNativeMethods.GetPrivateProfileString(Section, Key, string.Empty, temp, 255, Path.Combine(App.Root, inipath));
                return temp.ToString();
            }
        }
    }
}