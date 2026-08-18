using System.Collections.Generic;
using System.IO;
using System.Linq;
using CDV_Viewer.Data.SIGObjects;

namespace CDV_Viewer.CsvBase;

public class TableModeles : BaseTable<ModeleRow>
{
	public override string Path { get; protected set; } = "signalo\\modeles.csv";

	public override string Header => "ID;CIRCUIT;JOINT_E;JOINT_S;TOURNEE;POINTS;STAMP";

	public override void Link()
	{
		foreach (ModeleRow m in _items.Values)
		{
			SIGModele sIGModele = m.SIGModele;
			SIGCircuit sIGCircuit = Base.CsvCircuits[m.CIRCUIT].SIGCircuit;
			_ = sIGCircuit.ID;
			_ = m.CIRCUIT;
			sIGModele.Circuit = sIGCircuit;
			sIGModele.DemiJointE = sIGCircuit.DemiJoints.Find((SIGDemiJoint demiJoint) => demiJoint.Joint.ID == m.JOINT_E);
			sIGModele.DemiJointS = sIGCircuit.DemiJoints.Find((SIGDemiJoint demiJoint) => demiJoint.Joint.ID == m.JOINT_S);
			_ = sIGModele.DemiJointE;
			_ = sIGModele.DemiJointS;
			sIGCircuit.Modeles.Add(sIGModele);
		}
	}

	public TableModeles()
	{
	}

	public TableModeles(string fullName)
	{
		Path = System.IO.Path.GetFileName(fullName);
		TempBaseDirectory = System.IO.Path.GetDirectoryName(fullName);
	}

	public override ModeleRow RowFromCsv(string[] csvFields)
	{
		return ModeleRow.FromCsv(csvFields);
	}

	public override ModeleRow RowFromBinary(BinaryReader reader)
	{
		return ModeleRow.FromBinary(reader);
	}

	internal void ReplaceModeles(List<ModeleRow> modeles)
	{
		int num = 0;
		foreach (ModeleRow modele in modeles)
		{
			num++;
			modele.SetId(num);
		}
		_items = new SortedDictionary<int, ModeleRow>(modeles.ToDictionary((ModeleRow m) => m.ID));
		Base.NeedLink();
	}

	internal List<SIGModele> SigModeles()
	{
		return _items.Values.Select((ModeleRow m) => m.SIGModele).ToList();
	}
}
