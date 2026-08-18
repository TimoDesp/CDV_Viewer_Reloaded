using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using CDV_Viewer.CsvBase;
using CDV_Viewer.Traitements;

namespace CDV_Viewer.Data.SIGObjects;

public class SIGNoeud : ISigId, IComparable
{
	public enum NoeudType
	{
		Vide,
		BranchementVoiePointe,
		BranchementVoieTalon,
		BranchementLignePointe,
		BranchementLigneTalon,
		Tj,
		SautPk,
		RebroussementPk,
		ChangementVoie,
		ChangementLigne,
		Heurtoir,
		Error
	}

	public readonly List<SIGBranche> Branches = new List<SIGBranche>(4);

	private NoeudType? _type;

	public int Count => Branches.Count;

	public SIGBranche BrancheAmont { get; private set; }

	public SIGBranche BrancheAval { get; private set; }

	public SIGBranche AntenneAval { get; private set; }

	public SIGBranche AntenneAmont { get; private set; }

	public int ID { get; internal set; }

	public string Appareil
	{
		get
		{
			switch (Type)
			{
			case NoeudType.Vide:
			case NoeudType.Error:
				return "Vide";
			case NoeudType.BranchementVoiePointe:
			case NoeudType.BranchementLignePointe:
				return "Aiguille en Pointe";
			case NoeudType.BranchementVoieTalon:
			case NoeudType.BranchementLigneTalon:
				return "Aiguille en Talon";
			case NoeudType.ChangementLigne:
				return "Changement de Ligne";
			case NoeudType.ChangementVoie:
				return "Changement de Voie";
			case NoeudType.Heurtoir:
				return "Heurtoir";
			case NoeudType.Tj:
				return "Traversée Jonction";
			default:
				return "Vide";
			}
		}
	}

	public NoeudType Type
	{
		get
		{
			if (!_type.HasValue)
			{
				_type = setType();
			}
			return _type.Value;
		}
	}

	public bool IsMarqueur
	{
		get
		{
			if (Type == NoeudType.SautPk)
			{
				return BrancheAmont.PK == BrancheAval.PK;
			}
			return false;
		}
	}

	public bool IsSautPk
	{
		get
		{
			if (Type != NoeudType.SautPk)
			{
				return Type == NoeudType.RebroussementPk;
			}
			return true;
		}
	}

	public bool IsPointe
	{
		get
		{
			if (Type != NoeudType.BranchementVoiePointe)
			{
				return Type == NoeudType.BranchementLignePointe;
			}
			return true;
		}
	}

	public bool IsTalon
	{
		get
		{
			if (Type != NoeudType.BranchementVoieTalon)
			{
				return Type == NoeudType.BranchementLigneTalon;
			}
			return true;
		}
	}

	public bool IsTj => Type == NoeudType.Tj;

	public bool ChangementLigne
	{
		get
		{
			if (Type != NoeudType.BranchementLignePointe && Type != NoeudType.BranchementLigneTalon)
			{
				return Type == NoeudType.ChangementLigne;
			}
			return true;
		}
	}

	public bool HasAntenne
	{
		get
		{
			if (AntenneAmont == null)
			{
				return AntenneAval != null;
			}
			return true;
		}
	}

	public bool ChangementDeSens
	{
		get
		{
			switch (Type)
			{
			case NoeudType.BranchementLignePointe:
				if (this == AntenneAval.Voie.NoeudFin)
				{
					return true;
				}
				return false;
			case NoeudType.BranchementLigneTalon:
				if (this == AntenneAmont.Voie.NoeudDebut)
				{
					return true;
				}
				return false;
			case NoeudType.ChangementLigne:
				if (this == BrancheAmont.Voie.NoeudDebut)
				{
					return true;
				}
				if (this == BrancheAval.Voie.NoeudFin)
				{
					return true;
				}
				return false;
			default:
				return false;
			}
		}
	}

	public string MgvNoeudType
	{
		get
		{
			switch (Type)
			{
			case NoeudType.BranchementVoiePointe:
			case NoeudType.BranchementLignePointe:
				return "POINTE";
			case NoeudType.BranchementVoieTalon:
			case NoeudType.BranchementLigneTalon:
				return "TALON";
			case NoeudType.Vide:
			case NoeudType.SautPk:
			case NoeudType.RebroussementPk:
			case NoeudType.ChangementVoie:
			case NoeudType.ChangementLigne:
				return "NOEUD";
			default:
				return string.Empty;
			}
		}
	}

	public bool ChgSensPk
	{
		get
		{
			NoeudType type = Type;
			if ((uint)(type - 3) <= 1u || type == NoeudType.ChangementLigne)
			{
				if (AntenneAval != null && AntenneAval.ChgSensPk)
				{
					return true;
				}
				if (AntenneAmont != null && AntenneAmont.ChgSensPk)
				{
					return true;
				}
				if (BrancheAmont.ChgSensPk)
				{
					return true;
				}
				if (BrancheAval.ChgSensPk)
				{
					return true;
				}
				return false;
			}
			return false;
		}
	}

	public int PkOnTrack(SIGVoie voie)
	{
		if (BrancheAmont?.Voie == voie)
		{
			return BrancheAmont.PK;
		}
		if (AntenneAmont?.Voie == voie)
		{
			return AntenneAmont.PK;
		}
		if (AntenneAval?.Voie == voie)
		{
			return AntenneAval.PK;
		}
		if (BrancheAval?.Voie == voie)
		{
			return BrancheAval.PK;
		}
		return -999999;
	}

	public int PkOnLine(SIGLigne ligne)
	{
		if (BrancheAmont?.Voie?.Ligne == ligne)
		{
			return BrancheAmont.PK;
		}
		if (AntenneAmont?.Voie?.Ligne == ligne)
		{
			return AntenneAmont.PK;
		}
		if (AntenneAval?.Voie?.Ligne == ligne)
		{
			return AntenneAval.PK;
		}
		if (BrancheAval?.Voie?.Ligne == ligne)
		{
			return BrancheAval.PK;
		}
		return -999999;
	}

	public List<SIGBranche> BranchesInTrack(SIGVoie voie)
	{
		return Branches.FindAll((SIGBranche b) => b.Voie.ID == voie.ID);
	}

	public SIGBranche FirstBrancheInTrack(SIGVoie voie)
	{
		return Branches.Find((SIGBranche b) => b.Voie.ID == voie.ID);
	}

	public List<SIGBranche> BranchesInOtherTrack(SIGVoie voie)
	{
		return Branches.FindAll((SIGBranche b) => b.Voie.ID != voie.ID);
	}

	public SIGBranche FirstBrancheOtherTrack(SIGVoie voie)
	{
		return Branches.Find((SIGBranche b) => b.Voie.ID != voie.ID);
	}

	public List<SIGBranche> BranchesInLine(SIGLigne ligne)
	{
		return Branches.FindAll((SIGBranche b) => b.Voie.Ligne.ID == ligne.ID);
	}

	public List<SIGBranche> BranchesInOtherLine(SIGLigne ligne)
	{
		return Branches.FindAll((SIGBranche b) => b.Voie.Ligne.ID != ligne.ID);
	}

	public SIGVoie[] OtherTracks(SIGVoie voie)
	{
		return (from b in Branches
			group b by b.Voie into g
			select g.Key into v
			where v != voie
			select v).ToArray();
	}

	public SIGVoie[] OtherTracksOnSameLine(SIGVoie voie)
	{
		return (from b in Branches
			group b by b.Voie into g
			select g.Key into v
			where v != voie && v.Ligne == voie.Ligne
			select v).ToArray();
	}

	internal SIGBranche BrancheAvalInTrack(SIGVoie voie)
	{
		if (AntenneAval?.Voie == voie)
		{
			return AntenneAval;
		}
		if (BrancheAval?.Voie == voie)
		{
			return BrancheAval;
		}
		return null;
	}

	internal SIGBranche BrancheAmontInTrack(SIGVoie voie)
	{
		if (BrancheAmont?.Voie == voie)
		{
			return BrancheAmont;
		}
		if (AntenneAmont?.Voie == voie)
		{
			return AntenneAmont;
		}
		return null;
	}

	public List<SIGJoint> GetJointsAround()
	{
		List<SIGJoint> list = new List<SIGJoint>();
		SIGJoint item;
		if ((item = BrancheAmont?.GetNearestJoint()) != null)
		{
			list.Add(item);
		}
		if ((item = BrancheAval?.GetNearestJoint()) != null)
		{
			list.Add(item);
		}
		if ((item = AntenneAmont?.GetNearestJoint()) != null)
		{
			list.Add(item);
		}
		if ((item = AntenneAval?.GetNearestJoint()) != null)
		{
			list.Add(item);
		}
		return list;
	}

	private NoeudType setType()
	{
		bool flag = BrancheAmont != null;
		bool flag2 = BrancheAval != null;
		bool num = AntenneAmont != null;
		bool flag3 = AntenneAval != null;
		Branches.Clear();
		if (flag)
		{
			Branches.Add(BrancheAmont);
		}
		if (flag2)
		{
			Branches.Add(BrancheAval);
		}
		if (num)
		{
			Branches.Add(AntenneAmont);
		}
		if (flag3)
		{
			Branches.Add(AntenneAval);
		}
		switch (Branches.Count)
		{
		case 3:
			if (flag3)
			{
				if (AntenneAval.Voie.Ligne != BrancheAmont.Voie.Ligne)
				{
					return NoeudType.BranchementLignePointe;
				}
				return NoeudType.BranchementVoiePointe;
			}
			if (AntenneAmont.Voie.Ligne != BrancheAval.Voie.Ligne)
			{
				return NoeudType.BranchementLigneTalon;
			}
			return NoeudType.BranchementVoieTalon;
		case 1:
			if (HasAntenne)
			{
				return NoeudType.Error;
			}
			return NoeudType.Heurtoir;
		case 2:
			if (HasAntenne)
			{
				return NoeudType.Error;
			}
			if (BrancheAval.Voie != BrancheAmont.Voie)
			{
				if (BrancheAval.Voie.Ligne != BrancheAmont.Voie.Ligne)
				{
					return NoeudType.ChangementLigne;
				}
				return NoeudType.ChangementVoie;
			}
			if (BrancheAmont.PK > BrancheAval.PK)
			{
				return NoeudType.RebroussementPk;
			}
			return NoeudType.SautPk;
		case 4:
			return NoeudType.Tj;
		default:
			return NoeudType.Vide;
		}
	}

	public SIGNoeud(int id)
	{
		ID = id;
	}

	public SIGNoeud(IEnumerable<BrancheRow> l)
	{
		int num = -1;
		foreach (BrancheRow item in l)
		{
			if (num < 0)
			{
				num = (ID = item.NOEUD);
			}
			if (item.NOEUD != num)
			{
				throw new ArgumentException("Les branches doivent avoir le meme Noeud");
			}
			_addBranche(item.SIGBranche);
		}
		_organiseBranches();
		setType();
	}

	public List<SIGVoie> GetVoies()
	{
		return (from b in Branches
			group b by b.Voie into g
			select g.Key).ToList();
	}

	private SIGBranche _addBranche(SIGBranche branche)
	{
		_type = null;
		branche.Noeud = this;
		if (branche.IsAmont)
		{
			if (BrancheAmont != null && AntenneAmont != null)
			{
				MessageBox.Show($"Noeud id {ID} a plus de 2 de branches Amont");
				branche.Noeud = null;
				return null;
			}
			if (BrancheAmont == null)
			{
				return BrancheAmont = branche;
			}
			return AntenneAmont = branche;
		}
		if (branche.IsAval)
		{
			if (BrancheAval != null && AntenneAval != null)
			{
				MessageBox.Show($"Noeud id {ID} a plus de 2 de branches Aval");
				return null;
			}
			if (BrancheAval == null)
			{
				return BrancheAval = branche;
			}
			return AntenneAval = branche;
		}
		return null;
	}

	private void _organiseBranches()
	{
		_type = null;
		if (BrancheAmont != null && BrancheAval != null && (AntenneAmont != null || AntenneAval != null) && BrancheAmont.Voie != BrancheAval.Voie)
		{
			if (AntenneAval != null && AntenneAval.Voie == BrancheAmont.Voie)
			{
				SIGBranche antenneAval = AntenneAval;
				AntenneAval = BrancheAval;
				BrancheAval = antenneAval;
			}
			else if (AntenneAmont != null && AntenneAmont.Voie == BrancheAval.Voie)
			{
				SIGBranche antenneAmont = AntenneAmont;
				AntenneAmont = BrancheAmont;
				BrancheAmont = antenneAmont;
			}
			else if (AntenneAval != null && AntenneAmont != null && AntenneAmont.Voie == AntenneAval.Voie)
			{
				SIGBranche antenneAmont2 = AntenneAmont;
				AntenneAmont = BrancheAmont;
				BrancheAmont = antenneAmont2;
				antenneAmont2 = AntenneAval;
				AntenneAval = BrancheAval;
				BrancheAval = antenneAmont2;
			}
			else if (AntenneAval != null && BrancheAval.Voie.IsJonction())
			{
				SIGBranche antenneAval2 = AntenneAval;
				AntenneAval = BrancheAval;
				BrancheAval = antenneAval2;
			}
			else if (AntenneAmont != null && BrancheAmont.Voie.IsJonction())
			{
				SIGBranche antenneAmont3 = AntenneAmont;
				AntenneAmont = BrancheAmont;
				BrancheAmont = antenneAmont3;
			}
		}
	}

	public SIGBranche Add(SIGBranche branche)
	{
		SIGBranche sIGBranche = _addBranche(branche);
		if (sIGBranche == null)
		{
			return sIGBranche;
		}
		_organiseBranches();
		setType();
		return sIGBranche;
	}

	public void RemoveAllBranches()
	{
		SIGBranche sIGBranche = (AntenneAval = null);
		SIGBranche sIGBranche3 = (AntenneAmont = sIGBranche);
		SIGBranche brancheAmont = (BrancheAval = sIGBranche3);
		BrancheAmont = brancheAmont;
		Branches.Clear();
		_type = null;
	}

	public void SortMGV(SIGBranche brancheD, SIGBranche brancheF)
	{
	}

	public override string ToString()
	{
		string text = Type.ToString();
		switch (Type)
		{
		case NoeudType.ChangementVoie:
			return text + $"{BrancheAmont.Voie} pk {Chaines.PkToString(BrancheAmont.PK)} => {BrancheAval.Voie} pk {Chaines.PkToString(BrancheAval.PK)}";
		case NoeudType.SautPk:
		case NoeudType.RebroussementPk:
			return text + $" PK {Chaines.PkToString(BrancheAmont.PK)} de {Math.Abs(BrancheAval.PK - BrancheAmont.PK)} m";
		case NoeudType.BranchementVoiePointe:
			if (BrancheAmont.Voie.Ligne != AntenneAval.Voie.Ligne)
			{
				return text + $"PK {Chaines.PkToString(BrancheAmont.PK)} {BrancheAmont.Voie} => {AntenneAval.Voie}";
			}
			return text + $"PK {Chaines.PkToString(BrancheAmont.PK)} {BrancheAmont.Voie} => {AntenneAval.Voie} pk {Chaines.PkToString(AntenneAval.PK)}";
		case NoeudType.BranchementVoieTalon:
			if (BrancheAmont.Voie.Ligne != AntenneAmont.Voie.Ligne)
			{
				return text + $"PK {Chaines.PkToString(BrancheAmont.PK)} {BrancheAmont.Voie} => {AntenneAmont.Voie}";
			}
			return text + $"PK {Chaines.PkToString(BrancheAmont.PK)} {BrancheAmont.Voie} => {AntenneAmont.Voie} pk {Chaines.PkToString(AntenneAmont.PK)}";
		case NoeudType.Tj:
			return text + $"PK {Chaines.PkToString(BrancheAmont.PK)} {BrancheAmont.Voie} => {AntenneAval.Voie} pk {Chaines.PkToString(AntenneAval.PK)}";
		case NoeudType.Heurtoir:
			if (BrancheAmont != null)
			{
				return text + $" en Fin PK( {Chaines.PkToString(BrancheAmont.PK)})";
			}
			return text + $" en Debut PK( {Chaines.PkToString(BrancheAval.PK)})";
		default:
			return text;
		}
	}

	public int CompareTo(object obj)
	{
		return ID.CompareTo((obj as SIGNoeud).ID);
	}
}
