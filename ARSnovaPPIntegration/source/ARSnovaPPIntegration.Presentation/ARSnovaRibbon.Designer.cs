using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Configuration;
using System.Linq;
using ARSnovaPPIntegration.Presentation.Helpers;
using Microsoft.Office.Tools.Ribbon;
using Svg;

namespace ARSnovaPPIntegration.Presentation
{
    partial class ARSnovaRibbon : Microsoft.Office.Tools.Ribbon.RibbonBase
    {
        private SvgParser svgParser;

        private List<Svg.SvgGlyph> arsnovaGlyphs;

        /// <summary>
        /// Erforderliche Designervariable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        public ARSnovaRibbon()
            : base(Globals.Factory.GetRibbonFactory())
        {
            this.svgParser = new SvgParser(new System.Drawing.Size(4000, 4000));
            var arsnovaSvgDoc = this.svgParser.GetSvgDocument(@"..\..\ARSnovaPPIntegration.Common.Resources\arsnova.svg");
            this.arsnovaGlyphs = arsnovaSvgDoc.Children.FindSvgElementsOf<Svg.SvgGlyph>().ToList();

            InitializeComponent();
        }

        /// <summary> 
        /// Verwendete Ressourcen bereinigen.
        /// </summary>
        /// <param name="disposing">"true", wenn verwaltete Ressourcen gelöscht werden sollen, andernfalls "false".</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Vom Komponenten-Designer generierter Code

        /// <summary>
        /// Erforderliche Methode für Designerunterstützung -
        /// Der Inhalt der Methode darf nicht mit dem Code-Editor geändert werden.
        /// </summary>
        private void InitializeComponent()
        {
            this.arsnovaMainRibbonTab = this.Factory.CreateRibbonTab();
            this.infoGroup = this.Factory.CreateRibbonGroup();
            this.buttonGroup1 = this.Factory.CreateRibbonButtonGroup();
            this.button1 = this.Factory.CreateRibbonButton();
            this.button2 = this.Factory.CreateRibbonButton();
            this.button3 = this.Factory.CreateRibbonButton();
            this.arsnovaMainRibbonTab.SuspendLayout();
            this.infoGroup.SuspendLayout();
            this.buttonGroup1.SuspendLayout();
            this.SuspendLayout();
            // 
            // arsnovaMainRibbonTab
            // 
            this.arsnovaMainRibbonTab.ControlId.ControlIdType = Microsoft.Office.Tools.Ribbon.RibbonControlIdType.Office;
            this.arsnovaMainRibbonTab.Groups.Add(this.infoGroup);
            this.arsnovaMainRibbonTab.Label = "ARSnova";
            this.arsnovaMainRibbonTab.Name = "arsnovaMainRibbonTab";
            // 
            // infoGroup
            // 
            this.infoGroup.Items.Add(this.buttonGroup1);
            this.infoGroup.Label = "Help";
            this.infoGroup.Name = "infoGroup";
            // 
            // buttonGroup1
            // 
            this.buttonGroup1.Items.Add(this.button1);
            this.buttonGroup1.Items.Add(this.button2);
            this.buttonGroup1.Items.Add(this.button3);
            this.buttonGroup1.Name = "buttonGroup1";
            // 
            // button1
            // 
            this.button1.Label = "button1";
            this.button1.Name = "button1";
            this.button1.Image = Common.Resources.Images.ARSnova_Logo;
            // 
            // button2
            // 
            this.button2.Label = "button2";
            this.button2.Name = "button2";
            var svgDoc = new SvgDocument();
            this.button2.Image = this.svgParser.GetGlyphByName(this.arsnovaGlyphs, "info").Draw();
            
            
            // 
            // button3
            // 
            this.button3.Label = "button3";
            this.button3.Name = "button3";
            // 
            // ARSnovaRibbon
            // 
            this.Name = "ARSnovaRibbon";
            this.RibbonType = "Microsoft.PowerPoint.Presentation";
            this.Tabs.Add(this.arsnovaMainRibbonTab);
            this.Load += new Microsoft.Office.Tools.Ribbon.RibbonUIEventHandler(this.ARSnovaRibbon_Load);
            this.arsnovaMainRibbonTab.ResumeLayout(false);
            this.arsnovaMainRibbonTab.PerformLayout();
            this.infoGroup.ResumeLayout(false);
            this.infoGroup.PerformLayout();
            this.buttonGroup1.ResumeLayout(false);
            this.buttonGroup1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private void helpButton_Click(object sender, RibbonControlEventArgs e)
        {
            throw new NotImplementedException();
        }

        private void feedbackButton_Click(object sender, RibbonControlEventArgs e)
        {
            throw new NotImplementedException();
        }

        private void aboutButton_Click(object sender, RibbonControlEventArgs e)
        {
            throw new NotImplementedException();
        }

        internal Microsoft.Office.Tools.Ribbon.RibbonTab arsnovaMainRibbonTab;
        internal Microsoft.Office.Tools.Ribbon.RibbonGroup infoGroup;
        internal Microsoft.Office.Tools.Ribbon.RibbonButton help;
        internal Microsoft.Office.Tools.Ribbon.RibbonButton feedback;
        internal RibbonButtonGroup buttonGroup1;
        internal RibbonButton button1;
        internal RibbonButton button2;
        internal RibbonButton button3;
    }

    partial class ThisRibbonCollection
    {
        internal ARSnovaRibbon ARSnovaRibbon
        {
            get { return this.GetRibbon<ARSnovaRibbon>(); }
        }
    }
}
