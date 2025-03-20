using System;
using System.Collections.Generic;
using System.Text;

namespace PdfXSharp
{
    public class PdfResolver
    {
        public static event PdfResolveEventHandler Resolve;

        private static void OnResolve(PdfResolveEventArgs e)
        {
            Resolve?.Invoke(null, e);
        }

        internal static string GetPdfFileName()
        {
            var e = new PdfResolveEventArgs();
            OnResolve(e);
            return e.PdfFileName;
        }
    }
}
