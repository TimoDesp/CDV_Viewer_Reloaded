using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using CDV_Viewer.CsvBase;
using CDV_Viewer.Data.SIGObjects;
using CDV_Viewer.Properties;

namespace CDV_Viewer.Forms;

public class AddLigneDialog : Form
{
	private int _selectedLigne = -1;

	private IContainer components;

	private Label label1;

	private TextBox tbNumero;

	private Button bValider;

	private TextBox tbNom;

	private Label label2;

	private Button bAnnuler;

	private Panel panel1;

	public int SelectedLigne => _selectedLigne;

	public static void Open()
	{
		new AddLigneDialog().ShowDialog();
	}

	public AddLigneDialog()
	{
		InitializeComponent();
		bAnnuler.Click += bAnnuler_Click;
		bValider.Click += bValider_Click;
	}

	private void bAnnuler_Click(object sender, EventArgs e)
	{
		base.DialogResult = DialogResult.Cancel;
		Close();
	}

	private void bValider_Click(object sender, EventArgs e)
	{
		int result = -1;
		if (tbNumero.Text.Length > 6 || !int.TryParse(tbNumero.Text, out result))
		{
			MessageBox.Show("Numéro de ligne incorrect", Resources.APP_NAME);
			return;
		}
		foreach (SIGLigne ligne in Base.GetLignes())
		{
			if (ligne.ID == result)
			{
				MessageBox.Show("Ligne déjà existante", Resources.APP_NAME);
				return;
			}
		}
		if (tbNom.Text == "")
		{
			MessageBox.Show("Nom de ligne incorrect", Resources.APP_NAME);
			return;
		}
		Base.CreateLigne(result, tbNom.Text);
		MessageBox.Show("Ligne ajoutée !", Resources.APP_NAME);
		base.DialogResult = DialogResult.OK;
		_selectedLigne = result;
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
		this.tbNumero = new System.Windows.Forms.TextBox();
		this.bValider = new System.Windows.Forms.Button();
		this.tbNom = new System.Windows.Forms.TextBox();
		this.label2 = new System.Windows.Forms.Label();
		this.bAnnuler = new System.Windows.Forms.Button();
		this.panel1 = new System.Windows.Forms.Panel();
		this.panel1.SuspendLayout();
		base.SuspendLayout();
		this.label1.AutoSize = true;
		this.label1.Location = new System.Drawing.Point(13, 14);
		this.label1.Name = "label1";
		this.label1.Size = new System.Drawing.Size(90, 13);
		this.label1.TabIndex = 0;
		this.label1.Text = "Numéro de ligne :";
		this.tbNumero.Location = new System.Drawing.Point(109, 11);
		this.tbNumero.MaxLength = 6;
		this.tbNumero.Name = "tbNumero";
		this.tbNumero.Size = new System.Drawing.Size(77, 20);
		this.tbNumero.TabIndex = 1;
		this.bValider.Location = new System.Drawing.Point(56, 89);
		this.bValider.Name = "bValider";
		this.bValider.Size = new System.Drawing.Size(75, 23);
		this.bValider.TabIndex = 5;
		this.bValider.Text = "Valider";
		this.bValider.UseVisualStyleBackColor = true;
		this.tbNom.Location = new System.Drawing.Point(54, 37);
		this.tbNom.Name = "tbNom";
		this.tbNom.Size = new System.Drawing.Size(132, 20);
		this.tbNom.TabIndex = 4;
		this.label2.AutoSize = true;
		this.label2.Location = new System.Drawing.Point(13, 40);
		this.label2.Name = "label2";
		this.label2.Size = new System.Drawing.Size(35, 13);
		this.label2.TabIndex = 3;
		this.label2.Text = "Nom :";
		this.bAnnuler.Location = new System.Drawing.Point(137, 89);
		this.bAnnuler.Name = "bAnnuler";
		this.bAnnuler.Size = new System.Drawing.Size(75, 23);
		this.bAnnuler.TabIndex = 6;
		this.bAnnuler.Text = "Annuler";
		this.bAnnuler.UseVisualStyleBackColor = true;
		this.panel1.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
		this.panel1.Controls.Add(this.label1);
		this.panel1.Controls.Add(this.tbNumero);
		this.panel1.Controls.Add(this.tbNom);
		this.panel1.Controls.Add(this.label2);
		this.panel1.Location = new System.Drawing.Point(12, 12);
		this.panel1.Name = "panel1";
		this.panel1.Size = new System.Drawing.Size(200, 71);
		this.panel1.TabIndex = 7;
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.ClientSize = new System.Drawing.Size(223, 127);
		base.Controls.Add(this.panel1);
		base.Controls.Add(this.bAnnuler);
		base.Controls.Add(this.bValider);
		base.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
		base.MaximizeBox = false;
		base.MinimizeBox = false;
		base.Name = "AddLigneDialog";
		base.ShowIcon = false;
		base.ShowInTaskbar = false;
		base.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
		this.Text = "Créer une ligne";
		this.panel1.ResumeLayout(false);
		this.panel1.PerformLayout();
		base.ResumeLayout(false);
	}
}
