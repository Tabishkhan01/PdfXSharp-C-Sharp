using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PdfXSharp
{
    public class PdfDocument
    {
        public  int PdfPages { get; set; }
        public List<(int Width, int Height)> PageSizes { get; set; } = new List<(int, int)>();
        public List<string> PageTexts { get; set; } = new List<string>();
        public List<PdfTextBlock> TextBlocks { get; set; } = new List<PdfTextBlock>();


    }
    public class PdfTextBlock
    {
        public float X { get; set; }
        public float Y { get; set; }
        public string Text { get; set; }
        public string Font { get; set; }
    }
}
