using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

using ARSnovaPPIntegration.Presentation.Content;
using ARSnovaPPIntegration.Presentation.Window;

namespace ARSnovaPPIntegration.Presentation.ViewPresenter
{
    public class ViewPresenter
    {
        private readonly Dictionary<Type, ViewTypeConfiguration> viewTypeConfigurations =
            new Dictionary<Type, ViewTypeConfiguration>();

        private object runningViewModel;

        private Control runningView;

        private WindowContainer window;

        public void Add<TViewModel, TView>()
        {
            var viewConfiguration = new ViewTypeConfiguration { ViewModelType = typeof(TViewModel), ViewType = typeof(TView) };

            if (!this.viewTypeConfigurations.ContainsKey(viewConfiguration.ViewModelType))
            {
                this.viewTypeConfigurations.Add(viewConfiguration.ViewModelType, viewConfiguration);
            }
        }

        public void Show<TViewModel>(TViewModel viewModel) where TViewModel : class
        {
            var viewModelType = typeof(TViewModel);

            if (!this.viewTypeConfigurations.ContainsKey(viewModelType))
            {
                throw new ArgumentException($"ViewModel not found: '{viewModelType.FullName}'");
            }

            var viewTypeConfiguration = this.viewTypeConfigurations[viewModelType];

            // there are currently no view constructors with params -> add option for constructor calls with elements when necessary
            var view =
                (Control)viewTypeConfiguration.ViewType.GetConstructors()
                                     .FirstOrDefault(c => !c.GetParameters().Any())
                                     .Invoke(new object[0]);
            view.DataContext = viewModel;

            var isNewWindow = false;

            if (this.window == null)
            {
                this.window = new WindowContainer(this) {ShowInTaskbar = true};
                var logoBitmap = Images.ARSnova_Logo;
                var iconBitmapSource = System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(
                                                 logoBitmap.GetHbitmap(),
                                                 IntPtr.Zero,
                                                 Int32Rect.Empty,
                                                 BitmapSizeOptions.FromWidthAndHeight(16, 16));
                this.window.Icon = iconBitmapSource;

                isNewWindow = true;
            }
            else
            {
                this.ContentCleanUp();
            }

            this.SetWindowCommandBindings(viewModel);

            this.window.Content.Children.Clear();
            this.window.Content.Children.Add(view);

            this.runningViewModel = viewModel;
            this.runningView = view;

            // show -> calling prog doesn't wait (and freezes), showDialog() -> caller waits.... do we want to freeze pp? -> we want to freeze! 
            // side effect: we don't have to handle multiple windows -> the freshly openend one needs to be closed before opening another one (popups doesn't matter)

            if (isNewWindow)
            {
                this.window.ShowDialog();
            }
        }

        public void ContentCleanUp(bool closeWindow = false)
        {
            // TODO do I need to clean up event handlers / bindings (-> yes, done)? I think there should be any, check later!
            this.RemoveWindowCommandBindings(this.runningViewModel);

            (this.runningViewModel as IDisposable)?.Dispose();
            (this.runningView as IDisposable)?.Dispose();
            this.runningViewModel = null;
            this.runningView = null;

            if (closeWindow)
            {
                this.window.Close();
            }
        }

        public void ExternalWindowClose()
        {
            this.ContentCleanUp();
            this.window = null;
        }

        private void SetWindowCommandBindings(object viewModel)
        {
            var windowCommandsInViewModel = viewModel as IWindowCommandBindings;

            var windowCommandBindings = windowCommandsInViewModel?.WindowCommandBindings;

            if (windowCommandBindings == null)
            {
                throw new ArgumentException($"IWindowCommandBindings not implemented for ViewModel: '{viewModel.GetType().FullName}'");
            }

            this.window.SetWindowCommandBindings(windowCommandBindings);

            /*var baseViewModel = viewModel as BaseModel;

            if (baseViewModel == null)
            {
                throw new ArgumentException($"ViewModel isn't implementing the BaseModel: '{viewModel.GetType().FullName}'");
            }*/
        }
        
        private void RemoveWindowCommandBindings(object viewModel)
        {
            var windowCommandsInViewModel = viewModel as IWindowCommandBindings;

            var windowCommandBindings = windowCommandsInViewModel?.WindowCommandBindings;

            if (windowCommandBindings != null)
            {
                foreach (var windowCommandBinding in windowCommandBindings)
                {
                    this.window.CommandBindings.Remove(windowCommandBinding);
                }
            }
        }
    }
}
