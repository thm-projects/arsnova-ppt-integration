using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Interop;
using ARSnovaPPIntegration.Presentation.Models;

namespace ARSnovaPPIntegration.Presentation.ViewPresenter
{
    public class ViewPresenter : IViewPresenter
    {
        private readonly Dictionary<Type, ViewTypeConfiguration> viewTypeConfigurations =
            new Dictionary<Type, ViewTypeConfiguration>();

        private System.Windows.Window window;

        public void Add<TViewModel, TView>()
        {
            var viewConfiguration = new ViewTypeConfiguration { ViewModelType = typeof(TViewModel), ViewType = typeof(TView) };

            if (!this.viewTypeConfigurations.ContainsKey(viewConfiguration.ViewModelType))
            {
                this.viewTypeConfigurations.Add(viewConfiguration.ViewModelType, viewConfiguration);
            }
        }

        public void Show<TViewModel>(Action<TViewModel> viewModelAction = null) where TViewModel : class
        {
            var viewModelType = typeof(TViewModel);

            if (!this.viewTypeConfigurations.ContainsKey(viewModelType))
            {
                throw new ArgumentException($"ViewModel not found: '{viewModelType.FullName}'");
            }
        }

        public void Start()
        {
            var startViewModel = new SetupViewModel();
        }

        public void Exit()
        {
            this.window.Close();
            this.window.Visibility = Visibility.Collapsed;
            this.window.ShowInTaskbar = false;

            Application.Current.Shutdown();
        }

        public void Minimize()
        {
            this.window.WindowState = WindowState.Minimized;
        }

        public void Maximize()
        {
            this.window.WindowState = WindowState.Maximized;
        }

        public void Restore()
        {
            this.window.WindowState = WindowState.Normal;
        }
    }
}
