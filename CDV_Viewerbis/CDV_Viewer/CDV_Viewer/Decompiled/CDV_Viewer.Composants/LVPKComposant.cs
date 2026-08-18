using System;
using System.Drawing;
using System.Windows.Forms;
using CDV_Viewer.Controls;
using CDV_Viewer.Data;
using CDV_Viewer.Data.SIGObjects;
using CDV_Viewer.PopupForms;
using CDV_Viewer.Traitements;

namespace CDV_Viewer.Composants;

public abstract class LVPKComposant : Composant
{
	protected int _oldPk;

	protected int _pk;

	protected CVoie _support;

	protected SIGExtremite _extremite;

	protected int _pkMin;

	protected int _pkMax;

	protected bool _isMoving;

	private PkPopupForm _popupForm;

	public int Pk => _pk;

	public CVoie Support => _support;

	public SIGExtremite Extremite => _extremite;

	public override bool Contains(Point Pt, int pkPoint)
	{
		if (Math.Abs(pkPoint - _pk) > 50)
		{
			return false;
		}
		return _displayRectangle.Contains(Pt);
	}

	public override bool Contains(Point Pt)
	{
		if (_support is CVoieAdjacente)
		{
			return false;
		}
		return _displayRectangle.Contains(Pt);
	}

	public override Point GetPoint(int Pk)
	{
		return _support.GetPoint(_pk);
	}

	public override Point[] GetPath(int PkD, int PkF)
	{
		return new Point[1] { GetPoint(_pk) };
	}

	public override bool IsInGraph()
	{
		if (base.ComposantsViewer.PkD < _pk)
		{
			return base.ComposantsViewer.PkF > _pk;
		}
		return false;
	}

	protected override void OnMouseDown(MouseEventArgs e)
	{
		if (_mouseOn && Global.ModeEdition && ES.GetStateKey(Keys.ControlKey))
		{
			_oldPk = _pk;
			_isMoving = true;
			_popupForm = new PkPopupForm();
			_popupForm.Pk = _pk;
			_popupForm.Closing += PopupForm_Closing;
			base.ComposantsViewer.PopupContainer.Show(_popupForm, PopupState.Edit);
		}
	}

	protected override void OnMouseUp(MouseEventArgs e)
	{
		_isMoving = false;
	}

	protected override void OnMouseMove(MouseEventArgs e)
	{
		_cursor = (ES.GetStateKey(Keys.ControlKey) ? Cursors.SizeAll : Cursors.Hand);
		if (_isMoving)
		{
			_pk = base.ComposantsViewer.LocationToPk(e.X);
			_pk = Math.Min(_pk, _pkMax);
			_pk = Math.Max(_pk, _pkMin);
			base.ComposantsViewer.PopupContainer.SetPosition(GetPoint(_pk));
			_popupForm.Pk = _pk;
			base.ComposantsViewer.Invalidate();
		}
	}

	private void PopupForm_Closing(object sender, PopupFormResultEventArgs e)
	{
		_popupForm.Closing -= PopupForm_Closing;
		if (e.Result == PopupContainerResult.OK)
		{
			_pk = ((PkPopupForm)sender).Pk;
			if (_pk != _oldPk)
			{
				OnMoveEnd(new EventArgs());
			}
		}
		else
		{
			_pk = _oldPk;
			base.ComposantsViewer.Invalidate();
		}
		_popupForm = null;
	}

	protected virtual void OnMoveEnd(EventArgs e)
	{
	}
}
