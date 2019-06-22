using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Windows;

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
            try
            {
                tb_Again_times.Text = App.Set_tb_Again_times;
                cb_FileCovert.IsChecked = Convert.ToBoolean(App.Set_cb_FileCovert);

                if (Convert.ToBoolean(App.Set_rb_Answer_Eng) != true)
                {
                    rb_Answer_Eng.IsChecked = false;
                    rb_Answer_Cht.IsChecked = true;
                }
                if (Convert.ToBoolean(App.Set_rb_Order) != true)
                {
                    rb_Order.IsChecked = false;
                    rb_Random.IsChecked = true;
                }
            }
            catch
            {
                File.Delete(App.Root + "\\UserSetting.ini");
                System.Windows.MessageBox.Show("設定讀取失敗" + Environment.NewLine + "執行設定初始化...", "異常");
            }
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            App.Set_rb_Answer_Eng = rb_Answer_Eng.IsChecked.ToString();
            App.Set_rb_Order = rb_Order.IsChecked.ToString();
            App.Set_tb_Again_times = tb_Again_times.Text;
            App.Set_cb_FileCovert = cb_FileCovert.IsChecked.ToString();

            /* 結束程式時保存設定 */
            LoadSetting.SaveSetting();
        }

        private void NewQuestion()
        {
            if (rb_Answer_Eng.IsChecked == true)
                lb_Question.Content = App.LRC[App.ResultList.ToList()[App.Index]].NameCht;
            else lb_Question.Content = App.LRC[App.ResultList.ToList()[App.Index]].NameEng;
        }

        private void Btn_load_res_list_Click(object sender, RoutedEventArgs e)
        {
            /* 開啟選擇檔案視窗 */
            Microsoft.Win32.OpenFileDialog openFileDialog = new Microsoft.Win32.OpenFileDialog();
            openFileDialog.InitialDirectory = App.Root;
            openFileDialog.Filter = "(*.txt;*.db)|*.txt;*.db";

            if (openFileDialog.ShowDialog() == true)
            {
                lb_Database_name.Content = Path.GetFileName(openFileDialog.FileName);

                if (Path.GetExtension(openFileDialog.FileName) == ".txt")
                {
                    Database.Load_res_list(File.ReadAllText(openFileDialog.FileName));

                    if (cb_FileCovert.IsChecked == true)
                    {
                        File.WriteAllBytes(Path.GetFileNameWithoutExtension(openFileDialog.FileName) + ".db", GZip.Compress(File.ReadAllBytes(openFileDialog.FileName)));

                        if (cb_FileCovert_delete.IsChecked == true)
                        {
                            File.Delete(openFileDialog.FileName);
                        }
                    }
                }
                else if (Path.GetExtension(openFileDialog.FileName) == ".db")
                {
                    try
                    {
                        var stream = new StreamReader(new MemoryStream(GZip.Decompress(File.ReadAllBytes(openFileDialog.FileName))));
                        Database.Load_res_list(stream.ReadToEnd());
                    }
                    catch (Exception ex)
                    {
                        System.Windows.MessageBox.Show(ex.ToString());
                    }
                }
            }
        }

        private void Btn_Default_db_Click(object sender, RoutedEventArgs e)
        {
            lb_Database_name.Content = "Database.db";

            try
            {
                var stream = new StreamReader(new MemoryStream(GZip.Decompress(File.ReadAllBytes("Database.db"))));
                Database.Load_res_list(stream.ReadToEnd());
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show(ex.ToString());
            }
        }

        private void Btn_test_start_Click(object sender, RoutedEventArgs e)
        {
            if (App.LRC.Count > 0)
            {
                btn_test_start.Visibility = Visibility.Hidden;
                btn_test_stop.Visibility = Visibility.Visible;

                Control.Test(true);

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

                    NewQuestion();
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
            if (rb_Answer_Eng.IsChecked == true)
            {
                if (Database.CleanInput(tb_Answer.Text.ToLower()) == Database.CleanInput(App.LRC[App.ResultList[App.Index]].NameEng.ToLower()))
                {
                    Control.ClearText();
                    lb_AnswerCheck.Content = "正確!";
                    App.Index++;
                    NewQuestion();
                }
                else
                {
                    lb_AnswerCheck.Content = "錯誤!" + Environment.NewLine + "正確答案為 " + App.LRC[App.ResultList.ToList()[App.Index]].NameEng;
                    Control.Test(false);
                    Control.Again(true);
                }
            }
            else if (rb_Answer_Cht.IsChecked == true)
            {
                if (Database.CleanInput(tb_Answer.Text) == Database.CleanInput(App.LRC[App.ResultList[App.Index]].NameCht))
                {
                    Control.ClearText();
                    lb_AnswerCheck.Content = "正確!";
                    App.Index++;
                    NewQuestion();
                }
                else
                {
                    Control.Test(false);
                    lb_AnswerCheck.Content = "錯誤!" + Environment.NewLine + "正確答案為 " + App.LRC[App.ResultList[App.Index]].NameCht;
                }
            }
        }

        private void Btn_Again_Click(object sender, RoutedEventArgs e)
        {
            if (rb_Answer_Eng.IsChecked == true)
            {
                if (Database.CleanInput(tb_Again.Text.ToLower()) == Database.CleanInput(App.LRC[App.ResultList[App.Index]].NameEng.ToLower()))
                {
                    if (App.Again_Count < Convert.ToInt32(tb_Again_times.Text))
                    {
                        App.Again_Count++;
                        lb_Again_count.Content = App.Again_Count;
                        tb_Again.Text = "";

                        if (App.Again_Count == Convert.ToInt32(tb_Again_times.Text))
                        {
                            Control.EndExercise();
                            NewQuestion();
                        }
                    }
                }
                else
                {
                    tb_Again.Text = "";
                }
            }
            else if (rb_Answer_Cht.IsChecked == true)
            {
                if (Database.CleanInput(tb_Again.Text) == Database.CleanInput(App.LRC[App.ResultList[App.Index]].NameCht))
                {
                    if (App.Again_Count < Convert.ToInt32(tb_Again_times.Text))
                    {
                        App.Again_Count++;
                        lb_Again_count.Content = App.Again_Count;
                        tb_Again.Text = "";

                        if (App.Again_Count == Convert.ToInt32(tb_Again_times.Text))
                        {
                            Control.EndExercise();
                            NewQuestion();
                        }
                    }
                }
                else
                {
                    tb_Again.Text = "";
                }
            }            
        }

        private void Cb_FileCovert_Checked(object sender, RoutedEventArgs e)
        {
            cb_FileCovert_delete.IsEnabled = true;
        }

        private void Cb_FileCovert_Unchecked(object sender, RoutedEventArgs e)
        {
            cb_FileCovert_delete.IsChecked = false;
            cb_FileCovert_delete.IsEnabled = false;
        }
    }
}