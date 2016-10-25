using Microsoft.Office.Interop.PowerPoint;

namespace ARSnovaPPIntegration.Business.Contract
{
    public interface ISlideManipulator
    {
        void AddFooter(Slide slide, string header);

        void SetArsnovaStyle(Slide slide);

        void SetArsnovaClickStyle(Slide slide, string hashtag);
    }
}
