using System.Collections.Generic;

namespace ARSnovaPPIntegration.Communication.Model.ArsnovaEu
{
    public class ArsnovaVotingResultReturn
    {
        public List<ArsnovaVotingResultReturnElement> answerOptionElements { get; set; }
    }

    public class ArsnovaVotingResultReturnElement
    {
        public string _id { get; set; }

        public string _rev { get; set; }

        public string type { get; set; }

        public string sessionId { get; set; }

        public string questionId { get; set; }

        // 0,0,0,1 -> answeroption 4; 1,0,0,0 -> answeroption 1 etc.
        public string answerText { get; set; }

        public string answerTextRaw { get; set; }

        public string answerSubject { get; set; }

        public bool successfulFreeTextAnswer { get; set; }

        public string questionVariant { get; set; }

        public int questionValue { get; set; }

        public int piRound { get; set; }

        public int timestamp { get; set; }

        public bool read { get; set; }

        public int answerCount { get; set; }

        public bool abstention { get; set; }

        public int abstentionCount { get; set; }

        public string answerThumbnailImage { get; set; }
    }
}
