using Microsoft.Office.Interop.PowerPoint;

using ARSnovaPPIntegration.Common.Enum;

namespace ARSnovaPPIntegration.Business.Model
{
    public class SlideSessionModel
    {
        public SlideSessionModel(Slide slide)
        {
            this.Slide = slide;
        }

        public Slide Slide { get; set; }

        // TODO: Concept: which default values should be taken?
        public SessionType SessionType { get; set; } = SessionType.ArsnovaClick;
    }
}
