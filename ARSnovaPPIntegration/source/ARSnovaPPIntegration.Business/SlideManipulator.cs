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

using MSForms = Microsoft.Vbe.Interop.Forms;
using MSComp = Microsoft.VisualBasic.CompilerServices;

namespace ARSnovaPPIntegration.Business
{
    public class SlideManipulator : ISlideManipulator
    {
        private readonly ILocalizationService localizationService;

        private readonly IArsnovaClickService arsnovaClickService;

        private readonly ISessionInformationProvider sessionInformationProvider;

        private string font;

        public SlideManipulator(
            ILocalizationService localizationService,
            IArsnovaClickService arsnovaClickService,
            ISessionInformationProvider sessionInformationProvider)
        {
            this.localizationService = localizationService;
            this.arsnovaClickService = arsnovaClickService;
            this.sessionInformationProvider = sessionInformationProvider;
            this.font = "Calibri";
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
            subTitleTextRange.Font.Name = this.font;
            subTitleTextRange.Font.Size = 22;
            subTitleTextBox.TextEffect.Alignment = MsoTextEffectAlignment.msoTextEffectAlignmentCentered;

            var subTitleTextBox2 = introSlide.Shapes.AddTextbox(MsoTextOrientation.msoTextOrientationHorizontal, 100, 400, 750, 75);
            var subTitleTextRange2 = subTitleTextBox2.TextFrame.TextRange;
            subTitleTextRange2.Text = $"{slideSessionModel.Hashtag}";
            subTitleTextRange2.Font.Name = this.font;
            subTitleTextRange2.Font.Size = 24;
            subTitleTextBox2.TextEffect.Alignment = MsoTextEffectAlignment.msoTextEffectAlignmentCentered;
            subTitleTextBox2.TextEffect.FontBold = MsoTriState.msoCTrue;

            // TODO create QR-Code / get it from click server
        }

        public void AddQuizToStyledSlides(SlideQuestionModel slideQuestionModel, Slide questionInfoSlide, Slide resultsSlide)
        {
            var isClickSession = this.sessionInformationProvider.IsClickQuestion(slideQuestionModel.QuestionType);

            questionInfoSlide.Layout = PpSlideLayout.ppLayoutBlank;

            // TODO get default fonts from slidemaster?

            // question
            var isRangedQuestion = slideQuestionModel.AnswerOptionType == AnswerOptionType.ShowRangedAnswerOption;
            var questionTextBox = questionInfoSlide.Shapes.AddTextbox(
                MsoTextOrientation.msoTextOrientationHorizontal,
                50,
                isRangedQuestion ? 350 : 50,
                850,
                isRangedQuestion ? 200 : 75);
            var questionTextRange = questionTextBox.TextFrame.TextRange;
            questionTextRange.Text = slideQuestionModel.QuestionText;
            questionTextRange.Font.Name = this.font;
            questionTextRange.Font.Size = 22;
            questionTextBox.TextEffect.Alignment = MsoTextEffectAlignment.msoTextEffectAlignmentCentered;

            // answer options
            // no answer options on ranged questions
            if (slideQuestionModel.AnswerOptionType != AnswerOptionType.ShowRangedAnswerOption)
            {
                var answerOptionsString = slideQuestionModel.AnswerOptions
                    .Aggregate(string.Empty, (current, castedAnswerOption) => current + $"{this.PositionNumberToLetter(castedAnswerOption.Position - 1)}: {castedAnswerOption.Text}{Environment.NewLine}");

                var answerOptionsTextBox = questionInfoSlide.Shapes.AddTextbox(MsoTextOrientation.msoTextOrientationHorizontal, 50, 150, 850, 150);
                var answerOptionsTextRange = answerOptionsTextBox.TextFrame.TextRange;
                answerOptionsTextRange.Text = answerOptionsString;
                answerOptionsTextRange.Font.Name = this.font;
                answerOptionsTextRange.Font.Size = 20;
            }

            // start button under answer options
            /*var startButton = questionInfoSlide.Shapes.AddShape(MsoAutoShapeType.msoShapeActionButtonCustom, 50, 450, 850, 50);
            startButton.ActionSettings[PpMouseActivation.ppMouseClick].Action = PpActionType.ppActionRunMacro; // invoking my code from macro not possible
            startButton.ActionSettings[PpMouseActivation.ppMouseClick].Run = "StartVotingEvent";*/

            /*var startButton = questionInfoSlide.Shapes.AddOLEObject(50, 450, 850, 50, "Forms.CommandButton.1", null, MsoTriState.msoFalse, null, 0, null, MsoTriState.msoCTrue);
            startButton.Name = "startButton";
            startButton.TextFrame.TextRange.Text = this.localizationService.Translate("Start Quiz");
            startButton.ActionSettings[PpMouseActivation.ppMouseClick].Hyperlink.Address = "";*/


            var startButton = questionInfoSlide.Shapes.AddOLEObject(50, 450, 850, 50, "Forms.CommandButton.1");
            startButton.Name = "StartButton";
            // TODO add to localization files
            /*startButton.TextFrame.TextRange.Text = this.localizationService.Translate("Start Quiz");
            startButton.TextEffect.FontBold = MsoTriState.msoTrue;
            startButton.TextEffect.FontName = this.font;*/

            var startCommandButton = (MSForms.CommandButton)MSComp.NewLateBinding.LateGet(
                questionInfoSlide,
                null,
                "StartButton",
                new object[0],
                null,
                null,
                null);
            
            // TODO add to localization files
            startCommandButton.Caption = this.localizationService.Translate("Start quiz");
            startCommandButton.FontBold = true;

            startCommandButton.Click += this.OnStartButtonClick;

            //AddHandler CType(oshape.OLEFormat.Object, MSForms.CommandButton).Click, _
            //AddressOf Button1_Click

            //var startBtn = (Microsoft.Vbe.Interop.Forms.CommandButton)Microsoft.VisualBasic.CompilerServices.NewLateBinding.LateGet((Excel.Worksheet)xlApp.ActiveSheet, null, "startButton", new object[0], null, null, null);
            // results
            resultsSlide.Layout = PpSlideLayout.ppLayoutBlank;

            var resultsHeaderTextBox = resultsSlide.Shapes.AddTextbox(MsoTextOrientation.msoTextOrientationHorizontal, 50, 50, 850, 50);
            var resultsHeaderTextRange = resultsHeaderTextBox.TextFrame.TextRange;
            resultsHeaderTextRange.Text = this.localizationService.Translate("Results");
            resultsHeaderTextRange.Font.Name = this.font;
            resultsHeaderTextRange.Font.Size = 26;

            // results will be filled in later (after the quiz)
            if (isClickSession)
            {
                // leaderboard
                var resultsLeaderBoard1TextBox = resultsSlide.Shapes.AddTextbox(MsoTextOrientation.msoTextOrientationHorizontal, 50, 120, 400, 200);
                var resultsLeaderBoard1TextRange = resultsLeaderBoard1TextBox.TextFrame.TextRange;
                resultsLeaderBoard1TextRange.Font.Name = this.font;
                resultsLeaderBoard1TextRange.Font.Size = 20;

                var resultsLeaderBoard2TextBox = resultsSlide.Shapes.AddTextbox(MsoTextOrientation.msoTextOrientationHorizontal, 500, 120, 400, 200);
                var resultsLeaderBoard2TextRange = resultsLeaderBoard2TextBox.TextFrame.TextRange;
                resultsLeaderBoard2TextRange.Font.Name = this.font;
                resultsLeaderBoard2TextRange.Font.Size = 20;
            }
            
            // graph
            var row2Obj = resultsSlide.Shapes.AddTextbox(MsoTextOrientation.msoTextOrientationHorizontal, 210, 60, 200, 600);
            var row2ObjTextRange = row2Obj.TextFrame.TextRange;
            row2ObjTextRange.Font.Name = this.font;
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

        private void OnStartButtonClick()
        {
            // TODO
            Console.Write("click callback successful!");
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
