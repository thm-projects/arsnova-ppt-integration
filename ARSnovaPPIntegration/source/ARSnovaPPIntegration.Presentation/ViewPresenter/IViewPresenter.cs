using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ARSnovaPPIntegration.Presentation.ViewPresenter
{
    public interface IViewPresenter
    {
        void Show<TViewModel>(Action<TViewModel> viewModelAction = null) where TViewModel : class;
    }
}
