using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.IO;
using System.Text.RegularExpressions;

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
            LoadSetting.Init_Setting();
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            /* 結束程式時保存DataBase */
            LoadSetting.SaveSetting();
        }

        private void NewQuestion()
        {
            if (rb_Answer_Eng.IsChecked == true)
                lb_Question.Content = App.LRC[App.ResultList.ToList()[App.Index]].NameCht;
            else lb_Question.Content = App.LRC[App.ResultList.ToList()[App.Index]].NameEng;
        }

        private void Load_res_list(string fileText)
        {
            App.LRC.Clear();
            lv_res_list.ItemsSource = null;

            try
            {
                //lb_dabaotimetxt.Content = Regex.Match(fileText, "daBaoTime = \"[^\"]+\"").Value.Replace("daBaoTime = ", "").Trim('"');

                MatchCollection chtWords = Regex.Matches(fileText, "cht = \"[^\"]+\"");
                MatchCollection engWords = Regex.Matches(fileText, "eng = \"[^\"]+\"");

                foreach (Match mt in chtWords)
                {
                    string chtWordsName = mt.Value.Replace("cht = ", "").Trim('"');
                    App.LRC.Add(new ResCless { NameCht = chtWordsName, NameEng = "" });
                }

                int i = 0;
                foreach (Match mt in engWords)
                {
                    if (i > App.LRC.Count - 1)
                        break;

                    string engWordsName = mt.Value.Replace("eng = ", "").Trim('"');
                    App.LRC[i].NameEng = engWordsName;
                    i++;
                }
                lv_res_list.ItemsSource = App.LRC;
                lb_lsCount.Content = App.LRC.Count;
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show(ex.ToString());
            }
        }

        private void btn_load_res_list_Click(object sender, RoutedEventArgs e)
        {
            /* 開啟選擇檔案視窗 */
            Microsoft.Win32.OpenFileDialog openFileDialog = new Microsoft.Win32.OpenFileDialog();
            openFileDialog.InitialDirectory = App.Root;

            if (openFileDialog.ShowDialog() == true)
            {
                Load_res_list(File.ReadAllText(openFileDialog.FileName));
            }
        }

        private void btn_Default_db_Click(object sender, RoutedEventArgs e)
        {
            //File.WriteAllBytes("Database.db", GZip.Compress(File.ReadAllBytes("Database.db")));
            //File.WriteAllBytes("Database.txt", GZip.Decompress(File.ReadAllBytes("Database.db")));
            try
            {
                var stream = new StreamReader(new MemoryStream(GZip.Decompress(File.ReadAllBytes("Database.db"))));
                Load_res_list(stream.ReadToEnd());
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show(ex.ToString());
            }
        }

        private void btn_test_start_Click(object sender, RoutedEventArgs e)
        {
            if (App.LRC.Count > 0)
            {
                btn_test_start.Visibility = Visibility.Hidden;
                btn_test_stop.Visibility = Visibility.Visible;

                App.Control.Test(true);

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

        private void btn_test_stop_Click(object sender, RoutedEventArgs e)
        {
            btn_test_start.Visibility = Visibility.Visible;
            btn_test_stop.Visibility = Visibility.Hidden;

            App.Control.ClearText();
            App.Control.Exercise(false);
            App.Control.Test(false);
        }

        private void btn_Answer_Click(object sender, RoutedEventArgs e)
        {
            if (rb_Answer_Eng.IsChecked == true)
            {
                if (tb_Answer.Text.ToLower() == App.LRC[App.ResultList[App.Index]].NameEng.ToLower())
                {
                    App.Control.ClearText();
                    lb_AnswerCheck.Content = "正確!";
                    App.Index++;
                    NewQuestion();
                }
                else
                {
                    lb_AnswerCheck.Content = "錯誤!" + Environment.NewLine + "正確答案為 " + App.LRC[App.ResultList.ToList()[App.Index]].NameEng;
                    App.Control.Test(false);
                    App.Control.Exercise(true);
                }
            }
            else if (rb_Answer_Cht.IsChecked == true)
            {
                if (tb_Answer.Text == App.LRC[App.ResultList[App.Index]].NameCht)
                {
                    App.Control.ClearText();
                    lb_AnswerCheck.Content = "正確!";
                    App.Index++;
                    NewQuestion();
                }
                else
                {
                    App.Control.Test(false);
                    lb_AnswerCheck.Content = "錯誤!" + Environment.NewLine + "正確答案為 " + App.LRC[App.ResultList[App.Index]].NameCht;
                }
            }
        }

        private void btn_Again1_Click(object sender, RoutedEventArgs e)
        {
            if (rb_Answer_Eng.IsChecked == true)
            {
                if (tb_Again1.Text.ToLower() == App.LRC[App.ResultList[App.Index]].NameEng.ToLower())
                {
                    App.Control.Again1(false);
                }
                else
                {
                    tb_Again1.Text = "";
                }
            }
            else if (rb_Answer_Cht.IsChecked == true)
            {
                if (tb_Again1.Text == App.LRC[App.ResultList[App.Index]].NameCht)
                {
                    App.Control.Again1(false);
                }
                else
                {
                    tb_Again1.Text = "";
                }
            }            
        }

        private void btn_Again2_Click(object sender, RoutedEventArgs e)
        {
            if (rb_Answer_Eng.IsChecked == true)
            {
                if (tb_Again2.Text.ToLower() == App.LRC[App.ResultList[App.Index]].NameEng.ToLower())
                {
                    App.Control.Again2(false);
                }
                else
                {
                    tb_Again2.Text = "";
                }
            }
            else if (rb_Answer_Cht.IsChecked == true)
            {
                if (tb_Again1.Text.ToLower() == App.LRC[App.ResultList[App.Index]].NameCht.ToLower())
                {
                    App.Control.Again2(false);
                }
                else
                {
                    tb_Again2.Text = "";
                }
            }
        }

        private void btn_Again3_Click(object sender, RoutedEventArgs e)
        {
            if (rb_Answer_Eng.IsChecked == true)
            {
                if (tb_Again3.Text.ToLower() == App.LRC[App.ResultList[App.Index]].NameEng.ToLower())
                {
                    App.Control.Again3(false);
                    App.Control.ClearText();
                    App.Index++;
                    NewQuestion();
                    App.Control.Test(true);
                }
                else
                {
                    tb_Again3.Text = "";
                }
            }
            else if (rb_Answer_Cht.IsChecked == true)
            {
                if (tb_Again1.Text == App.LRC[App.ResultList[App.Index]].NameCht)
                {
                    App.Control.Again3(false);
                    App.Control.ClearText();
                    App.Index++;
                    NewQuestion();
                    App.Control.Test(true);
                }
                else
                {
                    tb_Again3.Text = "";
                }
            }
        }
    }
}