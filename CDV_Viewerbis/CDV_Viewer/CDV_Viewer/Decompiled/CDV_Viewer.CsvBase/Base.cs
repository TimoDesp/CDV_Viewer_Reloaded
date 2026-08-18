using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using CDV_Viewer.Data;
using CDV_Viewer.Data.SIGObjects;
using CDV_Viewer.DockControls;
using CDV_Viewer.Traitements;

namespace CDV_Viewer.CsvBase;

public static class Base
{
	internal static TableLignes CsvLignes = new TableLignes();

	internal static TableVoies CsvVoies = new TableVoies();

	internal static TablePositionsVoies CsvPositionsVoies = new TablePositionsVoies();

	internal static TableVoiesTimon CsvVoiesTimon = new TableVoiesTimon();

	internal static TableBranches CsvBranches = new TableBranches();

	internal static TableBalises CsvBalises = new TableBalises();

	internal static TableCircuits CsvCircuits = new TableCircuits();

	internal static TableJoints CsvJoints = new TableJoints();

	internal static TableJointsCircuits CsvJointsCircuits = new TableJointsCircuits();

	internal static TableModeles CsvModeles = new TableModeles();

	internal static ICsvTable[] CsvTables = new ICsvTable[10] { CsvLignes, CsvVoies, CsvPositionsVoies, CsvVoiesTimon, CsvBranches, CsvBalises, CsvCircuits, CsvJoints, CsvJointsCircuits, CsvModeles };

	public static int nb;

	private static bool _isSave;

	private static bool _needLink;

	public static bool IsLinked => !_needLink;

	public static bool IsSave => _isSave;

	public static event EventHandler SaveStateChanged;

	public static event EventHandler ListLignesChanged;

	public static bool Load()
	{
		_isSave = true;
		_needLink = true;
		ICsvTable[] csvTables = CsvTables;
		for (int i = 0; i < csvTables.Length; i++)
		{
			csvTables[i].Load(Paths.TempDataFolder);
		}
		Link();
		return true;
	}

	public static bool LoadFromBinary()
	{
		using (Stream input = new ZipArchive(File.Open(Path.Combine(Paths.DataFolder, "REF_MGV.BDB"), FileMode.Open), ZipArchiveMode.Read, leaveOpen: false).GetEntry("Binary").Open())
		{
			_isSave = true;
			_needLink = true;
			using BinaryReader reader = new BinaryReader(input);
			CsvLignes.LoadBinary(reader);
			CsvVoies.LoadBinary(reader);
			CsvPositionsVoies.LoadBinary(reader);
			CsvVoiesTimon.LoadBinary(reader);
			CsvBranches.LoadBinary(reader);
			CsvBalises.LoadBinary(reader);
			CsvCircuits.LoadBinary(reader);
			CsvJoints.LoadBinary(reader);
			CsvJointsCircuits.LoadBinary(reader);
			CsvModeles.LoadBinary(reader);
		}
		Link();
		return true;
	}

	public static void Close()
	{
		ICsvTable[] csvTables = CsvTables;
		for (int i = 0; i < csvTables.Length; i++)
		{
			csvTables[i].Clear();
		}
	}

	public static void SaveToTempFolder()
	{
		CsvLignes.Save();
		CsvVoies.Save();
		CsvPositionsVoies.Save();
		CsvVoiesTimon.Save();
		CsvBranches.Save();
		CsvBalises.Save();
		CsvCircuits.Save();
		CsvJoints.Save();
		CsvJointsCircuits.Save();
		CsvModeles.Save();
		_isSave = true;
		Base.SaveStateChanged?.Invoke(null, new EventArgs());
		ComposantsViewer.Viewer.RefreshLigne();
	}

	public static void SaveToBinary()
	{
		string text = Path.Combine(Paths.DataFolder, "REF_MGV.BDB");
		if (File.Exists(text))
		{
			File.Delete(text);
		}
		using ZipArchive zipArchive = ZipFile.Open(text, ZipArchiveMode.Create, null);
		ZipArchiveEntry zipArchiveEntry = zipArchive.CreateEntry("Binary", CompressionLevel.Optimal);
		DateTime now = DateTime.Now;
		zipArchiveEntry.LastWriteTime = now;
		using Stream output = zipArchiveEntry.Open();
		using BinaryWriter writer = new BinaryWriter(output);
		CsvLignes.BinarySave(writer);
		CsvVoies.BinarySave(writer);
		CsvPositionsVoies.BinarySave(writer);
		CsvVoiesTimon.BinarySave(writer);
		CsvBranches.BinarySave(writer);
		CsvBalises.BinarySave(writer);
		CsvCircuits.BinarySave(writer);
		CsvJoints.BinarySave(writer);
		CsvJointsCircuits.BinarySave(writer);
		CsvModeles.BinarySave(writer);
	}

	public static void NeedLink()
	{
		_needLink = true;
	}

	public static void Link()
	{
		if (_needLink)
		{
			Dialogs.ClearError();
			ICsvTable[] csvTables = CsvTables;
			for (int i = 0; i < csvTables.Length; i++)
			{
				csvTables[i].Link();
			}
			_needLink = false;
		}
	}

	public static void SetModif()
	{
		_isSave = false;
		Base.SaveStateChanged?.Invoke(null, new EventArgs());
	}

	public static void GetBase(List<SIGLigne> lignes, out List<SIGVoie> voies, out List<SIGNoeud> noeuds, out List<SIGJoint> joints, out List<SIGCircuit> circuits, out List<SIGModele> modeles)
	{
		lignes = CsvLignes.SigLignes();
		voies = CsvVoies.SigVoies();
		noeuds = CsvBranches.GetSigNoeuds();
		joints = CsvJoints.SigJoints();
		circuits = CsvCircuits.SigCircuits();
		modeles = CsvModeles.SigModeles();
	}

	public static SIGLigne GetLigne(int id)
	{
		return CsvLignes.SigLigne(id);
	}

	public static List<SIGLigne> GetLignes()
	{
		return CsvLignes.SigLignes();
	}

	public static void CreateLigne(int idLigne, string nom)
	{
		CsvLignes.Create(idLigne, nom);
		SetModif();
		Base.ListLignesChanged?.Invoke(null, new EventArgs());
	}

	public static void DeleteLigne(int id)
	{
		CsvLignes.Remove(id);
		SetModif();
		Base.ListLignesChanged?.Invoke(null, new EventArgs());
	}

	public static List<SIGVoie> GetVoies()
	{
		return CsvVoies.SigVoies();
	}

	public static SIGVoie GetVoie(int id)
	{
		VoieRow voieRow = CsvVoies[id];
		if (voieRow.ID < 0)
		{
			return null;
		}
		SIGVoie sIGVoie = new SIGVoie(voieRow.ID, voieRow.NOM);
		sIGVoie.Ligne = CsvLignes[voieRow.LIGNE].SIGLigne;
		int id2 = -1;
		int id3 = -1;
		int num = int.MaxValue;
		int num2 = int.MinValue;
		foreach (BrancheRow item3 in CsvBranches.FindAll((BrancheRow b) => b.VOIE == id))
		{
			if (item3.PK < num)
			{
				id2 = item3.NOEUD;
				num = item3.PK;
			}
			if (item3.PK > num2)
			{
				id3 = item3.NOEUD;
				num2 = item3.PK;
			}
		}
		SIGNoeud item = new SIGNoeud(CsvBranches.FromNoeud(id2));
		sIGVoie.Noeuds.Add(item);
		SIGNoeud item2 = new SIGNoeud(CsvBranches.FromNoeud(id3));
		sIGVoie.Noeuds.Add(item2);
		return sIGVoie;
	}

	public static int GetIDVoie(int ligne, string nom, int pk)
	{
		SIGLigne ligne2 = GetLigne(ligne);
		if (ligne2 != null)
		{
			foreach (SIGVoie voie in ligne2.Voies)
			{
				if (voie.Nom == nom && voie.PKDebut <= pk && voie.PKFin >= pk)
				{
					return voie.ID;
				}
			}
		}
		return int.MinValue;
	}

	public static int GetIdTimon(int id)
	{
		VoieTimonRow voieTimonRow = CsvVoiesTimon[id];
		if (voieTimonRow.ID >= 0)
		{
			return voieTimonRow.TIMON;
		}
		return -1;
	}

	public static Dictionary<int, int> GetIdsTimon()
	{
		return CsvVoiesTimon.ToDictionary((VoieTimonRow r) => r.ID, (VoieTimonRow r) => r.TIMON);
	}

	public static int GetPositionVoie(SIGVoie voie)
	{
		return CsvPositionsVoies.GetPosition(voie);
	}

	public static SIGVoie CreateVoie(SIGLigne ligne, string nomVoie, int pkDebut, int pkFin)
	{
		SIGVoie sIGVoie = CsvVoies.Create(ligne, nomVoie, pkDebut, pkFin);
		sIGVoie.NoeudDebut = CreateNoeud(sIGVoie, pkDebut, BrancheType.Aval);
		sIGVoie.NoeudFin = CreateNoeud(sIGVoie, pkFin, BrancheType.Amont);
		sIGVoie.Noeuds.Add(sIGVoie.NoeudDebut);
		sIGVoie.Noeuds.Add(sIGVoie.NoeudFin);
		sIGVoie.BrancheDebut = sIGVoie.NoeudDebut.BrancheAval;
		sIGVoie.BrancheFin = sIGVoie.NoeudDebut.BrancheAmont;
		SetModif();
		return sIGVoie;
	}

	public static SIGVoie CreateJonction(string NomJonction, SIGVoie voieDebut, int pkDebut, SIGVoie voieFin, int pkFin)
	{
		SIGVoie sIGVoie = CsvVoies.Create(voieDebut.Ligne, NomJonction, pkDebut, pkFin);
		SIGNoeud sIGNoeud = voieDebut.Branches.Find((SIGBranche b) => b.PK == pkDebut)?.Noeud;
		if (sIGNoeud == null)
		{
			sIGNoeud = CreateNoeud(voieDebut, pkDebut);
		}
		CreateBrancheInNoeud(sIGNoeud, sIGVoie, pkDebut, BrancheType.Aval);
		sIGVoie.NoeudDebut = sIGNoeud;
		sIGVoie.BrancheDebut = sIGNoeud.FirstBrancheInTrack(sIGVoie);
		sIGVoie.Noeuds.Add(sIGNoeud);
		SIGNoeud sIGNoeud2 = voieFin.Branches.Find((SIGBranche b) => b.PK == pkFin)?.Noeud;
		if (sIGNoeud2 == null)
		{
			sIGNoeud2 = CreateNoeud(voieFin, pkFin);
		}
		CreateBrancheInNoeud(sIGNoeud2, sIGVoie, pkFin, BrancheType.Amont);
		sIGVoie.NoeudFin = sIGNoeud2;
		sIGVoie.BrancheFin = sIGNoeud2.FirstBrancheInTrack(sIGVoie);
		sIGVoie.Noeuds.Add(sIGNoeud2);
		SetModif();
		return sIGVoie;
	}

	public static void SetPositionVoie(SIGVoie voie)
	{
		CsvPositionsVoies.SetPosition(voie);
		SetModif();
	}

	public static void UpdateVoie(SIGVoie voie)
	{
		CsvVoies.Update(voie);
		SetModif();
	}

	public static void DeleteVoie(SIGVoie voie)
	{
		foreach (SIGJoint joint in voie.Joints)
		{
			foreach (JointCircuitRow _rJointCircuit in CsvJointsCircuits.FromJoint(joint.ID))
			{
				if (_rJointCircuit.PRINCIPAL)
				{
					CsvJointsCircuits.RemoveAll((JointCircuitRow r) => r.CIRCUIT == _rJointCircuit.CIRCUIT);
					CsvCircuits.Remove(_rJointCircuit.CIRCUIT);
				}
				else
				{
					CsvJointsCircuits.Remove(_rJointCircuit.ID);
				}
			}
			CsvJoints.Remove(joint.ID);
		}
		foreach (SIGNoeud noeud in voie.Noeuds)
		{
			List<SIGBranche> list = noeud.BranchesInTrack(voie);
			List<SIGBranche> list2 = noeud.BranchesInOtherTrack(voie);
			foreach (SIGBranche item in list)
			{
				CsvBranches.Remove(item.ID);
			}
			if (list2.Count == 2 && list2[0].Voie == list2[1].Voie && list2[0].PK == list2[1].PK)
			{
				CsvBranches.Remove(list2[0].ID);
				CsvBranches.Remove(list2[1].ID);
			}
		}
		foreach (SIGBalise balise in voie.Balises)
		{
			CsvBalises.Remove(balise.ID);
		}
		CsvVoiesTimon.Remove(voie.ID);
		CsvPositionsVoies.Remove(voie.ID);
		CsvVoies.Remove(voie.ID);
		SetModif();
	}

	public static void DeletePositionVoie(int id)
	{
		CsvPositionsVoies.Remove(id);
		SetModif();
	}

	public static void SetIDTimon(int voie, int timon)
	{
		CsvVoiesTimon.SetTimonId(voie, timon);
		SetModif();
	}

	public static SIGNoeud GetNoeud(SIGVoie voie, int pk)
	{
		foreach (SIGBranche branch in voie.Branches)
		{
			if (branch.PK == pk)
			{
				return branch.Noeud;
			}
		}
		return null;
	}

	public static SIGNoeud GetNoeud(SIGVoie voie, int pk, int tolerance)
	{
		foreach (SIGBranche branch in voie.Branches)
		{
			if (Math.Abs(branch.PK - pk) <= tolerance)
			{
				return branch.Noeud;
			}
		}
		return null;
	}

	private static void MergeNoeuds(SIGNoeud noeudA, SIGNoeud noeudB)
	{
		int iD = noeudA.ID;
		int iD2 = noeudB.ID;
		foreach (BrancheRow item in CsvBranches.FromNoeud(iD2))
		{
			item.NOEUD = iD;
			noeudA.Add(item.SIGBranche);
		}
		NeedLink();
	}

	public static void UpdateNoeud(SIGNoeud noeud)
	{
		noeud.Branches.ForEach(delegate(SIGBranche branche)
		{
			UpdateBranche(branche);
		});
	}

	public static SIGNoeud CreateNoeud(SIGVoie voie, int pk)
	{
		return CreateNoeud(voie, pk, BrancheType.Amont, voie, pk, BrancheType.Aval);
	}

	public static SIGNoeud CreateSautdePK(SIGVoie voie, int pkDebut, int pkFin)
	{
		return CreateNoeud(voie, pkDebut, BrancheType.Amont, voie, pkFin, BrancheType.Aval);
	}

	public static SIGNoeud CreateNoeud(SIGVoie voie, int pk, BrancheType type)
	{
		SIGNoeud result = CsvBranches.CreateNoeud(voie, pk, type);
		SetModif();
		return result;
	}

	private static SIGNoeud CreateNoeud(SIGVoie voieD, int pkD, BrancheType typeD, SIGVoie voieF, int pkF, BrancheType typeF)
	{
		SIGNoeud result = CsvBranches.CreateNoeud(voieD, pkD, typeD, voieF, pkF, typeF);
		SetModif();
		return result;
	}

	private static void CreateBranchesAmontAval(SIGNoeud noeud, SIGVoie voie, int pk)
	{
		CsvBranches.Create(noeud, voie, pk, BrancheType.Amont);
		CsvBranches.Create(noeud, voie, pk, BrancheType.Aval);
		SetModif();
	}

	public static bool TryConnectNoeudToVoie(SIGNoeud noeud, SIGVoie voie, int pk, out string error)
	{
		error = "";
		if (pk == voie.PKDebut)
		{
			MergeNoeuds(noeud, voie.NoeudDebut);
		}
		else if (pk == voie.PKFin)
		{
			MergeNoeuds(noeud, voie.NoeudFin);
		}
		else
		{
			SIGNoeud noeud2 = GetNoeud(voie, pk);
			if (noeud2 != null)
			{
				bool flag = false;
				if (noeud.Type == SIGNoeud.NoeudType.Heurtoir)
				{
					flag = noeud.BrancheAval != null && noeud2.AntenneAval == null;
					flag |= noeud.BrancheAmont != null && noeud2.AntenneAmont == null;
				}
				if (!flag)
				{
					error = "Erreur, un noeud existe déjà à ce PK";
					return false;
				}
				MergeNoeuds(noeud, noeud2);
			}
			else
			{
				CreateBranchesAmontAval(noeud, voie, pk);
			}
		}
		return true;
	}

	public static SIGBranche CreateBrancheInNoeud(SIGNoeud noeud, SIGVoie voie, int pk, BrancheType type)
	{
		SIGBranche result = CsvBranches.Create(noeud, voie, pk, type);
		SetModif();
		return result;
	}

	public static void SepareBranche(SIGBranche branche)
	{
		SIGNoeud noeud = branche.Noeud;
		List<SIGBranche> list = noeud.BranchesInTrack(branche.Voie);
		List<SIGBranche> list2 = noeud.BranchesInOtherTrack(branche.Voie);
		int nOEUD = CsvBranches.FreeNoeudId();
		SIGNoeud sIGNoeud = new SIGNoeud(-1);
		foreach (SIGBranche item in list)
		{
			sIGNoeud.Add(item);
		}
		if (sIGNoeud.IsSautPk && sIGNoeud.BrancheAmont.PK == sIGNoeud.BrancheAmont.PK)
		{
			foreach (SIGBranche item2 in list)
			{
				CsvBranches.Remove(item2.ID);
			}
			nOEUD = noeud.ID;
		}
		SIGNoeud sIGNoeud2 = new SIGNoeud(-1);
		foreach (SIGBranche item3 in list2)
		{
			sIGNoeud2.Add(item3);
		}
		if (sIGNoeud2.IsSautPk && sIGNoeud2.BrancheAmont.PK == sIGNoeud2.BrancheAmont.PK)
		{
			foreach (SIGBranche item4 in list2)
			{
				CsvBranches.Remove(item4.ID);
			}
		}
		else
		{
			foreach (SIGBranche item5 in list2)
			{
				CsvBranches[item5.ID].NOEUD = nOEUD;
			}
		}
		NeedLink();
		SetModif();
	}

	public static List<SIGNoeud> SepareAllBranches(SIGNoeud noeud)
	{
		List<SIGNoeud> list = new List<SIGNoeud>();
		if (noeud.Branches.Count == 1)
		{
			CsvBranches.Remove(noeud.Branches[0].ID);
			noeud.RemoveAllBranches();
			NeedLink();
			SetModif();
			return list;
		}
		List<IGrouping<SIGVoie, SIGBranche>> list2 = (from b in noeud.Branches
			group b by b.Voie).ToList();
		bool flag = true;
		noeud.RemoveAllBranches();
		foreach (IGrouping<SIGVoie, SIGBranche> item2 in list2)
		{
			List<SIGBranche> list3 = item2.ToList();
			if (list3.Count == 2 && list3[0].PK == list3[1].PK)
			{
				list3.ForEach(delegate(SIGBranche b)
				{
					CsvBranches.Remove(b.ID);
				});
			}
			else if (flag)
			{
				flag = false;
				list3.ForEach(delegate(SIGBranche b)
				{
					noeud.Add(b);
				});
				list.Add(noeud);
			}
			else
			{
				SIGNoeud item = CsvBranches.CreateNoeud(list3);
				list.Add(item);
			}
		}
		NeedLink();
		SetModif();
		return list;
	}

	public static void UpdateBranche(SIGBranche branche)
	{
		CsvBranches.Update(branche);
		SetModif();
	}

	public static void DeleteNoeud(SIGNoeud noeud)
	{
		int idnoeud = noeud.ID;
		CsvBranches.RemoveAll((BrancheRow r) => r.NOEUD == idnoeud);
		CsvBranches.SigNoeuds.Remove(noeud);
		SetModif();
	}

	public static void AddBalises(List<SIGBalise> balises)
	{
		CsvBalises.Clear();
		CsvBalises.Add(balises.ConvertAll((SIGBalise b) => new BaliseRow(b)));
		SetModif();
	}

	public static void AddBalise(SIGVoie voie, BaliseType type, int pk, bool active)
	{
		CsvBalises.Create(voie, type, pk, active);
		SetModif();
	}

	public static void UpdateBalise(SIGBalise balise)
	{
		CsvBalises.Update(balise);
		SetModif();
	}

	public static void DeleteBalise(int idBalise)
	{
		CsvBalises.Remove(idBalise);
		SetModif();
	}

	public static SIGCircuit GetCircuit(int id)
	{
		CircuitRow circuitRow = CsvCircuits[id];
		if (circuitRow.ID != id)
		{
			return null;
		}
		return new SIGCircuit(circuitRow.ID)
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
	}

	public static SIGCircuit GetCircuitAmont(SIGJoint joint)
	{
		return joint?.DemiJointAmont?.Circuit;
	}

	public static SIGCircuit GetCircuitAval(SIGJoint joint)
	{
		return joint?.DemiJointAval?.Circuit;
	}

	public static SIGCircuit GetCircuitAdjacent(SIGDemiJoint demiJoint)
	{
		if (demiJoint.IsAmont)
		{
			return demiJoint.Joint?._demiJointAval?.Circuit;
		}
		if (demiJoint.IsAval)
		{
			return demiJoint.Joint?._demiJointAmont?.Circuit;
		}
		return null;
	}

	public static SIGCircuit GetCircuitByJoint(int iDjoint, bool apres)
	{
		foreach (JointCircuitRow item in CsvJointsCircuits.FromJoint(iDjoint))
		{
			SIGCircuit circuit = GetCircuit(item.ID);
			if (circuit == null)
			{
				continue;
			}
			if (apres)
			{
				if (circuit.DemiJointDebut.ID == iDjoint)
				{
					return circuit;
				}
			}
			else if (circuit.DemiJointFin.ID == iDjoint)
			{
				return circuit;
			}
		}
		return null;
	}

	public static List<SIGCircuit> GetCircuitsByJoint(int idJoint)
	{
		List<SIGCircuit> list = new List<SIGCircuit>();
		foreach (JointCircuitRow item in CsvJointsCircuits.FromJoint(idJoint))
		{
			SIGCircuit circuit = GetCircuit(item.ID);
			if (circuit != null)
			{
				list.Add(circuit);
			}
		}
		return list;
	}

	public static Dictionary<int, int> GetLignesCircuit(int circuit)
	{
		Dictionary<int, int> dictionary = new Dictionary<int, int>();
		foreach (JointCircuitRow item in CsvJointsCircuits.FindAll((JointCircuitRow r) => r.CIRCUIT == circuit))
		{
			JointRow jointRow = CsvJoints[item.JOINT];
			if (jointRow.ID == item.JOINT)
			{
				VoieRow voieRow = CsvVoies[jointRow.VOIE];
				if (voieRow.ID == jointRow.VOIE && !dictionary.ContainsKey(voieRow.LIGNE))
				{
					dictionary.Add(voieRow.LIGNE, jointRow.PK);
				}
			}
		}
		return dictionary;
	}

	public static void AddCircuit(SIGCircuit circuit, List<SIGDemiJoint> demiJoints)
	{
		circuit.ID = CsvCircuits.Create(circuit);
		foreach (SIGDemiJoint _demiJoint in demiJoints)
		{
			if (CsvJoints[_demiJoint.Joint.ID].ID >= 0)
			{
				JointRow jointRow = CsvJoints.Find((JointRow row) => row.VOIE == _demiJoint.Joint.Voie.ID && row.PK == _demiJoint.Joint.PK);
				if (jointRow == null)
				{
					_demiJoint.Joint.ID = CsvJoints.Create(_demiJoint.Joint);
				}
				else
				{
					_demiJoint.Joint.ID = jointRow.ID;
				}
			}
			_demiJoint.Circuit = circuit;
			CsvJointsCircuits.Create(_demiJoint);
		}
		MajUltraPOT.RefreshCircuit(circuit.ID);
		SetModif();
	}

	public static void UpdateCircuit(SIGCircuit circuit)
	{
		if (CsvCircuits[circuit.ID].Update())
		{
			MajUltraPOT.RefreshCircuit(circuit.ID);
			SetModif();
		}
	}

	public static void DeleteCircuit(int idCircuit)
	{
		CsvCircuits.Remove(idCircuit);
		CsvJointsCircuits.RemoveAll((JointCircuitRow r) => r.CIRCUIT == idCircuit);
		MajUltraPOT.RefreshCircuit(idCircuit);
		SetModif();
		DeleteModeles(idCircuit);
	}

	public static void AddJoint(SIGJoint joint)
	{
		CsvJoints.Create(joint);
		SetModif();
	}

	public static SIGJoint CreateJoint(SIGVoie voie, SIGCircuit cdvAmont, SIGCircuit cdvAval, int pk)
	{
		if (cdvAmont == null && cdvAval == null)
		{
			return null;
		}
		SIGJoint sIGJoint = new SIGJoint(-1)
		{
			Type = JointType.JI,
			PK = pk,
			Voie = voie
		};
		CsvJoints.Create(sIGJoint);
		if (cdvAmont != null)
		{
			CreateDemiJointAmont(sIGJoint, cdvAmont, principal: false);
		}
		if (cdvAval != null)
		{
			CreateDemiJointAval(sIGJoint, cdvAval, principal: false);
		}
		SetModif();
		return sIGJoint;
	}

	public static void EditJoint(SIGJoint _joint)
	{
		CsvJoints[_joint.ID].Update();
		MajUltraPOT.RefreshCircuit(CsvJointsCircuits.CircuitsIds(_joint.ID));
		SetModif();
	}

	public static void DeleteJoint(SIGJoint joint)
	{
		List<SIGDemiJoint> demiJoints = joint._demiJoints;
		CsvJoints.Remove(joint.ID);
		foreach (SIGDemiJoint item in demiJoints)
		{
			CsvJointsCircuits.Remove(item.ID);
		}
		MajUltraPOT.RefreshCircuit(demiJoints.ConvertAll((SIGDemiJoint d) => d.Circuit.ID));
		SetModif();
	}

	public static SIGDemiJoint CreateDemiJoint(SIGJoint jointOrigine, SIGCircuit cdvDestination, bool principal)
	{
		SIGDemiJoint sIGDemiJoint = new SIGDemiJoint(-1)
		{
			Circuit = cdvDestination,
			Joint = jointOrigine,
			DemiPas = 0.0,
			Principal = principal,
			DB = (jointOrigine.Type == JointType.SV || jointOrigine.Type == JointType.SVAC),
			Emetteur = cdvDestination.IsIPCS
		};
		CsvJointsCircuits.Create(sIGDemiJoint);
		jointOrigine._demiJoints.Add(sIGDemiJoint);
		return sIGDemiJoint;
	}

	public static void CreateDemiJointAval(SIGJoint jointOrigine, SIGCircuit cdvDestination, bool principal)
	{
		jointOrigine._demiJointAval = CreateDemiJoint(jointOrigine, cdvDestination, principal);
	}

	public static void CreateDemiJointAmont(SIGJoint jointOrigine, SIGCircuit cdvDestination, bool principal)
	{
		jointOrigine._demiJointAmont = CreateDemiJoint(jointOrigine, cdvDestination, principal);
	}

	public static void UpdateDemiJoint(SIGDemiJoint demiJoint)
	{
		CsvJointsCircuits.Update(demiJoint);
		SetModif();
	}

	public static void UpdateDemiJoint(SIGDemiJoint demiJoint, SIGCircuit circuit)
	{
		demiJoint.Circuit?.DemiJoints.Remove(demiJoint);
		demiJoint.Circuit = circuit;
		circuit.DemiJoints.Add(demiJoint);
		UpdateDemiJoint(demiJoint);
	}

	public static void DeleteDemiJoint(SIGDemiJoint demijoint)
	{
		SIGCircuit circuit = demijoint.Circuit;
		SIGJoint joint = demijoint.Joint;
		circuit.DemiJoints.Remove(demijoint);
		joint._demiJoints.Remove(demijoint);
		if (joint._demiJointAmont == demijoint)
		{
			joint._demiJointAmont = null;
		}
		if (joint._demiJointAval == demijoint)
		{
			joint._demiJointAval = null;
		}
		CsvJointsCircuits.Remove(demijoint.ID);
		SetModif();
	}

	public static void DeleteModele(int idModele)
	{
		CsvModeles.Remove(idModele);
		SetModif();
	}

	public static void DeleteModeles(int idCircuit)
	{
		CsvModeles.RemoveAll((ModeleRow m) => m.CIRCUIT == idCircuit);
		SetModif();
	}

	public static void VerifierBase(object parameter)
	{
		InterfaceProgressBar interfaceProgressBar = (InterfaceProgressBar)parameter;
		List<ErreurVerif> list = new List<ErreurVerif>();
		interfaceProgressBar.Maximum = 100;
		interfaceProgressBar.Texte = "Chargement de la base...";
		NeedLink();
		CsvLignes.SigLignes();
		List<SIGVoie> list2 = CsvVoies.SigVoies();
		List<SIGNoeud> list3 = CsvBranches.SigNoeuds.ToList();
		List<SIGJoint> list4 = CsvJoints.SigJoints();
		List<SIGCircuit> list5 = CsvCircuits.SigCircuits();
		List<SIGModele> list6 = CsvModeles.SigModeles();
		interfaceProgressBar.Maximum = list2.Count + list3.Count + list4.Count + list5.Count + list6.Count;
		interfaceProgressBar.Texte = "Vérification des voies...";
		foreach (SIGVoie item in list2)
		{
			if (item.Noeuds.Count < 2)
			{
				list.Add(new ErreurVerif(TypesErreurVerif.VoieSansNoeuds, item));
			}
			else
			{
				if (item.NoeudDebut == null)
				{
					list.Add(new ErreurVerif(TypesErreurVerif.VoieSansDebut, item));
				}
				else if (item.Branches.First().PK != item.PKDebut)
				{
					list.Add(new ErreurVerif(TypesErreurVerif.VoiePkDebutFaux, item));
				}
				if (item.NoeudFin == null)
				{
					list.Add(new ErreurVerif(TypesErreurVerif.VoieSansFin, item));
				}
				else if (item.Branches.Last().PK != item.PKFin)
				{
					list.Add(new ErreurVerif(TypesErreurVerif.VoiePkFinFaux, item));
				}
			}
			interfaceProgressBar.Avancement++;
		}
		interfaceProgressBar.Texte = "Vérification des noeuds...";
		foreach (SIGNoeud item2 in list3)
		{
			IEnumerable<IGrouping<int, SIGBranche>> enumerable = from b in item2.Branches
				group b by b.Voie.ID;
			if (enumerable.Count() > 3)
			{
				list.Add(new ErreurVerif(TypesErreurVerif.NoeudBranchesPlus3VoiesDifferentes, item2));
			}
			foreach (IGrouping<int, SIGBranche> item3 in enumerable)
			{
				if (item3.Count() > 2)
				{
					list.Add(new ErreurVerif(TypesErreurVerif.NoeudPlus2BranchesSurMemeVoie, item2));
				}
			}
			interfaceProgressBar.Avancement++;
		}
		interfaceProgressBar.Texte = "Vérification des joints...";
		foreach (SIGJoint item4 in list4)
		{
			if (item4.Type == JointType.INC)
			{
				list.Add(new ErreurVerif(TypesErreurVerif.JointTypeInconnu, item4));
			}
			JointTheorique jointTheorique;
			if ((jointTheorique = JointsTheoriques.GetJointTheorique(item4.Type)) != null && Math.Abs(item4.DemiLongueur - (double)jointTheorique.DemiLongueur) > 5.0)
			{
				list.Add(new ErreurVerif(TypesErreurVerif.JointLongueurNonCorrecte, item4));
			}
			if (item4.Voie == null)
			{
				list.Add(new ErreurVerif(TypesErreurVerif.JointSurAucuneVoie, item4));
			}
			else
			{
				if (item4.Voie.NoeudDebut != null && item4.PK <= item4.Voie.PKDebut)
				{
					list.Add(new ErreurVerif(TypesErreurVerif.JointAvantDebutVoie, item4));
				}
				if (item4.Voie.NoeudFin != null && item4.PK >= item4.Voie.PKFin)
				{
					list.Add(new ErreurVerif(TypesErreurVerif.JointApresFinVoie, item4));
				}
			}
			if (!item4.HasLinkedCircuit)
			{
				list.Add(new ErreurVerif(TypesErreurVerif.JointRelieAucunCircuit, item4));
			}
			foreach (SIGDemiJoint demiJoint in item4._demiJoints)
			{
				if (demiJoint?.Circuit == null)
				{
					list.Add(new ErreurVerif(TypesErreurVerif.JointLiaisonIncorrecte, item4));
				}
			}
			interfaceProgressBar.Avancement++;
		}
		interfaceProgressBar.Texte = "Vérification des circuits de voie...";
		foreach (SIGCircuit item5 in list5)
		{
			interfaceProgressBar.Avancement++;
			if (item5.Type == CircuitType.NC)
			{
				list.Add(new ErreurVerif(TypesErreurVerif.CircuitTypeInconnu, item5));
			}
			if (!CircuitTheorique.ValidFrequency(item5.Type, item5.Frequence))
			{
				list.Add(new ErreurVerif(TypesErreurVerif.CircuitFrequenceIncorrecte, item5));
			}
			if (item5.DemiJoints.Count < 2)
			{
				list.Add(new ErreurVerif(TypesErreurVerif.CircuitMoins2Joints, item5));
			}
			if (item5.DemiJoints.Count > 6)
			{
				list.Add(new ErreurVerif(TypesErreurVerif.CircuitPlusDe6Joints, item5));
			}
			bool flag = false;
			int num = 0;
			foreach (SIGDemiJoint demiJoint2 in item5.DemiJoints)
			{
				if (demiJoint2?.Joint == null)
				{
					flag = true;
					list.Add(new ErreurVerif(TypesErreurVerif.CircuitLiaisonIncorrecte, item5));
					continue;
				}
				if (item5.NeedJI && demiJoint2.Joint.Type != JointType.JI)
				{
					list.Add(new ErreurVerif(TypesErreurVerif.JointDoitEtreJI, item5));
				}
				if (demiJoint2.Principal)
				{
					num++;
				}
			}
			if (num != 2)
			{
				list.Add(new ErreurVerif(TypesErreurVerif.CircuitJointsPrincipauxDifferent2, item5));
				flag = true;
			}
			if (flag)
			{
				continue;
			}
			foreach (SIGDemiJoint demiJoint3 in item5.DemiJoints)
			{
				if (demiJoint3 != item5.DemiJointDebut && demiJoint3 != item5.DemiJointFin && CDV_Viewer.Traitements.Composants.FindParcours(demiJoint3.Joint, item5.DemiJointDebut.Joint).Count == 0 && CDV_Viewer.Traitements.Composants.FindParcours(demiJoint3.Joint, item5.DemiJointFin.Joint).Count == 0)
				{
					list.Add(new ErreurVerif(TypesErreurVerif.CircuitParcoursInterneIncorrect, item5));
				}
			}
			double num2 = (double)item5.GetLongueur() - item5.DemiJointDebut.Joint.DemiLongueur - item5.DemiJointFin.Joint.DemiLongueur;
			double num3 = num2 - item5.DemiJointDebut.DemiPas - item5.DemiJointFin.DemiPas;
			if (item5.NbPtsCompensation > 1)
			{
				if (item5.PasReel == 0.0 || item5.DemiJointDebut.DemiPas == 0.0 || item5.DemiJointFin.DemiPas == 0.0)
				{
					list.Add(new ErreurVerif(TypesErreurVerif.CircuitDonneesLongueurOuCompensationIncorrecte, item5));
				}
				else if (Math.Abs((int)(num3 / item5.PasReel) + 1 - item5.NbPtsCompensation) > 1)
				{
					list.Add(new ErreurVerif(TypesErreurVerif.CircuitDonneesLongueurOuCompensationIncorrecte, item5));
				}
			}
			else if (item5.NbPtsCompensation == 1 && Math.Abs(num3) > 10.0)
			{
				list.Add(new ErreurVerif(TypesErreurVerif.CircuitDonneesLongueurOuCompensationIncorrecte, item5));
			}
			foreach (SIGModele modele in item5.Modeles)
			{
				if (modele.DemiJointE == null)
				{
					list.Add(new ErreurVerif(TypesErreurVerif.ModeleIdJointIncorrect, modele));
				}
				if (modele.DemiJointS == null)
				{
					list.Add(new ErreurVerif(TypesErreurVerif.ModeleIdJointIncorrect, modele));
				}
				if (modele.DemiJointE != item5.DemiJointDebut || modele.DemiJointS != item5.DemiJointFin)
				{
					continue;
				}
				if (modele.Condos.Count != item5.NbPtsCompensation)
				{
					list.Add(new ErreurVerif(TypesErreurVerif.ModeleNbCondosDifferentNbCondosCircuit, modele));
				}
				if (modele.Condos.Count > 0)
				{
					int num4 = modele.Condos[0] - modele.Points[0].X;
					if (Math.Abs(modele.DemiJointE.DemiPas - (double)num4) > 5.0)
					{
						list.Add(new ErreurVerif(TypesErreurVerif.ModeleDemiPasDifferentDeDemiPasDemiJoint, modele));
					}
					int num5 = modele.Condos[modele.Condos.Count - 1] - modele.Condos[0] + 2 * num4;
					if (Math.Abs(num2 - (double)num5) > 10.0)
					{
						list.Add(new ErreurVerif(TypesErreurVerif.ModeleLongueurDifferentLongueurCircuit, modele));
					}
				}
			}
		}
		interfaceProgressBar.Texte = "Vérification des modèles...";
		foreach (SIGModele item6 in list6)
		{
			if (item6.Circuit == null)
			{
				list.Add(new ErreurVerif(TypesErreurVerif.ModeleSansCircuit, item6));
			}
			interfaceProgressBar.Avancement++;
		}
		interfaceProgressBar.Result = list;
	}

	public static void ImportCDV(List<SIGJoint> joints, List<SIGCircuit> circuits, List<SIGLinkJointCircuit> links)
	{
		List<JointRow> list = joints.ConvertAll((SIGJoint sig) => new JointRow(sig));
		CsvJoints.Add(list);
		List<CircuitRow> list2 = circuits.ConvertAll((SIGCircuit c) => new CircuitRow(c));
		CsvCircuits.Add(list2);
		List<JointCircuitRow> list3 = links.ConvertAll((SIGLinkJointCircuit l) => new JointCircuitRow(l));
		CsvJointsCircuits.Add(list3);
		SetModif();
	}
}
