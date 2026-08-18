using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace CDV_Viewer.Forms;

public class EditDebutFinVoieDialog : Form
{
	private IContainer components;

	private Label label1;

	private TextBox tbPkD;

	private TextBox tbPkF;

	private Label label2;

	private Button bValider;

	private Button bAnnuler;

	public int PkDebut
	{
		get
		{
			if (int.TryParse(tbPkD.Text, out var result))
			{
				return result;
			}
			return int.MinValue;
		}
	}

	public int PkFin
	{
		get
		{
			if (int.TryParse(tbPkF.Text, out var result))
			{
				return result;
			}
			return int.MinValue;
		}
	}

	public EditDebutFinVoieDialog()
	{
		InitializeComponent();
		bAnnuler.Click += bAnnuler_Click;
		bValider.Click += bValider_Click;
	}

	private void bAnnuler_Click(object sender, EventArgs e)
	{
		Close();
		base.DialogResult = DialogResult.Cancel;
	}

	private void bValider_Click(object sender, EventArgs e)
	{
		if (PkDebut == int.MinValue)
		{
			MessageBox.Show("PK de début incorrect");
			return;
		}
		if (PkFin == int.MinValue)
		{
			MessageBox.Show("PK de fin incorrect");
			return;
		}
		if (PkDebut < PkFin)
		{
			MessageBox.Show("Le PK de fin doit être superieur au pk de début");
			return;
		}
		Close();
		base.DialogResult = DialogResult.OK;
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
		this.tbPkD = new System.Windows.Forms.TextBox();
		this.tbPkF = new System.Windows.Forms.TextBox();
		this.label2 = new System.Windows.Forms.Label();
		this.bValider = new System.Windows.Forms.Button();
		this.bAnnuler = new System.Windows.Forms.Button();
		base.SuspendLayout();
		this.label1.AutoSize = true;
		this.label1.Location = new System.Drawing.Point(12, 9);
		this.label1.Name = "label1";
		this.label1.Size = new System.Drawing.Size(59, 13);
		this.label1.TabIndex = 0;
		this.label1.Text = "PK Debut :";
		this.tbPkD.Location = new System.Drawing.Point(86, 6);
		this.tbPkD.Name = "tbPkD";
		this.tbPkD.Size = new System.Drawing.Size(49, 20);
		this.tbPkD.TabIndex = 1;
		this.tbPkF.Location = new System.Drawing.Point(86, 32);
		this.tbPkF.Name = "tbPkF";
		this.tbPkF.Size = new System.Drawing.Size(49, 20);
		this.tbPkF.TabIndex = 3;
		this.label2.AutoSize = true;
		this.label2.Location = new System.Drawing.Point(12, 35);
		this.label2.Name = "label2";
		this.label2.Size = new System.Drawing.Size(47, 13);
		this.label2.TabIndex = 2;
		this.label2.Text = "PK Fin : ";
		this.bValider.Location = new System.Drawing.Point(12, 66);
		this.bValider.Name = "bValider";
		this.bValider.Size = new System.Drawing.Size(60, 23);
		this.bValider.TabIndex = 4;
		this.bValider.Text = "Valider";
		this.bValider.UseVisualStyleBackColor = true;
		this.bAnnuler.Location = new System.Drawing.Point(78, 66);
		this.bAnnuler.Name = "bAnnuler";
		this.bAnnuler.Size = new System.Drawing.Size(60, 23);
		this.bAnnuler.TabIndex = 5;
		this.bAnnuler.Text = "Annuler";
		this.bAnnuler.UseVisualStyleBackColor = true;
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.ClientSize = new System.Drawing.Size(153, 98);
		base.Controls.Add(this.bAnnuler);
		base.Controls.Add(this.bValider);
		base.Controls.Add(this.tbPkF);
		base.Controls.Add(this.label2);
		base.Controls.Add(this.tbPkD);
		base.Controls.Add(this.label1);
		base.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
		base.Name = "EditDebutFinVoieDialog";
		base.ResumeLayout(false);
		base.PerformLayout();
	}
}
