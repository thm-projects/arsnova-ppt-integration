using System;
using System.Collections.ObjectModel;
using ARSnovaPPIntegration.Common.Enum;

using Excel = Microsoft.Office.Interop.Excel;

namespace ARSnovaPPIntegration.Business.Model
{
    public class SlideQuestionModel
    {
        private ObservableCollection<GeneralAnswerOption> answerOptions;

        private string questionText;

        private QuestionTypeEnum questionType = QuestionTypeEnum.SingleChoiceClick;

        private Excel.XlChartType chartType = Excel.XlChartType.xl3DBarClustered;

        public SlideQuestionModel()
        {
            this.Id = Guid.NewGuid();
        }

        public event EventHandler ObjectChangedEventHandler;

        public string ArsnovaVotingId { get; set; }

        public int QuestionInfoSlideId { get; set; }

        public int? QuestionTimerSlideId { get; set; }

        public int? ResultsSlideId { get; set; }

        public string SlideNumbers { get; set; }

        public int Index { get; set; }

        public int RecalculatedOnlineIndex { get; set; }

        public Guid Id { get; set; }

        public int Countdown { get; set; } = 20;

        public Excel.XlChartType ChartType
        {
            get { return this.chartType; }
            set
            {
                this.chartType = value;
                this.ObjectChangedEventHandler?.Invoke(this, EventArgs.Empty);
            }
        }

        public QuestionTypeEnum QuestionType
        {
            get { return this.questionType; }
            set
            {
                this.questionType = value;
                this.ObjectChangedEventHandler?.Invoke(this, EventArgs.Empty);
            }
        }

        public string QuestionTypeText { get; set; }

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
                this.ObjectChangedEventHandler?.Invoke(this, EventArgs.Empty);
            }
        }

        public ObservableCollection<GeneralAnswerOption> AnswerOptions
        {
            get { return this.answerOptions; }
            set
            {
                this.answerOptions = value;
                this.AnswerOptionsSet = true;
            }
        }

        public bool AnswerOptionsSet { get; set; } = false;

        public int AnswerOptionAmount { get; set; } = 4;

        public QuestionTypeEnum QuestionInitType { get; set; }

        public void AnswerOptionModelChanged()
        {
            this.ObjectChangedEventHandler?.Invoke(this, EventArgs.Empty);
        }

        public bool Hidden { get; set; } = false;
    }
}
