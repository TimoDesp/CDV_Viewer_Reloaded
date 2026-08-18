using System.IO;
using CDV_Viewer.Data.SIGObjects;

namespace CDV_Viewer.CsvBase;

public class TablePositionsVoies : BaseTable<PositionVoieRow>
{
	public override string Path => "topo\\pos_voies.csv";

	public override string Header => "ID;VOIE;POSITION";

	public override PositionVoieRow RowFromCsv(string[] csvFields)
	{
		return PositionVoieRow.FromCsv(csvFields);
	}

	public override PositionVoieRow RowFromBinary(BinaryReader reader)
	{
		return PositionVoieRow.FromBinary(reader);
	}

	public override void Link()
	{
		foreach (PositionVoieRow value in _items.Values)
		{
			SIGVoie sIGVoie = Base.CsvVoies[value.VOIE].SIGVoie;
			if (sIGVoie != null)
			{
				sIGVoie.PositionY = value.POSITION;
			}
		}
	}

	public void SetPosition(SIGVoie voie)
	{
		if (!TryGetValue(voie.ID, out var row))
		{
			row = new PositionVoieRow(voie);
			Add(row);
		}
		else
		{
			row.POSITION = voie.PositionY;
		}
	}

	public int GetPosition(SIGVoie voie)
	{
		int num = int.MinValue;
		if (TryGetValue(voie.ID, out var row))
		{
			num = row.POSITION;
		}
		voie.PositionY = num;
		return num;
	}
}
