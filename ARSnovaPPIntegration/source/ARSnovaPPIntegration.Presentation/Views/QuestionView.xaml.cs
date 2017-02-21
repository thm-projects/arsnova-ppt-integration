using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using ARSnovaPPIntegration.Common.Contract;
using ARSnovaPPIntegration.Common.Enum;

namespace ARSnovaPPIntegration.Presentation.Views
{
    /// <summary>
    /// Interaktionslogik für EditArsnovaVotingView.xaml
    /// </summary>
    public partial class QuestionView : UserControl
    {
        public QuestionView()
        {
            this.InitializeComponent();

            this.Loaded += (o, e) =>
                      {
                          this.QuestionTextBox.Dispatcher.BeginInvoke(
                                  new Action(
                                      () =>
                                      {
                                          this.QuestionTextBox.Focus();
                                      }));
                      };
        }

        private void NumberValidationTextBox(object sender, TextCompositionEventArgs e)
        {
            var regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }
    }
}
