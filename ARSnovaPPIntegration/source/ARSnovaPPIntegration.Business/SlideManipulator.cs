using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;

using Microsoft.Office.Core;
using Microsoft.Office.Interop.PowerPoint;

using ARSnovaPPIntegration.Business.Contract;
using ARSnovaPPIntegration.Business.Model;
using ARSnovaPPIntegration.Business.Properties;
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

        private readonly ISessionInformationProvider sessionInformationProvider;

        public SlideManipulator(
            ILocalizationService localizationService,
            IArsnovaClickService arsnovaClickService,
            ISessionInformationProvider sessionInformationProvider)
        {
            this.localizationService = localizationService;
            this.arsnovaClickService = arsnovaClickService;
            this.sessionInformationProvider = sessionInformationProvider;
        }

        public void AddIntroSlide(SlideSessionModel slideSessionModel, Slide introSlide)
        {
            var isClickSession = slideSessionModel.SessionType == SessionType.ArsnovaClick;

            // Note: Interops reads RGB colors in the order: BGR!
            var backgroundRgbColor = isClickSession ? Color.FromArgb(136, 150, 0).ToArgb() : Color.FromArgb(219, 219, 219).ToArgb();
            var filePath = isClickSession ? this.GetFilePath(Images.click_header, "click_header.png") : this.GetFilePath(Images.arsnova_header, "arsnova_header.png");
            var sessionTypeName = isClickSession ? "arsnova.click" : "ARSnova.voting";

            introSlide.Layout = PpSlideLayout.ppLayoutBlank;

            introSlide.FollowMasterBackground = MsoTriState.msoFalse;
            introSlide.Background.Fill.ForeColor.RGB = backgroundRgbColor;

            introSlide.Shapes.AddPicture(
                filePath,
                MsoTriState.msoTrue,
                MsoTriState.msoTrue,
                100,
                isClickSession ? 175 : 125,
                750,
                isClickSession ? 75 : 150);

            var subTitleTextBox = introSlide.Shapes.AddTextbox(MsoTextOrientation.msoTextOrientationHorizontal, 100, 350, 750, 50);
            var subTitleTextRange = subTitleTextBox.TextFrame.TextRange;
            subTitleTextRange.Text = $"{this.localizationService.Translate("This presentation uses")} {sessionTypeName}, {this.localizationService.Translate("join the hashtag:")}";
            subTitleTextRange.Font.Name = "Arial";
            subTitleTextRange.Font.Size = 22;
            subTitleTextBox.TextEffect.Alignment = MsoTextEffectAlignment.msoTextEffectAlignmentCentered;

            var subTitleTextBox2 = introSlide.Shapes.AddTextbox(MsoTextOrientation.msoTextOrientationHorizontal, 100, 400, 750, 75);
            var subTitleTextRange2 = subTitleTextBox2.TextFrame.TextRange;
            subTitleTextRange2.Text = $"{slideSessionModel.Hashtag}";
            subTitleTextRange2.Font.Name = "Arial";
            subTitleTextRange2.Font.Size = 24;
            subTitleTextBox2.TextEffect.Alignment = MsoTextEffectAlignment.msoTextEffectAlignmentCentered;
            subTitleTextBox2.TextEffect.FontBold = MsoTriState.msoCTrue;

            // TODO create QR-Code / get it from click server
        }

        public void AddQuizToStyledSlides(SlideQuestionModel slideQuestionModel, Slide questionInfoSlide, Slide resultsSlide)
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
                    .Aggregate(string.Empty, (current, castedAnswerOption) => current + $"{this.PositionNumberToLetter(castedAnswerOption.Position - 1)}: {castedAnswerOption.Text}{Environment.NewLine}");

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

        public void AddQuizToSlideWithoutStyling(SlideQuestionModel slideQuestionModel, Slide slide)
        {
            // TODO
            // question


            // answer option


            // results
        }

        public void SetTimerOnSlide(Slide resultsSlide, int countdown)
        {
            var resultsHeaderObj = resultsSlide.Shapes[2].TextFrame.TextRange;
            resultsHeaderObj.Text = $"\t\t{this.localizationService.Translate("Countdown")}: {countdown}";
        }

        public void SetResults(SlideQuestionModel slideQuestionModel, Slide resultsSlide, List<ResultModel> results)
        {
            if (this.sessionInformationProvider.IsClickQuestion(slideQuestionModel.QuestionType))
            {
                // arsnova.click

            }
            else
            {
                // arsnova.voting
                // TODO
            }
        }

        public void SetLeaderboardOnSlide(Slide resultsSlide, List<ResultModel> best10Responses)
        {
            best10Responses = best10Responses.OrderBy(r => r.responseTime).ToList();

            var resultsColumn1Text = string.Empty;
            var resultsColumn2Text = string.Empty;
            var i = 1;

            foreach (var response in best10Responses)
            {
                if (i % 2 == 0)
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

        private string GetFilePath(Bitmap image, string name)
        { 
            var currentAssembly = System.Reflection.Assembly.GetExecutingAssembly().Location;
            var filePath = Path.GetDirectoryName(currentAssembly) + "\\" + name;

            if (!File.Exists(filePath))
            {
                image.Save(filePath);
            }

            return filePath;
        }

        private void AddChartToShape(SlideQuestionModel slideQuestionModel, Microsoft.Office.Interop.PowerPoint.Shape shape)
        {
            var chartWorksheet = shape.Chart.ChartData.Workbook.Worksheets(1);

            if (slideQuestionModel.QuestionType == QuestionTypeEnum.RangedQuestionClick)
            {
                // TODO
            }
            else
            {
                // Range: One Column for each answer option
                // One row for the amount of students voted for that answer option

                chartWorksheet.ListObjects("Table1").Resize(chartWorksheet.Range($"A1:B{slideQuestionModel.AnswerOptions.Count + 1}"));
                chartWorksheet.Range("Table1[[#Headers],[Series 1]]").Value = "Items";

                for (var i = 0; i < slideQuestionModel.AnswerOptions.Count; i++)
                {
                    var answerOption = slideQuestionModel.AnswerOptions.First(ao => ao.Position == i);

                    chartWorksheet.Range($"a{i + 2}").Value = answerOption.Text;

                    
                }
            }
        }

        private char PositionNumberToLetter(int number)
        {
            if (number < 0 || number > 25)
                throw new ArgumentOutOfRangeException(nameof(number));

            return "ABCDEFGHIJKLMNOPQRSTUVWXYZ"[number];
        }

        private List<ResultModel> GetBest10Responses(SlideQuestionModel slideQuestionModel, List<ResultModel> responses)
        {
            responses = this.FilterForCorrectResponsesClick(slideQuestionModel, responses);

            var best10Responses = new List<ResultModel>();

            for (var i = 0; i < 10; i++)
            {
                if (responses.Count == 0)
                    break;

                var minResponse = responses.First(r => r.responseTime == responses.Min(r2 => r2.responseTime));
                best10Responses.Add(minResponse);
                responses.Remove(minResponse);
            }

            return best10Responses;
        }

        private List<ResultModel> FilterForCorrectResponsesClick(SlideQuestionModel slideQuestionModel, List<ResultModel> responses)
        {
            var correctResponses = new List<ResultModel>();

            var correctAnswerOptionPositions = slideQuestionModel.AnswerOptions.Where(ao => ao.IsTrue).Select(ao => ao.Position).Select(correctAnswerOptionPosition => correctAnswerOptionPosition - 1).ToList();
            var correctAnswerOptionPositionsCount = correctAnswerOptionPositions.Count();

            switch (slideQuestionModel.QuestionType)
            {
                case QuestionTypeEnum.SingleChoiceClick:
                case QuestionTypeEnum.YesNoClick:
                case QuestionTypeEnum.TrueFalseClick:
                    var correctAnswerOptionPosition = correctAnswerOptionPositions.First();
                    foreach (var response in responses)
                    {
                        if (response.answerOptionNumber.First() == correctAnswerOptionPosition)
                            correctResponses.Add(response);
                    }
                    break;
                case QuestionTypeEnum.MultipleChoiceClick:
                    foreach (var response in responses)
                    {
                        if (correctAnswerOptionPositionsCount == response.answerOptionNumber.Count)
                        {
                            var allCorrect = true;

                            foreach (var answerOption in response.answerOptionNumber)
                            {
                                if (correctAnswerOptionPositions.All(ca => ca != answerOption))
                                {
                                    allCorrect = false;
                                }
                            }

                            if (allCorrect)
                                correctResponses.Add(response);
                        }
                    }
                    break;
                case QuestionTypeEnum.RangedQuestionClick:
                    foreach (var response in responses)
                    {
                        // TODO
                    }
                    break;
                case QuestionTypeEnum.FreeTextClick:
                    // TODO
                    break;
                case QuestionTypeEnum.SurveyClick:
                    correctResponses = responses;
                    break;
            }

            return correctResponses;
        }

        private void SetArsnovaClickStyle(Slide arsnovaSlide, string hashtag)
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
    }
}
