using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ARSnovaPPIntegration.Common.Contract;
using Microsoft.Office.Core;
using Microsoft.Office.Interop.PowerPoint;
using Microsoft.Practices.Unity;

namespace ARSnovaPPIntegration.Presentation.Helpers
{
    public class SlideManipulator
    {
        private readonly ILocalizationService localizationService;

        private Slide slide;

        public SlideManipulator(IUnityContainer unityContainer, Slide slide)
        {
            this.localizationService = unityContainer.Resolve<ILocalizationService>();

            this.slide = slide;
        }

        public SlideManipulator AddFooter(string header = "ARSnova Quiz")
        {
            this.slide.HeadersFooters.Footer.Visible = MsoTriState.msoTrue;
            this.slide.HeadersFooters.Footer.Text = this.localizationService.Translate(header);
            return this;
        }

        public Slide Build()
        {
            return this.slide;
        }
    }
}
