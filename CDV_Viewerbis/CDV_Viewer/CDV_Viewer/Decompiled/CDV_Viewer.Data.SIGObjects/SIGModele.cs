using System.Collections.Generic;
using System.Drawing;

namespace CDV_Viewer.Data.SIGObjects;

public class SIGModele
{
	public int ID = -1;

	public SIGCircuit Circuit;

	public SIGDemiJoint DemiJointE;

	public SIGDemiJoint DemiJointS;

	public List<Point> Points;

	public List<int> Condos;

	public SIGModele(int id)
	{
		ID = id;
	}
}
