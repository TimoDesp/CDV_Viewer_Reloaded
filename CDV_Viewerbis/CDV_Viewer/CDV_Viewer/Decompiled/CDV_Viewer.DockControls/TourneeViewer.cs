using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using CDV_Viewer.Controls;
using CDV_Viewer.Data;

namespace CDV_Viewer.DockControls;

public class TourneeViewer : DockChild
{
	public double _deltaX;

	private IContainer components;

	private SIGParcoursViewer parcoursViewer;

	private Label label4;

	private CloseButton bFermer;

	private Separator separator1;

	private Label lTournee;

	public TourneeViewer()
	{
		InitializeComponent();
		MinimumSize = new Size(460, 140);
		Global.Parcours.ParcoursChanged += Parcours_ParcoursChanged;
		bFermer.Click += bFermer_Click;
	}

	private void bFermer_Click(object sender, EventArgs e)
	{
		base.Visible = false;
	}

	private void Parcours_ParcoursChanged(object sender, EventArgs e)
	{
		if (Global.Parcours.IsOpen)
		{
			lTournee.Text = Global.Parcours.NomTournee;
			base.Visible = true;
		}
		else
		{
			lTournee.Text = "AUCUNE TOURNÉE CHARGÉE";
			base.Visible = false;
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
		this.separator1 = new CDV_Viewer.Controls.Separator();
		this.bFermer = new CDV_Viewer.Controls.CloseButton();
		this.label4 = new System.Windows.Forms.Label();
		this.parcoursViewer = new CDV_Viewer.Controls.SIGParcoursViewer();
		this.lTournee = new System.Windows.Forms.Label();
		base.SuspendLayout();
		this.separator1.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.separator1.ForeColor = System.Drawing.Color.Gainsboro;
		this.separator1.Location = new System.Drawing.Point(10, 25);
		this.separator1.Name = "separator1";
		this.separator1.Orientation = System.Windows.Forms.Orientation.Horizontal;
		this.separator1.Size = new System.Drawing.Size(530, 3);
		this.separator1.TabIndex = 14;
		this.separator1.Text = "separator1";
		this.bFermer.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
		this.bFermer.Cursor = System.Windows.Forms.Cursors.Hand;
		this.bFermer.Location = new System.Drawing.Point(532, 4);
		this.bFermer.Name = "bFermer";
		this.bFermer.Size = new System.Drawing.Size(15, 15);
		this.bFermer.TabIndex = 13;
		this.bFermer.Text = "closeButton1";
		this.bFermer.UseVisualStyleBackColor = true;
		this.label4.AutoSize = true;
		this.label4.Font = new System.Drawing.Font("Arial", 9f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		this.label4.ForeColor = System.Drawing.Color.FromArgb(224, 82, 6);
		this.label4.Location = new System.Drawing.Point(6, 6);
		this.label4.Name = "label4";
		this.label4.Size = new System.Drawing.Size(188, 15);
		this.label4.TabIndex = 12;
		this.label4.Text = "VISUALISATEUR DE TOURNÉES";
		this.parcoursViewer.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.parcoursViewer.BackColor = System.Drawing.Color.White;
		this.parcoursViewer.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
		this.parcoursViewer.Location = new System.Drawing.Point(15, 63);
		this.parcoursViewer.Name = "parcoursViewer";
		this.parcoursViewer.Size = new System.Drawing.Size(526, 49);
		this.parcoursViewer.TabIndex = 5;
		this.lTournee.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.lTournee.Font = new System.Drawing.Font("Arial", 12f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		this.lTournee.ForeColor = System.Drawing.SystemColors.ControlDarkDark;
		this.lTournee.Location = new System.Drawing.Point(15, 31);
		this.lTournee.Name = "lTournee";
		this.lTournee.Size = new System.Drawing.Size(526, 23);
		this.lTournee.TabIndex = 16;
		this.lTournee.Text = "NOM TOURNEE";
		this.lTournee.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
		this.BackColor = System.Drawing.Color.White;
		base.Controls.Add(this.lTournee);
		base.Controls.Add(this.separator1);
		base.Controls.Add(this.bFermer);
		base.Controls.Add(this.label4);
		base.Controls.Add(this.parcoursViewer);
		base.Name = "TourneeViewer";
		base.Size = new System.Drawing.Size(550, 125);
		base.Controls.SetChildIndex(this.parcoursViewer, 0);
		base.Controls.SetChildIndex(this.label4, 0);
		base.Controls.SetChildIndex(this.bFermer, 0);
		base.Controls.SetChildIndex(this.separator1, 0);
		base.Controls.SetChildIndex(this.lTournee, 0);
		base.ResumeLayout(false);
		base.PerformLayout();
	}
}
