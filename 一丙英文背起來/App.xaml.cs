using System;
using System.Collections.Generic;
using System.Windows;

namespace 一丙英文背起來
{
    /// <summary>
    /// App.xaml 的互動邏輯
    /// </summary>
    public partial class App : Application
    {
        public static string Root = Environment.CurrentDirectory;
        public static int Index = 0;
        public static List<int> ResultList;
        public static List<ResCless> LRC = new List<ResCless>();

        public static string Set_rb_Answer_Eng = "True";
        public static string Set_rb_Order = "True";

        public static class Control
        {
            public static void ClearText()
            {
                ((MainWindow)Current.MainWindow).lb_Question.Content = "";
                ((MainWindow)Current.MainWindow).lb_AnswerCheck.Content = "";
                ((MainWindow)Current.MainWindow).tb_Answer.Text = "";
                ((MainWindow)Current.MainWindow).tb_Again1.Text = "";
                ((MainWindow)Current.MainWindow).tb_Again2.Text = "";
                ((MainWindow)Current.MainWindow).tb_Again3.Text = "";
            }

            public static void Test(bool IsEnabled)
            {
                ((MainWindow)Current.MainWindow).tb_Answer.IsEnabled = IsEnabled;
                ((MainWindow)Current.MainWindow).btn_Answer.IsEnabled = IsEnabled;
            }

            public static void Exercise(bool IsEnabled)
            {
                Again1(IsEnabled);
                Again2(IsEnabled);
                Again3(IsEnabled);
            }

            public static void Again1(bool IsEnabled)
            {
                ((MainWindow)Current.MainWindow).tb_Again1.IsEnabled = IsEnabled;
                ((MainWindow)Current.MainWindow).btn_Again1.IsEnabled = IsEnabled;
            }

            public static void Again2(bool IsEnabled)
            {
                ((MainWindow)Current.MainWindow).tb_Again2.IsEnabled = IsEnabled;
                ((MainWindow)Current.MainWindow).btn_Again2.IsEnabled = IsEnabled;
            }

            public static void Again3(bool IsEnabled)
            {
                ((MainWindow)Current.MainWindow).tb_Again3.IsEnabled = IsEnabled;
                ((MainWindow)Current.MainWindow).btn_Again3.IsEnabled = IsEnabled;
            }
        }
    }

    public class ResCless
    {
        public string NameCht { get; set; }
        public string NameEng { get; set; }
    }
}
