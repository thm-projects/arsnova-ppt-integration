using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
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
                    // currently in view || no slide selected
                    return null;
                }
            }
        }
    }
}
