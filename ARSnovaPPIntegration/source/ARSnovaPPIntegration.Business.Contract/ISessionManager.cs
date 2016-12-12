using ARSnovaPPIntegration.Business.Model;

namespace ARSnovaPPIntegration.Business.Contract
{
    public interface ISessionManager
    {
        void CreateSession(SlideSessionModel slideSessionModel);

        void EditSession(SlideSessionModel slideSessionModel);
    }
}
