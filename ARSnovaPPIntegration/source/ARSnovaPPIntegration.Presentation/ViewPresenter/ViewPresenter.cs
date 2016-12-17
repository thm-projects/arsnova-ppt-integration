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

        private PresentationGroup activePresentationGroup;

        private List<PresentationGroup> presentationGroups = new List<PresentationGroup>();

        public void Add<TViewModel, TView>()
        {
            var viewConfiguration = new ViewTypeConfiguration { ViewModelType = typeof(TViewModel), ViewType = typeof(TView) };

            if (!this.viewTypeConfigurations.ContainsKey(viewConfiguration.ViewModelType))
            {
                this.viewTypeConfigurations.Add(viewConfiguration.ViewModelType, viewConfiguration);
            }
        }

        public void ShowInNewWindow<TViewModel>(TViewModel viewModel) where TViewModel : class
        {
            var newPresentationGroup = new PresentationGroup();

            // show just one window in the taskbar
            newPresentationGroup.Window = new WindowContainer(this) { ShowInTaskbar = !this.presentationGroups.Any() };

            var logoBitmap = Images.ARSnova_Logo;
            var iconBitmapSource = System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(
                                             logoBitmap.GetHbitmap(),
                                             IntPtr.Zero,
                                             Int32Rect.Empty,
                                             BitmapSizeOptions.FromWidthAndHeight(16, 16));
            newPresentationGroup.Window.Icon = iconBitmapSource;

            this.presentationGroups.Add(newPresentationGroup);

            this.Show(viewModel, newPresentationGroup, true);
        }

        public void Show<TViewModel>(TViewModel viewModel) where TViewModel : class
        {
            this.ContentCleanUp(this.activePresentationGroup.Window.WindowId, false);

            this.Show(viewModel, this.activePresentationGroup);
        }

        public void CloseWithoutPrompt()
        {
            this.activePresentationGroup.Window.ShowCloseWindowPrompt = false;
            this.activePresentationGroup.Window.Close();
        }

        public void CloseWithPrompt()
        {
            this.activePresentationGroup.Window.Close();
        }

        public void ContentCleanUp(Guid windowId, bool removeWindow = true)
        {
            var presentationGroup = this.presentationGroups.First(pg => pg.Window.WindowId == windowId);

            if (presentationGroup == null)
            {
                throw new ArgumentException($"Window with Id {windowId} not found.");
            }

            // TODO do I need to clean up event handlers / bindings (-> yes, done)? I think there should be any, check later!
            this.RemoveWindowCommandBindings(presentationGroup.ViewModel, presentationGroup.Window);

            (presentationGroup.ViewModel as IDisposable)?.Dispose();

            if (removeWindow)
            {
                if (this.activePresentationGroup.Window.WindowId == presentationGroup.Window.WindowId)
                {
                    this.activePresentationGroup = null;
                }

                this.presentationGroups.Remove(presentationGroup);
            }
        }

        private void Show<TViewModel>(TViewModel viewModel, PresentationGroup presentationGroup, bool isNewWindow = false) where TViewModel : class
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

            this.SetWindowCommandBindings(viewModel, presentationGroup.Window);

            presentationGroup.Window.Content.Children.Clear();
            presentationGroup.Window.Content.Children.Add(view);

            presentationGroup.ViewModel = viewModel;
            presentationGroup.View = view;

            this.activePresentationGroup = presentationGroup;

            // show -> calling prog doesn't wait (and freezes), showDialog() -> caller waits.... do we want to freeze pp? -> we want to freeze! 
            // side effect: we don't have to handle multiple windows -> the freshly openend one needs to be closed before opening another one (popups doesn't matter)

            if (isNewWindow)
            {
                presentationGroup.Window.ShowDialog();
            }
        }

        private void SetWindowCommandBindings(object viewModel, WindowContainer window)
        {
            var windowCommandsInViewModel = viewModel as IWindowCommandBindings;

            var windowCommandBindings = windowCommandsInViewModel?.WindowCommandBindings;

            if (windowCommandBindings == null)
            {
                throw new ArgumentException($"IWindowCommandBindings not implemented for ViewModel: '{viewModel.GetType().FullName}'");
            }

            window.SetWindowCommandBindings(windowCommandBindings);

            /*var baseViewModel = viewModel as BaseModel;

            if (baseViewModel == null)
            {
                throw new ArgumentException($"ViewModel isn't implementing the BaseModel: '{viewModel.GetType().FullName}'");
            }*/
        }
        
        private void RemoveWindowCommandBindings(object viewModel, WindowContainer window)
        {
            var windowCommandsInViewModel = viewModel as IWindowCommandBindings;

            var windowCommandBindings = windowCommandsInViewModel?.WindowCommandBindings;

            if (windowCommandBindings != null)
            {
                foreach (var windowCommandBinding in windowCommandBindings)
                {
                    window.CommandBindings.Remove(windowCommandBinding);
                }
            }
        }
    }
}
