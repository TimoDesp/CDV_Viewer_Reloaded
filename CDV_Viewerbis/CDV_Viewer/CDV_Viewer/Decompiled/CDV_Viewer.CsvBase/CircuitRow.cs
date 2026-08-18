using System;
using System.Globalization;
using System.IO;
using CDV_Viewer.Data.SIGObjects;

namespace CDV_Viewer.CsvBase;

public class CircuitRow : BaseRow
{
	public static readonly CircuitRow Empty = new CircuitRow();

	public string NOM;

	public CircuitType TYPE;

	public int FREQUENCE;

	public CompensationType COMPENSATION;

	public int POINTS;

	public double PAS_REEL;

	public double ICC_MIN;

	public double N_FUITE_LONG_ARR;

	public bool CALCUL_CONFORME;

	public double I_FUITE_MAX;

	public double DIAPHONIE_MAX;

	public SIGCircuit SIGCircuit { get; private set; }

	private CircuitRow()
	{
	}

	internal CircuitRow(int id)
	{
		_id = id;
	}

	internal CircuitRow(SIGCircuit circuit)
	{
		_id = circuit.ID;
		SIGCircuit = circuit;
		Update();
	}

	internal static CircuitRow FromCsv(string[] fields)
	{
		int id = BaseRow.GetId(fields);
		if (!Enum.TryParse<CircuitType>(fields[2], out var result))
		{
			result = CircuitType.NC;
		}
		if (!Enum.TryParse<CompensationType>(fields[4], out var result2))
		{
			result2 = CompensationType.NON;
		}
		CircuitRow circuitRow = new CircuitRow(id)
		{
			NOM = fields[1],
			FREQUENCE = Convert.ToInt32(fields[3]),
			POINTS = Convert.ToInt32(fields[5]),
			PAS_REEL = Convert.ToDouble(fields[6], CultureInfo.InvariantCulture),
			ICC_MIN = Convert.ToDouble(fields[7], CultureInfo.InvariantCulture),
			N_FUITE_LONG_ARR = Convert.ToDouble(fields[8], CultureInfo.InvariantCulture),
			CALCUL_CONFORME = (fields[9] == "1"),
			I_FUITE_MAX = Convert.ToDouble(fields[10], CultureInfo.InvariantCulture),
			DIAPHONIE_MAX = Convert.ToDouble(fields[11], CultureInfo.InvariantCulture),
			TYPE = result,
			COMPENSATION = result2
		};
		circuitRow.SIGCircuit = new SIGCircuit(id)
		{
			Nom = circuitRow.NOM,
			Type = circuitRow.TYPE,
			Frequence = circuitRow.FREQUENCE,
			Compensation = circuitRow.COMPENSATION,
			NbPtsCompensation = circuitRow.POINTS,
			PasReel = circuitRow.PAS_REEL,
			ICC = circuitRow.ICC_MIN,
			N_FUITE_LONG_ARR = circuitRow.N_FUITE_LONG_ARR,
			CALCUL_CONFORME = circuitRow.CALCUL_CONFORME,
			IFuite = circuitRow.I_FUITE_MAX,
			Diaphonie = circuitRow.DIAPHONIE_MAX
		};
		return circuitRow;
	}

	internal static CircuitRow FromBinary(BinaryReader reader)
	{
		int id = BaseRow.ReadId(reader);
		CircuitRow circuitRow = new CircuitRow(id)
		{
			NOM = BaseRow.ReadAsciiString(reader),
			TYPE = (CircuitType)reader.ReadSByte(),
			FREQUENCE = reader.ReadInt16(),
			COMPENSATION = (CompensationType)reader.ReadSByte(),
			POINTS = reader.ReadInt16(),
			PAS_REEL = reader.ReadSingle(),
			ICC_MIN = reader.ReadSingle(),
			N_FUITE_LONG_ARR = reader.ReadSingle(),
			CALCUL_CONFORME = reader.ReadBoolean(),
			I_FUITE_MAX = reader.ReadSingle(),
			DIAPHONIE_MAX = reader.ReadSingle()
		};
		circuitRow.SIGCircuit = new SIGCircuit(id)
		{
			Nom = circuitRow.NOM,
			Type = circuitRow.TYPE,
			Frequence = circuitRow.FREQUENCE,
			Compensation = circuitRow.COMPENSATION,
			NbPtsCompensation = circuitRow.POINTS,
			PasReel = circuitRow.PAS_REEL,
			ICC = circuitRow.ICC_MIN,
			N_FUITE_LONG_ARR = circuitRow.N_FUITE_LONG_ARR,
			CALCUL_CONFORME = circuitRow.CALCUL_CONFORME,
			IFuite = circuitRow.I_FUITE_MAX,
			Diaphonie = circuitRow.DIAPHONIE_MAX
		};
		return circuitRow;
	}

	internal bool Update()
	{
		if (SIGCircuit.ID != _id)
		{
			return false;
		}
		NOM = SIGCircuit.Nom;
		TYPE = SIGCircuit.Type;
		FREQUENCE = SIGCircuit.Frequence;
		COMPENSATION = SIGCircuit.Compensation;
		POINTS = SIGCircuit.NbPtsCompensation;
		PAS_REEL = SIGCircuit.PasReel;
		ICC_MIN = SIGCircuit.ICC;
		N_FUITE_LONG_ARR = SIGCircuit.N_FUITE_LONG_ARR;
		CALCUL_CONFORME = SIGCircuit.CALCUL_CONFORME;
		I_FUITE_MAX = SIGCircuit.IFuite;
		DIAPHONIE_MAX = SIGCircuit.Diaphonie;
		return true;
	}

	public override string ToCsv()
	{
		return base.ID.ToString() + ";" + NOM + ";" + TYPE.ToString() + ";" + FREQUENCE + ";" + COMPENSATION.ToString() + ";" + POINTS + ";" + PAS_REEL.ToString(CultureInfo.InvariantCulture) + ";" + ICC_MIN.ToString(CultureInfo.InvariantCulture) + ";" + N_FUITE_LONG_ARR.ToString(CultureInfo.InvariantCulture) + ";" + (CALCUL_CONFORME ? "1" : "0") + ";" + I_FUITE_MAX.ToString(CultureInfo.InvariantCulture) + ";" + DIAPHONIE_MAX.ToString(CultureInfo.InvariantCulture);
	}

	public override void ToBinaryStream(BinaryWriter writer)
	{
		BaseRow.WriteId(writer, base.ID);
		BaseRow.WriteAsciiString(writer, NOM);
		writer.Write((sbyte)TYPE);
		writer.Write((short)FREQUENCE);
		writer.Write((sbyte)COMPENSATION);
		writer.Write((short)POINTS);
		writer.Write((float)PAS_REEL);
		writer.Write((float)ICC_MIN);
		writer.Write((float)N_FUITE_LONG_ARR);
		writer.Write(CALCUL_CONFORME);
		writer.Write((float)I_FUITE_MAX);
		writer.Write((float)DIAPHONIE_MAX);
	}

	public override string ToString()
	{
		return $"[id={base.ID}]  {NOM} {TYPE}:{FREQUENCE}hz {COMPENSATION}";
	}
}
