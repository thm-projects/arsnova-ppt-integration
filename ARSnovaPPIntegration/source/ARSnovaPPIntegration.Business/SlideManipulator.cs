using System;
using ARSnovaPPIntegration.Business.Contract;
using ARSnovaPPIntegration.Common.Contract;
using Microsoft.Office.Core;
using Microsoft.Office.Interop.PowerPoint;
using Microsoft.Practices.ServiceLocation;

namespace ARSnovaPPIntegration.Business
{
    public class SlideManipulator : ISlideManipulator
    {
        private readonly ILocalizationService localizationService;

        public SlideManipulator()
        {
            this.localizationService = ServiceLocator.Current.GetInstance<ILocalizationService>();
        }

        public void AddFooter(Slide slide, string header = "ARSnova Quiz")
        {
            slide.HeadersFooters.Footer.Visible = MsoTriState.msoTrue;
            slide.HeadersFooters.Footer.Text = this.localizationService.Translate(header);
        }

        public void SetArsnovaStyle(Slide slide)
        {
            throw new NotImplementedException();
        }

        public void SetArsnovaClickStyle(Slide arsnovaSlide)
        {
            //var slideSize = Globals.ThisAddIn.Application.ActivePresentation.;

            // Set background
            //var backgroundFileName = "bg.jpg";
            //var bgRect = new RectangleF(new PointF(0, 0), slideSize.);
            arsnovaSlide.Background.Fill.UserPicture(@"C:\Users\Tjark Wilhelm Hoeck\Desktop\fox.jpg");




            //arsnovaSlide.HeadersFooters.Footer.Visible = MsoTriState.msoTrue;
            //arsnovaSlide.HeadersFooters.Header.Text = "arsnova test site";
        }
    }
}
