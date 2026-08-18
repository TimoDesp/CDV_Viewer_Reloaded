using System;
using System.Globalization;
using System.IO;
using CDV_Viewer.Data.SIGObjects;

namespace CDV_Viewer.CsvBase;

public class JointCircuitRow : BaseRow
{
	public static readonly JointCircuitRow EmptyRow = new JointCircuitRow();

	public int JOINT;

	public int CIRCUIT;

	public bool PRINCIPAL;

	public bool DB;

	public bool EMETTEUR;

	public double DEMI_PAS;

	public SIGDemiJoint SIGDemiJoint { get; private set; }

	public JointCircuitRow()
	{
	}

	internal JointCircuitRow(int id)
	{
		_id = id;
	}

	public JointCircuitRow(SIGDemiJoint demiJoint)
		: this(demiJoint.ID)
	{
		_id = demiJoint.ID;
		SIGDemiJoint = demiJoint;
		Update();
	}

	public JointCircuitRow(SIGLinkJointCircuit link)
		: this(link.ID)
	{
		JOINT = link.Joint;
		PRINCIPAL = link.Principal;
		DB = (EMETTEUR = false);
		DEMI_PAS = 0.0;
		CIRCUIT = link.Circuit;
	}

	public static JointCircuitRow FromCsv(string[] fields)
	{
		int id = BaseRow.GetId(fields);
		JointCircuitRow jointCircuitRow = new JointCircuitRow(id)
		{
			JOINT = Convert.ToInt32(fields[1]),
			CIRCUIT = Convert.ToInt32(fields[2]),
			PRINCIPAL = Convert.ToBoolean(fields[3]),
			DB = Convert.ToBoolean(fields[4]),
			EMETTEUR = Convert.ToBoolean(fields[5]),
			DEMI_PAS = Convert.ToDouble(fields[6], CultureInfo.InvariantCulture)
		};
		jointCircuitRow.SIGDemiJoint = new SIGDemiJoint(id)
		{
			Principal = jointCircuitRow.PRINCIPAL,
			DB = jointCircuitRow.DB,
			Emetteur = jointCircuitRow.EMETTEUR,
			DemiPas = jointCircuitRow.DEMI_PAS
		};
		return jointCircuitRow;
	}

	public static JointCircuitRow FromBinary(BinaryReader reader)
	{
		int id = BaseRow.ReadId(reader);
		JointCircuitRow jointCircuitRow = new JointCircuitRow(id)
		{
			JOINT = BaseRow.ReadId(reader),
			CIRCUIT = BaseRow.ReadId(reader),
			PRINCIPAL = reader.ReadBoolean(),
			DB = reader.ReadBoolean(),
			EMETTEUR = reader.ReadBoolean(),
			DEMI_PAS = reader.ReadSingle()
		};
		jointCircuitRow.SIGDemiJoint = new SIGDemiJoint(id)
		{
			Principal = jointCircuitRow.PRINCIPAL,
			DB = jointCircuitRow.DB,
			Emetteur = jointCircuitRow.EMETTEUR,
			DemiPas = jointCircuitRow.DEMI_PAS
		};
		return jointCircuitRow;
	}

	internal bool Update()
	{
		if (SIGDemiJoint.ID != _id)
		{
			return false;
		}
		bool result = JOINT != SIGDemiJoint.Joint.ID || CIRCUIT != SIGDemiJoint.Circuit.ID;
		PRINCIPAL = SIGDemiJoint.Principal;
		DB = SIGDemiJoint.DB;
		EMETTEUR = SIGDemiJoint.Emetteur;
		DEMI_PAS = SIGDemiJoint.DemiPas;
		JOINT = SIGDemiJoint.Joint.ID;
		CIRCUIT = SIGDemiJoint.Circuit.ID;
		return result;
	}

	public override string ToCsv()
	{
		return base.ID.ToString() + ";" + JOINT + ";" + CIRCUIT + ";" + PRINCIPAL.ToString() + ";" + DB.ToString() + ";" + EMETTEUR.ToString() + ";" + DEMI_PAS.ToString(CultureInfo.InvariantCulture);
	}

	public override void ToBinaryStream(BinaryWriter writer)
	{
		BaseRow.WriteId(writer, base.ID);
		BaseRow.WriteId(writer, JOINT);
		BaseRow.WriteId(writer, CIRCUIT);
		writer.Write(PRINCIPAL);
		writer.Write(DB);
		writer.Write(EMETTEUR);
		writer.Write((float)DEMI_PAS);
	}
}
