using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ARSnovaPPIntegration.Common.Contract;
using ARSnovaPPIntegration.Presentation.Models;
using Microsoft.Office.Interop.PowerPoint;

namespace ARSnovaPPIntegration.Presentation.Helpers
{
    public class RibbonHelper
    {
        private readonly ViewPresenter.ViewPresenter viewPresenter;

        private readonly ILocalizationService localizationService;

        public RibbonHelper(
            ViewPresenter.ViewPresenter viewPresenter,
            ILocalizationService localizationService)
        {
            this.viewPresenter = viewPresenter;
            this.localizationService = localizationService;
        }

        public void AddQuizToSlide(Slide slide)
        {
            this.viewPresenter.Show(
                new EditArsnovaVotingViewModel(
                    this.viewPresenter,
                    this.localizationService));
        }
    }
}
