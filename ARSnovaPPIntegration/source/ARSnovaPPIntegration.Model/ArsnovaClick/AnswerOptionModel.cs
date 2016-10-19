using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ARSnovaPPIntegration.Model.ArsnovaClick
{
    public class AnswerOptionModel
    {
        public string hashtag { get; set; }

        public int questionIndex { get; set; }

        public string answerText { get; set; }

        public int answerOptionNumber { get; set; }

        public bool isCorrect { get; set; }
    }
}
