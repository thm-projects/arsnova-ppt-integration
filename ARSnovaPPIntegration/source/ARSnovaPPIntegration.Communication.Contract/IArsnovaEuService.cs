using ARSnovaPPIntegration.Common.Enum;
using ARSnovaPPIntegration.Model;

namespace ARSnovaPPIntegration.Communication.Contract
{
    public interface IArsnovaEuService
    {
        void Login(LoginMethod loginMethod);

        SessionModel CreateNewSession();
    }
}
