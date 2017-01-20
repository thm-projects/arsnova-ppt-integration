using System;

namespace ARSnovaPPIntegration.Presentation.ViewManagement
{
    public interface IViewPresenter
    {
        void Add<TViewModel, TView>();

        void ShowInNewWindow<TViewModel>(TViewModel viewModel, Action<TViewModel> viewModelAction = null)
            where TViewModel : class;

        void Show<TViewModel>(TViewModel viewModel) where TViewModel : class;

        void CloseWithoutPrompt();

        void CloseWithPrompt();

        void Close(Guid windowId, bool removeWindow = true);
    }
}
