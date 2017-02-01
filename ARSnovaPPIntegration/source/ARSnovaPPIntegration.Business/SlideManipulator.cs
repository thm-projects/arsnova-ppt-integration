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

using Excel = Microsoft.Office.Interop.Excel;

namespace ARSnovaPPIntegration.Business
{
    public class SlideManipulator : ISlideManipulator
    {
        private readonly ILocalizationService localizationService;

        private readonly IArsnovaClickService arsnovaClickService;

        private readonly ISessionInformationProvider sessionInformationProvider;

        private readonly string font;

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
            subTitleTextRange.Text = $"{this.localizationService.Translate("This presentation uses")} {sessionTypeName}, {this.localizationService.Translate("join the hashtag")}:";
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

        public void AddQuizToStyledSlides(SlideQuestionModel slideQuestionModel, Slide questionInfoSlide, Slide questionTimerSlide, Slide resultsSlide)
        {
            this.RemoveShapesFromSlide(questionInfoSlide);
            this.RemoveShapesFromSlide(questionTimerSlide);
            this.RemoveShapesFromSlide(resultsSlide);

            questionInfoSlide.FollowMasterBackground = MsoTriState.msoTrue;
            questionTimerSlide.FollowMasterBackground = MsoTriState.msoTrue;
            resultsSlide.FollowMasterBackground = MsoTriState.msoTrue;

            questionInfoSlide.Layout = PpSlideLayout.ppLayoutBlank;
            this.AddQuestionSlideContent(slideQuestionModel, questionInfoSlide);

            // Button not possible, just added a textbox to start quiz on next slide
            var startQuizTextBox = questionInfoSlide.Shapes.AddTextbox(MsoTextOrientation.msoTextOrientationHorizontal, 50, 450, 850, 50);
            var startQuizTextRange = startQuizTextBox.TextFrame.TextRange;
            startQuizTextRange.Text = this.localizationService.Translate("Move to the next slide to start the quiz.");
            startQuizTextRange.Font.Name = this.font;
            startQuizTextRange.Font.Size = 20;
            startQuizTextBox.TextEffect.FontBold = MsoTriState.msoTrue;
            startQuizTextBox.TextEffect.Alignment = MsoTextEffectAlignment.msoTextEffectAlignmentCentered;


            questionTimerSlide.Layout = PpSlideLayout.ppLayoutBlank;
            this.AddQuestionSlideContent(slideQuestionModel, questionTimerSlide);

            // Timer
            var timerLabelTextBox = questionTimerSlide.Shapes.AddTextbox(MsoTextOrientation.msoTextOrientationHorizontal, 300, 450, 150, 75);
            var timerLabelTextRange = timerLabelTextBox.TextFrame.TextRange;
            timerLabelTextRange.Text = this.localizationService.Translate("Countdown:");
            timerLabelTextRange.Font.Name = this.font;
            timerLabelTextRange.Font.Size = 22;
            timerLabelTextBox.TextEffect.FontBold = MsoTriState.msoTrue;
            timerLabelTextBox.TextEffect.Alignment = MsoTextEffectAlignment.msoTextEffectAlignmentCentered;

            var timerTextBox = questionTimerSlide.Shapes.AddTextbox(MsoTextOrientation.msoTextOrientationHorizontal, 500, 450, 150, 75);
            var timerTextRange = timerTextBox.TextFrame.TextRange;
            timerTextRange.Text = slideQuestionModel.Countdown.ToString();
            timerTextRange.Font.Name = this.font;
            timerTextRange.Font.Size = 22;
            timerTextBox.TextEffect.FontBold = MsoTriState.msoTrue;
            timerTextBox.TextEffect.Alignment = MsoTextEffectAlignment.msoTextEffectAlignmentCentered;

            this.CleanResultsPage(resultsSlide);

            // results will be filled in later (after the quiz)
            // TEST ONLY, REMOVE BEFORE PRODUCTIVE USEAGE!
            /*var results = new List<ResultModel>()
                          {
                              new ResultModel
                              {
                                  answerOptionNumber = new List<int> {0},
                                  hashtag = "testxXx2",
                                  questionIndex = 0,
                                  responseTime = 18000,
                                  userNick = "TestUserTH"
                              },
                              new ResultModel
                              {
                                  answerOptionNumber = new List<int> {0},
                                  hashtag = "testxXx2",
                                  questionIndex = 0,
                                  responseTime = 18000,
                                  userNick = "TestUserTH2"
                              },
                              new ResultModel
                              {
                                  answerOptionNumber = new List<int> {0},
                                  hashtag = "testxXx2",
                                  questionIndex = 0,
                                  responseTime = 18000,
                                  userNick = "TestUserT3"
                              },
                              new ResultModel
                              {
                                  answerOptionNumber = new List<int> {0},
                                  hashtag = "testxXx2",
                                  questionIndex = 0,
                                  responseTime = 18000,
                                  userNick = "TestUserTH4"
                              },
                              new ResultModel
                              {
                                  answerOptionNumber = new List<int> {1},
                                  hashtag = "testxXx2",
                                  questionIndex = 0,
                                  responseTime = 18000,
                                  userNick = "TestUserT5"
                              },
                              new ResultModel
                              {
                                  answerOptionNumber = new List<int> {1},
                                  hashtag = "testxXx2",
                                  questionIndex = 0,
                                  responseTime = 18000,
                                  userNick = "TestUserTH6"
                              }
                          };
            this.SetResults(slideQuestionModel, resultsSlide, results);*/
        }

        public void AddQuizToSlideWithoutStyling(SlideQuestionModel slideQuestionModel, Slide slide)
        {
            // TODO
            // question


            // answer option


            // results
        }

        public void SetTimerOnSlide(Slide questionTimerSlide, int countdown)
        {
            questionTimerSlide.Shapes[4].TextFrame.TextRange.Text = countdown.ToString();
        }

        public void SetResults(SlideQuestionModel slideQuestionModel, Slide resultsSlide, List<ResultModel> results)
        {
            // chart init dimensions
            var floatLeft = 150;
            var floatTop = 300;
            var width = 650;
            var height = 250;

            if (this.sessionInformationProvider.IsClickQuestion(slideQuestionModel.QuestionType))
            {
                // arsnova.click
                var best10Responses = this.GetBest10Responses(slideQuestionModel, results);

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

                var leaderBoardColumn1TextBox =
                    resultsSlide.Shapes.AddTextbox(MsoTextOrientation.msoTextOrientationHorizontal, 50, 120, 400, 200);
                var leaderBoardColumn1TextRange = leaderBoardColumn1TextBox.TextFrame.TextRange;
                leaderBoardColumn1TextRange.Text = resultsColumn1Text;
                leaderBoardColumn1TextRange.Font.Name = this.font;
                leaderBoardColumn1TextRange.Font.Size = 20;
                leaderBoardColumn1TextBox.TextEffect.Alignment = MsoTextEffectAlignment.msoTextEffectAlignmentCentered;

                var leaderBoardColumn2TextBox =
                    resultsSlide.Shapes.AddTextbox(MsoTextOrientation.msoTextOrientationHorizontal, 500, 120, 400, 200);
                var leaderBoardColumn2TextRange = leaderBoardColumn2TextBox.TextFrame.TextRange;
                leaderBoardColumn2TextRange.Text = resultsColumn2Text;
                leaderBoardColumn2TextRange.Font.Name = this.font;
                leaderBoardColumn2TextRange.Font.Size = 20;
                leaderBoardColumn2TextBox.TextEffect.Alignment = MsoTextEffectAlignment.msoTextEffectAlignmentCentered;

                // chart dimensions
                switch (slideQuestionModel.ChartType)
                {
                    case Excel.XlChartType.xl3DColumnClustered:
                    case Excel.XlChartType.xl3DBarClustered:
                        // use default values
                        break;
                    case Excel.XlChartType.xlPie:
                        floatLeft = 275;
                        width = 250;
                        break;
                }
            }
            else
            {
                // arsnova.voting
                // chart dimensions
                switch (slideQuestionModel.ChartType)
                {
                    case Excel.XlChartType.xl3DColumnClustered:
                    case Excel.XlChartType.xl3DBarClustered:
                        floatLeft = 150;
                        floatTop = 150;
                        height = 450;
                        break;
                    case Excel.XlChartType.xl3DPie:
                        floatLeft = 200;
                        width = 500;
                        height = 450;
                        break;
                }
            } 

            this.AddChartToShape(slideQuestionModel, resultsSlide, results, floatLeft, floatTop, width, height);
        }

        public void CleanResultsPage(Slide resultsSlide)
        {
            this.RemoveShapesFromSlide(resultsSlide);

            resultsSlide.Layout = PpSlideLayout.ppLayoutBlank;

            var resultsHeaderTextBox = resultsSlide.Shapes.AddTextbox(MsoTextOrientation.msoTextOrientationHorizontal, 50, 50, 850, 50);
            var resultsHeaderTextRange = resultsHeaderTextBox.TextFrame.TextRange;
            resultsHeaderTextRange.Text = this.localizationService.Translate("Results");
            resultsHeaderTextRange.Font.Name = this.font;
            resultsHeaderTextRange.Font.Size = 26;
            resultsHeaderTextBox.TextEffect.FontBold = MsoTriState.msoTrue;
            resultsHeaderTextBox.TextEffect.Alignment = MsoTextEffectAlignment.msoTextEffectAlignmentCentered;
        }

        private void RemoveShapesFromSlide(Slide slide)
        {
            while (slide.Shapes.Count > 0)
            {
                slide.Shapes[1].Delete();
            }
        }

        private void AddQuestionSlideContent(SlideQuestionModel slideQuestionModel, Slide questionSlide)
        {
            // question
            var isRangedQuestion = slideQuestionModel.AnswerOptionType == AnswerOptionType.ShowRangedAnswerOption;
            var questionTextBox = questionSlide.Shapes.AddTextbox(
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

                var answerOptionsTextBox = questionSlide.Shapes.AddTextbox(MsoTextOrientation.msoTextOrientationHorizontal, 50, 150, 850, 150);
                var answerOptionsTextRange = answerOptionsTextBox.TextFrame.TextRange;
                answerOptionsTextRange.Text = answerOptionsString;
                answerOptionsTextRange.Font.Name = this.font;
                answerOptionsTextRange.Font.Size = 20;
            }
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

        private void AddChartToShape(
            SlideQuestionModel slideQuestionModel,
            Slide resultsSlide,
            List<ResultModel> results,
            int floatLeft,
            int floatTop,
            int width,
            int height)
        {
            // Create new chart in Excel
            var currentAssembly = System.Reflection.Assembly.GetExecutingAssembly().Location;
            var excelWorkBookPath = Path.GetDirectoryName(currentAssembly) + "\\" + "resultsChartData.xlsx";
            var chartName = "ARSnova Results Chart";

            if (File.Exists(excelWorkBookPath))
            {
                File.Delete(excelWorkBookPath);
            }

            var excelApp = new Excel.Application();
            var workBook = excelApp.Workbooks.Add(Excel.XlWBATemplate.xlWBATWorksheet);
            var workSheet = (Excel.Worksheet)(workBook.Worksheets[1]);
            workSheet.Name = "ARSnovaResults";
            Excel.Range dataRange;

            //switch(slideQuestionModel)
            if (slideQuestionModel.QuestionType == QuestionTypeEnum.RangedQuestionClick)
            {
                // TODO -> there is only right or wrong
                this.SetExcelCellValue(workSheet, "A1", this.localizationService.Translate("Right"));
                //this.SetExcelCellValue(workSheet, "B1", );
                this.SetExcelCellValue(workSheet, "A2", this.localizationService.Translate("Wrong"));
                //this.SetExcelCellValue(workSheet, "B2", );

                dataRange = workSheet.get_Range("A1", "B2");
            }
            else
            {
                // Range: One Column for each answer option
                // One row for the amount of students voted for that answer option
                for (var i = 0; i < slideQuestionModel.AnswerOptions.Count; i++)
                {
                    var answerOption = slideQuestionModel.AnswerOptions.First(ao => ao.Position - 1 == i);
                    this.SetExcelCellValue(workSheet, $"A{i + 1}", answerOption.Text);
                    this.SetExcelCellValue(workSheet, $"B{i + 1}", results.Count(r => r.answerOptionNumber.Contains(answerOption.Position - 1)));
                }

                dataRange = workSheet.get_Range("A1", $"B{slideQuestionModel.AnswerOptions.Count}");
            }

            var chartObjects = (Excel.ChartObjects)workSheet.ChartObjects(Type.Missing);
            var newChartObject = chartObjects.Add(floatLeft, floatTop, width, height);
            newChartObject.Name = chartName;

            newChartObject.Chart.ChartWizard(
                dataRange,
                slideQuestionModel.ChartType,
                this.GetChartFormat(slideQuestionModel.ChartType), // chart format
                Excel.XlRowCol.xlColumns,
                slideQuestionModel.AnswerOptions.Count - 1, // category labels
                0, // series labels
                false, // has legend
                slideQuestionModel.QuestionText, // title
                this.localizationService.Translate("Answers"), // category title
                this.localizationService.Translate("Amount"), // value title
                Type.Missing); // extra titel

            /*var chartTitle = chartShape.Chart.ChartTitle;
            chartTitle.Font.Italic = true;
            chartTitle.Text = slideQuestionModel.QuestionText;
            chartTitle.Font.Size = 16;
            chartTitle.Font.Color = Color.Black.ToArgb();
            chartTitle.Format.Line.Visible = MsoTriState.msoTrue;
            chartTitle.Format.Line.ForeColor.RGB = Color.Black.ToArgb();*/

            workBook.SaveAs(excelWorkBookPath, Type.Missing, Type.Missing, Type.Missing, Type.Missing,
                    Type.Missing, Excel.XlSaveAsAccessMode.xlNoChange, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing);

            // Copy chart to PowerPoint slide

            newChartObject.Copy();
            var shapeRange = resultsSlide.Shapes.Paste();

            shapeRange.Left = floatLeft;
            shapeRange.Top = floatTop;

            excelApp.Quit();

            // post-editing
            var chartObj =
                resultsSlide.Shapes[
                                this.sessionInformationProvider.IsClickQuestion(slideQuestionModel.QuestionType) ? 3 : 2
                            ].Chart;

            for (var i = 0; i < slideQuestionModel.AnswerOptions.Count; i++)
            {
                Excel.Series serie = chartObj.SeriesCollection(i+1);

                if (slideQuestionModel.AnswerOptions.First(ao => ao.Position == i).IsTrue)
                {
                    serie.Interior.Color = Color.FromArgb(0, 255, 0).ToArgb(); // green
                }
                else
                {
                    serie.Interior.Color = Color.FromArgb(0, 0, 255).ToArgb(); // red
                }
            }

            // to update content: Chart.Application.Update();
        }

        private int GetChartFormat(Excel.XlChartType chartType)
        {
            switch (chartType)
            {
                case Excel.XlChartType.xl3DColumnClustered:
                    return 1;
                case Excel.XlChartType.xl3DBarClustered:
                    return 2;
                case Excel.XlChartType.xl3DPie:
                    return 1;
                default:
                    return 1;
            }
        }

        private void SetExcelCellValue(Excel.Worksheet workSheet, string cell, object value)
        {
            workSheet.get_Range(cell, cell).set_Value(Excel.XlRangeValueDataType.xlRangeValueDefault, value);
        }

        private char PositionNumberToLetter(int number)
        {
            if (number < 0 || number > 25)
                throw new ArgumentOutOfRangeException(nameof(number));

            return "ABCDEFGHIJKLMNOPQRSTUVWXYZ"[number];
        }

        private List<ResultModel> GetBest10Responses(SlideQuestionModel slideQuestionModel, List<ResultModel> responses)
        {
            var correctResponsesresponses = this.FilterForCorrectResponsesClick(slideQuestionModel, responses);

            var best10Responses = new List<ResultModel>();

            for (var i = 0; i < 10; i++)
            {
                if (correctResponsesresponses.Count == 0)
                    break;

                var minResponse = correctResponsesresponses.First(r => r.responseTime == correctResponsesresponses.Min(r2 => r2.responseTime));
                best10Responses.Add(minResponse);
                correctResponsesresponses.Remove(minResponse);
            }

            return best10Responses.OrderBy(r => r.responseTime).ToList();
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
