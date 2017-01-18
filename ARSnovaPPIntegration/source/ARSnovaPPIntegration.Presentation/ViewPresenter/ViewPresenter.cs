using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
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

        private PresentationGroup oldActivePresentationGroup;

        private List<PresentationGroup> presentationGroups = new List<PresentationGroup>();

        public void Add<TViewModel, TView>()
        {
            var viewConfiguration = new ViewTypeConfiguration { ViewModelType = typeof(TViewModel), ViewType = typeof(TView) };

            if (!this.viewTypeConfigurations.ContainsKey(viewConfiguration.ViewModelType))
            {
                this.viewTypeConfigurations.Add(viewConfiguration.ViewModelType, viewConfiguration);
            }
        }

        public void ShowInNewWindow<TViewModel>(TViewModel viewModel, Action<TViewModel> viewModelAction = null) where TViewModel : class
        {
            var newPresentationGroup = new PresentationGroup();

            // show just one window in the taskbar
            newPresentationGroup.Window = new WindowContainer(this) { ShowInTaskbar = !this.presentationGroups.Any() };

            if (this.activePresentationGroup != null)
            {
                this.oldActivePresentationGroup = this.activePresentationGroup;
            }

            var logoBitmap = Images.ARSnova_Logo;
            var iconBitmapSource = System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(
                                             logoBitmap.GetHbitmap(),
                                             IntPtr.Zero,
                                             Int32Rect.Empty,
                                             BitmapSizeOptions.FromWidthAndHeight(16, 16));
            newPresentationGroup.Window.Icon = iconBitmapSource;

            this.presentationGroups.Add(newPresentationGroup);

            viewModelAction?.Invoke(viewModel);

            this.Show(viewModel, newPresentationGroup, true);
        }

        public void Show<TViewModel>(TViewModel viewModel) where TViewModel : class
        {
            var presentationGroup = this.presentationGroups.FirstOrDefault(vm => vm.ViewModel.GetType() == typeof(TViewModel));

            if (presentationGroup != null)
            {
                presentationGroup.Window = this.activePresentationGroup.Window;
            }
            else
            {
                presentationGroup = new PresentationGroup
                                    {
                                        ViewModel = viewModel,
                                        Window = this.activePresentationGroup.Window
                                    };
                this.presentationGroups.Add(presentationGroup);
            }
            
            // reset window events and bindings (because we want to use the same window again)
            this.Close(this.activePresentationGroup.Window.WindowId, false);

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

        public void Close(Guid windowId, bool removeWindow = true)
        {
            var presentationGroupsToClose = this.presentationGroups.FindAll(pg => pg.Window.WindowId == windowId);

            if (presentationGroupsToClose == null)
            {
                throw new ArgumentException($"Window with Id {windowId} not found.");
            }

            var setActivePresentation = true;

            foreach (var presentationGroup in presentationGroupsToClose)
            {
                this.RemoveWindowCommandBindings(presentationGroup.ViewModel, presentationGroup.Window);
                this.RemoveEventHandlers(presentationGroup.ViewModel);

                (presentationGroup.ViewModel as IDisposable)?.Dispose();
                (presentationGroup.View as IDisposable)?.Dispose();

                if (removeWindow)
                {
                    this.presentationGroups.Remove(presentationGroup);

                    if (setActivePresentation)
                    {
                        this.activePresentationGroup = this.oldActivePresentationGroup;
                        this.oldActivePresentationGroup = null;

                        setActivePresentation = false;
                    }
                }
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

        private void RemoveEventHandlers(object viewModel)
        {
            var type = viewModel.GetType();

            // taken from http://stackoverflow.com/questions/3783267/how-to-get-a-delegate-object-from-an-eventinfo
            Func<EventInfo, FieldInfo> ei2Fi =
            ei => this.GetType().GetField(ei.Name,
                BindingFlags.NonPublic |
                BindingFlags.Instance |
                BindingFlags.GetField);

            foreach (var ei in type.GetEvents())
            {
                var fi = ei2Fi(ei);
                var @delegate = fi?.GetValue(viewModel) as Delegate;

                if (@delegate == null)
                {
                    continue;
                }

                foreach (var subscriber in @delegate.GetInvocationList())
                {
                    ei.RemoveEventHandler(viewModel, subscriber);
                }
            }
        }
    }
}
