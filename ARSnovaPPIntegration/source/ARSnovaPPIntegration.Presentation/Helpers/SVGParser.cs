using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using ARSnovaPPIntegration.Common.Resources;
using Svg;

namespace ARSnovaPPIntegration.Presentation.Helpers
{
    public class SvgParser
    {
        private Size maximumSize;

        public SvgParser(Size maximumSize)
        {
            this.maximumSize = maximumSize;
        }

        public SvgDocument GetSvgDocument(string filePath)
        {
            var document = SvgDocument.Open(filePath);
            return this.AdjustSize(document);
        }

        /// <summary>
        /// Returns the first element in the given collection or null if there is no matching element. If multiple glyphs with the same name exists, the first one will be returned.
        /// </summary>
        /// <param name="arsnovaGlyphs"></param>
        /// <param name="glyphName"></param>
        /// <returns></returns>
        public SvgDocument GetGlyphByName(List<Svg.SvgGlyph> arsnovaGlyphs, string glyphName)
        {
            var glyph = arsnovaGlyphs.First(ag => ag.GlyphName == glyphName);

            if (glyph == null)
                return null;

            var svgDocument = new SvgDocument();
            svgDocument.Children.Add(glyph);
            return svgDocument;
        }

        private SvgDocument AdjustSize(SvgDocument document)
        {
            if (document.Height > this.maximumSize.Height)
            {
                document.Width = (int)((document.Width / (double)document.Height) * this.maximumSize.Height);
                document.Height = this.maximumSize.Height;
            }
            return document;
        }
    }
}
