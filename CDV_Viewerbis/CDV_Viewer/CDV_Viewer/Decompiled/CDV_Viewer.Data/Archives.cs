using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace CDV_Viewer.Data;

public static class Archives
{
	private static Archive _currentArchive;

	public static Archive CurrentArchive
	{
		get
		{
			return _currentArchive;
		}
		set
		{
			_currentArchive = value;
			Archives.CurrentArchiveChanged?.Invoke(null, new EventArgs());
		}
	}

	public static bool IsOpen => _currentArchive != null;

	public static event EventHandler CurrentArchiveChanged;

	public static List<Archive> GetHistorique()
	{
		List<Archive> list = new List<Archive>();
		if (!Directory.Exists(Paths.DataFolder))
		{
			return list;
		}
		string[] files = Directory.GetFiles(Paths.DataFolder);
		for (int i = 0; i < files.Length; i++)
		{
			Archive item;
			if ((item = Archive.Create(files[i])) != null)
			{
				list.Add(item);
			}
		}
		list.Sort();
		return list;
	}

	public static Archive GetLastArchive()
	{
		if (!Autorisations.Values.Edition)
		{
			if (File.Exists("REF_MGV.BDB"))
			{
				return new Archive("REF_MGV.BDB", DateTime.Now, "BINARY");
			}
			if (File.Exists(Path.Combine(Paths.DataFolder, "REF_MGV.BDB")))
			{
				return new Archive(Path.Combine(Paths.DataFolder, "REF_MGV.BDB"), DateTime.Now, "BINARY");
			}
		}
		List<Archive> historique = GetHistorique();
		if (historique.Count <= 0)
		{
			return null;
		}
		return historique.Last();
	}

	public static Archive GetArchive(DateTime date)
	{
		if (!Directory.Exists(Paths.DataFolder))
		{
			return null;
		}
		string[] files = Directory.GetFiles(Paths.DataFolder);
		for (int i = 0; i < files.Length; i++)
		{
			Archive archive;
			if ((archive = Archive.Create(files[i])) != null && archive.Date == date)
			{
				return archive;
			}
		}
		return null;
	}

	public static void CreateArchiveFromTempFolder(string description)
	{
		Archive archive = new Archive(Paths.DataFolder, DateTime.Now, description);
		archive.Save();
		CurrentArchive = archive;
	}

	public static void CreateTempFolder()
	{
		DirectoryInfo directoryInfo = Directory.CreateDirectory(Paths.TempDataFolder);
		if (Directory.Exists(Paths.TempDataFolder))
		{
			try
			{
				directoryInfo.Attributes = FileAttributes.Hidden | FileAttributes.Directory;
			}
			catch
			{
			}
		}
	}

	public static void ClearTempFolder()
	{
		Directory.Delete(Paths.TempDataFolder, recursive: true);
		CreateTempFolder();
	}
}
