using MimeDetective.Analyzers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace MimeDetective.Tests.Analyzers
{
    public class ZipFileAnalyzerTests
    {
        [Fact]
        public void DefaultConstructor()
        {
            var analyzer = new ZipFileAnalyzer();

            //assertion here just to have
            Assert.NotNull(analyzer);
        }


        [Theory]
        [InlineData("./Data/Documents/PptxPowerpoint2016.pptx", "pptx")]
        [InlineData("./Data/Documents/StrictOpenXMLWord2016.docx", "docx")]
        [InlineData("./Data/Documents/XlsxExcel2016.xlsx", "xlsx")]
        [InlineData("./Data/Documents/DocxWord2016.docx", "docx")]
        [InlineData("./Data/Zip/Images.zip", "zip")]
        [InlineData("./Data/Zip/ImagesBy7zip.zip", "zip")]
        public async Task Search(string path, string ext)
        {
            var analyzer = new ZipFileAnalyzer();
            FileInfo file = new FileInfo(path);
            FileType type = null;

            using (ReadResult result = await ReadResult.ReadFileHeaderAsync(file))
            {
                type = analyzer.Search(in result);
            }

            Assert.NotNull(type);
            Assert.Contains(ext, type.Extension);
        }
    }
}
