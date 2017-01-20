using System.Collections.Generic;
using System.Windows.Input;

namespace ARSnovaPPIntegration.Presentation.ViewManagement
{
    public interface IWindowCommandBindings
    {
        List<CommandBinding> WindowCommandBindings { get; }
    }
}
