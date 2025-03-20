using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PdfXSharp
{
    public class PdfDocument1
    {
        public  int PdfPagesCount { get; set; }
        public List<PdfPage> Pages { get; set; } = new List<PdfPage>();

    }
    
    public class PdfPage
    {
        public int PageNumber { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public string Text { get; set; }
    }
}
