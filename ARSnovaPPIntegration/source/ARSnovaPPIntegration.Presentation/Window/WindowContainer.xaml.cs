using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

using ARSnovaPPIntegration.Presentation.Commands;
using ARSnovaPPIntegration.Presentation.Models;

namespace ARSnovaPPIntegration.Presentation.Window
{
    /// <summary>
    /// Interaktionslogik für WindowContainer.xaml
    /// </summary>
    public partial class WindowContainer : INotifyPropertyChanged
    {
        private ViewPresenter.ViewPresenter viewPresenter;

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
            var close = PopUpWindow.CloseWindowPrompt();

            if (!close) {
                e.Cancel = true;
            }

            this.viewPresenter.ExternalWindowClose();
        }
    }
}
