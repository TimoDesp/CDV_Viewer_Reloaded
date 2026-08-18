using System.Collections.Generic;
using CDV_Viewer.Traitements;

namespace CDV_Viewer.Data.SIGObjects;

public class SIGExtremite
{
	public SIGVoie Voie;

	public int PK;

	public static bool HaveSameNode(SIGExtremite e1, SIGExtremite e2)
	{
		if (e1 == null || e2 == null)
		{
			return false;
		}
		if (e1 is SIGBranche && e2 is SIGBranche)
		{
			return (e1 as SIGBranche).Noeud == (e2 as SIGBranche).Noeud;
		}
		return false;
	}

	public SIGExtremite()
	{
	}

	public SIGExtremite(SIGVoie voie, int pk)
	{
		Voie = voie;
		PK = pk;
	}

	public override string ToString()
	{
		return $"{Voie} Pk {(double)PK / 1000.0:0.000}";
	}

	public List<SIGJoint> FindJointsAval()
	{
		return FindJoints(SearchDirection.Plus);
	}

	public List<SIGJoint> FindJointsAmont()
	{
		return FindJoints(SearchDirection.Moins);
	}

	private List<SIGJoint> FindJoints(SearchDirection direction)
	{
		return CDV_Viewer.Traitements.Composants.FindJoints(this, direction);
	}
}
