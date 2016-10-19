using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ARSnovaPPIntegration.Model.ArsnovaClick
{
    public class SessionConfiguration
    {
        public string hashtag { get; set; }

        public MusicConfiguration music { get; set; }

        public NickConfiguration nicks { get; set; }

        public string theme { get; set; }

        public bool readingConfirmationEnabled { get; set; }
    }

    public class MusicConfiguration
    {
        public bool isEnabled { get; set; }

        public int volumne { get; set; }

        public string title { get; set; }
    }

    public class NickConfiguration
    {
        public List<string> selectedValues { get; set; }

        public bool blockIllegal { get; set; }

        public bool restrictToCASLogin { get; set; }
    }
}
