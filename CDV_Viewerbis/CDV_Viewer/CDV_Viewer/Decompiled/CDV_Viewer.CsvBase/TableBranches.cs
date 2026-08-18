using System.Collections.Generic;
using System.IO;
using CDV_Viewer.Data.SIGObjects;
using CDV_Viewer.Traitements;

namespace CDV_Viewer.CsvBase;

public class TableBranches : BaseTable<BrancheRow>
{
	private SigDictionary<SIGNoeud> _sigNoeuds = new SigDictionary<SIGNoeud>();

	public override string Path => "topo\\branches.csv";

	public override string Header => "ID;NOEUD;VOIE;PK;TYPE";

	public SigDictionary<SIGNoeud> SigNoeuds => _sigNoeuds;

	public SIGNoeud SigNoeud(int id)
	{
		SIGNoeud value = null;
		_sigNoeuds.TryGetValue(id, out value);
		return value;
	}

	public List<BrancheRow> FromNoeud(int id)
	{
		List<BrancheRow> branches = new List<BrancheRow>();
		SIGNoeud value = null;
		if (!_sigNoeuds.TryGetValue(id, out value))
		{
			return branches;
		}
		value.Branches.ForEach(delegate(SIGBranche b)
		{
			branches.Add(base[b.ID]);
		});
		return branches;
	}

	public override BrancheRow RowFromCsv(string[] csvFields)
	{
		return BrancheRow.FromCsv(csvFields);
	}

	public override BrancheRow RowFromBinary(BinaryReader reader)
	{
		return BrancheRow.FromBinary(reader);
	}

	public int FreeNoeudId()
	{
		return _sigNoeuds?.FreeId() ?? (-1);
	}

	public override void Link()
	{
		LinkBrancheVoies();
		LinkBrancheNoeud();
		LinkNoeudsLignesAndSortBranches();
	}

	private void LinkBrancheVoies()
	{
		foreach (BrancheRow value in _items.Values)
		{
			SIGBranche sIGBranche = value.SIGBranche;
			SIGVoie sIGVoie = Base.CsvVoies[value.VOIE].SIGVoie;
			sIGVoie.Branches.Add(sIGBranche);
			sIGBranche.Voie = sIGVoie;
		}
	}

	private void LinkBrancheNoeud()
	{
		_sigNoeuds = SigDictionary<SIGNoeud>.FromRowsTable(_items.Values, (BrancheRow b) => b.NOEUD, (IEnumerable<BrancheRow> l) => new SIGNoeud(l));
	}

	private void LinkNoeudsLignesAndSortBranches()
	{
		SigDictionary<SIGVoie> sigDictionary = new SigDictionary<SIGVoie>();
		SigDictionary<SIGNoeud> noeudsInVoie = new SigDictionary<SIGNoeud>();
		foreach (LigneRow csvLigne in Base.CsvLignes)
		{
			SIGLigne sIGLigne = csvLigne.SIGLigne;
			LinkNoeudsVoiesAndSortBranches(sIGLigne, sigDictionary, noeudsInVoie);
			sIGLigne.VoiesAdjacentes = sigDictionary.ToList();
		}
		foreach (VoieRow csvVoie in Base.CsvVoies)
		{
			csvVoie.PKD = csvVoie.SIGVoie.PKDebut;
			csvVoie.PKF = csvVoie.SIGVoie.PKFin;
		}
	}

	private void LinkNoeudsVoiesAndSortBranches(SIGLigne ligne, SigDictionary<SIGVoie> voiesAdjacentes, SigDictionary<SIGNoeud> noeudsInVoie)
	{
		voiesAdjacentes.Clear();
		foreach (SIGVoie voie in ligne.Voies)
		{
			noeudsInVoie.Clear();
			List<SIGBranche> branches = voie.Branches;
			if (branches.Count == 0)
			{
				continue;
			}
			branches.Sort(delegate(SIGBranche b1, SIGBranche b2)
			{
				int num2 = b1.PK.CompareTo(b2.PK);
				return (num2 != 0) ? num2 : b1.Type.CompareTo(b2.Type);
			});
			SIGBranche sIGBranche = null;
			for (int num = 0; num < branches.Count; num++)
			{
				SIGBranche sIGBranche2 = branches[num];
				SIGNoeud noeud = sIGBranche2.Noeud;
				if (sIGBranche != null && sIGBranche2.Noeud == sIGBranche.Noeud)
				{
					if (sIGBranche2.Type == BrancheType.Amont)
					{
						branches[num - 1] = sIGBranche2;
						branches[num] = sIGBranche;
					}
				}
				else
				{
					noeudsInVoie[noeud.ID] = noeud;
					foreach (SIGBranche item in noeud.BranchesInOtherLine(ligne))
					{
						voiesAdjacentes[item.Voie.ID] = item.Voie;
					}
				}
				sIGBranche = sIGBranche2;
			}
			voie.Noeuds = noeudsInVoie.ToList();
			voie.GetBrancheDebut();
			voie.GetBrancheFin();
			if (voie.NoeudDebut == null)
			{
				Dialogs.BaseError("Voie sans Noeud Debut", voie, Base.CsvVoies[voie.ID].PKD);
			}
			else
			{
				_ = voie.NoeudDebut.Branches.Count;
				_ = 2;
			}
			if (voie.NoeudFin == null)
			{
				Dialogs.BaseError("Voie sans Noeud Fin", voie, Base.CsvVoies[voie.ID].PKF);
				continue;
			}
			_ = voie.NoeudFin.Branches.Count;
			_ = 2;
		}
	}

	public void Update(SIGBranche branche)
	{
		base[branche.ID].Update();
		Base.NeedLink();
	}

	public SIGNoeud CreateNoeud(IEnumerable<SIGBranche> branches)
	{
		SIGNoeud sIGNoeud = new SIGNoeud(FreeNoeudId());
		_sigNoeuds.Add(sIGNoeud);
		foreach (SIGBranche branch in branches)
		{
			sIGNoeud.Add(branch);
			if (!TryGetValue(branch.ID, out var row))
			{
				row = new BrancheRow(branch);
				Add(row);
			}
			else
			{
				row.Update();
			}
		}
		return sIGNoeud;
	}

	public SIGNoeud CreateNoeud(SIGVoie idVoie, int pk, BrancheType type)
	{
		SIGNoeud sIGNoeud = new SIGNoeud(FreeNoeudId());
		_sigNoeuds.Add(sIGNoeud);
		Create(sIGNoeud, idVoie, pk, type);
		return sIGNoeud;
	}

	public SIGNoeud CreateNoeud(SIGVoie voieD, int pkD, BrancheType typeD, SIGVoie voieF, int pkF, BrancheType typeF)
	{
		SIGNoeud sIGNoeud = new SIGNoeud(FreeNoeudId());
		_sigNoeuds.Add(sIGNoeud);
		Create(sIGNoeud, voieD, pkD, typeD);
		Create(sIGNoeud, voieF, pkF, typeF);
		return sIGNoeud;
	}

	public SIGBranche Create(SIGNoeud noeud, SIGVoie voie, int pk, BrancheType type)
	{
		int num = FreeId();
		SIGBranche sIGBranche = new SIGBranche(num)
		{
			ID = num,
			Noeud = noeud,
			PK = pk,
			Type = type,
			Voie = voie
		};
		noeud.Add(sIGBranche);
		BrancheRow row = new BrancheRow(sIGBranche);
		Add(row);
		return sIGBranche;
	}

	internal List<SIGNoeud> GetSigNoeuds()
	{
		return _sigNoeuds.ToList();
	}
}
