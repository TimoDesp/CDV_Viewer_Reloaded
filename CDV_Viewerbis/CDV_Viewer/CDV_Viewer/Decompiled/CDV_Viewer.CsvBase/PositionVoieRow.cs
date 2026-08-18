using System;
using System.IO;
using CDV_Viewer.Data.SIGObjects;

namespace CDV_Viewer.CsvBase;

public class PositionVoieRow : BaseRow
{
	public static PositionVoieRow EmptyRow = new PositionVoieRow();

	public int VOIE;

	public int POSITION;

	private PositionVoieRow()
	{
	}

	internal PositionVoieRow(int id)
	{
		_id = id;
	}

	internal PositionVoieRow(SIGVoie voie)
	{
		_id = (VOIE = voie.ID);
		POSITION = voie.PositionY;
	}

	public static PositionVoieRow FromCsv(string[] fields)
	{
		int num = Convert.ToInt32(fields[1]);
		return new PositionVoieRow
		{
			_id = num,
			VOIE = num,
			POSITION = Convert.ToInt32(fields[2])
		};
	}

	public static PositionVoieRow FromBinary(BinaryReader reader)
	{
		int num = BaseRow.ReadId(reader);
		return new PositionVoieRow
		{
			_id = num,
			VOIE = num,
			POSITION = reader.ReadSByte()
		};
	}

	public override string ToCsv()
	{
		return base.ID.ToString() + ";" + VOIE + ";" + POSITION;
	}

	public override void ToBinaryStream(BinaryWriter writer)
	{
		BaseRow.WriteId(writer, VOIE);
		writer.Write((sbyte)POSITION);
	}
}
