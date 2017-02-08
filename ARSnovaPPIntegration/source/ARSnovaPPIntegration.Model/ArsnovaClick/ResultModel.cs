using System.Collections.Generic;

namespace ARSnovaPPIntegration.Communication.Model.ArsnovaClick
{
    public class ResultModel
    {
        public string hashtag { get; set; }

        public int questionIndex { get; set; }

        public List<int> answerOptionNumber { get; set; }

        public string userNick { get; set; }

        public int responseTime { get; set; }

        public string freeTextInputValue { get; set; }

        public int rangedInputValue { get; set; }
    }
}
