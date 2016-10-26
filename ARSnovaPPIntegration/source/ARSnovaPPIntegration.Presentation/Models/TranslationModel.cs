using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Practices.ServiceLocation;

using ARSnovaPPIntegration.Common.Contract;

namespace ARSnovaPPIntegration.Presentation.Models
{
    public class TranslationModel
    {
        private readonly ILocalizationService localizationService;

        public TranslationModel()
        {
            this.localizationService = ServiceLocator.Current.GetInstance<ILocalizationService>();
        }

        public string Translate(string text)
        {
            return this.localizationService.Translate(text);
        }
    }
}
