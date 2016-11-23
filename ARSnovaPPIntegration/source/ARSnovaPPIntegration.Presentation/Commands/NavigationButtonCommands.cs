using System.Windows.Input;

namespace ARSnovaPPIntegration.Presentation.Commands
{
    public static class NavigationButtonCommands
    {
        public static RoutedUICommand Back { get; } = new RoutedUICommand("Back", "Back", typeof(NavigationButtonCommands));
    }
}
