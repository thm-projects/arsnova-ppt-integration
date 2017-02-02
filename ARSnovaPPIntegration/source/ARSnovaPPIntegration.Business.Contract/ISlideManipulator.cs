using System.Collections.Generic;

using Microsoft.Office.Interop.PowerPoint;

using ARSnovaPPIntegration.Business.Model;
using ARSnovaPPIntegration.Communication.Model.ArsnovaClick;

namespace ARSnovaPPIntegration.Business.Contract
{
    public interface ISlideManipulator
    {
        void AddIntroSlide(SlideSessionModel slideSessionModel, Slide introSlide);

        void AddQuizToStyledSlides(SlideQuestionModel slideQuestionModel, Slide questionInfoSlide, Slide questionTimerSlide, Slide resultsSlide);

        void AddQuizToSlideWithoutStyling(SlideQuestionModel slideQuestionModel, Slide slide);

        void SetTimerOnSlide(SlideQuestionModel slideQuestionModel, Slide resultsSlide, int countdown);

        void SetResults(SlideQuestionModel slideQuestionModel, Slide resultsSlide, List<ResultModel> results);

        void CleanResultsPage(Slide resultsSlide);
    }
}
