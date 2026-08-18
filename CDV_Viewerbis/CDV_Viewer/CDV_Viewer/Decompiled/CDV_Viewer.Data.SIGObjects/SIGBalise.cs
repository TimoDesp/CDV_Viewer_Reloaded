using System;
using CDV_Viewer.CsvBase;

namespace CDV_Viewer.Data.SIGObjects;

public class SIGBalise
{
	public int ID = -1;

	public SIGVoie Voie;

	public int PK;

	public bool Actif;

	public BaliseType Type = BaliseType.CRO;

	public SIGBalise()
	{
	}

	public SIGBalise(int id)
	{
		ID = id;
	}

	public static BaliseType TypeFromString(string typeStr)
	{
		if (!Enum.TryParse<BaliseType>(typeStr, out var result))
		{
			return BaliseType.CRO;
		}
		return result;
	}
}
