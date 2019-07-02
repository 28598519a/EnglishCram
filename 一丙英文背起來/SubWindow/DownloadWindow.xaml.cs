using System;
using System.ComponentModel;
using System.Net;
using System.Windows;

namespace 一丙英文背起來
{
    /// <summary>
    /// DownloadWindow.xaml 的互動邏輯
    /// </summary>
    public partial class DownloadWindow : Window
    {
        public DownloadWindow()
        {
            InitializeComponent();
            WindowStartupLocation = WindowStartupLocation.CenterScreen; //居中顯示
        }

        public string DownPath { get; set; }
        public string SavePath { get; set; }

        /// <summary>
        /// 等到視窗被Show出來後執行下載
        /// </summary>
        /// <param name="sender">委託</param>
        /// <param name="e">路由事件</param>
        private void DownloadWindow_Loaded(object sender, RoutedEventArgs e)
        {
            DownloadService(DownPath, SavePath);
        }

        /// <summary>
        /// 非同步下載 & 顯示進度
        /// </summary>
        /// <param name="downPath">來源網址</param>
        /// <param name="savePath">儲存位置</param>
        private void DownloadService(string downPath, string savePath)
        {
            using (WebClient wc = new WebClient())
            {
                try
                {
                    wc.DownloadFileCompleted += new AsyncCompletedEventHandler(wc_DownloadFileCompleted);
                    wc.DownloadProgressChanged += new DownloadProgressChangedEventHandler(wc_DownloadProgressChanged);
                    wc.DownloadFileAsync(new Uri(downPath), savePath);
                }
                catch (Exception ex)
                {
                    System.Windows.MessageBox.Show(ex.Message.ToString());
                }
            }
        }

        private void wc_DownloadFileCompleted(object sender, AsyncCompletedEventArgs e)
        {
            Height = 80;
            btn_DLfinish.Visibility = Visibility.Visible;
        }

        /// <summary>
        /// 更新進度UI狀態
        /// </summary>
        /// <param name="sender">委託</param>
        /// <param name="e">下載進度變更事件</param>
        private void wc_DownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            Dispatcher.BeginInvoke(new Action(delegate () {
                pb_DownLoad.Minimum = 0;
                pb_DownLoad.Maximum = (int)e.TotalBytesToReceive;
                pb_DownLoad.Value = (int)e.BytesReceived;
                lb_Percent.Content = e.ProgressPercentage + "%";
            }));
        }

        /// <summary>
        /// 點下載完成
        /// </summary>
        /// <param name="sender">委託</param>
        /// <param name="e">路由事件</param>
        private void Btn_DLfinish_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
