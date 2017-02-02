namespace ARSnovaPPIntegration.Communication.Model.ArsnovaClick
{
    public class AnswerOptionModel
    {
        public string hashtag { get; set; }

        public int questionIndex { get; set; }

        public string answerText { get; set; }

        public int answerOptionNumber { get; set; }

        public bool isCorrect { get; set; }

        public string type { get; set; }

        public bool configCaseSensitive { get; set; }

        public bool configTrimWhitespaces { get; set; }

        public bool configUseKeywords { get; set; }

        public bool configUsePunctuation { get; set; }
    }
}
