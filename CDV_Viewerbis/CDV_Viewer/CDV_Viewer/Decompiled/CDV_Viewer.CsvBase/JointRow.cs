using System;
using System.Globalization;
using System.IO;
using CDV_Viewer.Data.SIGObjects;

namespace CDV_Viewer.CsvBase;

public class JointRow : BaseRow
{
	public int VOIE;

	public int PK;

	public JointType TYPE;

	public double DEMI_LONGUEUR;

	public SIGJoint SIGJoint { get; private set; }

	private JointRow()
	{
	}

	internal JointRow(int id)
	{
		_id = id;
	}

	internal JointRow(SIGJoint sigJoint)
	{
		_id = sigJoint.ID;
		SIGJoint = sigJoint;
		Update();
	}

	public static JointRow FromCsv(string[] fields)
	{
		int id = BaseRow.GetId(fields);
		if (!Enum.TryParse<JointType>(fields[3], out var result))
		{
			result = JointType.INC;
		}
		JointRow jointRow = new JointRow(id)
		{
			VOIE = Convert.ToInt32(fields[1]),
			PK = Convert.ToInt32(fields[2]),
			DEMI_LONGUEUR = Convert.ToDouble(fields[4], CultureInfo.InvariantCulture),
			TYPE = result
		};
		jointRow.SIGJoint = new SIGJoint(id)
		{
			PK = jointRow.PK,
			Type = result,
			DemiLongueur = jointRow.DEMI_LONGUEUR
		};
		return jointRow;
	}

	public static JointRow FromBinary(BinaryReader reader)
	{
		int id = BaseRow.ReadId(reader);
		JointRow jointRow = new JointRow(id)
		{
			VOIE = BaseRow.ReadId(reader),
			PK = BaseRow.ReadPK(reader),
			TYPE = (JointType)reader.ReadSByte(),
			DEMI_LONGUEUR = reader.ReadSingle()
		};
		jointRow.SIGJoint = new SIGJoint(id)
		{
			PK = jointRow.PK,
			Type = jointRow.TYPE,
			DemiLongueur = jointRow.DEMI_LONGUEUR
		};
		return jointRow;
	}

	internal bool Update()
	{
		if (SIGJoint.ID != _id)
		{
			return false;
		}
		VOIE = SIGJoint.Voie.ID;
		PK = SIGJoint.PK;
		TYPE = SIGJoint.Type;
		DEMI_LONGUEUR = SIGJoint.DemiLongueur;
		return true;
	}

	public override string ToCsv()
	{
		return base.ID.ToString() + ";" + VOIE + ";" + PK + ";" + TYPE.ToString() + ";" + DEMI_LONGUEUR.ToString(CultureInfo.InvariantCulture);
	}

	public override void ToBinaryStream(BinaryWriter writer)
	{
		BaseRow.WriteId(writer, base.ID);
		BaseRow.WriteId(writer, VOIE);
		BaseRow.WritePK(writer, PK);
		writer.Write((sbyte)TYPE);
		writer.Write((float)DEMI_LONGUEUR);
	}

	public override string ToString()
	{
		return $"[id={base.ID}]  {TYPE} idVoie={VOIE} PK={(double)PK / 1000.0:0.###}";
	}
}
