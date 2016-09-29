using System.Reflection.Emit;

namespace ARSnovaPPIntegration.Communication.Contract
{
    public interface IArsnovaClickService
    {
        string GetAllRestMethods();

        string FindAllHashtags();
    }
}
