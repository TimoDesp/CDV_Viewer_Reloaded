using System;
using System.IO;
using CDV_Viewer.Data.SIGObjects;

namespace CDV_Viewer.CsvBase;

public class VoieRow : BaseRow
{
	public static readonly VoieRow Empty = new VoieRow();

	public int LIGNE;

	public string NOM;

	public int PKD;

	public int PKF;

	public SIGVoie SIGVoie { get; private set; }

	private VoieRow()
	{
	}

	internal VoieRow(int id)
	{
		_id = id;
	}

	internal VoieRow(SIGVoie voie)
	{
		_id = voie.ID;
		SIGVoie = voie;
		Update();
	}

	public static VoieRow FromCsv(string[] fields)
	{
		int id = BaseRow.GetId(fields);
		VoieRow voieRow = new VoieRow(id)
		{
			LIGNE = Convert.ToInt32(fields[1]),
			NOM = fields[2],
			PKD = Convert.ToInt32(fields[3]),
			PKF = Convert.ToInt32(fields[4])
		};
		voieRow.SIGVoie = new SIGVoie(id)
		{
			Nom = voieRow.NOM
		};
		return voieRow;
	}

	public static VoieRow FromBinary(BinaryReader reader)
	{
		int id = BaseRow.ReadId(reader);
		VoieRow voieRow = new VoieRow(id)
		{
			LIGNE = BaseRow.ReadId(reader),
			NOM = BaseRow.ReadAsciiString(reader),
			PKD = BaseRow.ReadPK(reader),
			PKF = BaseRow.ReadPK(reader)
		};
		voieRow.SIGVoie = new SIGVoie(id)
		{
			Nom = voieRow.NOM
		};
		return voieRow;
	}

	internal bool Update()
	{
		if (SIGVoie.ID != _id)
		{
			return false;
		}
		NOM = SIGVoie.Nom;
		PKD = SIGVoie.PKDebut;
		PKF = SIGVoie.PKFin;
		if (LIGNE == SIGVoie.Ligne.ID)
		{
			return false;
		}
		LIGNE = SIGVoie.Ligne.ID;
		return true;
	}

	public override string ToCsv()
	{
		return base.ID.ToString() + ";" + LIGNE + ";" + NOM + ";" + PKD + ";" + PKF;
	}

	public override void ToBinaryStream(BinaryWriter writer)
	{
		BaseRow.WriteId(writer, base.ID);
		BaseRow.WriteId(writer, LIGNE);
		BaseRow.WriteAsciiString(writer, NOM);
		BaseRow.WritePK(writer, PKD);
		BaseRow.WritePK(writer, PKF);
	}

	public override string ToString()
	{
		return $"[id={base.ID}]  {LIGNE}:{NOM}";
	}
}
