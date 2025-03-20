using PdfXSharp;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace PdfXtester
{
    class Program
    {
        static void Main(string[] args)
        {
            //using (PdfDocument pdf = PdfDocument.Load(@"D:\C#Projects\PdfXSharp\testpdf\Example1.pdf"))
            //{
            //    int totalPages = pdf.PageCount;
            //    for (int page = 0; page < totalPages; page++)
            //    {
            //        string extractedText = pdf.GetPdfText(page);

            //    }

            //}
            //PdfDocument document = PdfReader.Open(@"D:\C#Projects\PdfXSharp\testpdf\testp.pdf");
            SplitPdf(@"D:\C#Projects\PdfXSharp\testpdf\Example1.pdf", @"D:\C#Projects\PdfXSharp\testpdf");
        }
        static void SplitPdf(string inputPath, string outputFolder)
        {
            // Ensure the output directory exists
            Directory.CreateDirectory(outputFolder);

            //using (var document = PdfDocument.Load(inputPath))
            //{
            //    int totalPages = document.PageCount;
            //    for (int i = 0; i < totalPages; i++)
            //    {
            //        using (var singlePageDoc = PdfDocument.Load(inputPath))
            //        {
            //            for (int j = totalPages - 1; j >= 0; j--)
            //            {
            //                if (j != i)
            //                {
            //                    singlePageDoc.DeletePage(j);
            //                }
            //            }   

            //            string outputPdfPath = Path.Combine(outputFolder, $"Page_{i + 1}.pdf");
            //            singlePageDoc.Save(outputPdfPath);
            //        }
            //    }
            //}
            Directory.CreateDirectory(outputFolder);

            using (var document = PdfDocument.Load(inputPath))
            {
                int totalPages = document.PageCount;

                for (int i = 0; i < totalPages; i++)
                {
                    using (var singlePageDoc = PdfDocument.CreateNew())
                    {
                        singlePageDoc.AddPage(document, i);
                        string outputPdfPath = Path.Combine(outputFolder, $"Page_{i + 1}.pdf");
                        singlePageDoc.Save(outputPdfPath);
                    }
                }
            }
        }
    }
}
