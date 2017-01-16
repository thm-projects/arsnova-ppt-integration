using System.Collections.Generic;

using Microsoft.Office.Interop.PowerPoint;

using ARSnovaPPIntegration.Business.Model;
using ARSnovaPPIntegration.Communication.Model.ArsnovaClick;

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

        void SetResultsOnSlide(Slide resultsSlide, List<ResultModel> best10Responses);
    }
}
