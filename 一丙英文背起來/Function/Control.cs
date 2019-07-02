using System;
using System.Runtime.InteropServices;
using System.Security;
using System.Timers;
using System.Windows;

namespace 一丙英文背起來
{
    public class Control
    {
        /// <summary>
        /// 清理控件文字
        /// </summary>
        public static void ClearText()
        {
            ((MainWindow)Application.Current.MainWindow).lb_Question.Content = string.Empty;
            ((MainWindow)Application.Current.MainWindow).lb_AnswerCheck.Content = string.Empty;
            ((MainWindow)Application.Current.MainWindow).tb_Answer.Text = string.Empty;
            ((MainWindow)Application.Current.MainWindow).tb_Again.Text = string.Empty;
        }

        /// <summary>
        /// 測驗相關控件
        /// </summary>
        /// <param name="IsEnabled">啟用</param>
        public static void Test(bool IsEnabled)
        {
            ((MainWindow)Application.Current.MainWindow).tb_Answer.IsEnabled = IsEnabled;
            ((MainWindow)Application.Current.MainWindow).btn_Answer.IsEnabled = IsEnabled;
        }

        /// <summary>
        /// 練習相關控件
        /// </summary>
        /// <param name="IsEnabled">啟用</param>
        public static void Again(bool IsEnabled)
        {
            ((MainWindow)Application.Current.MainWindow).tb_Again.IsEnabled = IsEnabled;
            ((MainWindow)Application.Current.MainWindow).btn_Again.IsEnabled = IsEnabled;
        }

        /// <summary>
        /// 結束練習
        /// </summary>
        public static void EndExercise()
        {
            Again(false);
            App.Again_Count = 0;
            ClearText();
            App.Index++;
            Test(true);
        }

        /// <summary>
        /// 顯示主程式視窗
        /// </summary>
        /// <param name="show">顯示</param>
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

        /// <summary>
        /// 列表寬度自適應
        /// </summary>
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

        /// <summary>
        /// 從全域變數導入列表
        /// </summary>
        public static void Set_Lv_res_list()
        {
            ((MainWindow)Application.Current.MainWindow).lv_res_list.ItemsSource = null;
            ((MainWindow)Application.Current.MainWindow).lv_res_list.ItemsSource = App.LRC;
            ((MainWindow)Application.Current.MainWindow).lb_lsCount.Content = App.LRC.Count;
        }

        [SuppressUnmanagedCodeSecurity]
        internal static class SafeNativeMethods
        {
            /// <summary>
            /// 由視窗名稱找到視窗
            /// </summary>
            [DllImport("user32.dll", EntryPoint = "FindWindow", SetLastError = true, CharSet = CharSet.Unicode)]
            internal static extern IntPtr FindWindowByCaption(IntPtr ZeroOnly, string lpWindowName);

            /// <summary>
            /// 傳送委託給視窗
            /// </summary>
            [DllImport("user32.Dll", CharSet = CharSet.Unicode)]
            internal static extern int PostMessage(IntPtr hWnd, uint msg, int wParam, int lParam);
        }

        private const uint WM_CLOSE = 0x0010;

        /// <summary>
        /// 可定時關閉的MessageBox
        /// </summary>
        /// <param name="message">顯示的內容</param>
        /// <param name="caption">標題</param>
        /// <param name="second">秒數</param>
        public static void ShowAutoClosingMessageBox(string message, string caption = "通知", double second = 3)
        {
            Timer timer = new System.Timers.Timer(second * 1000) { AutoReset = false };
            // timer觸發時，關閉指定標題之視窗
            timer.Elapsed += delegate
            {
                IntPtr hWnd = SafeNativeMethods.FindWindowByCaption(IntPtr.Zero, caption);
                if (hWnd.ToInt32() != 0) SafeNativeMethods.PostMessage(hWnd, WM_CLOSE, 0, 0);
            };
            timer.Enabled = true;
            MessageBox.Show(message, caption);
        }
    }
}