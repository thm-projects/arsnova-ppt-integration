using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using ARSnovaPPIntegration.Common.Contract;
using ARSnovaPPIntegration.Presentation.Helpers;
using Microsoft.Practices.Unity;
using Office = Microsoft.Office.Core;

// TODO:  Führen Sie diese Schritte aus, um das Element auf dem Menüband (XML) zu aktivieren:

// 1: Kopieren Sie folgenden Codeblock in die ThisAddin-, ThisWorkbook- oder ThisDocument-Klasse.

//  protected override Microsoft.Office.Core.IRibbonExtensibility CreateRibbonExtensibilityObject()
//  {
//      return new Ribbon();
//  }

// 2. Erstellen Sie Rückrufmethoden im Abschnitt "Menübandrückrufe" dieser Klasse, um Benutzeraktionen
//    zu behandeln, z.B. das Klicken auf eine Schaltfläche. Hinweis: Wenn Sie dieses Menüband aus dem Menüband-Designer exportiert haben,
//    verschieben Sie den Code aus den Ereignishandlern in die Rückrufmethoden, und ändern Sie den Code für die Verwendung mit dem
//    Programmmodell für die Menübanderweiterung (RibbonX).

// 3. Weisen Sie den Steuerelementtags in der Menüband-XML-Datei Attribute zu, um die entsprechenden Rückrufmethoden im Code anzugeben.  

// Weitere Informationen erhalten Sie in der Menüband-XML-Dokumentation in der Hilfe zu Visual Studio-Tools für Office.


namespace ARSnovaPPIntegration.Presentation
{
    [ComVisible(true)]
    public class Ribbon : Office.IRibbonExtensibility
    {
        [Dependency]
        public ILocalizationService LocalizationService { get; set; }

        private Office.IRibbonUI ribbon;

        private SvgParser svgParser;

        private List<Svg.SvgGlyph> arsnovaGlyphs;

        public Ribbon()
        {
            this.svgParser = new SvgParser(new System.Drawing.Size(4000, 4000));
            var arsnovaSvgDoc = this.svgParser.GetSvgDocument(@"..\..\ARSnovaPPIntegration.Common.Resources\arsnova.svg");
            this.arsnovaGlyphs = arsnovaSvgDoc.Children.FindSvgElementsOf<Svg.SvgGlyph>().ToList();
        }

        #region manageQuiz

        public string GetQuizGroupLabel(Office.IRibbonControl control)
        {
            return this.LocalizationService.Translate("Manage Quiz");
        }

        public string GetAddButtonLabel(Office.IRibbonControl control)
        {
            return "Add";
        }

        public string GetAddButtonSupertip(Office.IRibbonControl control)
        {
            return "Add";
        }

        public Bitmap GetAddButtonImage(Office.IRibbonControl control)
        {
            return ARSnovaPPIntegration.Common.Resources.Images.Add;
        }

        public void AddButtonClick(Office.IRibbonControl control)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region infoGroup

        public string GetInfoGroupLabel(Office.IRibbonControl control)
        {
            return "Info";
        }

        // HelpButton

        public string GetHelpButtonLabel(Office.IRibbonControl control)
        {
            return "Help";
        }

        public string GetHelpButtonSupertip(Office.IRibbonControl control)
        {
            return "Help";
        }

        public Bitmap GetHelpButtonImage(Office.IRibbonControl control)
        {
            return ARSnovaPPIntegration.Common.Resources.Images.Info;

            //return this.svgParser.GetGlyphByName(this.arsnovaGlyphs, "info").Draw(); TODO currently not working as expected
        }

        public void HelpButtonClick(Office.IRibbonControl control)
        {
            throw new NotImplementedException();
        }

        // AboutButton

        public string GetAboutButtonLabel(Office.IRibbonControl control)
        {
            return "About";
        }

        public string GetAboutButtonSupertip(Office.IRibbonControl control)
        {
            return "About";
        }

        public Bitmap GetAboutButtonImage(Office.IRibbonControl control)
        {
            return ARSnovaPPIntegration.Common.Resources.Images.ARSnova_Logo;
        }

        public void AboutButtonClick(Office.IRibbonControl control)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region IRibbonExtensibility-Member

        public string GetCustomUI(string ribbonID)
        {
            return GetResourceText("ARSnovaPPIntegration.Presentation.Ribbon.xml");
        }

        #endregion

        #region Menübandrückrufe
        //Erstellen Sie hier Rückrufmethoden. Weitere Informationen zum Hinzufügen von Rückrufmethoden finden Sie unter "http://go.microsoft.com/fwlink/?LinkID=271226".

        public void Ribbon_Load(Office.IRibbonUI ribbonUI)
        {
            this.ribbon = ribbonUI;
        }

        #endregion

        #region Hilfsprogramme

        private static string GetResourceText(string resourceName)
        {
            Assembly asm = Assembly.GetExecutingAssembly();
            string[] resourceNames = asm.GetManifestResourceNames();
            for (int i = 0; i < resourceNames.Length; ++i)
            {
                if (string.Compare(resourceName, resourceNames[i], StringComparison.OrdinalIgnoreCase) == 0)
                {
                    using (StreamReader resourceReader = new StreamReader(asm.GetManifestResourceStream(resourceNames[i])))
                    {
                        if (resourceReader != null)
                        {
                            return resourceReader.ReadToEnd();
                        }
                    }
                }
            }
            return null;
        }

        #endregion
    }
}
