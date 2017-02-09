namespace ARSnovaPPIntegration.Communication.Model.ArsnovaEu
{
    public class SessionModel
    {
        public string type { get; set; }
        public string name { get; set; }
        public string shortName { get; set; }
        public string keyword { get; set; }
        public string creator { get; set; }
        public bool active { get; set; }
        public int lastOwnerActivity { get; set; }
        public object courseType { get; set; }
        public object courseId { get; set; }
        public object _conflicts { get; set; }
        public long creationTime { get; set; }
        public LearningProgressOptions learningProgressOptions { get; set; }
        public Features features { get; set; }
        public object ppAuthorName { get; set; }
        public object ppAuthorMail { get; set; }
        public object ppUniversity { get; set; }
        public object ppLogo { get; set; }
        public object ppSubject { get; set; }
        public object ppLicense { get; set; }
        public object ppDescription { get; set; }
        public object ppFaculty { get; set; }
        public object ppLevel { get; set; }
        public object sessionType { get; set; }
        public bool feedbackLock { get; set; }
        public string _id { get; set; }
        public string _rev { get; set; }
    }

    public class LearningProgressOptions
    {
        public string type { get; set; }
        public string questionVariant { get; set; }
    }

    public class Features
    {
        public bool custom { get; set; }
        public bool clicker { get; set; }
        public bool peerGrading { get; set; }
        public bool twitterWall { get; set; }
        public bool liveFeedback { get; set; }
        public bool interposedFeedback { get; set; }
        public bool liveClicker { get; set; }
        public bool flashcard { get; set; }
        public bool total { get; set; }
        public bool jitt { get; set; }
        public bool lecture { get; set; }
        public bool feedback { get; set; }
        public bool interposed { get; set; }
        public bool pi { get; set; }
        public bool learningProgress { get; set; }
        public bool slides { get; set; }
    }
}
