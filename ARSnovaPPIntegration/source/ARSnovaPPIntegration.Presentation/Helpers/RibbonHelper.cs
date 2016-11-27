using System;

using Microsoft.Office.Interop.PowerPoint;

using ARSnovaPPIntegration.Common.Contract;
using ARSnovaPPIntegration.Presentation.Models;
using ARSnovaPPIntegration.Business.Model;

namespace ARSnovaPPIntegration.Presentation.Helpers
{
    public class RibbonHelper
    {
        private readonly ViewPresenter.ViewPresenter viewPresenter;

        private readonly ILocalizationService localizationService;

        public RibbonHelper(
            ViewPresenter.ViewPresenter viewPresenter,
            ILocalizationService localizationService)
        {
            this.viewPresenter = viewPresenter;
            this.localizationService = localizationService;
        }

        public void StartQuizSetup(Slide slide)
        {
            var slideSessionModel = new SlideSessionModel();

            this.viewPresenter.Show(
                new SelectArsnovaTypeViewModel(
                    this.viewPresenter,
                    this.localizationService,
                    slideSessionModel));
        }

        public void EditQuizSetup(Slide slide)
        {
            // TODO Implement edit mode -> retrieve / build model from data in slide and start viewpresenter
            throw new NotImplementedException();
        }

        public Slide CreateNewSlide()
        {
            var currentSlide = SlideTracker.CurrentSlide;

            return currentSlide == null 
                ? this.CreateNewSlide(Globals.ThisAddIn.Application.ActivePresentation.Slides.Count)
                : this.CreateNewSlide(currentSlide.SlideIndex + 1);
        }

        public Slide CreateNewSlide(int index)
        {
            return Globals.ThisAddIn.Application.ActivePresentation.Slides.Add(index, PpSlideLayout.ppLayoutTitle);
        }
    }
}
