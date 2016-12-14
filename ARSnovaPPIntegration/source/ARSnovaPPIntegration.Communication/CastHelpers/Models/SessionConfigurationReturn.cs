using System.Collections.Generic;

using ARSnovaPPIntegration.Model.ArsnovaClick;

namespace ARSnovaPPIntegration.Communication.CastHelpers.Models
{
    public class SessionConfigurationWithId : SessionConfiguration
    {
        public string _id { get; set; }
    }

    public class SessionConfigurationReturn
    {
        public List<SessionConfigurationWithId> sessionConfiguration { get; set; }
    }
}
