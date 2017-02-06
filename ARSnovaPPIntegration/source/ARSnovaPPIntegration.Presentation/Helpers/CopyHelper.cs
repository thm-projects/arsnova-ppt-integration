using System;
using System.Collections.ObjectModel;

using ARSnovaPPIntegration.Business.Model;
using ARSnovaPPIntegration.Common.Enum;

namespace ARSnovaPPIntegration.Presentation.Helpers
{
    public static class CopyHelper
    {
        public static SlideQuestionModel CopySlideQuestionModel(SlideQuestionModel currentSlideQuestionModel)
        {
            var newSlideQuestionModel = new SlideQuestionModel
            {
                Id = currentSlideQuestionModel.Id,
                QuestionType = currentSlideQuestionModel.QuestionType,
                QuestionTypeSet = currentSlideQuestionModel.QuestionTypeSet,
                QuestionText = currentSlideQuestionModel.QuestionText,
                AnswerOptionsSet = currentSlideQuestionModel.AnswerOptionsSet,
                AnswerOptionAmount = currentSlideQuestionModel.AnswerOptionAmount,
                QuestionInitType = currentSlideQuestionModel.QuestionInitType,
                Index = currentSlideQuestionModel.Index,
                ChartType = currentSlideQuestionModel.ChartType,
                ResultsSlideId = currentSlideQuestionModel.ResultsSlideId,
                QuestionTimerSlideId = currentSlideQuestionModel.QuestionTimerSlideId,
                QuestionInfoSlideId = currentSlideQuestionModel.QuestionInfoSlideId,
                AnswerOptions = new ObservableCollection<GeneralAnswerOption>()
            };


            if (currentSlideQuestionModel.AnswerOptions != null)
            {
                foreach (var answerOption in currentSlideQuestionModel.AnswerOptions)
                {
                    newSlideQuestionModel.AnswerOptions.Add(CopyAnswerOptionModel(newSlideQuestionModel, answerOption));
                }
            }
            
            return newSlideQuestionModel;
        }

        private static GeneralAnswerOption CopyAnswerOptionModel(SlideQuestionModel slideQuestionModel, GeneralAnswerOption answerOption)
        {
            if (answerOption.AnswerOptionType == AnswerOptionType.ShowRangedAnswerOption)
            {
                var newAnswerOption = new GeneralAnswerOption
                {
                    RangedLowerLimit = answerOption.RangedLowerLimit,
                    RangedCorrectValue = answerOption.RangedCorrectValue,
                    RangedHigherLimit = answerOption.RangedHigherLimit,
                    AnswerOptionType = AnswerOptionType.ShowRangedAnswerOption                  
                };

                return newAnswerOption;
            }
            else
            {
                var newAnswerOption = new GeneralAnswerOption();

                newAnswerOption.ObjectChangedEventHandler += delegate {
                    slideQuestionModel.AnswerOptionsSet = true;
                    slideQuestionModel.AnswerOptionModelChanged();
                };

                newAnswerOption.Position = answerOption.Position;
                newAnswerOption.Text = answerOption.Text;
                newAnswerOption.IsTrue = answerOption.IsTrue;

                return newAnswerOption;
            }

            throw new ArgumentException($"Unknow answer option type {answerOption.GetType()}");
        }
    }
}
