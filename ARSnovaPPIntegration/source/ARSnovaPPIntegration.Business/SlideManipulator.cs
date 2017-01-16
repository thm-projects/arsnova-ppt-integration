using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Office.Core;
using Microsoft.Office.Interop.PowerPoint;
using Microsoft.Practices.ServiceLocation;

using ARSnovaPPIntegration.Business.Contract;
using ARSnovaPPIntegration.Business.Model;
using ARSnovaPPIntegration.Common.Contract;
using ARSnovaPPIntegration.Common.Contract.Exceptions;
using ARSnovaPPIntegration.Common.Enum;
using ARSnovaPPIntegration.Communication.Contract;
using ARSnovaPPIntegration.Communication.Model.ArsnovaClick;

namespace ARSnovaPPIntegration.Business
{
    public class SlideManipulator : ISlideManipulator
    {
        private readonly ILocalizationService localizationService;

        private readonly IArsnovaClickService arsnovaClickService;

        public SlideManipulator(ILocalizationService localizationService)
        {
            this.localizationService = localizationService;
            this.arsnovaClickService = ServiceLocator.Current.GetInstance<IArsnovaClickService>();
        }

        public void AddFooter(Slide slide, string header = "ARSnova Quiz")
        {
            slide.HeadersFooters.Footer.Visible = MsoTriState.msoTrue;
            slide.HeadersFooters.Footer.Text = this.localizationService.Translate(header);
        }

        public void SetArsnovaStyle(Slide slide)
        {
            throw new NotImplementedException();
        }

        public void SetArsnovaClickStyle(Slide arsnovaSlide, string hashtag)
        {
            var sessionConfiguration = this.arsnovaClickService.GetSessionConfiguration(hashtag);

            var themeName = string.Empty;

            // TODO create background-pictures
            switch (sessionConfiguration.theme)
            {
                case "theme-thm":
                    break;
                case "theme-elegant":
                    break;
                case "theme-arsnova":
                    break;
                case "theme-blackbeauty":
                    break;
                case "theme-hell":
                    break;
                case "theme-bluetouch":
                    break;
                case "theme-green":
                    break;
                case "theme-action":
                    break;
                case "theme-Psychology-RangedCorrectValue-Colours":
                    break;
                case "theme-arsnova-dot-click-contrast":
                    break;
                default:
                    throw new CommunicationException("Unexpected theme name");
            }

            // TODO
            // background
            arsnovaSlide.FollowMasterBackground = MsoTriState.msoFalse;
            arsnovaSlide.Background.Fill.UserPicture(@"C:\fox.jpg");
         
            // footer
            arsnovaSlide.HeadersFooters.Footer.Visible = MsoTriState.msoTrue;
            arsnovaSlide.HeadersFooters.Footer.Text = "Copyright arsnova team / Tjark Wilhelm Hoeck";
        }

        public void AddClickIntroSlide(Slide slide, string hashtag)
        {
            var titelObj = slide.Shapes[1].TextFrame.TextRange;
            titelObj.Text = this.localizationService.Translate("ARSnova.click");
            titelObj.Font.Name = "Arial";
            titelObj.Font.Size = 32;

            // Microsoft.Office.Interop.PowerPoint.Shape shape = slide.Shapes[2];
            // slide.Shapes.AddPicture(pictureFileName, Microsoft.Office.Core.MsoTriState.msoFalse, Microsoft.Office.Core.MsoTriState.msoTrue, shape.Left, shape.Top, shape.Width, shape.Height);

            var contentObj = slide.Shapes[2].TextFrame.TextRange;
            contentObj.Text = this.localizationService.Translate("This presentation uses arsnova.click, join the hashtag:");
            contentObj.Text += Environment.NewLine;
            contentObj.Text += Environment.NewLine;
            contentObj.Text += hashtag;
            contentObj.Paragraphs(-1).Lines(3, 1).Font.Name = "Arial";
            contentObj.Paragraphs(-1).Lines(3, 1).Font.Size = 26;
            // TODO create QR-Code / get it from click server
        }

        public void AddQuizToSlide(SlideQuestionModel slideQuestionModel, Slide questionInfoSlide, Slide resultsSlide)
        {
            questionInfoSlide.Layout = PpSlideLayout.ppLayoutText;

            // question
            var questionObj = questionInfoSlide.Shapes[1].TextFrame.TextRange;
            questionObj.Text = slideQuestionModel.QuestionText;
            questionObj.Font.Name = "Arial";
            questionObj.Font.Size = 26;

            // answer options
            // no answer options on ranged questions
            if (slideQuestionModel.AnswerOptionType != AnswerOptionType.ShowRangedAnswerOption)
            {
                var answerOptionsString = slideQuestionModel.AnswerOptions
                    .Aggregate(string.Empty, (current, castedAnswerOption) => current + $"{this.PositionNumberToLetter(castedAnswerOption.Position, true)}: {castedAnswerOption.Text}{Environment.NewLine}");

                var answerOptionsObj = questionInfoSlide.Shapes[2].TextFrame.TextRange;
                answerOptionsObj.Text = answerOptionsString;
                answerOptionsObj.Font.Name = "Arial";
                answerOptionsObj.Font.Size = 20;
            }

            // results
            resultsSlide.Layout = PpSlideLayout.ppLayoutBlank;
            var resultsHeaderObj = resultsSlide.Shapes.AddTextbox(MsoTextOrientation.msoTextOrientationHorizontal, 10, 10, 400, 50);
            var resultsHeaderTextRange = resultsHeaderObj.TextFrame.TextRange;
            resultsHeaderTextRange.Text = this.localizationService.Translate("Results");
            resultsHeaderTextRange.Font.Name = "Arial";
            resultsHeaderTextRange.Font.Size = 26;

            var row1Obj = resultsSlide.Shapes.AddTextbox(MsoTextOrientation.msoTextOrientationHorizontal, 10, 60, 200, 600);
            var row1ObjTextRange = row1Obj.TextFrame.TextRange;
            row1ObjTextRange.Font.Name = "Arial";
            row1ObjTextRange.Font.Size = 20;

            var row2Obj = resultsSlide.Shapes.AddTextbox(MsoTextOrientation.msoTextOrientationHorizontal, 210, 60, 200, 600);
            var row2ObjTextRange = row2Obj.TextFrame.TextRange;
            row2ObjTextRange.Font.Name = "Arial";
            row2ObjTextRange.Font.Size = 20;
        }

        private string PositionNumberToLetter(int number, bool isCaps)
        {
            var c = (isCaps ? 65 : 97) + (number - 1);
            return c.ToString();
        }

        public void SetTimerOnSlide(Slide timerSlide, int countdown)
        {
            var resultsHeaderObj = timerSlide.Shapes[2].TextFrame.TextRange;
            resultsHeaderObj.Text = $"\t\t{this.localizationService.Translate("Countdown")}: {countdown}";
        }

        public void InitTimerOnSlide(Slide timerSlide, int initCountdown)
        {
            var resultsHeaderObj = timerSlide.Shapes[2].TextFrame.TextRange;
            resultsHeaderObj.Text = $"\t\t{this.localizationService.Translate("Countdown")}: {initCountdown}";
        }

        public void SetResultsOnSlide(Slide resultsSlide, List<ResultModel> best10Responses)
        {
            best10Responses = best10Responses.OrderBy(r => r.responseTime).ToList();

            var resultsColumn1Text = string.Empty;
            var resultsColumn2Text = string.Empty;
            var i = 1;

            foreach (var response in best10Responses)
            {
                if (i%2 == 0)
                {
                    resultsColumn2Text += $"{i}. {response.userNick}{Environment.NewLine}";
                }
                else
                {
                    resultsColumn1Text += $"{i}. {response.userNick}{Environment.NewLine}";
                }

                i++;
            }

            var resultsColumn1 = resultsSlide.Shapes[2].TextFrame.TextRange;
            resultsColumn1.Font.Name = "Arial";
            resultsColumn1.Font.Size = 20;
            resultsColumn1.ParagraphFormat.Alignment = PpParagraphAlignment.ppAlignCenter;
            resultsColumn1.Text = resultsColumn1Text;

            var resultsColumn2 = resultsSlide.Shapes[3].TextFrame.TextRange;
            resultsColumn2.Font.Name = "Arial";
            resultsColumn2.Font.Size = 20;
            resultsColumn2.ParagraphFormat.Alignment = PpParagraphAlignment.ppAlignCenter;
            resultsColumn2.Text = resultsColumn2Text;
        }
    }
}
