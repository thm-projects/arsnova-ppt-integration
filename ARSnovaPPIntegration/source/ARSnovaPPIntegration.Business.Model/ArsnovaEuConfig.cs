using ARSnovaPPIntegration.Common.Enum;

namespace ARSnovaPPIntegration.Business.Model
{
    public class ArsnovaEuConfig
    {
        public string GuestUserName { get; set; }

        public LoginMethod LoginMethod { get; set; } = LoginMethod.Guest;
    }
}
