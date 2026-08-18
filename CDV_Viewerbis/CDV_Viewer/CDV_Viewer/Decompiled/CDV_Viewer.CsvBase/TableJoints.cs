using System.Collections.Generic;
using System.IO;
using System.Linq;
using CDV_Viewer.Data.SIGObjects;

namespace CDV_Viewer.CsvBase;

public class TableJoints : BaseTable<JointRow>
{
	public override string Path => "signalo\\joints.csv";

	public override string Header => "ID;VOIE;PK;TYPE;DEMI_LONGUEUR";

	public override JointRow RowFromCsv(string[] csvFields)
	{
		return JointRow.FromCsv(csvFields);
	}

	public override JointRow RowFromBinary(BinaryReader reader)
	{
		return JointRow.FromBinary(reader);
	}

	public override void Link()
	{
		LinkVoies();
	}

	private void LinkVoies()
	{
		foreach (JointRow value in _items.Values)
		{
			SIGJoint sIGJoint = value.SIGJoint;
			sIGJoint.Unlink();
			SIGVoie sIGVoie = Base.CsvVoies[value.VOIE].SIGVoie;
			sIGVoie.Joints.Add(sIGJoint);
			sIGJoint.Voie = sIGVoie;
		}
		foreach (VoieRow csvVoie in Base.CsvVoies)
		{
			csvVoie.SIGVoie.Joints.Sort((SIGJoint j1, SIGJoint j2) => j1.PK.CompareTo(j2.PK));
		}
	}

	public int Create(SIGJoint joint)
	{
		int result = (joint.ID = FreeId());
		JointRow row = new JointRow(joint);
		Add(row);
		return result;
	}

	internal List<SIGJoint> SigJoints()
	{
		return _items.Values.Select((JointRow j) => j.SIGJoint).ToList();
	}
}
