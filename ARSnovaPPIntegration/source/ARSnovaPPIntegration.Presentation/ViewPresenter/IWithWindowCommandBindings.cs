using System.Collections.Generic;
using System.Windows.Input;

namespace ARSnovaPPIntegration.Presentation.ViewPresenter
{
    public interface IWindowCommandBindings
    {
        List<CommandBinding> WindowCommandBindings { get; }
    }
}
