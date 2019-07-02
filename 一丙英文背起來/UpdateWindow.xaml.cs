using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;

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

        public void SetUpdateLabel(string ServerVersion, string ReleaseUrl)
        {
            lb_ServerVersion.Content += ServerVersion;
            tb_ReleaseUrl.Text = ReleaseUrl;
            hlk_ReleaseUrl.NavigateUri = new Uri(ReleaseUrl);
        }

        private void UpdateWindow_Closed(object sender, EventArgs e)
        {
            Control.MainVisibility(true);
        }

        private void Btn_ok_Click(object sender, RoutedEventArgs e)
        {
            Close();
            Control.MainVisibility(true);
        }

        private void Hlk_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Hyperlink link = sender as Hyperlink;
                Process.Start(new ProcessStartInfo(link.NavigateUri.AbsoluteUri));
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show(ex.Message.ToString());
            }
        }

        private void Hlk_MouseEnter(object sender, MouseEventArgs e)
        {
            Hyperlink link = (Hyperlink)e.OriginalSource;
            link.Foreground = Brushes.Red;
        }

        private void Hlk_MouseLeave(object sender, MouseEventArgs e)
        {
            Hyperlink link = (Hyperlink)e.OriginalSource;
            link.Foreground = Brushes.Blue;
        }
    }
}