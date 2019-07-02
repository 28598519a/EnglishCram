using System;
using System.Collections.Generic;
using System.Windows;

namespace 一丙英文背起來
{
    /// <summary>
    /// App.xaml 的互動邏輯
    /// 全域變數存放區
    /// </summary>
    public partial class App : Application
    {
        public static string Root = Environment.CurrentDirectory;
        public static string FileRoot = Root + "\\Default.db";
        public static string Host = "https://script.google.com";
        public static int Index = 0;
        public static int Again_Count = 0;
        public static double CurrentVersion = 1.8;
        public static List<int> ResultList;
        public static List<ResCless> LRC = new List<ResCless>();

        public static string Set_rb_Answer_Eng = "True";
        public static string Set_rb_Order = "False";
        public static string Set_rb_FileCovertExt = ".db";
        public static string Set_tb_Again_times = "3";
        public static string Set_tb_pfclim = "3";
        public static string Set_cb_AllowEnter = "true";
        public static string Set_cb_pfclim = "true";
    }
}