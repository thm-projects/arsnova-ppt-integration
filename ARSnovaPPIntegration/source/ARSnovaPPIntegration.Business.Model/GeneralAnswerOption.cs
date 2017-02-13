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

        private bool configCaseSensitive = false;

        private bool configTrimWhitespaces = false;

        private bool configUseKeywords = true;

        private bool configUsePunctuation = false;

        public event EventHandler ObjectChangedEventHandler;

        //public event EventHandler IsTruePropertyChangedEventHandler;
        public GeneralAnswerOption()
        {
            this.Id = Guid.NewGuid();
        }

        public Guid Id { get; set; }

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
                //this.IsTruePropertyChangedEventHandler?.Invoke(this, EventArgs.Empty);
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

        public bool ConfigCaseSensitive
        {
            get { return this.configCaseSensitive; }
            set
            {
                this.configCaseSensitive = value;
                this.ObjectChangedEventHandler?.Invoke(this, EventArgs.Empty);
            }
        }

        public bool ConfigTrimWhitespaces
        {
            get { return this.configTrimWhitespaces; }
            set
            {
                this.configTrimWhitespaces = value;
                this.ObjectChangedEventHandler?.Invoke(this, EventArgs.Empty);
            }
        }

        public bool ConfigUseKeywords
        {
            get { return this.configUseKeywords; }
            set
            {
                this.configUseKeywords = value;
                this.ObjectChangedEventHandler?.Invoke(this, EventArgs.Empty);
            }
        }

        public bool ConfigUsePunctuation
        {
            get { return this.configUsePunctuation; }
            set
            {
                this.configUsePunctuation = value;
                this.ObjectChangedEventHandler?.Invoke(this, EventArgs.Empty);
            }
        }
    }
}
