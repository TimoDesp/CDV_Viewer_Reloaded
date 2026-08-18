using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using CDV_Viewer.Properties;

namespace CDV_Viewer.Forms;

public class SaveDialog : Form
{
	private IContainer components;

	private Button bEnregistrer;

	private Button bAnnuler;

	private TextBox tbDescription;

	private Label label1;

	public string Description
	{
		get
		{
			return tbDescription.Text;
		}
		set
		{
			tbDescription.Text = value;
		}
	}

	public SaveDialog()
	{
		InitializeComponent();
		base.Icon = Icon.FromHandle(Resources.save.GetHicon());
		bAnnuler.Click += bAnnuler_Click;
		bEnregistrer.Click += bEnregistrer_Click;
	}

	private void bAnnuler_Click(object sender, EventArgs e)
	{
		base.DialogResult = DialogResult.Cancel;
		Close();
	}

	private void bEnregistrer_Click(object sender, EventArgs e)
	{
		if (tbDescription.Text == "")
		{
			MessageBox.Show("Veuillez saisir un commentaire", Resources.APP_NAME);
			return;
		}
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
		this.bEnregistrer = new System.Windows.Forms.Button();
		this.bAnnuler = new System.Windows.Forms.Button();
		this.tbDescription = new System.Windows.Forms.TextBox();
		this.label1 = new System.Windows.Forms.Label();
		base.SuspendLayout();
		this.bEnregistrer.Anchor = System.Windows.Forms.AnchorStyles.Top;
		this.bEnregistrer.Location = new System.Drawing.Point(79, 50);
		this.bEnregistrer.Name = "bEnregistrer";
		this.bEnregistrer.Size = new System.Drawing.Size(75, 23);
		this.bEnregistrer.TabIndex = 0;
		this.bEnregistrer.Text = "Enregistrer";
		this.bEnregistrer.UseVisualStyleBackColor = true;
		this.bAnnuler.Anchor = System.Windows.Forms.AnchorStyles.Top;
		this.bAnnuler.Location = new System.Drawing.Point(160, 50);
		this.bAnnuler.Name = "bAnnuler";
		this.bAnnuler.Size = new System.Drawing.Size(75, 23);
		this.bAnnuler.TabIndex = 1;
		this.bAnnuler.Text = "Annuler";
		this.bAnnuler.UseVisualStyleBackColor = true;
		this.tbDescription.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.tbDescription.Location = new System.Drawing.Point(89, 15);
		this.tbDescription.Name = "tbDescription";
		this.tbDescription.Size = new System.Drawing.Size(217, 20);
		this.tbDescription.TabIndex = 6;
		this.label1.AutoSize = true;
		this.label1.Location = new System.Drawing.Point(12, 18);
		this.label1.Name = "label1";
		this.label1.Size = new System.Drawing.Size(74, 13);
		this.label1.TabIndex = 5;
		this.label1.Text = "Commentaire :";
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.ClientSize = new System.Drawing.Size(323, 83);
		base.Controls.Add(this.tbDescription);
		base.Controls.Add(this.label1);
		base.Controls.Add(this.bAnnuler);
		base.Controls.Add(this.bEnregistrer);
		base.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Fixed3D;
		base.MaximizeBox = false;
		base.MinimizeBox = false;
		base.Name = "SaveDialog";
		base.ShowInTaskbar = false;
		base.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
		this.Text = "Enregistrer";
		base.ResumeLayout(false);
		base.PerformLayout();
	}
}
