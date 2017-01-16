using System;
using ARSnovaPPIntegration.Common.Enum;

namespace ARSnovaPPIntegration.Business.Model
{
    public class GeneralAnswerOption
    {
        private int position;

        private string text;

        private bool isTrue;

        private int rangedLowerLimit;

        private int rangedCorrectValue;

        private int rangedHigherLimit;

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

        public int RangedLowerLimit
        {
            get { return this.rangedLowerLimit; }
            set
            {
                this.rangedLowerLimit = value;
                this.ObjectChangedEventHandler?.Invoke(this, EventArgs.Empty);
            }
        }

        public int RangedCorrectValue
        {
            get { return this.rangedCorrectValue; }
            set
            {
                this.rangedCorrectValue = value;
                this.ObjectChangedEventHandler?.Invoke(this, EventArgs.Empty);
            }
        }

        public int RangedHigherLimit
        {
            get { return this.rangedHigherLimit; }
            set
            {
                this.rangedHigherLimit = value;
                this.ObjectChangedEventHandler?.Invoke(this, EventArgs.Empty);
            }
        }

        public AnswerOptionType AnswerOptionType { get; set; } = AnswerOptionType.ShowGeneralAnswerOptions;
    }
}
