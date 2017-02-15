using System.Collections.Generic;
using System.Net;

using ARSnovaPPIntegration.Common.Enum;

namespace ARSnovaPPIntegration.Business.Model
{
    public class ArsnovaEuConfig
    {
        public string GuestUserName { get; set; }

        public LoginMethod LoginMethod { get; set; } = LoginMethod.Guest;

        public bool IsAuthenticated { get; set; } = false;

        public List<Cookie> Cookies { get; set; }
    }
}
