using System;
using System.IO;
using CDV_Viewer.Data.SIGObjects;

namespace CDV_Viewer.CsvBase;

public class BaliseRow : BaseRow
{
	public int VOIE;

	public int PK;

	public bool ACTIF;

	public BaliseType TYPE;

	public SIGBalise SIGBalise { get; private set; }

	private BaliseRow()
	{
	}

	internal BaliseRow(int id)
	{
		_id = id;
	}

	internal BaliseRow(SIGBalise balise)
	{
		_id = balise.ID;
		SIGBalise = balise;
		VOIE = balise.Voie.ID;
		Update();
	}

	public static BaliseRow FromCsv(string[] fields)
	{
		int id = BaseRow.GetId(fields);
		if (!Enum.TryParse<BaliseType>(fields[4], out var result))
		{
			result = BaliseType.INC;
		}
		BaliseRow baliseRow = new BaliseRow(id)
		{
			VOIE = Convert.ToInt32(fields[1]),
			PK = Convert.ToInt32(fields[2]),
			ACTIF = Convert.ToBoolean(fields[3]),
			TYPE = result
		};
		baliseRow.SIGBalise = new SIGBalise(id)
		{
			Actif = baliseRow.ACTIF,
			PK = baliseRow.PK,
			Type = baliseRow.TYPE
		};
		return baliseRow;
	}

	public static BaliseRow FromBinary(BinaryReader reader)
	{
		int id = BaseRow.ReadId(reader);
		BaliseRow baliseRow = new BaliseRow(id)
		{
			VOIE = BaseRow.ReadId(reader),
			PK = BaseRow.ReadPK(reader),
			ACTIF = reader.ReadBoolean(),
			TYPE = (BaliseType)reader.ReadSByte()
		};
		baliseRow.SIGBalise = new SIGBalise(id)
		{
			Actif = baliseRow.ACTIF,
			PK = baliseRow.PK,
			Type = baliseRow.TYPE
		};
		return baliseRow;
	}

	internal bool Update()
	{
		if (SIGBalise.ID != _id)
		{
			return false;
		}
		PK = SIGBalise.PK;
		TYPE = SIGBalise.Type;
		if (VOIE != SIGBalise.Voie.ID)
		{
			return false;
		}
		VOIE = SIGBalise.Voie.ID;
		return true;
	}

	public override string ToCsv()
	{
		return base.ID.ToString() + ";" + VOIE + ";" + PK + ";" + ACTIF.ToString() + ";" + TYPE.ToString();
	}

	public override void ToBinaryStream(BinaryWriter writer)
	{
		BaseRow.WriteId(writer, base.ID);
		BaseRow.WriteId(writer, VOIE);
		BaseRow.WritePK(writer, PK);
		writer.Write(ACTIF);
		writer.Write((sbyte)TYPE);
	}

	public override string ToString()
	{
		return $"[id={base.ID}]  {TYPE} Actif={ACTIF} idVoie={VOIE} PK={(double)PK / 1000.0:0.###}";
	}
}
