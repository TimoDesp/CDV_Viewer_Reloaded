using System;
using System.IO;
using System.Xml.Serialization;
using CDV_Viewer.DockControls;

namespace CDV_Viewer.Data;

public class Preferences
{
	[Serializable]
	public class cAffichage
	{
		public bool InfoBulles = true;

		public bool Legende = true;

		public ModeVisualisation ModeVisualisation = ModeVisualisation.Signalisation;

		public static cAffichage Load()
		{
			XmlSerializer xmlSerializer = new XmlSerializer(typeof(cAffichage));
			FileStream fileStream = File.OpenRead(Paths.PAffichage);
			cAffichage result;
			try
			{
				result = (cAffichage)xmlSerializer.Deserialize(fileStream);
			}
			catch
			{
				fileStream.Close();
				fileStream.Dispose();
				throw new Exception();
			}
			fileStream.Close();
			fileStream.Dispose();
			return result;
		}

		public void Save()
		{
			ModeVisualisation = ComposantsViewer.Viewer.ModeVisualisation;
			XmlSerializer xmlSerializer = new XmlSerializer(typeof(cAffichage));
			FileStream fileStream = File.Create(Paths.PAffichage);
			xmlSerializer.Serialize(fileStream, this);
			fileStream.Close();
			fileStream.Dispose();
		}
	}

	private static cAffichage _affichage;

	public static cAffichage Affichage
	{
		get
		{
			if (_affichage == null)
			{
				Load();
			}
			return _affichage;
		}
	}

	public static void Load()
	{
		try
		{
			if (!Directory.Exists(Paths.PDirectory))
			{
				Directory.CreateDirectory(Paths.PDirectory);
			}
		}
		catch
		{
		}
		try
		{
			_affichage = cAffichage.Load();
		}
		catch
		{
			_affichage = new cAffichage();
			try
			{
				_affichage.Save();
			}
			catch
			{
			}
		}
	}

	public static void Save()
	{
		try
		{
			_affichage.Save();
		}
		catch
		{
		}
	}
}
