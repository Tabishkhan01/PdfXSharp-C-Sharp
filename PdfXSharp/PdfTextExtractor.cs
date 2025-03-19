using PdfiumViewer;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Xml.Serialization;



public class PdfTextExtractor
{
    public PdfDocument ExtractText(string pdfPath)
    {
        PdfDocument document = new PdfDocument();

        try
        {
            using (PdfDocument pdf = PdfDocument.Load(pdfPath))
            {
                int totalPages = pdf.PageCount;
                document.PdfPagesCount = totalPages;

                for (int page = 0; page < totalPages; page++)  // PdfiumViewer uses 0-based indexing
                {
                    string extractedText = pdf.GetPdfText(page);
                    document.Pages.Add(new PdfPage { PageNumber = page + 1, Text = extractedText });
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error extracting text: {ex.Message}");
        }

        return document;
    }

    public static string ExportToJson(PdfDocument document)
    {
        return JsonSerializer.Serialize(document, new JsonSerializerOptions { WriteIndented = true });
    }

    public static string ExportToXml(PdfDocument document)
    {
        XmlSerializer serializer = new XmlSerializer(typeof(PdfDocument));
        using (StringWriter writer = new StringWriter())
        {
            serializer.Serialize(writer, document);
            return writer.ToString();
        }
    }
}
