using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using CDV_Viewer.Controls;
using CDV_Viewer.CsvBase;
using CDV_Viewer.Data;
using CDV_Viewer.Data.SIGObjects;
using CDV_Viewer.PopupForms;
using CDV_Viewer.Properties;
using CDV_Viewer.Styles;

namespace CDV_Viewer.Composants;

public class CSautPK : CNoeud
{
	public static Brush selectedBrush = new SolidBrush(Colors.NoeudSelected);

	public static Brush normalBrush = new SolidBrush(Colors.Noeud);

	public static Pen sautPen = new Pen(Color.Gold, 3f);

	public static Pen arrowPen = new Pen(Color.Gold, 2f);

	private SIGBranche _extremiteDest;

	private int _pkDest;

	public CSautPK(CVoie support, SIGNoeud noeud)
	{
		_erreur = true;
		_support = support;
		if (_support == null)
		{
			return;
		}
		SIGNoeud.NoeudType type = noeud.Type;
		if (type == SIGNoeud.NoeudType.SautPk || type == SIGNoeud.NoeudType.RebroussementPk)
		{
			base.Id = noeud.ID;
			_erreur = false;
			_extremite = noeud.BrancheAmont;
			_extremiteDest = noeud.BrancheAval;
			SIGVoie voie = _extremite.Voie;
			_pk = _extremite.PK;
			_pkDest = _extremiteDest.PK;
			_pkMin = voie.PKDebut;
			_pkMax = voie.PKFin;
			if (base.Noeud == voie.NoeudDebut)
			{
				_pkMin = -1000000;
			}
			if (base.Noeud == voie.NoeudFin)
			{
				_pkMax = 1000000;
			}
			_ordre = Global.OrdreNoeud;
		}
	}

	public override bool Contains(Point Pt, int pkPoint)
	{
		if (Math.Abs(_pk + _pkDest + 2 * pkPoint) > Math.Abs(_pk - _pkDest) + 50)
		{
			return false;
		}
		return _displayPath.Contains(Pt);
	}

	public override bool Contains(Point Pt)
	{
		return _displayPath.Contains(Pt);
	}

	public override void RecalculPKs()
	{
		_pk = _extremite.PK;
		_pkDest = _extremiteDest.PK;
		_pkMin = _extremite.Voie.PKDebut;
		_pkMax = _extremite.Voie.PKFin;
	}

	public override Point[] GetPath(int PkD, int PkF)
	{
		Point point = base.Support.GetPoint(PkD);
		Point point2 = point;
		if (PkD == PkF)
		{
			int dx = ((_pk - _support.Voie.PKDebut > _support.Voie.PKFin - _pk) ? 20 : (-20));
			int dy = ((_support.Voie.PositionY > 0) ? 12 : (-12));
			point2.Offset(dx, dy);
			return new Point[1] { point };
		}
		Point point3 = base.Support.GetPoint(PkF);
		Point point4 = point3;
		point2.Offset(0, -16);
		point4.Offset(0, -16);
		return new Point[4] { point, point2, point4, point3 };
	}

	protected override bool OnPaint(PaintEventArgs e)
	{
		if (!Visible)
		{
			return false;
		}
		Point[] path = GetPath(_pk, _pkDest);
		Brush brush = (_mouseOn ? selectedBrush : normalBrush);
		if (_pk == _pkDest)
		{
			Point point = path[0];
			Point p = point;
			int dx = ((base.Support.Voie.PKDebut + base.Support.Voie.PKFin - 2 * _pk > 0) ? (-16) : 16);
			int dy = ((e.ClipRectangle.Height - 2 * point.Y > 0) ? (-16) : 16);
			p.Offset(dx, dy);
			_displayPath.FromPoints(point, p);
			e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
			e.Graphics.FillEllipse(brush, new Rectangle(point.X - 4, point.Y - 4, 7, 7));
			e.Graphics.SmoothingMode = SmoothingMode.Default;
			if (!_mouseOn)
			{
				_ = CVoie.VoiePen;
			}
			else
			{
				_ = CVoie.VoiePenSelected;
			}
			_displayPath.Draw(e.Graphics, CVoie.VoiePen);
		}
		else
		{
			_displayPath.FromPointArray(path);
			Point point2 = path[0];
			e.Graphics.FillRectangle(brush, new Rectangle(point2.X - 4, point2.Y - 6, 7, 9));
			e.Graphics.DrawLines(sautPen, path);
			point2 = path[path.Length - 1];
			e.Graphics.DrawLine(arrowPen, point2.X + 5, point2.Y - 8, point2.X, point2.Y);
			e.Graphics.DrawLine(arrowPen, point2.X - 5, point2.Y - 8, point2.X, point2.Y);
		}
		_displayRectangle = _displayPath.MaxBounds(7, 7);
		return true;
	}

	public override ComposantMenu GetContextMenu()
	{
		string text = $"{_support.Voie.Ligne.ID} {_support.Voie.Nom}";
		ComposantMenu composantMenu = new ComposantMenu("Saut de PK " + text);
		if (Global.ModeEdition && _pk == _pkDest)
		{
			bool num = _pk - _support.Voie.PKDebut < 200;
			bool flag = _support.Voie.PKFin - _pk < 200;
			if (num || flag)
			{
				composantMenu.AddItem("Creer un tiroir ...", Resources.Edit, Tiroir, AuthorizedMode.Edit);
			}
		}
		composantMenu.AddItem("Modifier...", Resources.Edit, Modifier, AuthorizedMode.Edit);
		composantMenu.AddItem("Suprimer...", Supprimer, AuthorizedMode.Edit);
		composantMenu.AddItem("Propriétés...", Resources.Properties, Proprietes, AuthorizedMode.Always);
		return composantMenu;
	}

	public override PopupForm GetPropertyWindow()
	{
		return new NoeudPopupForm(base.Noeud);
	}

	protected override void OnMouseDoubleClick(MouseEventArgs e)
	{
		Proprietes(this, new EventArgs());
	}

	protected override void OnMoveEnd(EventArgs e)
	{
		int pK = _extremite.PK;
		foreach (SIGBranche branch in base.Noeud.Branches)
		{
			if (branch.Voie == _extremite.Voie || branch.PK == pK)
			{
				branch.PK += _pk - pK;
				Base.UpdateBranche(branch);
			}
		}
	}

	private void Modifier(object sender, EventArgs e)
	{
		NoeudEditPopupForm noeudEditPopupForm = new NoeudEditPopupForm(base.Noeud);
		noeudEditPopupForm.Closing += EditPopupForm_Closing;
		base.ComposantsViewer.PopupContainer.Show(noeudEditPopupForm, "Modifier Saut de PK", PopupContainerButtons.Valider);
	}

	private void Supprimer(object sender, EventArgs e)
	{
		if (MessageBox.Show("Êtes-vous sur de vouloir supprimer ce Saut de PK ?", Resources.APP_NAME, MessageBoxButtons.YesNo) == DialogResult.Yes)
		{
			if (base.Noeud == _support.Voie.NoeudDebut)
			{
				Base.CsvBranches.Remove(base.Noeud.BrancheAmont.ID);
				Base.NeedLink();
			}
			else if (base.Noeud == _support.Voie.NoeudFin)
			{
				Base.CsvBranches.Remove(base.Noeud.BrancheAval.ID);
				Base.NeedLink();
			}
			else
			{
				Base.DeleteNoeud(base.Noeud);
			}
			base.ComposantsViewer.RefreshLigne();
		}
	}

	private void Proprietes(object sender, EventArgs e)
	{
		base.ComposantsViewer.PopupContainer.Show(GetPropertyWindow(), "Propriétés");
	}

	private void Tiroir(object sender, EventArgs e)
	{
		int pkD = _pk;
		int pkF = _pk;
		if (_pk - _support.Voie.PKDebut < 200)
		{
			pkD = _pk - 400;
		}
		else
		{
			pkF = _pk + 400;
		}
		EditNomPopupForm editNomPopupForm = new EditNomPopupForm();
		editNomPopupForm.PkD = pkD;
		editNomPopupForm.PkF = pkF;
		editNomPopupForm.Nom = _support.Voie.Nom.Trim('V') + "T";
		EditNomPopupForm editNomPopupForm2 = editNomPopupForm;
		editNomPopupForm2.Closing += AjoutTiroirPopupForm_Closing;
		base.ComposantsViewer.PopupContainer.Show(editNomPopupForm2, "Tiroir", PopupContainerButtons.Valider);
	}

	private void EditPopupForm_Closing(object sender, PopupFormResultEventArgs e)
	{
		if (e.Result != PopupContainerResult.OK)
		{
			return;
		}
		_ = (NoeudEditPopupForm)sender;
		foreach (SIGBranche branch in base.Noeud.Branches)
		{
			Base.UpdateBranche(branch);
		}
		base.ComposantsViewer.RefreshLigne();
	}

	private void AjoutTiroirPopupForm_Closing(object sender, PopupFormResultEventArgs e)
	{
		if (e.Result != PopupContainerResult.OK)
		{
			return;
		}
		e.Canceled = true;
		EditNomPopupForm _popupForm = (EditNomPopupForm)sender;
		int pkDebut = _popupForm.PkD;
		int pkFin = _popupForm.PkF;
		SIGLigne ligne = _support.Voie.Ligne;
		if (ligne.Voies.Find((SIGVoie voie) => voie.Nom == _popupForm.Nom && voie.PKFin >= pkDebut && voie.PKDebut <= pkFin) != null)
		{
			MessageBox.Show("Une voie portant ce nom existe déjà entre ces 2 PKs", Resources.APP_NAME);
			return;
		}
		e.Canceled = false;
		SIGVoie sIGVoie = Base.CreateVoie(ligne, _popupForm.Nom, _popupForm.PkD, _popupForm.PkF);
		sIGVoie.PositionY = _support.Voie.PositionY;
		Base.SetPositionVoie(sIGVoie);
		bool flag = pkDebut == _pk;
		SIGNoeud sIGNoeud = (flag ? sIGVoie.NoeudDebut : sIGVoie.NoeudFin);
		int pk = _pk;
		if (!Base.TryConnectNoeudToVoie(sIGNoeud, _support.Voie, pk, out var error))
		{
			MessageBox.Show(error);
			return;
		}
		SIGCircuit circuit = _support.Voie.GetCircuit(pk);
		if (circuit == null)
		{
			base.ComposantsViewer.RefreshLigne();
			return;
		}
		sIGNoeud.FirstBrancheInTrack(sIGVoie);
		if (!flag)
		{
			Base.CreateJoint(sIGVoie, null, circuit, pk - 100);
		}
		else
		{
			Base.CreateJoint(sIGVoie, circuit, null, pk + 100);
		}
		base.ComposantsViewer.RefreshLigne();
	}
}
