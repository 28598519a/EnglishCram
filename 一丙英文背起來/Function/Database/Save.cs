using System;
using System.IO;
using System.Text;
using System.Windows;
using Excel = Microsoft.Office.Interop.Excel; //Define為Excel，以避免Application名稱衝突

namespace 一丙英文背起來
{
    namespace Database
    {
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
                File.WriteAllBytes(path, Compress.GZip.Compress(List_bytes));
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