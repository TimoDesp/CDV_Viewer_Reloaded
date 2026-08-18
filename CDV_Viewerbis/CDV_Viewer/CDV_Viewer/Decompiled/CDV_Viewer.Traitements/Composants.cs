using System;
using System.Collections.Generic;
using System.Linq;
using CDV_Viewer.Data;
using CDV_Viewer.Data.SIGObjects;

namespace CDV_Viewer.Traitements;

public static class Composants
{
	private static bool TryFindParcoursOnSameTrack(SIGDemiJoint demiJointA, SIGDemiJoint demiJointB, List<SIGSegment> result)
	{
		result.Clear();
		if (demiJointA.Joint.Voie != demiJointB.Joint.Voie)
		{
			return false;
		}
		SIGVoie voie = demiJointA.Joint.Voie;
		result.AddRange(voie.FindSegments(demiJointA.Joint, demiJointB.Joint));
		return true;
	}

	private static bool TryFindParcoursOnSameLine(SIGDemiJoint demiJointA, SIGDemiJoint demiJointB, List<SIGSegment> result)
	{
		result.Clear();
		SIGVoie voie = demiJointA.Joint.Voie;
		SIGVoie voie2 = demiJointB.Joint.Voie;
		if (voie.Ligne != voie2.Ligne)
		{
			return false;
		}
		SIGBranche[] array = voie.FindNoeudOnSameLine(voie2, demiJointA.Joint.PK, demiJointB.Joint.PK);
		if (array == null)
		{
			return false;
		}
		result.AddRange(voie.FindSegments(demiJointA.Joint, array[0]));
		result.AddRange(voie2.FindSegments(array[1], demiJointB.Joint));
		return true;
	}

	public static List<SIGSegment> FindParcours(SIGDemiJoint demiJointA, SIGDemiJoint demiJointB)
	{
		List<SIGSegment> result = new List<SIGSegment>();
		if (TryFindParcoursOnSameTrack(demiJointA, demiJointB, result))
		{
			return result;
		}
		if (TryFindParcoursOnSameLine(demiJointA, demiJointB, result))
		{
			return result;
		}
		return FindParcours(demiJointA.Joint, demiJointB.Joint);
	}

	public static List<SIGSegment> FindParcours(SIGExtremite extremiteA, SIGJoint jointB)
	{
		List<List<SIGSegment>> chemins = new List<List<SIGSegment>>();
		if (extremiteA == null || jointB == null)
		{
			return new List<SIGSegment>();
		}
		FindJoint(jointB, extremiteA, SearchDirection.PlusMoins, ref chemins, new List<SIGSegment>(), 0);
		if (chemins.Count == 0)
		{
			return new List<SIGSegment>();
		}
		int num = -1;
		int num2 = int.MaxValue;
		for (int i = 0; i < chemins.Count; i++)
		{
			int longueur = SIGSegment.GetLongueur(chemins[i]);
			if (num == -1 || chemins[i].Count < chemins[num].Count || longueur < num2)
			{
				num = i;
				num2 = longueur;
			}
		}
		return chemins[num];
	}

	private static void FindJoint(SIGJoint joint, SIGExtremite extremite, SearchDirection direction, ref List<List<SIGSegment>> chemins, List<SIGSegment> chemin, int profondeur)
	{
		int num = profondeur + 1;
		if (num > 5)
		{
			return;
		}
		SIGExtremite extremiteAmont = null;
		SIGExtremite extremiteAval = null;
		FindBornes(extremite, out extremiteAmont, out extremiteAval);
		if (direction != SearchDirection.Plus && extremiteAmont != null)
		{
			if (extremiteAmont is SIGJoint)
			{
				if (extremiteAmont == joint)
				{
					chemin.Add(new SIGSegment(extremite.Voie, extremite, joint));
					chemins.Add(chemin);
				}
			}
			else
			{
				List<SIGSegment> list = new List<SIGSegment>(chemin);
				list.Add(new SIGSegment(extremite.Voie, extremite, extremiteAmont));
				foreach (SIGBranche branch in ((SIGBranche)extremiteAmont).Noeud.Branches)
				{
					if (branch.Type != ((SIGBranche)extremiteAmont).Type)
					{
						SearchDirection direction2 = SearchDirection.Moins;
						if (branch.Voie.Ligne != extremite.Voie.Ligne)
						{
							direction2 = SearchDirection.PlusMoins;
						}
						FindJoint(joint, branch, direction2, ref chemins, list, num);
					}
				}
			}
		}
		if (chemins.Count > 0 || direction == SearchDirection.Moins || extremiteAval == null)
		{
			return;
		}
		if (extremiteAval is SIGJoint)
		{
			if (extremiteAval == joint)
			{
				chemin.Add(new SIGSegment(extremite.Voie, extremite, joint));
				chemins.Add(chemin);
			}
			return;
		}
		List<SIGSegment> list2 = new List<SIGSegment>(chemin);
		list2.Add(new SIGSegment(extremite.Voie, extremite, extremiteAval));
		foreach (SIGBranche branch2 in ((SIGBranche)extremiteAval).Noeud.Branches)
		{
			if (branch2.Type != ((SIGBranche)extremiteAval).Type)
			{
				SearchDirection direction3 = SearchDirection.Plus;
				if (branch2.Voie.Ligne != extremite.Voie.Ligne)
				{
					direction3 = SearchDirection.PlusMoins;
				}
				if (branch2.Voie == ((SIGBranche)extremiteAval).Voie && branch2.PK < ((SIGBranche)extremiteAval).PK)
				{
					list2.Add(new SIGSegment(extremite.Voie, branch2, extremiteAval));
					FindJoint(joint, (SIGBranche)extremiteAval, direction3, ref chemins, list2, num);
				}
				else
				{
					FindJoint(joint, branch2, direction3, ref chemins, list2, num);
				}
			}
		}
	}

	public static void GetJointsAround(SIGExtremite extremite, List<SIGJoint> joints)
	{
		int pk_min;
		int pk_max;
		if (extremite is SIGBranche)
		{
			SIGBranche sIGBranche = extremite as SIGBranche;
			pk_min = (pk_max = sIGBranche.PK);
			if ((sIGBranche.IsAmont && !sIGBranche.ChgSensPk) || (sIGBranche.IsAval && sIGBranche.ChgSensPk))
			{
				SIGJoint sIGJoint = extremite.Voie.JointBefore(extremite.PK, 5000);
				joints.Add(sIGJoint);
				if (sIGJoint == null)
				{
					return;
				}
				pk_max = sIGJoint.PK;
			}
			else
			{
				SIGJoint sIGJoint = extremite.Voie.JointAfter(extremite.PK, 5000);
				joints.Add(sIGJoint);
				if (sIGJoint == null)
				{
					return;
				}
				pk_min = sIGJoint.PK;
			}
		}
		else
		{
			pk_min = Math.Max(extremite.Voie.BrancheDebut.PK, extremite.PK - 5000);
			pk_max = Math.Max(extremite.Voie.BrancheFin.PK, extremite.PK + 5000);
			SIGJoint sIGJoint2 = extremite.Voie.JointBefore(extremite.PK, 5000);
			SIGJoint sIGJoint3 = extremite.Voie.JointAfter(extremite.PK, 5000);
			if (sIGJoint2 != null)
			{
				pk_min = sIGJoint2.PK;
				joints.Add(sIGJoint2);
			}
			if (sIGJoint3 != null)
			{
				pk_max = sIGJoint3.PK;
				joints.Add(sIGJoint3);
			}
		}
		foreach (SIGNoeud item in extremite.Voie.Branches.FindAll((SIGBranche b) => b.PK > pk_min && b.PK < pk_max).ConvertAll((SIGBranche b) => b.Noeud).Distinct())
		{
			foreach (SIGBranche item2 in item.BranchesInOtherTrack(extremite.Voie))
			{
				GetJointsAround(item2, joints);
			}
		}
	}

	public static List<SIGJoint> GetNextJoints(SIGExtremite extremite)
	{
		List<List<SIGSegment>> chemins = new List<List<SIGSegment>>();
		FindJoint(extremite, ref chemins, new List<SIGSegment>(), 0);
		List<SIGJoint> list = new List<SIGJoint>();
		foreach (List<SIGSegment> item2 in chemins)
		{
			SIGJoint item = (SIGJoint)item2[item2.Count - 1].ExtremiteF;
			if (!list.Contains(item))
			{
				list.Add(item);
			}
		}
		return list;
	}

	private static void FindJoint(SIGExtremite extremite, ref List<List<SIGSegment>> chemins, List<SIGSegment> chemin, int profondeur)
	{
		int num = profondeur + 1;
		if (num > 5)
		{
			return;
		}
		SIGJoint jointAval = null;
		SIGJoint jointAmont = null;
		FindJointsBornes(extremite, out jointAmont, out jointAval);
		int _pkMin;
		if (jointAmont != null)
		{
			_pkMin = jointAmont.PK;
			List<SIGSegment> list = new List<SIGSegment>(chemin);
			list.Add(new SIGSegment(extremite.Voie, extremite, jointAmont));
			chemins.Add(list);
		}
		else
		{
			_pkMin = extremite.PK - 5000;
		}
		int _pkMax;
		if (jointAval != null)
		{
			_pkMax = jointAval.PK;
			List<SIGSegment> list2 = new List<SIGSegment>(chemin);
			list2.Add(new SIGSegment(extremite.Voie, extremite, jointAval));
			chemins.Add(list2);
		}
		else
		{
			_pkMax = extremite.PK + 5000;
		}
		foreach (SIGNoeud item in extremite.Voie.Noeuds.FindAll((SIGNoeud noeud) => noeud.Branches.Find((SIGBranche branche) => branche.Voie == extremite.Voie && branche.PK > _pkMin && branche.PK < _pkMax) != null))
		{
			foreach (SIGBranche branch in item.Branches)
			{
				if (branch.Voie != extremite.Voie)
				{
					continue;
				}
				if (jointAmont == null && branch.PK < extremite.PK)
				{
					List<SIGSegment> list3 = new List<SIGSegment>(chemin);
					list3.Add(new SIGSegment(extremite.Voie, branch, extremite));
					foreach (SIGBranche item2 in item.BranchesInOtherTrack(extremite.Voie))
					{
						FindJoint(item2, ref chemins, list3, num);
					}
				}
				if (jointAval != null || branch.PK <= extremite.PK)
				{
					continue;
				}
				List<SIGSegment> list4 = new List<SIGSegment>(chemin);
				list4.Add(new SIGSegment(extremite.Voie, extremite, branch));
				foreach (SIGBranche item3 in item.BranchesInOtherTrack(extremite.Voie))
				{
					FindJoint(item3, ref chemins, list4, num);
				}
			}
		}
	}

	public static List<SIGJoint> FindJoints(SIGExtremite extremite, SearchDirection direction)
	{
		List<SIGJoint> joints = new List<SIGJoint>();
		FindJoints(extremite, direction, ref joints, 0);
		return joints;
	}

	private static void FindJoints(SIGExtremite extremite, SearchDirection direction, ref List<SIGJoint> joints, int profondeur)
	{
		int num = profondeur + 1;
		if (num > 5)
		{
			return;
		}
		SIGExtremite extremiteAmont = null;
		SIGExtremite extremiteAval = null;
		FindBornes(extremite, out extremiteAmont, out extremiteAval);
		if (direction != SearchDirection.Plus && extremiteAmont != null)
		{
			if (extremiteAmont is SIGJoint)
			{
				joints.Add((SIGJoint)extremiteAmont);
			}
			else
			{
				SIGBranche sIGBranche = extremiteAmont as SIGBranche;
				foreach (SIGBranche branch in sIGBranche.Noeud.Branches)
				{
					if (branch != sIGBranche && branch.Type != sIGBranche.Type)
					{
						SearchDirection direction2 = SearchDirection.Moins;
						if (branch.Voie.Ligne != extremite.Voie.Ligne)
						{
							direction2 = SearchDirection.PlusMoins;
						}
						FindJoints(branch, direction2, ref joints, num);
					}
				}
			}
		}
		if (direction == SearchDirection.Moins || extremiteAval == null)
		{
			return;
		}
		if (extremiteAval is SIGJoint)
		{
			joints.Add((SIGJoint)extremiteAval);
			return;
		}
		SIGBranche sIGBranche2 = extremiteAval as SIGBranche;
		foreach (SIGBranche branch2 in sIGBranche2.Noeud.Branches)
		{
			if (branch2 != sIGBranche2 && branch2.Type != sIGBranche2.Type)
			{
				SearchDirection direction3 = SearchDirection.Plus;
				if (branch2.Voie.Ligne != extremite.Voie.Ligne)
				{
					direction3 = SearchDirection.PlusMoins;
				}
				if (branch2.Voie == sIGBranche2.Voie && branch2.PK < sIGBranche2.PK)
				{
					FindJoints(sIGBranche2, direction3, ref joints, num);
				}
				else
				{
					FindJoints(branch2, direction3, ref joints, num);
				}
			}
		}
	}

	public static SIGJoint FindFirstJoint(SIGExtremite extremite, SearchDirection direction)
	{
		return FindFirstJoint(extremite, direction, 0);
	}

	private static SIGJoint FindFirstJoint(SIGExtremite extremite, SearchDirection direction, int profondeur)
	{
		int num = profondeur + 1;
		if (num > 5)
		{
			return null;
		}
		SIGExtremite extremiteAmont = null;
		SIGExtremite extremiteAval = null;
		FindBornes(extremite, out extremiteAmont, out extremiteAval);
		if (direction != SearchDirection.Plus && extremiteAmont != null)
		{
			if (extremiteAmont is SIGJoint)
			{
				return extremiteAmont as SIGJoint;
			}
			SIGBranche sIGBranche = extremiteAval as SIGBranche;
			foreach (SIGBranche branch in sIGBranche.Noeud.Branches)
			{
				if (branch.Type != sIGBranche.Type)
				{
					SIGJoint sIGJoint = FindFirstJoint(branch, SearchDirection.Moins, num);
					if (sIGJoint != null)
					{
						return sIGJoint;
					}
				}
			}
			foreach (SIGBranche branch2 in sIGBranche.Noeud.Branches)
			{
				if (!branch2.IsAval && branch2.Noeud == branch2.Voie.NoeudFin)
				{
					SIGJoint sIGJoint = FindFirstJoint(branch2, SearchDirection.Plus, num);
					if (sIGJoint != null)
					{
						return sIGJoint;
					}
				}
			}
		}
		if (direction != SearchDirection.Moins && extremiteAval != null)
		{
			if (extremiteAval is SIGJoint)
			{
				return (SIGJoint)extremiteAval;
			}
			SIGBranche sIGBranche2 = extremiteAval as SIGBranche;
			foreach (SIGBranche branch3 in sIGBranche2.Noeud.Branches)
			{
				if (branch3.Type != sIGBranche2.Type)
				{
					SIGJoint sIGJoint2 = FindFirstJoint(branch3, SearchDirection.Plus, num);
					if (sIGJoint2 != null)
					{
						return sIGJoint2;
					}
				}
			}
			foreach (SIGBranche branch4 in sIGBranche2.Noeud.Branches)
			{
				if (!branch4.IsAmont && branch4.Noeud == branch4.Voie.NoeudDebut)
				{
					SIGJoint sIGJoint2 = FindFirstJoint(branch4, SearchDirection.Moins, num);
					if (sIGJoint2 != null)
					{
						return sIGJoint2;
					}
				}
			}
		}
		return null;
	}

	public static List<SIGDemiJoint> FindDemiJoints(SIGExtremite extremite, SearchDirection direction)
	{
		List<SIGDemiJoint> demiJoints = new List<SIGDemiJoint>();
		FindDemiJoints(extremite, direction, ref demiJoints, 0);
		return demiJoints;
	}

	private static void FindDemiJoints(SIGExtremite extremite, SearchDirection direction, ref List<SIGDemiJoint> demiJoints, int profondeur)
	{
		int num = profondeur + 1;
		if (num > 5)
		{
			return;
		}
		SIGExtremite extremiteAmont = null;
		SIGExtremite extremiteAval = null;
		FindBornes(extremite, out extremiteAmont, out extremiteAval);
		if (direction != SearchDirection.Plus && extremiteAmont != null)
		{
			if (extremiteAmont is SIGJoint)
			{
				demiJoints.Add((extremiteAmont as SIGJoint).DemiJointAval);
			}
			else
			{
				SIGBranche sIGBranche = extremiteAmont as SIGBranche;
				foreach (SIGBranche branch in sIGBranche.Noeud.Branches)
				{
					if (branch != sIGBranche && branch.Type != sIGBranche.Type)
					{
						if (branch.Voie.Ligne != extremite.Voie.Ligne && branch.PK > sIGBranche.PK)
						{
							FindDemiJoints(sIGBranche, SearchDirection.Plus, ref demiJoints, num);
						}
						else
						{
							FindDemiJoints(branch, direction, ref demiJoints, num);
						}
					}
				}
			}
		}
		if (direction == SearchDirection.Moins || extremiteAval == null)
		{
			return;
		}
		if (extremiteAval is SIGJoint)
		{
			demiJoints.Add((extremiteAval as SIGJoint).DemiJointAmont);
			return;
		}
		SIGBranche sIGBranche2 = extremiteAval as SIGBranche;
		foreach (SIGBranche branch2 in sIGBranche2.Noeud.Branches)
		{
			if (branch2 != sIGBranche2)
			{
				if (branch2.Type == sIGBranche2.Type)
				{
					FindDemiJoints(branch2, SearchDirection.Moins, ref demiJoints, num);
				}
				else if (branch2.Voie == sIGBranche2.Voie && branch2.PK < sIGBranche2.PK)
				{
					FindDemiJoints(sIGBranche2, direction, ref demiJoints, num);
				}
				else
				{
					FindDemiJoints(branch2, direction, ref demiJoints, num);
				}
			}
		}
	}

	private static void FindJointsBornes(SIGExtremite extremite, out SIGJoint jointAmont, out SIGJoint jointAval)
	{
		SIGJoint sIGJoint = extremite.Voie.Joints.Find((SIGJoint j) => j.PK > extremite.PK);
		SIGJoint sIGJoint2 = extremite.Voie.Joints.FindLast((SIGJoint j) => j.PK < extremite.PK);
		if (sIGJoint != null && sIGJoint.PK - extremite.PK > 5000)
		{
			sIGJoint = null;
		}
		if (sIGJoint2 != null && extremite.PK - sIGJoint2.PK > 5000)
		{
			sIGJoint2 = null;
		}
		jointAmont = sIGJoint2;
		jointAval = sIGJoint;
	}

	private static void FindBornes(SIGExtremite extremite, out SIGExtremite extremiteAmont, out SIGExtremite extremiteAval)
	{
		SIGExtremite sIGExtremite = null;
		SIGExtremite sIGExtremite2 = null;
		foreach (SIGJoint joint in extremite.Voie.Joints)
		{
			if (Math.Abs(joint.PK - extremite.PK) > 5000)
			{
				continue;
			}
			if (joint.PK < extremite.PK)
			{
				if (sIGExtremite == null || joint.PK > sIGExtremite.PK)
				{
					sIGExtremite = joint;
				}
			}
			else if (joint.PK > extremite.PK && (sIGExtremite2 == null || joint.PK < sIGExtremite2.PK))
			{
				sIGExtremite2 = joint;
			}
		}
		foreach (SIGNoeud noeud in extremite.Voie.Noeuds)
		{
			if (noeud.IsSautPk || (extremite is SIGBranche && ((SIGBranche)extremite).Noeud == noeud))
			{
				continue;
			}
			foreach (SIGBranche branch in noeud.Branches)
			{
				if (branch.Voie != extremite.Voie || Math.Abs(branch.PK - extremite.PK) > 5000)
				{
					continue;
				}
				if (branch.PK < extremite.PK)
				{
					if ((branch.IsAval || noeud == extremite.Voie.NoeudDebut) && (sIGExtremite == null || branch.PK > sIGExtremite.PK))
					{
						sIGExtremite = branch;
					}
				}
				else if (branch.PK > extremite.PK && (branch.IsAmont || noeud == extremite.Voie.NoeudFin) && (sIGExtremite2 == null || branch.PK < sIGExtremite2.PK))
				{
					sIGExtremite2 = branch;
				}
			}
		}
		extremiteAmont = sIGExtremite;
		extremiteAval = sIGExtremite2;
	}
}
