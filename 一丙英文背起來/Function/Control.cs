using System.Windows;

namespace 一丙英文背起來
{
    public static class Control
    {
        public static void ClearText()
        {
            ((MainWindow)Application.Current.MainWindow).lb_Question.Content = "";
            ((MainWindow)Application.Current.MainWindow).lb_AnswerCheck.Content = "";
            ((MainWindow)Application.Current.MainWindow).tb_Answer.Text = "";
            ((MainWindow)Application.Current.MainWindow).tb_Again.Text = "";
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
    }
}