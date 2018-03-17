using MimeDetective.Analyzers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace MimeDetective.Tests.Analyzers
{
    public class ArrayBasedTrieTests
    {
        [Fact]
        public void DefaultConstructor()
        {
            var analyzer = new ArrayBasedTrie();

            //assertion here just to have
            Assert.NotNull(analyzer);

            analyzer.Insert(MimeTypes.ZIP);
        }

        [Fact]
        public void EnumerableConstructor()
        {
            var analyzer = new ArrayBasedTrie(MimeTypes.Types);

            //assertion here just to have
            Assert.NotNull(analyzer);
            Assert.Throws<ArgumentNullException>(() => new ArrayBasedTrie(null));

            analyzer.Insert(MimeTypes.WORD);
        }

        [Fact]
        public void Insert()
        {
            var analyzer = new ArrayBasedTrie();
            Assert.Throws<ArgumentNullException>(() => analyzer.Insert(null));

            foreach (var fileType in MimeTypes.Types)
            {
                analyzer.Insert(fileType);
            }

            analyzer.Insert(MimeTypes.WORD);
        }

        [Theory]
        [InlineData("./Data/Documents/XlsExcel2016.xls", "xls")]
        [InlineData("./Data/Documents/PptPowerpoint2016.ppt", "ppt")]
        [InlineData("./Data/Documents/DocWord2016.doc", "doc")]
        [InlineData("./Data/Documents/PdfWord2016.pdf", "pdf")]
        [InlineData("./Data/Zip/empty.zip", "zip")]
        [InlineData("./Data/Zip/images.zip", "zip")]
        [InlineData("./Data/Zip/imagesBy7zip.zip", "zip")]
        [InlineData("./Data/images/test.gif", "gif")]
        [InlineData("./Data/Audio/wavVLC.wav", "wav")]
        public async Task Search(string path, string ext)
        {
            var analyzer = new ArrayBasedTrie(MimeTypes.Types);
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
