using System;
using System.Collections.Generic;
using System.Text;

namespace MimeDetective.Analyzers
{
    public class LinearCountingAnalyzer : IFileAnalyzer
    {
        private readonly List<FileType> types;

        /// <summary>
        /// Constructs an empty LinearCountingAnalyzer, use <see cref="Insert(FileType)"/> to add file types
        /// </summary>
        public LinearCountingAnalyzer()
        {
            types = new List<FileType>();
        }

        /// <summary>
        /// Constructs a LinearCountingAnalyzer using the supplied IEnumerable<FileType>
        /// </summary>
        /// <param name="fileTypes"></param>
        public LinearCountingAnalyzer(IEnumerable<FileType> fileTypes)
        {
            if (fileTypes is null)
                throw new ArgumentNullException(nameof(fileTypes));

            types = new List<FileType>();

            foreach (var fileType in fileTypes)
            {
                if ((object)fileType != null)
                    Insert(fileType);
            }
        }

        public void Insert(FileType fileType)
        {
            if (fileType is null)
                throw new ArgumentNullException(nameof(fileType));

            types.Add(fileType);
        }

        public FileType Search(in ReadResult readResult)
        {
            if (readResult.ReadLength == 0)
                return null;

            uint highestMatchingCount = 0;
            FileType highestMatchingType = null;

            // compare the file header to the stored file headers
            for (int typeIndex = 0; typeIndex < types.Count; typeIndex++)
            {
                FileType type = types[typeIndex];

                uint matchingCount = 0;
                int iOffset = type.HeaderOffset;
                int readLength = iOffset + type.Header.Length;

                if (readLength > readResult.ReadLength)
                    continue;

                for (int i = 0; iOffset < readLength; i++, iOffset++)
                {
                    if (type.Header[i] is null || type.Header[i].Value == readResult.Array[iOffset])
                        matchingCount++;
                }

                if (type.Header.Length == matchingCount && matchingCount > highestMatchingCount)
                {
                    highestMatchingType = type;
                    highestMatchingCount = matchingCount;
                }
            }

            return highestMatchingType;
        }
    }
}
