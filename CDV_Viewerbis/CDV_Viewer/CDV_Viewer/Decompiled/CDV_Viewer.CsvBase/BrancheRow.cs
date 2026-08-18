using System;
using System.IO;
using CDV_Viewer.Data.SIGObjects;

namespace CDV_Viewer.CsvBase;

public class BrancheRow : BaseRow
{
	public int NOEUD;

	public int VOIE;

	public int PK;

	public BrancheType TYPE;

	public SIGBranche SIGBranche { get; private set; }

	private BrancheRow()
	{
	}

	internal BrancheRow(int id)
	{
		_id = id;
	}

	internal BrancheRow(SIGBranche branche)
	{
		_id = branche.ID;
		SIGBranche = branche;
		Update();
	}

	public static BrancheRow FromCsv(string[] fields)
	{
		int id = BaseRow.GetId(fields);
		if (!Enum.TryParse<BrancheType>(fields[4], out var result))
		{
			result = BrancheType.Amont;
		}
		BrancheRow brancheRow = new BrancheRow(id)
		{
			NOEUD = Convert.ToInt32(fields[1]),
			VOIE = Convert.ToInt32(fields[2]),
			PK = Convert.ToInt32(fields[3]),
			TYPE = result
		};
		brancheRow.SIGBranche = new SIGBranche(id)
		{
			PK = brancheRow.PK,
			Type = brancheRow.TYPE
		};
		return brancheRow;
	}

	public static BrancheRow FromBinary(BinaryReader reader)
	{
		int id = BaseRow.ReadId(reader);
		BrancheRow brancheRow = new BrancheRow(id)
		{
			NOEUD = BaseRow.ReadId(reader),
			VOIE = BaseRow.ReadId(reader),
			PK = BaseRow.ReadPK(reader),
			TYPE = (BrancheType)reader.ReadSByte()
		};
		brancheRow.SIGBranche = new SIGBranche(id)
		{
			PK = brancheRow.PK,
			Type = brancheRow.TYPE
		};
		return brancheRow;
	}

	internal bool Update()
	{
		NOEUD = SIGBranche.Noeud.ID;
		VOIE = SIGBranche.Voie.ID;
		PK = SIGBranche.PK;
		TYPE = SIGBranche.Type;
		return true;
	}

	public override string ToCsv()
	{
		return base.ID.ToString() + ";" + NOEUD + ";" + VOIE + ";" + PK + ";" + TYPE.ToString();
	}

	public override void ToBinaryStream(BinaryWriter writer)
	{
		BaseRow.WriteId(writer, base.ID);
		BaseRow.WriteId(writer, NOEUD);
		BaseRow.WriteId(writer, VOIE);
		BaseRow.WritePK(writer, PK);
		writer.Write((sbyte)TYPE);
	}

	public override string ToString()
	{
		return $"[id={base.ID}]  {NOEUD} {TYPE} idVoie={VOIE} PK={(double)PK / 1000.0:0.###}";
	}
}
