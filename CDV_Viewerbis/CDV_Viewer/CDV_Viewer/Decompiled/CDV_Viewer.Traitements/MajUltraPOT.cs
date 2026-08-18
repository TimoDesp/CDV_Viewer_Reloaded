using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Windows.Forms;
using CDV_Viewer.CsvBase;
using CDV_Viewer.DockControls;

namespace CDV_Viewer.Traitements;

public static class MajUltraPOT
{
	private delegate void EmptyDelegate();

	private static bool _actif = false;

	private static string _path = string.Empty;

	private static List<string> _lignes = new List<string>();

	private static Thread _thread;

	public static bool Actif
	{
		get
		{
			return _actif;
		}
		set
		{
			if (value)
			{
				SaveFileDialog saveFileDialog = new SaveFileDialog();
				if (Directory.Exists("D:\\Outils_SOL\\REF_MGV\\Data"))
				{
					saveFileDialog.InitialDirectory = "D:\\Outils_SOL\\REF_MGV\\Data";
				}
				saveFileDialog.FileName = "LISTE_CDV.CSV";
				if (saveFileDialog.ShowDialog() == DialogResult.OK)
				{
					_path = saveFileDialog.FileName;
					_actif = true;
					RefreshAll();
				}
			}
			else
			{
				_actif = false;
			}
		}
	}

	public static void RefreshAll()
	{
		if (_actif && (_thread == null || _thread.ThreadState != ThreadState.Running))
		{
			_thread.Start(_path);
			ExportMGV.EndExportUltraPOT += ExportMGV_EndExportUltraPOT;
		}
	}

	private static void ExportMGV_EndExportUltraPOT(object sender, EventArgs e)
	{
		ExportMGV.EndExportUltraPOT -= ExportMGV_EndExportUltraPOT;
		_lignes = new List<string>(File.ReadAllLines(_path));
		ComposantsViewer.Viewer.Invoke(new EmptyDelegate(EndThread));
	}

	public static void RefreshCircuit(int idCircuit)
	{
		RefreshCircuit(new List<int> { idCircuit });
	}

	public static void RefreshCircuit(List<int> idsCircuit)
	{
		if (_actif && (_thread == null || _thread.ThreadState != ThreadState.Running))
		{
			_thread = new Thread(ThreadRefreshCircuit);
			_thread.Start(idsCircuit);
		}
	}

	public static void ThreadRefreshCircuit(object idCircuit)
	{
		List<int> list = (List<int>)idCircuit;
		for (int i = 0; i < _lignes.Count; i++)
		{
			string text = _lignes[i].Split(';')[1];
			bool flag = false;
			for (int j = 0; j < list.Count; j++)
			{
				if (list[j].ToString() == text)
				{
					flag = true;
					break;
				}
			}
			if (flag)
			{
				_lignes.RemoveAt(i);
				i--;
			}
		}
		foreach (int item in list)
		{
			int.Parse(_lignes[_lignes.Count - 1].Split(';')[0]);
			Base.GetCircuit(item);
		}
		File.WriteAllLines(_path, _lignes.ToArray());
		ComposantsViewer.Viewer.Invoke(new EmptyDelegate(EndThread));
	}

	public static void EndThread()
	{
	}
}
