using System;
using System.Windows;

namespace 一丙英文背起來
{
    /// <summary>
    /// Update.xaml 的互動邏輯
    /// </summary>
    public partial class UpdateWindow : Window
    {
        public UpdateWindow()
        {
            InitializeComponent();
            WindowStartupLocation = WindowStartupLocation.CenterScreen; //居中顯示
        }

        string DownLoadUrl;
        /// <summary>
        /// 設定顯示的內容
        /// </summary>
        /// <param name="ServerVersion">更新版本</param>
        /// <param name="ReleaseUrl">下載網址</param>
        /// <param name="ReleaseDate">版本釋出日期</param>
        public void SetUpdateLabel(string ServerVersion, string ReleaseUrl, string ReleaseDate)
        {
            lb_ServerVersion.Content += ServerVersion;
            lb_ReleaseDate.Content += ReleaseDate;
            DownLoadUrl = ReleaseUrl;
        }

        /// <summary>
        /// 更新視窗關閉時恢復主視窗顯示
        /// </summary>
        /// <param name="sender">委託</param>
        /// <param name="e">事件</param>
        private void UpdateWindow_Closed(object sender, EventArgs e)
        {
            Control.MainVisibility(true);
        }

        /// <summary>
        /// 開始更新
        /// </summary>
        /// <param name="sender">委託</param>
        /// <param name="e">路由事件</param>
        private void Btn_update_Click(object sender, RoutedEventArgs e)
        {
            WebServices.DownLoadFile(DownLoadUrl, App.Root, false);
            Close();
        }

        /// <summary>
        /// 忽略更新
        /// </summary>
        /// <param name="sender">委託</param>
        /// <param name="e">路由事件</param>
        private void Btn_ignore_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}