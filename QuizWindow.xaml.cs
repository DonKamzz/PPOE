using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace PPOE
{
    /// <summary>
    /// Interaction logic for QuizWindow.xaml
    /// </summary>
    public partial class QuizWindow : Window
    {
        private QuizManager quizManager;
        private string userName;
        private bool answered = false;

        public QuizWindow(string userName)
        {
            InitializeComponent();
            this.userName = userName;
            quizManager = new QuizManager(new DatabaseHelper());
            StartQuiz();
        }

        // Title Bar Controls
        private void TitleBar_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }

        private void CloseWindow_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void StartQuiz()
        {
            quizManager.StartQuiz();
            answered = false;
            ShowQuestion();
        }

        private void ShowQuestion()
        {
            var question = quizManager.GetCurrentQuestion();
            if (question == null)
            {
                ShowComplete();
                return;
            }

            txtQuestion.Text = question.Question;

            // FIXED: Use GetCurrentNumber() method
            txtProgress.Text = $"Question {quizManager.GetCurrentNumber()} of {quizManager.GetTotalQuestions()}";

            // FIXED: Use GetScore() method
            txtScore.Text = $" | Score: {quizManager.GetScore()}";
            txtFeedback.Text = "Select an answer to see feedback";

            // FIXED: Use GetTotalQuestions() method
            progressBar.Maximum = quizManager.GetTotalQuestions();
            progressBar.Value = quizManager.GetCurrentNumber() - 1;

            var options = question.GetOptions();
            var letters = question.GetOptionLetters();

            btnOptionA.Visibility = Visibility.Collapsed;
            btnOptionB.Visibility = Visibility.Collapsed;
            btnOptionC.Visibility = Visibility.Collapsed;
            btnOptionD.Visibility = Visibility.Collapsed;

            if (options.Count > 0)
            {
                btnOptionA.Visibility = Visibility.Visible;
                btnOptionA.Content = $"A. {options[0]}";
                btnOptionA.Tag = letters[0];
            }
            if (options.Count > 1)
            {
                btnOptionB.Visibility = Visibility.Visible;
                btnOptionB.Content = $"B. {options[1]}";
                btnOptionB.Tag = letters[1];
            }
            if (options.Count > 2)
            {
                btnOptionC.Visibility = Visibility.Visible;
                btnOptionC.Content = $"C. {options[2]}";
                btnOptionC.Tag = letters[2];
            }
            if (options.Count > 3)
            {
                btnOptionD.Visibility = Visibility.Visible;
                btnOptionD.Content = $"D. {options[3]}";
                btnOptionD.Tag = letters[3];
            }

            ResetButtons();
            btnNext.IsEnabled = false;
            answered = false;
        }

        private void ResetButtons()
        {
            Button[] buttons = { btnOptionA, btnOptionB, btnOptionC, btnOptionD };
            foreach (var btn in buttons)
            {
                btn.Background = new SolidColorBrush(Color.FromRgb(18, 18, 42));
                btn.Foreground = new SolidColorBrush(Color.FromRgb(226, 232, 240));
                btn.IsEnabled = true;
                btn.BorderBrush = new SolidColorBrush(Color.FromRgb(26, 26, 62));
            }
        }

        private void btnOption_Click(object sender, RoutedEventArgs e)
        {
            if (answered) return;

            Button clicked = sender as Button;
            string answer = clicked.Tag.ToString();

            string feedback = quizManager.SubmitAnswer(answer);
            txtFeedback.Text = feedback;

            // FIXED: Use IsComplete() method
            if (quizManager.IsComplete())
            {
                ShowComplete();
                return;
            }

            answered = true;
            btnNext.IsEnabled = true;

            Button[] buttons = { btnOptionA, btnOptionB, btnOptionC, btnOptionD };
            foreach (var btn in buttons)
            {
                btn.IsEnabled = false;
            }
        }

        private void ShowComplete()
        {
            txtQuestion.Text = "Quiz Complete!";
            txtProgress.Text = "Completed!";

            // FIXED: Use GetScore() and GetTotalQuestions() methods
            txtScore.Text = $" | Final Score: {quizManager.GetScore()} out of {quizManager.GetTotalQuestions()}";

            btnOptionA.Visibility = Visibility.Collapsed;
            btnOptionB.Visibility = Visibility.Collapsed;
            btnOptionC.Visibility = Visibility.Collapsed;
            btnOptionD.Visibility = Visibility.Collapsed;

            btnNext.IsEnabled = false;
            progressBar.Value = progressBar.Maximum;
        }

        private void btnNext_Click(object sender, RoutedEventArgs e)
        {
            ShowQuestion();
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
