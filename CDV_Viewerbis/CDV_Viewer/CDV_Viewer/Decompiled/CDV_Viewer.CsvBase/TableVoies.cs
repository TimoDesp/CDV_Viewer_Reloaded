using System.Collections.Generic;
using System.IO;
using System.Linq;
using CDV_Viewer.Data.SIGObjects;

namespace CDV_Viewer.CsvBase;

public class TableVoies : BaseTable<VoieRow>
{
	public override string Path => "topo\\Voies.csv";

	public override string Header => "ID;LIGNE;NOM;PKD;PKF";

	public override VoieRow RowFromCsv(string[] csvFields)
	{
		return VoieRow.FromCsv(csvFields);
	}

	public override VoieRow RowFromBinary(BinaryReader reader)
	{
		return VoieRow.FromBinary(reader);
	}

	public override void Link()
	{
		foreach (VoieRow value in _items.Values)
		{
			SIGVoie sIGVoie = value.SIGVoie;
			sIGVoie.Unlink();
			(sIGVoie.Ligne = Base.CsvLignes[value.LIGNE].SIGLigne).Voies.Add(sIGVoie);
		}
	}

	public SIGVoie Create(SIGLigne ligne, string nom, int pkDebut, int pkFin)
	{
		SIGVoie obj = new SIGVoie(FreeId())
		{
			Ligne = ligne,
			Nom = nom
		};
		VoieRow row = new VoieRow(obj)
		{
			PKD = pkDebut,
			PKF = pkFin
		};
		Add(row);
		return obj;
	}

	internal SIGVoie SigVoies(int idVoie)
	{
		if (!TryGetValue(idVoie, out var row))
		{
			return null;
		}
		return row.SIGVoie;
	}

	internal List<SIGVoie> SigVoies()
	{
		return _items.Values.Select((VoieRow v) => v.SIGVoie).ToList();
	}

	internal void Update(SIGVoie voie)
	{
		if (TryGetValue(voie.ID, out var row))
		{
			row.NOM = voie.Nom;
			row.PKD = voie.PKDebut;
			row.PKF = voie.PKFin;
		}
	}
}
