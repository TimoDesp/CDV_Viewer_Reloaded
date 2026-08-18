using System.Collections.Generic;
using CDV_Viewer.CsvBase;

namespace CDV_Viewer.Data.SIGObjects;

public class SIGBranche : SIGExtremite
{
	public class NoeudComparer : IEqualityComparer<SIGBranche>
	{
		public bool Equals(SIGBranche x, SIGBranche y)
		{
			return x.Noeud.ID == y.Noeud.ID;
		}

		public int GetHashCode(SIGBranche obj)
		{
			return obj.Noeud.ID.GetHashCode();
		}
	}

	public class VoieComparer : IEqualityComparer<SIGBranche>
	{
		public bool Equals(SIGBranche x, SIGBranche y)
		{
			return x.Voie.ID == y.Voie.ID;
		}

		public int GetHashCode(SIGBranche obj)
		{
			return obj.Voie.ID.GetHashCode();
		}
	}

	public static NoeudComparer CompareNoeud;

	public static VoieComparer CompareVoie;

	public int ID;

	public SIGNoeud Noeud;

	public BrancheType Type;

	public bool IsAmont => Type == BrancheType.Amont;

	public bool IsAval => Type == BrancheType.Aval;

	public bool ChgSensPk
	{
		get
		{
			if (IsAmont)
			{
				return Noeud == Voie.NoeudDebut;
			}
			return Noeud == Voie.NoeudFin;
		}
	}

	public bool IsBrancheDebut => Voie.BrancheDebut == this;

	public bool IsBrancheFin => Voie.BrancheFin == this;

	public bool IsAntenne
	{
		get
		{
			if (Noeud.AntenneAmont != this)
			{
				return Noeud.AntenneAval == this;
			}
			return true;
		}
	}

	public SIGBranche()
	{
	}

	public SIGBranche(int id)
	{
		ID = id;
	}

	public SIGBranche(int id, SIGNoeud noeud, SIGVoie voie, int pk, BrancheType type)
	{
		ID = id;
		Noeud = noeud;
		Voie = voie;
		PK = pk;
		Type = type;
	}

	public SIGJoint GetNearestJoint()
	{
		if (IsBrancheDebut)
		{
			return Voie.FirstJoint;
		}
		if (IsBrancheFin)
		{
			return Voie.LastJoint;
		}
		if (IsAval)
		{
			return Voie.JointAfter(PK);
		}
		return Voie.JointBefore(PK);
	}

	public SIGDemiJoint GetNearestDemiJoint()
	{
		if (IsBrancheDebut)
		{
			return Voie.FirstJoint?.DemiJointAmont;
		}
		if (IsBrancheFin)
		{
			return Voie.LastJoint?.DemiJointAval;
		}
		if (IsAval)
		{
			return Voie.JointAfter(PK)?.DemiJointAmont;
		}
		return Voie.JointBefore(PK)?.DemiJointAval;
	}

	public SIGDemiJoint GetNearestOpositeDemiJoint()
	{
		if (IsBrancheDebut)
		{
			return Voie.FirstJoint?.DemiJointAval;
		}
		if (IsBrancheFin)
		{
			return Voie.LastJoint?.DemiJointAmont;
		}
		if (IsAval)
		{
			return Voie.JointAfter(PK)?.DemiJointAval;
		}
		return Voie.JointBefore(PK)?.DemiJointAmont;
	}

	public override string ToString()
	{
		return $"Branche {Type} -> " + base.ToString();
	}
}
