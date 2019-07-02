using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;
using Excel = Microsoft.Office.Interop.Excel; //Define為Excel，以避免Application名稱衝突

namespace 一丙英文背起來
{
    public class Database
    {
        public static void NewQuestion()
        {
            if (App.Index >= App.ResultList.Count)
            {
                App.Index = 0;
                Random GetRandomInt = new Random(Guid.NewGuid().GetHashCode());
                App.ResultList = App.ResultList.OrderBy(o => GetRandomInt.Next()).ToList();
            }
            if (((MainWindow)Application.Current.MainWindow).rb_Answer_Eng.IsChecked == true)
                ((MainWindow)Application.Current.MainWindow).lb_Question.Content = App.LRC[App.ResultList.ToList()[App.Index]].NameCht;
            else ((MainWindow)Application.Current.MainWindow).lb_Question.Content = App.LRC[App.ResultList.ToList()[App.Index]].NameEng;
        }

        public static string CleanInput(string strIn)
        {
            /* \W 匹配任何非文字字元 (亦可用^\w) */
            try
            {
                return Regex.Replace(strIn, @"[\W]+", string.Empty, RegexOptions.None, TimeSpan.FromSeconds(1.5));
            }
            /* 替換時間長於1.5秒就放棄替換 */
            catch (RegexMatchTimeoutException)
            {
                //return string.Empty;
                return strIn;
            }
        }

        public class Load
        {
            public static void txt_list(string fileText)
            {
                App.LRC.Clear();
                ((MainWindow)Application.Current.MainWindow).lv_res_list.ItemsSource = null;

                try
                {
                    /* [^\"]+ 匹配任何除了 " 以外的字元 ; [0-9]匹配數字 */
                    MatchCollection chtWords = Regex.Matches(fileText, "cht = \"[^\"]+\"");
                    MatchCollection engWords = Regex.Matches(fileText, "eng = \"[^\"]+\"");
                    MatchCollection Proficiency = Regex.Matches(fileText, "pfc = \"[0-9]+\"");

                    foreach (Match mt in chtWords)
                    {
                        string chtWordsName = mt.Value.Replace("cht = ", string.Empty).Trim('"');
                        App.LRC.Add(new ResCless { NameCht = chtWordsName, NameEng = string.Empty, Proficiency = string.Empty });
                    }

                    int i = 0;
                    foreach (Match mt in engWords)
                    {
                        if (i > App.LRC.Count - 1)
                            break;

                        string engWordsName = mt.Value.Replace("eng = ", string.Empty).Trim('"');
                        App.LRC[i].NameEng = engWordsName;
                        i++;
                    }
                    int k = 0;
                    foreach (Match mt in Proficiency)
                    {
                        if (k > App.LRC.Count - 1)
                            break;

                        string ProficiencyName = mt.Value.Replace("pfc = ", string.Empty).Trim('"');
                        App.LRC[k].Proficiency = ProficiencyName;
                        k++;
                    }
                    ((MainWindow)Application.Current.MainWindow).lv_res_list.ItemsSource = App.LRC;
                    ((MainWindow)Application.Current.MainWindow).lb_lsCount.Content = App.LRC.Count;
                }
                catch (Exception ex)
                {
                    System.Windows.MessageBox.Show(ex.Message.ToString());
                }
            }

            public static void excel_list(string filePath)
            {
                App.LRC.Clear();
                ((MainWindow)Application.Current.MainWindow).lv_res_list.ItemsSource = null;

                /* 呼叫Excel程式 且 不顯示 */
                Excel.Application SrcExcelApp = new Excel.Application();
                SrcExcelApp.Visible = false;

                Excel.Workbook SrcWorkBook = null;
                Excel.Worksheet SrcWorksheet = null;
                Excel.Range SrcRange = null;

                try
                {
                    /*
                     * 開啟Excel檔
                     * 指定活頁簿,代表Sheet1
                     * 取得有值的範圍
                     */
                    SrcWorkBook = SrcExcelApp.Workbooks.Open(filePath);
                    SrcWorksheet = (Excel.Worksheet)SrcWorkBook.Sheets[1];
                    SrcRange = SrcWorksheet.UsedRange;

                    int count = 0, i = 0;
                    foreach (Excel.Range item in SrcRange)
                    {
                        if (count % 3 == 0)
                        {
                            App.LRC.Add(new ResCless { NameCht = item.Cells.Text, NameEng = string.Empty, Proficiency = string.Empty });
                        }
                        else if (count % 3 == 1)
                        {
                            App.LRC[i].NameEng = item.Cells.Text;
                        }
                        else if (count % 3 == 2)
                        {
                            App.LRC[i].Proficiency = item.Cells.Text;
                            i++;
                        }
                        count++;
                    }
                    ((MainWindow)Application.Current.MainWindow).lv_res_list.ItemsSource = App.LRC;
                    ((MainWindow)Application.Current.MainWindow).lb_lsCount.Content = App.LRC.Count;
                }
                catch (Exception ex)
                {
                    System.Windows.MessageBox.Show(ex.Message.ToString());
                }
                finally
                {
                    /*
                     * 關閉Excel活頁簿
                     * 關閉Excel
                     * 釋放Excel資源 ，並呼叫GC回收
                     */
                    SrcWorkBook.Close();
                    SrcWorkBook = null;
                    SrcExcelApp.Quit();
                    System.Runtime.InteropServices.Marshal.FinalReleaseComObject(SrcExcelApp);
                    GC.Collect();
                }
            }
        }

        public class Save
        {
            public static void As_db(string path)
            {
                byte[] List_bytes = Encoding.Unicode.GetBytes(LRC2txt());
                File.WriteAllBytes(path + ".db", GZip.Compress(List_bytes));
            }

            public static void As_txt(string path)
            {
                File.WriteAllText(path + ".txt",LRC2txt());
            }

            private static string LRC2txt()
            {
                string list = string.Empty;
                for (int i = 0; i < App.LRC.Count; i++)
                {
                    list += "cht = \"" + App.LRC[i].NameCht + "\"" + Environment.NewLine +
                        "eng = \"" + App.LRC[i].NameEng + "\"" + Environment.NewLine +
                        "pfc = \"" + App.LRC[i].Proficiency + "\"" + Environment.NewLine;
                }
                return list;
            }

            public static void As_excel(string path)
            {
                /* 呼叫Excel程式 且 不顯示 */
                Excel.Application SrcExcelApp = new Excel.Application();
                SrcExcelApp.Visible = false;

                Excel.Workbook SrcWorkBook = null;
                Excel.Worksheet SrcWorksheet = new Excel.Worksheet();
                Excel.Range SrcRange = null;

                try
                {
                    /*
                     * 產生Excel檔
                     * 指定活頁簿,代表Sheet1
                     */
                    SrcWorkBook = SrcExcelApp.Workbooks.Add();
                    SrcWorksheet = SrcWorkBook.Sheets[1];

                    for (int i = 1; i <= App.LRC.Count; i++)
                    {
                        SrcExcelApp.Cells[i, 1] = App.LRC[i - 1].NameCht;
                        SrcExcelApp.Cells[i, 2] = App.LRC[i - 1].NameEng;
                        SrcExcelApp.Cells[i, 3] = App.LRC[i - 1].Proficiency;
                    }

                    /*
                     * 取得有值的範圍
                     * 自動調整列高
                     * 自動調整欄寬
                     */
                    SrcRange = SrcWorksheet.UsedRange;
                    SrcRange.EntireRow.AutoFit();
                    SrcRange.EntireColumn.AutoFit();
                }
                catch (Exception ex)
                {
                    System.Windows.MessageBox.Show(ex.Message.ToString());
                }
                finally
                {
                    /*
                     * 儲存Excel檔 (副檔名由Excel版本判斷)
                     * 關閉Excel活頁簿
                     * 關閉Excel
                     * 釋放Excel資源，並呼叫GC回收
                     */
                    SrcWorkBook.SaveAs(path);
                    SrcWorkBook.Close();
                    SrcWorkBook = null;
                    SrcExcelApp.Quit();
                    System.Runtime.InteropServices.Marshal.FinalReleaseComObject(SrcExcelApp);
                    GC.Collect();
                }
            }
        }
    }
}