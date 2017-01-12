using ARSnovaPPIntegration.Business.Model;
using Microsoft.Office.Interop.PowerPoint;

namespace ARSnovaPPIntegration.Business.Contract
{
    public interface ISlideManipulator
    {
        void AddFooter(Slide slide, string header);

        void SetArsnovaStyle(Slide slide);

        void SetArsnovaClickStyle(Slide slide, string hashtag);

        void AddClickIntroSlide(Slide slide, string hashtag);

        void AddQuizToSlide(SlideQuestionModel slideQuestionModel, Slide questionInfoSlide, Slide resultsSlide);

        void SetTimerOnSlide(Slide timerSlide, int countdown);

        void InitTimerOnSlide(Slide timerSlide, int initCountdown);
    }
}
