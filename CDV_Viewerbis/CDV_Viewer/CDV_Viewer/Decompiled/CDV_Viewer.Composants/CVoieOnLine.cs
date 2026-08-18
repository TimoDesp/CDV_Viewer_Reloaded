using System;
using System.Collections.Generic;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;
using CDV_Viewer.Controls;
using CDV_Viewer.CsvBase;
using CDV_Viewer.Data;
using CDV_Viewer.Data.SIGObjects;
using CDV_Viewer.PopupForms;
using CDV_Viewer.Properties;
using CDV_Viewer.Traitements;

namespace CDV_Viewer.Composants;

public abstract class CVoieOnLine : CVoie
{
	public enum TypesVoie
	{
		Principale,
		Jonction
	}

	protected delegate void CircuitEditPopupFormDelegate(CircuitEditPopupForm popupForm);

	protected int _pkD;

	protected int _pkF;

	protected int _posY;

	protected bool _composantsLoaded;

	protected CNoeud _noeudDebut;

	protected CVoieOnLine _voieDebut;

	protected CNoeud _noeudFin;

	protected CVoieOnLine _voieFin;

	protected UnscaledLocation[] _unscaledfullPath;

	public TypesVoie Type { get; protected set; }

	public virtual int PosY => _posY;

	public CNoeud NoeudDebut => _noeudDebut;

	public CVoieOnLine VoieDebut => _voieDebut;

	public CNoeud NoeudFin => _noeudFin;

	public CVoieOnLine VoieFin => _voieFin;

	public bool HasVoieDebut => _voieDebut != null;

	public bool HasVoieFin => _voieFin != null;

	public override int PosYDebut => _voieDebut?.PosY ?? PosY;

	public override int PosYFin => _voieFin?.PosY ?? PosY;

	public int PkSelected { get; protected set; }

	public override bool Contains(Point Pt, int pkPoint)
	{
		if (pkPoint < _pkD || pkPoint > _pkF)
		{
			return false;
		}
		return _displayPath.Contains(Pt);
	}

	public override bool IsInGraph()
	{
		if (base.ComposantsViewer.PkD > _pkF)
		{
			return false;
		}
		if (base.ComposantsViewer.PkF < _pkD)
		{
			return false;
		}
		return true;
	}

	public override PopupForm GetPropertyWindow()
	{
		return new DisplayNomPksPopupForm
		{
			Nom = _voie.Nom,
			PkD = _voie.PKDebut,
			PkF = _voie.PKFin
		};
	}

	internal UnscaledLocation GetUnscaledPoint(int pk)
	{
		if (_unscaledfullPath == null)
		{
			RecalculPKs();
		}
		int num = _unscaledfullPath.Length - 1;
		if (pk <= _pkD)
		{
			return _unscaledfullPath[0];
		}
		if (pk < _pkF)
		{
			UnscaledLocation p = _unscaledfullPath[0];
			for (int i = 1; i <= num; i++)
			{
				UnscaledLocation unscaledLocation = _unscaledfullPath[i];
				if (pk < unscaledLocation.Pk)
				{
					return UnscaledLocation.Interpolated(pk, p, unscaledLocation);
				}
				p = unscaledLocation;
			}
		}
		return _unscaledfullPath[num];
	}

	public Point[] GetFullPath(Converter<UnscaledLocation, Point> Scaling)
	{
		return Array.ConvertAll(_unscaledfullPath, Scaling);
	}

	public override Point[] GetPath(int pkd, int pkf)
	{
		if (pkd < base.ComposantsViewer.PkD)
		{
			pkd = base.ComposantsViewer.PkD;
		}
		if (pkf > base.ComposantsViewer.PkF)
		{
			pkf = base.ComposantsViewer.PkF;
		}
		if (pkd < _pkD)
		{
			pkd = _pkD;
		}
		if (pkf > _pkF)
		{
			pkf = _pkF;
		}
		Point point = internalGetPoint(pkd);
		Point point2 = internalGetPoint(pkf);
		if (_unscaledfullPath.Length < 3)
		{
			return new Point[2] { point, point2 };
		}
		UnscaledLocation[] array = Array.FindAll(_unscaledfullPath, (UnscaledLocation p) => p.Pk > pkd && p.Pk < pkf);
		if (array != null && array.Length == 0)
		{
			return new Point[2] { point, point2 };
		}
		Point[] array2 = new Point[array.Length + 2];
		int num = 0;
		array2[num++] = point;
		UnscaledLocation[] array3 = array;
		foreach (UnscaledLocation unscaledLocation in array3)
		{
			array2[num++] = unscaledLocation.Scale();
		}
		array2[num] = point2;
		return array2;
	}

	public override Point GetPoint(int pk)
	{
		if (pk < base.ComposantsViewer.PkD)
		{
			pk = base.ComposantsViewer.PkD;
		}
		if (pk > base.ComposantsViewer.PkF)
		{
			pk = base.ComposantsViewer.PkF;
		}
		return internalGetPoint(pk);
	}

	public Point GetPoint(int pk, out double angle)
	{
		if (pk < base.ComposantsViewer.PkD)
		{
			pk = base.ComposantsViewer.PkD;
		}
		if (pk > base.ComposantsViewer.PkF)
		{
			pk = base.ComposantsViewer.PkF;
		}
		return internalGetPoint(pk, out angle);
	}

	internal Point internalGetPoint(int pk)
	{
		if (_unscaledfullPath == null)
		{
			RecalculPKs();
		}
		int num = _unscaledfullPath.Length - 1;
		if (pk <= _pkD)
		{
			return _unscaledfullPath[0].Scale();
		}
		if (pk < _pkF)
		{
			UnscaledLocation p = _unscaledfullPath[0];
			for (int i = 1; i <= num; i++)
			{
				UnscaledLocation unscaledLocation = _unscaledfullPath[i];
				if (pk < unscaledLocation.Pk)
				{
					return UnscaledLocation.InterpolatedScaled(pk, p, unscaledLocation);
				}
				p = unscaledLocation;
			}
		}
		return _unscaledfullPath[num].Scale();
	}

	internal Point internalGetPoint(int pk, out double angle)
	{
		if (_unscaledfullPath == null)
		{
			RecalculPKs();
		}
		angle = 0.0;
		int num = _unscaledfullPath.Length - 1;
		if (pk <= _pkD)
		{
			return _unscaledfullPath[0].Scale();
		}
		if (pk < _pkF)
		{
			UnscaledLocation p = _unscaledfullPath[0];
			for (int i = 1; i <= num; i++)
			{
				UnscaledLocation unscaledLocation = _unscaledfullPath[i];
				if (pk < unscaledLocation.Pk)
				{
					return UnscaledLocation.InterpolatedScaled(pk, p, unscaledLocation, out angle);
				}
				p = unscaledLocation;
			}
		}
		return _unscaledfullPath[num].Scale();
	}

	public override void OnComposantsLoaded()
	{
		_composantsLoaded = true;
		_noeudDebut = base.Composants.GetNoeud(base.Voie.NoeudDebut);
		_noeudFin = base.Composants.GetNoeud(base.Voie.NoeudFin);
		CVoieOnLine cVoieOnLine = (CVoieOnLine)(_noeudDebut?.Support);
		CVoieOnLine cVoieOnLine2 = (CVoieOnLine)(_noeudFin?.Support);
		SIGBranche brancheAmont = base.Voie.NoeudDebut.BrancheAmont;
		SIGBranche brancheAval = base.Voie.NoeudFin.BrancheAval;
		if (cVoieOnLine == null || base.Voie.NoeudDebut.BranchesInTrack(base.Voie).Count > 1)
		{
			Dialogs.BaseError($"incoherence du Noeud ({brancheAmont.Noeud})", base.Voie, brancheAmont.PK);
		}
		if (cVoieOnLine2 == null || base.Voie.NoeudFin.BranchesInTrack(base.Voie).Count > 1)
		{
			Dialogs.BaseError($"incoherence du Noeud ({brancheAval.Noeud})", base.Voie, brancheAval.PK);
		}
		if (cVoieOnLine == this)
		{
			cVoieOnLine = null;
		}
		if (cVoieOnLine2 == this)
		{
			cVoieOnLine2 = null;
		}
		if (brancheAmont?.Voie == base.Voie)
		{
			brancheAmont = base.Voie.NoeudDebut.BrancheAval;
			if (brancheAmont?.Voie == base.Voie)
			{
				Dialogs.BaseError($"incoherence du Noeud ({brancheAmont.Noeud})", base.Voie, brancheAmont.PK);
				brancheAmont = null;
			}
		}
		if (brancheAval?.Voie == base.Voie)
		{
			brancheAval = base.Voie.NoeudFin.BrancheAmont;
			if (brancheAval?.Voie == base.Voie)
			{
				Dialogs.BaseError($"incoherence du Noeud ({brancheAval.Noeud})", base.Voie, brancheAval.PK);
				brancheAval = null;
			}
		}
		_voieDebut = cVoieOnLine;
		_voieFin = cVoieOnLine2;
		RecalculPKs();
	}

	protected override void OnMouseClick(MouseEventArgs e)
	{
		if (e.Button == MouseButtons.Right)
		{
			PkSelected = base.ComposantsViewer.LocationToPk(e.X);
		}
	}

	protected override void OnMouseDoubleClick(MouseEventArgs e)
	{
		Proprietes();
	}

	protected void AddCommonMenu(ComposantMenu _menu)
	{
		_menu.AddItem("Modifier...", Resources.Edit, Modifier, AuthorizedMode.Edit);
		_menu.AddItem("Supprimer...", Resources.Delete, Supprimer, AuthorizedMode.Edit);
		_menu.AddItem("Propriétés...", Resources.Properties, Proprietes, AuthorizedMode.Read);
	}

	protected void AddCdvItems(ComposantMenu _menu, ToolStripMenuItem insertMenu)
	{
		if (base.ComposantsViewer.ModeVisualisation == ModeVisualisation.Signalisation && base.Voie.GetCircuit(PkSelected) == null)
		{
			_menu.AddDropDownItem(insertMenu, "-", AuthorizedMode.Edit);
			_menu.AddDropDownItem(insertMenu, "Joint...", InsertJoint, AuthorizedMode.Edit);
			_menu.AddDropDownItem(insertMenu, "Circuit de Voie...", InsertCircuit, AuthorizedMode.Edit);
		}
	}

	protected void InsertTJD()
	{
		RelierNoeudPopupForm relierNoeudPopupForm = new RelierNoeudPopupForm(new SIGExtremite(_voie, PkSelected));
		relierNoeudPopupForm.Closing += TjdPopupForm_Closed;
		base.ComposantsViewer.PopupContainer.Show(relierNoeudPopupForm, "Creation TJD");
	}

	protected void TjdPopupForm_Closed(object sender, PopupFormResultEventArgs e)
	{
		if (e.Result != PopupContainerResult.OK)
		{
			return;
		}
		RelierNoeudPopupForm relierNoeudPopupForm = sender as RelierNoeudPopupForm;
		SIGVoie voieDestination = relierNoeudPopupForm.VoieDestination;
		if (voieDestination == null)
		{
			MessageBox.Show("Voie destination Inconnue");
			return;
		}
		int pkOrigine = relierNoeudPopupForm.PkOrigine;
		int pkDestination = relierNoeudPopupForm.PkDestination;
		SIGNoeud sIGNoeud = Base.CreateNoeud(_voie, pkDestination);
		if (!Base.TryConnectNoeudToVoie(sIGNoeud, voieDestination, pkDestination, out var error))
		{
			MessageBox.Show(error);
			Base.DeleteNoeud(sIGNoeud);
			return;
		}
		SIGCircuit circuit = voieDestination.GetCircuit(pkDestination);
		if (circuit == null)
		{
			base.ComposantsViewer.RefreshLigne();
			return;
		}
		sIGNoeud.GetJointsAround();
		SIGNoeud.NoeudType type = sIGNoeud.Type;
		if (type == SIGNoeud.NoeudType.Heurtoir)
		{
			SIGBranche sIGBranche = sIGNoeud.FirstBrancheInTrack(_voie);
			SIGJoint nearestJoint = sIGBranche.GetNearestJoint();
			if (sIGBranche.IsBrancheDebut)
			{
				if (nearestJoint != null && nearestJoint.PK - pkOrigine < 1000)
				{
					Base.CreateDemiJointAval(nearestJoint, circuit, principal: false);
				}
				else
				{
					Base.CreateJoint(_voie, null, circuit, pkOrigine + 100);
				}
			}
			else if (nearestJoint != null && pkOrigine - nearestJoint.PK < 1000)
			{
				Base.CreateDemiJointAmont(nearestJoint, circuit, principal: false);
			}
			else
			{
				Base.CreateJoint(_voie, circuit, null, pkOrigine - 100);
			}
		}
		base.ComposantsViewer.RefreshLigne();
	}

	protected void InsertJoint()
	{
		base.Composants.AddOperation(new CInsertJoint(this, PkSelected));
	}

	protected void InsertCircuit()
	{
		List<SIGJoint> list = new List<SIGJoint>();
		CDV_Viewer.Traitements.Composants.GetJointsAround(new SIGExtremite(_voie, PkSelected), list);
		if (list.Count < 2)
		{
			Dialogs.Message("Moins de 2 joints ont étés trouvés aux alentours. Il est donc impossible d'insérer un circuit de voie.");
			return;
		}
		if (list.Contains(null))
		{
			Dialogs.Message("Impossible d'insérer un circuit de voie, il manque des joints.");
			return;
		}
		SelectJointPrincipauxPopupForm _jPopupForm = new SelectJointPrincipauxPopupForm(list);
		_jPopupForm.Closed += delegate(object o, PopupFormResultEventArgs result)
		{
			JointPopupForm_Closed(_jPopupForm, result);
		};
		base.ComposantsViewer.PopupContainer.Show(_jPopupForm, "SÉLECTION DES JOINTS PRINCIPAUX", PopupContainerButtons.Valider);
	}

	protected void JointPopupForm_Closed(SelectJointPrincipauxPopupForm _jPopupForm, PopupFormResultEventArgs e)
	{
		if (e.Result == PopupContainerResult.OK)
		{
			CircuitEditPopupForm _cPopupForm = new CircuitEditPopupForm();
			_cPopupForm.Tag = _jPopupForm;
			_cPopupForm.Closed += delegate(object o, PopupFormResultEventArgs result)
			{
				CircuitPopupForm_Closed(_cPopupForm, result);
			};
			new Thread(OpenCircuitPopupForm).Start(_cPopupForm);
		}
	}

	protected void OpenCircuitPopupForm(object param)
	{
		base.ComposantsViewer.Invoke(new CircuitEditPopupFormDelegate(OpenCircuitPopupForm), (CircuitEditPopupForm)param);
	}

	protected void OpenCircuitPopupForm(CircuitEditPopupForm popupForm)
	{
		base.ComposantsViewer.PopupContainer.Show(popupForm, "CONFIRMER L'AJOUT", PopupContainerButtons.Valider);
	}

	protected void CircuitPopupForm_Closed(CircuitEditPopupForm _cPopupForm, PopupFormResultEventArgs e)
	{
		if (e.Result != PopupContainerResult.OK)
		{
			return;
		}
		SelectJointPrincipauxPopupForm selectJointPrincipauxPopupForm = (SelectJointPrincipauxPopupForm)_cPopupForm.Tag;
		List<SIGDemiJoint> list = new List<SIGDemiJoint>();
		foreach (SIGJoint joint in selectJointPrincipauxPopupForm.Joints)
		{
			SIGDemiJoint item = new SIGDemiJoint(-1)
			{
				Circuit = _cPopupForm.Circuit,
				Joint = joint,
				DemiPas = 0.0,
				Principal = (joint == selectJointPrincipauxPopupForm.JointDebut || joint == selectJointPrincipauxPopupForm.JointFin),
				DB = (joint.Type == JointType.SV || joint.Type == JointType.SVAC),
				Emetteur = _cPopupForm.Circuit.IsIPCS
			};
			list.Add(item);
		}
		Base.AddCircuit(_cPopupForm.Circuit, list);
		base.ComposantsViewer.RefreshLigne();
	}

	protected void Modifier()
	{
		EditNomPopupForm _popupForm = new EditNomPopupForm
		{
			Nom = _voie.Nom,
			PkD = _voie.PKDebut,
			PkF = _voie.PKFin
		};
		_popupForm.Closing += delegate(object o, PopupFormResultEventArgs e)
		{
			EditPopupForm_Closing(_popupForm, e);
		};
		base.ComposantsViewer.PopupContainer.Show(_popupForm, "Modifier", PopupContainerButtons.Valider);
	}

	protected void EditPopupForm_Closing(EditNomPopupForm _popupForm, PopupFormResultEventArgs e)
	{
		if (e.Result == PopupContainerResult.OK)
		{
			_voie.Nom = _popupForm.Nom;
			Base.UpdateVoie(_voie);
		}
	}

	protected void Supprimer()
	{
		if (Dialogs.Confirm("Êtes-vous sur de vouloir supprimer cette voie ?"))
		{
			Base.DeleteVoie(base.Voie);
			base.ComposantsViewer.RefreshLigne();
		}
	}

	protected void Proprietes()
	{
		base.ComposantsViewer.PopupContainer.Show(GetPropertyWindow(), "Propriétés");
	}

	public static CVoieOnLine Create(SIGVoie _voie)
	{
		if (_voie.IsJonction())
		{
			return new CJonction(_voie);
		}
		return new CVoiePrincipale(_voie);
	}
}
