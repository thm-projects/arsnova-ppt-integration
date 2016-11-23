using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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

namespace ARSnovaPPIntegration.Presentation.Views
{
    /// <summary>
    /// Interaktionslogik für EditArsnovaVotingView.xaml
    /// </summary>
    public partial class EditArsnovaVotingView : UserControl
    {
        public readonly ILocalizationService LocalizationService;

        public EditArsnovaVotingView(ILocalizationService localizationService)
        {
            this.LocalizationService = localizationService;
            InitializeComponent();
        }
    }
}
