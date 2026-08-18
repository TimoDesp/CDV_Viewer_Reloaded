using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using CDV_Viewer.CsvBase;
using CDV_Viewer.Data.SIGObjects;
using CDV_Viewer.Properties;

namespace CDV_Viewer.Forms;

public class ExportDialog : Form
{
	private List<int> _lignes = new List<int>();

	private bool _toutesLignes = true;

	private bool _archive;

	private bool _loc;

	private bool _mgv;

	private bool _ultrapot;

	private bool _timon;

	private IContainer components;

	private Label label1;

	private TextBox tbOutPath;

	private Button bParcourir;

	private ListView lvLignes;

	private Label label2;

	private Button bSelect;

	private Button bDeselect;

	private Panel panel1;

	private Button bExporter;

	private Button bFermer;

	private ColumnHeader chCheck;

	private ColumnHeader chNumLigne;

	private ColumnHeader chNomLigne;

	private CheckBox cbTimon;

	private CheckBox cbUltraPot;

	private CheckBox cbMGV;

	private CheckBox cbArchive;

	private CheckBox cbLoc;

	public List<int> Lignes => _lignes;

	public bool ToutesLignes => _toutesLignes;

	public bool Archive => _archive;

	public bool Loc => _loc;

	public bool MGV => _mgv;

	public bool UltraPot => _ultrapot;

	public bool Timon => _timon;

	public string OutPath => tbOutPath.Text;

	public ExportDialog()
	{
		InitializeComponent();
		base.Icon = Icon.FromHandle(Resources.Export.GetHicon());
		base.Load += SIGExportForm_Load;
		bParcourir.Click += bParcourir_Click;
		tbOutPath.Click += tbOutPath_Click;
		bSelect.Click += bSelect_Click;
		bDeselect.Click += bDeselect_Click;
		cbMGV.CheckedChanged += cbMGV_CheckedChanged;
		bFermer.Click += bFermer_Click;
		bExporter.Click += bExporter_Click;
	}

	private void SIGExportForm_Load(object sender, EventArgs e)
	{
		foreach (SIGLigne ligne in Base.GetLignes())
		{
			ListViewItem listViewItem = new ListViewItem(new string[3]
			{
				"",
				ligne.ID.ToString(),
				ligne.Nom
			});
			listViewItem.Checked = true;
			lvLignes.Items.Add(listViewItem);
		}
	}

	private void bParcourir_Click(object sender, EventArgs e)
	{
		FolderBrowserDialog folderBrowserDialog = new FolderBrowserDialog();
		folderBrowserDialog.Description = "Veuillez sélectionner le dossier dans lequel seront exportés les fichiers";
		if (Directory.Exists("D:\\Outils_SOL\\REF_MGV\\Data"))
		{
			folderBrowserDialog.SelectedPath = "D:\\Outils_SOL\\REF_MGV\\Data";
		}
		if (folderBrowserDialog.ShowDialog() == DialogResult.OK)
		{
			tbOutPath.Text = folderBrowserDialog.SelectedPath;
		}
	}

	private void tbOutPath_Click(object sender, EventArgs e)
	{
		bParcourir.PerformClick();
	}

	private void bSelect_Click(object sender, EventArgs e)
	{
		foreach (ListViewItem item in lvLignes.Items)
		{
			item.Checked = true;
		}
	}

	private void bDeselect_Click(object sender, EventArgs e)
	{
		foreach (ListViewItem item in lvLignes.Items)
		{
			item.Checked = false;
		}
	}

	private void cbMGV_CheckedChanged(object sender, EventArgs e)
	{
		cbTimon.Enabled = cbMGV.Checked;
		if (!cbTimon.Enabled)
		{
			cbTimon.Checked = false;
		}
	}

	private void bFermer_Click(object sender, EventArgs e)
	{
		base.DialogResult = DialogResult.Cancel;
		Close();
	}

	private void bExporter_Click(object sender, EventArgs e)
	{
		if (Directory.Exists(tbOutPath.Text))
		{
			foreach (ListViewItem item in lvLignes.Items)
			{
				if (item.Checked)
				{
					_lignes.Add(int.Parse(item.SubItems[1].Text));
				}
			}
			if (_lignes.Count < lvLignes.Items.Count)
			{
				_toutesLignes = false;
			}
			_archive = cbArchive.Checked;
			_loc = cbLoc.Checked;
			_mgv = cbMGV.Checked;
			_ultrapot = cbUltraPot.Checked;
			_timon = cbTimon.Checked;
			base.DialogResult = DialogResult.OK;
			Close();
		}
		else
		{
			MessageBox.Show("Dossier de sortie incorrect", Resources.APP_NAME);
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
		this.label1 = new System.Windows.Forms.Label();
		this.tbOutPath = new System.Windows.Forms.TextBox();
		this.bParcourir = new System.Windows.Forms.Button();
		this.lvLignes = new System.Windows.Forms.ListView();
		this.chCheck = new System.Windows.Forms.ColumnHeader();
		this.chNumLigne = new System.Windows.Forms.ColumnHeader();
		this.chNomLigne = new System.Windows.Forms.ColumnHeader();
		this.label2 = new System.Windows.Forms.Label();
		this.bSelect = new System.Windows.Forms.Button();
		this.bDeselect = new System.Windows.Forms.Button();
		this.panel1 = new System.Windows.Forms.Panel();
		this.cbLoc = new System.Windows.Forms.CheckBox();
		this.cbArchive = new System.Windows.Forms.CheckBox();
		this.cbTimon = new System.Windows.Forms.CheckBox();
		this.cbUltraPot = new System.Windows.Forms.CheckBox();
		this.cbMGV = new System.Windows.Forms.CheckBox();
		this.bExporter = new System.Windows.Forms.Button();
		this.bFermer = new System.Windows.Forms.Button();
		this.panel1.SuspendLayout();
		base.SuspendLayout();
		this.label1.AutoSize = true;
		this.label1.Location = new System.Drawing.Point(14, 16);
		this.label1.Name = "label1";
		this.label1.Size = new System.Drawing.Size(91, 13);
		this.label1.TabIndex = 0;
		this.label1.Text = "Dossier de sortie :";
		this.tbOutPath.Location = new System.Drawing.Point(111, 13);
		this.tbOutPath.Name = "tbOutPath";
		this.tbOutPath.Size = new System.Drawing.Size(200, 20);
		this.tbOutPath.TabIndex = 1;
		this.bParcourir.Location = new System.Drawing.Point(317, 11);
		this.bParcourir.Name = "bParcourir";
		this.bParcourir.Size = new System.Drawing.Size(75, 23);
		this.bParcourir.TabIndex = 2;
		this.bParcourir.Text = "Parcourir...";
		this.bParcourir.UseVisualStyleBackColor = true;
		this.lvLignes.CheckBoxes = true;
		this.lvLignes.Columns.AddRange(new System.Windows.Forms.ColumnHeader[3] { this.chCheck, this.chNumLigne, this.chNomLigne });
		this.lvLignes.Location = new System.Drawing.Point(14, 64);
		this.lvLignes.Name = "lvLignes";
		this.lvLignes.Size = new System.Drawing.Size(378, 163);
		this.lvLignes.TabIndex = 3;
		this.lvLignes.UseCompatibleStateImageBehavior = false;
		this.lvLignes.View = System.Windows.Forms.View.Details;
		this.chCheck.Text = "";
		this.chCheck.Width = 20;
		this.chNumLigne.Text = "Numéro";
		this.chNomLigne.Text = "Nom";
		this.chNomLigne.Width = 200;
		this.label2.AutoSize = true;
		this.label2.Location = new System.Drawing.Point(14, 48);
		this.label2.Name = "label2";
		this.label2.Size = new System.Drawing.Size(94, 13);
		this.label2.TabIndex = 4;
		this.label2.Text = "Lignes à exporter :";
		this.bSelect.Location = new System.Drawing.Point(165, 233);
		this.bSelect.Name = "bSelect";
		this.bSelect.Size = new System.Drawing.Size(100, 23);
		this.bSelect.TabIndex = 5;
		this.bSelect.Text = "Tout sélectionner";
		this.bSelect.UseVisualStyleBackColor = true;
		this.bDeselect.Location = new System.Drawing.Point(271, 233);
		this.bDeselect.Name = "bDeselect";
		this.bDeselect.Size = new System.Drawing.Size(120, 23);
		this.bDeselect.TabIndex = 6;
		this.bDeselect.Text = "Tout Désélectionner";
		this.bDeselect.UseVisualStyleBackColor = true;
		this.panel1.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
		this.panel1.Controls.Add(this.cbLoc);
		this.panel1.Controls.Add(this.cbArchive);
		this.panel1.Controls.Add(this.cbTimon);
		this.panel1.Controls.Add(this.cbUltraPot);
		this.panel1.Controls.Add(this.cbMGV);
		this.panel1.Controls.Add(this.lvLignes);
		this.panel1.Controls.Add(this.bDeselect);
		this.panel1.Controls.Add(this.label1);
		this.panel1.Controls.Add(this.bSelect);
		this.panel1.Controls.Add(this.tbOutPath);
		this.panel1.Controls.Add(this.label2);
		this.panel1.Controls.Add(this.bParcourir);
		this.panel1.Location = new System.Drawing.Point(12, 12);
		this.panel1.Name = "panel1";
		this.panel1.Size = new System.Drawing.Size(406, 396);
		this.panel1.TabIndex = 7;
		this.cbLoc.AutoSize = true;
		this.cbLoc.Checked = true;
		this.cbLoc.CheckState = System.Windows.Forms.CheckState.Checked;
		this.cbLoc.Location = new System.Drawing.Point(14, 360);
		this.cbLoc.Name = "cbLoc";
		this.cbLoc.Size = new System.Drawing.Size(66, 17);
		this.cbLoc.TabIndex = 11;
		this.cbLoc.Text = "LOC NG";
		this.cbLoc.UseVisualStyleBackColor = true;
		this.cbArchive.AutoSize = true;
		this.cbArchive.Checked = true;
		this.cbArchive.CheckState = System.Windows.Forms.CheckState.Checked;
		this.cbArchive.Location = new System.Drawing.Point(14, 265);
		this.cbArchive.Name = "cbArchive";
		this.cbArchive.Size = new System.Drawing.Size(122, 17);
		this.cbArchive.TabIndex = 10;
		this.cbArchive.Text = "Archive CDV Viewer";
		this.cbArchive.UseVisualStyleBackColor = true;
		this.cbTimon.AutoSize = true;
		this.cbTimon.Checked = true;
		this.cbTimon.CheckState = System.Windows.Forms.CheckState.Checked;
		this.cbTimon.Location = new System.Drawing.Point(14, 337);
		this.cbTimon.Name = "cbTimon";
		this.cbTimon.Size = new System.Drawing.Size(61, 17);
		this.cbTimon.TabIndex = 9;
		this.cbTimon.Text = "TIMON";
		this.cbTimon.UseVisualStyleBackColor = true;
		this.cbUltraPot.AutoSize = true;
		this.cbUltraPot.Checked = true;
		this.cbUltraPot.CheckState = System.Windows.Forms.CheckState.Checked;
		this.cbUltraPot.Location = new System.Drawing.Point(14, 313);
		this.cbUltraPot.Name = "cbUltraPot";
		this.cbUltraPot.Size = new System.Drawing.Size(84, 17);
		this.cbUltraPot.TabIndex = 8;
		this.cbUltraPot.Text = "LISTE_CDV";
		this.cbUltraPot.UseVisualStyleBackColor = true;
		this.cbMGV.AutoSize = true;
		this.cbMGV.Checked = true;
		this.cbMGV.CheckState = System.Windows.Forms.CheckState.Checked;
		this.cbMGV.Location = new System.Drawing.Point(14, 290);
		this.cbMGV.Name = "cbMGV";
		this.cbMGV.Size = new System.Drawing.Size(77, 17);
		this.cbMGV.TabIndex = 7;
		this.cbMGV.Text = "REF_MGV";
		this.cbMGV.UseVisualStyleBackColor = true;
		this.bExporter.Location = new System.Drawing.Point(262, 414);
		this.bExporter.Name = "bExporter";
		this.bExporter.Size = new System.Drawing.Size(75, 23);
		this.bExporter.TabIndex = 7;
		this.bExporter.Text = "Exporter";
		this.bExporter.UseVisualStyleBackColor = true;
		this.bFermer.Location = new System.Drawing.Point(343, 414);
		this.bFermer.Name = "bFermer";
		this.bFermer.Size = new System.Drawing.Size(75, 23);
		this.bFermer.TabIndex = 8;
		this.bFermer.Text = "Fermer";
		this.bFermer.UseVisualStyleBackColor = true;
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.ClientSize = new System.Drawing.Size(427, 449);
		base.Controls.Add(this.bFermer);
		base.Controls.Add(this.bExporter);
		base.Controls.Add(this.panel1);
		base.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
		base.MaximizeBox = false;
		base.MinimizeBox = false;
		base.Name = "ExportDialog";
		base.ShowInTaskbar = false;
		base.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
		this.Text = "Exporter";
		this.panel1.ResumeLayout(false);
		this.panel1.PerformLayout();
		base.ResumeLayout(false);
	}
}
