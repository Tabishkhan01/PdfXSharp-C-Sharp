using PdfXSharp;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.IO.Compression;
using System.Text;
using System.Text.RegularExpressions;

public class PdfReader
{
    public static PdfDocument Open(string pdfPath)
    {
        PdfDocument document = new PdfDocument();
        try
        {
            byte[] pdfBytes = File.ReadAllBytes(pdfPath);
            string rawContent = Encoding.ASCII.GetString(pdfBytes);
            string decompressedContent = DecompressFlateStreams(pdfBytes, rawContent);
            int pagecount = Regex.Matches(decompressedContent, @"/Type\s*/Page\b").Count;
            
            PdfTextExtractor textExtractor = new PdfTextExtractor();
            document = textExtractor.ExtractText(pdfPath, pagecount);

            var mediaBoxMatches = Regex.Matches(decompressedContent, @"/MediaBox\s*\[\s*([\d.]+)\s+([\d.]+)\s+([\d.]+)\s+([\d.]+)\s*\]");
           
            for (int i = 0; i < mediaBoxMatches.Count && i < document.Pages.Count; i++)
            {
                Match match = mediaBoxMatches[i];

                if (match.Success)
                {
                    float width = float.Parse(match.Groups[3].Value, CultureInfo.InvariantCulture);
                    float height = float.Parse(match.Groups[4].Value, CultureInfo.InvariantCulture);

                    document.Pages[i].Width = (int)width;
                    document.Pages[i].Height = (int)height;
                }
            }

        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
        }
        return document;
    }

    private static string DecompressFlateStreams(byte[] pdfBytes, string rawContent)
    {
        StringBuilder decompressedContent = new StringBuilder(rawContent);
        byte[] streamMarker = Encoding.ASCII.GetBytes("stream");
        byte[] endStreamMarker = Encoding.ASCII.GetBytes("endstream");

        // Get Latin-1 encoding (ISO-8859-1)
        Encoding latin1Encoding;
        try
        {
            latin1Encoding = Encoding.GetEncoding(28591); 
        }
        catch
        {
           
            latin1Encoding = Encoding.ASCII;
        }

        for (int i = 0; i < pdfBytes.Length; i++)
        {
            if (ByteMatch(pdfBytes, i, streamMarker))
            {
                int streamStart = i + streamMarker.Length;
                int streamEnd = FindBytePattern(pdfBytes, streamStart, endStreamMarker);
                if (streamEnd == -1) continue;

                string precedingText = Encoding.ASCII.GetString(pdfBytes, 0, i);
                if (Regex.IsMatch(precedingText, @"/Filter\s*/FlateDecode"))
                {
                    byte[] streamData = new byte[streamEnd - streamStart];
                    Array.Copy(pdfBytes, streamStart, streamData, 0, streamData.Length);

                    try
                    {
                        byte[] decompressed = DecompressFlate(streamData);
                        // Use Latin-1 encoding here
                        string decompressedText = latin1Encoding.GetString(decompressed);
                        decompressedContent.AppendLine(decompressedText);
                    }
                    catch { }
                }
                i = streamEnd + endStreamMarker.Length - 1;
            }
        }
        return decompressedContent.ToString();
    }

    // Helper: Match byte pattern
    private static bool ByteMatch(byte[] bytes, int index, byte[] pattern)
    {
        if (index + pattern.Length > bytes.Length) return false;
        for (int i = 0; i < pattern.Length; i++)
        {
            if (bytes[index + i] != pattern[i]) return false;
        }
        return true;
    }

    // Helper: Find byte pattern
    private static int FindBytePattern(byte[] bytes, int start, byte[] pattern)
    {
        for (int i = start; i <= bytes.Length - pattern.Length; i++)
        {
            if (ByteMatch(bytes, i, pattern)) return i;
        }
        return -1;
    }

    // Helper: Decompress Flate data
    private static byte[] DecompressFlate(byte[] data)
    {
        try
        {
            using (MemoryStream ms = new MemoryStream(data))
            {
                if (data.Length > 2 && (data[0] == 0x78 || data[0] == 0x58)) ms.Position = 2;
                using (DeflateStream decompressor = new DeflateStream(ms, CompressionMode.Decompress))
                using (MemoryStream output = new MemoryStream())
                {
                    decompressor.CopyTo(output);
                    return output.ToArray();
                }
            }
        }
        catch
        {
            return new byte[0];
        }
    }

    
}
