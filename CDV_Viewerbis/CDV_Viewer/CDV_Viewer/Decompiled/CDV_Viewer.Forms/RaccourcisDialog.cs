using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using CDV_Viewer.Data;

namespace CDV_Viewer.Forms;

public class RaccourcisDialog : Form
{
	private IContainer components;

	private ListView lvRaccourcis;

	private Button bFermer;

	private ColumnHeader ch1;

	private ColumnHeader ch2;

	public RaccourcisDialog()
	{
		InitializeComponent();
		base.Load += RaccourcisForm_Load;
		bFermer.Click += bFermer_Click;
	}

	private void RaccourcisForm_Load(object sender, EventArgs e)
	{
		Dictionary<string, string> dictionary = new Dictionary<string, string>();
		dictionary.Add("<-", "Déplacement gauche");
		dictionary.Add("->", "Déplacement droit");
		dictionary.Add("-", "Zoom -");
		dictionary.Add("+", "Zoom -");
		dictionary.Add("X", "Zoom Minimum");
		dictionary.Add("I", "Active/Désactive l'affichage automatique des infobulle");
		dictionary.Add("L", "Affiche/Cache la légende");
		dictionary.Add("M", "Change de mode d'affichage");
		dictionary.Add("C", "Affiche/Cache le visualisateur de circuit de voie");
		dictionary.Add("T", "Affiche/Cache le visualisateur de tournée");
		if (Autorisations.Values.Edition)
		{
			dictionary.Add("Ctrl + O", "Ouvrir...");
			dictionary.Add("Ctrl + S", "Enregistrer...");
			dictionary.Add("Ctrl + F", "Fermer...");
			dictionary.Add("Ctrl + E", "Exporter...");
			dictionary.Add("Ctrl + V", "Vérifier la base...");
			dictionary.Add("Ctrl + P", "Ouvre la fenêtre de position des voies");
		}
		dictionary.Add("F1", "Ouvre la fenêtre d'aide");
		foreach (KeyValuePair<string, string> item in dictionary)
		{
			lvRaccourcis.Items.Add(new ListViewItem(new string[2] { item.Key, item.Value }));
		}
	}

	private void bFermer_Click(object sender, EventArgs e)
	{
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
		this.lvRaccourcis = new System.Windows.Forms.ListView();
		this.ch1 = new System.Windows.Forms.ColumnHeader();
		this.ch2 = new System.Windows.Forms.ColumnHeader();
		this.bFermer = new System.Windows.Forms.Button();
		base.SuspendLayout();
		this.lvRaccourcis.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.lvRaccourcis.Columns.AddRange(new System.Windows.Forms.ColumnHeader[2] { this.ch1, this.ch2 });
		this.lvRaccourcis.Location = new System.Drawing.Point(12, 12);
		this.lvRaccourcis.Name = "lvRaccourcis";
		this.lvRaccourcis.Size = new System.Drawing.Size(354, 287);
		this.lvRaccourcis.TabIndex = 0;
		this.lvRaccourcis.UseCompatibleStateImageBehavior = false;
		this.lvRaccourcis.View = System.Windows.Forms.View.Details;
		this.ch1.Text = "Commande";
		this.ch1.Width = 70;
		this.ch2.Text = "Description";
		this.ch2.Width = 280;
		this.bFermer.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
		this.bFermer.Location = new System.Drawing.Point(291, 305);
		this.bFermer.Name = "bFermer";
		this.bFermer.Size = new System.Drawing.Size(75, 23);
		this.bFermer.TabIndex = 1;
		this.bFermer.Text = "Fermer";
		this.bFermer.UseVisualStyleBackColor = true;
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.ClientSize = new System.Drawing.Size(378, 340);
		base.Controls.Add(this.bFermer);
		base.Controls.Add(this.lvRaccourcis);
		base.MaximizeBox = false;
		base.MinimizeBox = false;
		base.Name = "RaccourcisDialog";
		base.ShowIcon = false;
		base.ShowInTaskbar = false;
		base.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
		this.Text = "Liste des raccourcis";
		base.ResumeLayout(false);
	}
}
