using System;
using System.Linq;
using System.Runtime.InteropServices;

using Microsoft.Office.Interop.PowerPoint;

using ARSnovaPPIntegration.Business.Model;

namespace ARSnovaPPIntegration.Presentation.Helpers
{
    public static class SlideTracker
    {
        public static int CurrentShowedPresentationSlidePosition
        {
            get
            {
                if (Globals.ThisAddIn.Application.SlideShowWindows.Count <= 0)
                {
                    // currently not in presentation mode
                    return 0;
                }
                return Globals.ThisAddIn.Application.ActivePresentation.SlideShowWindow.View.CurrentShowPosition;
            }
        }

        public static Slide CurrentSlide
        {
            get
            {
                try
                {
                    return Globals.ThisAddIn.Application.ActiveWindow.View.Slide as Slide;
                }
                catch (COMException)
                {
                    // no slide selected or currently in view
                    return null;
                }
            }
        }

        public static ShapeRange CurrentSelectedShapeRange
        {
            get
            {
                try
                {
                    return Globals.ThisAddIn.Application.ActiveWindow.Selection.ShapeRange;
                }
                catch (COMException)
                {
                    return null;
                }
            }
        }

        public static void RemoveSlide(int slideId)
        {
            GetSlideById(slideId).Delete();
        }

        public static Tuple<bool, SlideQuestionModel> IsPresentationOnStartArsnovaClickSlide()
        {
            var currentShowedSlidePosition = CurrentShowedPresentationSlidePosition;

            var slideSessionModel = PresentationInformationStore.GetStoredSlideSessionModel();

            if (slideSessionModel == null)
            {
                return new Tuple<bool, SlideQuestionModel>(false, null);
            }

            foreach (var slideQuestionModel in slideSessionModel.Questions)
            {
                if (slideQuestionModel.QuestionTimerSlideId.HasValue 
                    && GetSlideById(slideQuestionModel.QuestionTimerSlideId.Value).SlideNumber == currentShowedSlidePosition)
                    return new Tuple<bool, SlideQuestionModel>(true, slideQuestionModel);
            }

            return new Tuple<bool, SlideQuestionModel>(false, null);
        }

        public static Tuple<bool, SlideQuestionModel> IsPresentationOnStartArsnovaVotingSlide()
        {
            // TODO
            return new Tuple<bool, SlideQuestionModel>(false, null);
        }

        public static bool IsArsnovaSlide(Slide slide)
        {
            if (slide == null)
                return false;

            var slideSessionModel = PresentationInformationStore.GetStoredSlideSessionModel();

            return slideSessionModel != null && 
                slideSessionModel.Questions.Any(questionModel => questionModel.QuestionInfoSlideId == slide.SlideID
                                                                    || questionModel.QuestionTimerSlideId == slide.SlideID
                                                                    || questionModel.ResultsSlideId == slide.SlideID);
        }

        public static SlideQuestionModel GetQuestionModelFromSlide(Slide slide)
        {
            var slideSessionModel = PresentationInformationStore.GetStoredSlideSessionModel();

            return slideSessionModel.Questions.First(questionModel => questionModel.QuestionInfoSlideId == slide.SlideID);
        }

        public static Slide GetSlideById(int slideId)
        {
            try
            {
                return Globals.ThisAddIn.Application.ActivePresentation.Slides.FindBySlideID(slideId);
            }
            catch (Exception ex)
            {
                // no slide with id found
                return null;
            }
            
        }

        public static Slide GetSlideByIndex(int slideIndex)
        {
            foreach (Slide slide in Globals.ThisAddIn.Application.ActivePresentation.Slides)
            {
                if (slide.SlideIndex == slideIndex)
                    return slide;
            }

            return null;
        }
    }
}
