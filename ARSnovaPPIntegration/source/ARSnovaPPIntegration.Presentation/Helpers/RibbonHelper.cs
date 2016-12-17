using System;

using Microsoft.Practices.ServiceLocation;
using Microsoft.Office.Interop.PowerPoint;

using ARSnovaPPIntegration.Common.Contract;
using ARSnovaPPIntegration.Presentation.Models;
using ARSnovaPPIntegration.Business.Contract;
using ARSnovaPPIntegration.Business.Model;
using ARSnovaPPIntegration.Common.Contract.Translators;

namespace ARSnovaPPIntegration.Presentation.Helpers
{
    public class RibbonHelper
    {
        private readonly ViewPresenter.ViewPresenter viewPresenter;

        private readonly ILocalizationService localizationService;

        private readonly ISessionManager sessionManager;

        private readonly ISessionInformationProvider sessionInformationProvider;

        private readonly IQuestionTypeTranslator questionTypeTranslator;

        public RibbonHelper(
            ViewPresenter.ViewPresenter viewPresenter,
            ILocalizationService localizationService)
        {
            this.viewPresenter = viewPresenter;
            this.localizationService = localizationService;

            this.sessionManager = ServiceLocator.Current.GetInstance<ISessionManager>();
            this.sessionInformationProvider = ServiceLocator.Current.GetInstance<ISessionInformationProvider>();
            this.questionTypeTranslator = ServiceLocator.Current.GetInstance<IQuestionTypeTranslator>();
        }

        public void StartQuizSetup(Slide slide)
        {
            var slideSessionModel = new SlideSessionModel();

            this.viewPresenter.ShowInNewWindow(
                new SelectArsnovaTypeViewViewModel(
                    new ViewModelRequirements(
                        this.viewPresenter,
                        this.questionTypeTranslator,
                        this.localizationService,
                        this.sessionManager,
                        this.sessionInformationProvider,
                        slideSessionModel)));
        }

        public void EditQuizSetup(Slide slide)
        {
            throw new NotImplementedException();

            var slideSessionModel = new SlideSessionModel(true);

            // TODO Implement edit mode -> retrieve / build model from data in slide and start viewpresenter

            this.viewPresenter.ShowInNewWindow(
                new SelectArsnovaTypeViewViewModel(
                    new ViewModelRequirements(
                        this.viewPresenter,
                        this.questionTypeTranslator,
                        this.localizationService,
                        this.sessionManager,
                        this.sessionInformationProvider,
                        slideSessionModel)));
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
