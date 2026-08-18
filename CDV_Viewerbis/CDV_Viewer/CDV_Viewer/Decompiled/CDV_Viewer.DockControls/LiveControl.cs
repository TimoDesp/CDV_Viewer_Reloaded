using System;
using System.ComponentModel;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;
using CDV_Viewer.Composants;
using CDV_Viewer.Controls;
using CDV_Viewer.Data;

namespace CDV_Viewer.DockControls;

public class LiveControl : DockChild
{
	private bool _started;

	private CMarqueurPK _marqueur;

	private IContainer components;

	private Label lLigne;

	private Label lVoie;

	private Label lPK;

	private Label lVitesse;

	private Button bStart;

	private CloseButton bFermer;

	public LiveControl()
	{
		InitializeComponent();
		MinimumSize = new Size(200, 30);
		MaximumSize = new Size(4000, 30);
		bStart.Click += bStart_Click;
		bFermer.Click += bFermer_Click;
	}

	private void bFermer_Click(object sender, EventArgs e)
	{
		base.Visible = false;
	}

	private void bStart_Click(object sender, EventArgs e)
	{
		if (_started)
		{
			LocReceiver.CurrentLoc.Stop();
			LocReceiver.CurrentLoc.LocChanged -= CurrentLoc_LocChanged;
			ComposantsViewer.Viewer.Composants.Remove(_marqueur);
			_started = false;
		}
		else
		{
			LocReceiver.CurrentLoc.Start();
			LocReceiver.CurrentLoc.LocChanged += CurrentLoc_LocChanged;
			_marqueur = new CMarqueurPK(0, yPos: false);
			ComposantsViewer.Viewer.Composants.Add(_marqueur);
			_started = true;
		}
	}

	private void CurrentLoc_LocChanged(object sender, EventArgs e)
	{
		Invoke(new ThreadStart(RefreshData));
	}

	private void RefreshData()
	{
		lLigne.Text = LocReceiver.CurrentLoc.FormatLigne;
		lVoie.Text = LocReceiver.CurrentLoc.FormatVoie;
		lPK.Text = LocReceiver.CurrentLoc.FormatPK;
		lVitesse.Text = LocReceiver.CurrentLoc.FormatVitesse;
		if (ComposantsViewer.Viewer != null)
		{
			if (_marqueur != null)
			{
				_marqueur.SetPK(LocReceiver.CurrentLoc.PK);
			}
			ComposantsViewer.Viewer.SetLignePK(LocReceiver.CurrentLoc.Ligne, LocReceiver.CurrentLoc.PK);
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
		this.lLigne = new System.Windows.Forms.Label();
		this.lVoie = new System.Windows.Forms.Label();
		this.lPK = new System.Windows.Forms.Label();
		this.lVitesse = new System.Windows.Forms.Label();
		this.bStart = new System.Windows.Forms.Button();
		this.bFermer = new CDV_Viewer.Controls.CloseButton();
		base.SuspendLayout();
		this.lLigne.AutoSize = true;
		this.lLigne.Location = new System.Drawing.Point(59, 9);
		this.lLigne.Name = "lLigne";
		this.lLigne.Size = new System.Drawing.Size(43, 13);
		this.lLigne.TabIndex = 0;
		this.lLigne.Text = "000000";
		this.lVoie.AutoSize = true;
		this.lVoie.Location = new System.Drawing.Point(108, 9);
		this.lVoie.Name = "lVoie";
		this.lVoie.Size = new System.Drawing.Size(38, 13);
		this.lVoie.TabIndex = 1;
		this.lVoie.Text = "V1      ";
		this.lPK.AutoSize = true;
		this.lPK.Location = new System.Drawing.Point(152, 9);
		this.lPK.Name = "lPK";
		this.lPK.Size = new System.Drawing.Size(49, 13);
		this.lPK.TabIndex = 2;
		this.lPK.Text = "000+000";
		this.lVitesse.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
		this.lVitesse.AutoSize = true;
		this.lVitesse.Location = new System.Drawing.Point(359, 9);
		this.lVitesse.Name = "lVitesse";
		this.lVitesse.Size = new System.Drawing.Size(38, 13);
		this.lVitesse.TabIndex = 3;
		this.lVitesse.Text = "0km/h";
		this.bStart.Location = new System.Drawing.Point(6, 4);
		this.bStart.Name = "bStart";
		this.bStart.Size = new System.Drawing.Size(43, 23);
		this.bStart.TabIndex = 4;
		this.bStart.Text = "Start";
		this.bStart.UseVisualStyleBackColor = true;
		this.bFermer.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
		this.bFermer.Cursor = System.Windows.Forms.Cursors.Hand;
		this.bFermer.Location = new System.Drawing.Point(401, 8);
		this.bFermer.Name = "bFermer";
		this.bFermer.Size = new System.Drawing.Size(15, 15);
		this.bFermer.TabIndex = 5;
		this.bFermer.Text = "closeButton1";
		this.bFermer.UseVisualStyleBackColor = true;
		base.Controls.Add(this.bFermer);
		base.Controls.Add(this.bStart);
		base.Controls.Add(this.lVitesse);
		base.Controls.Add(this.lPK);
		base.Controls.Add(this.lVoie);
		base.Controls.Add(this.lLigne);
		base.Name = "LiveControl";
		base.Size = new System.Drawing.Size(422, 30);
		base.ResumeLayout(false);
		base.PerformLayout();
	}
}
