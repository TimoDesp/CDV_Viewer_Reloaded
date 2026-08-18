using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Windows.Forms;
using CDV_Viewer.CsvBase;
using CDV_Viewer.Data;
using CDV_Viewer.Data.SIGObjects;
using CDV_Viewer.Forms;
using CDV_Viewer.Properties;

namespace CDV_Viewer.Traitements;

public static class ImportMGV
{
	private delegate void DelegateEmpty();

	private delegate string DialogDelegate(FolderBrowserDialog dialog);

	private static string _inPath;

	public static void Import()
	{
		FolderBrowserDialog folderBrowserDialog = new FolderBrowserDialog();
		folderBrowserDialog.Description = "Veuillez sélectionner le dossier contenant le réferentiel MGV_REF";
		if (folderBrowserDialog.ShowDialog() == DialogResult.OK)
		{
			_inPath = folderBrowserDialog.SelectedPath;
			if (!File.Exists(_inPath + Paths.ExportMGV_TLignes))
			{
				MessageBox.Show("Le fichier " + Paths.ExportMGV_TLignes + " est manquant !", Resources.APP_NAME);
				return;
			}
			if (!File.Exists(_inPath + Paths.ExportMGV_TVoies))
			{
				MessageBox.Show("Le fichier " + Paths.ExportMGV_TVoies + " est manquant !", Resources.APP_NAME);
				return;
			}
			if (!File.Exists(_inPath + Paths.ExportMGV_TBranches))
			{
				MessageBox.Show("Le fichier " + Paths.ExportMGV_TBranches + " est manquant !", Resources.APP_NAME);
				return;
			}
			if (!File.Exists(_inPath + Paths.ExportMGV_TCircuits))
			{
				MessageBox.Show("Le fichier " + Paths.ExportMGV_TCircuits + " est manquant !", Resources.APP_NAME);
				return;
			}
			if (!File.Exists(_inPath + Paths.ExportMGV_TModeles))
			{
				MessageBox.Show("Le fichier " + Paths.ExportMGV_TModeles + " est manquant !", Resources.APP_NAME);
				return;
			}
			if (!File.Exists(_inPath + Paths.ExportMGV_TJoints))
			{
				MessageBox.Show("Le fichier " + Paths.ExportMGV_TJoints + " est manquant !", Resources.APP_NAME);
				return;
			}
			if (!File.Exists(_inPath + Paths.ExportMGV_TNoeuds))
			{
				MessageBox.Show("Le fichier " + Paths.ExportMGV_TNoeuds + " est manquant !", Resources.APP_NAME);
				return;
			}
			if (!File.Exists(_inPath + Paths.ExportMGV_TSegments))
			{
				MessageBox.Show("Le fichier " + Paths.ExportMGV_TSegments + " est manquant !", Resources.APP_NAME);
				return;
			}
			Archives.ClearTempFolder();
			Directory.CreateDirectory(Path.GetDirectoryName(Paths.TLignes));
			Directory.CreateDirectory(Path.GetDirectoryName(Paths.TCircuits));
			Global.MainForm.Enabled = false;
			LoadingForm loadingForm = new LoadingForm();
			loadingForm.Show();
			new Thread(ImportThread).Start(loadingForm);
		}
	}

	private static void ImportThread(object param)
	{
		LoadingForm loadingForm = (LoadingForm)param;
		File.Copy(Paths.CSchema, Paths.Schema);
		loadingForm.Maximum = 10;
		loadingForm.Avancement = 0;
		loadingForm.Texte = "Import des éléments géographiques...";
		ImportLignes();
		loadingForm.Avancement = 1;
		ImportVoies();
		loadingForm.Avancement = 2;
		ImportBranches();
		loadingForm.Texte = "Import des éléments de signalisation...";
		loadingForm.Avancement = 3;
		ImportJoints();
		loadingForm.Avancement = 4;
		ImportCircuits();
		loadingForm.Avancement = 5;
		ImportJointsCircuits();
		loadingForm.Avancement = 6;
		ImportModeles();
		loadingForm.Avancement = 7;
		loadingForm.Texte = "Chargement de la base...";
		if (!Base.Load())
		{
			Global.MainForm.Invoke(new DelegateEmpty(ErreurImport));
			return;
		}
		loadingForm.Avancement = 8;
		loadingForm.Texte = "Calcul de la position des voies...";
		SetPositionsVoies();
		if (MessageBox.Show("Voulez-vous importer les balises (CRO et LGV) présentes dans le LOC_NG ?", Resources.APP_NAME, MessageBoxButtons.YesNo) == DialogResult.Yes)
		{
			loadingForm.Avancement = 9;
			loadingForm.Texte = "Import des balises du LOC_NG...";
			string text = Global.MainForm.Invoke(() => SelectLocNgDataFolder());
			if (text != string.Empty)
			{
				ImportBalises(text);
			}
		}
		loadingForm.Avancement = 10;
		loadingForm.End = true;
		Global.MainForm.Invoke(new DelegateEmpty(EndImport));
	}

	public static void SetPositionsVoies()
	{
		foreach (SIGLigne ligne in Base.GetLignes())
		{
			List<SIGVoie> list = ligne.VoiesWithoutJonctions.FindAll((SIGVoie voie) => voie.PositionY == int.MinValue);
			Dictionary<string, int> dictionary = new Dictionary<string, int>();
			Dictionary<string, List<SIGVoie>> dictionary2 = ligne.VoieByName();
			if (dictionary2.ContainsKey("U"))
			{
				dictionary.Add("U", 0);
			}
			if (dictionary2.ContainsKey("VU"))
			{
				dictionary.Add("VU", 0);
			}
			if (dictionary2.ContainsKey("UNIQUE"))
			{
				dictionary.Add("UNIQUE", 0);
			}
			if (dictionary2.ContainsKey("V1"))
			{
				dictionary.Add("V1", -1);
			}
			if (dictionary2.ContainsKey("V2"))
			{
				dictionary.Add("V2", 1);
			}
			if (dictionary2.ContainsKey("VA"))
			{
				dictionary.Add("VA", -1);
			}
			if (dictionary2.ContainsKey("VB"))
			{
				dictionary.Add("VB", 1);
			}
			foreach (SIGVoie item in list)
			{
				if (dictionary.ContainsKey(item.Nom))
				{
					continue;
				}
				if (Chaines.GetFirstNombre(item.Nom, out var _) % 2 == 1)
				{
					for (int num = -1; num >= -30; num--)
					{
						bool flag = true;
						foreach (KeyValuePair<string, int> _key in dictionary)
						{
							if (_key.Value != num)
							{
								continue;
							}
							foreach (SIGVoie item2 in list.FindAll((SIGVoie voie) => voie.Nom == _key.Key))
							{
								if (item.Intersect(item2))
								{
									flag = false;
								}
							}
						}
						if (flag)
						{
							dictionary.Add(item.Nom, num);
							break;
						}
					}
					continue;
				}
				for (int num2 = 1; num2 <= 30; num2++)
				{
					bool flag2 = true;
					foreach (KeyValuePair<string, int> _key2 in dictionary)
					{
						if (_key2.Value != num2)
						{
							continue;
						}
						foreach (SIGVoie item3 in list.FindAll((SIGVoie voie) => voie.Nom == _key2.Key))
						{
							if (item.Intersect(item3))
							{
								flag2 = false;
							}
						}
					}
					if (flag2)
					{
						dictionary.Add(item.Nom, num2);
						break;
					}
				}
			}
			foreach (KeyValuePair<string, int> _key3 in dictionary)
			{
				foreach (SIGVoie item4 in list.FindAll((SIGVoie voie) => voie.Nom == _key3.Key))
				{
					item4.PositionY = _key3.Value;
					Base.SetPositionVoie(item4);
				}
			}
		}
	}

	private static string SelectLocNgDataFolder()
	{
		FolderBrowserDialog folderBrowserDialog = new FolderBrowserDialog
		{
			Description = "Veuillez séléctionner le dossier LIGTXT présent dans le repertoire du LocNG"
		};
		if (Directory.Exists(Paths.LocLignesFolder))
		{
			folderBrowserDialog.SelectedPath = Paths.LocLignesFolder;
		}
		if (folderBrowserDialog.ShowDialog() == DialogResult.OK)
		{
			return folderBrowserDialog.SelectedPath;
		}
		return string.Empty;
	}

	private static void EndImport()
	{
		SaveDialog saveDialog = new SaveDialog();
		if (saveDialog.ShowDialog() == DialogResult.OK)
		{
			Base.SaveToTempFolder();
			Archives.CreateArchiveFromTempFolder(saveDialog.Description);
		}
		else
		{
			Archives.ClearTempFolder();
		}
		MessageBox.Show("Import terminé !", Resources.APP_NAME);
		Global.MainForm.Enabled = true;
	}

	private static void ErreurImport()
	{
		Archives.ClearTempFolder();
		MessageBox.Show("Erreur lors de l'import !");
		Global.MainForm.Enabled = true;
	}

	private static void ImportLignes()
	{
		StreamReader streamReader = new StreamReader(_inPath + Paths.ExportMGV_TLignes);
		string value = streamReader.ReadToEnd();
		streamReader.Close();
		streamReader.Dispose();
		StreamWriter streamWriter = new StreamWriter(Paths.TLignes);
		streamWriter.WriteLine("ID;NOM");
		streamWriter.WriteLine(value);
		streamWriter.Close();
		streamWriter.Dispose();
	}

	private static void ImportVoies()
	{
		List<string> list = new List<string>();
		List<string> list2 = new List<string>();
		StreamReader streamReader = new StreamReader(_inPath + Paths.ExportMGV_TVoies);
		streamReader.ReadLine();
		while (streamReader.Peek() >= 0)
		{
			string[] array = streamReader.ReadLine().Split(';');
			list.Add(array[0] + ";" + array[2] + ";" + array[3] + ";" + array[4] + ";" + array[5]);
			list2.Add(array[0] + ";" + array[1]);
		}
		streamReader.Close();
		streamReader.Dispose();
		StreamWriter streamWriter = new StreamWriter(Paths.TVoies);
		streamWriter.WriteLine("ID;LIGNE;NOM;PKD;PKF");
		foreach (string item in list)
		{
			streamWriter.WriteLine(item);
		}
		streamWriter.Close();
		streamWriter.Dispose();
		streamWriter = new StreamWriter(Paths.TVoiesTimon);
		streamWriter.WriteLine("ID;TIMON");
		foreach (string item2 in list2)
		{
			streamWriter.WriteLine(item2);
		}
		streamWriter.Close();
		streamWriter.Dispose();
		streamWriter = new StreamWriter(Paths.TPosVoies);
		streamWriter.WriteLine("ID;VOIE;POSITION");
		streamWriter.Close();
		streamWriter.Dispose();
	}

	private static void ImportBranches()
	{
		List<string> list = new List<string>();
		StreamReader streamReader = new StreamReader(_inPath + Paths.ExportMGV_TNoeuds);
		streamReader.ReadLine();
		int num = 0;
		int num2 = 0;
		StreamReader streamReader2 = new StreamReader(_inPath + Paths.ExportMGV_TBranches);
		while (streamReader.Peek() >= 0)
		{
			string[] array = streamReader.ReadLine().Split(';');
			streamReader2.BaseStream.Seek(0L, SeekOrigin.Begin);
			streamReader2.DiscardBufferedData();
			streamReader2.ReadLine();
			int num3 = int.Parse(array[1]) * 100 + int.Parse(array[2]) * 10;
			while (streamReader2.Peek() >= 0)
			{
				string[] array2 = streamReader2.ReadLine().Split(';');
				int num4 = int.Parse(array2[0]);
				if (num4 >= num3 && num4 < num3 + 10)
				{
					list.Add(num2 + ";" + num + ";" + array2[1] + ";" + array2[2] + ";" + array2[4]);
					num2++;
				}
			}
			num++;
		}
		streamReader.Close();
		streamReader.Dispose();
		StreamReader streamReader3 = new StreamReader(_inPath + Paths.ExportMGV_TVoies);
		streamReader3.ReadLine();
		while (streamReader3.Peek() >= 0)
		{
			string[] array3 = streamReader3.ReadLine().Split(';');
			streamReader2.BaseStream.Seek(0L, SeekOrigin.Begin);
			streamReader2.DiscardBufferedData();
			streamReader2.ReadLine();
			bool flag = false;
			bool flag2 = false;
			while (streamReader2.Peek() >= 0)
			{
				string[] array4 = streamReader2.ReadLine().Split(';');
				if (array4[1] == array3[0])
				{
					if (int.Parse(array4[2]) <= int.Parse(array3[4]))
					{
						flag = true;
					}
					if (int.Parse(array4[2]) >= int.Parse(array3[5]))
					{
						flag2 = true;
					}
				}
			}
			if (!flag)
			{
				list.Add(num2 + ";" + num + ";" + array3[0] + ";" + array3[4] + ";Aval");
				num2++;
				num++;
			}
			if (!flag2)
			{
				list.Add(num2 + ";" + num + ";" + array3[0] + ";" + array3[5] + ";Amont");
				num2++;
				num++;
			}
		}
		streamReader2.Close();
		streamReader2.Dispose();
		streamReader3.Close();
		streamReader3.Dispose();
		StreamWriter streamWriter = new StreamWriter(Paths.TBranches);
		streamWriter.WriteLine("ID;NOEUD;VOIE;PK;TYPE");
		foreach (string item in list)
		{
			streamWriter.WriteLine(item);
		}
		streamWriter.Close();
		streamWriter.Dispose();
	}

	private static void ImportJoints()
	{
		List<string> list = new List<string>();
		StreamReader streamReader = new StreamReader(_inPath + Paths.ExportMGV_TJoints);
		streamReader.ReadLine();
		while (streamReader.Peek() >= 0)
		{
			string[] array = streamReader.ReadLine().Split(';');
			list.Add(array[0] + ";" + array[4] + ";" + array[5] + ";" + array[3] + ";" + array[6] + ";" + array[7] + ";" + array[8]);
		}
		streamReader.Close();
		streamReader.Dispose();
		StreamWriter streamWriter = new StreamWriter(Paths.TJoints);
		streamWriter.WriteLine("ID;VOIE;PK;TYPE;LONGUEUR;DB_AMONT;DB_AVAL");
		foreach (string item in list)
		{
			streamWriter.WriteLine(item);
		}
		streamWriter.Close();
		streamWriter.Dispose();
	}

	private static void ImportCircuits()
	{
		List<string> list = new List<string>();
		StreamReader streamReader = new StreamReader(_inPath + Paths.ExportMGV_TCircuits);
		streamReader.ReadLine();
		while (streamReader.Peek() >= 0)
		{
			string[] array = streamReader.ReadLine().Split(';');
			list.Add(array[0] + ";" + array[1] + ";" + array[2] + ";" + array[3] + ";" + array[5] + ";" + array[6] + ";" + array[7] + ";" + array[8] + ";" + array[9] + ";" + array[10] + ";" + array[11] + ";" + array[12] + ";" + array[13]);
		}
		streamReader.Close();
		streamReader.Dispose();
		StreamWriter streamWriter = new StreamWriter(Paths.TCircuits);
		streamWriter.WriteLine("ID;NOM;TYPE;FREQUENCE;COMPENSATION;POINTS;PAS_TH;PAS_REEL;ICC_MIN;I_FUITE_MAX;DIAPHONIE_MAX;EMETTEUR_DEBUT;EMETTEUR_FIN");
		foreach (string item in list)
		{
			streamWriter.WriteLine(item);
		}
		streamWriter.Close();
		streamWriter.Dispose();
	}

	private static void ImportJointsCircuits()
	{
		List<string> list = new List<string>();
		StreamReader streamReader = new StreamReader(_inPath + Paths.ExportMGV_TCircuits);
		streamReader.ReadLine();
		int num = 0;
		StreamReader streamReader2 = new StreamReader(_inPath + Paths.ExportMGV_TSegments);
		while (streamReader.Peek() >= 0)
		{
			int num2 = int.Parse(streamReader.ReadLine().Split(';')[0]);
			List<int[]> list2 = new List<int[]>();
			streamReader2.BaseStream.Seek(0L, SeekOrigin.Begin);
			streamReader2.DiscardBufferedData();
			streamReader2.ReadLine();
			while (streamReader2.Peek() >= 0)
			{
				string[] array = streamReader2.ReadLine().Split(';');
				int num3 = int.Parse(array[0]);
				if (num2 == num3)
				{
					int num4 = int.Parse(array[1]);
					int num5 = int.Parse(array[2]);
					int num6 = int.Parse(array[3]);
					if (num5 != 0 || num6 != 0)
					{
						list2.Add(new int[3] { num4, num5, num6 });
					}
				}
			}
			if (list2.Count == 1)
			{
				list.Add(num + ";" + list2[0][1] + ";" + num2 + ";True;");
				num++;
				list.Add(num + ";" + list2[0][2] + ";" + num2 + ";True;");
				num++;
				continue;
			}
			list.Add(num + ";" + list2[0][1] + ";" + num2 + ";True;");
			num++;
			for (int i = 1; i < list2.Count - 1; i++)
			{
				if (list2[i][1] == 0)
				{
					list.Add(num + ";" + list2[i][2] + ";" + num2 + ";False;");
				}
				else
				{
					list.Add(num + ";" + list2[i][1] + ";" + num2 + ";False;");
				}
				num++;
			}
			list.Add(num + ";" + list2[list2.Count - 1][2] + ";" + num2 + ";True;");
			num++;
		}
		streamReader2.Close();
		streamReader2.Dispose();
		streamReader.Close();
		streamReader.Dispose();
		StreamWriter streamWriter = new StreamWriter(Paths.TJointsCircuits);
		streamWriter.WriteLine("ID;JOINT;CIRCUIT;PRINCIPAL");
		foreach (string item in list)
		{
			streamWriter.WriteLine(item);
		}
		streamWriter.Close();
		streamWriter.Dispose();
	}

	private static void ImportModeles()
	{
		List<string> list = new List<string>();
		StreamReader streamReader = new StreamReader(_inPath + Paths.ExportMGV_TModeles);
		int num = 0;
		streamReader.ReadLine();
		while (streamReader.Peek() >= 0)
		{
			string[] array = streamReader.ReadLine().Split(';');
			string text = num + ";" + array[0] + ";" + array[1] + ";" + array[2] + ";" + array[3] + ";";
			for (int i = 4; i < array.Length; i++)
			{
				text = text + array[i] + "-";
			}
			list.Add(text.Substring(0, text.Length - 1));
			num++;
		}
		streamReader.Close();
		streamReader.Dispose();
		StreamWriter streamWriter = new StreamWriter(Paths.TModeles);
		streamWriter.WriteLine("ID;CIRCUIT;JOINT_E;JOINT_S;TOURNEE;POINTS");
		foreach (string item in list)
		{
			streamWriter.WriteLine(item);
		}
		streamWriter.Close();
		streamWriter.Dispose();
	}

	private static void ImportBalises(string path)
	{
		List<SIGBalise> list = new List<SIGBalise>();
		string[] files = Directory.GetFiles(path);
		string[] _cells;
		int _pk;
		foreach (string path2 in files)
		{
			if (Path.GetExtension(path2).ToLower() != ".txt" || !int.TryParse(Path.GetFileNameWithoutExtension(path2).Substring(1), out var result))
			{
				continue;
			}
			SIGLigne ligne = Base.GetLigne(result);
			if (ligne == null)
			{
				continue;
			}
			string[] array = File.ReadAllLines(path2);
			foreach (string text in array)
			{
				_cells = text.Split(';');
				if (_cells.Length != 22)
				{
					continue;
				}
				string text2 = _cells[5].Trim();
				if ((!(text2 != "BLGV") || !(text2 != "CRO")) && int.TryParse(_cells[0], out _pk))
				{
					bool actif = false;
					if (_cells[9] == "C")
					{
						actif = true;
					}
					SIGVoie sIGVoie = ligne.Voies.Find((SIGVoie voie) => voie.Nom == _cells[4].Trim() && voie.PKDebut <= _pk && voie.PKFin >= _pk);
					if (sIGVoie != null)
					{
						list.Add(new SIGBalise(-1)
						{
							Voie = sIGVoie,
							PK = _pk,
							Actif = actif,
							Type = SIGBalise.TypeFromString(text2)
						});
					}
				}
			}
		}
		Base.AddBalises(list);
	}
}
