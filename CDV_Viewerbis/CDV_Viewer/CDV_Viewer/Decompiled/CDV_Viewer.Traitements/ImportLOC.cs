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

public class ImportLOC
{
	private delegate void DelegateEmpty();

	private static string _inPath;

	public static void Import(string _inFolderPath)
	{
		_inPath = _inFolderPath;
		Global.MainForm.Enabled = false;
		LoadingForm loadingForm = new LoadingForm();
		loadingForm.Show();
		new Thread(ImportThread).Start(loadingForm);
	}

	private static void ImportThread(object param)
	{
		LoadingForm loadingForm = (LoadingForm)param;
		string[] files = Directory.GetFiles(_inPath);
		loadingForm.Maximum = files.Length;
		loadingForm.Avancement = 0;
		loadingForm.Texte = "Import des balises...";
		List<SIGBalise> list = new List<SIGBalise>();
		string[] array = files;
		foreach (string path in array)
		{
			if (Path.GetExtension(path).ToLower() != ".txt" || !int.TryParse(Path.GetFileNameWithoutExtension(path).Substring(1), out var result))
			{
				loadingForm.Avancement++;
				continue;
			}
			SIGLigne ligne = Base.GetLigne(result);
			if (ligne == null)
			{
				loadingForm.Avancement++;
				continue;
			}
			string[] array2 = File.ReadAllLines(path);
			foreach (string text in array2)
			{
				string[] _cells = text.Split(';');
				if (_cells.Length != 22)
				{
					continue;
				}
				string text2 = _cells[5].Trim();
				if ((!(text2 != "BLGV") || !(text2 != "CRO")) && int.TryParse(_cells[0], out var _pk))
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
			loadingForm.Avancement++;
		}
		Base.AddBalises(list);
		loadingForm.End = true;
		MessageBox.Show("Import terminé !", Resources.APP_NAME);
		Global.MainForm.Invoke((DelegateEmpty)delegate
		{
			Global.MainForm.Enabled = true;
		});
	}
}
