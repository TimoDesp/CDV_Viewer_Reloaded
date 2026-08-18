using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using CDV_Viewer.Controls;
using CDV_Viewer.CsvBase;
using CDV_Viewer.Data;
using CDV_Viewer.Data.SIGObjects;
using CDV_Viewer.PopupForms;
using CDV_Viewer.Properties;
using CDV_Viewer.Styles;

namespace CDV_Viewer.Composants;

public class CCircuit : Composant
{
	private static StringFormat _formatCdvName = new StringFormat
	{
		Alignment = StringAlignment.Center
	};

	private int _pkD;

	private int _pkF;

	private SIGCircuit _circuit;

	private List<CSegment> _segments = new List<CSegment>();

	private CSegment _segmentPrincipal;

	public override bool IsComposantSignalisation => true;

	public int PKD => _pkD;

	public int PKF => _pkF;

	public SIGCircuit Circuit => _circuit;

	public List<CSegment> Segments => _segments;

	public CCircuit(ComposantsCollection composants, SIGCircuit circuit)
	{
		_erreur = true;
		_circuit = circuit;
		if (_circuit != null)
		{
			CreateSegments(composants);
			if (_segments.Count != 0)
			{
				base.Id = circuit.ID;
				_ordre = Global.OrdreCircuit;
				ZoomVisible = Global.ZoomVisibleCircuits;
				ZoomTexteVisible = Global.ZoomTexteVisibleCircuits;
				_erreur = false;
			}
		}
	}

	private void CreateSegments(ComposantsCollection composants)
	{
		_segments.Clear();
		_segmentPrincipal = null;
		List<SIGSegment> parcours = SIGSegment.GetParcours(_circuit);
		if (EditModeError(parcours.Count == 0, $"Pas de parcours possible sur {Circuit}"))
		{
			return;
		}
		SIGLigne ligne = composants.ComposantsViewer.Ligne;
		foreach (SIGSegment item in parcours)
		{
			if (ligne != item.Voie.Ligne)
			{
				CVoieAdjacente voieAdjacente = composants.GetVoieAdjacente(item.Voie, item.ExtremiteD.PK, item.ExtremiteF.PK);
				if (voieAdjacente != null)
				{
					int pkDestination = voieAdjacente.PkDestination;
					int num2;
					int num = (num2 = voieAdjacente.Pk);
					int num3 = ((!voieAdjacente.Noeud.ChgSensPk) ? 1 : (-1));
					if (item.ExtremiteD.PK != pkDestination)
					{
						num2 -= num3;
					}
					if (item.ExtremiteF.PK != pkDestination)
					{
						num += num3;
					}
					_segments.Add(new CSegment(voieAdjacente, num2, num));
				}
				continue;
			}
			CVoie voie = composants.GetVoie(item.Voie);
			if (voie == null)
			{
				continue;
			}
			CSegment cSegment = new CSegment(voie, item.ExtremiteD.PK, item.ExtremiteF.PK);
			_segments.Add(cSegment);
			if (item.HasOnlyJoints)
			{
				SIGDemiJoint sIGDemiJoint = (item.ExtremiteD as SIGJoint).DemiJoint(Circuit);
				SIGDemiJoint sIGDemiJoint2 = (item.ExtremiteD as SIGJoint).DemiJoint(Circuit);
				if (sIGDemiJoint.Principal && sIGDemiJoint2.Principal)
				{
					if (_segmentPrincipal == null)
					{
						_segmentPrincipal = cSegment;
					}
					else if (Global.ModeEdition)
					{
						MessageBox.Show($"Erreur de Demi joint Principal sur {Circuit}");
					}
				}
			}
			else if (!item.HasOnlyBranches)
			{
				SIGDemiJoint obj = (item.ExtremiteD as SIGJoint)?.DemiJoint(Circuit);
				if (obj != null && obj.Principal && _segmentPrincipal == null)
				{
					_segmentPrincipal = cSegment;
				}
				SIGDemiJoint obj2 = (item.ExtremiteF as SIGJoint)?.DemiJoint(Circuit);
				if (obj2 != null && obj2.Principal && _segmentPrincipal == null)
				{
					_segmentPrincipal = cSegment;
				}
			}
		}
		_segmentPrincipal = _segmentPrincipal ?? _segments[0];
	}

	public override bool Contains(Point Pt, int pkPoint)
	{
		if (pkPoint < _pkD || pkPoint > _pkF)
		{
			return false;
		}
		return _displayPath.Contains(Pt, 5);
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

	public override void RecalculPKs()
	{
		_pkD = int.MaxValue;
		_pkF = int.MinValue;
		foreach (CSegment segment in _segments)
		{
			_pkD = Math.Min(_pkD, segment.PkD);
			_pkD = Math.Min(_pkD, segment.PkF);
			_pkF = Math.Max(_pkF, segment.PkD);
			_pkF = Math.Max(_pkF, segment.PkF);
		}
	}

	protected override bool OnPaint(PaintEventArgs e)
	{
		if (!Visible)
		{
			return false;
		}
		if (_segments.Count == 0)
		{
			return false;
		}
		Color color = Colors.Cdv(Circuit.Frequence);
		if (_mouseOn)
		{
			color = Colors.GetCdvSelectedColor(color);
		}
		_displayPath.Clear();
		_displayPath.FromCSegments(_segments);
		_displayRectangle = _displayPath.MaxBounds();
		if (base.TexteVisible)
		{
			int num = (_segmentPrincipal.PtD.Y + _segmentPrincipal.PtF.Y) / 2;
			if (!_mouseOn)
			{
				_ = Global.DefaultFont;
			}
			else
			{
				_ = Global.DefaultBoldFont;
			}
			e.Graphics.DrawString(Circuit.Nom, Global.DefaultFont, new SolidBrush(color), new Rectangle(_displayRectangle.X, num + 3, _displayRectangle.Width, Global.DefaultFontSize * 2), _formatCdvName);
		}
		bool cdvDrawingModeCenter = Global.CdvDrawingModeCenter;
		Pen pen = (cdvDrawingModeCenter ? new Pen(color, 5f) : new Pen(color, 4f));
		foreach (Line line in _displayPath.Lines)
		{
			line.Draw(e.Graphics, pen);
			if (cdvDrawingModeCenter)
			{
				line.Draw(e.Graphics, Pens.White);
			}
		}
		return true;
	}

	public override ComposantMenu GetContextMenu()
	{
		string text = "Circuit de voie " + Circuit.Nom;
		if (Autorisations.Values.Edition)
		{
			text = text + " (" + Circuit.ID + ")";
		}
		ComposantMenu composantMenu = new ComposantMenu(text);
		composantMenu.AddItem("Afficher dans le visualisateur de modèles", Resources.Graph, Ouvrir, AuthorizedMode.Always);
		composantMenu.AddItem("Modifier...", Resources.Edit, Modifier, AuthorizedMode.Edit);
		if (Circuit.DemiJoints.Count > 2)
		{
			composantMenu.AddItem("Modifier Joints Pricipaux", Resources.Edit, ModifierJointsPrincipaux, AuthorizedMode.Edit);
		}
		composantMenu.AddItem("Supprimer...", Resources.Delete, Supprimer, AuthorizedMode.Edit);
		composantMenu.AddItem("Propriétés...", Resources.Properties, Proprietes, AuthorizedMode.Read);
		return composantMenu;
	}

	public override PopupForm GetPropertyWindow()
	{
		return new CircuitPopupForm(Circuit);
	}

	protected override void OnMouseUp(MouseEventArgs e)
	{
		if (_mouseOn && e.Button == MouseButtons.Left)
		{
			bool showInfobulle = base.ComposantsViewer.ShowInfobulle;
			bool flag = PopupContainer.Popup.State == PopupState.Edit;
			if ((!showInfobulle || flag) && Global.ModeleViewer.Enabled && Global.ModeleViewer.Circuit != Circuit)
			{
				Global.ModeleViewer.Circuit = Circuit;
			}
		}
	}

	protected override void OnMouseDoubleClick(MouseEventArgs e)
	{
		Ouvrir(this, EventArgs.Empty);
	}

	private void Ouvrir(object sender, EventArgs e)
	{
		Global.ModeleViewer.Circuit = Circuit;
		Global.ModeleViewer.Visible = true;
	}

	private void Modifier(object sender, EventArgs e)
	{
		CircuitEditPopupForm circuitEditPopupForm = new CircuitEditPopupForm(_circuit);
		circuitEditPopupForm.Closing += PopupFormModifier_Closing;
		base.ComposantsViewer.PopupContainer.Show(circuitEditPopupForm, "Modifier", PopupContainerButtons.Valider);
	}

	private void PopupFormModifier_Closing(object sender, PopupFormResultEventArgs e)
	{
		if (e.Result == PopupContainerResult.OK)
		{
			Base.UpdateCircuit(_circuit);
			base.ComposantsViewer.RefreshLigne();
		}
	}

	private void ModifierJointsPrincipaux(object sender, EventArgs e)
	{
		SelectJointPrincipauxPopupForm selectJointPrincipauxPopupForm = new SelectJointPrincipauxPopupForm(Circuit.DemiJoints);
		selectJointPrincipauxPopupForm.Closed += JointPopupForm_Closed;
		base.ComposantsViewer.PopupContainer.Show(selectJointPrincipauxPopupForm, "SÉLECTION DES JOINTS PRINCIPAUX", PopupContainerButtons.Valider);
	}

	private void JointPopupForm_Closed(object sender, PopupFormResultEventArgs e)
	{
		if (e.Result != PopupContainerResult.OK)
		{
			return;
		}
		SelectJointPrincipauxPopupForm selectJointPrincipauxPopupForm = (SelectJointPrincipauxPopupForm)sender;
		foreach (SIGDemiJoint demiJoint in Circuit.DemiJoints)
		{
			if (demiJoint.Joint == selectJointPrincipauxPopupForm.JointDebut)
			{
				demiJoint.Principal = true;
			}
			else if (demiJoint.Joint == selectJointPrincipauxPopupForm.JointFin)
			{
				demiJoint.Principal = true;
			}
			else
			{
				demiJoint.Principal = false;
			}
			Base.UpdateDemiJoint(demiJoint);
		}
		base.ComposantsViewer.RefreshLigne();
	}

	private void Supprimer(object sender, EventArgs e)
	{
		if (MessageBox.Show("Êtes-vous sur de vouloir supprimer ce circuit de voie ?", Resources.APP_NAME, MessageBoxButtons.YesNo) != DialogResult.Yes)
		{
			return;
		}
		SIGCircuit circuitAdjacent = Base.GetCircuitAdjacent(Circuit.DemiJointDebut);
		SIGCircuit circuitAdjacent2 = Base.GetCircuitAdjacent(Circuit.DemiJointFin);
		List<SIGDemiJoint> list = Circuit.DemiJoints.FindAll((SIGDemiJoint d) => d != Circuit.DemiJointFin && d != Circuit.DemiJointDebut);
		Base.DeleteCircuit(Circuit.ID);
		if (circuitAdjacent != null && circuitAdjacent2 != null && MessageBox.Show("Voulez-vous fusionner les circuits de voies adjacents ?", "", MessageBoxButtons.YesNo) == DialogResult.Yes)
		{
			foreach (SIGDemiJoint item in list)
			{
				Base.UpdateDemiJoint(item, circuitAdjacent);
			}
			foreach (SIGDemiJoint demiJoint in circuitAdjacent2.DemiJoints)
			{
				if (demiJoint != circuitAdjacent2.DemiJointDebut)
				{
					Base.UpdateDemiJoint(demiJoint, circuitAdjacent);
				}
			}
			Base.DeleteJoint(circuitAdjacent.DemiJointFin.Joint);
			Base.DeleteJoint(circuitAdjacent2.DemiJointDebut.Joint);
			Base.DeleteCircuit(circuitAdjacent2.ID);
		}
		base.ComposantsViewer.RefreshLigne();
	}

	private void Proprietes(object sender, EventArgs e)
	{
		base.ComposantsViewer.PopupContainer.Show(GetPropertyWindow(), "Propriétés");
	}
}
