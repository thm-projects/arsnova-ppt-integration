using System;

namespace ARSnovaPPIntegration.Business.Model
{
    public class RangedAnswerOption
    {
        private int lowerLimit;

        private int correct;

        private int higherLimit;

        public event EventHandler ObjectChangedEventHandler;

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
    }
}
