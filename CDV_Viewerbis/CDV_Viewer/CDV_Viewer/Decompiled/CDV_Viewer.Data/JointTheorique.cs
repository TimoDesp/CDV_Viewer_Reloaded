using CDV_Viewer.CsvBase;

namespace CDV_Viewer.Data;

public class JointTheorique
{
	public JointType Type;

	public int DemiLongueur;

	public bool DB;

	public JointTheorique(JointType type, int demiLongueur)
	{
		Type = type;
		DemiLongueur = demiLongueur;
	}

	public JointTheorique(JointType type, int demiLongueur, bool db)
	{
		Type = type;
		DemiLongueur = demiLongueur;
		DB = db;
	}
}
