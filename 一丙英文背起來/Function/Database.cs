using System;
using System.Text.RegularExpressions;
using System.Windows;

namespace 一丙英文背起來
{
    public class Database
    {
        public static string CleanInput(string strIn)
        {
            /* \W 匹配任何非文字字元 (亦可用^\w) */
            try
            {
                return Regex.Replace(strIn, @"[\W]+", "", RegexOptions.None, TimeSpan.FromSeconds(1.5));
            }
            /* 替換時間長於1.5秒就放棄替換 */
            catch (RegexMatchTimeoutException)
            {
                //return string.Empty;
                return strIn;
            }
        }

        public static void Load_res_list(string fileText)
        {
            App.LRC.Clear();
            ((MainWindow)Application.Current.MainWindow).lv_res_list.ItemsSource = null;

            try
            {
                /* [^\"]+ 匹配任何除了 " 以外的字元 */
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
                ((MainWindow)Application.Current.MainWindow).lv_res_list.ItemsSource = App.LRC;
                ((MainWindow)Application.Current.MainWindow).lb_lsCount.Content = App.LRC.Count;
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show(ex.ToString());
            }
        }

        public static void Make_res_list(string fileText)
        {

        }
    }
}
