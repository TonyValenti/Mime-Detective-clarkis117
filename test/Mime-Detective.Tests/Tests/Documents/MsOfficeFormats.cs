using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;
using System.IO;
using MimeDetective;
using static MimeDetective.Utilities.TypeComparisions;

namespace MimeDetective.Tests.Documents
{
    public class MsOfficeFormats
    {
        public const string DocsPath = "./Data/Documents/";

        //examples of doc which don't match specific sub header but match ms doc header
        [Theory]
        [InlineData("OpenOfficePpt.ppt")]
        [InlineData("OpenOfficeWord6.0Doc.doc")]
        [InlineData("OpenOfficeWord95Doc.doc")]
        [InlineData("OpenOfficeWordDoc.doc")]
        [InlineData("OpenOfficeExcel.xls")]
        [InlineData("OpenOfficeExcel50.xls")]
        [InlineData("OpenOfficeExcel95.xls")]
        [InlineData("XlsExcel2007.xls")]
        public async Task IsMSOLEDocType(string fileName)
        {
            var info = GetFileInfo(DocsPath, fileName);

            await AssertIsType(info, MimeTypes.MS_OFFICE);
        }

        [Theory]
        [InlineData("DocWord2016.doc")]
        [InlineData("DocWord97.doc")]
        [InlineData("OpenOfficeWord6.0Doc.doc")]
        [InlineData("OpenOfficeWord95Doc.doc")]
        [InlineData("OpenOfficeWordDoc.doc")]
        [InlineData("DocxWord2016.docx")]
        [InlineData("StrictOpenXMLWord2016.docx")]
        public void IsDoc(string filePath)
        {
            var info = GetFileInfo(DocsPath, filePath);

            Assert.True(info.IsWord());
        }

        [Theory]
        [InlineData("DocWord2016")]
        [InlineData("DocWord97")]
        public async Task IsDoc2(string filePath)
        {
            var info = GetFileInfo(DocsPath, filePath, ".doc");

            Assert.True(info.IsWord());

            await AssertIsType(info, MimeTypes.WORD);
        }

        [Theory]
        [InlineData("DocxWord2016")]
        [InlineData("StrictOpenXMLWord2016")]
        public async Task IsDocx(string filePath)
        {
            var info = GetFileInfo(DocsPath, filePath, ".docx");

            Assert.True(info.IsWord());

            await AssertIsType(info, MimeTypes.WORDX);
        }

        [Theory]
        [InlineData("PptPowerpoint2016.ppt")]
        [InlineData("OpenOfficePpt.ppt")]
        [InlineData("PptxPowerpoint2016.pptx")]
        public void IsPointPoint(string filePath)
        {
            var info = GetFileInfo(DocsPath, filePath);

            Assert.True(info.IsPowerPoint());
        }

        [Theory]
        [InlineData("PptPowerpoint2016")]
        public async Task IsPowerPoint(string filePath)
        {
            var info = GetFileInfo(DocsPath, filePath, ".ppt");

            Assert.True(info.IsPowerPoint());

            await AssertIsType(info, MimeTypes.PPT);
        }

        [Theory]
        [InlineData("PptxPowerpoint2016")]
        public async Task IsPowerPointX(string filePath)
        {
            var info = GetFileInfo(DocsPath, filePath, ".pptx");

            Assert.True(info.IsPowerPoint());

            await AssertIsType(info, MimeTypes.PPTX);
        }

        [Theory]
        [InlineData("XlsExcel2016.xls")]
        [InlineData("XlsExcel2007.xls")]
        [InlineData("OpenOfficeExcel.xls")]
        [InlineData("OpenOfficeExcel50.xls")]
        [InlineData("OpenOfficeExcel95.xls")]
        [InlineData("XlsxExcel2016.xlsx")]
        public void IsExcel(string filePath)
        {
            var info = GetFileInfo(DocsPath, filePath);

            Assert.True(info.IsExcel());
        }

        [Theory]
        [InlineData("XlsExcel2016")]
        public async Task IsExcel2(string filePath)
        {
            var info = GetFileInfo(DocsPath, filePath, ".xls");

            Assert.True(info.IsExcel());

            await AssertIsType(info, MimeTypes.EXCEL);
        }

        [Theory]
        [InlineData("XlsxExcel2016")]
        public async Task IsExcelX(string filePath)
        {
            var info = GetFileInfo(DocsPath, filePath, ".xlsx");

            Assert.True(info.IsExcel());

            await AssertIsType(info, MimeTypes.EXCELX);
        }

        [Theory]
        [InlineData("Message.msg")]
        public async Task IsMSG(string fileName)
        {
            var info = GetFileInfo(DocsPath, fileName);
            await AssertIsType(info, MimeTypes.OUTLOOK_MSG);
        }
    }
}
