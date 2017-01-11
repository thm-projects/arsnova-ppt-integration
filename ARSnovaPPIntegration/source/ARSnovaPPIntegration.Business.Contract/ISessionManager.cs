using ARSnovaPPIntegration.Business.Model;

namespace ARSnovaPPIntegration.Business.Contract
{
    public interface ISessionManager
    {
        ValidationResult SetSession(SlideSessionModel slideSessionModel);

        void StartSession(SlideSessionModel slideSessionModel, int questionIndex);

        ValidationResult ActivateClickSession(SlideSessionModel slideSessionModel);
    }
}
