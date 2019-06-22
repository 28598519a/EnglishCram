using System.Windows;

namespace 一丙英文背起來
{
    public static class Control
    {
        public static void ClearText()
        {
            ((MainWindow)Application.Current.MainWindow).lb_Question.Content = string.Empty;
            ((MainWindow)Application.Current.MainWindow).lb_AnswerCheck.Content = string.Empty;
            ((MainWindow)Application.Current.MainWindow).tb_Answer.Text = string.Empty;
            ((MainWindow)Application.Current.MainWindow).tb_Again.Text = string.Empty;
        }

        public static void Test(bool IsEnabled)
        {
            ((MainWindow)Application.Current.MainWindow).tb_Answer.IsEnabled = IsEnabled;
            ((MainWindow)Application.Current.MainWindow).btn_Answer.IsEnabled = IsEnabled;
        }

        public static void Again(bool IsEnabled)
        {
            ((MainWindow)Application.Current.MainWindow).tb_Again.IsEnabled = IsEnabled;
            ((MainWindow)Application.Current.MainWindow).btn_Again.IsEnabled = IsEnabled;
        }

        public static void EndExercise()
        {
            Again(false);
            App.Again_Count = 0;
            ClearText();
            App.Index++;
            Test(true);
        }

        public static void MainVisibility(bool show)
        {
            if (show)
            {
                ((MainWindow)Application.Current.MainWindow).Show();
            }
            else
            {
                ((MainWindow)Application.Current.MainWindow).Hide();
            }
        }
    }
}