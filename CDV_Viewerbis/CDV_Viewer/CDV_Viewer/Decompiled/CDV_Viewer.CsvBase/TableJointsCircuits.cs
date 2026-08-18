using System.Collections.Generic;
using System.IO;
using System.Linq;
using CDV_Viewer.Data.SIGObjects;
using CDV_Viewer.Traitements;

namespace CDV_Viewer.CsvBase;

public class TableJointsCircuits : BaseTable<JointCircuitRow>
{
	private Dictionary<int, int[]> _idCircuits = new Dictionary<int, int[]>();

	private Dictionary<int, int[]> _idJoints = new Dictionary<int, int[]>();

	public override string Path => "signalo\\joints_circuits.csv";

	public override string Header => "ID;JOINT;CIRCUIT;PRINCIPAL;DB;EMETTEUR;DEMI_PAS";

	public override JointCircuitRow RowFromCsv(string[] csvFields)
	{
		return JointCircuitRow.FromCsv(csvFields);
	}

	public override JointCircuitRow RowFromBinary(BinaryReader reader)
	{
		return JointCircuitRow.FromBinary(reader);
	}

	public override void Link()
	{
		_idJoints = (from d in _items.Values
			group d by d.CIRCUIT).ToDictionary((IGrouping<int, JointCircuitRow> g) => g.Key, (IGrouping<int, JointCircuitRow> g) => g.Select((JointCircuitRow r) => r.JOINT).ToArray());
		_idCircuits = (from d in _items.Values
			group d by d.JOINT).ToDictionary((IGrouping<int, JointCircuitRow> g) => g.Key, (IGrouping<int, JointCircuitRow> g) => g.Select((JointCircuitRow r) => r.CIRCUIT).ToArray());
		foreach (JointCircuitRow value in _items.Values)
		{
			SIGDemiJoint sIGDemiJoint = value.SIGDemiJoint;
			sIGDemiJoint.UnLink();
			SIGCircuit sIGCircuit = Base.CsvCircuits[value.CIRCUIT].SIGCircuit;
			SIGJoint sIGJoint = (sIGDemiJoint.Joint = Base.CsvJoints[value.JOINT].SIGJoint);
			sIGDemiJoint.Circuit = sIGCircuit;
			if (!sIGJoint.AddDemiJoint(sIGDemiJoint))
			{
				Dialogs.BaseError("Erreur un base !! joint contenant plus de 2 demi joints", sIGJoint.Voie, sIGJoint.PK);
			}
			else
			{
				sIGCircuit.DemiJoints.Add(sIGDemiJoint);
			}
		}
		foreach (CircuitRow csvCircuit in Base.CsvCircuits)
		{
			csvCircuit.SIGCircuit.DemiJoints.Sort();
		}
		foreach (LigneRow csvLigne in Base.CsvLignes)
		{
			SigDictionary<SIGCircuit> sigDictionary = new SigDictionary<SIGCircuit>();
			foreach (SIGVoie voie in csvLigne.SIGLigne.Voies)
			{
				foreach (SIGJoint joint in voie.Joints)
				{
					foreach (SIGDemiJoint demiJoint in joint._demiJoints)
					{
						sigDictionary[demiJoint.Circuit.ID] = demiJoint.Circuit;
					}
				}
			}
			csvLigne.SIGLigne.Circuits = sigDictionary.ToList();
		}
		foreach (JointRow csvJoint in Base.CsvJoints)
		{
			csvJoint.SIGJoint.SetDemiJointAmont();
			csvJoint.SIGJoint.SetDemiJointAval();
		}
		foreach (CircuitRow csvCircuit2 in Base.CsvCircuits)
		{
			_ = csvCircuit2.ID;
			_ = 1784;
			_ = csvCircuit2.SIGCircuit.DemiJointDebut;
			_ = csvCircuit2.SIGCircuit.DemiJointFin;
		}
	}

	public int Create(SIGDemiJoint demiJoint)
	{
		int result = (demiJoint.ID = FreeId());
		JointCircuitRow row = new JointCircuitRow(demiJoint);
		Add(row);
		return result;
	}

	public List<JointCircuitRow> FromJoint(int idJoint)
	{
		return _items.Values.Where((JointCircuitRow d) => d.JOINT == idJoint).ToList();
	}

	public List<int> CircuitsIds(int idJoint)
	{
		return FromJoint(idJoint).ConvertAll((JointCircuitRow jc) => jc.CIRCUIT);
	}

	internal void Update(SIGDemiJoint demiJoint)
	{
		if ((demiJoint?.ID ?? (-1)) >= 0)
		{
			base[demiJoint.ID].Update();
			Base.NeedLink();
		}
	}
}
