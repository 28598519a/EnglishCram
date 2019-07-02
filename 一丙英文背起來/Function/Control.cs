using System;
using System.Runtime.InteropServices;
using System.Windows;

namespace 一丙英文背起來
{
    public static class Control
    {
        public static void ClearText()
        {
            ((MainWindow)Application.Current.MainWindow).lb_Question.Content = string.Empty;
            ((MainWindow)Application.Current.MainWindow).lb_AnswerCheck.Content = string.Empty;
            ((MainWindow)Application.Current.MainWindow).tb_Answer.Text = string.Empty;
            ((MainWindow)Application.Current.MainWindow).tb_Again.Text = string.Empty;
        }

        public static void Test(bool IsEnabled)
        {
            ((MainWindow)Application.Current.MainWindow).tb_Answer.IsEnabled = IsEnabled;
            ((MainWindow)Application.Current.MainWindow).btn_Answer.IsEnabled = IsEnabled;
        }

        public static void Again(bool IsEnabled)
        {
            ((MainWindow)Application.Current.MainWindow).tb_Again.IsEnabled = IsEnabled;
            ((MainWindow)Application.Current.MainWindow).btn_Again.IsEnabled = IsEnabled;
        }

        public static void EndExercise()
        {
            Again(false);
            App.Again_Count = 0;
            ClearText();
            App.Index++;
            Test(true);
        }

        public static void MainVisibility(bool show)
        {
            if (show)
            {
                ((MainWindow)Application.Current.MainWindow).Show();
            }
            else
            {
                ((MainWindow)Application.Current.MainWindow).Hide();
            }
        }

        [DllImport("user32.dll", EntryPoint = "FindWindow", SetLastError = true)]
        static extern IntPtr FindWindowByCaption(IntPtr ZeroOnly, string lpWindowName);

        [DllImport("user32.Dll")]
        static extern int PostMessage(IntPtr hWnd, uint msg, int wParam, int lParam);

        private const uint WM_CLOSE = 0x0010;

        public static void ShowAutoClosingMessageBox(string message, string caption, double second)
        {
            var timer = new System.Timers.Timer(second * 1000) { AutoReset = false };
            timer.Elapsed += delegate
            {
                IntPtr hWnd = FindWindowByCaption(IntPtr.Zero, caption);
                if (hWnd.ToInt32() != 0) PostMessage(hWnd, WM_CLOSE, 0, 0);
            };
            timer.Enabled = true;
            MessageBox.Show(message, caption);
        }
    }
}