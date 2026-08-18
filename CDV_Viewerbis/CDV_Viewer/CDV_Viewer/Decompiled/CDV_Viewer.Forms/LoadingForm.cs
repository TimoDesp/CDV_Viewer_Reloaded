using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace CDV_Viewer.Forms;

public class LoadingForm : Form
{
	public int Maximum;

	public int Avancement;

	public string Texte = string.Empty;

	public bool End;

	private IContainer components;

	private ProgressBar progressBar;

	private Label texte;

	private Timer timer;

	public LoadingForm()
	{
		InitializeComponent();
		timer.Start();
		timer.Tick += timer_Tick;
	}

	private void timer_Tick(object sender, EventArgs e)
	{
		if (End)
		{
			Close();
		}
		progressBar.Maximum = Maximum;
		progressBar.Value = Avancement;
		texte.Text = Texte;
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
		this.components = new System.ComponentModel.Container();
		this.progressBar = new System.Windows.Forms.ProgressBar();
		this.texte = new System.Windows.Forms.Label();
		this.timer = new System.Windows.Forms.Timer(this.components);
		base.SuspendLayout();
		this.progressBar.Location = new System.Drawing.Point(12, 12);
		this.progressBar.Name = "progressBar";
		this.progressBar.Size = new System.Drawing.Size(304, 16);
		this.progressBar.TabIndex = 0;
		this.progressBar.UseWaitCursor = true;
		this.texte.AutoSize = true;
		this.texte.Location = new System.Drawing.Point(12, 33);
		this.texte.Name = "texte";
		this.texte.Size = new System.Drawing.Size(73, 13);
		this.texte.TabIndex = 1;
		this.texte.Text = "Chargement...";
		this.texte.UseWaitCursor = true;
		this.timer.Interval = 200;
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.ClientSize = new System.Drawing.Size(320, 52);
		base.ControlBox = false;
		base.Controls.Add(this.texte);
		base.Controls.Add(this.progressBar);
		base.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
		base.MaximizeBox = false;
		base.MinimizeBox = false;
		base.Name = "LoadingForm";
		base.ShowIcon = false;
		base.ShowInTaskbar = false;
		base.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
		this.Text = "Chargement...";
		base.TopMost = true;
		base.UseWaitCursor = true;
		base.ResumeLayout(false);
		base.PerformLayout();
	}
}
