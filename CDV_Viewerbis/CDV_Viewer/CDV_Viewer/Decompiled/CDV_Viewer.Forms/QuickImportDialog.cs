using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using CDV_Viewer.CsvBase;
using CDV_Viewer.Data;
using CDV_Viewer.Data.SIGObjects;
using CDV_Viewer.Styles;
using CDV_Viewer.Traitements;

namespace CDV_Viewer.Forms;

public class QuickImportDialog : Form
{
	private List<SIGJoint> _joints;

	private List<SIGCircuit> _circuits;

	private List<SIGLinkJointCircuit> _links;

	private IContainer components;

	private PictureBox pbGraph;

	private Button bValider;

	private Button bAnnuler;

	public QuickImportDialog(List<SIGJoint> joints, List<SIGCircuit> circuits, List<SIGLinkJointCircuit> links)
	{
		InitializeComponent();
		_joints = joints;
		_circuits = circuits;
		_links = links;
		base.Resize += SIGPreviewImportForm_Resize;
		pbGraph.Paint += pbGraph_Paint;
		bAnnuler.Click += bAnnuler_Click;
		bValider.Click += bValider_Click;
	}

	private void SIGPreviewImportForm_Resize(object sender, EventArgs e)
	{
		pbGraph.Invalidate();
	}

	private void pbGraph_Paint(object sender, PaintEventArgs e)
	{
		Graphics graphics = e.Graphics;
		if (_joints.Count == 0)
		{
			return;
		}
		int num = pbGraph.Height / 2 - 1;
		int num2 = _joints[0].PK;
		int num3 = _joints[0].PK;
		foreach (SIGJoint joint in _joints)
		{
			num2 = Math.Min(num2, joint.PK);
			num3 = Math.Max(num3, joint.PK);
		}
		double num4 = Math.Max((double)(num3 - num2) / (double)(pbGraph.Width - 60), 1.0);
		StringFormat stringFormat = new StringFormat();
		stringFormat.Alignment = StringAlignment.Center;
		Font font = new Font(Global.DefaultFont, FontStyle.Bold);
		foreach (SIGCircuit _circuit in _circuits)
		{
			List<SIGLinkJointCircuit> _linksCircuit = _links.FindAll((SIGLinkJointCircuit link) => link.Circuit == _circuit.ID);
			if (_linksCircuit.Count == 2)
			{
				SIGJoint sIGJoint = _joints.Find((SIGJoint joint) => joint.ID == _linksCircuit[0].Joint);
				SIGJoint sIGJoint2 = _joints.Find((SIGJoint joint) => joint.ID == _linksCircuit[1].Joint);
				if (sIGJoint != null && sIGJoint2 != null)
				{
					Color color = Colors.GetColor("CDV" + _circuit.Frequence);
					Pen pen = new Pen(color, 2f);
					graphics.DrawLine(pen, (int)(20.0 + (double)(sIGJoint.PK - num2) / num4), num, (int)(20.0 + (double)(sIGJoint2.PK - num2) / num4), num);
					int num5 = (sIGJoint.PK + sIGJoint2.PK) / 2;
					graphics.DrawString(_circuit.Nom, font, new SolidBrush(color), new Rectangle((int)((double)(num5 - num2) / num4), num - 15, 40, 10), stringFormat);
				}
			}
		}
		font = Global.DefaultFont;
		foreach (SIGJoint joint2 in _joints)
		{
			Pen pen2 = new Pen(Colors.GetColor("Joint"), 2f);
			Point point = new Point((int)(20.0 + (double)(joint2.PK - num2) / num4), num);
			switch (joint2.Type)
			{
			case JointType.INC:
			{
				Rectangle rectangle = new Rectangle(point.X - 5, point.Y - 5, 10, 10);
				graphics.FillRectangle(new SolidBrush(Colors.GetColor("Joint")), rectangle);
				stringFormat.LineAlignment = StringAlignment.Center;
				graphics.DrawString("?", new Font("Arial", 7f), Brushes.White, rectangle, stringFormat);
				break;
			}
			case JointType.CC:
				graphics.DrawLine(pen2, point.X - 3, point.Y - 4, point.X - 3, point.Y + 4);
				graphics.DrawLine(pen2, point.X, point.Y - 5, point.X, point.Y + 5);
				graphics.DrawLine(pen2, point.X + 3, point.Y - 4, point.X + 3, point.Y + 4);
				graphics.DrawLine(pen2, point.X - 3, point.Y, point.X + 3, point.Y);
				break;
			case JointType.SVAC:
				graphics.DrawLine(pen2, point.X - 3, point.Y - 5, point.X - 3, point.Y + 5);
				graphics.DrawLine(pen2, point.X + 3, point.Y - 5, point.X + 3, point.Y + 5);
				graphics.DrawLine(pen2, point.X - 3, point.Y, point.X + 3, point.Y);
				break;
			case JointType.JI:
				graphics.DrawLine(pen2, point.X - 1, point.Y - 5, point.X - 1, point.Y + 5);
				graphics.DrawLine(pen2, point.X + 1, point.Y - 5, point.X + 1, point.Y + 5);
				break;
			}
			graphics.DrawString(Chaines.PkToString(joint2.PK), font, Brushes.Black, new Rectangle(point.X - 20, num - 20, 40, 10), stringFormat);
		}
	}

	private void bAnnuler_Click(object sender, EventArgs e)
	{
		base.DialogResult = DialogResult.Cancel;
		Close();
	}

	private void bValider_Click(object sender, EventArgs e)
	{
		base.DialogResult = DialogResult.OK;
		Close();
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
		this.pbGraph = new System.Windows.Forms.PictureBox();
		this.bValider = new System.Windows.Forms.Button();
		this.bAnnuler = new System.Windows.Forms.Button();
		((System.ComponentModel.ISupportInitialize)this.pbGraph).BeginInit();
		base.SuspendLayout();
		this.pbGraph.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.pbGraph.BackColor = System.Drawing.Color.White;
		this.pbGraph.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
		this.pbGraph.Location = new System.Drawing.Point(12, 12);
		this.pbGraph.Name = "pbGraph";
		this.pbGraph.Size = new System.Drawing.Size(414, 259);
		this.pbGraph.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
		this.pbGraph.TabIndex = 0;
		this.pbGraph.TabStop = false;
		this.bValider.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
		this.bValider.Location = new System.Drawing.Point(270, 277);
		this.bValider.Name = "bValider";
		this.bValider.Size = new System.Drawing.Size(75, 23);
		this.bValider.TabIndex = 1;
		this.bValider.Text = "Valider";
		this.bValider.UseVisualStyleBackColor = true;
		this.bAnnuler.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
		this.bAnnuler.Location = new System.Drawing.Point(351, 277);
		this.bAnnuler.Name = "bAnnuler";
		this.bAnnuler.Size = new System.Drawing.Size(75, 23);
		this.bAnnuler.TabIndex = 2;
		this.bAnnuler.Text = "Annuler";
		this.bAnnuler.UseVisualStyleBackColor = true;
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.ClientSize = new System.Drawing.Size(438, 309);
		base.Controls.Add(this.bAnnuler);
		base.Controls.Add(this.bValider);
		base.Controls.Add(this.pbGraph);
		this.DoubleBuffered = true;
		this.MinimumSize = new System.Drawing.Size(300, 200);
		base.Name = "QuickImportDialog";
		base.ShowIcon = false;
		base.ShowInTaskbar = false;
		base.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
		this.Text = "Prévisualisation de l'import";
		((System.ComponentModel.ISupportInitialize)this.pbGraph).EndInit();
		base.ResumeLayout(false);
	}
}
