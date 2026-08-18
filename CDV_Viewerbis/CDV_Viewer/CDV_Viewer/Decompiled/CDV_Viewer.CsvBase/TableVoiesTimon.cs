using System.IO;

namespace CDV_Viewer.CsvBase;

public class TableVoiesTimon : BaseTable<VoieTimonRow>
{
	public override string Path => "topo\\voies_timon.csv";

	public override string Header => "ID;TIMON";

	public override VoieTimonRow RowFromCsv(string[] csvFields)
	{
		return VoieTimonRow.FromCsv(csvFields);
	}

	public override VoieTimonRow RowFromBinary(BinaryReader reader)
	{
		return VoieTimonRow.FromBinary(reader);
	}

	public void SetTimonId(int idvoie, int idTimon)
	{
		if (!TryGetValue(idvoie, out var row))
		{
			row = new VoieTimonRow(idvoie)
			{
				TIMON = idTimon
			};
			Add(row);
		}
		else
		{
			row.TIMON = idTimon;
		}
	}

	public int GetTimonId(int idvoie)
	{
		if (!TryGetValue(idvoie, out var row))
		{
			return -1;
		}
		return row.TIMON;
	}
}
