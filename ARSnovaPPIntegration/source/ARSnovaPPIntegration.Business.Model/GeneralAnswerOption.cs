using System;

namespace ARSnovaPPIntegration.Business.Model
{
    public class GeneralAnswerOption
    {
        private int position;

        private string text;

        private bool isTrue;

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
    }
}
