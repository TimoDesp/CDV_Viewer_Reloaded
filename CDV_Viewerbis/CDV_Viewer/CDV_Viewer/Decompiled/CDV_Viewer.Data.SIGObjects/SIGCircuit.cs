using System;
using System.Collections.Generic;
using CDV_Viewer.CsvBase;
using CDV_Viewer.DockControls;
using CDV_Viewer.Traitements;

namespace CDV_Viewer.Data.SIGObjects;

public class SIGCircuit : IComparable, ISigId
{
	public string Nom;

	public CircuitType Type;

	public int Frequence;

	public CompensationType Compensation;

	public int NbPtsCompensation;

	public double PasReel;

	public double ICC;

	public double N_FUITE_LONG_ARR;

	public bool CALCUL_CONFORME;

	public double IFuite;

	public double Diaphonie;

	public List<SIGModele> Modeles = new List<SIGModele>();

	public List<SIGDemiJoint> DemiJoints = new List<SIGDemiJoint>();

	private SIGDemiJoint _demiJointDebut;

	private SIGDemiJoint _demiJointFin;

	public int ID { get; set; } = -1;

	public SIGDemiJoint DemiJointDebut
	{
		get
		{
			if (_demiJointDebut == null && _demiJointFin == null)
			{
				SetDemiJointsDebutFin();
			}
			return _demiJointDebut;
		}
	}

	public SIGDemiJoint DemiJointFin
	{
		get
		{
			if (_demiJointDebut == null && _demiJointFin == null)
			{
				SetDemiJointsDebutFin();
			}
			return _demiJointFin;
		}
	}

	public int PKDebut => DemiJointDebut?.Joint?.PK ?? 0;

	public int PKFin => DemiJointFin?.Joint?.PK ?? 999999;

	public bool NeedJI
	{
		get
		{
			if (Type != CircuitType.ITE)
			{
				return Type == CircuitType.NC;
			}
			return true;
		}
	}

	public bool IsTVM
	{
		get
		{
			if (Type != CircuitType.SEI && Type != CircuitType.TVM300)
			{
				return Type == CircuitType.TVM430;
			}
			return true;
		}
	}

	public bool IsIPCS
	{
		get
		{
			bool isTVM = IsTVM;
			foreach (SIGDemiJoint demiJoint in DemiJoints)
			{
				if (!demiJoint.Emetteur)
				{
					return false;
				}
			}
			return isTVM;
		}
	}

	public SIGCircuit()
	{
	}

	public SIGCircuit(int id)
	{
		ID = id;
	}

	public static SIGCircuit Create(CircuitType type, int frequence, List<SIGJoint> joints)
	{
		SIGCircuit sIGCircuit = new SIGCircuit();
		switch (type)
		{
		case CircuitType.ITE:
			sIGCircuit.Compensation = CompensationType.NON;
			break;
		case CircuitType.UM71:
			sIGCircuit.Compensation = CompensationType.NON;
			break;
		case CircuitType.TVM300:
			sIGCircuit.Compensation = CompensationType.P_CONSTANT;
			break;
		case CircuitType.SEI:
			sIGCircuit.Compensation = CompensationType.P_VARIABLE;
			break;
		case CircuitType.TVM430:
			sIGCircuit.Compensation = CompensationType.P_VARIABLE;
			break;
		}
		if (sIGCircuit.Compensation == CompensationType.P_CONSTANT)
		{
			sIGCircuit.PasReel = 100.0;
			double num = (double)(joints[1].PK - joints[0].PK) - (joints[0].DemiLongueur + joints[1].DemiLongueur);
			sIGCircuit.NbPtsCompensation = (int)Math.Ceiling(num / sIGCircuit.PasReel) + 1;
			if ((num - (double)sIGCircuit.NbPtsCompensation * sIGCircuit.PasReel) / 2.0 < 30.0)
			{
				sIGCircuit.NbPtsCompensation--;
			}
		}
		if (sIGCircuit.Compensation == CompensationType.P_VARIABLE)
		{
			int num2 = -1;
			switch (frequence)
			{
			case 1700:
			case 2000:
				num2 = 60;
				break;
			case 2300:
			case 2600:
				num2 = 80;
				break;
			}
			if (num2 > -1)
			{
				double num3 = (double)(joints[1].PK - joints[0].PK) - (joints[0].DemiLongueur + joints[1].DemiLongueur);
				sIGCircuit.NbPtsCompensation = 1 + (int)Math.Ceiling((num3 - (double)num2) / (double)num2);
				if (sIGCircuit.NbPtsCompensation >= 1)
				{
					if ((num3 - (double)num2) / (double)(sIGCircuit.NbPtsCompensation - 1) < (double)(2 * num2 / 3))
					{
						sIGCircuit.NbPtsCompensation--;
					}
					if (sIGCircuit.NbPtsCompensation >= 1)
					{
						sIGCircuit.PasReel = (num3 - (double)num2) / (double)(sIGCircuit.NbPtsCompensation - 1);
					}
				}
			}
		}
		sIGCircuit.PasReel = Math.Round(sIGCircuit.PasReel * 100.0) / 100.0;
		if (type == CircuitType.TVM300)
		{
			sIGCircuit.N_FUITE_LONG_ARR = 200.0;
			if (frequence == 2600)
			{
				sIGCircuit.ICC = 450.0;
			}
			else
			{
				sIGCircuit.ICC = 500.0;
			}
			sIGCircuit.IFuite = 100.0;
			sIGCircuit.Diaphonie = 200.0;
		}
		if (type == CircuitType.TVM430 || type == CircuitType.SEI)
		{
			sIGCircuit.N_FUITE_LONG_ARR = 320.0;
			sIGCircuit.ICC = 800.0;
			sIGCircuit.IFuite = 160.0;
			sIGCircuit.Diaphonie = 320.0;
		}
		return sIGCircuit;
	}

	internal void SetDemiJointsDebutFin()
	{
		if (DemiJoints.Count == 0)
		{
			Console.WriteLine($"{this}  pas de demi joint");
			return;
		}
		int num = 0;
		int num2 = 0;
		int num3 = 0;
		_demiJointDebut = null;
		_demiJointFin = null;
		SIGDemiJoint sIGDemiJoint = null;
		foreach (SIGDemiJoint demiJoint in DemiJoints)
		{
			if (!demiJoint.Principal)
			{
				continue;
			}
			num3++;
			if (demiJoint.IsAval)
			{
				num++;
				if (_demiJointDebut == null)
				{
					_demiJointDebut = demiJoint;
				}
				else
				{
					sIGDemiJoint = demiJoint;
				}
			}
			else
			{
				num2++;
				if (_demiJointFin == null)
				{
					_demiJointFin = demiJoint;
				}
				else
				{
					sIGDemiJoint = demiJoint;
				}
			}
		}
		if (num == 1 && num2 == 1)
		{
			return;
		}
		if (num3 == 0)
		{
			Console.WriteLine($"{this}  pas de demi joint principal");
			return;
		}
		if (num3 < 2)
		{
			if (_demiJointDebut?.Joint?.Voie == null)
			{
				_ = _demiJointFin.Joint.Voie;
			}
			Console.WriteLine($"{this}  {_demiJointDebut.Joint.Voie.FullName}  1 seul demi joint principal");
			return;
		}
		if (sIGDemiJoint == null)
		{
			Console.WriteLine($"{this}  {num3} DemiJointPrincipal");
			return;
		}
		if (_demiJointDebut == null)
		{
			_demiJointDebut = sIGDemiJoint;
		}
		if (_demiJointFin == null)
		{
			_demiJointFin = sIGDemiJoint;
		}
		if (num3 != 2 || _demiJointDebut.Joint.Voie.Ligne == _demiJointFin.Joint.Voie.Ligne)
		{
			Console.WriteLine($"{this}  {_demiJointDebut.Joint.Voie.FullName} {num3} DemiJointsPrincipaux");
		}
	}

	public SIGModele GetModele(SIGDemiJoint demijointDebut, SIGDemiJoint demijointFin)
	{
		foreach (SIGModele modele in Modeles)
		{
			if (modele.DemiJointE == demijointDebut && modele.DemiJointS == DemiJointFin)
			{
				return modele;
			}
		}
		return null;
	}

	public int CompareTo(object Y)
	{
		return ID.CompareTo(((SIGCircuit)Y).ID);
	}

	public SIGCircuit GetNextCircuit(MoveOrientation orientation)
	{
		if (DemiJoints.Count == 0)
		{
			return null;
		}
		int positionY = DemiJoints[0].Joint.Voie.PositionY;
		SIGDemiJoint sIGDemiJoint = null;
		for (int i = 1; i < DemiJoints.Count; i++)
		{
			if (DemiJoints[i].Joint.Voie.Ligne != DemiJoints[0].Joint.Voie.Ligne)
			{
				continue;
			}
			int positionY2 = DemiJoints[i].Joint.Voie.PositionY;
			bool flag = DemiJoints[i].Joint.PK > DemiJoints[0].Joint.PK;
			if (!ComposantsViewer.Viewer.PkCroissant)
			{
				flag = !flag;
				switch (orientation)
				{
				case MoveOrientation.E:
					orientation = MoveOrientation.W;
					break;
				case MoveOrientation.NE:
					orientation = MoveOrientation.NW;
					break;
				case MoveOrientation.NW:
					orientation = MoveOrientation.NE;
					break;
				case MoveOrientation.SE:
					orientation = MoveOrientation.SW;
					break;
				case MoveOrientation.SW:
					orientation = MoveOrientation.SE;
					break;
				case MoveOrientation.W:
					orientation = MoveOrientation.E;
					break;
				}
			}
			if (positionY2 <= int.MinValue)
			{
				continue;
			}
			if (positionY2 < positionY)
			{
				if (flag)
				{
					if (orientation == MoveOrientation.NE)
					{
						sIGDemiJoint = DemiJoints[i].Joint._demiJoints.Find((SIGDemiJoint demiJoint) => demiJoint.Circuit != this);
					}
				}
				else if (orientation == MoveOrientation.NW)
				{
					sIGDemiJoint = DemiJoints[i].Joint._demiJoints.Find((SIGDemiJoint demiJoint) => demiJoint.Circuit != this);
				}
			}
			else if (positionY2 > positionY)
			{
				if (flag)
				{
					if (orientation == MoveOrientation.SE)
					{
						sIGDemiJoint = DemiJoints[i].Joint._demiJoints.Find((SIGDemiJoint demiJoint) => demiJoint.Circuit != this);
					}
				}
				else if (orientation == MoveOrientation.SW)
				{
					sIGDemiJoint = DemiJoints[i].Joint._demiJoints.Find((SIGDemiJoint demiJoint) => demiJoint.Circuit != this);
				}
			}
			else
			{
				if (!flag)
				{
					continue;
				}
				switch (orientation)
				{
				case MoveOrientation.E:
					sIGDemiJoint = DemiJoints[i].Joint._demiJoints.Find((SIGDemiJoint demiJoint) => demiJoint.Circuit != this);
					break;
				case MoveOrientation.W:
					sIGDemiJoint = DemiJoints[0].Joint._demiJoints.Find((SIGDemiJoint demiJoint) => demiJoint.Circuit != this);
					break;
				}
			}
		}
		return sIGDemiJoint?.Circuit;
	}

	public int GetLongueur()
	{
		return SIGSegment.GetLongueur(CDV_Viewer.Traitements.Composants.FindParcours(DemiJointDebut.Joint, DemiJointFin.Joint));
	}

	public double GetLongueurUtile()
	{
		return (double)GetLongueur() - (DemiJointDebut?.Joint?.DemiLongueur ?? (0.0 + DemiJointFin?.Joint?.DemiLongueur) ?? 0.0);
	}

	public override string ToString()
	{
		return $"{Nom} : id({ID})";
	}

	internal void UnLink()
	{
		Modeles = new List<SIGModele>();
		DemiJoints = new List<SIGDemiJoint>();
		_demiJointDebut = (_demiJointFin = null);
	}
}
