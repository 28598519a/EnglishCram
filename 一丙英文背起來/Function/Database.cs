using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using Excel = Microsoft.Office.Interop.Excel; //Define為Excel，以避免Application名稱衝突

namespace 一丙英文背起來
{
    namespace Database
    {
        public class Question
        {
            /// <summary>
            /// 產生新的題目
            /// </summary>
            /// <returns>產生成功</returns>
            public static bool NewQuestion()
            {
                if (App.Index >= App.ResultList.Count)
                {
                    App.Index = 0;
                    Random GetRandomInt = new Random(Guid.NewGuid().GetHashCode());
                    App.ResultList = App.ResultList.OrderBy(o => GetRandomInt.Next()).ToList();

                    // 取得設定的熟練度
                    int pfclim = Convert.ToInt32(((MainWindow)Application.Current.MainWindow).tb_pfclim.Text);
                    for (int i = App.ResultList.Count - 1; i >= 0; )
                    {
                        if (App.LRC[App.ResultList[i]].Proficiency >= pfclim)
                        {
                            if (i.Equals(0)) return false;
                            i--;
                        }
                        else
                        {
                            break;
                        }
                    }
                }
                if (((MainWindow)Application.Current.MainWindow).rb_Answer_Eng.IsChecked == true)
                    ((MainWindow)Application.Current.MainWindow).lb_Question.Content = App.LRC[App.ResultList.ToList()[App.Index]].NameCht;
                else ((MainWindow)Application.Current.MainWindow).lb_Question.Content = App.LRC[App.ResultList.ToList()[App.Index]].NameEng;
                return true;
            }

            /// <summary>
            /// 清理輸入內容
            /// </summary>
            /// <param name="strIn">輸入內容</param>
            /// <returns>純文字</returns>
            public static string CleanInput(string strIn)
            {
                // \W 匹配任何非文字字元 (亦可用^\w)
                try
                {
                    return Regex.Replace(strIn, @"[\W]+", string.Empty, RegexOptions.None, TimeSpan.FromSeconds(1.5));
                }
                // 替換時間長於1.5秒就放棄替換
                catch (RegexMatchTimeoutException)
                {
                    //return string.Empty;
                    return strIn;
                }
            }
        }

        public class Load
        {
            /// <summary>
            /// txt題庫導入列表
            /// </summary>
            /// <param name="fileText">txt檔的文字內容</param>
            public static void txt_list(string fileText)
            {
                App.LRC.Clear();

                try
                {
                    // [^\"]+ 匹配任何除了 " 以外的字元 ; [0-9]匹配數字
                    MatchCollection chtWords = Regex.Matches(fileText, "cht = \"[^\"]+\"");
                    MatchCollection engWords = Regex.Matches(fileText, "eng = \"[^\"]+\"");
                    MatchCollection Proficiency = Regex.Matches(fileText, "pfc = \"[0-9]+\"");

                    foreach (Match mt in chtWords)
                    {
                        string chtWordsName = mt.Value.Replace("cht = ", string.Empty).Trim('"');
                        App.LRC.Add(new ResCless { NameCht = chtWordsName, NameEng = string.Empty, Proficiency = 0 });
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

                        int ProficiencyName = Convert.ToInt32(mt.Value.Replace("pfc = ", string.Empty).Trim('"'));
                        App.LRC[k].Proficiency = ProficiencyName;
                        k++;
                    }
                    Control.Set_Lv_res_list();
                }
                catch (Exception ex)
                {
                    System.Windows.MessageBox.Show(ex.Message.ToString());
                }
            }

            /// <summary>
            /// excel題庫導入列表
            /// </summary>
            /// <param name="filePath">excel檔案路徑</param>
            public static void Excel_list(string filePath)
            {
                App.LRC.Clear();

                // 呼叫Excel程式 且 不顯示
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
                            App.LRC.Add(new ResCless { NameCht = item.Cells.Text, NameEng = string.Empty, Proficiency = 0 });
                        }
                        else if (count % 3 == 1)
                        {
                            App.LRC[i].NameEng = item.Cells.Text;
                        }
                        else if (count % 3 == 2)
                        {
                            App.LRC[i].Proficiency = Convert.ToInt32(item.Cells.Text);
                            i++;
                        }
                        count++;
                    }
                    Control.Set_Lv_res_list();
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
                    SrcExcelApp.Quit();
                    System.Runtime.InteropServices.Marshal.FinalReleaseComObject(SrcExcelApp);
                    GC.Collect();
                }
            }
        }

        public class Save
        {
            /// <summary>
            /// 保存列表為db題庫
            /// </summary>
            /// <param name="path">存檔路徑</param>
            public static void As_db(string path)
            {
                path += ".db";
                if (File.Exists(path))
                {
                    MessageBoxResult dialogResult = System.Windows.MessageBox.Show("檔案已存在，是否覆蓋", "警告", MessageBoxButton.YesNo);
                    if (dialogResult == MessageBoxResult.No)
                    {
                        return;
                    }
                }
                byte[] List_bytes = Encoding.Unicode.GetBytes(LRC2txt());
                File.WriteAllBytes(path, GZip.Compress(List_bytes));
            }

            /// <summary>
            /// 保存列表為txt題庫
            /// </summary>
            /// <param name="path">存檔路徑</param>
            public static void As_txt(string path)
            {
                path += ".txt";
                if (File.Exists(path))
                {
                    MessageBoxResult dialogResult = System.Windows.MessageBox.Show("檔案已存在，是否覆蓋", "警告", MessageBoxButton.YesNo);
                    if (dialogResult == MessageBoxResult.No)
                    {
                        return;
                    }
                }
                File.WriteAllText(path, LRC2txt());
            }

            /// <summary>
            /// 列表轉TEXT
            /// </summary>
            /// <returns>TEXT</returns>
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

            /// <summary>
            /// 保存列表為excel題庫
            /// </summary>
            /// <param name="path">存檔路徑</param>
            public static void As_excel(string path)
            {
                // 呼叫Excel程式 且 不顯示
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

                    try
                    {
                        // 儲存Excel檔 (使用當前Excel版本預設的副檔名)
                        SrcWorkBook.SaveAs(path);
                    }
                    catch
                    {
                        // 覆蓋選否時，後續潛在的窗口不顯示，直接以預設值選擇
                        SrcExcelApp.DisplayAlerts = false;
                    }
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
                     * 釋放Excel資源，並呼叫GC回收
                     */
                    SrcWorkBook.Close();
                    SrcExcelApp.Quit();
                    System.Runtime.InteropServices.Marshal.FinalReleaseComObject(SrcExcelApp);
                    GC.Collect();
                }
            }
        }
    }
}