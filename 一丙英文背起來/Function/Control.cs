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

        public static void Lv_res_list_autowidth()
        {
            /*
             * 中文、熟練度欄寬度自適應
             * 強制更新欄位狀態已確保ActualWidth被設定
             * 英文欄位取得剩餘寬度
             */
            ((MainWindow)Application.Current.MainWindow).GC_Cht.Width = double.NaN;
            ((MainWindow)Application.Current.MainWindow).GC_Eng.Width = double.NaN;
            ((MainWindow)Application.Current.MainWindow).GC_Pfc.Width = double.NaN;
            ((MainWindow)Application.Current.MainWindow).lv_res_list.UpdateLayout();

            if (((MainWindow)Application.Current.MainWindow).lv_res_list.Width > ((MainWindow)Application.Current.MainWindow).GC_Cht.ActualWidth + ((MainWindow)Application.Current.MainWindow).GC_Eng.ActualWidth + ((MainWindow)Application.Current.MainWindow).GC_Pfc.ActualWidth)
            {
                ((MainWindow)Application.Current.MainWindow).GC_Eng.Width = ((MainWindow)Application.Current.MainWindow).lv_res_list.Width - ((MainWindow)Application.Current.MainWindow).GC_Cht.ActualWidth - ((MainWindow)Application.Current.MainWindow).GC_Pfc.ActualWidth - 10;
            }
        }

        public static void Set_Lv_res_list()
        {
            ((MainWindow)Application.Current.MainWindow).lv_res_list.ItemsSource = null;
            ((MainWindow)Application.Current.MainWindow).lv_res_list.ItemsSource = App.LRC;
            ((MainWindow)Application.Current.MainWindow).lb_lsCount.Content = App.LRC.Count;
        }

        [DllImport("user32.dll", EntryPoint = "FindWindow", SetLastError = true)]
        private static extern IntPtr FindWindowByCaption(IntPtr ZeroOnly, string lpWindowName);

        [DllImport("user32.Dll")]
        private static extern int PostMessage(IntPtr hWnd, uint msg, int wParam, int lParam);

        private const uint WM_CLOSE = 0x0010;

        public static void ShowAutoClosingMessageBox(string message, string caption = "通知", double second = 3)
        {
            var timer = new System.Timers.Timer(second * 1000) { AutoReset = false };
            // timer觸發時，關閉指定標題之視窗
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