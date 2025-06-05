
<div align="center">
  <h1>📄 PdfXSharp-C-Sharp</h1>
  <p><em>Empower Your PDFs with Seamless Creation and Control</em></p>

  <img src="https://img.shields.io/github/last-commit/Tabishkhan01/PdfXSharp-C-Sharp" alt="Last Commit">
  <img src="https://img.shields.io/github/languages/top/Tabishkhan01/PdfXSharp-C-Sharp" alt="Top Language">
  <img src="https://img.shields.io/github/languages/count/Tabishkhan01/PdfXSharp-C-Sharp" alt="Language Count">

  <p><em>Built using modern technologies:</em></p>
  <img src="https://img.shields.io/badge/NuGet-Package-blue" alt="NuGet">
  <img src="https://img.shields.io/badge/C%23-8.0-blueviolet" alt="C#">
</div>

---

## 🚀 Overview

**PdfXSharp-C-Sharp** is a lightweight C# utility for working with PDF documents using the PdfSharp library. It enables developers to extract text, automate processing, and build custom PDF workflows quickly and reliably.

---

## 🛠 Features

- ✅ Extract text from PDF files
- ✅ Basic PDF creation and manipulation
- ✅ Add watermarking or annotations (planned)
- ✅ Password protection support (planned)
- ✅ Easy integration with WinForms/.NET projects

---

## 📦 Installation

1. Clone the repository:
   ```bash
   git clone https://github.com/Tabishkhan01/PdfXSharp-C-Sharp.git
   ```

2. Open the solution in Visual Studio.

3. Restore NuGet packages and build the project.

---

## 🧑‍💻 Usage

### Example: Extract Text

```csharp
PdfTextExtractor extractor = new PdfTextExtractor("sample.pdf");
string content = extractor.ExtractAllText();
Console.WriteLine(content);
```

---

## 📁 Project Structure

```plaintext
PdfXSharp-C-Sharp/
├── src/              # Core library
├── examples/         # Sample implementation
├── assets/           # Sample PDFs and images
└── README.md         # Project documentation
```

---

## 📊 Technologies Used

- C#
- .NET Core / .NET Framework
- PdfSharp / PdfSharpCore
- GitHub Actions (CI/CD)
- Visual Studio

---

## 🧪 Roadmap

- [x] Basic text extraction
- [ ] PDF page split/merge
- [ ] Watermarking
- [ ] PDF encryption/passwords
- [ ] Export to Word/Image

---

## 🤝 Contributing

Contributions are welcome! Feel free to:
- Fork the project
- Open issues
- Submit pull requests

---

## 📄 License

This project is licensed under the **MIT License**. See the [LICENSE](LICENSE) file for more details.

---

