using ARSnovaPPIntegration.Business.Model;

namespace ARSnovaPPIntegration.Business.Contract
{
    public interface ISessionManager
    {
        ValidationResult CreateSession(SlideSessionModel slideSessionModel);

        ValidationResult EditSession(SlideSessionModel slideSessionModel);
    }
}
