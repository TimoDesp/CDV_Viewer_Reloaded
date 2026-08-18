using System;
using System.Collections.Generic;
using System.IO;

namespace CDV_Viewer.Data;

public class Parcours
{
	public List<EtapeParcours> Etapes = new List<EtapeParcours>();

	public string XmlPath { get; private set; }

	public string NomTournee { get; private set; }

	public bool IsOpen { get; private set; }

	public bool SensPK
	{
		get
		{
			if (Etapes.Count > 0)
			{
				return Etapes[0].PkD < Etapes[0].PkF;
			}
			return false;
		}
	}

	public event EventHandler ParcoursChanged;

	public Parcours()
	{
		Archives.CurrentArchiveChanged += Archives_CurrentArchiveChanged;
	}

	public bool Load(string xmlFile, string tourneeName)
	{
		try
		{
			string path = Path.GetDirectoryName(Path.GetDirectoryName(xmlFile)) + "\\data_out";
			string text = string.Empty;
			string[] directories = Directory.GetDirectories(path);
			foreach (string text2 in directories)
			{
				string fileName = Path.GetFileName(text2);
				if (fileName.Substring(fileName.IndexOf("-") + 1) == tourneeName)
				{
					text = text2;
					break;
				}
			}
			if (text != string.Empty)
			{
				Load(text);
			}
			else
			{
				NomTournee = string.Empty;
				IsOpen = false;
			}
		}
		catch
		{
			NomTournee = string.Empty;
			IsOpen = false;
		}
		return IsOpen;
	}

	public bool Load(string tourneeFolder)
	{
		try
		{
			string fileName = Path.GetFileName(tourneeFolder);
			fileName = fileName.Substring(fileName.IndexOf("-") + 1);
			Etapes.Clear();
			string path = string.Empty;
			string[] files = Directory.GetFiles(tourneeFolder);
			foreach (string text in files)
			{
				if (Path.GetExtension(text).ToLower() == ".csv")
				{
					path = text;
				}
			}
			string[] array = File.ReadAllLines(path);
			if (array.Length > 2)
			{
				for (int j = 2; j < array.Length; j++)
				{
					string[] array2 = array[j].Split(';');
					if (array2[0] == "[P]")
					{
						int ligne = int.Parse(array2[1]);
						string voie = array2[2];
						int deltaXD = int.Parse(array2[6]);
						int pkD = int.Parse(array2[7]);
						int deltaXF = int.Parse(array2[10]);
						int pkF = int.Parse(array2[11]);
						Etapes.Add(new EtapeParcours(ligne, voie, deltaXD, pkD, deltaXF, pkF));
					}
				}
			}
			if (Etapes.Count > 0)
			{
				NomTournee = fileName;
				IsOpen = true;
			}
			else
			{
				NomTournee = string.Empty;
				IsOpen = false;
			}
		}
		catch
		{
			NomTournee = string.Empty;
			IsOpen = false;
		}
		if (this.ParcoursChanged != null)
		{
			this.ParcoursChanged(this, new EventArgs());
		}
		return IsOpen;
	}

	public bool GetLigneVoiePK(int deltaX, out int ligne, out int voie, out int pk)
	{
		ligne = 0;
		voie = 0;
		pk = 0;
		foreach (EtapeParcours etape in Etapes)
		{
			if (deltaX >= etape.DeltaXD && deltaX <= etape.DeltaXF)
			{
				ligne = etape.Ligne;
				voie = etape.IdVoie;
				pk = (int)((double)etape.PkD + (double)(deltaX - etape.DeltaXD) * (double)(etape.PkF - etape.PkD) / (double)(etape.DeltaXF - etape.DeltaXD));
				return true;
			}
		}
		return false;
	}

	public int GetDelta(int ligne, int pk)
	{
		foreach (EtapeParcours etape in Etapes)
		{
			int num = Math.Min(etape.PkD, etape.PkF);
			int num2 = Math.Max(etape.PkD, etape.PkF);
			if (ligne == etape.Ligne && pk >= num && pk <= num2)
			{
				return (int)((double)etape.DeltaXD + (double)Math.Abs(pk - etape.PkD) * (double)(etape.DeltaXF - etape.DeltaXD) / (double)Math.Abs(etape.PkF - etape.PkD));
			}
		}
		return int.MinValue;
	}

	public void Close()
	{
		Etapes.Clear();
		NomTournee = string.Empty;
		IsOpen = false;
		if (this.ParcoursChanged != null)
		{
			this.ParcoursChanged(this, new EventArgs());
		}
	}

	private void Archives_CurrentArchiveChanged(object sender, EventArgs e)
	{
		if (Archives.CurrentArchive == null)
		{
			Close();
		}
	}
}
