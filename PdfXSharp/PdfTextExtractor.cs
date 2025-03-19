
using Newtonsoft.Json;
using PdfXSharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Xml.Serialization;

public class  PdfTextExtractor
{
    public  PdfDocument PdfTextExtract(string pdfPath)
    {
        PdfDocument document = new PdfDocument();
        try
        {
            byte[] pdfBytes = File.ReadAllBytes(pdfPath);
            string rawContent = Encoding.ASCII.GetString(pdfBytes);

            // Step 1: Decompress PDF streams (FlateDecode)
            string decompressedContent = DecompressFlateStreams(pdfBytes, rawContent);

            // Step 2: Extract text with positions
            ExtractTextWithPositioning(decompressedContent, document);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error extracting text: {ex.Message}");
        }
        return document;
    }

    public static void ExtractTextWithPositioning(string content, PdfDocument document)
    {
        var textBlocks = new List<PdfTextBlock>();
        var matches = Regex.Matches(content, @"BT\s*(.*?)\s*ET", RegexOptions.Singleline);

        foreach (Match match in matches)
        {
            string textBlock = match.Groups[1].Value;
            var textMatches = Regex.Matches(textBlock, @"([-+]?\d*\.\d+|\d+)\s+([-+]?\d*\.\d+|\d+)\s+Td\s*\((.*?)\)|<([0-9A-Fa-f]+)>");

            float x = 0, y = 0;
            foreach (Match textMatch in textMatches)
            {
                if (!string.IsNullOrEmpty(textMatch.Groups[1].Value) && !string.IsNullOrEmpty(textMatch.Groups[2].Value))
                {
                    x = float.Parse(textMatch.Groups[1].Value);
                    y = float.Parse(textMatch.Groups[2].Value);
                }

                string text = "";
                if (!string.IsNullOrEmpty(textMatch.Groups[3].Value))
                {
                    text = Regex.Unescape(textMatch.Groups[3].Value);
                }
                else if (!string.IsNullOrEmpty(textMatch.Groups[4].Value))
                {
                    text = HexToString(textMatch.Groups[4].Value);
                }

                textBlocks.Add(new PdfTextBlock { X = x, Y = y, Text = text, Font = "Unknown" });
            }
        }

        document.TextBlocks = textBlocks;
    }

    private static string DecompressFlateStreams(byte[] pdfBytes, string rawContent)
    {
        StringBuilder decompressedContent = new StringBuilder();
        byte[] streamMarker = Encoding.ASCII.GetBytes("stream");
        byte[] endStreamMarker = Encoding.ASCII.GetBytes("endstream");

        for (int i = 0; i < pdfBytes.Length; i++)
        {
            if (ByteMatch(pdfBytes, i, streamMarker))
            {
                int streamStart = i + streamMarker.Length;
                int streamEnd = FindBytePattern(pdfBytes, streamStart, endStreamMarker);
                if (streamEnd == -1) continue;

                byte[] streamData = new byte[streamEnd - streamStart];
                Array.Copy(pdfBytes, streamStart, streamData, 0, streamData.Length);

                try
                {
                    byte[] decompressed = DecompressFlate(streamData);
                    string decompressedText = Encoding.UTF8.GetString(decompressed);
                    decompressedContent.AppendLine(decompressedText);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Decompression error: {ex.Message}");
                }
            }
        }
        return decompressedContent.ToString();
    }

    private static bool ByteMatch(byte[] bytes, int index, byte[] pattern)
    {
        if (index + pattern.Length > bytes.Length) return false;
        for (int i = 0; i < pattern.Length; i++)
        {
            if (bytes[index + i] != pattern[i]) return false;
        }
        return true;
    }

    private static int FindBytePattern(byte[] bytes, int start, byte[] pattern)
    {
        for (int i = start; i <= bytes.Length - pattern.Length; i++)
        {
            if (ByteMatch(bytes, i, pattern)) return i;
        }
        return -1;
    }

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
            return Encoding.UTF8.GetString(bytes);
        }
        catch
        {
            return "";
        }
    }

    public static string ExportToJson(PdfDocument document)
    {
        return System.Text.Json.JsonSerializer.Serialize(document, new JsonSerializerOptions { WriteIndented = true });
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

