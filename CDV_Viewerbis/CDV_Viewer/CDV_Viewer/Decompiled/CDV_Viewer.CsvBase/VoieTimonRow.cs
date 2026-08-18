using System;
using System.IO;

namespace CDV_Viewer.CsvBase;

public class VoieTimonRow : BaseRow
{
	public int TIMON;

	private VoieTimonRow()
	{
	}

	internal VoieTimonRow(int id)
	{
		_id = id;
	}

	public static VoieTimonRow FromCsv(string[] fields)
	{
		return new VoieTimonRow(BaseRow.GetId(fields))
		{
			TIMON = Convert.ToInt32(fields[1])
		};
	}

	public static VoieTimonRow FromBinary(BinaryReader reader)
	{
		return new VoieTimonRow(BaseRow.ReadId(reader))
		{
			TIMON = BaseRow.ReadId(reader)
		};
	}

	public override string ToCsv()
	{
		return base.ID.ToString() + ";" + TIMON;
	}

	public override void ToBinaryStream(BinaryWriter writer)
	{
		BaseRow.WriteId(writer, base.ID);
		BaseRow.WriteId(writer, TIMON);
	}
}
