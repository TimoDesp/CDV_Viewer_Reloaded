using System;
using System.Collections.Generic;
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

public class CJoint : LVPKComposant
{
	public static Pen JointPen;

	public static Pen JointSelectedPen;

	private static StringFormat _textformat;

	private static GraphicsPath JI_Path;

	private static GraphicsPath CC_Path;

	private static GraphicsPath SV_Path;

	private static GraphicsPath SVAC_Path;

	private static GraphicsPath INC_Path;

	private int old_pos = int.MinValue;

	public override bool IsComposantSignalisation => false;

	public SIGJoint Joint => (SIGJoint)_extremite;

	public override bool Visible
	{
		get
		{
			if (!base.Visible)
			{
				return false;
			}
			return base.ComposantsViewer.ModeVisualisation == ModeVisualisation.Signalisation;
		}
	}

	static CJoint()
	{
		JointPen = new Pen(Colors.Joint, 2f);
		JointSelectedPen = new Pen(Colors.GetSelectedColor(Colors.Joint), 2f);
		_textformat = new StringFormat
		{
			Alignment = StringAlignment.Center
		};
		JI_Path = new GraphicsPath();
		CC_Path = new GraphicsPath();
		SV_Path = new GraphicsPath();
		SVAC_Path = new GraphicsPath();
		INC_Path = new GraphicsPath();
		JI_Path.AddLine(1, -5, 1, 5);
		JI_Path.AddLine(1, 5, -1, 5);
		JI_Path.AddLine(-1, 5, -1, -5);
		JI_Path.CloseFigure();
		CC_Path.AddLine(-3, -4, -3, 4);
		CC_Path.StartFigure();
		CC_Path.AddLine(0, -5, 0, 5);
		CC_Path.StartFigure();
		CC_Path.AddLine(3, -4, 3, 4);
		CC_Path.StartFigure();
		CC_Path.AddLine(-3, 0, 3, 0);
		SV_Path.AddLine(-4, -5, -4, 5);
		SV_Path.StartFigure();
		SV_Path.AddLine(4, -5, 4, 5);
		SV_Path.StartFigure();
		SV_Path.AddLine(-4, 0, 4, 0);
		SVAC_Path.AddLine(-3, -5, -3, 5);
		SVAC_Path.StartFigure();
		SVAC_Path.AddLine(3, -5, 3, 5);
		SVAC_Path.StartFigure();
		SVAC_Path.AddLine(-3, 0, 3, 0);
		INC_Path.AddRectangle(new Rectangle(-5, -5, 10, 10));
	}

	public CJoint(CVoie support, SIGJoint joint)
	{
		_erreur = true;
		_support = support;
		_extremite = joint;
		if (_support != null && _extremite != null)
		{
			base.Id = joint.ID;
			_erreur = false;
			_ordre = Global.OrdreJoint;
			ZoomVisible = Global.ZoomVisibleJoints;
			ZoomTexteVisible = Global.ZoomTexteVisibleJoints;
		}
	}

	public override void RecalculPKs()
	{
		if (_support is CVoieAdjacente)
		{
			CVoieAdjacente cVoieAdjacente = _support as CVoieAdjacente;
			int pk = cVoieAdjacente.Pk;
			SIGBranche sIGBranche = cVoieAdjacente.Noeud.FirstBrancheInTrack(Joint.Voie);
			int num = 1;
			if (sIGBranche.IsAval && sIGBranche.Noeud == cVoieAdjacente.Voie.NoeudFin)
			{
				num = -1;
			}
			if (sIGBranche.IsAmont && sIGBranche.Noeud == cVoieAdjacente.Voie.NoeudDebut)
			{
				num = -1;
			}
			if (Joint.PK < sIGBranche.PK)
			{
				_pkMin = (_pkMax = (_pk = pk - num));
			}
			else
			{
				_pkMin = (_pkMax = (_pk = pk + num));
			}
			return;
		}
		_pk = Joint.PK;
		_pkMin = Joint.PK - 10000;
		_pkMax = Joint.PK + 10000;
		foreach (SIGJoint joint in _support.Voie.Joints)
		{
			if (joint.PK < Joint.PK)
			{
				_pkMin = joint.PK;
			}
			else if (joint.PK > Joint.PK)
			{
				_pkMax = joint.PK;
				break;
			}
		}
	}

	public override bool Contains(Point Pt, int pkPoint)
	{
		if (_support is CVoieAdjacente)
		{
			return false;
		}
		return base.Contains(Pt, pkPoint);
	}

	public override bool Contains(Point Pt)
	{
		if (_support is CVoieAdjacente)
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
		if (_support is CVoieAdjacente)
		{
			return false;
		}
		double angle;
		Point point = (_support as CVoieOnLine).internalGetPoint(_pk, out angle);
		old_pos = point.Y;
		_displayPath.FromLine(new Line(point, point));
		_displayRectangle = _displayPath.MaxBounds(10, 10);
		Pen pen = (_mouseOn ? JointSelectedPen : JointPen);
		if (Global.ModeEdition && base.TexteVisible)
		{
			e.Graphics.DrawString(Joint.ID.ToString(), Global.DefaultFont, pen.Brush, new Rectangle(point.X - 3 * Global.DefaultFontSize, point.Y - 2 * Global.DefaultFontSize - 5, 6 * Global.DefaultFontSize, 2 * Global.DefaultFontSize), _textformat);
		}
		Matrix matrix = new Matrix();
		matrix.Translate(point.X, point.Y);
		if (angle != 0.0)
		{
			matrix.Rotate((float)angle);
		}
		e.Graphics.Transform = matrix;
		switch (Joint.Type)
		{
		case JointType.CC:
			e.Graphics.DrawPath(pen, CC_Path);
			break;
		case JointType.SV:
			e.Graphics.DrawPath(pen, SV_Path);
			break;
		case JointType.SVAC:
			e.Graphics.DrawPath(pen, SVAC_Path);
			break;
		case JointType.JI:
			e.Graphics.DrawPath(pen, JI_Path);
			break;
		case JointType.INC:
			e.Graphics.FillRectangle(pen.Brush, -5, -5, 10, 10);
			e.Graphics.DrawString("?", Global.DefaultFont, Brushes.White, new Rectangle(-5, -5, 10, 10), _textformat);
			break;
		}
		DrawEmetteurs(e.Graphics);
		e.Graphics.ResetTransform();
		return true;
	}

	private void DrawEmetteurs(Graphics g)
	{
		if (!base.ComposantsViewer.ShowEmetteurs)
		{
			return;
		}
		bool flag = Joint.DemiJointAmont?.Emetteur ?? false;
		bool flag2 = Joint.DemiJointAval?.Emetteur ?? false;
		if (flag || flag2)
		{
			int num = ((_support.Voie.PositionY > 0) ? 7 : (-10));
			num = 7;
			if (flag)
			{
				g.FillRectangle(Brushes.OrangeRed, -8, num, 4, 4);
			}
			if (flag2)
			{
				g.FillRectangle(Brushes.OrangeRed, 4, num, 4, 4);
			}
		}
	}

	public override ComposantMenu GetContextMenu()
	{
		ComposantMenu composantMenu = ((!Autorisations.Values.Edition) ? new ComposantMenu("Joint " + Joint.Type) : new ComposantMenu(string.Concat("Joint ", Joint.ID, " (", Joint.Type, ")")));
		AddSubMenuRelier(composantMenu);
		composantMenu.AddItem("Modifier...", Resources.Edit, Modifier, AuthorizedMode.Edit);
		if (!Joint.HasLinkedCircuit)
		{
			composantMenu.AddItem("Supprimer...", Resources.Delete, Supprimer, AuthorizedMode.Edit);
		}
		composantMenu.AddItem("Propriétés...", Resources.Properties, Proprietes, AuthorizedMode.Read);
		return composantMenu;
	}

	public void AddSubMenuRelier(ComposantMenu _menu)
	{
		Func<SIGDemiJoint, string> func = (SIGDemiJoint d) => $"Relié au circuit de voie {d.Circuit.Nom}";
		ToolStripMenuItem toolStripMenuItem = _menu.AddMenu("Relier", AuthorizedMode.Edit);
		if (toolStripMenuItem == null)
		{
			return;
		}
		List<SIGCircuit> list = new List<SIGCircuit>();
		if (Joint.HasCircuitAmont)
		{
			bool enabled = !Joint.DemiJointAmont.Principal;
			string texte = func(Joint.DemiJointAmont);
			_menu.AddDropDownItem(toolStripMenuItem, texte, Resources.OkButton, delegate
			{
				RelierCircuit(Joint.DemiJointAmont.Circuit);
			}, AuthorizedMode.Edit, enabled);
			list.Add(Joint.DemiJointAmont.Circuit);
		}
		else
		{
			foreach (SIGDemiJoint _nextDemiJoint in CDV_Viewer.Traitements.Composants.FindDemiJoints(Joint, SearchDirection.Moins))
			{
				if (_nextDemiJoint?.Circuit != null && !list.Contains(_nextDemiJoint.Circuit))
				{
					string texte2 = func(_nextDemiJoint);
					_menu.AddDropDownItem(toolStripMenuItem, texte2, delegate
					{
						RelierCircuit(_nextDemiJoint.Circuit);
					}, AuthorizedMode.Edit);
					list.Add(_nextDemiJoint.Circuit);
				}
			}
		}
		if (Joint.HasCircuitAval)
		{
			bool enabled2 = !Joint.DemiJointAval.Principal;
			string texte3 = func(Joint.DemiJointAval);
			_menu.AddDropDownItem(toolStripMenuItem, texte3, Resources.OkButton, delegate
			{
				RelierCircuit(Joint.DemiJointAval.Circuit);
			}, AuthorizedMode.Edit, enabled2);
			list.Add(Joint.DemiJointAval.Circuit);
			return;
		}
		foreach (SIGDemiJoint _nextDemiJoint2 in CDV_Viewer.Traitements.Composants.FindDemiJoints(Joint, SearchDirection.Plus))
		{
			if (_nextDemiJoint2?.Circuit != null && !list.Contains(_nextDemiJoint2.Circuit))
			{
				string texte4 = func(_nextDemiJoint2);
				_menu.AddDropDownItem(toolStripMenuItem, texte4, delegate
				{
					RelierCircuit(_nextDemiJoint2.Circuit);
				}, AuthorizedMode.Edit);
				list.Add(_nextDemiJoint2.Circuit);
			}
		}
	}

	public override PopupForm GetPropertyWindow()
	{
		return new JointPopupForm(Joint, editMode: false);
	}

	protected override void OnMouseDoubleClick(MouseEventArgs e)
	{
		Proprietes(this, new EventArgs());
	}

	protected override void OnMoveEnd(EventArgs e)
	{
		Joint.PK = _pk;
		Base.EditJoint(Joint);
		RecalculPKs();
		if (!Dialogs.Confirm("Voulez-vous recalculer les pas des circuits de voie adjacents ?"))
		{
			UpdateCompensation(Joint);
		}
	}

	private void RelierCircuit(SIGCircuit circuit)
	{
		if (circuit == null)
		{
			return;
		}
		SIGDemiJoint sIGDemiJoint = Joint.DemiJoint(circuit);
		if (sIGDemiJoint != null)
		{
			if (MessageBox.Show($"Le Circuit de voie {circuit} va etre supprimé") == DialogResult.OK)
			{
				Base.DeleteDemiJoint(sIGDemiJoint);
			}
		}
		else
		{
			bool principal = circuit.DemiJoints.FindAll((SIGDemiJoint d) => d.Principal).Count < 2;
			Base.CreateDemiJoint(Joint, circuit, principal);
		}
		base.ComposantsViewer.RefreshLigne();
	}

	private void Modifier(object sender, EventArgs e)
	{
		JointPopupForm _popupForm = new JointPopupForm(Joint, editMode: true);
		_popupForm.Closed += delegate(object s, PopupFormResultEventArgs evnt)
		{
			ModifierPopupForm_Closed(_popupForm, evnt.Result);
		};
		base.ComposantsViewer.PopupContainer.Show(_popupForm, "Modifier", PopupContainerButtons.Valider);
	}

	private void Supprimer(object sender, EventArgs e)
	{
		if (Joint.HasLinkedCircuit)
		{
			Dialogs.Message("Impossible de supprimer ce joint, supprimez d'abord les circuits de voies adjacents.");
		}
		else if (Dialogs.Confirm("Êtes-vous sur de vouloir supprimer ce joint ?"))
		{
			Base.DeleteJoint(Joint);
			base.ComposantsViewer.RefreshLigne();
		}
	}

	private void Proprietes(object sender, EventArgs e)
	{
		base.ComposantsViewer.PopupContainer.Show(GetPropertyWindow(), "Propriétés");
	}

	private void ModifierPopupForm_Closed(JointPopupForm form, PopupContainerResult button)
	{
		if (button == PopupContainerResult.OK)
		{
			Base.EditJoint(Joint);
			Base.UpdateDemiJoint(Joint.DemiJointAmont);
			Base.UpdateDemiJoint(Joint.DemiJointAval);
			base.ComposantsViewer.RefreshLigne();
		}
	}

	private void UpdateCompensation(SIGJoint Joint)
	{
		foreach (SIGDemiJoint demiJoint in Joint._demiJoints)
		{
			if (demiJoint.Circuit == null)
			{
				continue;
			}
			double longueurUtile = demiJoint.Circuit.GetLongueurUtile();
			int num = CircuitTheorique.PasTheorique(demiJoint.Circuit.Compensation, demiJoint.Circuit.Frequence);
			if (num == 0)
			{
				continue;
			}
			demiJoint.Circuit.NbPtsCompensation = CircuitTheorique.GetNbPointsCompensation(demiJoint.Circuit.Compensation, longueurUtile, num);
			demiJoint.Circuit.PasReel = CircuitTheorique.GetPas(demiJoint.Circuit.Compensation, longueurUtile, num, demiJoint.Circuit.NbPtsCompensation);
			double demiPas = CircuitTheorique.GetDemiPas(demiJoint.Circuit.Compensation, longueurUtile, num, demiJoint.Circuit.NbPtsCompensation);
			foreach (SIGDemiJoint demiJoint2 in demiJoint.Circuit.DemiJoints)
			{
				demiJoint2.DemiPas = demiPas;
				Base.UpdateDemiJoint(demiJoint2);
			}
			Base.UpdateCircuit(demiJoint.Circuit);
		}
	}
}
