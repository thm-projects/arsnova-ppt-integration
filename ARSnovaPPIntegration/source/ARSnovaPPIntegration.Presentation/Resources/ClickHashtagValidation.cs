using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows.Controls;

using Microsoft.Practices.ServiceLocation;

using ARSnovaPPIntegration.Business.Contract;
using ARSnovaPPIntegration.Common.Contract;

namespace ARSnovaPPIntegration.Presentation.Resources
{
    public class ClickHashtagValidation : ValidationRule
    {
        private readonly ILocalizationService localizationService;

        private readonly List<string> alreadyTakenHashtags;

        public ClickHashtagValidation()
        {
            this.localizationService = ServiceLocator.Current.GetInstance<ILocalizationService>();
            var sessionInformationProvider = ServiceLocator.Current.GetInstance<ISessionInformationProvider>();

            this.alreadyTakenHashtags = sessionInformationProvider.GetHashtagList();
        }

        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            string hashtag;

            try
            {
                hashtag = value.ToString().ToLower();
            }
            catch (Exception e)
            {
                // should not happen
                return new ValidationResult(false, "Illegal characters or " + e.Message);
            }

            if (string.IsNullOrEmpty(hashtag))
            {
                return new ValidationResult(false, this.localizationService.Translate("Hashtag is needed."));
            }

            if (this.alreadyTakenHashtags.Any(h => h.ToLower() == hashtag))
            {
                return new ValidationResult(false, this.localizationService.Translate("Hashtag already taken."));
            }

            return new ValidationResult(true, null);
        }
    }
}
