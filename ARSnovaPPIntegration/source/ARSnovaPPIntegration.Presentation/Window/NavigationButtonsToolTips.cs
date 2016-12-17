using ARSnovaPPIntegration.Common.Contract;
using Microsoft.Practices.ServiceLocation;

namespace ARSnovaPPIntegration.Presentation.Window
{
    public class NavigationButtonsToolTips
    {
        private readonly ILocalizationService localizationService;

        public NavigationButtonsToolTips ()
        {
            this.localizationService = ServiceLocator.Current.GetInstance<ILocalizationService>();
        }

        public string Back => this.localizationService.Translate("Back");

        public string Forward => this.localizationService.Translate("Forward");

        public string Cancel => this.localizationService.Translate("Cancel");

        public string Finish => this.localizationService.Translate("Finish");

        public string New => this.localizationService.Translate("New");

        public string Edit => this.localizationService.Translate("Edit");

        public string Delete => this.localizationService.Translate("Delete");
    }
}
