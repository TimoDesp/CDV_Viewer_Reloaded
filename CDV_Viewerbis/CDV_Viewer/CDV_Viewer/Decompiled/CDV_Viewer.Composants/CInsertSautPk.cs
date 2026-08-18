using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using CDV_Viewer.Controls;
using CDV_Viewer.CsvBase;
using CDV_Viewer.Data;
using CDV_Viewer.Data.SIGObjects;
using CDV_Viewer.PopupForms;

namespace CDV_Viewer.Composants;

public class CInsertSautPk : COperation
{
	private CVoie _voie;

	private int _pkD;

	private int _pkF;

	private EditNomPksPopupForm _popupForm;

	private Point _mouseLocation;

	public override bool IsComposantSignalisation => false;

	public CInsertSautPk(CVoie voie, int pk)
	{
		_voie = voie;
		_pkD = pk;
	}

	protected override bool OnPaint(PaintEventArgs e)
	{
		Point point = _voie.GetPoint(_pkD);
		e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
		e.Graphics.FillEllipse(new SolidBrush(Color.Blue), new Rectangle(point.X - 3, point.Y - 3, 6, 6));
		if (_voie.DisplayPath.Contains(_mouseLocation, 12))
		{
			int num = base.ComposantsViewer.LocationToPk(_mouseLocation.X);
			if (num != _pkD)
			{
				Point point2 = _voie.GetPoint(num);
				e.Graphics.DrawLine(CSautPK.sautPen, point.X, point.Y, point.X, point.Y - 20);
				e.Graphics.DrawLine(CSautPK.sautPen, point.X, point.Y - 20, point2.X, point2.Y - 20);
				e.Graphics.DrawLine(CSautPK.sautPen, point2.X, point2.Y - 20, point2.X, point2.Y);
				e.Graphics.DrawLine(CSautPK.arrowPen, point2.X + 5, point2.Y - 8, point2.X, point2.Y);
				e.Graphics.DrawLine(CSautPK.arrowPen, point2.X - 5, point2.Y - 8, point2.X, point2.Y);
			}
		}
		e.Graphics.SmoothingMode = SmoothingMode.Default;
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
		int pkF = base.ComposantsViewer.LocationToPk(e.X);
		if (_voie.Contains(e.Location))
		{
			_pkF = pkF;
			_popupForm = new EditNomPksPopupForm();
			_popupForm.Nom = "Saut de PK ";
			_popupForm.PkD = _pkD;
			_popupForm.PkF = _pkF;
			_popupForm.Closing += PopupForm_Closing;
			base.ComposantsViewer.PopupContainer.Show(_popupForm, "CONFIRMER L'AJOUT", PopupContainerButtons.Valider);
		}
	}

	private void PopupForm_Closing(object sender, PopupFormResultEventArgs e)
	{
		_popupForm.Closing -= PopupForm_Closing;
		if (e.Result == PopupContainerResult.OK)
		{
			_pkD = _popupForm.PkD;
			_pkF = _popupForm.PkF;
			SIGVoie voie = _voie.Voie;
			if (voie.Branches.Find((SIGBranche b) => b.PK == _pkD)?.Noeud == null)
			{
				Base.CreateSautdePK(voie, _pkD, _pkF);
			}
			base.ComposantsViewer.RefreshLigne();
		}
		_popupForm = null;
	}
}
