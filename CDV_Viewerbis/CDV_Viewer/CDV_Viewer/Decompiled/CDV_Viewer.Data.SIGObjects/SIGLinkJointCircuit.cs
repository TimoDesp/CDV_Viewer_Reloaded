namespace CDV_Viewer.Data.SIGObjects;

public class SIGLinkJointCircuit
{
	public int ID;

	public int Joint;

	public int Circuit;

	public bool Principal;

	public SIGLinkJointCircuit(int id, int idJoint, int idCircuit, bool principal)
	{
		ID = id;
		Joint = idJoint;
		Circuit = idCircuit;
		Principal = principal;
	}
}
