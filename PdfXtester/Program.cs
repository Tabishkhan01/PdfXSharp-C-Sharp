using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using PdfXSharp;
namespace PdfXtester
{
    class Program
    {
        static void Main(string[] args)
        {
            PdfDocument document = PdfReader.Open(@"D:\C#Projects\PdfXSharp\testpdf\testp.pdf");
        }
    }
}
