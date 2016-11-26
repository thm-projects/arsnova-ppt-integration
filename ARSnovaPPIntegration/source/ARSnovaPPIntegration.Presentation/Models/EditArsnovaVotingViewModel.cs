﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using ARSnovaPPIntegration.Business.Model;
using ARSnovaPPIntegration.Common.Contract;
using ARSnovaPPIntegration.Presentation.Commands;
using ARSnovaPPIntegration.Presentation.ViewPresenter;
using ARSnovaPPIntegration.Presentation.Window;
using ARSnovaPPIntegration.Common.Enum;

namespace ARSnovaPPIntegration.Presentation.Models
{
    public class EditArsnovaVotingViewModel : OnPropertyChangedBaseModel, IWindowCommandBindings
    {
        private readonly ViewPresenter.ViewPresenter viewPresenter;

        private readonly ILocalizationService localizationService;

        private SlideSessionModel slideSessionModel;

        public List<CommandBinding> WindowCommandBindings { get; } = new List<CommandBinding>();

        public EditArsnovaVotingViewModel(
            ViewPresenter.ViewPresenter viewPresenter,
            ILocalizationService localizationService,
            SlideSessionModel slideSessionModel)
        {
            this.viewPresenter = viewPresenter;
            this.localizationService = localizationService;

            this.slideSessionModel = slideSessionModel;

            this.InitializeWindowCommandBindings();
        }

        public bool IsArsnovaClickSession
        {
            get { return this.slideSessionModel.SessionType == SessionType.ArsnovaClick; }
            set
            {
                if (value)
                {
                    this.slideSessionModel.SessionType = SessionType.ArsnovaClick;
                    this.OnPropertyChanged(nameof(this.IsArsnovaVotingSession));
                }
            }
        }

        public bool IsArsnovaVotingSession
        {
            get { return this.slideSessionModel.SessionType == SessionType.ArsnovaVoting; }
            set
            {
                if (value)
                {
                    this.slideSessionModel.SessionType = SessionType.ArsnovaVoting;
                    this.OnPropertyChanged(nameof(this.IsArsnovaClickSession));
                }
            }
        }

        private void InitializeWindowCommandBindings()
        {
            this.WindowCommandBindings.AddRange(
                    new List<CommandBinding>
                    {
                        new CommandBinding(
                            NavigationButtonCommands.Cancel,
                            (e, o) =>
                            {
                                var cancel = PopUpWindow.ConfirmationWindow(
                                    this.localizationService.Translate("Cancel"),
                                    this.localizationService.Translate(
                                            "If this process is canceld, every progress will be deleted. Do you like to continue?"));
                                if (cancel)
                                {
                                    this.viewPresenter.Close<EditArsnovaVotingViewModel>();
                                }
                            },
                            (e, o) => o.CanExecute = true),
                        new CommandBinding(
                            NavigationButtonCommands.Forward,
                            (e, o) =>
                            {
                                // TODO ViewPresenter: Forward to next view
                            },
                            (e, o) => o.CanExecute = true)
                    });
        }
    }
}
