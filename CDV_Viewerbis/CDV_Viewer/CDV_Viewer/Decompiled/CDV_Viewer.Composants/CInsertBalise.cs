using System;
using System.Drawing;
using System.Windows.Forms;
using CDV_Viewer.Controls;
using CDV_Viewer.CsvBase;
using CDV_Viewer.Data;
using CDV_Viewer.Data.SIGObjects;
using CDV_Viewer.PopupForms;

namespace CDV_Viewer.Composants;

public class CInsertBalise : COperation
{
	private static StringFormat _centeredText = new StringFormat
	{
		Alignment = StringAlignment.Center,
		LineAlignment = StringAlignment.Center
	};

	private static Font _font = new Font("Arial", 7f);

	private CVoie _voie;

	private CVoie _voieSelected;

	private int _pk;

	private Point _mouseLocation;

	private int _pkMin;

	private int _pkMax;

	private BalisePopupForm _popupForm;

	public override bool IsComposantSignalisation => false;

	public CInsertBalise(CVoie voie, int pk)
	{
		_voieSelected = (_voie = voie);
		_pk = pk;
		_pkMin = _voie.Voie.PKDebut;
		_pkMax = _voie.Voie.PKFin;
	}

	protected override bool OnPaint(PaintEventArgs e)
	{
		int val = base.ComposantsViewer.LocationToPk(_mouseLocation.X);
		CVoiePrincipale cVoiePrincipale = base.Composants.DisplayedVoieOnLine(_mouseLocation, 20);
		if (cVoiePrincipale != null)
		{
			if (cVoiePrincipale != _voie)
			{
				_pkMin = cVoiePrincipale.Voie.PKDebut;
				_pkMax = cVoiePrincipale.Voie.PKFin;
				_voie = cVoiePrincipale;
			}
			Brush brush = ((_voie != _voieSelected) ? Brushes.Green : Brushes.Red);
			_pk = Math.Min(_pkMax, Math.Max(_pkMin, val));
			Point point = _voie.GetPoint(_pk);
			_displayRectangle = new Rectangle(point.X - 8, point.Y - 8, 16, 6);
			_displayPath.FromRectangle(_displayRectangle);
			e.Graphics.FillRectangle(brush, _displayRectangle);
		}
		else
		{
			_voie = null;
			_displayRectangle = new Rectangle(_mouseLocation.X - 5, _mouseLocation.Y - 8, 10, 8);
			e.Graphics.FillRectangle(Brushes.Gray, _displayRectangle);
		}
		return true;
	}

	protected override void OnMouseMove(MouseEventArgs e)
	{
		_mouseLocation = e.Location;
		base.ComposantsViewer.Invalidate();
	}

	protected override void OnMouseClick(MouseEventArgs e)
	{
		base.Composants.RemoveOperation();
		if (_voie != null)
		{
			_popupForm = new BalisePopupForm(_voie.Voie, _pk);
			_popupForm.Closing += PopupForm_Closing;
			base.ComposantsViewer.PopupContainer.Show(_popupForm, "CONFIRMER L'AJOUT", PopupContainerButtons.Valider);
		}
	}

	private void PopupForm_Closing(object sender, PopupFormResultEventArgs e)
	{
		_popupForm.Closing -= PopupForm_Closing;
		if (e.Result == PopupContainerResult.OK)
		{
			CreateBalise();
		}
		_popupForm = null;
	}

	private void CreateBalise()
	{
		Base.AddBalise(_popupForm.Voie, SIGBalise.TypeFromString(_popupForm.Type), _popupForm.Pk, _popupForm.Actif);
		base.ComposantsViewer.RefreshLigne();
	}
}
