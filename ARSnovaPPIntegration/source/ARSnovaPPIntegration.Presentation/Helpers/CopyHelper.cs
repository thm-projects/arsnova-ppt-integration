using System;
using System.Collections.ObjectModel;

using ARSnovaPPIntegration.Business.Model;

namespace ARSnovaPPIntegration.Presentation.Helpers
{
    public static class CopyHelper
    {
        public static SlideQuestionModel CopySlideQuestionModel(SlideQuestionModel currentSlideQuestionModel)
        {
            var newSlideQuestionModel = new SlideQuestionModel(currentSlideQuestionModel.QuestionTypeTranslator)
            {
                Id = currentSlideQuestionModel.Id,
                QuestionType = currentSlideQuestionModel.QuestionType,
                QuestionTypeSet = currentSlideQuestionModel.QuestionTypeSet,
                QuestionText = currentSlideQuestionModel.QuestionText,
                AnswerOptionsSet = currentSlideQuestionModel.AnswerOptionsSet,
                AnswerOptionType = currentSlideQuestionModel.AnswerOptionType,
                AnswerOptionAmount = currentSlideQuestionModel.AnswerOptionAmount,
                AnswerOptionInitType = currentSlideQuestionModel.AnswerOptionInitType,
                Index = currentSlideQuestionModel.Index,
                AnswerOptions = new ObservableCollection<object>()
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

        private static object CopyAnswerOptionModel(SlideQuestionModel slideQuestionModel, object answerOption)
        {
            if (answerOption.GetType() == typeof(GeneralAnswerOption))
            {
                var castedAnswerOption = answerOption as GeneralAnswerOption;
                var newAnswerOption = new GeneralAnswerOption();

                newAnswerOption.ObjectChangedEventHandler += delegate {
                    slideQuestionModel.AnswerOptionsSet = true;
                    slideQuestionModel.AnswerOptionModelChanged();
                };

                newAnswerOption.Position = castedAnswerOption.Position;
                newAnswerOption.Text = castedAnswerOption.Text;
                newAnswerOption.IsTrue = castedAnswerOption.IsTrue;

                return newAnswerOption;
            }

            if (answerOption.GetType() == typeof(RangedAnswerOption))
            {
                var castedAnswerOption = answerOption as RangedAnswerOption;
                var newAnswerOption = new RangedAnswerOption();

                newAnswerOption.LowerLimit = castedAnswerOption.LowerLimit;
                newAnswerOption.Correct = castedAnswerOption.Correct;
                newAnswerOption.HigherLimit = castedAnswerOption.HigherLimit;

                return newAnswerOption;
            }

            throw new ArgumentException($"Unknow answer option type {answerOption.GetType()}");
        }
    }
}
