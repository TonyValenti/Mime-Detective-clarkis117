using System.IO;

namespace MimeDetective
{
    /// <summary>
    /// A set of extension methods for use with document formats.
    /// </summary>
    public static partial class FileInfoExtensions
    {
        /// <summary>
        /// Determines whether the specified file is RTF document.
        /// </summary>
        /// <param name="fileInfo">The FileInfo.</param>
        /// <returns>
        ///   <c>true</c> if the specified file is RTF; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsRtf(this FileInfo fileInfo) => fileInfo.IsType(MimeTypes.RTF);

        /// <summary>
        /// Determines whether the specified file is PDF.
        /// </summary>
        /// <param name="file">The file.</param>
        /// <returns>
        ///   <c>true</c> if the specified file is PDF; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsPdf(this FileInfo file) => file.IsType(MimeTypes.PDF);


        /// <summary>
        /// Determines whether the specified file info is ms-word document file
        /// This includes .doc and .docx files
        /// </summary>
        /// <param name="fileInfo">The file info.</param>
        /// <returns>
        ///   <c>true</c> if the specified file info is doc; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsWord(this FileInfo fileInfo)
        {
            var fileType = fileInfo.GetFileType();

            return (fileType == MimeTypes.WORD) || (fileType == MimeTypes.WORDX) || (fileType == MimeTypes.MS_OFFICE);
        }

        /// <summary>
        /// Determines whether the specified file info is ms-word document file
        /// This includes .ppt and .pptx files
        /// </summary>
        /// <param name="fileInfo">The file info.</param>
        /// <returns>
        ///   <c>true</c> if the specified file info is doc; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsPowerPoint(this FileInfo fileInfo)
        {
            var fileType = fileInfo.GetFileType();

            return (fileType == MimeTypes.PPT) || (fileType == MimeTypes.PPTX) || (fileType == MimeTypes.MS_OFFICE);
        }

        /// <summary>
        /// Determines whether the specified file info is ms-word document file
        /// this includes old xls and xlsx files
        /// </summary>
        /// <param name="fileInfo">The file info.</param>
        /// <returns>
        ///   <c>true</c> if the specified file info is doc; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsExcel(this FileInfo fileInfo)
        {
            var fileType = fileInfo.GetFileType();

            return (fileType == MimeTypes.EXCEL) || (fileType == MimeTypes.EXCELX) || (fileType == MimeTypes.MS_OFFICE);
        }

        public static bool IsOutlookMSG(this FileInfo file) => file.IsType(MimeTypes.OUTLOOK_MSG);
    }
}