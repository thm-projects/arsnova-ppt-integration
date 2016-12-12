using System.Collections.Generic;

namespace ARSnovaPPIntegration.Model.ArsnovaClick
{
    public class HashtagInfo
    {
        public string _id { get; set; }

        public string hashtag { get; set; }
    }

    public class HashtagInfos
    {
        public List<HashtagInfo> hashtags { get; set; }
    }
}
