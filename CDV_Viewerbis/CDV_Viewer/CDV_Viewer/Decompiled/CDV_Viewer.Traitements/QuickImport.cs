using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;
using CDV_Viewer.CsvBase;
using CDV_Viewer.Data.SIGObjects;
using CDV_Viewer.DockControls;
using CDV_Viewer.Forms;
using CDV_Viewer.Properties;

namespace CDV_Viewer.Traitements;

public static class QuickImport
{
	public class ImportData
	{
		public string _nom;

		public int _pkD;

		public int _pkF;

		public CircuitType _type;

		public int _frequence;

		public JointType _typeD;

		public JointType _typeF;

		public static ImportData Create(int _numLigne, string _csvLine, out bool BadFormat)
		{
			BadFormat = false;
			if (string.IsNullOrWhiteSpace(_csvLine))
			{
				return null;
			}
			string[] array = _csvLine.Split(';');
			if (array.Length != 7)
			{
				MessageBox.Show("Format incorrect (ligne " + _numLigne, Resources.APP_NAME);
				return null;
			}
			if (array[1] == "PK_DEBUT")
			{
				return null;
			}
			ImportData importData = new ImportData();
			importData._nom = array[0];
			if (importData._nom == string.Empty)
			{
				MessageBox.Show("Erreur à la ligne " + _numLigne + " (NOM)", Resources.APP_NAME);
				BadFormat = true;
				return null;
			}
			if (!int.TryParse(array[1], out importData._pkD))
			{
				MessageBox.Show("Erreur à la ligne " + _numLigne + " (PK_DEBUT)", Resources.APP_NAME);
				BadFormat = true;
				return null;
			}
			if (!int.TryParse(array[2], out importData._pkF))
			{
				MessageBox.Show("Erreur à la ligne " + _numLigne + " (PK_FIN)", Resources.APP_NAME);
				BadFormat = true;
				return null;
			}
			if (!int.TryParse(array[4], out importData._frequence))
			{
				importData._frequence = 0;
			}
			if (!Enum.TryParse<CircuitType>(array[3], out importData._type))
			{
				importData._type = CircuitType.ITE;
				importData._frequence = 0;
			}
			if (!Enum.TryParse<JointType>(array[5], out importData._typeD))
			{
				importData._typeD = JointType.CC;
			}
			if (!Enum.TryParse<JointType>(array[6], out importData._typeF))
			{
				importData._typeF = JointType.CC;
			}
			return importData;
		}
	}

	public static void Import(string filePath, int voie)
	{
		List<ImportData> list = new List<ImportData>();
		int numLigne = 1;
		string[] array = File.ReadAllLines(filePath);
		foreach (string csvLine in array)
		{
			bool BadFormat;
			ImportData importData = ImportData.Create(numLigne, csvLine, out BadFormat);
			if (BadFormat)
			{
				return;
			}
			if (importData != null)
			{
				list.Add(importData);
			}
		}
		if (list.Count < 1)
		{
			return;
		}
		int FirstPK = list[0]._pkD;
		int LastPK = list[list.Count - 1]._pkF;
		SIGVoie voie2 = Base.GetVoie(voie);
		List<SIGJoint> list2 = voie2.Joints.FindAll((SIGJoint _joint) => _joint.PK >= FirstPK && _joint.PK <= LastPK);
		List<SIGJoint> list3 = new List<SIGJoint>();
		if (list2.Count > 2)
		{
			MessageBox.Show("Il existe des joint entre " + Chaines.PkToString(FirstPK) + " et " + Chaines.PkToString(LastPK), Resources.APP_NAME);
			return;
		}
		if (list2.Count == 1)
		{
			if (list2[0].PK == FirstPK && list2[0].DemiJointAval != null)
			{
				list3.Add(list2[0]);
			}
			else
			{
				if (list2[0].PK != LastPK || list2[0].DemiJointAmont == null)
				{
					MessageBox.Show("Il existe des joint entre " + Chaines.PkToString(FirstPK) + " et " + Chaines.PkToString(LastPK), Resources.APP_NAME);
					return;
				}
				list3.Add(list2[0]);
			}
		}
		if (list2.Count == 2)
		{
			if (list2[0].PK != FirstPK || list2[0].DemiJointAval == null || list2[1].PK != LastPK || list2[1].DemiJointAmont == null)
			{
				MessageBox.Show("Il existe des joint entre " + Chaines.PkToString(FirstPK) + " et " + Chaines.PkToString(LastPK), Resources.APP_NAME);
				return;
			}
			list3.Add(list2[0]);
			list3.Add(list2[1]);
		}
		List<SIGCircuit> list4 = new List<SIGCircuit>();
		List<SIGLinkJointCircuit> list5 = new List<SIGLinkJointCircuit>();
		foreach (ImportData _data in list)
		{
			SIGJoint sIGJoint = list3.Find((SIGJoint joint) => joint.PK == _data._pkD);
			SIGJoint sIGJoint2 = list3.Find((SIGJoint joint) => joint.PK == _data._pkF);
			if (sIGJoint == null)
			{
				int iD = Base.CsvJoints.FreeId();
				sIGJoint = new SIGJoint
				{
					ID = iD,
					Voie = voie2,
					PK = _data._pkD,
					Type = _data._typeD
				};
				list3.Add(sIGJoint);
			}
			if (sIGJoint2 == null)
			{
				int iD2 = Base.CsvJoints.FreeId();
				sIGJoint2 = new SIGJoint
				{
					ID = iD2,
					Voie = voie2,
					PK = _data._pkF,
					Type = _data._typeF
				};
				list3.Add(sIGJoint2);
			}
			SIGCircuit sIGCircuit = SIGCircuit.Create(_data._type, _data._frequence, new List<SIGJoint>(new SIGJoint[2] { sIGJoint, sIGJoint2 }));
			sIGCircuit.ID = Base.CsvCircuits.FreeId();
			sIGCircuit.Nom = _data._nom;
			sIGCircuit.Type = _data._type;
			sIGCircuit.Frequence = _data._frequence;
			sIGJoint.DemiJointAval.Emetteur = true;
			sIGJoint.DemiJointAmont.Emetteur = true;
			list4.Add(sIGCircuit);
			SIGLinkJointCircuit item = new SIGLinkJointCircuit(Base.CsvJointsCircuits.FreeId(), sIGJoint.ID, sIGCircuit.ID, principal: true);
			list5.Add(item);
			SIGLinkJointCircuit item2 = new SIGLinkJointCircuit(Base.CsvJointsCircuits.FreeId(), sIGJoint2.ID, sIGCircuit.ID, principal: true);
			list5.Add(item2);
		}
		if (new QuickImportDialog(list3, list4, list5).ShowDialog() != DialogResult.OK)
		{
			return;
		}
		foreach (SIGCircuit _circuit in list4)
		{
			List<SIGLinkJointCircuit> _linksCircuit = list5.FindAll((SIGLinkJointCircuit link) => link.Circuit == _circuit.ID);
			if (_linksCircuit.Count != 2)
			{
				continue;
			}
			SIGJoint sIGJoint3 = list3.Find((SIGJoint joint) => joint.ID == _linksCircuit[0].Joint);
			SIGJoint sIGJoint4 = list3.Find((SIGJoint joint) => joint.ID == _linksCircuit[1].Joint);
			if (sIGJoint3 == null || sIGJoint4 == null)
			{
				continue;
			}
			foreach (SIGNoeud noeud in voie2.Noeuds)
			{
				foreach (SIGBranche branch in noeud.Branches)
				{
					if (branch.Voie == voie2)
					{
						if (branch.PK < sIGJoint3.PK || branch.PK > sIGJoint4.PK)
						{
							break;
						}
						continue;
					}
					SIGVoie _jonction = branch.Voie;
					if (_jonction.IsJonction())
					{
						List<SIGJoint> list6 = _jonction.Joints.FindAll((SIGJoint j) => j.PK > _jonction.PKDebut && j.PK < _jonction.PKFin);
						if (list6.Count > 1)
						{
							MessageBox.Show("Il existe des joint sur la jonction " + _jonction, Resources.APP_NAME);
						}
						SIGJoint sIGJoint5;
						if (list6.Count == 0)
						{
							int iD3 = Base.CsvJoints.FreeId();
							sIGJoint5 = new SIGJoint
							{
								ID = iD3,
								Voie = voie2,
								PK = (voie2.PKDebut + voie2.PKFin) / 2,
								Type = JointType.JI
							};
							list3.Add(sIGJoint5);
						}
						else
						{
							sIGJoint5 = list6[0];
						}
						list5.Add(new SIGLinkJointCircuit(Base.CsvJointsCircuits.FreeId(), sIGJoint5.ID, _circuit.ID, principal: false));
					}
				}
			}
		}
		Base.ImportCDV(list3, list4, list5);
		MessageBox.Show("Import terminé !", Resources.APP_NAME);
		ComposantsViewer.Viewer.RefreshLigne();
	}
}
