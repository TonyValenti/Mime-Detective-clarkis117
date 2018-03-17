using System;
using MimeDetective.Analyzers;
using System.Collections.Generic;

namespace MimeDetective
{
    /// <summary>
    /// This static class controls/holds all analyzers used by all extension methods
    /// </summary>
    public static class MimeAnalyzers
    {
        private static IFileAnalyzer primaryAnalyzer = new DictionaryBasedTrie(MimeTypes.Types);

        /// <summary>
        ///
        /// </summary>
        public static IFileAnalyzer PrimaryAnalyzer
        {
            get
            {
                return primaryAnalyzer;
            }

            set
            {
                if (value is null)
                    throw new ArgumentNullException(nameof(value));

                primaryAnalyzer = value;
            }
        }

        //secondary headers should go here
        //special handling cases go here
        public static Dictionary<FileType, IReadOnlyFileAnalyzer> SecondaryAnalyzers { get; } = new Dictionary<FileType, IReadOnlyFileAnalyzer>();

        static MimeAnalyzers()
        {
            SecondaryAnalyzers.Add(MimeTypes.ZIP, new ZipFileAnalyzer());
            SecondaryAnalyzers.Add(MimeTypes.MS_OFFICE, new MsOfficeAnalyzer());
        }

        internal static FileType GetFileType(in ReadResult readResult)
        {
            FileType match = null;

            match = PrimaryAnalyzer.Search(in readResult);

            if ((object)match != null && SecondaryAnalyzers.TryGetValue(match, out var secondaryAnalyzer))
            {
                match = secondaryAnalyzer.Search(in readResult);
            }

            return match;
        }
    }
}