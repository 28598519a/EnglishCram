using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;

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
                    for (int i = App.ResultList.Count - 1; i >= 0;)
                    {
                        if (App.LRC[App.ResultList[i]].Proficiency >= pfclim)
                        {
                            if (i.Equals(0)) return false;
                            i--;
                        }
                        else break;
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
    }
}
