﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace MimeDetective
{
	/// <summary>
	/// Helper class to identify file type by the file header, not file extension.
	/// </summary>
	public static class MimeTypes
	{
		// all the file types to be put into one list
		public static FileType[] types;

		static MimeTypes()
		{
			types = new FileType[] {PDF, WORD, EXCEL, JPEG, ZIP, RAR, RTF, PNG, PPT, GIF, DLL_EXE, MSDOC,
				BMP, DLL_EXE, ZIP_7z, ZIP_7z_2, GZ_TGZ, TAR_ZH, TAR_ZV, OGG, ICO, XML, MIDI, FLV, WAVE, DWG, LIB_COFF, PST, PSD,
				AES, SKR, SKR_2, PKR, EML_FROM, ELF, TXT_UTF8, TXT_UTF16_BE, TXT_UTF16_LE, TXT_UTF32_BE, TXT_UTF32_LE, TIFF, TiffBigEndian, TiffLittleEndian };
		}

		#region Constants

		// file headers are taken from here:
		//http://www.garykessler.net/library/file_sigs.html
		//mime types are taken from here:
		//http://www.webmaster-toolkit.com/mime-types.shtml

		#region office, excel, ppt and documents, xml, pdf, rtf, msdoc
		// office and documents
		public readonly static FileType WORD = new FileType(new byte?[] { 0xEC, 0xA5, 0xC1, 0x00 }, 512, "doc", "application/msword");
		public readonly static FileType EXCEL = new FileType(new byte?[] { 0x09, 0x08, 0x10, 0x00, 0x00, 0x06, 0x05, 0x00 }, 512, "xls", "application/excel");
		public readonly static FileType PPT = new FileType(new byte?[] { 0xFD, 0xFF, 0xFF, 0xFF, null, 0x00, 0x00, 0x00 }, 512, "ppt", "application/mspowerpoint");

		//ms office and openoffice docs (they're zip files: rename and enjoy!)
		//don't add them to the list, as they will be 'subtypes' of the ZIP type
		public readonly static FileType WORDX = new FileType(new byte?[0], 512, "docx", "application/vnd.openxmlformats-officedocument.wordprocessingml.document");
		public readonly static FileType EXCELX = new FileType(new byte?[0], 512, "xlsx", "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
		public readonly static FileType ODT = new FileType(new byte?[0], 512, "odt", "application/vnd.oasis.opendocument.text");
		public readonly static FileType ODS = new FileType(new byte?[0], 512, "ods", "application/vnd.oasis.opendocument.spreadsheet");

		// common documents
		public readonly static FileType RTF = new FileType(new byte?[] { 0x7B, 0x5C, 0x72, 0x74, 0x66, 0x31 }, "rtf", "application/rtf");
		public readonly static FileType PDF = new FileType(new byte?[] { 0x25, 0x50, 0x44, 0x46 }, "pdf", "application/pdf");
		public readonly static FileType MSDOC = new FileType(new byte?[] { 0xD0, 0xCF, 0x11, 0xE0, 0xA1, 0xB1, 0x1A, 0xE1 }, "", "application/octet-stream");
		//application/xml text/xml
		public readonly static FileType XML = new FileType(new byte?[] { 0x72, 0x73, 0x69, 0x6F, 0x6E, 0x3D, 0x22, 0x31, 0x2E, 0x30, 0x22, 0x3F, 0x3E },
															"xml,xul", "text/xml");

		//text files
		public readonly static FileType TXT = new FileType(new byte?[0], "txt", "text/plain");
		public readonly static FileType TXT_UTF8 = new FileType(new byte?[] { 0xEF, 0xBB, 0xBF }, "txt", "text/plain");
		public readonly static FileType TXT_UTF16_BE = new FileType(new byte?[] { 0xFE, 0xFF }, "txt", "text/plain");
		public readonly static FileType TXT_UTF16_LE = new FileType(new byte?[] { 0xFF, 0xFE }, "txt", "text/plain");
		public readonly static FileType TXT_UTF32_BE = new FileType(new byte?[] { 0x00, 0x00, 0xFE, 0xFF }, "txt", "text/plain");
		public readonly static FileType TXT_UTF32_LE = new FileType(new byte?[] { 0xFF, 0xFE, 0x00, 0x00 }, "txt", "text/plain");

		#endregion

		// graphics
		#region Graphics jpeg, png, gif, bmp, ico

		public readonly static FileType JPEG = new FileType(new byte?[] { 0xFF, 0xD8, 0xFF }, "jpg", "image/jpeg");
		public readonly static FileType PNG = new FileType(new byte?[] { 0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A }, "png", "image/png");
		public readonly static FileType GIF = new FileType(new byte?[] { 0x47, 0x49, 0x46, 0x38, null, 0x61 }, "gif", "image/gif");
		public readonly static FileType BMP = new FileType(new byte?[] { 0x42, 0x4D }, "bmp", "image/bmp"); // or image/x-windows-bmp
		public readonly static FileType ICO = new FileType(new byte?[] { 0, 0, 1, 0 }, "ico", "image/x-icon");
		//tiff
		//todo review support for tiffs, values for files need verified
		public readonly static FileType TIFF = new FileType(new byte?[] { 0x49, 0x44, 0x33 }, "tiff", "image/tiff");
		public readonly static FileType TiffLittleEndian = new FileType(new byte?[] { 0x49, 0x49, 0x2A, 0, 0x10, 0, 0, 0, 0x43, 0x52 }, "tiff", "image/tiff");
		public readonly static FileType TiffBigEndian = new FileType(new byte?[] { 0x4D, 0x4D, 0x4D, 0x44, 0, 0 }, "tiff", "image/tiff");

		#endregion

		#region Zip, 7zip, rar, dll_exe, tar, bz2, gz_tgz

		public readonly static FileType GZ_TGZ = new FileType(new byte?[] { 0x1F, 0x8B, 0x08 }, "gz, tgz", "application/x-gz");

		public readonly static FileType ZIP_7z = new FileType(new byte?[] { 66, 77 }, "7z", "application/x-compressed");
		public readonly static FileType ZIP_7z_2 = new FileType(new byte?[] { 0x37, 0x7A, 0xBC, 0xAF, 0x27, 0x1C }, "7z", "application/x-compressed");

		public readonly static FileType ZIP = new FileType(new byte?[] { 0x50, 0x4B, 0x03, 0x04 }, "zip", "application/x-compressed");
		public readonly static FileType RAR = new FileType(new byte?[] { 0x52, 0x61, 0x72, 0x21 }, "rar", "application/x-compressed");
		public readonly static FileType DLL_EXE = new FileType(new byte?[] { 0x4D, 0x5A }, "dll, exe", "application/octet-stream");

		//Compressed tape archive file using standard (Lempel-Ziv-Welch) compression
		public readonly static FileType TAR_ZV = new FileType(new byte?[] { 0x1F, 0x9D }, "tar.z", "application/x-tar");

		//Compressed tape archive file using LZH (Lempel-Ziv-Huffman) compression
		public readonly static FileType TAR_ZH = new FileType(new byte?[] { 0x1F, 0xA0 }, "tar.z", "application/x-tar");

		//bzip2 compressed archive
		public readonly static FileType BZ2 = new FileType(new byte?[] { 0x42, 0x5A, 0x68 }, "bz2,tar,bz2,tbz2,tb2", "application/x-bzip2");


		#endregion


		#region Media ogg, midi, flv, dwg, pst, psd

		// media 
		public readonly static FileType OGG = new FileType(new byte?[] { 103, 103, 83, 0, 2, 0, 0, 0, 0, 0, 0, 0, 0 }, "oga,ogg,ogv,ogx", "application/ogg");
		//MID, MIDI	 	Musical Instrument Digital Interface (MIDI) sound file
		public readonly static FileType MIDI = new FileType(new byte?[] { 0x4D, 0x54, 0x68, 0x64 }, "midi,mid", "audio/midi");

		//FLV	 	Flash video file
		public readonly static FileType FLV = new FileType(new byte?[] { 0x46, 0x4C, 0x56, 0x01 }, "flv", "application/unknown");

		//WAV	 	Resource Interchange File Format -- Audio for Windows file, where xx xx xx xx is the file size (little endian), audio/wav audio/x-wav

		public readonly static FileType WAVE = new FileType(new byte?[] { 0x52, 0x49, 0x46, 0x46, null, null, null, null, 
															0x57, 0x41, 0x56, 0x45, 0x66, 0x6D, 0x74, 0x20	}, "wav", "audio/wav");

		public readonly static FileType PST = new FileType(new byte?[] { 0x21, 0x42, 0x44, 0x4E }, "pst", "application/octet-stream");

		//eneric AutoCAD drawing image/vnd.dwg  image/x-dwg application/acad
		public readonly static FileType DWG = new FileType(new byte?[] { 0x41, 0x43, 0x31, 0x30 }, "dwg", "application/acad");

		//Photoshop image file
		public readonly static FileType PSD = new FileType(new byte?[] { 0x38, 0x42, 0x50, 0x53 }, "psd", "application/octet-stream");

		#endregion

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


		#endregion

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

		// number of bytes we read from a file
		public const int MaxHeaderSize = 560;  // some file formats have headers offset to 512 bytes
	   
		#endregion

		#region Main Methods

		public static void SaveToXmlFile(string path)
		{
			using (FileStream file = File.OpenWrite(path))
			{
				var serializer = new System.Xml.Serialization.XmlSerializer(types.GetType());
				serializer.Serialize(file, types);
			}
		}

		public static void LoadFromXmlFile(string path)
		{
			using (FileStream file = File.OpenRead(path))
			{
				var serializer = new System.Xml.Serialization.XmlSerializer(types.GetType());
				FileType[] tmpTypes = (FileType[])serializer.Deserialize(file);

				int typeOrgLenth = types.Length;

				Array.Resize(ref types, types.Length + tmpTypes.Length);

				Array.Copy(tmpTypes, 0, types, typeOrgLenth, tmpTypes.Length);
			}
		}

		/*
		public static FileType GetFileType(FileInfo file)
		{
			return MimeTypes.GetFileType(() => MimeTypes.ReadFileHeader(file, MimeTypes.MaxHeaderSize), file);
		}
		*/

		/// <summary>
		/// Read header of a file and depending on the information in the header
		/// return object FileType.
		/// Return null in case when the file type is not identified. 
		/// Throws Application exception if the file can not be read or does not exist
		/// </summary>
		/// <param name="fileHeaderReadFunc">A function which returns the bytes found</param>
		/// <param name="fileFullName">If given and file typ is a zip file, a check for docx and xlsx is done</param>
		/// <returns>FileType or null not identified</returns>
		public static FileType GetFileType(Func<byte[]> fileHeaderReadFunc, Stream stream = null, byte[] data = null)
		{
			return getFileType( fileHeaderReadFunc(), stream);
		}

		public static async Task<FileType> GetFileTypeAsync(Func<Task<byte[]>> fileHeaderReadFunc, Stream stream = null, byte[] data = null)
		{
			return getFileType(await fileHeaderReadFunc(), stream);
		}

		private static FileType getFileType(byte[] fileHeader, Stream stream = null, byte[] data = null)
		{
			// if none of the types match, return null
			FileType fileType = null;

			// read first n-bytes from the file
			//byte[] fileHeader = fileHeaderReadFunc();

			// checking if it's binary (not really exact, but should do the job)
			// shouldn't work with UTF-16 OR UTF-32 files
			if (!fileHeader.Any(b => b == 0))
			{
				fileType = TXT;
			}
			else
			{
				// compare the file header to the stored file headers
				foreach (FileType type in types)
				{
					int matchingCount = GetFileMatchingCount(fileHeader, type);

					if (matchingCount == type.Header.Length)
					{
						// check for docx and xlsx only if a file name is given
						// there may be situations where the file name is not given
						// or it is unpractical to write a temp file to get the FileInfo
						if (type.Equals(ZIP))
						{
							if (stream != null)
							{
								fileType = CheckForDocxAndXlsxStream(type, stream);
							}
							else
							{
								var memstream = new MemoryStream(data);

								fileType = CheckForDocxAndXlsxStream(type, memstream);
							}
						}
						else
						{
							fileType = type;    // if all the bytes match, return the type
						}

						break;
					}
				}
			}
			return fileType;
		}

		/// <summary>
		/// Gets the list of FileTypes based on list of extensions in Comma-Separated-Values string
		/// </summary>
		/// <param name="CSV">The CSV String with extensions</param>
		/// <returns>List of FileTypes</returns>
		public static List<FileType> GetFileTypesByExtensions(String CSV)
		{
			String[] extensions = CSV.ToUpper().Replace(" ", "").Split(',');

			List<FileType> result = new List<FileType>();

			foreach (FileType type in types)
			{
				if (extensions.Contains(type.Extension.ToUpper()))
				{
					result.Add(type);
				}
			}
			return result;
		}

		private static FileType CheckForDocxAndXlsxStream(FileType type, Stream zipData)
		{
			FileType result = null;

			//check for docx and xlsx
			using (var zipFile = new ZipArchive(zipData))
			{
				if (zipFile.Entries.Any(e => e.FullName.StartsWith("word/")))
					result = WORDX;
				else if (zipFile.Entries.Any(e => e.FullName.StartsWith("xl/")))
					result = EXCELX;
				else
					result = CheckForOdtAndOds(result, zipFile);
			}
			return result;
		}

		/*
		private static FileType CheckForDocxAndXlsx(FileType type, FileInfo fileInfo)
		{
			FileType result = null;

			//check for docx and xlsx
			using (var zipFile = ZipFile.OpenRead(fileInfo.FullName))
			{
				if (zipFile.Entries.Any(e => e.FullName.StartsWith("word/")))
					result = WORDX;
				else if (zipFile.Entries.Any(e => e.FullName.StartsWith("xl/")))
					result = EXCELX;
				else
					result = CheckForOdtAndOds(result, zipFile);
			}
			return result;
		}
		*/

		private static FileType CheckForOdtAndOds(FileType result, ZipArchive zipFile)
		{
			var ooMimeType = zipFile.Entries.FirstOrDefault(e => e.FullName == "mimetype");
			if (ooMimeType != null)
			{
				using (var textReader = new StreamReader(ooMimeType.Open()))
				{
					var mimeType = textReader.ReadToEnd();

					textReader.Dispose();

					if (mimeType == ODT.Mime)
						result = ODT;
					else if (mimeType == ODS.Mime)
						result = ODS;
				}
			}

			return result;
		}

		private static int GetFileMatchingCount(byte[] fileHeader, FileType type)
		{
			int matchingCount = 0;

			for (int i = 0; i < type.Header.Length; i++)
			{
				// if file offset is not set to zero, we need to take this into account when comparing.
				// if byte in type.header is set to null, means this byte is variable, ignore it
				if (type.Header[i] != null && type.Header[i] != fileHeader[i + type.HeaderOffset])
				{
					// if one of the bytes does not match, move on to the next type
					matchingCount = 0;
					break;
				}
				else
				{
					matchingCount++;
				}
			}

			return matchingCount;
		}

#endregion

#region Byte Header Get Methods

		/// <summary>
		/// Reads the file header - first (16) bytes from the file
		/// </summary>
		/// <param name="file">The file to work with</param>
		/// <returns>Array of bytes</returns>
		internal static Byte[] ReadFileHeader(FileInfo file, int MaxHeaderSize)
		{
			byte[] header = new byte[MaxHeaderSize];
			try  // read file
			{
				using (FileStream fsSource = new FileStream(file.FullName, FileMode.Open, FileAccess.Read))
				{
					// read first symbols from file into array of bytes.
					fsSource.Read(header, 0, MaxHeaderSize);
				}   // close the file stream

			}
			catch (Exception e) // file could not be found/read
			{
				throw new Exception("Could not read file : " + e.Message);
			}

			return header;
		}

		internal static async Task<byte[]> ReadFileHeaderAsync(FileInfo file, int MaxHeaderSize)
		{
			byte[] header = new byte[MaxHeaderSize];

			try  // read file
			{
				using (FileStream fsSource = new FileStream(file.FullName, FileMode.Open, FileAccess.Read))
				{
					// read first symbols from file into array of bytes.
					await fsSource.ReadAsync(header, 0, MaxHeaderSize);
				}   // close the file stream

			}
			catch (Exception e) // file could not be found/read
			{
				throw new System.IO.FileLoadException("Could not read file : " + e.Message, file.FullName, e);
			}

			return header;
		}

		/// <summary>
		/// Takes a stream does, not dispose of stream, resets read position to beginning though
		/// </summary>
		/// <param name="stream"></param>
		/// <param name="MaxHeaderSize"></param>
		/// <returns></returns>
		internal static byte[] ReadHeaderFromStream(Stream stream, int MaxHeaderSize)
		{
			byte[] header = new byte[MaxHeaderSize];

			try  // read stream
			{
				if (!stream.CanRead)
				{
					throw new System.IO.IOException("Could not read from Stream");
				}

				if (stream.Position > 0)
				{
					stream.Seek(0, SeekOrigin.Begin);
				}

				stream.Read(header, 0, MaxHeaderSize);

			}
			catch (Exception e) // file could not be found/read
			{
				throw new Exception("Could not read Stream : " + e.Message);
			}

			return header;
		}

		/// <summary>
		/// Takes a stream does, not dispose of stream, resets read position to beginning though
		/// </summary>
		/// <param name="stream"></param>
		/// <param name="MaxHeaderSize"></param>
		/// <returns></returns>
		internal static async Task<byte[]> ReadHeaderFromStreamAsync(Stream stream, int MaxHeaderSize)
		{
			byte[] header = new byte[MaxHeaderSize];

			try  // read stream
			{
				if (!stream.CanRead)
				{
					throw new System.IO.IOException("Could not read from Stream");
				}

				if (stream.Position > 0)
				{
					stream.Seek(0, SeekOrigin.Begin);
				}

				await stream.ReadAsync(header, 0, MaxHeaderSize);

			}
			catch (Exception e) // file could not be found/read
			{
				throw new Exception("Could not read Stream : " + e.Message);
			}

			return header;
		}

		internal static byte[] ReadHeaderFromByteArray(byte[] byteArray, int MaxHeaderSize)
		{
			if (byteArray.Length < MaxHeaderSize)
			{
				throw new ArgumentException("Is smaller than" + nameof(MaxHeaderSize), nameof(byteArray));
			}

			byte[] header = new byte[MaxHeaderSize];

			Array.Copy(byteArray, header, MaxHeaderSize);

			return header;
		}

#endregion
	}
}
