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
        public void SetUpdateLabel(string ServerVersion, string ReleaseUrl, string ReleaseDate)
        {
            lb_ServerVersion.Content += ServerVersion;
            lb_ReleaseDate.Content += ReleaseDate;
            DownLoadUrl = ReleaseUrl;
        }

        private void UpdateWindow_Closed(object sender, EventArgs e)
        {
            Control.MainVisibility(true);
        }

        private void Btn_update_Click(object sender, RoutedEventArgs e)
        {
            WebServices.DownLoadFile(DownLoadUrl, App.Root, false);
            Close();
        }

        private void Btn_ignore_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}