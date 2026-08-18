using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;
using CDV_Viewer.CsvBase;
using CDV_Viewer.Data;
using CDV_Viewer.Properties;

namespace CDV_Viewer.Forms;

public class EditBaseDialog : Form
{
	private IContainer components;

	private Label label1;

	private Button bDossier;

	private Button bTerminer;

	private Label label2;

	private Label label3;

	public static void Open()
	{
		new EditBaseDialog().ShowDialog();
	}

	public EditBaseDialog()
	{
		InitializeComponent();
		if (!Base.IsSave)
		{
			MessageBox.Show("l'archive n'est pas a jour", Resources.APP_NAME, MessageBoxButtons.OK);
			return;
		}
		Archives.CurrentArchive.ToTempFolder();
		bDossier.Click += bDossier_Click;
		bTerminer.Click += bTerminer_Click;
	}

	private void bDossier_Click(object sender, EventArgs e)
	{
		Process.Start(Paths.TempDataFolder);
	}

	private void bTerminer_Click(object sender, EventArgs e)
	{
		Archives.CurrentArchive.LoadFromTempFolder();
		Base.SetModif();
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
		this.label1 = new System.Windows.Forms.Label();
		this.bDossier = new System.Windows.Forms.Button();
		this.bTerminer = new System.Windows.Forms.Button();
		this.label2 = new System.Windows.Forms.Label();
		this.label3 = new System.Windows.Forms.Label();
		base.SuspendLayout();
		this.label1.AutoSize = true;
		this.label1.Location = new System.Drawing.Point(12, 9);
		this.label1.Name = "label1";
		this.label1.Size = new System.Drawing.Size(304, 13);
		this.label1.TabIndex = 3;
		this.label1.Text = "Les fichiers de la base sont présent dans le dossier temporaire. ";
		this.bDossier.Location = new System.Drawing.Point(95, 91);
		this.bDossier.Name = "bDossier";
		this.bDossier.Size = new System.Drawing.Size(154, 23);
		this.bDossier.TabIndex = 4;
		this.bDossier.Text = "Acceder aux fichiers";
		this.bDossier.UseVisualStyleBackColor = true;
		this.bTerminer.Location = new System.Drawing.Point(95, 141);
		this.bTerminer.Name = "bTerminer";
		this.bTerminer.Size = new System.Drawing.Size(154, 23);
		this.bTerminer.TabIndex = 5;
		this.bTerminer.Text = "Accepter les modifications";
		this.bTerminer.UseVisualStyleBackColor = true;
		this.label2.AutoSize = true;
		this.label2.Location = new System.Drawing.Point(12, 31);
		this.label2.Name = "label2";
		this.label2.Size = new System.Drawing.Size(204, 13);
		this.label2.TabIndex = 6;
		this.label2.Text = "Vous pouvez éditez ces fichiers à la main.";
		this.label3.AutoSize = true;
		this.label3.Location = new System.Drawing.Point(12, 53);
		this.label3.Name = "label3";
		this.label3.Size = new System.Drawing.Size(322, 13);
		this.label3.TabIndex = 7;
		this.label3.Text = "Lorsque vous avez terminer l'édition, cliquez sur le bouton Terminer";
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.ClientSize = new System.Drawing.Size(346, 173);
		base.Controls.Add(this.label3);
		base.Controls.Add(this.label2);
		base.Controls.Add(this.bTerminer);
		base.Controls.Add(this.bDossier);
		base.Controls.Add(this.label1);
		base.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
		base.MaximizeBox = false;
		base.MinimizeBox = false;
		base.Name = "EditBaseDialog";
		this.Text = "Edition Manuelle de la base";
		base.ResumeLayout(false);
		base.PerformLayout();
	}
}
