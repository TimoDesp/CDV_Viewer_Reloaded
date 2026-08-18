using System;
using CDV_Viewer.CsvBase;
using CDV_Viewer.Data.SIGObjects;

namespace CDV_Viewer.Data;

public class CircuitTheorique
{
	public CircuitType Type;

	public int Frequence;

	public bool ChoixCompensation;

	public CompensationType TypeCompensation;

	public int PasTh;

	public int IFuite;

	public int Diaphonie;

	public int ICCMin;

	public int N_FUITE_LONG_ARR;

	public CircuitTheorique(CircuitType type, int frequence, bool choixCompensation, CompensationType typeCompensation, int pasTh, int iFuite, int diaphonie, int iccMin, int n_fuite_long_arr)
	{
		Type = type;
		Frequence = frequence;
		ChoixCompensation = choixCompensation;
		TypeCompensation = typeCompensation;
		PasTh = pasTh;
		IFuite = iFuite;
		Diaphonie = diaphonie;
		ICCMin = iccMin;
		N_FUITE_LONG_ARR = n_fuite_long_arr;
		CircuitsTheoriques.Add(this);
	}

	public static bool Exist(CompensationType typeCompensation, int frequence)
	{
		if (frequence < 1700 && typeCompensation != CompensationType.NON)
		{
			return false;
		}
		if (frequence > 2600 && typeCompensation != CompensationType.NON)
		{
			return false;
		}
		return true;
	}

	public static bool CanBeCompensed(CircuitType typeCircuit, int frequence)
	{
		return typeCircuit switch
		{
			CircuitType.ITE => false, 
			CircuitType.NC => false, 
			_ => true, 
		};
	}

	public static CompensationType DefaultCompensation(CircuitType typeCircuit)
	{
		switch (typeCircuit)
		{
		case CircuitType.NC:
		case CircuitType.ITE:
			return CompensationType.NON;
		case CircuitType.TVM300:
			return CompensationType.P_CONSTANT;
		default:
			return CompensationType.P_VARIABLE;
		}
	}

	public static CircuitType GetDefaultType(SIGDemiJoint[] demiJoints)
	{
		foreach (SIGDemiJoint sIGDemiJoint in demiJoints)
		{
			SIGDemiJoint demiJointAmont = sIGDemiJoint.Joint.DemiJointAmont;
			if (demiJointAmont == sIGDemiJoint)
			{
				demiJointAmont = sIGDemiJoint.Joint.DemiJointAmont;
			}
			if (demiJointAmont?.Circuit != null)
			{
				return demiJointAmont.Circuit.Type;
			}
		}
		return CircuitType.NC;
	}

	public static int GetDefaultFrequency(CircuitType typeCircuit, SIGDemiJoint[] demiJoints)
	{
		if (typeCircuit == CircuitType.NC || typeCircuit == CircuitType.ITE)
		{
			return 0;
		}
		foreach (SIGDemiJoint sIGDemiJoint in demiJoints)
		{
			SIGDemiJoint demiJointAmont = sIGDemiJoint.Joint.DemiJointAmont;
			if (demiJointAmont == sIGDemiJoint)
			{
				demiJointAmont = sIGDemiJoint.Joint.DemiJointAmont;
			}
			int num = demiJointAmont?.Circuit?.Frequence ?? 0;
			if (num != 0)
			{
				return OppositeFrequency(num);
			}
		}
		return 1700;
	}

	public static int OppositeFrequency(int f)
	{
		return f switch
		{
			1700 => 2000, 
			2000 => 1700, 
			2300 => 2600, 
			2600 => 2300, 
			_ => 0, 
		};
	}

	public static int PasTheorique(CompensationType typeCompensation, int frequence)
	{
		switch (typeCompensation)
		{
		case CompensationType.NON:
			return 0;
		case CompensationType.P_CONSTANT:
			return 100;
		default:
			if (frequence >= 2150)
			{
				return 80;
			}
			return 60;
		}
	}

	public static int FuiteMax(CircuitType typeCircuit)
	{
		switch (typeCircuit)
		{
		case CircuitType.NC:
		case CircuitType.ITE:
			return 0;
		case CircuitType.TVM300:
			return 100;
		default:
			return 160;
		}
	}

	public static int DiaphonieMax(CircuitType typeCircuit)
	{
		switch (typeCircuit)
		{
		case CircuitType.NC:
		case CircuitType.ITE:
			return 0;
		case CircuitType.TVM300:
			return 200;
		default:
			return 320;
		}
	}

	public static int IccMin(CircuitType typeCircuit, int frequence)
	{
		switch (typeCircuit)
		{
		case CircuitType.NC:
		case CircuitType.ITE:
			return 0;
		case CircuitType.TVM300:
			if (frequence != 2600)
			{
				return 500;
			}
			return 450;
		default:
			return 800;
		}
	}

	public static int FuiteArriereMax(CircuitType typeCircuit)
	{
		switch (typeCircuit)
		{
		case CircuitType.NC:
		case CircuitType.ITE:
			return 0;
		case CircuitType.TVM300:
			return 200;
		default:
			return 320;
		}
	}

	public static object[] ValidFrequencies(CircuitType typeCircuit)
	{
		return typeCircuit switch
		{
			CircuitType.ITE => new object[1] { 3 }, 
			CircuitType.NC => new object[1] { 0 }, 
			_ => new object[4] { 1700, 2000, 2300, 2600 }, 
		};
	}

	public static bool ValidFrequency(CircuitType typeCircuit, int frequency)
	{
		switch (typeCircuit)
		{
		case CircuitType.ITE:
			return frequency == 3;
		case CircuitType.NC:
			return frequency == 0;
		default:
			switch (frequency)
			{
			case 1700:
			case 2000:
			case 2300:
			case 2600:
				return true;
			default:
				return false;
			}
		}
	}

	public static int GetNbPointsCompensation(CompensationType compensation, double longueurUtile, int pasTh)
	{
		switch (compensation)
		{
		case CompensationType.P_CONSTANT:
			if (longueurUtile < 60.0)
			{
				return 0;
			}
			if (longueurUtile < 160.0)
			{
				return 1;
			}
			return (int)Math.Floor((longueurUtile + 40.0) / (double)pasTh);
		case CompensationType.P_VARIABLE:
		{
			int num = 0;
			if (longueurUtile < (double)(pasTh * 2 / 3))
			{
				return 0;
			}
			num = 1;
			if (longueurUtile > (double)(pasTh / 2))
			{
				num = (int)Math.Round(longueurUtile / (double)pasTh);
			}
			return num;
		}
		default:
			return 0;
		}
	}

	public static double GetPas(CompensationType compensation, double longueurUtile, int pasTh, int nbPtsCompensation)
	{
		switch (compensation)
		{
		case CompensationType.P_CONSTANT:
			if (nbPtsCompensation <= 1)
			{
				return 0.0;
			}
			if (nbPtsCompensation > 3)
			{
				return pasTh;
			}
			return longueurUtile / (double)nbPtsCompensation;
		case CompensationType.P_VARIABLE:
			if (nbPtsCompensation == 1)
			{
				return 0.0;
			}
			if (nbPtsCompensation == 2)
			{
				return longueurUtile / 3.0;
			}
			if (nbPtsCompensation > 1)
			{
				return (longueurUtile - (double)pasTh) / (double)(nbPtsCompensation - 1);
			}
			break;
		}
		return 0.0;
	}

	public static double GetDemiPas(CompensationType compensation, double longueurUtile, int pasTh, int nbPtsCompensation)
	{
		if (nbPtsCompensation == 0)
		{
			return 0.0;
		}
		switch (compensation)
		{
		case CompensationType.P_CONSTANT:
			if (nbPtsCompensation == 1)
			{
				return Math.Round(longueurUtile / 2.0, 2);
			}
			if (nbPtsCompensation > 3)
			{
				return Math.Round((longueurUtile - (double)((nbPtsCompensation - 1) * pasTh)) / 2.0, 2);
			}
			return longueurUtile / (double)nbPtsCompensation / 2.0;
		case CompensationType.P_VARIABLE:
			return nbPtsCompensation switch
			{
				1 => Math.Round(longueurUtile / 2.0, 2), 
				2 => Math.Round(longueurUtile / 3.0, 2), 
				_ => Math.Round((double)pasTh / 2.0, 2), 
			};
		default:
			return 0.0;
		}
	}
}
