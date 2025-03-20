using System;
using System.Collections.Generic;
using System.Text;

namespace PdfXSharp
{
    public class PdfResolveEventArgs : EventArgs
    {
        public string PdfFileName { get; set; }
    }

    public delegate void PdfResolveEventHandler(object sender, PdfResolveEventArgs e);
}
