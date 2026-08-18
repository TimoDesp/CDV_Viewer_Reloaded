using System.Collections.Generic;
using CDV_Viewer.CsvBase;
using CDV_Viewer.Traitements;

namespace CDV_Viewer.Data.SIGObjects;

public class SIGJoint : SIGExtremite
{
	public int ID = -1;

	public JointType Type;

	public double DemiLongueur;

	internal List<SIGDemiJoint> _demiJoints = new List<SIGDemiJoint>(2);

	internal SIGDemiJoint _demiJointAmont;

	internal SIGDemiJoint _demiJointAval;

	public SIGDemiJoint DemiJointAmont => _demiJointAmont;

	public SIGDemiJoint DemiJointAval => _demiJointAval;

	public bool HasCircuitAmont => DemiJointAmont?.Circuit != null;

	public bool HasCircuitAval => DemiJointAval?.Circuit != null;

	public bool HasLinkedCircuit
	{
		get
		{
			if (_demiJoints.Count == 0)
			{
				return false;
			}
			foreach (SIGDemiJoint demiJoint in _demiJoints)
			{
				if (demiJoint.Circuit != null)
				{
					return true;
				}
			}
			return false;
		}
	}

	internal bool SetDemiJointAmont()
	{
		if (!HasLinkedCircuit)
		{
			return false;
		}
		List<SIGJoint> list = FindJointsAmont();
		SIGDemiJoint sIGDemiJoint = null;
		foreach (SIGJoint item in list)
		{
			foreach (SIGDemiJoint demiJoint in item._demiJoints)
			{
				foreach (SIGDemiJoint demiJoint2 in _demiJoints)
				{
					if (demiJoint.Circuit.ID == demiJoint2.Circuit.ID)
					{
						sIGDemiJoint = demiJoint2;
						goto end_IL_00a1;
					}
				}
			}
			continue;
			end_IL_00a1:
			break;
		}
		if (sIGDemiJoint != null)
		{
			if (_demiJointAmont == null)
			{
				_demiJointAmont = sIGDemiJoint;
			}
			else if (_demiJointAmont != sIGDemiJoint)
			{
				Dialogs.BaseError("Probleme de liaison de joint", Voie, PK);
			}
			if (_demiJoints.Count == 2)
			{
				SIGDemiJoint sIGDemiJoint2 = ((_demiJoints[0] == sIGDemiJoint) ? _demiJoints[1] : _demiJoints[0]);
				if (_demiJointAval == null)
				{
					_demiJointAval = sIGDemiJoint2;
				}
				else if (_demiJointAval != sIGDemiJoint2)
				{
					Dialogs.BaseError("Probleme de liaison de joint", Voie, PK);
				}
			}
			return true;
		}
		return false;
	}

	internal bool SetDemiJointAval()
	{
		if (!HasLinkedCircuit)
		{
			return false;
		}
		SIGDemiJoint sIGDemiJoint = null;
		foreach (SIGJoint item in FindJointsAval())
		{
			foreach (SIGDemiJoint demiJoint in item._demiJoints)
			{
				foreach (SIGDemiJoint demiJoint2 in _demiJoints)
				{
					if (demiJoint.Circuit.ID == demiJoint2.Circuit.ID)
					{
						sIGDemiJoint = demiJoint2;
						goto end_IL_00a1;
					}
				}
			}
			continue;
			end_IL_00a1:
			break;
		}
		if (sIGDemiJoint != null)
		{
			if (_demiJointAval == null)
			{
				_demiJointAval = sIGDemiJoint;
			}
			else if (_demiJointAval != sIGDemiJoint)
			{
				Dialogs.BaseError("Probleme de liaison de joint", Voie, PK);
			}
			if (_demiJoints.Count == 2)
			{
				SIGDemiJoint sIGDemiJoint2 = ((_demiJoints[0] == sIGDemiJoint) ? _demiJoints[1] : _demiJoints[0]);
				if (_demiJointAmont == null)
				{
					_demiJointAmont = sIGDemiJoint2;
				}
				else if (_demiJointAmont != sIGDemiJoint2)
				{
					Dialogs.BaseError("Probleme de liaison de joint", Voie, PK);
				}
			}
			return true;
		}
		return false;
	}

	public bool IsLinked(SIGCircuit circuit)
	{
		if (DemiJointAmont?.Circuit == circuit)
		{
			return true;
		}
		return DemiJointAval?.Circuit == circuit;
	}

	public SIGDemiJoint DemiJoint(SIGCircuit circuit)
	{
		if (DemiJointAmont?.Circuit == circuit)
		{
			return DemiJointAmont;
		}
		if (DemiJointAval?.Circuit == circuit)
		{
			return DemiJointAval;
		}
		return null;
	}

	public SIGJoint()
	{
	}

	public SIGJoint(int id)
	{
		ID = id;
	}

	internal bool AddDemiJoint(SIGDemiJoint demijoint)
	{
		if (demijoint == null)
		{
			return false;
		}
		if (_demiJoints.Count > 1)
		{
			return false;
		}
		_demiJoints.Add(demijoint);
		return true;
	}

	internal void Unlink()
	{
		_demiJointAval = (_demiJointAmont = null);
		_demiJoints.Clear();
	}

	public override string ToString()
	{
		return $"{Type} sur " + base.ToString();
	}

	internal object Circuits()
	{
		return _demiJoints.ConvertAll((SIGDemiJoint d) => d.Circuit);
	}
}
