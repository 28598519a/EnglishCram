using System;
using System.Text.RegularExpressions;
using Excel = Microsoft.Office.Interop.Excel; //Define為Excel，以避免Application名稱衝突

namespace 一丙英文背起來
{
    namespace Database
    {
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
    }
}