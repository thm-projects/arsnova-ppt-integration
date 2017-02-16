using System.Collections.Generic;

using Microsoft.Office.Interop.PowerPoint;

using ARSnovaPPIntegration.Business.Model;
using ARSnovaPPIntegration.Communication.Model.ArsnovaClick;
using ARSnovaPPIntegration.Communication.Model.ArsnovaEu;

namespace ARSnovaPPIntegration.Business.Contract
{
    public interface ISlideManipulator
    {
        void AddIntroSlide(SlideSessionModel slideSessionModel, Slide introSlide);

        void AddQuizToStyledSlides(SlideQuestionModel slideQuestionModel, Slide questionInfoSlide, Slide questionTimerSlide, Slide resultsSlide);

        void AddQuizToStyledSlides(SlideQuestionModel slideQuestionModel, Slide questionInfoSlide, Slide resultsSlide);

        void SetTimerOnSlide(SlideQuestionModel slideQuestionModel, Slide resultsSlide, int countdown);

        void SetClickResults(SlideQuestionModel slideQuestionModel, Slide resultsSlide, List<ResultModel> results);

        void SetVotingResults(SlideQuestionModel slideQuestionModel, Slide resultsSlide, ArsnovaVotingResultReturn results);

        void CleanResultsPage(Slide resultsSlide);
    }
}
