using System;

namespace MimeDetective
{
    public static class ByteArrayExtensions
    {

        public readonly static byte?[] EmptyHeader = new byte?[0];
        public readonly static FileType TXT = new FileType(EmptyHeader, "txt", "text/plain");

        /// <summary>
        /// Read header of bytes and depending on the information in the header
        /// return object FileType.
        /// Return null in case when the file type is not identified.
        /// Throws Application exception if the file can not be read or does not exist
        /// </summary>
        /// <param name="file">The FileInfo object.</param>
        /// <returns>FileType or null not identified</returns>
        public static FileType GetFileType(this byte[] bytes)
        {
            int min = bytes.Length > MimeTypes.MaxHeaderSize ? MimeTypes.MaxHeaderSize : bytes.Length;
            using (ReadResult readResult = new ReadResult(bytes, min))
            {

                var mimeType = MimeAnalyzers.GetFileType(in readResult);

                if (mimeType == null)
                    return TXT;
                return mimeType;
            }
        }
    }
}
