using System;
using System.ComponentModel;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;
using CDV_Viewer.Data;
using CDV_Viewer.Properties;

namespace CDV_Viewer.Forms;

public class InitForm : Form
{
	private delegate void DelegateEmpty();

	private delegate void StringDelegate(string texte);

	private IContainer components;

	private Label label1;

	private Label VersionLabel;

	private Label DateAppLabel;

	private Label label3;

	public void Invoke(Action action)
	{
		Invoke((Delegate)action);
	}

	public InitForm()
	{
		InitializeComponent();
		VersionLabel.Text = "v" + Resources.APP_VERSION;
		DateAppLabel.Text = Resources.DATE_APP_VERSION;
		base.Load += ChargementForm_Load;
	}

	private void LoadBase()
	{
		if (!Autorisations.Values.CorrectKey)
		{
			Invoke(delegate
			{
				CloseWithMessage("Clé invalide");
			});
			return;
		}
		Preferences.Load();
		CustomFonts.Load();
		Invoke(delegate
		{
			LaunchMainForm();
		});
	}

	private void LaunchMainForm()
	{
		MainForm mainForm = new MainForm();
		mainForm.FormClosed += MainForm_FormClosed;
		mainForm.Show();
		Hide();
	}

	private void CloseWithMessage(string message)
	{
		MessageBox.Show(message, Resources.APP_NAME);
		Close();
	}

	protected override void OnPaintBackground(PaintEventArgs e)
	{
		e.Graphics.DrawImage(Resources.intro, DisplayRectangle);
		e.Graphics.DrawLine(new Pen(Color.Gray), 0, 0, base.Width - 1, 0);
		e.Graphics.DrawLine(new Pen(Color.Gray), base.Width - 1, 0, base.Width - 1, base.Height - 1);
		e.Graphics.DrawLine(new Pen(Color.Gray), base.Width - 1, base.Height - 1, 0, base.Height - 1);
		e.Graphics.DrawLine(new Pen(Color.Gray), 0, base.Height - 1, 0, 0);
	}

	private void ChargementForm_Load(object sender, EventArgs e)
	{
		label1.BackColor = Color.Transparent;
		new Thread(LoadBase).Start();
	}

	private void MainForm_FormClosed(object sender, FormClosedEventArgs e)
	{
		Preferences.Save();
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
		System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(CDV_Viewer.Forms.InitForm));
		this.label1 = new System.Windows.Forms.Label();
		this.VersionLabel = new System.Windows.Forms.Label();
		this.DateAppLabel = new System.Windows.Forms.Label();
		this.label3 = new System.Windows.Forms.Label();
		base.SuspendLayout();
		this.label1.BackColor = System.Drawing.Color.Transparent;
		this.label1.ForeColor = System.Drawing.Color.FromArgb(103, 92, 83);
		this.label1.Location = new System.Drawing.Point(90, 170);
		this.label1.Name = "label1";
		this.label1.Size = new System.Drawing.Size(140, 16);
		this.label1.TabIndex = 12;
		this.label1.Text = "Chargement...";
		this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
		this.VersionLabel.BackColor = System.Drawing.Color.Transparent;
		this.VersionLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 12f, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, 0);
		this.VersionLabel.ForeColor = System.Drawing.Color.Purple;
		this.VersionLabel.Location = new System.Drawing.Point(98, 116);
		this.VersionLabel.Name = "VersionLabel";
		this.VersionLabel.Size = new System.Drawing.Size(63, 20);
		this.VersionLabel.TabIndex = 13;
		this.VersionLabel.Text = "v0.0";
		this.VersionLabel.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
		this.DateAppLabel.AutoSize = true;
		this.DateAppLabel.BackColor = System.Drawing.Color.Transparent;
		this.DateAppLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75f, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, 0);
		this.DateAppLabel.ForeColor = System.Drawing.Color.Purple;
		this.DateAppLabel.Location = new System.Drawing.Point(194, 117);
		this.DateAppLabel.Margin = new System.Windows.Forms.Padding(0, 0, 3, 0);
		this.DateAppLabel.Name = "DateAppLabel";
		this.DateAppLabel.Size = new System.Drawing.Size(74, 16);
		this.DateAppLabel.TabIndex = 13;
		this.DateAppLabel.Text = "xx/xx/xxxx";
		this.DateAppLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
		this.label3.AutoSize = true;
		this.label3.BackColor = System.Drawing.Color.Transparent;
		this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75f, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, 0);
		this.label3.ForeColor = System.Drawing.Color.Purple;
		this.label3.Location = new System.Drawing.Point(167, 117);
		this.label3.Name = "label3";
		this.label3.Size = new System.Drawing.Size(25, 16);
		this.label3.TabIndex = 13;
		this.label3.Text = "du";
		this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		this.BackColor = System.Drawing.Color.White;
		this.BackgroundImage = CDV_Viewer.Properties.Resources.intro;
		base.ClientSize = new System.Drawing.Size(320, 200);
		base.Controls.Add(this.label3);
		base.Controls.Add(this.DateAppLabel);
		base.Controls.Add(this.VersionLabel);
		base.Controls.Add(this.label1);
		this.ForeColor = System.Drawing.Color.Black;
		base.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
		base.Icon = (System.Drawing.Icon)resources.GetObject("$this.Icon");
		base.Name = "InitForm";
		base.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
		this.Text = "ChargementForm";
		base.ResumeLayout(false);
		base.PerformLayout();
	}
}
