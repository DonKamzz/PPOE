using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PPOE
{
    public class QuizManager
    {
        private DatabaseHelper dbHelper;
        private List<QuizQuestion> questions;
        private int currentIndex = 0;
        private int score = 0;
        private bool completed = false;

        public QuizManager(DatabaseHelper databaseHelper)
        {
            dbHelper = databaseHelper;
            LoadQuestions();
        }

        private void LoadQuestions()
        {
            questions = dbHelper.GetQuizQuestions();
            if (questions.Count == 0)
            {
                // Fallback if database is empty
                questions = new List<QuizQuestion>();
                for (int i = 1; i <= 15; i++)
                {
                    questions.Add(new QuizQuestion
                    {
                        Question = $"Question {i}: Sample question?",
                        OptionA = "Option A",
                        OptionB = "Option B",
                        OptionC = "Option C",
                        OptionD = "Option D",
                        CorrectAnswer = "A",
                        Explanation = "This is a sample explanation."
                    });
                }
            }
            Shuffle();
        }

        private void Shuffle()
        {
            Random rand = new Random();
            questions = questions.OrderBy(q => rand.Next()).ToList();
        }

        public void StartQuiz()
        {
            currentIndex = 0;
            score = 0;
            completed = false;
            Shuffle();
        }

        public QuizQuestion GetCurrentQuestion()
        {
            return currentIndex < questions.Count ? questions[currentIndex] : null;
        }

        // ========== ADDED: GetCurrentNumber method ==========
        public int GetCurrentNumber()
        {
            return currentIndex + 1;
        }

        // ========== ADDED: GetTotalQuestions method ==========
        public int GetTotalQuestions()
        {
            return questions.Count;
        }

        // ========== ADDED: GetScore method ==========
        public int GetScore()
        {
            return score;
        }

        // ========== ADDED: IsComplete method ==========
        public bool IsComplete()
        {
            return completed;
        }

        public string SubmitAnswer(string answer)
        {
            if (currentIndex >= questions.Count) return "Quiz completed!";

            var question = questions[currentIndex];
            bool isCorrect = question.IsAnswerCorrect(answer);

            string feedback = isCorrect ? "✅ Correct! " : $"❌ Incorrect. Answer: {question.CorrectAnswer}. ";
            feedback += question.Explanation;

            if (isCorrect) score++;

            currentIndex++;

            if (currentIndex >= questions.Count)
            {
                completed = true;
                double pct = (double)score / questions.Count * 100;
                string msg = pct >= 80 ? "⭐ Excellent!" : pct >= 60 ? "👍 Good job!" : "📚 Keep learning!";
                feedback += $"\n\n━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━\n";
                feedback += $"SCORE: {score}/{questions.Count} ({pct:F0}%)\n{msg}";
            }

            return feedback;
        }
    }
}
