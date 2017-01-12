using System;

using Microsoft.Office.Interop.PowerPoint;

using ARSnovaPPIntegration.Business.Model;

namespace ARSnovaPPIntegration.Business.Contract
{
    public interface ISessionManager
    {
        ValidationResult SetSession(SlideSessionModel slideSessionModel);

        ValidationResult SetHashtag(SlideSessionModel slideSessionModel);

        void StartSession(SlideSessionModel slideSessionModel, int questionIndex, Slide questionSlide, Slide resultsSlide);

        ValidationResult ActivateClickSession(SlideSessionModel slideSessionModel);

        event EventHandler ShowNextSlideEventHandler;
    }
}
