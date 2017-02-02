using System.Collections.Generic;

namespace ARSnovaPPIntegration.Communication.Model.ArsnovaClick
{
    public class QuestionGroupModel
    {
        public string hashtag { get; set; }

        public List<QuestionModel> questionList { get; set; }

        public ConfigurationModel configuration { get; set; }

        public string type { get; set; }
    }

    public class QuestionModel
    {
        public string hashtag { get; set; }

        public string questionText { get; set; }

        public int timer { get; set; }

        public int startTime { get; set; }

        public int questionIndex { get; set; }

        public bool displayAnswerText { get; set; }

        public List<AnswerOptionModel> answerOptionList { get; set; }

        public string type { get; set; }

        public int rangeMin { get; set; }

        public int rangeMax { get; set; }

        public int correctValue { get; set; }
    }

    public class ConfigurationModel
    {
        public string hashtag { get; set; }

        public MusicModel music { get; set; }

        public NicksModel nicks { get; set; }

        public string theme { get; set; }

        public bool readingConfirmationEnabled { get; set; }

        public bool showResponseProgress { get; set; }
    }

    public class MusicModel
    {
        public string hashtag { get; set; }

        public bool isUsingGlobalVolume { get; set; }

        public bool lobbyEnabled { get; set; }

        public string lobbyTitle { get; set; }

        public int lobbyVolume { get; set; }

        public bool countdownRunningEnabled { get; set; }

        public string countdownRunningTitle { get; set; }

        public int countdownRunningVolume { get; set; }

        public bool countdownEndEnabled { get; set; }

        public string countdownEndTitle { get; set; }

        public int countdownEndVolume { get; set; }
    }

    public class NicksModel
    {
        public string hashtag { get; set; }

        public List<string> selectedValues { get; set; }

        public bool blockIllegal { get; set; }

        public bool restrictToCASLogin { get; set; }
    }
}
