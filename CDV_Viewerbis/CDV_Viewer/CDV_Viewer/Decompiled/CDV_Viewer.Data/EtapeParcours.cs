using System;
using CDV_Viewer.CsvBase;

namespace CDV_Viewer.Data;

public class EtapeParcours
{
	public int Ligne = -1;

	public string Voie = string.Empty;

	public int IdVoie = -1;

	public int PkD;

	public int PkF;

	public int DeltaXD;

	public int DeltaXF;

	public int Longueur => Math.Abs(DeltaXF - DeltaXD);

	public EtapeParcours()
	{
	}

	public EtapeParcours(int ligne, string voie, int pkD, int deltaXD)
	{
		Ligne = ligne;
		Voie = voie;
		IdVoie = Base.GetIDVoie(ligne, voie, pkD);
		PkD = pkD;
		DeltaXD = deltaXD;
	}

	public EtapeParcours(int ligne, string voie, int deltaXD, int pkD, int deltaXF, int pkF)
	{
		Ligne = ligne;
		Voie = voie;
		IdVoie = Base.GetIDVoie(ligne, voie, pkD);
		PkD = pkD;
		PkF = pkF;
		DeltaXD = deltaXD;
		DeltaXF = deltaXF;
	}
}
