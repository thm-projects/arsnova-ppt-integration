using System.Collections.ObjectModel;

using Microsoft.Office.Interop.PowerPoint;

using ARSnovaPPIntegration.Common.Enum;

namespace ARSnovaPPIntegration.Business.Model
{
    public class SlideSessionModel
    {
        private ObservableCollection<object> answerOptions;

        private string questionText;

        public SlideSessionModel(Slide slide, bool edit = false)
        {
            this.Slide = slide;
        }

        // TODO: Concept: which default values should be taken?

        public Slide Slide { get; set; }

        public string Hashtag { get; set; }

        public bool NewSession { get; set; } = true;

        public SessionType SessionType { get; set; } = SessionType.ArsnovaClick;

        public bool SessionTypeSet { get; set; } = false;

        public QuestionTypeEnum QuestionType { get; set; } = QuestionTypeEnum.SingleChoiceClick;

        public bool QuestionTypeSet { get; set; } = false;

        public string QuestionText
        {
            get
            {
                return this.questionText;
            }
            set
            {
                this.questionText = value;
                this.QuestionTypeSet = true;
            }
        }

        public ObservableCollection<object> AnswerOptions
        {
            get { return this.answerOptions; }
            set
            {
                this.answerOptions = value;
                this.AnswerOptionsSet = true;
            }
        }

        public bool AnswerOptionsSet { get; set; } = false;

        public AnswerOptionType AnswerOptionType { get; set; }

        public int AnswerOptionAmount { get; set; } = 4;

        public AnswerOptionType AnswerOptionInitType { get; set; }
    }
}
