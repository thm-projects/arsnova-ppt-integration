using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;

using ARSnovaPPIntegration.Presentation.Commands;
using ARSnovaPPIntegration.Presentation.ViewManagement;

namespace ARSnovaPPIntegration.Presentation.Window
{
    /// <summary>
    /// Interaktionslogik für WindowContainer.xaml
    /// </summary>
    public partial class WindowContainer : INotifyPropertyChanged
    {
        private readonly ViewPresenter viewPresenter;

        public bool ShowCloseWindowPrompt { get; set; } = true;

        public NavigationButtonsToolTips NavigationButtonsToolTips { get; } = new NavigationButtonsToolTips();

        public WindowContainer(ViewPresenter viewPresenter)
        {
            this.viewPresenter = viewPresenter;
            this.InitializeComponent();
            this.DataContext = this;

            this.WindowId = Guid.NewGuid();
        }

        public Guid WindowId { get; private set; }

        public bool BackButtonVisibility
        {
            get
            {
                return this.CommandBindings.OfType<CommandBinding>().Any(c => c.Command == NavigationButtonCommands.Back);
            }
        }

        public bool ForwardButtonVisibility
        {
            get
            {
                return this.CommandBindings.OfType<CommandBinding>().Any(c => c.Command == NavigationButtonCommands.Forward);
            }
        }

        public bool CancelButtonVisibility
        {
            get
            {
                return this.CommandBindings.OfType<CommandBinding>().Any(c => c.Command == NavigationButtonCommands.Cancel);
            }
        }

        public bool FinishButtonVisibility
        {
            get
            {
                return this.CommandBindings.OfType<CommandBinding>().Any(c => c.Command == NavigationButtonCommands.Finish);
            }
        }

        public bool NewButtonVisibility
        {
            get
            {
                return this.CommandBindings.OfType<CommandBinding>().Any(c => c.Command == NavigationButtonCommands.New);
            }
        }

        public bool EditButtonVisibility
        {
            get
            {
                return this.CommandBindings.OfType<CommandBinding>().Any(c => c.Command == NavigationButtonCommands.Edit);
            }
        }

        public bool DeleteButtonVisibility
        {
            get
            {
                return this.CommandBindings.OfType<CommandBinding>().Any(c => c.Command == NavigationButtonCommands.Delete);
            }
        }

        public bool ToolBarVisibility
        {
            get { return this.NewButtonVisibility || this.EditButtonVisibility || this.DeleteButtonVisibility; }
        }
        public void SetWindowCommandBindings(List<CommandBinding> commandBindings)
        {
            this.CommandBindings.AddRange(commandBindings);
            this.OnPropertyChanged("CommandBindings");

            // This is affecting the button visibilities, too
            this.OnPropertyChanged("BackButtonVisibility");
            this.OnPropertyChanged("ForwardButtonVisibility");
            this.OnPropertyChanged("CancelButtonVisibility");
            this.OnPropertyChanged("FinishButtonVisibility");
            this.OnPropertyChanged("NewButtonVisibility");
            this.OnPropertyChanged("EditButtonVisibility");
            this.OnPropertyChanged("DeleteButtonVisibility");
            this.OnPropertyChanged("ToolBarVisibility");
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void Window_Closing(object sender, CancelEventArgs e)
        {
            var windowContainer = sender as WindowContainer;

            if (windowContainer == null)
                return;

            if (windowContainer.ShowCloseWindowPrompt)
            {
                var close = PopUpWindow.CloseWindowPrompt();

                if (!close)
                {
                    e.Cancel = true;
                }
                else
                {
                    this.viewPresenter.Close(this.WindowId);
                }
            }
            else
            {
                windowContainer.ShowCloseWindowPrompt = true;
                this.viewPresenter.Close(this.WindowId);
            }
        }
    }
}
