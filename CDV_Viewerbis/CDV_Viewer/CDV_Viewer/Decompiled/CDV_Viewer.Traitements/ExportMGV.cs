using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Windows.Forms;
using CDV_Viewer.CsvBase;
using CDV_Viewer.Data;
using CDV_Viewer.Data.SIGObjects;
using CDV_Viewer.Forms;
using CDV_Viewer.Properties;

namespace CDV_Viewer.Traitements;

public class ExportMGV
{
	private delegate void DelegateEmpty();

	private class TimonSegment
	{
		public List<object> Elements = new List<object>();

		public TimonExtremite ExtremiteDebut
		{
			get
			{
				if (Elements.Count < 2)
				{
					return default(TimonExtremite);
				}
				SortByPK();
				if (Elements[0] is SIGJoint)
				{
					_ = (SIGJoint)Elements[0];
					return default(TimonExtremite);
				}
				return new TimonExtremite(((SIGBranche)Elements[0]).PK, "NV", db: false, 0);
			}
		}

		public TimonExtremite ExtremiteFin
		{
			get
			{
				if (Elements.Count < 2)
				{
					return default(TimonExtremite);
				}
				SortByPK();
				if (Elements[Elements.Count - 1] is SIGJoint)
				{
					_ = (SIGJoint)Elements[Elements.Count - 1];
					return default(TimonExtremite);
				}
				return new TimonExtremite(((SIGBranche)Elements[Elements.Count - 1]).PK, "NV", db: false, 0);
			}
		}

		public bool CanExport => Elements.Count >= 2;

		private void SortByPK()
		{
			Elements.Sort(delegate(object x, object y)
			{
				int num = ((!(x is SIGJoint)) ? ((SIGBranche)x).PK : ((SIGJoint)x).PK);
				int value = ((!(y is SIGJoint)) ? ((SIGBranche)y).PK : ((SIGJoint)y).PK);
				return num.CompareTo(value);
			});
		}
	}

	private struct TimonExtremite
	{
		public int PK;

		public string Type;

		public bool DB;

		public int IdJoint;

		public TimonExtremite(int pk, string type, bool db, int idJoint)
		{
			PK = pk;
			Type = type;
			DB = db;
			IdJoint = idJoint;
		}
	}

	private static string _outPath;

	private static List<int> _lignesAExporter;

	private static List<int> _voiesAExporter = new List<int>();

	private static bool _toutesLignes = true;

	private static bool _archive;

	private static bool _loc;

	private static bool _mgv;

	private static bool _ultrapot;

	private static bool _timon;

	public static event EventHandler EndExportUltraPOT;

	public static void Export(string outPath, bool archive, bool loc, bool mgv, bool ultrapot, bool timon)
	{
		_outPath = outPath;
		_toutesLignes = true;
		_archive = archive;
		_loc = loc;
		_mgv = mgv;
		_ultrapot = ultrapot;
		_timon = timon;
		Launch();
	}

	public static void Export(string outPath, List<int> lignes, bool archive, bool loc, bool mgv, bool ultrapot, bool timon)
	{
		_outPath = outPath;
		_toutesLignes = false;
		_lignesAExporter = lignes;
		_archive = archive;
		_loc = loc;
		_mgv = mgv;
		_ultrapot = ultrapot;
		_timon = timon;
		Launch();
	}

	private static void Launch()
	{
		Global.MainForm.Enabled = false;
		LoadingForm loadingForm = new LoadingForm();
		loadingForm.Show();
		new Thread(ExportThread).Start(loadingForm);
	}

	private static void ExportThread(object param)
	{
		Archives.CurrentArchive.ToTempFolder();
		LoadingForm loadingForm = (LoadingForm)param;
		int num = 0;
		if (_archive)
		{
			num++;
		}
		if (_loc)
		{
			num++;
		}
		if (_mgv)
		{
			num += 5;
		}
		if (_ultrapot)
		{
			num++;
		}
		if (_timon)
		{
			num += 3;
		}
		loadingForm.Maximum = num;
		if (_archive)
		{
			loadingForm.Texte = "Export de l'archive...";
			ExportArchive();
			loadingForm.Avancement++;
		}
		loadingForm.Texte = "Chargement de la base...";
		List<SIGLigne> list = new List<SIGLigne>();
		if (_toutesLignes)
		{
			list = Base.GetLignes();
		}
		else
		{
			foreach (SIGLigne ligne in Base.GetLignes())
			{
				if (_lignesAExporter.Contains(ligne.ID))
				{
					list.Add(ligne);
				}
			}
		}
		Base.GetBase(list, out var voies, out var _, out var joints, out var circuits, out var modeles);
		if (_ultrapot)
		{
			loadingForm.Texte = "Export UltraPOT...";
			ExportUltraPOT(circuits, _outPath + Paths.ExportUltrapot);
			loadingForm.Avancement++;
		}
		if (_loc)
		{
			loadingForm.Texte = "Export LOC NG...";
			ExportLOCNG(joints);
			loadingForm.Avancement++;
		}
		if (_timon)
		{
			loadingForm.Texte = "Export TIMON...";
			if (!CorrectsIDTimon(voies) && MessageBox.Show("Attention, certaines voies n'ont pas d'identifiant TIMON associés. Voulez-vous les renseigner ?", Resources.APP_NAME, MessageBoxButtons.YesNo) == DialogResult.Yes)
			{
				new CorrespondancesTimonDialog().ShowDialog();
			}
			ExportTIMONCircuits(circuits);
			loadingForm.Avancement++;
			ExportTIMONModeles(modeles);
			loadingForm.Avancement++;
			ExportTIMONSegments(circuits);
			loadingForm.Avancement++;
		}
		loadingForm.End = true;
		Archives.ClearTempFolder();
		MessageBox.Show("Export terminé !", Resources.APP_NAME);
		Global.MainForm.Invoke((DelegateEmpty)delegate
		{
			Global.MainForm.Enabled = true;
		});
	}

	public static void ExportArchive()
	{
		Archives.CurrentArchive.Export(_outPath + "\\CDV_VIEWER_" + DateTime.Now.ToString("ddMMyyyy_HHmmss") + ".zip");
	}

	public static void ExportMGVLignes(List<SIGLigne> lignes)
	{
		List<string> list = new List<string>();
		foreach (SIGLigne ligne in lignes)
		{
			list.Add(ligne.ID + ";" + ligne.Nom);
		}
		File.WriteAllLines(_outPath + Paths.ExportMGV_TLignes, list.ToArray());
	}

	public static void ExportMGVVoies(List<SIGVoie> voies)
	{
		List<string> list = new List<string>();
		list.Add("ID_VOIE;ID_TV;ID_LIGNE;REPERE_VOIE;PK_DEBUT;PK_FIN");
		foreach (SIGVoie voie in voies)
		{
			list.Add(voie.ID + ";" + Base.GetIdTimon(voie.ID) + ";" + voie.Ligne.ID + ";" + voie.Nom + ";" + voie.PKDebut + ";" + voie.PKFin);
		}
		File.WriteAllLines(_outPath + Paths.ExportMGV_TVoies, list.ToArray());
	}

	public static void AddChemin(SIGCircuit circuit, SIGExtremite extremiteD, SIGJoint jointF, ref int numSegment, ref int numNoeud, int profondeur, ref List<string> tJoints, ref List<string> tNDV, ref List<string> tBR, ref List<string> tSEG)
	{
		int num = profondeur + 1;
		if (num > 5)
		{
			return;
		}
		List<SIGSegment> list = Composants.FindParcours(extremiteD, jointF);
		string text = string.Empty;
		string _idJointD;
		for (int i = 0; i < list.Count - 1; i++)
		{
			int num2 = numNoeud;
			SIGNoeud noeud = ((SIGBranche)list[i].ExtremiteF).Noeud;
			tNDV.Add(noeud.MgvNoeudType + ";" + circuit.ID + ";" + num2);
			numNoeud++;
			_idJointD = string.Empty;
			if (list[i].ExtremiteD is SIGJoint)
			{
				_idJointD = ((SIGJoint)list[i].ExtremiteD).ID.ToString();
				string empty = string.Empty;
				if ((empty = tJoints.Find((string ligne) => ligne.Split(';')[0] == _idJointD)) == null)
				{
					empty = string.Concat(_idJointD, ";0;0;", ((SIGJoint)list[i].ExtremiteD).Type, ";", ((SIGJoint)list[i].ExtremiteD).Voie.ID, ";", ((SIGJoint)list[i].ExtremiteD).PK, ";", ((SIGJoint)list[i].ExtremiteD).DemiLongueur, ";False;False");
				}
				else
				{
					tJoints.RemoveAll((string ligne) => ligne.Split(';')[0] == _idJointD);
				}
				string[] array = empty.Split(';');
				string text2 = circuit.ID + numSegment.ToString("00");
				if (((SIGBranche)list[i].ExtremiteF).IsAmont)
				{
					tJoints.Add(array[0] + ";" + array[1] + ";" + text2 + ";" + array[3] + ";" + array[4] + ";" + array[5] + ";" + array[6] + ";" + array[7] + ";" + ((SIGJoint)list[i].ExtremiteD)._demiJoints.Find((SIGDemiJoint demiJoint) => demiJoint.Circuit == circuit).DB);
				}
				else
				{
					tJoints.Add(array[0] + ";" + text2 + ";" + array[2] + ";" + array[3] + ";" + array[4] + ";" + array[5] + ";" + array[6] + ";" + ((SIGJoint)list[i].ExtremiteD)._demiJoints.Find((SIGDemiJoint demiJoint) => demiJoint.Circuit == circuit).DB + ";" + array[8]);
				}
			}
			tSEG.Add(circuit.ID + ";" + numSegment + ";" + _idJointD + ";;" + text + ";" + circuit.ID.ToString() + num2.ToString() + "1");
			numSegment++;
			noeud.SortMGV((SIGBranche)list[i].ExtremiteF, (SIGBranche)list[i + 1].ExtremiteD);
			int num3 = 1;
			foreach (SIGBranche branch in noeud.Branches)
			{
				tBR.Add(string.Concat(circuit.ID.ToString(), numNoeud.ToString(), num3.ToString(), ";", branch.Voie, ";", circuit.ID.ToString(), numSegment.ToString(), ";", branch.Type));
				if (branch == (SIGBranche)list[i].ExtremiteF || branch == (SIGBranche)list[i + 1].ExtremiteD)
				{
					continue;
				}
				foreach (SIGJoint item in Composants.FindJoints(branch, branch.IsAmont ? SearchDirection.Moins : SearchDirection.Plus))
				{
					foreach (SIGSegment item2 in Composants.FindParcours(branch, item))
					{
						_ = item2;
						AddChemin(circuit, branch, item, ref numSegment, ref numNoeud, num, ref tJoints, ref tNDV, ref tBR, ref tSEG);
					}
				}
			}
			text = circuit.ID.ToString() + num2 + noeud.Count;
		}
		_idJointD = string.Empty;
		if (list[list.Count - 1].ExtremiteD is SIGJoint)
		{
			_idJointD = ((SIGJoint)list[list.Count - 1].ExtremiteD).ID.ToString();
		}
		tSEG.Add(circuit.ID + ";" + numSegment + ";" + _idJointD + ";" + ((SIGJoint)list[list.Count - 1].ExtremiteF).ID + ";" + text + ";;");
		numSegment++;
	}

	public static void ExportMGVModeles(List<SIGModele> modeles)
	{
		List<string> list = new List<string>();
		list.Add("ID_CDV;ID_JOINT_DEBUT;ID_JOINT_FIN;NOM_TOURNEE;P0;P1;...;Pn+1");
		StreamReader streamReader = new StreamReader(Paths.TModeles);
		streamReader.ReadLine();
		string[] _cells;
		while (streamReader.Peek() >= 0)
		{
			_cells = streamReader.ReadLine().Split(';');
			if (modeles.Find((SIGModele modele) => modele.ID.ToString() == _cells[0]) != null)
			{
				list.Add(_cells[1] + ";" + _cells[2] + ";" + _cells[3] + ";" + _cells[4] + ";" + _cells[5].Replace('-', ';'));
			}
		}
		streamReader.Close();
		streamReader.Dispose();
		File.WriteAllLines(_outPath + Paths.ExportMGV_TModeles, list.ToArray());
	}

	public static void ExportUltraPOT(List<SIGCircuit> circuits, string path)
	{
		List<string> list = new List<string>();
		list.Add("ID_SEGMENT;ID_CDV;FREQUENCE;TYPE;ANTENNES;REPERE_CDV;DIAPHONIE_MAX;I_FUITE_MAX;ICC_MIN;NO_SEGMENT;EXTREMITE_DEBUT;LIGNE_DEBUT;VOIE_DEBUT;PK_DEBUT;DB_DEBUT;EMETTEUR_DEBUT;EXTREMITE_FIN;LIGNE_FIN;VOIE_FIN;PK_FIN;DB_FIN;EMETTEUR_FIN;LONGUEUR;PAS_THEORIQUE;PAS_REEL;TYPE_COMPENSATION;DEMI_PAS_EXTR1;DEMI_PAS_EXTR2;L_NEUTRALISE1;L_NEUTRALISE2;NB_POINTS_COMPENSATIONS");
		int num = 0;
		foreach (SIGCircuit circuit in circuits)
		{
			bool flag = false;
			foreach (SIGDemiJoint demiJoint in circuit.DemiJoints)
			{
				if (demiJoint == null || demiJoint.Joint == null)
				{
					flag = true;
					break;
				}
			}
			if (flag)
			{
				continue;
			}
			int num2 = 1;
			foreach (SIGDemiJoint demiJoint2 in circuit.DemiJoints)
			{
				SIGDemiJoint sIGDemiJoint = null;
				SIGDemiJoint sIGDemiJoint2 = null;
				if (demiJoint2 == circuit.DemiJointDebut)
				{
					continue;
				}
				if (Composants.FindParcours(circuit.DemiJointDebut.Joint, demiJoint2.Joint).Count > 0)
				{
					sIGDemiJoint = circuit.DemiJointDebut;
					sIGDemiJoint2 = demiJoint2;
				}
				else
				{
					if (Composants.FindParcours(demiJoint2.Joint, circuit.DemiJointFin.Joint).Count <= 0)
					{
						continue;
					}
					sIGDemiJoint = demiJoint2;
					sIGDemiJoint2 = circuit.DemiJointFin;
				}
				List<SIGSegment> list2 = Composants.FindParcours(sIGDemiJoint.Joint, sIGDemiJoint2.Joint);
				int num3 = CircuitTheorique.PasTheorique(circuit.Compensation, circuit.Frequence);
				list.Add(num + ";" + circuit.ID + ";" + circuit.Frequence + ";" + circuit.Type.ToString().Replace("TVM", "") + ";" + WriteBoolean(list2.Count > 1) + ";" + circuit.Nom + ";" + circuit.Diaphonie + ";" + circuit.IFuite + ";" + circuit.ICC + ";" + num2 + ";" + sIGDemiJoint.Joint.Type.ToString() + ";" + sIGDemiJoint.Joint.Voie.Ligne.ID + ";" + sIGDemiJoint.Joint.Voie.Nom + ";" + sIGDemiJoint.Joint.PK + ";" + sIGDemiJoint.DB.ToString() + ";" + sIGDemiJoint.Emetteur.ToString() + ";" + sIGDemiJoint2.Joint.Type.ToString() + ";" + sIGDemiJoint2.Joint.Voie.Ligne.ID + ";" + sIGDemiJoint2.Joint.Voie.Nom + ";" + sIGDemiJoint2.Joint.PK + ";" + sIGDemiJoint2.DB.ToString() + ";" + sIGDemiJoint2.Emetteur.ToString() + ";" + SIGSegment.GetLongueur(list2) + ";" + num3 + ";" + circuit.PasReel + ";" + circuit.Compensation.ToString() + ";" + sIGDemiJoint.DemiPas + ";" + sIGDemiJoint2.DemiPas + ";" + sIGDemiJoint.Joint.DemiLongueur + ";" + sIGDemiJoint2.Joint.DemiLongueur + ";" + circuit.NbPtsCompensation);
				num++;
				num2++;
			}
		}
		File.WriteAllLines(path, list.ToArray());
	}

	private static string WriteBoolean(bool valeur)
	{
		if (valeur)
		{
			return "O";
		}
		return "N";
	}

	private static void ExportLOCNG(List<SIGJoint> joints)
	{
		List<string> list = new List<string>();
		list.Add("LIGNE;VOIE;LONGUEUR;FREQUENCE");
		foreach (SIGJoint joint in joints)
		{
			foreach (SIGDemiJoint demiJoint in joint._demiJoints)
			{
				if (demiJoint.Circuit != null && demiJoint.Circuit.Frequence > 1000)
				{
					list.Add(joint.Voie.Ligne.ID + ";" + joint.Voie.Nom + ";" + (int)Math.Round((double)joint.PK - joint.DemiLongueur) + ";" + demiJoint.Circuit.Frequence);
				}
			}
		}
		File.WriteAllLines(_outPath + Paths.ExportLoc, list.ToArray());
	}

	private static bool CorrectsIDTimon(List<SIGVoie> voies)
	{
		Dictionary<int, int> idsTimon = Base.GetIdsTimon();
		foreach (SIGVoie voie in voies)
		{
			if (!idsTimon.ContainsKey(voie.ID))
			{
				return false;
			}
		}
		return true;
	}

	public static void ExportTIMONCircuits(List<SIGCircuit> circuits)
	{
		List<string> list = new List<string>();
		list.Add("c;A_REPERE;A_TYPE_CDV;A_FREQUENCE;A_ANTENNE;N_ICC_MIN;N_DIAPHONIE_L_MAX;N_DIAPHONIE_T_MAX;A_COMPENSE;N_NB_PT_COMPENSATION;N_NB_CONDENSATEUR;N_FUITE_LONG_ARR");
		foreach (SIGCircuit circuit in circuits)
		{
			bool flag = false;
			foreach (SIGDemiJoint demiJoint in circuit.DemiJoints)
			{
				if (demiJoint == null || demiJoint.Joint == null)
				{
					flag = true;
					break;
				}
			}
			if (!flag && CircuitTheorique.ValidFrequency(circuit.Type, circuit.Frequence) && Composants.FindParcours(circuit.DemiJointDebut.Joint, circuit.DemiJointFin.Joint).Count > 0)
			{
				string text = circuit.ID + ";";
				text = text + circuit.Nom + ";";
				text = string.Concat(text, circuit.Type, ";");
				text = text + circuit.Frequence + ";";
				text = text + WriteBoolean(circuit.DemiJoints.Count > 2) + ";";
				text = text + circuit.ICC + ";";
				text = text + circuit.IFuite + ";";
				text = text + circuit.Diaphonie + ";";
				text = text + WriteBoolean(circuit.NbPtsCompensation > 0) + ";";
				text = text + circuit.NbPtsCompensation + ";";
				int num = circuit.NbPtsCompensation;
				if (circuit.DemiJointDebut.DB)
				{
					num--;
				}
				if (circuit.DemiJointFin.DB)
				{
					num--;
				}
				if (num < 0)
				{
					num = 0;
				}
				text = text + num + ";";
				text += circuit.N_FUITE_LONG_ARR;
				list.Add(text);
			}
		}
		File.WriteAllLines(_outPath + Paths.ExportTIMON_CDV, list.ToArray());
	}

	public static void ExportTIMONModeles(List<SIGModele> modeles)
	{
		List<string> list = new List<string>();
		list.Add("ID_CDV;ID_JOINT_DEBUT;ID_JOINT_FIN;NOM_TOURNEE;P0,P1,...,Pn+1");
		bool flag = true;
		string[] array = File.ReadAllLines(Paths.TModeles);
		string[] _cells;
		foreach (string text in array)
		{
			if (flag)
			{
				flag = false;
				continue;
			}
			_cells = text.Split(';');
			if (modeles.Find((SIGModele modele) => modele.ID.ToString() == _cells[0]) != null)
			{
				list.Add(_cells[1] + ";" + _cells[2] + ";" + _cells[3] + ";" + _cells[4] + ";" + _cells[5].Replace('-', ','));
			}
		}
		File.WriteAllLines(_outPath + Paths.ExportTIMON_MODELES, list.ToArray());
	}

	public static void ExportTIMONSegments(List<SIGCircuit> circuits)
	{
		List<string> list = new List<string>();
		list.Add("N_ID_SEG_CDV;N_NUM_TR_VOIE;N_ID_CDV;N_NUM_SEGMENT;N_PK_DEBUT;N_PK_FIN;A_EXTREMITE_1;A_EXTREMITE_2;A_EMETTEUR_DEBUT;A_EMETTEUR_FIN;N_LONGUEUR;A_PRESENCE_DB1;A_PRESENCE_DB2;ID_JOINT_DEBUT;ID_JOINT_FIN");
		Dictionary<int, int> idsTimon = Base.GetIdsTimon();
		int num = 0;
		int num2 = 0;
		foreach (SIGCircuit circuit in circuits)
		{
			int num3 = 1;
			List<SIGSegment> segments = new List<SIGSegment>();
			for (int i = 0; i < circuit.DemiJoints.Count; i++)
			{
				for (int j = i + 1; j < circuit.DemiJoints.Count; j++)
				{
					segments.AddRange(Composants.FindParcours(circuit.DemiJoints[i].Joint, circuit.DemiJoints[j].Joint));
				}
			}
			SIGSegment.Merge(ref segments);
			foreach (SIGSegment _segment in segments)
			{
				if (!idsTimon.TryGetValue(_segment.Voie.ID, out var value) || value <= 0)
				{
					continue;
				}
				if (_segment.PkD > _segment.PkF)
				{
					SIGExtremite extremiteD = _segment.ExtremiteD;
					_segment.ExtremiteD = _segment.ExtremiteF;
					_segment.ExtremiteF = extremiteD;
				}
				string text = "NV";
				string text2 = "NV";
				bool valeur = false;
				bool valeur2 = false;
				bool valeur3 = false;
				bool valeur4 = false;
				int num4 = 0;
				int num5 = 0;
				if (_segment.ExtremiteD is SIGJoint)
				{
					num4 = ((SIGJoint)_segment.ExtremiteD).ID;
					text = ((SIGJoint)_segment.ExtremiteD).Type.ToString();
					SIGDemiJoint sIGDemiJoint = circuit.DemiJoints.Find((SIGDemiJoint demiJoint) => demiJoint.Joint == _segment.ExtremiteD);
					if (sIGDemiJoint != null)
					{
						valeur = sIGDemiJoint.Emetteur;
						valeur3 = sIGDemiJoint.DB;
					}
				}
				if (_segment.ExtremiteF is SIGJoint)
				{
					num5 = ((SIGJoint)_segment.ExtremiteF).ID;
					text2 = ((SIGJoint)_segment.ExtremiteF).Type.ToString();
					SIGDemiJoint sIGDemiJoint2 = circuit.DemiJoints.Find((SIGDemiJoint demiJoint) => demiJoint.Joint == _segment.ExtremiteF);
					if (sIGDemiJoint2 != null)
					{
						valeur2 = sIGDemiJoint2.Emetteur;
						valeur4 = sIGDemiJoint2.DB;
					}
				}
				num2 = num * 100 + num3;
				list.Add(num2 + ";" + value + ";" + circuit.ID + ";" + num3 + ";" + WriteTimonPK(_segment.PkD) + ";" + WriteTimonPK(_segment.PkF) + ";" + text + ";" + text2 + ";" + WriteBoolean(valeur) + ";" + WriteBoolean(valeur2) + ";" + Math.Abs(_segment.PkF - _segment.PkD) + ";" + WriteBoolean(valeur3) + ";" + WriteBoolean(valeur4) + ";" + num4 + ";" + num5);
				num3++;
			}
			num++;
		}
		File.WriteAllLines(_outPath + Paths.ExportTIMON_SEGMENTS, list.ToArray());
	}

	public static void OldExportTIMONSegments()
	{
	}

	private static string WriteTimonPK(int pk)
	{
		string text = pk.ToString();
		if (Math.Abs(pk) < 1000)
		{
			return text;
		}
		string text2 = text.Substring(0, text.Length - 3);
		string text3 = text.Substring(text.Length - 3);
		return text2 + "00" + text3;
	}

	public static List<SIGLigne> GetSelectedLignes(List<SIGLigne> lignes)
	{
		List<SIGLigne> list = new List<SIGLigne>();
		foreach (SIGLigne ligne in lignes)
		{
			if (_lignesAExporter.Contains(ligne.ID))
			{
				list.Add(ligne);
			}
		}
		return list;
	}

	public static List<SIGVoie> GetSelectedVoies(List<SIGVoie> voies)
	{
		List<SIGVoie> list = new List<SIGVoie>();
		_voiesAExporter.Clear();
		foreach (SIGVoie voie in voies)
		{
			if (!_lignesAExporter.Contains(voie.Ligne.ID))
			{
				_voiesAExporter.Add(voie.ID);
				list.Add(voie);
			}
		}
		return list;
	}

	public static List<SIGCircuit> GetSelectedCircuits(List<SIGCircuit> circuits)
	{
		return new List<SIGCircuit>();
	}

	public static List<SIGJoint> GetSelectedJoints(List<SIGJoint> joints)
	{
		if (_voiesAExporter.Count == 0)
		{
			GetSelectedVoies(Base.GetVoies());
		}
		List<SIGJoint> list = new List<SIGJoint>();
		foreach (SIGJoint joint in joints)
		{
			if (_voiesAExporter.Contains(joint.Voie.ID))
			{
				list.Add(joint);
			}
		}
		return list;
	}

	public static List<string> AddJoint(List<string> TJoints, string joint)
	{
		bool flag = false;
		for (int i = 0; i < TJoints.Count; i++)
		{
			if (TJoints[i].Split(';')[0] == joint.Split(';')[0])
			{
				string[] array = TJoints[i].Split(';');
				string[] array2 = joint.Split(';');
				if (array2[1] == "0")
				{
					TJoints[i] = array[0] + ";" + array[1] + ";" + array2[2] + ";" + array[3] + ";" + array[4] + ";" + array[5] + ";" + array[6] + ";" + array[7] + ";" + array[8];
				}
				else
				{
					TJoints[i] = array[0] + ";" + array2[1] + ";" + array[2] + ";" + array[3] + ";" + array[4] + ";" + array[5] + ";" + array[6] + ";" + array[7] + ";" + array[8];
				}
				flag = true;
			}
		}
		if (!flag)
		{
			TJoints.Add(joint);
		}
		return TJoints;
	}
}
