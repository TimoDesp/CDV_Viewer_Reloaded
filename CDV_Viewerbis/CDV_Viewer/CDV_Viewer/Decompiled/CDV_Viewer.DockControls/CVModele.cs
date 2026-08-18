using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using CDV_Viewer.Composants;
using CDV_Viewer.Controls;
using CDV_Viewer.Data;
using CDV_Viewer.Data.SIGObjects;
using CDV_Viewer.Styles;
using CDV_Viewer.Traitements;

namespace CDV_Viewer.DockControls;

public class CVModele : DockChild
{
	public static CVModele Viewer;

	private SIGCircuit _circuit;

	private List<CMarqueurPK> _marqueurs = new List<CMarqueurPK>();

	private IContainer components;

	private SIGModeleViewer modeleViewer;

	private Separator separator1;

	private Label label4;

	private CloseButton bFermer;

	private FlecheButton fbE;

	private FlecheButton fbW;

	private FlecheButton fbNW;

	private FlecheButton fbSW;

	private FlecheButton fbNE;

	private FlecheButton fbSE;

	private SimpleListView lvModeles;

	private Separator separator2;

	private Label lEmpty;

	private Label lNom;

	public SIGCircuit Circuit
	{
		get
		{
			return _circuit;
		}
		set
		{
			SetCircuit(value);
		}
	}

	public event EventHandler CircuitChanged;

	public CVModele()
	{
		InitializeComponent();
		MinimumSize = new Size(400, 200);
		modeleViewer.LineWidth = 2;
		bFermer.Click += bFermer_Click;
		lvModeles.SelectedIndexChanged += lvModeles_SelectedIndexChanged;
		fbE.Click += flecheButton_Click;
		fbW.Click += flecheButton_Click;
		fbNE.Click += flecheButton_Click;
		fbNW.Click += flecheButton_Click;
		fbSE.Click += flecheButton_Click;
		fbSW.Click += flecheButton_Click;
		Viewer = this;
		ComposantsViewer.Viewer.SensPkChanged += ComposantsViewer_SensPkChanged;
	}

	private new void Move(MoveOrientation orientation)
	{
		SIGCircuit nextCircuit = _circuit.GetNextCircuit(orientation);
		SetCircuit(nextCircuit);
	}

	private void MoveUp()
	{
		SIGCircuit nextCircuit = _circuit.GetNextCircuit(MoveOrientation.NE);
		if (nextCircuit == null)
		{
			nextCircuit = _circuit.GetNextCircuit(MoveOrientation.NW);
		}
		SetCircuit(nextCircuit);
	}

	private void MoveDown()
	{
		SIGCircuit nextCircuit = _circuit.GetNextCircuit(MoveOrientation.SE);
		if (nextCircuit == null)
		{
			nextCircuit = _circuit.GetNextCircuit(MoveOrientation.SW);
		}
		SetCircuit(nextCircuit);
	}

	private void SetCircuit(SIGCircuit circuit)
	{
		ComposantsViewer viewer = ComposantsViewer.Viewer;
		if (circuit == null)
		{
			_circuit = null;
			lNom.Text = string.Empty;
			lNom.ForeColor = Color.Black;
			modeleViewer.Modele = null;
		}
		else
		{
			if (circuit == _circuit)
			{
				return;
			}
			_circuit = circuit;
			viewer.MoveToCDV(_circuit);
			lNom.Text = _circuit.Nom;
			lNom.ForeColor = Colors.GetColor("CDV" + _circuit.Frequence);
			fbE.Visible = _circuit.GetNextCircuit(MoveOrientation.E) != null;
			fbW.Visible = _circuit.GetNextCircuit(MoveOrientation.W) != null;
			fbNE.Visible = _circuit.GetNextCircuit(MoveOrientation.NE) != null;
			fbNW.Visible = _circuit.GetNextCircuit(MoveOrientation.NW) != null;
			fbSE.Visible = _circuit.GetNextCircuit(MoveOrientation.SE) != null;
			fbSW.Visible = _circuit.GetNextCircuit(MoveOrientation.SW) != null;
			CCircuit circuit2 = viewer.Composants.GetCircuit(_circuit);
			SetMarqueurs(circuit2);
			viewer.SaveLimits();
			int num = int.MaxValue;
			int num2 = int.MinValue;
			int num3 = int.MaxValue;
			int num4 = int.MinValue;
			foreach (CSegment segment in circuit2.Segments)
			{
				num = Math.Min(num, Math.Min(segment.PkD, segment.PkF));
				num2 = Math.Max(num2, Math.Max(segment.PkD, segment.PkF));
				num3 = Math.Min(num3, segment.Support.MinPosY);
				num4 = Math.Max(num4, segment.Support.MaxPosY);
			}
			if (num3 == num4)
			{
				num3--;
				num4++;
			}
			viewer.SetLimits(100, 40, 10, 10, num, num2, num3, num4);
			lvModeles.Items.Clear();
			for (int i = 0; i < _circuit.DemiJoints.Count; i++)
			{
				SIGDemiJoint sIGDemiJoint = _circuit.DemiJoints[i];
				for (int j = i + 1; j < _circuit.DemiJoints.Count; j++)
				{
					SIGDemiJoint sIGDemiJoint2 = _circuit.DemiJoints[j];
					List<SIGSegment> list = CDV_Viewer.Traitements.Composants.FindParcours(sIGDemiJoint.Joint, sIGDemiJoint2.Joint);
					List<SIGSegment> list2 = CDV_Viewer.Traitements.Composants.FindParcours(sIGDemiJoint2.Joint, sIGDemiJoint.Joint);
					if (list.Count > 0 && list2.Count > 0)
					{
						SIGModele modele = _circuit.GetModele(sIGDemiJoint, sIGDemiJoint2);
						SIGModele modele2 = _circuit.GetModele(sIGDemiJoint2, sIGDemiJoint);
						lvModeles.Items.Add(new SimpleListViewItem(modele, GetBitmap(list, circuit2.Segments)));
						lvModeles.Items.Add(new SimpleListViewItem(modele2, GetBitmap(list2, circuit2.Segments)));
					}
				}
			}
			viewer.RestoreLimits();
			int count = lvModeles.Items.Count;
			lEmpty.Visible = count == 0;
			if (count > 0)
			{
				modeleViewer.Modele = (SIGModele)lvModeles.Items[0].Tag;
			}
			else
			{
				modeleViewer.Modele = null;
			}
			this.CircuitChanged?.Invoke(this, new EventArgs());
		}
	}

	private void SetMarqueurs(CCircuit composantCircuit)
	{
		ComposantsViewer viewer = ComposantsViewer.Viewer;
		foreach (CMarqueurPK marqueur in _marqueurs)
		{
			viewer.Composants.Remove(marqueur);
		}
		_marqueurs.Clear();
		if (!base.Visible || composantCircuit == null)
		{
			return;
		}
		foreach (CSegment _segment in composantCircuit.Segments)
		{
			bool yPos = false;
			if (!(_segment.Support is CVoieAdjacente))
			{
				if (_marqueurs.Find((CMarqueurPK marqueur) => marqueur.PK == _segment.PkD) == null)
				{
					CMarqueurPK cMarqueurPK = new CMarqueurPK(_segment.PkD, yPos);
					_marqueurs.Add(cMarqueurPK);
					viewer.Composants.Add(cMarqueurPK);
				}
				if (_marqueurs.Find((CMarqueurPK marqueur) => marqueur.PK == _segment.PkF) == null)
				{
					CMarqueurPK cMarqueurPK2 = new CMarqueurPK(_segment.PkF, yPos);
					_marqueurs.Add(cMarqueurPK2);
					viewer.Composants.Add(cMarqueurPK2);
				}
			}
		}
	}

	private Bitmap GetBitmap(List<SIGSegment> chemin, List<CSegment> cSegments)
	{
		Bitmap bitmap = new Bitmap(120, 60);
		Pen pen = new Pen(Color.Gray, 2f);
		Pen pen2 = new Pen(lNom.ForeColor, 2f);
		AdjustableArrowCap customEndCap = new AdjustableArrowCap(6f, 6f);
		Pen pen3 = new Pen(lNom.ForeColor, 2f)
		{
			CustomEndCap = customEndCap
		};
		using Graphics graphics = Graphics.FromImage(bitmap);
		foreach (CSegment cSegment2 in cSegments)
		{
			Point[] path = cSegment2.Path;
			for (int i = 0; i < path.Length - 1; i++)
			{
				graphics.DrawLine(pen, path[i], path[i + 1]);
			}
		}
		foreach (SIGSegment _segment in chemin)
		{
			CSegment cSegment = cSegments.Find((CSegment segment) => segment.Support.Voie == _segment.Voie);
			if (cSegment == null)
			{
				continue;
			}
			int v = _segment.PkD;
			int v2 = _segment.PkF;
			if (cSegment.Support is CVoieAdjacente)
			{
				CVoieAdjacente obj = (CVoieAdjacente)cSegment.Support;
				v = obj.Pk;
				v2 = v + 1;
				if (_segment.PkD > _segment.PkF)
				{
					Calculs.Swap(ref v, ref v2);
				}
				if (obj.Noeud.ChgSensPk)
				{
					Calculs.Swap(ref v, ref v2);
				}
			}
			Point[] path2 = cSegment.Support.GetPath(v, v2);
			for (int num = 0; num < path2.Length - 1; num++)
			{
				graphics.DrawLine(pen2, path2[num], path2[num + 1]);
				int num2 = path2[num + 1].X - path2[num].X;
				int num3 = path2[num + 1].Y - path2[num].Y;
				if ((double)(num2 * num2 + num3 * num3) > 100.0)
				{
					Point pt = new Point((path2[num + 1].X + path2[num].X) / 2, (path2[num + 1].Y + path2[num].Y) / 2);
					graphics.DrawLine(pt2: new Point(pt.X + 4 * Math.Sign(num2), (Math.Abs(num2) * pt.Y + 4 * num3) / Math.Abs(num2)), pen: pen3, pt1: pt);
				}
			}
		}
		return bitmap;
	}

	protected override void OnKeyUp(KeyEventArgs e)
	{
		if (base.Enabled)
		{
			switch (e.KeyData)
			{
			case Keys.Left:
				Move(MoveOrientation.W);
				break;
			case Keys.Right:
				Move(MoveOrientation.E);
				break;
			case Keys.Up:
				MoveUp();
				break;
			case Keys.Down:
				MoveDown();
				break;
			}
		}
	}

	protected override void OnVisibleChanged(EventArgs e)
	{
		base.OnVisibleChanged(e);
		CCircuit circuit = ComposantsViewer.Viewer.Composants.GetCircuit(_circuit);
		SetMarqueurs(circuit);
	}

	private void bFermer_Click(object sender, EventArgs e)
	{
		base.Visible = false;
	}

	private void lvModeles_SelectedIndexChanged(object sender, EventArgs e)
	{
		modeleViewer.Modele = (SIGModele)lvModeles.SelectedItem.Tag;
	}

	private void flecheButton_Click(object sender, EventArgs e)
	{
		Move(((FlecheButton)sender).Orientation);
	}

	private void ComposantsViewer_SensPkChanged(object sender, EventArgs e)
	{
		SetCircuit(_circuit);
		for (int i = 0; i < lvModeles.Items.Count; i++)
		{
			if (lvModeles.Items[i].Tag == modeleViewer.Modele)
			{
				lvModeles.SelectedIndex = i;
			}
		}
	}

	protected override void Dispose(bool disposing)
	{
		if (disposing && components != null)
		{
			components.Dispose();
		}
		base.Dispose(disposing);
	}

	private void InitializeComponent()
	{
		this.modeleViewer = new CDV_Viewer.Controls.SIGModeleViewer();
		this.separator1 = new CDV_Viewer.Controls.Separator();
		this.label4 = new System.Windows.Forms.Label();
		this.bFermer = new CDV_Viewer.Controls.CloseButton();
		this.fbE = new CDV_Viewer.Controls.FlecheButton();
		this.fbW = new CDV_Viewer.Controls.FlecheButton();
		this.fbNW = new CDV_Viewer.Controls.FlecheButton();
		this.fbSW = new CDV_Viewer.Controls.FlecheButton();
		this.fbNE = new CDV_Viewer.Controls.FlecheButton();
		this.fbSE = new CDV_Viewer.Controls.FlecheButton();
		this.lvModeles = new CDV_Viewer.Controls.SimpleListView();
		this.separator2 = new CDV_Viewer.Controls.Separator();
		this.lEmpty = new System.Windows.Forms.Label();
		this.lNom = new System.Windows.Forms.Label();
		base.SuspendLayout();
		this.modeleViewer.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.modeleViewer.Location = new System.Drawing.Point(0, 152);
		this.modeleViewer.MinimumSize = new System.Drawing.Size(50, 50);
		this.modeleViewer.Modele = null;
		this.modeleViewer.Name = "modeleViewer";
		this.modeleViewer.Size = new System.Drawing.Size(480, 150);
		this.modeleViewer.Statique = false;
		this.modeleViewer.TabIndex = 0;
		this.separator1.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.separator1.ForeColor = System.Drawing.Color.Gainsboro;
		this.separator1.Location = new System.Drawing.Point(10, 25);
		this.separator1.Name = "separator1";
		this.separator1.Orientation = System.Windows.Forms.Orientation.Horizontal;
		this.separator1.Size = new System.Drawing.Size(460, 3);
		this.separator1.TabIndex = 19;
		this.separator1.Text = "separator1";
		this.label4.AutoSize = true;
		this.label4.Font = new System.Drawing.Font("Arial", 9f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		this.label4.ForeColor = System.Drawing.Color.FromArgb(224, 82, 6);
		this.label4.Location = new System.Drawing.Point(6, 6);
		this.label4.Name = "label4";
		this.label4.Size = new System.Drawing.Size(179, 15);
		this.label4.TabIndex = 18;
		this.label4.Text = "VISUALISATEUR DE MODÈLES";
		this.bFermer.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
		this.bFermer.Cursor = System.Windows.Forms.Cursors.Hand;
		this.bFermer.Location = new System.Drawing.Point(462, 3);
		this.bFermer.Name = "bFermer";
		this.bFermer.Size = new System.Drawing.Size(15, 15);
		this.bFermer.TabIndex = 17;
		this.bFermer.Text = "closeButton1";
		this.bFermer.UseVisualStyleBackColor = true;
		this.fbE.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
		this.fbE.Cursor = System.Windows.Forms.Cursors.Hand;
		this.fbE.Location = new System.Drawing.Point(452, 62);
		this.fbE.Name = "fbE";
		this.fbE.Orientation = CDV_Viewer.Data.MoveOrientation.E;
		this.fbE.Size = new System.Drawing.Size(28, 44);
		this.fbE.TabIndex = 20;
		this.fbE.Text = "flecheButton1";
		this.fbE.UseVisualStyleBackColor = true;
		this.fbW.Cursor = System.Windows.Forms.Cursors.Hand;
		this.fbW.Location = new System.Drawing.Point(0, 62);
		this.fbW.Name = "fbW";
		this.fbW.Orientation = CDV_Viewer.Data.MoveOrientation.W;
		this.fbW.Size = new System.Drawing.Size(28, 44);
		this.fbW.TabIndex = 21;
		this.fbW.Text = "flecheButton1";
		this.fbW.UseVisualStyleBackColor = true;
		this.fbNW.Cursor = System.Windows.Forms.Cursors.Hand;
		this.fbNW.Location = new System.Drawing.Point(0, 34);
		this.fbNW.Name = "fbNW";
		this.fbNW.Orientation = CDV_Viewer.Data.MoveOrientation.NW;
		this.fbNW.Size = new System.Drawing.Size(28, 28);
		this.fbNW.TabIndex = 22;
		this.fbNW.Text = "flecheButton2";
		this.fbNW.UseVisualStyleBackColor = true;
		this.fbSW.Cursor = System.Windows.Forms.Cursors.Hand;
		this.fbSW.Location = new System.Drawing.Point(0, 106);
		this.fbSW.Name = "fbSW";
		this.fbSW.Orientation = CDV_Viewer.Data.MoveOrientation.SW;
		this.fbSW.Size = new System.Drawing.Size(28, 28);
		this.fbSW.TabIndex = 23;
		this.fbSW.Text = "flecheButton3";
		this.fbSW.UseVisualStyleBackColor = true;
		this.fbNE.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
		this.fbNE.Cursor = System.Windows.Forms.Cursors.Hand;
		this.fbNE.Location = new System.Drawing.Point(452, 34);
		this.fbNE.Name = "fbNE";
		this.fbNE.Orientation = CDV_Viewer.Data.MoveOrientation.NE;
		this.fbNE.Size = new System.Drawing.Size(28, 28);
		this.fbNE.TabIndex = 24;
		this.fbNE.Text = "flecheButton4";
		this.fbNE.UseVisualStyleBackColor = true;
		this.fbSE.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
		this.fbSE.Cursor = System.Windows.Forms.Cursors.Hand;
		this.fbSE.Location = new System.Drawing.Point(451, 106);
		this.fbSE.Name = "fbSE";
		this.fbSE.Orientation = CDV_Viewer.Data.MoveOrientation.SE;
		this.fbSE.Size = new System.Drawing.Size(28, 28);
		this.fbSE.TabIndex = 25;
		this.fbSE.Text = "flecheButton5";
		this.fbSE.UseVisualStyleBackColor = true;
		this.lvModeles.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.lvModeles.Cursor = System.Windows.Forms.Cursors.Hand;
		this.lvModeles.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
		this.lvModeles.HoverColor = System.Drawing.Color.FromArgb(230, 230, 230);
		this.lvModeles.ItemSize = 140;
		this.lvModeles.Location = new System.Drawing.Point(35, 62);
		this.lvModeles.Name = "lvModeles";
		this.lvModeles.Orientation = CDV_Viewer.Controls.ScrollBarOrientation.Horizontal;
		this.lvModeles.SelectedColor = System.Drawing.Color.Gainsboro;
		this.lvModeles.SelectedIndex = -1;
		this.lvModeles.Size = new System.Drawing.Size(410, 72);
		this.lvModeles.TabIndex = 26;
		this.lvModeles.Text = "simpleListView1";
		this.separator2.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.separator2.ForeColor = System.Drawing.Color.Gainsboro;
		this.separator2.Location = new System.Drawing.Point(9, 142);
		this.separator2.Name = "separator2";
		this.separator2.Orientation = System.Windows.Forms.Orientation.Horizontal;
		this.separator2.Size = new System.Drawing.Size(460, 3);
		this.separator2.TabIndex = 27;
		this.separator2.Text = "separator2";
		this.lEmpty.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.lEmpty.Font = new System.Drawing.Font("Arial", 14.25f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		this.lEmpty.ForeColor = System.Drawing.Color.Gray;
		this.lEmpty.Location = new System.Drawing.Point(30, 62);
		this.lEmpty.Name = "lEmpty";
		this.lEmpty.Size = new System.Drawing.Size(418, 72);
		this.lEmpty.TabIndex = 28;
		this.lEmpty.Text = "AUCUN MODÈLE DISPONIBLE";
		this.lEmpty.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
		this.lNom.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.lNom.Font = new System.Drawing.Font("Arial", 18f, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, 0);
		this.lNom.Location = new System.Drawing.Point(35, 34);
		this.lNom.Name = "lNom";
		this.lNom.Size = new System.Drawing.Size(413, 28);
		this.lNom.TabIndex = 29;
		this.lNom.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
		this.BackColor = System.Drawing.Color.White;
		base.Controls.Add(this.lNom);
		base.Controls.Add(this.lEmpty);
		base.Controls.Add(this.separator2);
		base.Controls.Add(this.lvModeles);
		base.Controls.Add(this.fbSE);
		base.Controls.Add(this.fbNE);
		base.Controls.Add(this.fbSW);
		base.Controls.Add(this.fbNW);
		base.Controls.Add(this.fbW);
		base.Controls.Add(this.fbE);
		base.Controls.Add(this.separator1);
		base.Controls.Add(this.label4);
		base.Controls.Add(this.bFermer);
		base.Controls.Add(this.modeleViewer);
		base.Name = "SIGControlModeleViewer";
		base.Size = new System.Drawing.Size(480, 305);
		base.Controls.SetChildIndex(this.modeleViewer, 0);
		base.Controls.SetChildIndex(this.bFermer, 0);
		base.Controls.SetChildIndex(this.label4, 0);
		base.Controls.SetChildIndex(this.separator1, 0);
		base.Controls.SetChildIndex(this.fbE, 0);
		base.Controls.SetChildIndex(this.fbW, 0);
		base.Controls.SetChildIndex(this.fbNW, 0);
		base.Controls.SetChildIndex(this.fbSW, 0);
		base.Controls.SetChildIndex(this.fbNE, 0);
		base.Controls.SetChildIndex(this.fbSE, 0);
		base.Controls.SetChildIndex(this.lvModeles, 0);
		base.Controls.SetChildIndex(this.separator2, 0);
		base.Controls.SetChildIndex(this.lEmpty, 0);
		base.Controls.SetChildIndex(this.lNom, 0);
		base.ResumeLayout(false);
		base.PerformLayout();
	}
}
