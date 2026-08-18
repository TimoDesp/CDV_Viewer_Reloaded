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
using CDV_Viewer.Traitements;

namespace CDV_Viewer.Composants;

public class CBalise : LVPKComposant
{
	private const int OFFSET_Y_SUPPORT = 6;

	public static readonly Shape Crocodile = new Shape(new Point(-6, 2), new Point(-3, -2), new Point(0, 2), new Point(3, -2), new Point(6, 2))
	{
		SmoothingMode = SmoothingMode.AntiAlias
	};

	private SIGBalise _balise;

	public override bool IsComposantSignalisation => false;

	public SIGBalise Balise
	{
		get
		{
			return _balise;
		}
		set
		{
			_balise = value;
		}
	}

	public CBalise(CVoie support, SIGBalise balise)
	{
		_balise = balise;
		_support = support;
		if (_balise == null || _support == null)
		{
			_erreur = true;
			return;
		}
		base.Id = balise.ID;
		_ordre = Global.OrdreBalise;
		ZoomVisible = Global.ZoomVisibleBalises;
	}

	public override void RecalculPKs()
	{
		_pk = _balise.PK;
		_pkMin = _support.Voie.PKDebut;
		_pkMax = _support.Voie.PKFin;
	}

	public override bool Contains(Point Pt, int pkPoint)
	{
		if (Math.Abs(pkPoint - _balise.PK) > 50)
		{
			return false;
		}
		return _displayRectangle.Contains(Pt);
	}

	protected override bool OnPaint(PaintEventArgs e)
	{
		if (!Visible)
		{
			return false;
		}
		Point point = base.Support.GetPoint(_pk);
		point.Y -= 6;
		Color color = (_balise.Actif ? Colors.BaliseEnabled : Colors.BaliseDisabled);
		if (_mouseOn)
		{
			color = Colors.GetSelectedColor(color);
		}
		switch (Balise.Type)
		{
		case BaliseType.CRO:
		{
			Pen pen = new Pen(color, 2f);
			Crocodile.Draw(e.Graphics, pen, point);
			_displayPath.FromShape(Crocodile);
			_displayRectangle = _displayPath.MaxBounds();
			break;
		}
		case BaliseType.BLGV:
		{
			Pen pen = new Pen(color, 3f);
			_displayPath.FromPointArray(new Point(point.X - 6, point.Y), new Point(point.X + 6, point.Y));
			_displayRectangle = _displayPath.MaxBounds(0, 3);
			_displayPath.Draw(e.Graphics, pen);
			break;
		}
		default:
		{
			Pen pen = new Pen(color, 3f);
			_displayRectangle = new Rectangle(point.X - 4, point.Y - 1, 8, 8);
			_displayPath.FromRectangle(_displayRectangle);
			_displayPath.Fill(e.Graphics, pen.Brush);
			break;
		}
		}
		return true;
	}

	public override ComposantMenu GetContextMenu()
	{
		ComposantMenu composantMenu = new ComposantMenu("Balise " + _balise.Type);
		composantMenu.AddItem("Modifier...", Resources.Edit, Modifier, AuthorizedMode.Edit);
		composantMenu.AddItem("Supprimer...", Resources.Delete, Supprimer, AuthorizedMode.Edit);
		composantMenu.AddItem("Propriétés...", Resources.Properties, Proprietes, AuthorizedMode.Read);
		return composantMenu;
	}

	public override PopupForm GetPropertyWindow()
	{
		return new BalisePopupForm(Balise, editMode: false);
	}

	protected override void OnMouseDoubleClick(MouseEventArgs e)
	{
		Proprietes(this, EventArgs.Empty);
	}

	protected override void OnMoveEnd(EventArgs e)
	{
		_balise.PK = _pk;
		Base.UpdateBalise(_balise);
	}

	private void Modifier(object sender, EventArgs e)
	{
		BalisePopupForm balisePopupForm = new BalisePopupForm(Balise, editMode: true);
		balisePopupForm.Closing += EditPopupForm_Closing;
		base.ComposantsViewer.PopupContainer.Show(balisePopupForm, "Modifier Saut de PK", PopupContainerButtons.Valider);
	}

	private void EditPopupForm_Closing(object sender, PopupFormResultEventArgs e)
	{
		BalisePopupForm balisePopupForm = sender as BalisePopupForm;
		Balise.PK = balisePopupForm.Pk;
		Balise.Type = SIGBalise.TypeFromString(balisePopupForm.Type);
		Balise.Actif = balisePopupForm.Actif;
		Base.UpdateBalise(Balise);
	}

	private void Supprimer(object sender, EventArgs e)
	{
		if (Dialogs.Confirm("Êtes-vous sur de vouloir supprimer cette balise ?"))
		{
			Base.DeleteBalise(Balise.ID);
			base.ComposantsViewer.RefreshLigne();
		}
	}

	private void Proprietes(object sender, EventArgs e)
	{
		base.ComposantsViewer.PopupContainer.Show(GetPropertyWindow(), "Propriétés");
	}
}
