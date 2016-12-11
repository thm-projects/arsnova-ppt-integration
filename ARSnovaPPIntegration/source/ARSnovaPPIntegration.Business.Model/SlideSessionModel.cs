using System.Collections.ObjectModel;

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

        // TODO: Concept: which default values should be taken?

        public Slide Slide { get; set; }

        public bool NewSession { get; set; } = true;

        public SessionType SessionType { get; set; } = SessionType.ArsnovaClick;

        public bool SessionTypeSet { get; set; } = false;

        public QuestionTypeEnum QuestionType { get; set; }

        public bool QuestionTypeSet { get; set; } = false;

        public string QuestionText { get; set; }

        public ObservableCollection<object> AnswerOptions { get; set; }

        public bool AnswerOptionsSet { get; set; } = false;

        public AnswerOptionType AnswerOptionType { get; set; }

        public int AnswerOptionAmount { get; set; } = 4;

        public AnswerOptionType AnswerOptionInitType { get; set; }
    }
}
