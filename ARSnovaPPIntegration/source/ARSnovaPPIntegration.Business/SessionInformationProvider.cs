using System;
using System.Collections.Generic;
using System.Linq;

using ARSnovaPPIntegration.Business.Contract;
using ARSnovaPPIntegration.Business.Model;
using ARSnovaPPIntegration.Common.Contract;
using ARSnovaPPIntegration.Common.Contract.Translators;
using ARSnovaPPIntegration.Common.Enum;
using ARSnovaPPIntegration.Communication.Contract;

using Excel = Microsoft.Office.Interop.Excel;

namespace ARSnovaPPIntegration.Business
{
    public class SessionInformationProvider : ISessionInformationProvider
    {
        private readonly IArsnovaClickService arsnovaClickService;

        private readonly IQuestionTypeTranslator questionTypeTranslator;

        private readonly ILocalizationService localizationService;

        private readonly List<QuestionTypeEnum> votingQuestionTypes = new List<QuestionTypeEnum>
        {
            QuestionTypeEnum.SingleChoiceVoting,
            QuestionTypeEnum.MultipleChoiceVoting,
            QuestionTypeEnum.YesNoVoting,
            QuestionTypeEnum.FreeTextVoting,
            QuestionTypeEnum.EvaluationVoting,
            QuestionTypeEnum.GradsVoting
        };

        private readonly List<QuestionTypeEnum> clickQuestionTypes = new List<QuestionTypeEnum>
        {
            QuestionTypeEnum.SingleChoiceClick,
            QuestionTypeEnum.MultipleChoiceClick,
            QuestionTypeEnum.YesNoClick,
            QuestionTypeEnum.TrueFalseClick,
            QuestionTypeEnum.RangedQuestionClick,
            QuestionTypeEnum.FreeTextClick,
            QuestionTypeEnum.SurveyClick
        };

        public SessionInformationProvider(
            IArsnovaClickService arsnovaClickService,
            IQuestionTypeTranslator questionTypeTranslator,
            ILocalizationService localizationService)
        {
            this.arsnovaClickService = arsnovaClickService;
            this.questionTypeTranslator = questionTypeTranslator;
            this.localizationService = localizationService;
        }

        public List<QuestionType> GetAvailableQuestionsClick()
        {
            return this.clickQuestionTypes.Select(qte => new QuestionType
            {
                QuestionTypeEnum = qte,
                Name = this.questionTypeTranslator.TranslateQuestionType(qte)
            }).ToList();
        }

        public List<QuestionType> GetAvailableQuestionsVoting()
        {
            return this.votingQuestionTypes.Select(qte => new QuestionType
            {
                QuestionTypeEnum = qte,
                Name = this.questionTypeTranslator.TranslateQuestionType(qte)
            }).ToList();
        }

        public List<ExcelChartType> GetExcelChartTypes()
        {
            return new List<ExcelChartType>
            {
                new ExcelChartType
                {
                    Name= this.localizationService.Translate("Bar-Clustered"),
                    ChartType = Excel.XlChartType.xl3DBarClustered
                },
                new ExcelChartType
                {
                    Name= this.localizationService.Translate("Pie"),
                    ChartType = Excel.XlChartType.xl3DPie
                },
                new ExcelChartType
                {
                    Name= this.localizationService.Translate("Grouped columns"),
                    ChartType = Excel.XlChartType.xl3DColumnClustered
                }
            };
        }

        public bool IsClickQuestion(QuestionTypeEnum questionType)
        {
            return this.clickQuestionTypes.Contains(questionType);
        }

        public bool IsSingleChoiceQuestion(QuestionTypeEnum questionType)
        {
            return questionType == QuestionTypeEnum.SingleChoiceVoting
                   || questionType == QuestionTypeEnum.YesNoVoting
                   || questionType == QuestionTypeEnum.SingleChoiceClick
                   || questionType == QuestionTypeEnum.YesNoClick
                   || questionType == QuestionTypeEnum.TrueFalseClick;
        }

        public bool IsMultipleChoiceQuestion(QuestionTypeEnum questionType)
        {
            return questionType == QuestionTypeEnum.MultipleChoiceVoting
                   || questionType == QuestionTypeEnum.MultipleChoiceClick;
        }

        public AnswerOptionType GetAnswerOptionType(QuestionTypeEnum questionType)
        {
            switch (questionType)
            {
                case QuestionTypeEnum.SingleChoiceVoting:
                case QuestionTypeEnum.MultipleChoiceVoting:
                case QuestionTypeEnum.SingleChoiceClick:
                case QuestionTypeEnum.MultipleChoiceClick:
                case QuestionTypeEnum.SurveyClick:
                    return AnswerOptionType.ShowGeneralAnswerOptions;
                case QuestionTypeEnum.FreeTextVoting:
                case QuestionTypeEnum.FreeTextClick:
                    return AnswerOptionType.ShowFreeTextAnswerOptions;
                case QuestionTypeEnum.EvaluationVoting:
                case QuestionTypeEnum.GradsVoting:
                    return AnswerOptionType.ShowGradeOrEvaluationAnswerOptions;
                case QuestionTypeEnum.RangedQuestionClick:
                    return AnswerOptionType.ShowRangedAnswerOption;
                case QuestionTypeEnum.YesNoVoting:
                case QuestionTypeEnum.YesNoClick:
                case QuestionTypeEnum.TrueFalseClick:
                    return AnswerOptionType.ShowTwoAnswerOptions;
                default:
                    throw new ArgumentException($"QuestionType not handled in GetAnswerOptionType: '{questionType}'");
            }
        }

        public List<string> GetHashtagList()
        {
            var allHashtagInfos = this.arsnovaClickService.GetAllHashtagInfos();

            return allHashtagInfos.Select(hashtagInfo => hashtagInfo.hashtag).ToList();
        }
    }
}
