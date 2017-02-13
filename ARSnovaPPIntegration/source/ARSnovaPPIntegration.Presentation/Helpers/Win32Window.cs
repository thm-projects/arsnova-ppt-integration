using System;
using System.Windows.Interop;

namespace ARSnovaPPIntegration.Presentation.Helpers
{
    public class Win32Window : IWin32Window
    {
        ///<summary>
        /// The <b>Handle</b> of the PowerPoint WindowObject.
        ///</summary>
        public IntPtr Handle { get; }

        ///<summary>
        /// The <b>Win32Window</b> class could be used to get the parent IWin32Window for Wpf and MessageBoxes.
        ///</summary>
        ///<param name="handle">The current handler.</param>
        public Win32Window(IntPtr handle)
        {
            this.Handle = handle;
        }
    }
}
