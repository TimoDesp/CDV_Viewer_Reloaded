using System;

namespace CDV_Viewer.Data.SIGObjects;

public class SIGDemiJoint : IComparable
{
	public int ID = -1;

	public SIGCircuit Circuit;

	public SIGJoint Joint;

	public double DemiPas;

	public bool Principal;

	public bool DB;

	public bool Emetteur;

	public bool IsAmont => Joint?._demiJointAmont == this;

	public bool IsAval => Joint?._demiJointAval == this;

	public SIGDemiJoint Oposite
	{
		get
		{
			if (!IsAmont)
			{
				return Joint?.DemiJointAmont;
			}
			return Joint?._demiJointAval;
		}
	}

	public SIGDemiJoint()
	{
	}

	public SIGDemiJoint(int id)
	{
		ID = id;
	}

	public SIGDemiJoint(int id, SIGCircuit circuit, SIGJoint joint, double demiPas, bool principal, bool db, bool emetteur)
	{
		ID = id;
		Joint = joint;
		Circuit = circuit;
		DemiPas = demiPas;
		Principal = principal;
		DB = db;
		Emetteur = emetteur;
	}

	public int CompareTo(object y)
	{
		SIGDemiJoint sIGDemiJoint = (SIGDemiJoint)y;
		if (!Principal && sIGDemiJoint.Principal)
		{
			return 1;
		}
		if (Principal && !sIGDemiJoint.Principal)
		{
			return -1;
		}
		return Joint.PK.CompareTo(sIGDemiJoint.Joint.PK);
	}

	public override string ToString()
	{
		return $"{Joint} -> {Circuit}";
	}

	internal void UnLink()
	{
		Joint = null;
		Circuit = null;
	}
}
