using System;
using System.IO;
using CDV_Viewer.Data.SIGObjects;

namespace CDV_Viewer.CsvBase;

public class TableBalises : BaseTable<BaliseRow>
{
	public override string Path => "topo\\balises.csv";

	public override string Header => "ID;VOIE;PK;ACTIF;TYPE";

	public override void Link()
	{
		foreach (BaliseRow value in _items.Values)
		{
			SIGVoie sIGVoie = Base.CsvVoies[value.VOIE].SIGVoie;
			SIGBalise sIGBalise = value.SIGBalise;
			sIGBalise.Voie = sIGVoie;
			sIGVoie.Balises.Add(sIGBalise);
		}
	}

	public override BaliseRow RowFromCsv(string[] csvFields)
	{
		return BaliseRow.FromCsv(csvFields);
	}

	public override BaliseRow RowFromBinary(BinaryReader reader)
	{
		return BaliseRow.FromBinary(reader);
	}

	internal void Update(SIGBalise newbalise)
	{
		BaliseRow baliseRow = _items[newbalise.ID];
		if (baliseRow.SIGBalise == newbalise)
		{
			if (baliseRow.Update())
			{
				Base.NeedLink();
			}
			return;
		}
		throw new NotImplementedException();
	}

	public int Create(SIGBalise balise)
	{
		int result = (balise.ID = FreeId());
		BaliseRow row = new BaliseRow(balise);
		Add(row);
		return result;
	}

	public int Create(SIGVoie voie, BaliseType type, int pk, bool active)
	{
		SIGBalise balise = new SIGBalise
		{
			Voie = voie,
			PK = pk,
			Type = type,
			Actif = active
		};
		return Create(balise);
	}
}
