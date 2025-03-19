using PdfXSharp;
using System;
using System.Collections.Generic;
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
            PdfTextExtractor textExtractor = new PdfTextExtractor();

            document = textExtractor.PdfTextExtract(pdfPath);

            byte[] pdfBytes = File.ReadAllBytes(pdfPath);
            string rawContent = Encoding.ASCII.GetString(pdfBytes);

            // Step 1: Decompress streams (if possible)
            string decompressedContent = DecompressFlateStreams(pdfBytes, rawContent);

            // Step 2: Extract page count
            document.PdfPages = Regex.Matches(decompressedContent, @"/Type\s*/Page\b").Count;

            // Step 3: Extract page sizes
            var mediaBoxMatches = Regex.Matches(decompressedContent, @"/MediaBox\s*\[\s*([\d.]+)\s+([\d.]+)\s+([\d.]+)\s+([\d.]+)\s*\]");
            foreach (Match match in mediaBoxMatches)
            {
                float width = float.Parse(match.Groups[3].Value);
                float height = float.Parse(match.Groups[4].Value);
                document.PageSizes.Add(((int)width, (int)height));
            }

            // Step 4: Extract text (improved regex)
            var textMatches = Regex.Matches(decompressedContent, @"(?:\((.*?)\)|<([0-9A-Fa-f]+)>)\s*T[Jj]");
            foreach (Match match in textMatches)
            {
                string text = "";
                if (!string.IsNullOrEmpty(match.Groups[1].Value))
                {
                    text = Regex.Unescape(match.Groups[1].Value); // Handle escaped chars like \(, \n
                }
                else if (!string.IsNullOrEmpty(match.Groups[2].Value))
                {
                    text = HexToString(match.Groups[2].Value);
                }
                document.PageTexts.Add(text);
            }

        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
        }
        return document;
    }

    // Helper: Decompress FlateDecode streams
    private static string DecompressFlateStreams(byte[] pdfBytes, string rawContent)
    {
        StringBuilder decompressedContent = new StringBuilder(rawContent);
        byte[] streamMarker = Encoding.ASCII.GetBytes("stream");
        byte[] endStreamMarker = Encoding.ASCII.GetBytes("endstream");

        // Get Latin-1 encoding (ISO-8859-1)
        Encoding latin1Encoding;
        try
        {
            latin1Encoding = Encoding.GetEncoding(28591); // Code page for ISO-8859-1
        }
        catch
        {
            // Fallback to ASCII if Latin-1 is unavailable
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

    // Helper: Convert hex to text
    private static string HexToString(string hex)
    {
        hex = hex.Replace(" ", "");
        try
        {
            byte[] bytes = new byte[hex.Length / 2];
            for (int i = 0; i < bytes.Length; i++)
            {
                bytes[i] = Convert.ToByte(hex.Substring(i * 2, 2), 16);
            }
            return Encoding.ASCII.GetString(bytes);
        }
        catch
        {
            return "";
        }
    }

}
