using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MimeDetective;
using Xunit;
using System.IO;
using MimeDetective.Utilities;

namespace MimeDetective.Tests.Text
{
    public class TextTests
    {
        public const string TextPath = "./Data/Text/";

        public const string TextFile = "test.txt";

        [Fact]
        public async Task IsTxt()
        {
            var info = new FileInfo(TextPath + TextFile);

            var fileType = await info.GetFileTypeAsync();

            Assert.Equal(fileType.Extension, MimeTypes.TXT.Extension);
        }

        [Fact]
        public async Task IsXml_UTF8_WithBOM()
        {
            // this XML file is encoded with: UTF-8
            // this XML does NOT include a Byte Order Mark (EF BB BF) to signal the encoding
            var info = new FileInfo(TextPath + "MindMap.NoBOM.smmx");

            var fileType = await info.GetFileTypeAsync();

            Assert.Equal(MimeTypes.XML.Extension, fileType.Extension);
            Assert.Equal("application/xml", fileType.Mime);
        }

        [Fact]
        public async Task IsXml_UTF8_WithoutBOM()
        {
            // this XML file is encoded with: UTF-8
            // this XML INCLUDES a Byte Order Mark (EF BB BF) to signal the encoding
            var info = new FileInfo(TextPath + "MindMap.WithBOM.smmx");

            var fileType = await info.GetFileTypeAsync();

            Assert.Equal(MimeTypes.XML.Extension, fileType.Extension);
            Assert.Equal("application/xml", fileType.Mime);
        }

        [Fact]
        public async Task IsXml_UCS2LE_WithBOM()
        {
            // this XML file is encoded with: UCS-2 Little Endian (UTF16)
            // this XML INCLUDES a Byte Order Mark (FEFF) to signal the encoding
            var info = new FileInfo(TextPath + "MindMap.UCS2LE.WithBOM.smmx");

            var fileType = await info.GetFileTypeAsync();

            Assert.Equal(MimeTypes.XML.Extension, fileType.Extension);
            Assert.Equal("application/xml", fileType.Mime);
        }

        [Fact]
        public async Task IsXml_UCS2BE_WithBOM()
        {
            // this XML file is encoded with: UCS-2 Little Endian (UTF16)
            // this XML INCLUDES a Byte Order Mark (FEFF) to signal the encoding
            var info = new FileInfo(TextPath + "MindMap.UCS2BE.WithBOM.smmx");

            var fileType = await info.GetFileTypeAsync();

            Assert.Equal(MimeTypes.XML.Extension, fileType.Extension);
            Assert.Equal("application/xml", fileType.Mime);
        }
    }
}
