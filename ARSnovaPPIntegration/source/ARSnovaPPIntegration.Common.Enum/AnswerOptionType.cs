namespace ARSnovaPPIntegration.Common.Enum
{
    public enum AnswerOptionType
    {
        /*
         * QuestionTypeEnum.SingleChoiceVoting
         * QuestionTypeEnum.MultipleChoiceVoting
         * QuestionTypeEnum.SingleChoiceClick
         * QuestionTypeEnum.MultipleChoiceClick
         * QuestionTypeEnum.SurveyClick;
         */
        ShowGeneralAnswerOptions = 1,

        /*
         * QuestionTypeEnum.FreeTextVoting
         * QuestionTypeEnum.FreeTextClick
         */
        ShowFreeTextAnswerOptions = 2,

        /*
         * QuestionTypeEnum.EvaluationVoting
         */
        ShowEvaluationAnswerOptions = 3,

        /*
         * QuestionTypeEnum.GradsVoting
         */
        ShowGradeAnswerOptions = 4,

        /*
         * QuestionTypeEnum.RangedQuestionClick
         */
        ShowRangedAnswerOption = 5,

        /*
         * QuestionTypeEnum.YesNoVoting
         * QuestionTypeEnum.YesNoClick
         * QuestionTypeEnum.TrueFalseClick
         */
        ShowTwoAnswerOptions = 6
    }
}
