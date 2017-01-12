using System;
using ARSnovaPPIntegration.Common.Enum;

namespace ARSnovaPPIntegration.Business.Model
{
    public class GeneralAnswerOption
    {
        private int position;

        private string text;

        private bool isTrue;

        private int lowerLimit;

        private int correct;

        private int higherLimit;

        public event EventHandler ObjectChangedEventHandler;

        public int Position
        {
            get { return this.position; }
            set
            {
                this.position = value;
                this.ObjectChangedEventHandler?.Invoke(this, EventArgs.Empty);
            }
        }

        public string Text
        {
            get { return this.text; }
            set
            {
                this.text = value;
                this.ObjectChangedEventHandler?.Invoke(this, EventArgs.Empty);
            }
        }
        public bool IsTrue
        {
            get { return this.isTrue; }
            set
            {
                this.isTrue = value;
                this.ObjectChangedEventHandler?.Invoke(this, EventArgs.Empty);
            }
        }

        public int LowerLimit
        {
            get { return this.lowerLimit; }
            set
            {
                this.lowerLimit = value;
                this.ObjectChangedEventHandler?.Invoke(this, EventArgs.Empty);
            }
        }

        public int Correct
        {
            get { return this.correct; }
            set
            {
                this.correct = value;
                this.ObjectChangedEventHandler?.Invoke(this, EventArgs.Empty);
            }
        }

        public int HigherLimit
        {
            get { return this.higherLimit; }
            set
            {
                this.higherLimit = value;
                this.ObjectChangedEventHandler?.Invoke(this, EventArgs.Empty);
            }
        }

        public AnswerOptionType AnswerOptionType { get; set; } = AnswerOptionType.ShowGeneralAnswerOptions;
    }
}
