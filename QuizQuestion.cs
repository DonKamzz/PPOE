using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PPOE
{
    public class QuizQuestion
    {
        public int Id { get; set; }
        public string Question { get; set; }
        public string OptionA { get; set; }
        public string OptionB { get; set; }
        public string OptionC { get; set; }
        public string OptionD { get; set; }
        public string CorrectAnswer { get; set; }
        public string Explanation { get; set; }
        public string Category { get; set; }

        public bool IsTrueFalse => string.IsNullOrEmpty(OptionC);

        public List<string> GetOptions()
        {
            var options = new List<string>();
            if (!string.IsNullOrEmpty(OptionA)) options.Add(OptionA);
            if (!string.IsNullOrEmpty(OptionB)) options.Add(OptionB);
            if (!string.IsNullOrEmpty(OptionC)) options.Add(OptionC);
            if (!string.IsNullOrEmpty(OptionD)) options.Add(OptionD);
            return options;
        }

        public List<string> GetOptionLetters()
        {
            var letters = new List<string>();
            if (!string.IsNullOrEmpty(OptionA)) letters.Add("A");
            if (!string.IsNullOrEmpty(OptionB)) letters.Add("B");
            if (!string.IsNullOrEmpty(OptionC)) letters.Add("C");
            if (!string.IsNullOrEmpty(OptionD)) letters.Add("D");
            return letters;
        }

        public bool IsAnswerCorrect(string answer)
        {
            if (string.IsNullOrEmpty(answer) || string.IsNullOrEmpty(CorrectAnswer))
                return false;
            return CorrectAnswer.Trim().Equals(answer.Trim(), System.StringComparison.OrdinalIgnoreCase);
        }
    }
}
