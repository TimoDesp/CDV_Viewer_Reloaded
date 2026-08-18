using System;
using System.Collections.Generic;
using CDV_Viewer.Traitements;

namespace CDV_Viewer.Data.SIGObjects;

public class SIGSegment
{
	public SIGVoie Voie;

	public SIGExtremite ExtremiteD;

	public SIGExtremite ExtremiteF;

	public int PkD => ExtremiteD.PK;

	public int PkF => ExtremiteF.PK;

	public bool HasOnlyBranches
	{
		get
		{
			if (ExtremiteD is SIGBranche)
			{
				return ExtremiteF is SIGBranche;
			}
			return false;
		}
	}

	public bool HasOnlyJoints
	{
		get
		{
			if (ExtremiteD is SIGJoint)
			{
				return ExtremiteF is SIGJoint;
			}
			return false;
		}
	}

	public bool StartWithJoint => ExtremiteD is SIGJoint;

	public bool EndWithJoint => ExtremiteF is SIGJoint;

	public bool StartWithBranche => ExtremiteD is SIGBranche;

	public bool EndWithBranche => ExtremiteF is SIGBranche;

	public SIGSegment(SIGVoie voie, SIGExtremite extremiteD, SIGExtremite extremiteF)
	{
		Voie = voie;
		ExtremiteD = extremiteD;
		ExtremiteF = extremiteF;
	}

	public static void Merge(ref List<SIGSegment> segments)
	{
		for (int i = 0; i < segments.Count; i++)
		{
			if (segments[i].PkD > segments[i].PkF)
			{
				SIGExtremite extremiteD = segments[i].ExtremiteD;
				segments[i].ExtremiteD = segments[i].ExtremiteF;
				segments[i].ExtremiteF = extremiteD;
			}
		}
		for (int j = 0; j < segments.Count; j++)
		{
			SIGSegment sIGSegment;
			if ((sIGSegment = segments[j]) == null)
			{
				continue;
			}
			_ = sIGSegment.ExtremiteD;
			_ = sIGSegment.ExtremiteF;
			for (int k = j + 1; k < segments.Count; k++)
			{
				SIGSegment sIGSegment2;
				if ((sIGSegment2 = segments[k]) != null && sIGSegment.Voie == sIGSegment2.Voie)
				{
					_ = sIGSegment2.ExtremiteD;
					_ = sIGSegment2.ExtremiteF;
					int pkD = sIGSegment.PkD;
					int pkF = sIGSegment.PkF;
					int pkD2 = sIGSegment2.PkD;
					int pkF2 = sIGSegment2.PkF;
					if (pkD <= pkD2 && pkF >= pkF2)
					{
						segments[k] = null;
					}
					else if (pkD2 <= pkD && pkF2 >= pkF)
					{
						segments[j].ExtremiteD = segments[k].ExtremiteD;
						segments[j].ExtremiteF = segments[k].ExtremiteF;
						segments[k] = null;
					}
					else if (pkD <= pkF2 && pkF > pkF2)
					{
						SIGExtremite.HaveSameNode(sIGSegment.ExtremiteD, sIGSegment2.ExtremiteF);
						segments[j].ExtremiteD = segments[k].ExtremiteD;
						segments[k] = null;
					}
					else if (pkD2 <= pkF && pkF2 > pkF)
					{
						SIGExtremite.HaveSameNode(sIGSegment.ExtremiteF, sIGSegment2.ExtremiteD);
						segments[j].ExtremiteF = segments[k].ExtremiteF;
						segments[k] = null;
					}
				}
			}
		}
		segments = segments.FindAll((SIGSegment s) => s != null);
	}

	public static int GetLongueur(List<SIGSegment> segments)
	{
		int num = 0;
		foreach (SIGSegment segment in segments)
		{
			num += Math.Abs(segment.PkF - segment.PkD);
		}
		return num;
	}

	public static List<SIGSegment> GetParcours(SIGCircuit circuit)
	{
		List<SIGDemiJoint> demiJoints = circuit.DemiJoints;
		int count = circuit.DemiJoints.Count;
		List<SIGSegment> segments = new List<SIGSegment>();
		for (int i = 0; i < count; i++)
		{
			for (int j = i + 1; j < count; j++)
			{
				List<SIGSegment> list = CDV_Viewer.Traitements.Composants.FindParcours(demiJoints[i], demiJoints[j]);
				if (list.Count > 0)
				{
					segments.AddRange(list);
				}
			}
		}
		Merge(ref segments);
		return segments;
	}

	public override string ToString()
	{
		return $"({(double)PkD / 1000.0:0.000}) - {Voie} - ({(double)PkF / 1000.0:0.000})";
	}
}
