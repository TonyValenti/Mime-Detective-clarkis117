using System;
using System.Buffers;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;

namespace MimeDetective
{
    /// <summary>
    /// Helper class to identify file type by the file header, not file extension.
    /// file headers are taken from here:
    /// http://www.garykessler.net/library/file_sigs.html
    /// mime types are taken from here:
    /// http://www.webmaster-toolkit.com/mime-types.shtml
    /// </summary>
    public static class MimeTypes
    {
        // number of bytes we read from a file
        // some file formats have headers offset to 512 bytes
        public const ushort MaxHeaderSize = 560;

        #region Constants

        #region office, excel, ppt and documents, xml, pdf, rtf, msdoc

        /// <summary>
        /// This is for usage when a file type definition requires content inspection instead of reading headers
        /// </summary>
        public readonly static byte?[] EmptyHeader = new byte?[0];

        // office and documents
        public readonly static FileType WORD = new FileType(new byte?[] { 0xEC, 0xA5, 0xC1, 0x00 }, "doc", "application/msword", 512);

        public readonly static FileType EXCEL = new FileType(new byte?[] { 0x09, 0x08, 0x10, 0x00, 0x00, 0x06, 0x05, 0x00 }, "xls", "application/excel", 512);

        //see source control for old version, def maybe wrong period
        public readonly static FileType PPT = new FileType(new byte?[] { 0xA0, 0x46, 0x1D, 0xF0 }, "ppt", "application/mspowerpoint", 512);

        //ms office and openoffice docs (they're zip files: rename and enjoy!)
        //don't add them to the list, as they will be 'subtypes' of the ZIP type
        //Open Xml Document formats
        public readonly static FileType WORDX = new FileType(EmptyHeader, "docx", "application/vnd.openxmlformats-officedocument.wordprocessingml.document", 512);
        public readonly static FileType PPTX = new FileType(EmptyHeader, "pptx", "application/vnd.openxmlformats-officedocument.presentationml.presentation", 512);
        public readonly static FileType EXCELX = new FileType(EmptyHeader, "xlsx", "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", 512);

        //Open Document formats
        public readonly static FileType ODT = new FileType(EmptyHeader, "odt", "application/vnd.oasis.opendocument.text", 512);
        public readonly static FileType ODS = new FileType(EmptyHeader, "ods", "application/vnd.oasis.opendocument.spreadsheet", 512);
        public readonly static FileType ODP = new FileType(EmptyHeader, "odp", "application/vnd.oasis.opendocument.presentation", 512);
        public readonly static FileType ODG = new FileType(EmptyHeader, "odg", "application/vnd.oasis.opendocument.graphics", 512);


        // common documents
        public readonly static FileType RTF = new FileType(new byte?[] { 0x7B, 0x5C, 0x72, 0x74, 0x66, 0x31 }, "rtf", "application/rtf");

        public readonly static FileType PDF = new FileType(new byte?[] { 0x25, 0x50, 0x44, 0x46 }, "pdf", "application/pdf");

        //todo place holder extension
        public readonly static FileType MS_OFFICE = new FileType(new byte?[] { 0xD0, 0xCF, 0x11, 0xE0, 0xA1, 0xB1, 0x1A, 0xE1 }, "doc,ppt,xls", "application/octet-stream");

        //application/xml text/xml
        //                                                               r     s     i     o     n     =     "     1     .     0     "     ?     >
        public readonly static FileType XML = new FileType(new byte?[] { 0x72, 0x73, 0x69, 0x6F, 0x6E, 0x3D, 0x22, 0x31, 0x2E, 0x30, 0x22, 0x3F, 0x3E }, "xml", "text/xml");

        // XML file encoded with UTF-8                                    <     ?     x     m     l     (spc)
        public readonly static FileType XML_NoBom = new FileType(new byte?[] { 0x3C, 0x3F, 0x78, 0x6D, 0x6C, 0x20, }, "xml", "application/xml");
        // XML file encoded with UTF-8 + Byte order mark                         Byte Order Mark    <     ?     x     m     l     (spc)
        public readonly static FileType XML_Utf8Bom = new FileType(new byte?[] { 0x0EF, 0xBB, 0xBF, 0x3C, 0x3F, 0x78, 0x6D, 0x6C, 0x20, }, "xml", "application/xml");
        // XML file encoded with UCS-2 Big Endian                               BOM FEFF     <           ?           x           m           l           (spc)
        public readonly static FileType XML_UCS2BE = new FileType(new byte?[] { 0x0FF, 0xFE, 0x3C, 0x00, 0x3F, 0x00, 0x78, 0x00, 0x6D, 0x00, 0x6C, 0x00, 0x20, 0x00, }, "xml", "application/xml");
        // XML file encoded with UCS-2 Little Endian                            BOM FFFE     <           ?           x           m           l           (spc)
        public readonly static FileType XML_UCS2LE = new FileType(new byte?[] { 0x0FE, 0xFF, 0x00, 0x3C, 0x00, 0x3F, 0x00, 0x78, 0x00, 0x6D, 0x00, 0x6C, 0x00, 0x20, }, "xml", "application/xml");

        //text files
        public readonly static FileType TXT = new FileType(EmptyHeader, "txt", "text/plain");

        public readonly static FileType TXT_UTF8 = new FileType(new byte?[] { 0xEF, 0xBB, 0xBF }, "txt", "text/plain");
        public readonly static FileType TXT_UTF16_BE = new FileType(new byte?[] { 0xFE, 0xFF }, "txt", "text/plain");
        public readonly static FileType TXT_UTF16_LE = new FileType(new byte?[] { 0xFF, 0xFE }, "txt", "text/plain");
        public readonly static FileType TXT_UTF32_BE = new FileType(new byte?[] { 0x00, 0x00, 0xFE, 0xFF }, "txt", "text/plain");
        public readonly static FileType TXT_UTF32_LE = new FileType(new byte?[] { 0xFF, 0xFE, 0x00, 0x00 }, "txt", "text/plain");

        #endregion office, excel, ppt and documents, xml, pdf, rtf, msdoc

        // graphics

        #region Graphics jpeg, png, gif, bmp, ico, tiff

        public readonly static FileType JPEG = new FileType(new byte?[] { 0xFF, 0xD8, 0xFF }, "jpg", "image/jpeg");
        public readonly static FileType PNG = new FileType(new byte?[] { 0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A }, "png", "image/png");
        public readonly static FileType GIF = new FileType(new byte?[] { 0x47, 0x49, 0x46, 0x38, null, 0x61 }, "gif", "image/gif");
        public readonly static FileType BMP = new FileType(new byte?[] { 0x42, 0x4D }, "bmp", "image/bmp"); // or image/x-windows-bmp

        //TODO review this
        public readonly static FileType ICO = new FileType(new byte?[] { 0, 0, 1, 0 }, "ico", "image/x-icon");

        //tiff
        //todo review support for tiffs, values for files need verified
        public readonly static FileType Tiff = new FileType(new byte?[] { 0x49, 0x20, 0x49 }, "tiff", "image/tiff");

        public readonly static FileType TiffLittleEndian = new FileType(new byte?[] { 0x49, 0x49, 0x2A, 0 }, "tiff", "image/tiff");
        public readonly static FileType TiffBigEndian = new FileType(new byte?[] { 0x4D, 0x4D, 0, 0x2A }, "tiff", "image/tiff");
        public readonly static FileType TiffBig = new FileType(new byte?[] { 0x4D, 0x4D, 0, 0x2B }, "tiff", "image/tiff");

        #endregion Graphics jpeg, png, gif, bmp, ico, tiff

        #region Video

        /// <summary>
        /// Base Magic Word for all complex MPEG4 container formats
        /// ex. ....ftypisom (header starting after ....)
        /// </summary>
        public readonly static FileType MP4Container = new FileType(new byte?[] { 0x66, 0x74, 0x79, 0x70 }, "mp4,m4v,m4a,mp4a,mov", "video/mp4", 4);

        public readonly static FileType Mp4ISOv1 = new FileType(new byte?[] { 0x66, 0x74, 0x79, 0x70, 0x69, 0x73, 0x6F, 0x6D }, "mp4", "video/mp4", 4);

        public readonly static FileType Mp4QuickTime = new FileType(new byte?[] { 0x66, 0x74, 0x79, 0x70, 0x6D, 0x70, 0x34, 0x32 }, "m4v", "video/x-m4v", 4);

        public readonly static FileType MovQuickTime = new FileType(new byte?[] { 0x66, 0x74, 0x79, 0x70, 0x71, 0x74, 0x20, 0x20 }, "mov", "video/quicktime", 4);

        public readonly static FileType MP4VideoFiles = new FileType(new byte?[] { 0x66, 0x74, 0x79, 0x70, 0x33, 0x67, 0x70, 0x35 }, "mp4", "video/mp4", 4);

        public readonly static FileType Mp4VideoFile = new FileType(new byte?[] { 0x66, 0x74, 0x79, 0x70, 0x4D, 0x53, 0x4E, 0x56 }, "mp4", "video/mp4", 4);

        public readonly static FileType Mp4A = new FileType(new byte?[] { 0x66, 0x74, 0x79, 0x70, 0x4D, 0x34, 0x41, 0x20 }, "mp4a,m4a", "audio/mp4", 4);

        //FLV	 	Flash video file
        public readonly static FileType FLV = new FileType(new byte?[] { 0x46, 0x4C, 0x56, 0x01 }, "flv", "application/unknown");

        public readonly static FileType ThreeGPP2File = new FileType(new byte?[] { 0x66, 0x74, 0x79, 0x70, 0x33, 0x67, 0x70 }, "3gp", "video/3gg", 4);

        #endregion Video

        #region Audio

        /// <summary>
        /// MP3 file with ID3 Meta-data
        /// </summary>
        public readonly static FileType Mp3ID3 = new FileType(new byte?[] { 0x49, 0x44, 0x33 }, "mp3", "audio/mpeg");
        //todo this needs analyzer support public readonly static FileType Mp3SyncFrame = new FileType(new byte?[] { 0xFF, 0xF1 }, "mp3", "audio/mpeg");

        //WAV	 	Resource Interchange File Format -- Audio for Windows file, where xx xx xx xx is the file size (little endian), audio/wav audio/x-wav

        public readonly static FileType Wav = new FileType(new byte?[] { 0x52, 0x49, 0x46, 0x46, null, null, null, null,
            0x57, 0x41, 0x56, 0x45, 0x66, 0x6D, 0x74, 0x20 }, "wav", "audio/wav");

        //MID, MIDI	 	Musical Instrument Digital Interface (MIDI) sound file
        public readonly static FileType MIDI = new FileType(new byte?[] { 0x4D, 0x54, 0x68, 0x64 }, "midi,mid", "audio/midi");

        /// <summary>
        /// File type on Kessler's site is wrong it should be "fLaC" only
        /// </summary>
        public readonly static FileType Flac = new FileType(new byte?[] { 0x66, 0x4C, 0x61, 0x43 }, "flac", "audio/x-flac");

        #endregion Audio

        #region Zip, 7zip, rar, dll_exe, tar, bz2, gz_tgz

        public readonly static FileType GZ_TGZ = new FileType(new byte?[] { 0x1F, 0x8B, 0x08 }, "gz,tgz", "application/x-gz");

        //public readonly static FileType ZIP_7z = new FileType(new byte?[] { 66, 77 }, "7z", "application/x-compressed");
        public readonly static FileType ZIP_7z = new FileType(new byte?[] { 0x37, 0x7A, 0xBC, 0xAF, 0x27, 0x1C }, "7z", "application/x-compressed");

        public readonly static FileType ZIP = new FileType(new byte?[] { 0x50, 0x4B, 0x03, 0x04 }, "zip", "application/x-compressed");
        public readonly static FileType ZIP_EMPTY = new FileType(new byte?[] { 0x50, 0x4B, 0x05, 0x06 }, "zip", "application/x-compressed");
        public readonly static FileType RAR = new FileType(new byte?[] { 0x52, 0x61, 0x72, 0x21 }, "rar", "application/x-compressed");
        public readonly static FileType DLL_EXE = new FileType(new byte?[] { 0x4D, 0x5A }, "dll,exe", "application/octet-stream");

        //Compressed tape archive file using standard (Lempel-Ziv-Welch) compression
        public readonly static FileType TAR_ZV = new FileType(new byte?[] { 0x1F, 0x9D }, "tar.z", "application/x-tar");

        //Compressed tape archive file using LZH (Lempel-Ziv-Huffman) compression
        public readonly static FileType TAR_ZH = new FileType(new byte?[] { 0x1F, 0xA0 }, "tar.z", "application/x-tar");

        //bzip2 compressed archive
        public readonly static FileType BZ2 = new FileType(new byte?[] { 0x42, 0x5A, 0x68 }, "bz2,tar,bz2,tbz2,tb2", "application/x-bzip2");

        #endregion Zip, 7zip, rar, dll_exe, tar, bz2, gz_tgz

        #region Media ogg, dwg, pst, psd

        // media
        public readonly static FileType OGG = new FileType(new byte?[] { 0x4F, 0x67, 0x67, 0x53 }, "oga,ogg,ogv,ogx", "application/ogg");

        public readonly static FileType PST = new FileType(new byte?[] { 0x21, 0x42, 0x44, 0x4E }, "pst", "application/octet-stream");

        //eneric AutoCAD drawing image/vnd.dwg  image/x-dwg application/acad
        public readonly static FileType DWG = new FileType(new byte?[] { 0x41, 0x43, 0x31, 0x30 }, "dwg", "application/acad");

        //Photoshop image file
        public readonly static FileType PSD = new FileType(new byte?[] { 0x38, 0x42, 0x50, 0x53 }, "psd", "application/octet-stream");

        #endregion Media ogg, dwg, pst, psd

        public readonly static FileType LIB_COFF = new FileType(new byte?[] { 0x21, 0x3C, 0x61, 0x72, 0x63, 0x68, 0x3E, 0x0A }, "lib", "application/octet-stream");

        #region Crypto aes, skr, skr_2, pkr

        //AES Crypt file format. (The fourth byte is the version number.)
        public readonly static FileType AES = new FileType(new byte?[] { 0x41, 0x45, 0x53 }, "aes", "application/octet-stream");

        //SKR	 	PGP secret keyring file
        public readonly static FileType SKR = new FileType(new byte?[] { 0x95, 0x00 }, "skr", "application/octet-stream");

        //SKR	 	PGP secret keyring file
        public readonly static FileType SKR_2 = new FileType(new byte?[] { 0x95, 0x01 }, "skr", "application/octet-stream");

        //PKR	 	PGP public keyring file
        public readonly static FileType PKR = new FileType(new byte?[] { 0x99, 0x01 }, "pkr", "application/octet-stream");

        #endregion Crypto aes, skr, skr_2, pkr

        /*
         * 46 72 6F 6D 20 20 20 or	 	From
        46 72 6F 6D 20 3F 3F 3F or	 	From ???
        46 72 6F 6D 3A 20	 	From:
        EML	 	A commmon file extension for e-mail files. Signatures shown here
        are for Netscape, Eudora, and a generic signature, respectively.
        EML is also used by Outlook Express and QuickMail.
         */
        public readonly static FileType EML_FROM = new FileType(new byte?[] { 0x46, 0x72, 0x6F, 0x6D }, "eml", "message/rfc822");

        //EVTX	 	Windows Vista event log file
        public readonly static FileType ELF = new FileType(new byte?[] { 0x45, 0x6C, 0x66, 0x46, 0x69, 0x6C, 0x65, 0x00 }, "elf", "text/plain");

        public static readonly FileType[] Types = new FileType[] { PDF, JPEG, ZIP, ZIP_EMPTY, RAR, RTF, PNG, GIF, DLL_EXE, MS_OFFICE,
                BMP, DLL_EXE, ZIP_7z, GZ_TGZ, TAR_ZH, TAR_ZV, OGG, ICO, XML, XML_NoBom, XML_Utf8Bom, XML_UCS2BE, XML_UCS2LE, DWG, LIB_COFF, PST, PSD, BZ2,
                AES, SKR, SKR_2, PKR, EML_FROM, ELF, TXT_UTF8, TXT_UTF16_BE, TXT_UTF16_LE, TXT_UTF32_BE, TXT_UTF32_LE,
                Mp3ID3, Wav, Flac, MIDI,
                Tiff, TiffLittleEndian, TiffBigEndian, TiffBig,
                MP4Container, Mp4ISOv1, MovQuickTime, MP4VideoFiles, Mp4QuickTime, Mp4VideoFile, ThreeGPP2File, Mp4A, FLV };

        //public static readonly FileType[] sortedTypes = Types.OrderBy(x => x.Header.Length).ToArray();


        /// <summary>
        /// OpenDocument And OpenXML Document types
        /// </summary>
        public static readonly FileType[] XmlTypes = new FileType[] { WORDX, EXCELX, PPTX, ODS, ODT, ODG, ODP };

        #endregion Constants

        /*
        public static void SaveToXmlFile(string path, IEnumerable<FileType> types)
        {
            using (FileStream file = File.OpenWrite(path))
            {
                var serializer = new System.Xml.Serialization.XmlSerializer(typeof(IEnumerable<FileType>));
                serializer.Serialize(file, types);
            }
        }

        public static FileType[] LoadFromXmlFile(string path)
        {
            using (FileStream file = File.OpenRead(path))
            {
                var serializer = new System.Xml.Serialization.XmlSerializer(typeof(FileType[]));

                return (FileType[])serializer.Deserialize(file);
            }
        }*/

        /// <summary>
        /// Gets the list of FileTypes based on list of extensions in Comma-Separated-Values string
        /// </summary>
        /// <param name="CSV">The CSV String with extensions</param>
        /// <returns>List of FileTypes</returns>
        public static List<FileType> GetFileTypesByExtensions(string csv)
        {
            List<FileType> result = new List<FileType>();

            foreach (FileType type in Types)
            {
                if (csv.IndexOf(type.Extension, 0, StringComparison.OrdinalIgnoreCase) > 0)
                    result.Add(type);
            }

            return result;
        }  
    }
}