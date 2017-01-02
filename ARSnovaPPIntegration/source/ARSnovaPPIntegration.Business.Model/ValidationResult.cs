namespace ARSnovaPPIntegration.Business.Model
{
    public class ValidationResult
    {
        private string failureTitel;

        private string failureMessage;

        public ValidationResult()
        {
            this.Success = true;
        }

        public bool Success { get; private set; }

        public string FailureTitel
        {
            get { return this.failureTitel; }
            set
            {
                this.failureTitel = value;
                this.Success = false;
            }
        }

        public string FailureMessage
        {
            get { return this.failureMessage; }
            set
            {
                this.failureMessage = value;
                this.Success = false;
            }
        }
    }
}
