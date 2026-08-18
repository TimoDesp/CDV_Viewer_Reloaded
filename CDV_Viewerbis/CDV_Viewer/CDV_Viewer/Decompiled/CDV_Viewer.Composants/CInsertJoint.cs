using System;
using System.Drawing;
using System.Windows.Forms;
using CDV_Viewer.Controls;
using CDV_Viewer.CsvBase;
using CDV_Viewer.Data;
using CDV_Viewer.Data.SIGObjects;
using CDV_Viewer.DockControls;
using CDV_Viewer.PopupForms;
using CDV_Viewer.Traitements;

namespace CDV_Viewer.Composants;

public class CInsertJoint : COperation
{
	private CVoie _voie;

	private int _pk;

	private int _pkMin;

	private int _pkMax;

	private JointPopupForm _popupForm;

	public override bool IsComposantSignalisation => true;

	public CInsertJoint(CVoie voie, int pk)
	{
		_voie = voie;
		_pk = pk;
		_pkMin = _voie.Voie.PKDebut;
		_pkMax = _voie.Voie.PKFin;
		foreach (SIGJoint joint in _voie.Voie.Joints)
		{
			if (joint.PK < _pk && joint.PK > _pkMin)
			{
				_pkMin = joint.PK;
			}
			if (joint.PK > _pk && joint.PK < _pkMax)
			{
				_pkMax = joint.PK;
			}
		}
		_popupForm = new JointPopupForm(_voie.Voie, _pk);
		_popupForm.Closing += PopupForm_Closing;
		base.ComposantsViewer = ComposantsViewer.Viewer;
		base.ComposantsViewer.PopupContainer.Show(_popupForm, "CONFIRMER L'AJOUT", PopupContainerButtons.Valider);
	}

	protected override bool OnPaint(PaintEventArgs e)
	{
		Point point = _voie.GetPoint(_pk);
		RectangleF rectangleF = new RectangleF(point.X - 5, point.Y - 5, 10f, 10f);
		StringFormat stringFormat = new StringFormat();
		StringAlignment alignment = (stringFormat.LineAlignment = StringAlignment.Center);
		stringFormat.Alignment = alignment;
		e.Graphics.FillRectangle(Brushes.Black, rectangleF);
		e.Graphics.DrawString("?", new Font("Arial", 7f), Brushes.White, rectangleF, stringFormat);
		return true;
	}

	protected override void OnMouseMove(MouseEventArgs e)
	{
		int val = base.ComposantsViewer.LocationToPk(e.X);
		_pk = Math.Min(_pkMax, Math.Max(_pkMin, val));
		base.ComposantsViewer.Invalidate();
	}

	protected override void OnMouseClick(MouseEventArgs e)
	{
	}

	private void PopupForm_Closing(object sender, PopupFormResultEventArgs e)
	{
		_popupForm.Closing -= PopupForm_Closing;
		if (e.Result == PopupContainerResult.OK)
		{
			CreateJoint();
		}
		_popupForm = null;
		base.Composants.RemoveOperation();
	}

	private void CreateJoint()
	{
		if (_popupForm.Joint.Voie.GetCircuit(_popupForm.Joint.PK) != null)
		{
			Dialogs.Message("Impossible de créer un Joint dans un CDV existant");
			return;
		}
		Base.AddJoint(_popupForm.Joint);
		base.ComposantsViewer.RefreshLigne();
	}
}
