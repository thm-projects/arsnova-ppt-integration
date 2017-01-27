using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Linq;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using ARSnovaPPIntegration.Business.Model;
using ARSnovaPPIntegration.Presentation.Commands;
using ARSnovaPPIntegration.Common.Enum;
using ARSnovaPPIntegration.Presentation.Content;
using ARSnovaPPIntegration.Presentation.Helpers;
using ARSnovaPPIntegration.Presentation.Window;

namespace ARSnovaPPIntegration.Presentation.Models
{
    public class SelectArsnovaTypeViewViewModel : BaseViewModel
    {
        public SelectArsnovaTypeViewViewModel(ViewModelRequirements requirements)
            : base(requirements)
        {
            this.InitializeWindowCommandBindings();

            if (string.IsNullOrEmpty(this.SlideSessionModel.Hashtag))
            {
                var presentationName = Globals.ThisAddIn.Application.ActivePresentation.Name;
                var hashtagList = requirements.SessionInformationProvider.GetHashtagList();

                while (hashtagList.Any(h => h.ToLower() == presentationName.ToLower()))
                {
                    presentationName += "1";
                }

                this.SlideSessionModel.Hashtag = presentationName;
            }
        }

        public delegate void OnSelectArsnovaTypeViewCloseEventHandler();

        public event OnSelectArsnovaTypeViewCloseEventHandler OnSelectArsnovaTypeViewClose;

        public bool IsArsnovaClickSession
        {
            get { return this.SlideSessionModel.SessionType == SessionType.ArsnovaClick; }
            set
            {
                if (value)
                {
                    if (this.SlideSessionModel.Questions.Any())
                    {
                        var reset = PopUpWindow.ConfirmationWindow(
                            this.LocalizationService.Translate("Reset"),
                            this.LocalizationService.Translate(
                                    "If this value is changed, other Session-Properties like the answer options or the question type will be reseted. Do you want to continue?"));

                        if (reset)
                        {
                            this.SlideSessionModel.SessionType = SessionType.ArsnovaClick;
                            this.OnSessionTypeSelectionChanged();
                            this.SlideSessionModel.Questions = new ObservableCollection<SlideQuestionModel>();
                        }
                    }
                    else
                    {
                        this.SlideSessionModel.SessionType = SessionType.ArsnovaClick;
                        this.OnSessionTypeSelectionChanged();
                    }
                } 
            }
        }

        public string Hashtag
        {
            get { return this.SlideSessionModel.Hashtag; }
            set { this.SlideSessionModel.Hashtag = value; }
        }

        public BitmapImage ArsnovaClickLogo => BitmapToBitmapImageConverter.ConvertBitmapImageToBitmap(Images.ARSnovaClick_Logo);

        public BitmapImage ArsnovaVotingLogo => BitmapToBitmapImageConverter.ConvertBitmapImageToBitmap(Images.ARSnova_Logo);

        public bool IsArsnovaVotingSession
        {
            get { return this.SlideSessionModel.SessionType == SessionType.ArsnovaVoting; }
            set
            {
                if (value)
                {
                    if (this.SlideSessionModel.Questions.Any())
                    {
                        var reset = PopUpWindow.ConfirmationWindow(
                            this.LocalizationService.Translate("Reset"),
                            this.LocalizationService.Translate(
                                    "If this value is changed, other Session-Properties like the answer options or the question type will be reseted. Do you want to continue?"));

                        if (reset)
                        {
                            this.SlideSessionModel.SessionType = SessionType.ArsnovaVoting;
                            this.OnSessionTypeSelectionChanged();

                            SlideTracker.RemoveSlide(this.SlideSessionModel.IntroSlideId);

                            foreach (var slideQuestionModel in this.SlideSessionModel.Questions)
                            {
                                SlideTracker.RemoveSlide(slideQuestionModel.QuestionInfoSlideId);

                                if (!slideQuestionModel.QuizInOneShape)
                                {
                                    SlideTracker.RemoveSlide(slideQuestionModel.ResultsSlideId);
                                    SlideTracker.RemoveSlide(slideQuestionModel.QuestionTimerSlideId);
                                }
                            }

                            this.SlideSessionModel.Questions = new ObservableCollection<SlideQuestionModel>(); 
                        }
                    }
                    else
                    {
                        this.SlideSessionModel.SessionType = SessionType.ArsnovaVoting;
                        this.OnSessionTypeSelectionChanged();
                    }
                }
            }
        }

        public string Header => this.LocalizationService.Translate("New question");

        public string Text
            =>
            this.LocalizationService.Translate(
                    "Which type of question do you want to ask? Arsnova.voting is the serious, grown up one while arsnova.click is faster, more colorful and crammed up with gamification.");

        private void OnSessionTypeSelectionChanged()
        {
            this.OnPropertyChanged(nameof(this.IsArsnovaClickSession));
            this.OnPropertyChanged(nameof(this.IsArsnovaVotingSession));
        }

        private void InitializeWindowCommandBindings()
        {
            this.WindowCommandBindings.AddRange(
                    new List<CommandBinding>
                    {
                        new CommandBinding(
                            NavigationButtonCommands.Finish,
                            (e, o) =>
                            {
                                this.SessionManager.SetHashtag(this.SlideSessionModel);
                                this.SlideSessionModel.SessionTypeSet = true;
                                PresentationInformationStore.StoreSlideSessionModel(this.SlideSessionModel);
                                this.OnSelectArsnovaTypeViewClose?.Invoke();
                                this.ViewPresenter.CloseWithoutPrompt();
                            },
                            (e, o) => o.CanExecute = true)
                    });
        }
    }
}
