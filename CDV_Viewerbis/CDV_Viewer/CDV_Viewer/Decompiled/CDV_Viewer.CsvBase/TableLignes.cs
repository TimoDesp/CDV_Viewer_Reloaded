using System.Collections.Generic;
using System.IO;
using System.Linq;
using CDV_Viewer.Data.SIGObjects;

namespace CDV_Viewer.CsvBase;

public class TableLignes : BaseTable<LigneRow>
{
	public override string Path => "topo\\Lignes.csv";

	public override string Header => "ID;NOM";

	public override LigneRow RowFromCsv(string[] csvFields)
	{
		return LigneRow.FromCsv(csvFields);
	}

	public override LigneRow RowFromBinary(BinaryReader reader)
	{
		return LigneRow.FromBinary(reader);
	}

	public override void Link()
	{
		foreach (LigneRow value in _items.Values)
		{
			value.SIGLigne.Unlink();
		}
	}

	public bool Create(int idLigne, string nom)
	{
		if (!ContainsKey(idLigne))
		{
			LigneRow row = new LigneRow(new SIGLigne(idLigne, nom));
			Add(row);
			return true;
		}
		return false;
	}

	public bool Update(int idLigne)
	{
		if (!TryGetValue(idLigne, out var row))
		{
			return false;
		}
		row.Update();
		return true;
	}

	internal SIGLigne SigLigne(int idLigne)
	{
		if (!Base.IsLinked)
		{
			Base.Link();
		}
		if (TryGetValue(idLigne, out var row))
		{
			return row.SIGLigne;
		}
		return null;
	}

	internal List<SIGLigne> SigLignes()
	{
		if (!Base.IsLinked)
		{
			Base.Link();
		}
		return _items.Values.Select((LigneRow l) => l.SIGLigne).ToList();
	}
}
