using System.Runtime.InteropServices;

using Microsoft.Office.Interop.PowerPoint;

namespace ARSnovaPPIntegration.Presentation.Helpers
{
    public static class SlideTracker
    {
        public static Slide CurrentSlide
        {
            get
            {
                try
                {
                    return Globals.ThisAddIn.Application.ActiveWindow.View.Slide as Slide;
                }
                catch (COMException)
                {
                    // no slide selected or currently in view
                    return null;
                }
            }
        }

        public static bool IsArsnovaSlide(Slide slide)
        {
            return slide.Name.StartsWith("ArsnovaSlide");
        }
    }
}
