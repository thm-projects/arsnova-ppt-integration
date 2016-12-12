using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows.Input;

using ARSnovaPPIntegration.Presentation.Commands;

namespace ARSnovaPPIntegration.Presentation.Window
{
    /// <summary>
    /// Interaktionslogik für WindowContainer.xaml
    /// </summary>
    public partial class WindowContainer : INotifyPropertyChanged
    {
        private readonly ViewPresenter.ViewPresenter viewPresenter;

        public bool ShowCloseWindowPrompt { get; set; } = true;

        public NavigationButtonsToolTips NavigationButtonsToolTips { get; } = new NavigationButtonsToolTips();

        public WindowContainer(ViewPresenter.ViewPresenter viewPresenter)
        {
            this.viewPresenter = viewPresenter;
            this.InitializeComponent();
            this.DataContext = this;
        }

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

        public void SetWindowCommandBindings(List<CommandBinding> commandBindings)
        {
            this.CommandBindings.AddRange(commandBindings);
            this.OnPropertyChanged("CommandBindings");

            // This is affecting the button visibilities, too
            this.OnPropertyChanged("BackButtonVisibility");
            this.OnPropertyChanged("ForwardButtonVisibility");
            this.OnPropertyChanged("CancelButtonVisibility");
            this.OnPropertyChanged("FinishButtonVisibility");
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
                    this.viewPresenter.ContentCleanUp();
                }
            }
            else
            {
                windowContainer.ShowCloseWindowPrompt = true;
                this.viewPresenter.ContentCleanUp();
            }
        }
    }
}
