﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Windows;
using System.Windows.Input;

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
            }
            catch
            {
                File.Delete(App.Root + "\\UserSetting.ini");
                System.Windows.MessageBox.Show("設定讀取失敗" + Environment.NewLine + "執行部分設定初始化...", "異常");
            }

            WebServices.CheckVersion(App.CurrentVersion);
        }

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
            App.Set_cb_AllowEnter = cb_AllowEnter.IsChecked.ToString();

            /* 結束程式時保存設定 */
            LoadSetting.SaveSetting();
        }

        private void Btn_Load_txt_list_Click(object sender, RoutedEventArgs e)
        {
            // 開啟選擇檔案視窗
            Microsoft.Win32.OpenFileDialog openFileDialog = new Microsoft.Win32.OpenFileDialog();
            openFileDialog.InitialDirectory = App.Root;
            openFileDialog.Filter = "(*.db;*.txt;*.xls;*.xlsx)|*.db;*.txt;*.xls;*.xlsx";

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

        private void Btn_Default_db_Click(object sender, RoutedEventArgs e)
        {
            string DefaultFileName = "Default.db";

            if (File.Exists(DefaultFileName))
            {
                lb_Database_name.Content = DefaultFileName;
                try
                {
                    var stream = new StreamReader(new MemoryStream(GZip.Decompress(File.ReadAllBytes(DefaultFileName))));
                    Database.Load.txt_list(stream.ReadToEnd());
                }
                catch (Exception ex)
                {
                    System.Windows.MessageBox.Show(ex.Message.ToString());
                }
                Control.Lv_res_list_autowidth();
            }
            else
            {
                System.Windows.MessageBox.Show("找不到" + DefaultFileName, "提示");
            }
        }

        private void Btn_test_start_Click(object sender, RoutedEventArgs e)
        {
            if (App.LRC.Count > 0)
            {
                btn_test_start.Visibility = Visibility.Hidden;
                btn_test_stop.Visibility = Visibility.Visible;

                Control.Test(true);
                tb_Answer.Focus();

                List<int> listLinq = new List<int>(Enumerable.Range(0, App.LRC.Count));

                if (lv_res_list.HasItems != false)
                {
                    if (rb_Order.IsChecked == true)
                    {
                        App.ResultList = listLinq.OrderBy(o => o).ToList();
                    }
                    else if (rb_Random.IsChecked == true)
                    {
                        Random GetRandomInt = new Random(Guid.NewGuid().GetHashCode());
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

        private void Btn_test_stop_Click(object sender, RoutedEventArgs e)
        {
            btn_test_start.Visibility = Visibility.Visible;
            btn_test_stop.Visibility = Visibility.Hidden;

            Control.ClearText();
            Control.Again(false);
            Control.Test(false);
        }

        private void Btn_Answer_Click(object sender, RoutedEventArgs e)
        {
            tb_Answer.Focus();

            if (rb_Answer_Eng.IsChecked == true)
            {
                if (Database.Question.CleanInput(tb_Answer.Text.ToLower()) == Database.Question.CleanInput(App.LRC[App.ResultList[App.Index]].NameEng.ToLower()))
                {
                    Control.ClearText();
                    lb_AnswerCheck.Content = "正確!";
                    App.LRC[App.ResultList[App.Index]].Proficiency++;
                    Control.Set_Lv_res_list();
                    App.Index++;
                    Database.Question.NewQuestion();
                }
                else
                {
                    App.LRC[App.ResultList[App.Index]].Proficiency--;
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
                if (Database.Question.CleanInput(tb_Answer.Text) == Database.Question.CleanInput(App.LRC[App.ResultList[App.Index]].NameCht))
                {
                    Control.ClearText();
                    lb_AnswerCheck.Content = "正確!";
                    App.LRC[App.ResultList[App.Index]].Proficiency++;
                    Control.Set_Lv_res_list();
                    App.Index++;
                    Database.Question.NewQuestion();
                }
                else
                {
                    App.LRC[App.ResultList[App.Index]].Proficiency--;
                    Control.Set_Lv_res_list();
                    lb_Again_count.Content = App.Again_Count;
                    lb_AnswerCheck.Content = "錯誤!" + Environment.NewLine + "正確答案為 " + App.LRC[App.ResultList[App.Index]].NameCht;
                    Control.Test(false);
                    tb_Again.Focus();
                }
            }
        }

        private void Btn_Again_Click(object sender, RoutedEventArgs e)
        {
            tb_Again.Focus();

            int AgainTimes = 3;
            try
            {
                AgainTimes = Convert.ToInt32(tb_Again_times.Text);
            }
            catch(Exception ex)
            {
                System.Windows.MessageBox.Show(ex.Message.ToString());
                tb_Again_times.Text = "3";
            }

            if (rb_Answer_Eng.IsChecked == true)
            {
                if (Database.Question.CleanInput(tb_Again.Text.ToLower()) == Database.Question.CleanInput(App.LRC[App.ResultList[App.Index]].NameEng.ToLower()))
                {
                    if (App.Again_Count < AgainTimes)
                    {
                        App.Again_Count++;
                        lb_Again_count.Content = App.Again_Count;
                        tb_Again.Text = string.Empty;

                        if (App.Again_Count == AgainTimes)
                        {
                            Control.EndExercise();
                            Database.Question.NewQuestion();
                            tb_Answer.Focus();
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
                if (Database.Question.CleanInput(tb_Again.Text) == Database.Question.CleanInput(App.LRC[App.ResultList[App.Index]].NameCht))
                {
                    if (App.Again_Count < AgainTimes)
                    {
                        App.Again_Count++;
                        lb_Again_count.Content = App.Again_Count;
                        tb_Again.Text = string.Empty;

                        if (App.Again_Count == AgainTimes)
                        {
                            Control.EndExercise();
                            Database.Question.NewQuestion();
                            tb_Answer.Focus();
                        }
                    }
                }
                else
                {
                    tb_Again.Text = string.Empty;
                }
            }
        } 

        private void Tb_Answer_KeyDown(object sender, KeyEventArgs e)
        {
            //讀到輸入Enter 且 允許以Enter送出
            if (e.Key == Key.Enter && cb_AllowEnter.IsChecked == true)
            {
                //模擬點擊 送出鍵
                Btn_Answer_Click(sender, e);
            }
        }

        private void Tb_Again_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            //讀到輸入Enter 且 允許以Enter送出
            if (e.Key == Key.Enter && cb_AllowEnter.IsChecked == true)
            {
                //模擬點擊 送出鍵
                Btn_Again_Click(sender, e);
            }
        }

        private void Mi_about_Click(object sender, RoutedEventArgs e)
        {
            AboutWindow aboutWindow = new AboutWindow();
            aboutWindow.Show();
        }
    }
}