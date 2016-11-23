using System;
using System.Collections.Generic;
using System.Globalization;
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
using ARSnovaPPIntegration.Presentation.Commands;
using ARSnovaPPIntegration.Presentation.Helpers;

namespace ARSnovaPPIntegration.Presentation.Window
{
    /// <summary>
    /// Interaktionslogik für WindowContainer.xaml
    /// </summary>
    public partial class WindowContainer
    {
        // TODO Do I need onPropertyChanged Events here (will there be any changing tooltips / button bindings?)

        private readonly NavigationButtonsVisibilities navigationButtonsVisibilities = new NavigationButtonsVisibilities();

        public NavigationButtonsToolTips NavigationButtonsToolTips { get; } = new NavigationButtonsToolTips();

        public WindowContainer()
        {
            this.DataContext = this;
            this.InitializeComponent();
        }

        public bool BackButtonVisibility
        {
            get
            {
                var hasCommandBinding =
                    this.CommandBindings.OfType<CommandBinding>().Any(c => c.Command == NavigationButtonCommands.Back);
                var isVisible = this.navigationButtonsVisibilities.Back;
                return hasCommandBinding && isVisible;
            }
        }
    }
}
