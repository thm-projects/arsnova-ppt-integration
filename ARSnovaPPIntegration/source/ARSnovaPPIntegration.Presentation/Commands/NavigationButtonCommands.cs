using System.Windows.Input;

namespace ARSnovaPPIntegration.Presentation.Commands
{
    public static class NavigationButtonCommands
    {
        public static RoutedUICommand Back { get; } = new RoutedUICommand("Back", "Back", typeof(NavigationButtonCommands));

        public static RoutedUICommand Forward { get; } = new RoutedUICommand("Forward", "Forward", typeof(NavigationButtonCommands));

        public static RoutedUICommand Cancel { get; } = new RoutedUICommand("Cancel", "Cancel", typeof(NavigationButtonCommands));

        public static RoutedUICommand Finish { get; } = new RoutedUICommand("Finish", "Finish", typeof(NavigationButtonCommands));

        public static RoutedUICommand New { get; } = new RoutedUICommand("New", "New", typeof(NavigationButtonCommands));

        public static RoutedUICommand Edit { get; } = new RoutedUICommand("Edit", "Edit", typeof(NavigationButtonCommands));

        public static RoutedUICommand Delete { get; } = new RoutedUICommand("Delete", "Delete", typeof(NavigationButtonCommands));
    }
}
