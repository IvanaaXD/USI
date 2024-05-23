using System.Collections.Generic;
using iText.Layout;
using iText.Layout.Element;
using iText.Kernel.Pdf;
using iText.Kernel.Font;
using iText.IO.Font.Constants;
using iText.Layout.Properties;

public class PdfGenerator
{
    private PdfDocument pdfDocument;
    private iText.Layout.Document document;

    public PdfGenerator(string filePath)
    {
        pdfDocument = new PdfDocument(new PdfWriter(filePath));
        document = new iText.Layout.Document(pdfDocument);
    }

    public void AddTitle(string title)
    {
        var titleFont = PdfFontFactory.CreateFont(StandardFonts.HELVETICA_BOLD);
        Paragraph titleParagraph = new Paragraph(title)
            .SetFont(titleFont)
            .SetFontSize(18)
            .SetTextAlignment(TextAlignment.CENTER);
        document.Add(titleParagraph);
    }

    public void AddSubtitle(string subtitle)
    {
        var subtitleFont = PdfFontFactory.CreateFont(StandardFonts.HELVETICA);
        Paragraph subtitleParagraph = new Paragraph(subtitle)
            .SetFont(subtitleFont)
            .SetFontSize(14);
        document.Add(subtitleParagraph);
    }

    public void AddTable<TKey, TValue>(Dictionary<TKey, TValue> tableData, string keyColumnName, string valueColumnName)
    {
        Table table = new Table(2);

        table.AddHeaderCell(keyColumnName).SetWidth(200f);
        table.AddHeaderCell(valueColumnName).SetWidth(200f);

        foreach (var entry in tableData)
        {
            table.AddCell(entry.Key.ToString()).SetWidth(200f);
            table.AddCell(entry.Value.ToString()).SetWidth(200f);
        }

        table.SetHorizontalAlignment(HorizontalAlignment.CENTER);
        document.Add(table);
    }
    public void AddTupleTable<TKey>(Dictionary<TKey, (double, double, double)> tableData, string keyColumnName, string valueOneColumnName, string valueTwoColumnName, string valueThreeColumnName)
    {
        Table table = new Table(4);

        table.AddHeaderCell(keyColumnName).SetWidth(300f);
        table.AddHeaderCell(valueOneColumnName).SetWidth(300f);
        table.AddHeaderCell(valueTwoColumnName).SetWidth(300f);
        table.AddHeaderCell(valueThreeColumnName).SetWidth(300f);

        foreach (var entry in tableData)
        {
            table.AddCell(entry.Key.ToString()).SetWidth(300f);
            table.AddCell(entry.Value.Item1.ToString()).SetWidth(300f);
            table.AddCell(entry.Value.Item2.ToString()).SetWidth(300f);
            table.AddCell(entry.Value.Item3.ToString()).SetWidth(300f);
        }

        table.SetHorizontalAlignment(HorizontalAlignment.CENTER);
        document.Add(table);
    }

    public void AddNewPage()
    {
        document.Add(new AreaBreak(AreaBreakType.NEXT_PAGE));
    }
    public void AddNewLine()
    {
        document.Add(new Paragraph("\n\n"));
    }

    public void SaveAndClose()
    {
        document.Close();
        pdfDocument.Close();
    }
    public PdfDocument GetDocument()
    {
        return pdfDocument;
    }
}
