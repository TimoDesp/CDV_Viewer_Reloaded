using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using CDV_Viewer.CsvBase;
using CDV_Viewer.Data;
using CDV_Viewer.Forms;

namespace CDV_Viewer.Traitements;

public static class ImportModeles
{
	public static void Import()
	{
		OpenFileDialog openFileDialog = new OpenFileDialog
		{
			Filter = "Tous les fichiers Modèles|Modeles*.csv",
			Title = "Importer des Modèles",
			InitialDirectory = Path.Combine(Paths.DataFolder, "Signalo"),
			RestoreDirectory = true
		};
		if (openFileDialog.ShowDialog() != DialogResult.OK)
		{
			return;
		}
		TableModeles tableModeles = new TableModeles(openFileDialog.FileName);
		tableModeles.Load(tableModeles.TempBaseDirectory);
		List<ModeleRow> list = tableModeles.ToList();
		List<ModeleRow> badModeles = list.FindAll((ModeleRow m) => m.SIGModele.Points.Count == 0);
		if (badModeles.Count > 0)
		{
			LoggerForm.ShowDialog(delegate
			{
				foreach (ModeleRow item in badModeles)
				{
					LoggerForm.WriteLine($"Modele du Cdv {Base.GetCircuit(item.CIRCUIT)} corrompu");
				}
			});
		}
		Dictionary<Point, ModeleRow> dictionary = list.ToDictionary((ModeleRow r) => new Point(r.JOINT_E, r.JOINT_S));
		List<ModeleRow> list2 = new List<ModeleRow>(Math.Max(Base.CsvModeles.Count, list.Count));
		int num = 0;
		foreach (ModeleRow csvModele in Base.CsvModeles)
		{
			if (dictionary.TryGetValue(new Point(csvModele.JOINT_E, csvModele.JOINT_S), out var value))
			{
				value.updated = true;
				if (value.STAMP > csvModele.STAMP)
				{
					list2.Add(value);
					num++;
					continue;
				}
			}
			list2.Add(csvModele);
		}
		list = list.FindAll((ModeleRow m) => !m.updated);
		int count = list.Count;
		list2.AddRange(list);
		if (num == 0 && count == 0)
		{
			MessageBox.Show("La base des modèles était à jour");
			return;
		}
		Base.CsvModeles.ReplaceModeles(list2);
		MessageBox.Show($"{count} Nouveau modèles ajoutés\n{num} Modèles mis à jour");
		Base.Link();
	}
}
