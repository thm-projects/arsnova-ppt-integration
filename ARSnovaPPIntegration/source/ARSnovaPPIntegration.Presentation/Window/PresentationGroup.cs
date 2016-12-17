using System.Windows.Controls;

namespace ARSnovaPPIntegration.Presentation.Window
{
    public class PresentationGroup
    {
        public object ViewModel { get; set; }

        public Control View { get; set; }

        public WindowContainer Window { get; set; }
    }
}
