using CDV_Viewer.Data.SIGObjects;

namespace CDV_Viewer.Traitements;

public class CircuitOnLigne
{
	public SIGCircuit Circuit;

	public int Ligne;

	public int PK;

	public CircuitOnLigne(SIGCircuit circuit, int ligne, int pk)
	{
		Circuit = circuit;
		Ligne = ligne;
		PK = pk;
	}
}
