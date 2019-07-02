using System;
using System.Diagnostics;
using System.IO;
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
        public string ProgramName { get; set; }

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
            string name = WebServices.DownLoadFile(DownLoadUrl, Path.Combine(App.Root, "cache"), true);
            string parentfolder = Directory.GetParent(App.Root).ToString();
            Compress.Zip.Decompress(Path.Combine(App.Root, "cache", name), parentfolder);

            StartProcess(Path.Combine(parentfolder, Path.GetFileNameWithoutExtension(name)));
            StartProcess(Path.Combine(parentfolder, Path.GetFileNameWithoutExtension(name), ProgramName));
            DeletExeFolder();
            Environment.Exit(Environment.ExitCode);
        }

        /// <summary>
        /// 啟動進程
        /// </summary>
        /// <param name="filename">完整程式路徑</param>
        /// <param name="show">視窗顯示</param>
        /// <param name="arguments">附加參數</param>
        private void StartProcess(string filename, ProcessWindowStyle show = ProcessWindowStyle.Normal, string arguments = "")
        {
            Process process = new Process();
            process.StartInfo.FileName = filename;
            process.StartInfo.WorkingDirectory = Path.GetDirectoryName(filename);
            process.StartInfo.Arguments = arguments;
            process.StartInfo.WindowStyle = show;
            try
            {
                process.Start();
            }
            catch
            {
                //進程創建失敗
                process.Dispose();
            }
        }

        public void DeletExeFolder()
        {
            string fileName = Path.GetTempPath() + "remove.cmd";
            StreamWriter cmd = new StreamWriter(fileName, false, System.Text.Encoding.Default);
            string exePath = App.Root;

            cmd.WriteLine("timeout /t 2");
            cmd.WriteLine(string.Format("rd \"{0}\" /s /q", exePath));
            cmd.WriteLine(string.Format("del \"{0}\" /q", fileName));

            cmd.Close();

            StartProcess(fileName, ProcessWindowStyle.Hidden);
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