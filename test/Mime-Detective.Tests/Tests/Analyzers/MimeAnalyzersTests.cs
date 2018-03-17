using MimeDetective.Analyzers;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace MimeDetective.Tests.Analyzers
{
    public class MimeAnalyzersTests
    {
        [Fact]
        public void DefaultPrimaryAnalyzerNotNullOrEmpty()
        {
            Assert.NotNull(MimeAnalyzers.PrimaryAnalyzer);
            Assert.IsType<DictionaryBasedTrie>(MimeAnalyzers.PrimaryAnalyzer);
        }

        [Fact]
        public void PrimaryAnalyzerPropertyThrowsWhenAssignedNull()
        {
            MimeAnalyzers.PrimaryAnalyzer = MimeAnalyzers.PrimaryAnalyzer;
            Assert.Throws<ArgumentNullException>(() => MimeAnalyzers.PrimaryAnalyzer = null);
        }

        [Fact]
        public void DefaultSecondaryAnalyzerNotNullOrEmpty()
        {
            Assert.NotNull(MimeAnalyzers.SecondaryAnalyzers);
            Assert.NotEmpty(MimeAnalyzers.SecondaryAnalyzers);
            Assert.IsType<ZipFileAnalyzer>(MimeAnalyzers.SecondaryAnalyzers[MimeTypes.ZIP]);
            Assert.IsType<MsOfficeAnalyzer>(MimeAnalyzers.SecondaryAnalyzers[MimeTypes.MS_OFFICE]);
        }
    }
}
