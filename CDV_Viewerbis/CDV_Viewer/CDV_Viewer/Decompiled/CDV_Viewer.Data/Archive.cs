using System;
using System.Globalization;
using System.IO;
using System.IO.Compression;
using System.Text;
using CDV_Viewer.CsvBase;
using CDV_Viewer.DockControls;
using CDV_Viewer.Traitements;

namespace CDV_Viewer.Data;

public class Archive : IComparable<Archive>
{
	public string FilePath;

	public DateTime Date;

	public string Description;

	public bool IsBinary { get; private set; }

	public string ShortName => Path.GetFileNameWithoutExtension(FilePath);

	private Archive()
	{
	}

	public Archive(string path, DateTime date, string description)
	{
		FilePath = path;
		string text = Path.GetExtension(FilePath).ToLower();
		IsBinary = text == ".bdb";
		if (!IsBinary && text != ".zip")
		{
			FilePath = Path.Combine(FilePath, "CDV_VIEWER_" + date.ToString("yyyyMMdd_HHmmss") + ".zip");
		}
		Date = date;
		Description = description;
	}

	public static Archive Create(string zipfilePath)
	{
		zipfilePath = zipfilePath.ToLower();
		if (Path.GetExtension(zipfilePath) != ".zip")
		{
			return null;
		}
		string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(zipfilePath);
		if (!fileNameWithoutExtension.StartsWith("cdv_viewer_"))
		{
			return null;
		}
		string s = fileNameWithoutExtension.Substring(11);
		if (!DateTime.TryParseExact(s, "yyyyMMdd_HHmmss", null, DateTimeStyles.None, out var result) && !DateTime.TryParseExact(s, "ddMMyyyy_HHmmss", null, DateTimeStyles.None, out result) && !DateTime.TryParseExact(s, "yyyyMMdd", null, DateTimeStyles.None, out result) && !DateTime.TryParseExact(s, "ddMMyyyy", null, DateTimeStyles.None, out result))
		{
			return null;
		}
		return new Archive
		{
			FilePath = zipfilePath.ToUpper(),
			Date = result,
			Description = GetDescription(zipfilePath)
		};
	}

	public static void CreateZipFromStream(string entryName, Stream sourcestream, string destinationArchiveFileName, CompressionLevel? compressionLevel)
	{
		using ZipArchive zipArchive = ZipFile.Open(destinationArchiveFileName, ZipArchiveMode.Create, null);
		ZipArchiveEntry obj = (compressionLevel.HasValue ? zipArchive.CreateEntry(entryName, compressionLevel.Value) : zipArchive.CreateEntry(entryName));
		DateTime now = DateTime.Now;
		obj.LastWriteTime = now;
		using Stream destination = obj.Open();
		sourcestream.CopyTo(destination);
	}

	public static string GetDescription(string path)
	{
		string result = "";
		using (FileStream fileStream = new FileStream(path, FileMode.Open))
		{
			byte[] array = new byte[256];
			fileStream.Seek(-array.Length, SeekOrigin.End);
			fileStream.Read(array, 0, 256);
			uint num = 0u;
			for (int num2 = array.Length - 18; num2 >= 0; num2--)
			{
				num |= (uint)(array[num2] << 24);
				if (num == 1347093766)
				{
					num2 += 20;
					num = (uint)(array[num2] | (array[num2 + 1] << 8));
					if (num == 0 || num2 + 2 + num > array.Length)
					{
						break;
					}
					return Encoding.ASCII.GetString(array, num2 + 2, (int)num);
				}
				num >>= 8;
			}
		}
		return result;
	}

	public void ToTempFolder()
	{
		if (!Directory.Exists(Paths.TempDataFolder))
		{
			Archives.CreateTempFolder();
		}
		Archives.ClearTempFolder();
		using ZipArchive source = ZipFile.OpenRead(FilePath);
		source.ExtractToDirectory(Paths.TempDataFolder);
	}

	public void Close()
	{
		Archives.ClearTempFolder();
		Archives.CurrentArchive = null;
	}

	public bool Load()
	{
		if (IsBinary)
		{
			return LoadBinary();
		}
		ToTempFolder();
		return LoadFromTempFolder();
	}

	public bool LoadBinary()
	{
		bool num = Base.LoadFromBinary();
		if (num)
		{
			Archives.CurrentArchive = this;
		}
		if (Dialogs.BaseLinkError)
		{
			ComposantsViewer.Viewer.SetLigne(Dialogs.FirstErrorVoie.Ligne.ID);
			ComposantsViewer.Viewer.LightVoie(Dialogs.FirstErrorVoie, Dialogs.FirstErrorPk);
			return num;
		}
		int num2 = Global.ListeLignes?.CurrentLigne ?? (-1);
		if (num2 > 0)
		{
			ComposantsViewer.Viewer.SetLigne(num2);
		}
		return num;
	}

	public bool LoadFromTempFolder()
	{
		bool num = Base.Load();
		if (num)
		{
			Archives.CurrentArchive = this;
		}
		if (Dialogs.BaseLinkError)
		{
			ComposantsViewer.Viewer.SetLigne(Dialogs.FirstErrorVoie.Ligne.ID);
			ComposantsViewer.Viewer.LightVoie(Dialogs.FirstErrorVoie, Dialogs.FirstErrorPk);
			return num;
		}
		int num2 = Global.ListeLignes?.CurrentLigne ?? (-1);
		if (num2 >= 0)
		{
			ComposantsViewer.Viewer.SetLigne(num2);
		}
		return num;
	}

	public void Save()
	{
		if (File.Exists(FilePath))
		{
			File.Delete(FilePath);
		}
		File.Copy(Paths.CSchema, Paths.Schema, overwrite: true);
		ZipFile.CreateFromDirectory(Paths.TempDataFolder, FilePath, CompressionLevel.Optimal, includeBaseDirectory: false);
		if (!(Description == ""))
		{
			using (BinaryWriter binaryWriter = new BinaryWriter(File.OpenWrite(FilePath)))
			{
				binaryWriter.BaseStream.Seek(-2L, SeekOrigin.End);
				byte[] bytes = Encoding.ASCII.GetBytes(Description);
				binaryWriter.Write((ushort)bytes.Length);
				binaryWriter.Write(bytes);
			}
			Archives.CurrentArchive = this;
		}
	}

	public void Export(string filePath)
	{
		if (File.Exists(filePath))
		{
			File.Delete(filePath);
		}
		File.Copy(Paths.CSchema, Paths.Schema, overwrite: true);
		ZipFile.CreateFromDirectory(Paths.TempDataFolder, filePath, CompressionLevel.Optimal, includeBaseDirectory: false);
	}

	public int CompareTo(Archive other)
	{
		return Date.CompareTo(other.Date);
	}
}
