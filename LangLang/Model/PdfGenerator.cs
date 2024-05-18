using System.Collections.Generic;
using PdfSharp.Pdf;
using PdfSharp.Drawing;

namespace LangLang.Model
{
    public class PdfGenerator
    {
        private PdfDocument document;
        private PdfPage page;
        private XGraphics gfx;
        private XFont fontTitle;
        private XFont fontNormal;
        private int marginTop = 40;
        private int marginLeft = 50;
        private int lineHeight = 20;

        public PdfGenerator(int titleFontSize, int textFontSize, int marginTop, int marginLeft, int lineHeight)
        {
            document = new PdfDocument();
            page = document.AddPage();
            gfx = XGraphics.FromPdfPage(page);
            fontTitle = new XFont("Verdana", titleFontSize);
            fontNormal = new XFont("Verdana", textFontSize);

            this.marginTop = marginTop;
            this.marginLeft = marginLeft;
            this.lineHeight = lineHeight;
        }
        public PdfGenerator(int marginTop, int marginLeft, int lineHeight)
        {
            document = new PdfDocument();
            page = document.AddPage();
            gfx = XGraphics.FromPdfPage(page);
            fontTitle = new XFont("Verdana", 20);
            fontNormal = new XFont("Verdana", 12);

            this.marginTop = marginTop;
            this.marginLeft = marginLeft;
            this.lineHeight = lineHeight;
        }
        public PdfGenerator()
        {
            document = new PdfDocument();
            page = document.AddPage();
            gfx = XGraphics.FromPdfPage(page);
            fontTitle = new XFont("Verdana", 20);
            fontNormal = new XFont("Verdana", 12);
        }
        public void AddTitle(string title)
        {
            XRect titleRect = new XRect(0, marginTop, page.Width.Point, lineHeight);
            gfx.DrawString(title, fontTitle, XBrushes.Black, titleRect, XStringFormats.TopCenter);
        }

        public void AddTable<TKey, TValue>(Dictionary<TKey, TValue> data)
        {
            double tableWidth = page.Width.Point - (2 * marginLeft);
            int x = marginLeft;
            int y = marginTop + lineHeight + 20;

            DrawTableHeader(x, y, tableWidth);
            y += lineHeight;
            DrawTable(data, x, y, tableWidth);
        }

        private void DrawTableHeader(int x, int y, double tableWidth)
        {
            gfx.DrawString("Course", fontNormal, XBrushes.Black, x + (tableWidth / 4), y);
            gfx.DrawString("Penalty", fontNormal, XBrushes.Black, x + (3 * tableWidth / 4), y);
        }

        private void DrawTable<TKey, TValue>(Dictionary<TKey, TValue> data, int x, int y, double tableWidth)
        {
            foreach (var item in data)
            {
                string keyString = item.Key.ToString();
                string valueString = item.Value.ToString();

                gfx.DrawString(keyString, fontNormal, XBrushes.Black, x + (tableWidth / 4), y);
                gfx.DrawString(valueString, fontNormal, XBrushes.Black, x + (3 * tableWidth / 4), y);
                y += lineHeight;
            }
        }

        public void AddPage()
        {
            page = document.AddPage();
        }

        public PdfDocument GetPdfDocument()
        {
            return document;
        }
        public void Save(string filePath)
        {
            document.Save(filePath);
        }
    }
}
