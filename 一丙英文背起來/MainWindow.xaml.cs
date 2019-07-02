using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Windows;
using System.Windows.Input;
using System.Windows.Controls;

namespace 一丙英文背起來
{
    /// <summary>
    /// MainWindow.xaml 的互動邏輯
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            WindowStartupLocation = WindowStartupLocation.CenterScreen; //居中顯示

            LoadSetting.Init_Setting(); //讀取設定
            tb_Again_times.Text = App.Set_tb_Again_times;
            tb_pfclim.Text = App.Set_tb_pfclim;
            if (App.Set_rb_FileCovertExt.Equals(".xlsx"))
            {
                rb_fmt_xlsx.IsChecked = true;
            }
            else if (App.Set_rb_FileCovertExt.Equals(".txt"))
            {
                rb_fmt_txt.IsChecked = true;
            }
            try
            {
                if (Convert.ToBoolean(App.Set_rb_Answer_Eng) != true)
                {
                    rb_Answer_Cht.IsChecked = true;
                }
                if (Convert.ToBoolean(App.Set_rb_Order) != true)
                {
                    rb_Random.IsChecked = true;
                }
                if (Convert.ToBoolean(App.Set_cb_AllowEnter) != true)
                {
                    cb_AllowEnter.IsChecked = false;
                }
                if (Convert.ToBoolean(App.Set_cb_pfclim) != true)
                {
                    cb_pfclim.IsChecked = false;
                }
            }
            catch
            {
                File.Delete(App.Root + "\\UserSetting.ini");
                System.Windows.MessageBox.Show("設定讀取失敗" + Environment.NewLine + "執行部分設定初始化...", "異常");
            }

            WebServices.CheckVersion(App.CurrentVersion);
        }

        /// <summary>
        /// 主程式關閉時保存設定
        /// </summary>
        /// <param name="sender">委託</param>
        /// <param name="e">事件</param>
        private void Window_Closed(object sender, EventArgs e)
        {
            App.Set_rb_Answer_Eng = rb_Answer_Eng.IsChecked.ToString();
            App.Set_rb_Order = rb_Order.IsChecked.ToString();
            if (rb_fmt_xlsx.IsChecked == true)
            {
                App.Set_rb_FileCovertExt = ".xlxs";
            }
            else if (rb_fmt_txt.IsChecked == true)
            {
                App.Set_rb_FileCovertExt = ".txt";
            }
            else
            {
                App.Set_rb_FileCovertExt = ".db";
            }
            App.Set_tb_Again_times = tb_Again_times.Text;
            App.Set_tb_pfclim = tb_pfclim.Text;
            App.Set_cb_AllowEnter = cb_AllowEnter.IsChecked.ToString();
            App.Set_cb_pfclim = cb_pfclim.IsChecked.ToString();

            // 保存設定
            LoadSetting.SaveSetting();
        }

        /// <summary>
        /// 選擇題庫並載入列表
        /// </summary>
        /// <param name="sender">委託</param>
        /// <param name="e">路由事件</param>
        private void Btn_Load_txt_list_Click(object sender, RoutedEventArgs e)
        {
            // 開啟選擇檔案視窗
            Microsoft.Win32.OpenFileDialog openFileDialog = new Microsoft.Win32.OpenFileDialog();
            openFileDialog.InitialDirectory = App.Root;
            openFileDialog.Filter = "(*.db;*.txt;*.xls;*.xlsx)|*.db;*.txt;*.xls;*.xlsx";

            // 如果視窗開啟有成功
            if (openFileDialog.ShowDialog() == true)
            {
                App.FileRoot = openFileDialog.FileName;
                lb_Database_name.Content = Path.GetFileName(App.FileRoot);
                string FileExt = Path.GetExtension(App.FileRoot);

                if (FileExt.Equals(".txt"))
                {
                    Database.Load.txt_list(File.ReadAllText(App.FileRoot));
                }
                else if (FileExt.Equals(".db"))
                {
                    try
                    {
                        // db副檔名的題庫要先用GZIP解壓，再讀取TEXT內容
                        StreamReader stream = new StreamReader(new MemoryStream(GZip.Decompress(File.ReadAllBytes(App.FileRoot))));
                        Database.Load.txt_list(stream.ReadToEnd());
                    }
                    catch (Exception ex)
                    {
                        System.Windows.MessageBox.Show(ex.Message.ToString());
                    }
                }
                else if (FileExt.Equals(".xls") || FileExt.Equals(".xlsx"))
                {
                    // 注意: 執行之電腦必須有安裝Excel才能使用
                    Database.Load.Excel_list(App.FileRoot);
                }
                Control.Lv_res_list_autowidth();
            }
        }

        /// <summary>
        /// 導出當前題庫的內容，並以選擇的格式保存
        /// </summary>
        /// <param name="sender">委託</param>
        /// <param name="e">路由事件</param>
        private void Btn_FileCovert_Click(object sender, RoutedEventArgs e)
        {
            if (lb_Database_name.Content.ToString() != string.Empty)
            {
                if (cb_FileCovert_delete.IsChecked == true)
                {
                    File.Delete(App.FileRoot);
                }

                // 取得不包含附檔名之檔案路徑
                string path = App.FileRoot.Replace(Path.GetExtension(App.FileRoot), string.Empty);

                if (rb_fmt_db.IsChecked == true)
                {
                    Database.Save.As_db(path);
                }
                else if (rb_fmt_txt.IsChecked == true)
                {
                    Database.Save.As_txt(path);
                }
                else if (rb_fmt_xlsx.IsChecked == true)
                {
                    Database.Save.As_excel(path);
                }
            }
            else
            {
                Control.ShowAutoClosingMessageBox("資料庫沒有任何資料");
            }
        }

        /// <summary>
        /// 載入默認的db檔案(Default.db)
        /// </summary>
        /// <param name="sender">委託</param>
        /// <param name="e">路由事件</param>
        private void Btn_Default_db_Click(object sender, RoutedEventArgs e)
        {
            string DefaultFileName = "Default.db";

            if (File.Exists(DefaultFileName))
            {
                lb_Database_name.Content = DefaultFileName;
                try
                {
                    // db副檔名的題庫要先用GZIP解壓，再讀取TEXT內容
                    var stream = new StreamReader(new MemoryStream(GZip.Decompress(File.ReadAllBytes(DefaultFileName))));
                    Database.Load.txt_list(stream.ReadToEnd());
                }
                catch (Exception ex)
                {
                    System.Windows.MessageBox.Show(ex.Message.ToString());
                }
                // 依據讀取的內容調整欄位寬度
                Control.Lv_res_list_autowidth();
            }
            else
            {
                System.Windows.MessageBox.Show("找不到" + DefaultFileName, "提示");
            }
        }

        /// <summary>
        /// 開始測驗
        /// </summary>
        /// <param name="sender">委託</param>
        /// <param name="e">路由事件</param>
        private void Btn_test_start_Click(object sender, RoutedEventArgs e)
        {
            // 如果列表有資料
            if (App.LRC.Count > 0)
            {
                // 開始測驗按鈕 -> 結束測驗按鈕
                btn_test_start.Visibility = Visibility.Hidden;
                btn_test_stop.Visibility = Visibility.Visible;

                Control.Test(true);
                tb_Answer.Focus();  // 跳至輸入框

                // 依列表的題數依序產生一整數陣列
                List<int> listLinq = new List<int>(Enumerable.Range(0, App.LRC.Count));

                if (lv_res_list.HasItems != false)
                {
                    if (rb_Order.IsChecked == true)
                    {
                        // 將這個陣列交給全域變數，用來作為索引值
                        App.ResultList = listLinq.ToList();
                    }
                    else if (rb_Random.IsChecked == true)
                    {
                        // 利用產生的GUID產生雜湊值後做為亂數種子
                        Random GetRandomInt = new Random(Guid.NewGuid().GetHashCode());
                        // 將這個陣列，以亂數排序後，交給全域變數，用來作為索引值
                        App.ResultList = listLinq.OrderBy(o => GetRandomInt.Next()).ToList();
                    }
                    Database.Question.NewQuestion();
                }
            }
            else
            {
                System.Windows.MessageBox.Show("請載入題庫","提示");
                tbc_main.SelectedItem = tbi_db;
            }
        }

        /// <summary>
        /// 結束測驗
        /// </summary>
        /// <param name="sender">委託</param>
        /// <param name="e">路由事件</param>
        private void Btn_test_stop_Click(object sender, RoutedEventArgs e)
        {
            // 結束測驗按鈕 -> 開始測驗按鈕
            btn_test_start.Visibility = Visibility.Visible;
            btn_test_stop.Visibility = Visibility.Hidden;

            Control.ClearText();
            Control.Again(false);
            Control.Test(false);
        }

        /// <summary>
        /// 送出回答的答案
        /// </summary>
        /// <param name="sender">委託</param>
        /// <param name="e">路由事件</param>
        private void Btn_Answer_Click(object sender, RoutedEventArgs e)
        {
            tb_Answer.Focus(); // 跳至輸入框

            int pfclim = 3;
            try
            {
                // 取得設定的熟練度
                pfclim = Convert.ToInt32(tb_pfclim.Text);
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show(ex.Message.ToString());
                tb_pfclim.Text = "3";
            }

            if (rb_Answer_Eng.IsChecked == true)
            {
                // 全部轉小寫、清除標點後進行比對
                if (Database.Question.CleanInput(tb_Answer.Text.ToLower()) == Database.Question.CleanInput(App.LRC[App.ResultList[App.Index]].NameEng.ToLower()))
                {
                    Control.ClearText();
                    lb_AnswerCheck.Content = "正確!";
                    if (Math.Abs(App.LRC[App.ResultList[App.Index]].Proficiency) < Math.Abs(pfclim))
                    {
                        // 熟練度提升
                        App.LRC[App.ResultList[App.Index]].Proficiency++;
                    }
                    Control.Set_Lv_res_list();
                    
                    do{
                        App.Index++;
                        if (App.Index > App.ResultList.Count - 1)
                        {
                            break;
                        }
                    } while (App.LRC[App.ResultList[App.Index]].Proficiency >= pfclim && cb_pfclim.IsChecked == true) ;

                    if (!Database.Question.NewQuestion())
                    {
                        Btn_test_stop_Click(sender, e);
                    }
                }
                else
                {
                    if (Math.Abs(App.LRC[App.ResultList[App.Index]].Proficiency) < Math.Abs(pfclim))
                    {
                        // 熟練度下降
                        App.LRC[App.ResultList[App.Index]].Proficiency--;
                    }
                    Control.Set_Lv_res_list();
                    lb_AnswerCheck.Content = "錯誤!" + Environment.NewLine + "正確答案為 " + App.LRC[App.ResultList.ToList()[App.Index]].NameEng;
                    lb_Again_count.Content = App.Again_Count;
                    Control.Test(false);
                    Control.Again(true);
                    tb_Again.Focus();
                }
            }
            else if (rb_Answer_Cht.IsChecked == true)
            {
                // 清除標點後進行比對
                if (Database.Question.CleanInput(tb_Answer.Text) == Database.Question.CleanInput(App.LRC[App.ResultList[App.Index]].NameCht))
                {
                    Control.ClearText();
                    lb_AnswerCheck.Content = "正確!";
                    if (Math.Abs(App.LRC[App.ResultList[App.Index]].Proficiency) < Math.Abs(pfclim))
                    {
                        // 熟練度提升
                        App.LRC[App.ResultList[App.Index]].Proficiency++;
                    }
                    Control.Set_Lv_res_list();

                    do
                    {
                        App.Index++;
                        if (App.Index > App.ResultList.Count - 1)
                        {
                            break;
                        }
                    } while (App.LRC[App.ResultList[App.Index]].Proficiency >= pfclim && cb_pfclim.IsChecked == true);

                    if (!Database.Question.NewQuestion())
                    {
                        Btn_test_stop_Click(sender, e);
                    }
                }
                else
                {
                    if (Math.Abs(App.LRC[App.ResultList[App.Index]].Proficiency) < Math.Abs(pfclim))
                    {
                        // 熟練度下降
                        App.LRC[App.ResultList[App.Index]].Proficiency--;
                    }
                    Control.Set_Lv_res_list();
                    lb_Again_count.Content = App.Again_Count;
                    lb_AnswerCheck.Content = "錯誤!" + Environment.NewLine + "正確答案為 " + App.LRC[App.ResultList[App.Index]].NameCht;
                    Control.Test(false);
                    tb_Again.Focus(); // 跳至輸入框
                }
            }
        }

        /// <summary>
        /// 送出練習的答案
        /// </summary>
        /// <param name="sender">委託</param>
        /// <param name="e">路由事件</param>
        private void Btn_Again_Click(object sender, RoutedEventArgs e)
        {
            tb_Again.Focus(); // 跳至輸入框

            int AgainTimes = 3;
            try
            {
                // 取得設定的練習次數
                AgainTimes = Convert.ToInt32(tb_Again_times.Text);
            }
            catch(Exception ex)
            {
                System.Windows.MessageBox.Show(ex.Message.ToString());
                tb_Again_times.Text = "3";
            }

            if (rb_Answer_Eng.IsChecked == true)
            {
                // 全部轉小寫、清除標點後進行比對
                if (Database.Question.CleanInput(tb_Again.Text.ToLower()) == Database.Question.CleanInput(App.LRC[App.ResultList[App.Index]].NameEng.ToLower()))
                {
                    if (App.Again_Count < AgainTimes)
                    {
                        // 完成一次練習
                        App.Again_Count++;
                        lb_Again_count.Content = App.Again_Count;
                        tb_Again.Text = string.Empty;

                        if (App.Again_Count == AgainTimes)
                        {
                            Control.EndExercise();
                            Database.Question.NewQuestion();
                            tb_Answer.Focus(); // 跳至輸入框
                        }
                    }
                }
                else
                {
                    tb_Again.Text = string.Empty;
                }
            }
            else if (rb_Answer_Cht.IsChecked == true)
            {
                // 清除標點後進行比對
                if (Database.Question.CleanInput(tb_Again.Text) == Database.Question.CleanInput(App.LRC[App.ResultList[App.Index]].NameCht))
                {
                    if (App.Again_Count < AgainTimes)
                    {
                        // 完成一次練習
                        App.Again_Count++;
                        lb_Again_count.Content = App.Again_Count;
                        tb_Again.Text = string.Empty;

                        if (App.Again_Count == AgainTimes)
                        {
                            Control.EndExercise();
                            Database.Question.NewQuestion();
                            tb_Answer.Focus(); // 跳至輸入框
                        }
                    }
                }
                else
                {
                    tb_Again.Text = string.Empty;
                }
            }
        }

        /// <summary>
        /// 偵測輸入框的輸入內容
        /// </summary>
        /// <param name="sender">委託</param>
        /// <param name="e">路由事件</param>
        private void Tb_Answer_KeyDown(object sender, KeyEventArgs e)
        {
            //讀到輸入Enter 且 允許以Enter送出
            if (e.Key == Key.Enter && cb_AllowEnter.IsChecked == true)
            {
                //模擬點擊 送出鍵
                Btn_Answer_Click(sender, e);
            }
        }

        /// <summary>
        /// 偵測輸入框的輸入內容
        /// </summary>
        /// <param name="sender">委託</param>
        /// <param name="e">路由事件</param>
        private void Tb_Again_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            //讀到輸入Enter 且 允許以Enter送出
            if (e.Key == Key.Enter && cb_AllowEnter.IsChecked == true)
            {
                //模擬點擊 送出鍵
                Btn_Again_Click(sender, e);
            }
        }

        /// <summary>
        /// 關於這個程式
        /// </summary>
        /// <param name="sender">委託</param>
        /// <param name="e">路由事件</param>
        private void Mi_about_Click(object sender, RoutedEventArgs e)
        {
            AboutWindow aboutWindow = new AboutWindow();
            aboutWindow.Show();
        }

        /// <summary>
        /// 歸零熟練度
        /// </summary>
        /// <param name="sender">委託</param>
        /// <param name="e">路由事件</param>
        private void Btn_clrpfs_Click(object sender, RoutedEventArgs e)
        {
            for ( int i = 0; i < App.LRC.Count; i++)
            {
                App.LRC[App.ResultList[App.Index]].Proficiency = 0;
            }
            Control.Set_Lv_res_list();
        }

        /// <summary>
        /// 網頁加載完成
        /// </summary>
        /// <param name="sender">委託</param>
        /// <param name="e">瀏覽事件</param>
        private void Wb_live2d_LoadCompleted(object sender, System.Windows.Navigation.NavigationEventArgs e)
        {
            string script1 = "document.body.style.overflow ='hidden'";     //關閉Scrollbar
            string script2 = "document.body.style.zoom ='50%'";            //調整網頁內容大小
            WebBrowser wb = (WebBrowser)sender;
            wb.InvokeScript("execScript", new Object[] { script1, "JavaScript" });
            wb.InvokeScript("execScript", new Object[] { script2, "JavaScript" });
        }

        /// <summary>
        /// Live2D選擇黑貓
        /// </summary>
        /// <param name="sender">委託</param>
        /// <param name="e">路由事件</param>
        private void Rb_Live2d_nekob_Checked(object sender, RoutedEventArgs e)
        {
            Callneko("http://28598519a.github.io/l2d-neko-black");
        }

        /// <summary>
        /// Live2D選擇白貓
        /// </summary>
        /// <param name="sender">委託</param>
        /// <param name="e">路由事件</param>
        private void Rb_Live2d_nekow_Checked(object sender, RoutedEventArgs e)
        {
            Callneko("http://28598519a.github.io/l2d-neko-white");
        }

        /// <summary>
        /// 嘗試加載網頁內容
        /// </summary>
        /// <param name="url">網址</param>
        private void Callneko(string url)
        {
            if (WebServices.WebRequestTest(url))
            {
                wb_live2d.Navigate(url);
                wb_live2d.Visibility = Visibility.Visible;
            }
            else
            {
                wb_live2d.Visibility = Visibility.Collapsed;
            }
        }
    }
}