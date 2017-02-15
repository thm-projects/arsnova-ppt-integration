using System.Collections.Generic;

namespace ARSnovaPPIntegration.Communication.Model.ArsnovaEu
{
    public class LectureQuestionModel
    {
        public string _id { get; set; }

        public string type { get; set; }

        public string questionType { get; set; }

        public string questionVariant { get; set; }

        public string subject { get; set; }

        public string text { get; set; }

        public bool active { get; set; }

        public string releasedFor { get; set; }

        public List<PossibleAnswerObject> possibleAnswers { get; set; }

        public bool noCorrect { get; set; }

        public string sessionId { get; set; }

        public string sessionKeyword { get; set; }

        public int timestamp { get; set; }

        public int number { get; set; }

        public int duration { get; set; }

        public int piRound { get; set; }

        public int piRoundEndTime { get; set; }

        public int piRoundStartTime { get; set; }

        public bool piRoundFinished { get; set; }

        public bool piRoundActive { get; set; }

        public bool votingDisabled { get; set; }

        public bool showStatistic { get; set; }

        public bool showAnswer { get; set; }

        public bool abstention { get; set; }

        public bool ignoreCaseSensitive { get; set; }

        public bool ignoreWhitespaces { get; set; }

        public bool ignorePunctuation { get; set; }

        public bool fixedAnswer { get; set; }

        public bool strictMode { get; set; }

        public int rating { get; set; }

        public string correctAnswer { get; set; }

        public string get_id { get; set; }

        public string get_rev { get; set; }

        public string image { get; set; }

        public string fcImage { get; set; }

        public int gridSize { get; set; }

        public int offsetX { get; set; }

        public int offsetY { get; set; }

        public int zoomLvl { get; set; }

        public int gridOffsetX { get; set; }

        public int gridOffsetY { get; set; }

        public int gridZoomLvl { get; set; }

        public int gridSizeX { get; set; }

        public int gridSizeY { get; set; }

        public bool gridIsHidden { get; set; }

        public int imgRotation { get; set; }

        public bool toggleFieldsLeft { get; set; }

        public int numClickableFields { get; set; }

        public int thresholdCorrectAnswers { get; set; }

        public bool cvIsColored { get; set; }

        public string gridLineColor { get; set; }

        public int numberOfDots { get; set; }

        public string gridType { get; set; }

        public string scaleFactor { get; set; }

        public string gridScaleFactor { get; set; }

        public bool imageQuestion { get; set; }

        public bool textAnswerEnabled { get; set; }

        public string hint { get; set; }

        public string solution { get; set; }

        public string session { get; set; }
    }

    public class PossibleAnswerObject
    {
        public string id { get; set; }

        public string text { get; set; }

        public bool correct { get; set; }

        public int value { get; set; }
    }
}
