using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ARSnovaPPIntegration.Presentation.ViewPresenter
{
    public interface IViewPresenter
    {
        void Show<TViewModel>(TViewModel viewModel) where TViewModel : class;
    }
}
