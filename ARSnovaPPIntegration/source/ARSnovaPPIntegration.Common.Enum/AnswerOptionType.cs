namespace ARSnovaPPIntegration.Common.Enum
{
    public enum AnswerOptionType
    {
        /*
         * QuestionTypeEnum.SingleChoiceVoting
         * QuestionTypeEnum.MultipleChoiceVoting
         * QuestionTypeEnum.SingleChoiceClick
         * QuestionTypeEnum.MultipleChoiceClick
         */
        ShowGeneralAnswerOptions = 1,

        /*
         * QuestionTypeEnum.FreeTextVoting
         * QuestionTypeEnum.FreeTextClick
         */
        ShowFreeTextAnswerOptions = 2,

        /*
         * QuestionTypeEnum.EvaluationVoting
         * QuestionTypeEnum.GradsVoting
         */
        ShowGradeOrEvaluationAnswerOptions = 3,

        /*
         * QuestionTypeEnum.RangedQuestionClick
         */
        ShowRangedAnswerOption = 4,

        /*
         * QuestionTypeEnum.YesNoVoting
         * QuestionTypeEnum.YesNoClick
         * QuestionTypeEnum.TrueFalseClick
         */
        ShowTwoAnswerOptions = 5,

        /*
         * QuestionTypeEnum.SurveyClick;
         */
        ShowSurveyAnswerOptions = 6
    }
}
