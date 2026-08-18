using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using CDV_Viewer.Controls;
using CDV_Viewer.CsvBase;
using CDV_Viewer.Data;
using CDV_Viewer.Data.SIGObjects;
using CDV_Viewer.PopupForms;

namespace CDV_Viewer.Composants;

public class CInsertJonction : COperation
{
	private static Brush NoeudBrush = new SolidBrush(Color.Blue);

	private static Pen JonctionPen = new Pen(Color.Purple, 2f);

	private CVoie _voieD;

	private CVoie _voieF;

	private int _pkD;

	private int _pkF;

	private EditNomPksPopupForm _popupForm;

	private Point _mouseLocation;

	public override bool IsComposantSignalisation => false;

	public CInsertJonction(CVoie voie, int pk)
	{
		_voieD = voie;
		_pkD = pk;
	}

	protected override bool OnPaint(PaintEventArgs e)
	{
		Point point = _voieD.GetPoint(_pkD);
		Graphics graphics = e.Graphics;
		graphics.SmoothingMode = SmoothingMode.AntiAlias;
		graphics.FillEllipse(NoeudBrush, new Rectangle(point.X - 3, point.Y - 3, 6, 6));
		int pk = base.ComposantsViewer.LocationToPk(_mouseLocation.X);
		_voieF = base.Composants.DisplayedVoieOnLine(_mouseLocation);
		if (_voieF != null && _voieF != _voieD)
		{
			Point point2 = _voieF.GetPoint(pk);
			graphics.FillEllipse(NoeudBrush, new Rectangle(point2.X - 3, point2.Y - 3, 6, 6));
			graphics.DrawLine(JonctionPen, point, point2);
		}
		graphics.SmoothingMode = SmoothingMode.Default;
		return true;
	}

	protected override void OnMouseMove(MouseEventArgs e)
	{
		_mouseLocation = e.Location;
		base.ComposantsViewer.Invalidate();
	}

	protected override void OnMouseClick(MouseEventArgs e)
	{
		int pkF = base.ComposantsViewer.LocationToPk(e.X);
		if (_voieF != null)
		{
			_pkF = pkF;
			_popupForm = new EditNomPksPopupForm
			{
				Nom = "J" + $"{_pkD / 100:0000}",
				PkD = _pkD,
				PkF = _pkF
			};
			_popupForm.Closing += PopupForm_Closing;
			base.ComposantsViewer.PopupContainer.Show(_popupForm, "CONFIRMER L'AJOUT", PopupContainerButtons.Valider);
		}
	}

	private void PopupForm_Closing(object sender, PopupFormResultEventArgs e)
	{
		_popupForm.Closing -= PopupForm_Closing;
		if (e.Result == PopupContainerResult.OK && _voieD != _voieF)
		{
			CreateJonction();
		}
		_popupForm = null;
		base.Composants.RemoveOperation();
	}

	private void CreateJonction()
	{
		_pkD = _popupForm.PkD;
		_pkF = _popupForm.PkF;
		SIGVoie voie = Base.CreateJonction(_popupForm.Nom, _voieD.Voie, _pkD, _voieF.Voie, _pkF);
		SIGCircuit circuit = _voieD.Voie.GetCircuit(_pkD);
		SIGCircuit circuit2 = _voieF.Voie.GetCircuit(_pkF);
		Base.CreateJoint(voie, circuit, circuit2, (_pkD + _pkF) / 2);
		base.ComposantsViewer.RefreshLigne();
	}
}
