using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Windows.Forms;
using CDV_Viewer.Controls;
using CDV_Viewer.CsvBase;
using CDV_Viewer.Data;
using CDV_Viewer.Data.SIGObjects;
using CDV_Viewer.PopupForms;
using CDV_Viewer.Properties;
using CDV_Viewer.Styles;
using CDV_Viewer.Traitements;

namespace CDV_Viewer.Composants;

public class CNoeud : LVPKComposant
{
	private static Brush BrushNoeud = new SolidBrush(Colors.Noeud);

	private static Brush BrushNoeudSelected = new SolidBrush(Colors.NoeudSelected);

	private static Pen PenNoeud = new Pen(Colors.Noeud, 2f);

	private static Pen PenNoeudSelected = new Pen(Colors.NoeudSelected, 2f);

	public override bool IsComposantSignalisation => false;

	public SIGNoeud Noeud => ((SIGBranche)_extremite).Noeud;

	protected CNoeud()
	{
	}

	public CNoeud(CVoie support, SIGNoeud noeud)
	{
		_support = support;
		SIGBranche sIGBranche = (SIGBranche)(_extremite = noeud.FirstBrancheInTrack(_support.Voie));
		if (_support == null || sIGBranche == null)
		{
			_erreur = true;
			return;
		}
		base.Id = noeud.ID;
		_pk = sIGBranche.PK;
		_pkMin = sIGBranche.Voie.PKDebut;
		_pkMax = sIGBranche.Voie.PKFin;
		if (Noeud == sIGBranche.Voie.NoeudDebut)
		{
			_pkMin = -1000000;
		}
		if (Noeud == sIGBranche.Voie.NoeudFin)
		{
			_pkMax = 1000000;
		}
		_ordre = Global.OrdreNoeud;
	}

	public void SetSupport()
	{
		if (Noeud.Type == SIGNoeud.NoeudType.Heurtoir || Noeud.ChangementLigne)
		{
			return;
		}
		if (_support.Voie.NoeudDebut == Noeud)
		{
			SIGVoie[] array = Noeud.OtherTracksOnSameLine(_support.Voie);
			foreach (SIGVoie sIGVoie in array)
			{
				if (sIGVoie.IsVoiePrincipale())
				{
					CVoieOnLine voie = base.Composants.GetVoie(sIGVoie);
					if (voie != null)
					{
						_support = voie;
					}
					break;
				}
			}
		}
		else if (_support.Voie.NoeudFin == Noeud)
		{
			SIGVoie[] array = Noeud.OtherTracksOnSameLine(_support.Voie);
			foreach (SIGVoie sIGVoie2 in array)
			{
				if (sIGVoie2.IsVoiePrincipale())
				{
					CVoieOnLine voie2 = base.Composants.GetVoie(sIGVoie2);
					if (voie2 != null)
					{
						_support = voie2;
					}
					break;
				}
			}
		}
		else
		{
			if (!_support.Voie.IsJonction() || Noeud.Type != SIGNoeud.NoeudType.Tj)
			{
				return;
			}
			SIGVoie[] array = Noeud.OtherTracksOnSameLine(_support.Voie);
			foreach (SIGVoie sIGVoie3 in array)
			{
				if (sIGVoie3.IsVoiePrincipale())
				{
					CVoieOnLine voie3 = base.Composants.GetVoie(sIGVoie3);
					if (voie3 != null)
					{
						_support = voie3;
					}
					break;
				}
			}
		}
	}

	public override void RecalculPKs()
	{
		SIGBranche sIGBranche = Noeud.FirstBrancheInTrack(_support.Voie);
		_pk = sIGBranche.PK;
		_pkMin = sIGBranche.Voie.PKDebut;
		_pkMax = sIGBranche.Voie.PKFin;
	}

	public override bool Contains(Point Pt, int pkPoint)
	{
		if (Math.Abs(pkPoint - _pk) > 50)
		{
			return false;
		}
		Point point = base.Support.GetPoint(_pk);
		if (Math.Abs(point.Y - Pt.Y) > 4)
		{
			return false;
		}
		if (Math.Abs(point.X - Pt.X) > 4)
		{
			return false;
		}
		return true;
	}

	public override bool Contains(Point Pt)
	{
		return _displayRectangle.Contains(Pt);
	}

	protected override bool OnPaint(PaintEventArgs e)
	{
		if (!Visible)
		{
			return false;
		}
		Point point = base.Support.GetPoint(_pk);
		_displayPath.FromLine(new Line(point, point));
		_displayRectangle = _displayPath.MaxBounds(9, 9);
		if (Noeud.Type != SIGNoeud.NoeudType.Heurtoir)
		{
			Brush brush = (_mouseOn ? BrushNoeudSelected : BrushNoeud);
			e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
			e.Graphics.FillEllipse(brush, _displayRectangle);
			if (Noeud.ChangementDeSens)
			{
				_displayPath.MaxBounds(8, 8);
				e.Graphics.DrawEllipse(new Pen(Color.Red, 1f), _displayRectangle);
			}
			e.Graphics.SmoothingMode = SmoothingMode.Default;
		}
		else
		{
			Pen pen = (_mouseOn ? PenNoeudSelected : PenNoeud);
			int num = ((Noeud.BrancheAmont != null) ? (-3) : 3);
			e.Graphics.DrawLine(pen, point.X + num, point.Y + 5, point.X, point.Y + 5);
			e.Graphics.DrawLine(pen, point.X, point.Y + 5, point.X, point.Y - 5);
			e.Graphics.DrawLine(pen, point.X, point.Y - 5, point.X + num, point.Y - 5);
			e.Graphics.DrawLine(pen, point.X + num, point.Y + 7, point.X + num, point.Y + 3);
			e.Graphics.DrawLine(pen, point.X + num, point.Y - 7, point.X + num, point.Y - 3);
		}
		return true;
	}

	public override ComposantMenu GetContextMenu()
	{
		Noeud.FirstBrancheInTrack(_support.Voie);
		string text = $"({_support.Voie.Ligne.ID} {_support.Voie.Nom})";
		SIGVoie[] array = Noeud.OtherTracks(_support.Voie);
		foreach (SIGVoie sIGVoie in array)
		{
			text += $" - ({sIGVoie.Ligne.ID} {sIGVoie.Nom})";
		}
		ComposantMenu composantMenu = new ComposantMenu($"{Noeud.Appareil} " + text);
		bool flag = _support.Voie.GetCircuit(_pk) != null;
		if (Noeud.Count <= 4)
		{
			composantMenu.AddItem("Modifier...", Resources.Edit, Modifier, AuthorizedMode.Edit);
			if (!flag)
			{
				composantMenu.AddItem("Relier...", Relier, AuthorizedMode.Edit);
			}
		}
		foreach (SIGBranche item in Noeud.BranchesInOtherTrack(_support.Voie).Distinct(SIGBranche.CompareVoie))
		{
			string texte = $"Supprimer la liaison avec {item.Voie.FullName}";
			composantMenu.AddItem(item, texte, SupprimerLiaison, AuthorizedMode.Edit);
		}
		CVoiePrincipale cvoie = _support as CVoiePrincipale;
		bool flag2 = Noeud.Type == SIGNoeud.NoeudType.Heurtoir;
		if ((!flag && !flag2) || ES.GetStateKey(Keys.ControlKey))
		{
			composantMenu.AddItem("Supprimer", Resources.Delete, Supprimer, AuthorizedMode.Edit);
		}
		composantMenu.AddItem("Propriétés...", Resources.Properties, Proprietes, AuthorizedMode.Always);
		composantMenu.AddMenu(() => cvoie?.GetPositionMenu(cvoie.Voie.FullName), AuthorizedMode.Edit);
		return composantMenu;
	}

	public override PopupForm GetPropertyWindow()
	{
		return new NoeudPopupForm(Noeud);
	}

	protected override void OnMouseDoubleClick(MouseEventArgs e)
	{
		Proprietes(this, EventArgs.Empty);
	}

	protected override void OnMoveEnd(EventArgs e)
	{
		int pK = _extremite.PK;
		foreach (SIGBranche item in Noeud.BranchesInTrack(_extremite.Voie))
		{
			if (item.PK == pK)
			{
				item.PK += _pk - pK;
				Base.UpdateBranche(item);
			}
		}
	}

	private void Modifier(object sender, EventArgs e)
	{
		NoeudEditPopupForm noeudEditPopupForm = new NoeudEditPopupForm(Noeud);
		noeudEditPopupForm.Closing += EditPopupForm_Closing;
		base.ComposantsViewer.PopupContainer.Show(noeudEditPopupForm, "Modifier", PopupContainerButtons.Valider);
	}

	private void Relier(object sender, EventArgs e)
	{
		RelierNoeudPopupForm relierNoeudPopupForm = new RelierNoeudPopupForm(_extremite);
		relierNoeudPopupForm.Closing += RelierPopupForm_Closing;
		base.ComposantsViewer.PopupContainer.Show(relierNoeudPopupForm, "Relier");
	}

	private void SupprimerLiaison(object sender, EventArgs e)
	{
		SIGBranche branche = ((ToolStripItem)sender)?.Tag as SIGBranche;
		if (branche == null || !Dialogs.Confirm("Êtes-vous sur de vouloir supprimer ce lien ?"))
		{
			return;
		}
		List<SIGDemiJoint> list = _support.Voie.GetCircuit(_pk)?.DemiJoints;
		if (list != null)
		{
			foreach (SIGDemiJoint item in list.FindAll((SIGDemiJoint d) => d.Joint.Voie == branche.Voie))
			{
				Base.DeleteDemiJoint(item);
			}
		}
		Base.SepareBranche(branche);
		base.ComposantsViewer.RefreshLigne();
	}

	private void Supprimer(object sender, EventArgs e)
	{
		if (Dialogs.Confirm("Êtes-vous sur de vouloir supprimer ce noeud ?"))
		{
			Base.SepareAllBranches(Noeud);
			base.ComposantsViewer.RefreshLigne();
		}
	}

	private void Proprietes(object sender, EventArgs e)
	{
		base.ComposantsViewer.PopupContainer.Show(GetPropertyWindow(), "Propriétés");
	}

	private void RelierPopupForm_Closing(object sender, PopupFormResultEventArgs e)
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
		if (!Base.TryConnectNoeudToVoie(Noeud, voieDestination, pkDestination, out var error))
		{
			MessageBox.Show(error);
			return;
		}
		SIGCircuit circuit = voieDestination.GetCircuit(pkDestination);
		if (circuit == null)
		{
			base.ComposantsViewer.RefreshLigne();
			return;
		}
		Noeud.GetJointsAround();
		SIGNoeud.NoeudType type = Noeud.Type;
		if (type == SIGNoeud.NoeudType.Heurtoir)
		{
			SIGBranche sIGBranche = Noeud.FirstBrancheInTrack(_support.Voie);
			SIGJoint nearestJoint = sIGBranche.GetNearestJoint();
			if (sIGBranche.IsBrancheDebut)
			{
				if (nearestJoint != null && nearestJoint.PK - pkOrigine < 1000)
				{
					Base.CreateDemiJointAval(nearestJoint, circuit, principal: false);
				}
				else
				{
					Base.CreateJoint(_support.Voie, null, circuit, pkOrigine + 100);
				}
			}
			else if (nearestJoint != null && pkOrigine - nearestJoint.PK < 1000)
			{
				Base.CreateDemiJointAmont(nearestJoint, circuit, principal: false);
			}
			else
			{
				Base.CreateJoint(_support.Voie, circuit, null, pkOrigine - 100);
			}
		}
		base.ComposantsViewer.RefreshLigne();
	}

	private void EditPopupForm_Closing(object sender, PopupFormResultEventArgs e)
	{
		if (e.Result == PopupContainerResult.OK)
		{
			_ = (NoeudEditPopupForm)sender;
			Base.UpdateNoeud(Noeud);
			base.ComposantsViewer.RefreshLigne();
		}
	}

	public override string ToString()
	{
		return Noeud?.ToString();
	}
}
