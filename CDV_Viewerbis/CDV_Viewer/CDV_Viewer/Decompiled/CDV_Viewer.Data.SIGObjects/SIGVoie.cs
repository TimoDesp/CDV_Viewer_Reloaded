using System;
using System.Collections.Generic;
using CDV_Viewer.Traitements;

namespace CDV_Viewer.Data.SIGObjects;

public class SIGVoie : IComparable, ISigId
{
	public class Segment
	{
		private Segment Precedent { get; set; }

		private SIGExtremite Debut { get; set; }

		private SIGExtremite Fin { get; set; }

		private Segment Suivant { get; set; }

		public void Insert(Segment S)
		{
			_ = Precedent;
			Precedent = S;
			S.Precedent = this;
			S.Suivant = this;
		}

		public void Add(Segment S)
		{
			Segment suivant = Suivant;
			Suivant = S;
			S.Precedent = this;
			S.Suivant = suivant;
		}
	}

	private string _nom;

	public SIGLigne Ligne;

	public List<SIGNoeud> Noeuds = new List<SIGNoeud>();

	public List<SIGBranche> Branches = new List<SIGBranche>();

	public List<SIGJoint> Joints = new List<SIGJoint>();

	public int PositionY;

	public List<SIGBalise> Balises = new List<SIGBalise>();

	private bool _isJonction;

	public int ID { get; private set; }

	public string Nom
	{
		get
		{
			return _nom;
		}
		set
		{
			_nom = value;
			_isJonction = _nom.Length > 1 && Nom[0] == 'J';
		}
	}

	public SIGJoint FirstJoint
	{
		get
		{
			if (Joints.Count <= 0)
			{
				return null;
			}
			return Joints[0];
		}
	}

	public SIGJoint LastJoint
	{
		get
		{
			if (Joints.Count <= 0)
			{
				return null;
			}
			return Joints[Joints.Count - 1];
		}
	}

	public SIGNoeud NoeudDebut { get; internal set; }

	public SIGNoeud NoeudFin { get; internal set; }

	public SIGBranche BrancheDebut { get; internal set; }

	public SIGBranche BrancheFin { get; internal set; }

	public int PKDebut { get; internal set; } = int.MaxValue;

	public int PKFin { get; internal set; } = int.MaxValue;

	public string FullName => Ligne?.ID.ToString() + ":" + Nom;

	public SIGJoint JointBefore(int pk)
	{
		if (Joints.Count - 1 < 0)
		{
			return null;
		}
		for (int num = Joints.Count - 1; num >= 0; num--)
		{
			if (Joints[num].PK < pk)
			{
				return Joints[num];
			}
		}
		return null;
	}

	public SIGJoint JointBefore(int pk, int distanceMax)
	{
		if (Joints.Count - 1 < 0)
		{
			return null;
		}
		for (int num = Joints.Count - 1; num >= 0; num--)
		{
			if (Joints[num].PK < pk && pk - Joints[num].PK < distanceMax)
			{
				return Joints[num];
			}
		}
		return null;
	}

	public SIGJoint JointAfter(int pk)
	{
		int count = Joints.Count;
		if (count <= 0)
		{
			return null;
		}
		for (int i = 0; i < count; i++)
		{
			if (Joints[i].PK > pk)
			{
				return Joints[i];
			}
		}
		return null;
	}

	public SIGJoint JointAfter(int pk, int distanceMax)
	{
		int count = Joints.Count;
		if (count <= 0)
		{
			return null;
		}
		for (int i = 0; i < count; i++)
		{
			if (Joints[i].PK > pk && Joints[i].PK - pk < distanceMax)
			{
				return Joints[i];
			}
		}
		return null;
	}

	public SIGJoint[] JointsAround(int pk)
	{
		SIGJoint sIGJoint = null;
		SIGJoint sIGJoint2 = null;
		int count = Joints.Count;
		if (count > 0)
		{
			for (int i = 0; i < count; i++)
			{
				sIGJoint2 = Joints[i];
				if (sIGJoint2.PK > pk)
				{
					return new SIGJoint[2] { sIGJoint, sIGJoint2 };
				}
				sIGJoint = Joints[i];
			}
			return new SIGJoint[2] { sIGJoint, null };
		}
		return new SIGJoint[2] { sIGJoint, sIGJoint2 };
	}

	public SIGNoeud NoeudBefore(SIGNoeud noeud)
	{
		return BrancheBefore(noeud)?.Noeud;
	}

	public SIGNoeud NoeudAfter(SIGNoeud noeud)
	{
		return BrancheAfter(noeud)?.Noeud;
	}

	public SIGBranche BrancheBefore(SIGNoeud noeud)
	{
		if (Branches.Count - 1 < 0)
		{
			return null;
		}
		SIGBranche sIGBranche = null;
		int num = Branches.Count - 1;
		while (num >= 0 && Branches[num].Noeud != noeud)
		{
			num--;
		}
		while (num >= 0)
		{
			if ((sIGBranche = Branches[num]).Noeud != noeud)
			{
				return sIGBranche;
			}
			num--;
		}
		return null;
	}

	public SIGBranche BrancheAfter(SIGNoeud noeud)
	{
		int count = Branches.Count;
		if (count < 0)
		{
			return null;
		}
		SIGBranche sIGBranche = null;
		int i;
		for (i = 0; i < count && Branches[i].Noeud != noeud; i++)
		{
		}
		for (; i < count; i++)
		{
			if ((sIGBranche = Branches[i]).Noeud != noeud)
			{
				return sIGBranche;
			}
		}
		return null;
	}

	public SIGBranche[] BranchesAround(int pk)
	{
		SIGBranche sIGBranche = null;
		SIGBranche sIGBranche2 = null;
		int count = Branches.Count;
		if (count > 0)
		{
			for (int i = 0; i < count; i++)
			{
				sIGBranche2 = Branches[i];
				if (sIGBranche2.PK > pk)
				{
					return new SIGBranche[2] { sIGBranche, sIGBranche2 };
				}
				sIGBranche = Branches[i];
			}
			return new SIGBranche[2] { sIGBranche, null };
		}
		return new SIGBranche[2] { sIGBranche, sIGBranche2 };
	}

	public SIGCircuit GetCircuit(int pk)
	{
		SIGJoint[] array = JointsAround(pk);
		SIGDemiJoint sIGDemiJoint = null;
		SIGDemiJoint sIGDemiJoint2 = null;
		sIGDemiJoint = array[0]?.DemiJointAval;
		sIGDemiJoint2 = array[1]?.DemiJointAmont;
		if (sIGDemiJoint == null && sIGDemiJoint2 == null)
		{
			return null;
		}
		if (sIGDemiJoint == null)
		{
			return sIGDemiJoint2.Circuit;
		}
		if (sIGDemiJoint2 == null)
		{
			return sIGDemiJoint.Circuit;
		}
		if (sIGDemiJoint.Circuit == sIGDemiJoint2.Circuit)
		{
			return sIGDemiJoint.Circuit;
		}
		Dialogs.Message($"Incohérence en base\n (2 joints autour du PK{pk} linkés sur des Cdv différents)");
		return null;
	}

	public bool IsJonction()
	{
		return _isJonction;
	}

	public bool IsVoiePrincipale()
	{
		return !IsJonction();
	}

	public static bool IsJonction(SIGVoie v)
	{
		return v?.IsJonction() ?? false;
	}

	public static bool IsVoiePrincipale(SIGVoie v)
	{
		return v?.IsVoiePrincipale() ?? false;
	}

	public SIGVoie()
	{
	}

	public SIGVoie(int id, string nom)
	{
		ID = id;
		Nom = nom;
	}

	public SIGVoie(int id)
	{
		ID = id;
	}

	internal void Unlink()
	{
		Ligne = null;
		Branches.Clear();
		Balises.Clear();
		Joints.Clear();
		Noeuds.Clear();
		PKDebut = int.MaxValue;
		PKFin = int.MinValue;
	}

	internal SIGBranche GetBrancheDebut()
	{
		PKDebut = 0;
		NoeudDebut = null;
		if (Branches.Count == 0)
		{
			return null;
		}
		BrancheDebut = Branches[0];
		PKDebut = BrancheDebut.PK;
		NoeudDebut = BrancheDebut.Noeud;
		return BrancheDebut;
	}

	internal SIGBranche GetBrancheFin()
	{
		BrancheFin = null;
		PKFin = 1000000;
		int num = Branches.Count - 1;
		if (num < 0)
		{
			return null;
		}
		BrancheFin = Branches[num];
		NoeudFin = BrancheFin.Noeud;
		PKFin = BrancheFin.PK;
		return BrancheFin;
	}

	public List<SIGSegment> FindSegments(SIGJoint j1, SIGJoint j2)
	{
		List<SIGSegment> list = new List<SIGSegment>();
		int pK = j1.PK;
		int pK2 = j2.PK;
		if (pK2 < pK)
		{
			pK = j2.PK;
			pK2 = j1.PK;
			SIGSegment sIGSegment = new SIGSegment(this, j1, null);
			list.Add(sIGSegment);
			for (int num = Branches.Count - 1; num >= 0; num--)
			{
				SIGBranche sIGBranche = Branches[num];
				if (sIGBranche.PK <= pK2)
				{
					if (sIGBranche.PK < pK)
					{
						break;
					}
					if (!sIGBranche.IsAmont)
					{
						sIGSegment.ExtremiteF = sIGBranche;
						if (--num < 0)
						{
							break;
						}
						if (Branches[num].PK != sIGBranche.PK)
						{
							sIGSegment = new SIGSegment(this, Branches[num], null);
							list.Add(sIGSegment);
						}
					}
				}
			}
			if (sIGSegment.ExtremiteD == null)
			{
				Dialogs.Message($"Noeud à cheval sur le joint {j1} ou {j2}");
				return new List<SIGSegment>
				{
					new SIGSegment(this, j1, j2)
				};
			}
			sIGSegment.ExtremiteF = j2;
		}
		else
		{
			SIGSegment sIGSegment2 = new SIGSegment(this, j1, null);
			list.Add(sIGSegment2);
			for (int i = 0; i < Branches.Count; i++)
			{
				SIGBranche sIGBranche2 = Branches[i];
				if (sIGBranche2.Noeud.IsSautPk || sIGBranche2.PK < pK)
				{
					continue;
				}
				if (sIGBranche2.PK > pK2)
				{
					break;
				}
				if (!sIGBranche2.IsAval)
				{
					sIGSegment2.ExtremiteF = sIGBranche2;
					if (++i >= Branches.Count)
					{
						break;
					}
					if (Branches[i].PK != sIGBranche2.PK)
					{
						sIGSegment2 = new SIGSegment(this, Branches[i], null);
						list.Add(sIGSegment2);
					}
				}
			}
			if (sIGSegment2.ExtremiteD == null)
			{
				Dialogs.Message($"Noeud à cheval sur le joint {j1} ou {j2}");
				return new List<SIGSegment>
				{
					new SIGSegment(this, j1, j2)
				};
			}
			sIGSegment2.ExtremiteF = j2;
		}
		return list;
	}

	public List<SIGSegment> FindSegments(SIGExtremite e1, SIGExtremite e2)
	{
		List<SIGSegment> list = new List<SIGSegment>();
		int pK = e1.PK;
		int pK2 = e2.PK;
		if (pK2 < pK)
		{
			pK = e2.PK;
			pK2 = e1.PK;
			SIGSegment sIGSegment = new SIGSegment(this, e1, null);
			list.Add(sIGSegment);
			for (int num = Branches.Count - 1; num >= 0; num--)
			{
				SIGBranche sIGBranche = Branches[num];
				if (sIGBranche.PK <= pK2)
				{
					if (sIGBranche.PK < pK)
					{
						break;
					}
					if (!sIGBranche.IsAmont)
					{
						sIGSegment.ExtremiteF = sIGBranche;
						if (--num < 0)
						{
							break;
						}
						if (Branches[num].PK != sIGBranche.PK)
						{
							sIGSegment = new SIGSegment(this, Branches[num], null);
							list.Add(sIGSegment);
						}
					}
				}
			}
			if (sIGSegment.ExtremiteD == null)
			{
				Dialogs.Message($"Noeud à cheval sur le joint {e1} ou {e2}");
				return new List<SIGSegment>
				{
					new SIGSegment(this, e1, e2)
				};
			}
			sIGSegment.ExtremiteF = e2;
		}
		else
		{
			SIGSegment sIGSegment2 = new SIGSegment(this, e1, null);
			list.Add(sIGSegment2);
			for (int i = 0; i < Branches.Count; i++)
			{
				SIGBranche sIGBranche2 = Branches[i];
				if (sIGBranche2.PK < pK)
				{
					continue;
				}
				if (sIGBranche2.PK > pK2)
				{
					break;
				}
				if (!sIGBranche2.IsAval)
				{
					sIGSegment2.ExtremiteF = sIGBranche2;
					if (++i >= Branches.Count)
					{
						break;
					}
					if (Branches[i].PK != sIGBranche2.PK)
					{
						sIGSegment2 = new SIGSegment(this, Branches[i], null);
						list.Add(sIGSegment2);
					}
				}
			}
			if (sIGSegment2.ExtremiteD == null)
			{
				Dialogs.Message($"Noeud à cheval sur le joint {e1} ou {e2}");
				return new List<SIGSegment>
				{
					new SIGSegment(this, e1, e2)
				};
			}
			sIGSegment2.ExtremiteF = e2;
		}
		return list;
	}

	public SIGBranche[] FindNoeudOnSameLine(SIGVoie v2, int pk1, int pk2)
	{
		if (Ligne != v2.Ligne)
		{
			return null;
		}
		if (Noeuds.Count < 2)
		{
			Dialogs.BaseError($"pas de Noeud sur la voie {this}", this, 0);
			return null;
		}
		if (v2.Noeuds.Count < 2)
		{
			Dialogs.BaseError($"pas de Noeud sur la voie {v2}", v2, 0);
			return null;
		}
		if (pk1 > pk2)
		{
			int num = pk1;
			pk1 = pk2;
			pk2 = num;
			foreach (SIGBranche branch in Branches)
			{
				if (branch.IsAmont || branch.PK < pk1)
				{
					continue;
				}
				if (branch.PK > pk2)
				{
					break;
				}
				foreach (SIGBranche branch2 in v2.Branches)
				{
					if (!branch2.IsAval && branch2.PK >= pk1)
					{
						if (branch2.PK > pk2)
						{
							break;
						}
						if (branch2.Noeud == branch.Noeud)
						{
							return new SIGBranche[2] { branch, branch2 };
						}
					}
				}
			}
		}
		else
		{
			foreach (SIGBranche branch3 in Branches)
			{
				if (branch3.IsAval || branch3.PK < pk1)
				{
					continue;
				}
				if (branch3.PK > pk2)
				{
					break;
				}
				foreach (SIGBranche branch4 in v2.Branches)
				{
					if (!branch4.IsAmont && branch4.PK >= pk1)
					{
						if (branch4.PK > pk2)
						{
							break;
						}
						if (branch4.Noeud == branch3.Noeud)
						{
							return new SIGBranche[2] { branch3, branch4 };
						}
					}
				}
			}
		}
		return null;
	}

	public SIGNoeud FindNoeudOnOtherTrack(SIGVoie v2)
	{
		foreach (SIGNoeud noeud in Noeuds)
		{
			if (noeud.FirstBrancheInTrack(v2) != null)
			{
				return noeud;
			}
		}
		return null;
	}

	public bool Intersect(SIGVoie voieB)
	{
		if (PKDebut <= voieB.PKFin)
		{
			return PKFin >= voieB.PKDebut;
		}
		return false;
	}

	public int CompareTo(object Y)
	{
		SIGVoie sIGVoie = (SIGVoie)Y;
		if (this == sIGVoie)
		{
			return 0;
		}
		if (Nom == "UNIQUE")
		{
			return -1;
		}
		if (sIGVoie.Nom == "UNIQUE")
		{
			return 1;
		}
		bool flag = Nom[0] == 'V';
		bool flag2 = sIGVoie.Nom[0] == 'V';
		if (flag == flag2)
		{
			Chaines.GetFirstNombre(Nom, out var position);
			Chaines.GetFirstNombre(sIGVoie.Nom, out var position2);
			int num = position.CompareTo(position2);
			if (num == 0)
			{
				num = position.CompareTo(position);
			}
			return num;
		}
		if (!flag)
		{
			return 1;
		}
		return -1;
	}

	public override string ToString()
	{
		if (Ligne != null)
		{
			return "Voie " + Ligne.ID + ":" + Nom;
		}
		return "Voie " + Nom;
	}
}
