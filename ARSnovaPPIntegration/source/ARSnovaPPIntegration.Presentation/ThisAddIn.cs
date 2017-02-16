using System;
using System.Globalization;
using System.Timers;
using Microsoft.Practices.ServiceLocation;
using Microsoft.Office.Core;
using Microsoft.Office.Interop.PowerPoint;
using Microsoft.Practices.Unity;

using ARSnovaPPIntegration.Common.Contract;
using ARSnovaPPIntegration.Common.Contract.Exceptions;
using ARSnovaPPIntegration.Presentation.Configuration;
using ARSnovaPPIntegration.Presentation.Helpers;
using ARSnovaPPIntegration.Presentation.Models;
using ARSnovaPPIntegration.Presentation.ViewManagement;
using ARSnovaPPIntegration.Presentation.Views;

namespace ARSnovaPPIntegration.Presentation
{
    public partial class ThisAddIn
    {
        private Bootstrapper bootstrapper;

        private ExceptionHandler exceptionHandler;

        private ViewPresenter viewPresenter;

        private Ribbon ribbon;

        private RibbonHelper ribbonHelper;

        private Timer keepAliveTimer;

        private void ThisAddInStartup(object sender, EventArgs e)
        {
            // Add new context menu entries
            // Supported from v2000 until v2016 (current): http://officeone.mvps.org/vba/events_version.html
            //this.Application.WindowBeforeRightClick += this.application_windowBeforeRightClick;

            // Order of bindings are priorities!
            // high priority: window actions
            /* TODO
            ((Microsoft.Office.Interop.PowerPoint.EApplication_Event)Application).NewPresentation += OnNewPresentation;
            Application.AfterNewPresentation += OnAfterNewPresentation;
            Application.PresentationOpen += OnPrensentationOpen;
            Application.PresentationClose += OnPresentationClose;*/

            // mid priority: window actions
            
            this.Application.SlideShowBegin += this.OnSlideShowBegin;
            this.Application.SlideShowNextSlide += this.OnSlideChange;
            this.Application.SlideShowOnPrevious += this.OnSlideChange;
            this.Application.SlideShowEnd += this.OnSlideShowEnd;

            // low priority: slide actions
            this.Application.SlideSelectionChanged += this.OnSlideSelectionChanged;
        }
        
        /* private void ThisAddIn_Shutdown(object sender, System.EventArgs e)
        {
            // Clean up events?
        }*/

        /*public void application_windowBeforeRightClick(Selection selection, ref bool cancel)
        {
            if (selection != null && selection.Type == PpSelectionType.ppSelectionSlides && selection.SlideRange != null)
            {
              
            }
        }*/

        private void OnSlideShowBegin(SlideShowWindow slideShowWindow)
        {
            try
            {
                this.ribbonHelper.ActivateSessionIfExists();

                if (this.ribbonHelper.IsARSnovaClickSession())
                {
                    this.ribbonHelper.CleanUpOnStart();

                    this.keepAliveTimer = new Timer();
                    this.keepAliveTimer.Elapsed += this.KeepAliveEvent;
                    this.keepAliveTimer.Interval = 180000;
                    this.keepAliveTimer.Enabled = true;
                }
            }
            catch (CommunicationException arsnovaComException)
            {
                this.exceptionHandler.Handle(arsnovaComException.Message);
            }
            catch (Exception e)
            {
                this.exceptionHandler.Handle(e.Message);
            }
        }

        private void KeepAliveEvent(object source, ElapsedEventArgs e)
        {
            var slideSessionModel = PresentationInformationStore.GetStoredSlideSessionModel();

            this.ribbonHelper.SendKeepAlive(slideSessionModel);
        }

        private void OnSlideChange(SlideShowWindow slideShowWindow)
        {
            // TODO check if timer is running and cancel next or previous event!

            // start arsnova click question when getting on results slide
            var isSlideStartArsnovaClickQuestion = SlideTracker.IsPresentationOnStartArsnovaSlide();

            if (isSlideStartArsnovaClickQuestion.Item1)
            {
                this.ribbonHelper.StartQuiz(isSlideStartArsnovaClickQuestion.Item2);
                return;
            }

            var isSlideArsnovaVotingResultsSlide = SlideTracker.IsPresentationOnResultsArsnovaVotingSlide();

            if (isSlideArsnovaVotingResultsSlide.Item1)
            {
                //GetResults
                this.ribbonHelper.GetAndDisplayArsnovaVotingResults(isSlideArsnovaVotingResultsSlide.Item2);
            }
        }

        private void OnSlideShowEnd(Microsoft.Office.Interop.PowerPoint.Presentation presentation)
        {
            this.ribbonHelper.RemoveClickQuizDataOnServer();
            this.keepAliveTimer?.Dispose();
        }

        private void OnSlideSelectionChanged(SlideRange slideRange)
        {
            this.ribbon.OneArsnovaSlideSelected = false;

            if (slideRange.Count == 1)
            {
                var selectedSlide = slideRange[1];

                if (SlideTracker.IsArsnovaSlide(selectedSlide))
                {
                    this.ribbon.OneArsnovaSlideSelected = true;
                }
            }

            // update control ui's
            this.ribbon.RefreshRibbonControl("AddButton");
            this.ribbon.RefreshRibbonControl("EditButton");
            this.ribbon.RefreshRibbonControl("DeleteButton");
            this.ribbon.RefreshRibbonControl("HideButton");
        }

        private void Setup()
        {
            // Setup Unity
            this.bootstrapper = new Bootstrapper();
            this.bootstrapper.SetupUnityServiceLocator();

            // Setup ExceptionHandler
            // TODO setup not complete (doesn't catch everythink)
            this.exceptionHandler = new ExceptionHandler();

            // Setup ViewPresenter
            this.ConfigureViewPresenter();

        }

        private void ConfigureViewPresenter()
        {
            this.viewPresenter = new ViewPresenter();
            this.viewPresenter.Add<SelectArsnovaTypeViewViewModel, SelectArsnovaTypeView>();
            this.viewPresenter.Add<QuestionViewViewModel, QuestionView>();
            this.viewPresenter.Add<AnswerOptionViewViewModel, AnswerOptionView>();
            this.viewPresenter.Add<SessionOverviewViewViewModel, SessionOverviewView>();

            this.bootstrapper.UnityContainer.RegisterInstance<IViewPresenter>(this.viewPresenter);
            this.bootstrapper.UnityContainer.RegisterInstance<ViewPresenter>(this.viewPresenter);
        }

        protected override IRibbonExtensibility CreateRibbonExtensibilityObject()
        {
            // set the cultureinfo according to the running office instance
            var app = this.GetHostItem<Application>(typeof(Application), "Application");
            var languageId = app.LanguageSettings.LanguageID[MsoAppLanguageID.msoLanguageIDUI];
            System.Threading.Thread.CurrentThread.CurrentUICulture = new CultureInfo(languageId);

            // is called on office load (create ribbon bar) -> init here instead of startup because some dependencies are already needed
            this.Setup();

            this.ribbonHelper = new RibbonHelper(this.viewPresenter, ServiceLocator.Current.GetInstance<ILocalizationService>());

            this.ribbon = new Ribbon(this.viewPresenter, this.ribbonHelper);

            return this.ribbon;
        }

        #region Von VSTO generierter Code

        /// <summary>
        /// Erforderliche Methode für die Designerunterstützung.
        /// Der Inhalt der Methode darf nicht mit dem Code-Editor geändert werden.
        /// </summary>
       private void InternalStartup()
        {
            // already init on CreateRibbon
            this.Startup += this.ThisAddInStartup;
            //this.Shutdown += new System.EventHandler(ThisAddIn_Shutdown);
        }

        #endregion
    }
}
