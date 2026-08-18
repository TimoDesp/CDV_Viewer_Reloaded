using System.IO;
using CDV_Viewer.Data.SIGObjects;

namespace CDV_Viewer.CsvBase;

public class LigneRow : BaseRow
{
	public string NOM;

	public SIGLigne SIGLigne { get; private set; }

	private LigneRow()
	{
	}

	internal LigneRow(int id)
	{
		_id = id;
	}

	internal LigneRow(SIGLigne newLigne)
	{
		_id = newLigne.ID;
		SIGLigne = newLigne;
		Update();
	}

	internal bool Update()
	{
		NOM = SIGLigne.Nom;
		return false;
	}

	public static LigneRow FromCsv(string[] fields)
	{
		int id = BaseRow.GetId(fields);
		LigneRow ligneRow = new LigneRow(id)
		{
			NOM = fields[1]
		};
		ligneRow.SIGLigne = new SIGLigne(id, ligneRow.NOM);
		return ligneRow;
	}

	public static LigneRow FromBinary(BinaryReader reader)
	{
		int id = BaseRow.ReadId(reader);
		LigneRow ligneRow = new LigneRow(id)
		{
			NOM = reader.ReadString()
		};
		ligneRow.SIGLigne = new SIGLigne(id, ligneRow.NOM);
		return ligneRow;
	}

	public override string ToCsv()
	{
		return base.ID + ";" + NOM;
	}

	public override void ToBinaryStream(BinaryWriter writer)
	{
		BaseRow.WriteId(writer, base.ID);
		writer.Write(NOM);
	}

	public override string ToString()
	{
		return ToCsv();
	}
}
