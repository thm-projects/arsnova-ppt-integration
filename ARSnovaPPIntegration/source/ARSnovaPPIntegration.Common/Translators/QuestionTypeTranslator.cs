using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.Practices.ServiceLocation;

using ARSnovaPPIntegration.Common.Contract;
using ARSnovaPPIntegration.Common.Contract.Translators;
using ARSnovaPPIntegration.Common.Enum;

namespace ARSnovaPPIntegration.Common.Translators
{
    public class QuestionTypeTranslator : IQuestionTypeTranslator
    {
        private readonly List<Tuple<QuestionTypeEnum, string>> questionTypeTranslations;

        public QuestionTypeTranslator()
        {
            var localizationService = ServiceLocator.Current.GetInstance<ILocalizationService>();

            this.questionTypeTranslations = new List<Tuple<QuestionTypeEnum, string>>
            {
                // arsnova voting
                new Tuple<QuestionTypeEnum, string>(QuestionTypeEnum.SingleChoiceVoting,
                    localizationService.Translate("Single Choice")),
                new Tuple<QuestionTypeEnum, string>(QuestionTypeEnum.MultipleChoiceVoting,
                    localizationService.Translate("Multiple Choice")),
                new Tuple<QuestionTypeEnum, string>(QuestionTypeEnum.YesNoVoting,
                    localizationService.Translate("Yes|No")),
                new Tuple<QuestionTypeEnum, string>(QuestionTypeEnum.FreeTextVoting,
                    localizationService.Translate("Freetext")),
                new Tuple<QuestionTypeEnum, string>(QuestionTypeEnum.EvaluationVoting,
                    localizationService.Translate("Evaluation")),
                new Tuple<QuestionTypeEnum, string>(QuestionTypeEnum.GradsVoting,
                    localizationService.Translate("Grading")),

                // arsnova click
                new Tuple<QuestionTypeEnum, string>(QuestionTypeEnum.SingleChoiceClick,
                    localizationService.Translate("Single Choice")),
                new Tuple<QuestionTypeEnum, string>(QuestionTypeEnum.MultipleChoiceClick,
                    localizationService.Translate("Multiple Choice")),
                new Tuple<QuestionTypeEnum, string>(QuestionTypeEnum.YesNoClick,
                    localizationService.Translate("Yes|No")),
                new Tuple<QuestionTypeEnum, string>(QuestionTypeEnum.TrueFalseClick,
                    localizationService.Translate("True|False")),
                new Tuple<QuestionTypeEnum, string>(QuestionTypeEnum.RangedQuestionClick,
                    localizationService.Translate("Estimation")),
                new Tuple<QuestionTypeEnum, string>(QuestionTypeEnum.FreeTextClick,
                    localizationService.Translate("Freetext")),
                new Tuple<QuestionTypeEnum, string>(QuestionTypeEnum.SurveyClick,
                    localizationService.Translate("Survey")),
            };
        }

        public string TranslateQuestionType(QuestionTypeEnum questionType)
        {
            return this.questionTypeTranslations.First(qt => qt.Item1 == questionType).Item2;
        }
    }
}
