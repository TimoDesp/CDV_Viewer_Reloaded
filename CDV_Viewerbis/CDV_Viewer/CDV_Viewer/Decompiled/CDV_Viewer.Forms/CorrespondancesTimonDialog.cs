using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using CDV_Viewer.CsvBase;
using CDV_Viewer.Data.SIGObjects;

namespace CDV_Viewer.Forms;

public class CorrespondancesTimonDialog : Form
{
	private IContainer components;

	private Label label1;

	private ComboBox cbLigne;

	private CheckBox cbNonRenseigne;

	private DataGridView dgvTimon;

	private Panel panel1;

	private Button bValider;

	private Button bFermer;

	private DataGridViewTextBoxColumn cID;

	private DataGridViewTextBoxColumn cLigne;

	private DataGridViewTextBoxColumn cVoie;

	private DataGridViewTextBoxColumn cPKD;

	private DataGridViewTextBoxColumn cPKF;

	private DataGridViewTextBoxColumn cTimon;

	public static void Open()
	{
		new CorrespondancesTimonDialog().ShowDialog();
	}

	public CorrespondancesTimonDialog()
	{
		InitializeComponent();
		base.Load += SIGCorresTimon_Load;
		cbLigne.SelectedIndexChanged += cbLigne_SelectedIndexChanged;
		cbNonRenseigne.CheckedChanged += cbNonRenseigne_CheckedChanged;
		dgvTimon.CellValueChanged += dgvTimon_CellValueChanged;
		bFermer.Click += bFermer_Click;
		bValider.Click += bValider_Click;
	}

	private void LoadDataGrid()
	{
		dgvTimon.Rows.Clear();
		Dictionary<int, int> idsTimon = Base.GetIdsTimon();
		List<SIGLigne> list = new List<SIGLigne>();
		if (cbLigne.SelectedIndex == 0)
		{
			list = Base.GetLignes();
		}
		else
		{
			SIGLigne ligne = Base.GetLigne((int)cbLigne.SelectedItem);
			if (ligne == null)
			{
				return;
			}
			list.Add(ligne);
		}
		Base.GetBase(list, out var voies, out var _, out var _, out var _, out var _);
		foreach (SIGVoie item in voies)
		{
			int value;
			bool flag = idsTimon.TryGetValue(item.ID, out value);
			if (!(cbNonRenseigne.Checked && flag) || value <= 0)
			{
				DataGridViewRow dataGridViewRow = new DataGridViewRow();
				dataGridViewRow.Cells.Add(new DataGridViewTextBoxCell());
				dataGridViewRow.Cells.Add(new DataGridViewTextBoxCell());
				dataGridViewRow.Cells.Add(new DataGridViewTextBoxCell());
				dataGridViewRow.Cells.Add(new DataGridViewTextBoxCell());
				dataGridViewRow.Cells.Add(new DataGridViewTextBoxCell());
				dataGridViewRow.Cells.Add(new DataGridViewTextBoxCell());
				dataGridViewRow.Cells[0].Value = item.ID;
				dataGridViewRow.Cells[1].Value = item.Ligne.ID;
				dataGridViewRow.Cells[2].Value = item.Nom;
				dataGridViewRow.Cells[3].Value = item.PKDebut;
				dataGridViewRow.Cells[4].Value = item.PKFin;
				if (flag && value > 0)
				{
					dataGridViewRow.Cells[5].Value = value;
				}
				else
				{
					dataGridViewRow.Cells[5].Style.BackColor = Color.Red;
				}
				dgvTimon.Rows.Add(dataGridViewRow);
			}
		}
	}

	private void SIGCorresTimon_Load(object sender, EventArgs e)
	{
		cbLigne.Items.Add("Toutes");
		foreach (SIGLigne ligne in Base.GetLignes())
		{
			cbLigne.Items.Add(ligne.ID);
		}
		cbLigne.SelectedItem = 0;
	}

	private void cbLigne_SelectedIndexChanged(object sender, EventArgs e)
	{
		LoadDataGrid();
	}

	private void cbNonRenseigne_CheckedChanged(object sender, EventArgs e)
	{
		LoadDataGrid();
	}

	private void dgvTimon_CellValueChanged(object sender, DataGridViewCellEventArgs e)
	{
		if (int.TryParse((string)dgvTimon.Rows[e.RowIndex].Cells[5].Value, out var result))
		{
			Base.SetIDTimon((int)dgvTimon.Rows[e.RowIndex].Cells[0].Value, result);
			dgvTimon.Rows[e.RowIndex].Cells[5].Style.BackColor = Color.White;
		}
	}

	private void bFermer_Click(object sender, EventArgs e)
	{
		base.DialogResult = DialogResult.Cancel;
		Close();
	}

	private void bValider_Click(object sender, EventArgs e)
	{
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
		this.label1 = new System.Windows.Forms.Label();
		this.cbLigne = new System.Windows.Forms.ComboBox();
		this.cbNonRenseigne = new System.Windows.Forms.CheckBox();
		this.dgvTimon = new System.Windows.Forms.DataGridView();
		this.cID = new System.Windows.Forms.DataGridViewTextBoxColumn();
		this.cLigne = new System.Windows.Forms.DataGridViewTextBoxColumn();
		this.cVoie = new System.Windows.Forms.DataGridViewTextBoxColumn();
		this.cPKD = new System.Windows.Forms.DataGridViewTextBoxColumn();
		this.cPKF = new System.Windows.Forms.DataGridViewTextBoxColumn();
		this.cTimon = new System.Windows.Forms.DataGridViewTextBoxColumn();
		this.panel1 = new System.Windows.Forms.Panel();
		this.bValider = new System.Windows.Forms.Button();
		this.bFermer = new System.Windows.Forms.Button();
		((System.ComponentModel.ISupportInitialize)this.dgvTimon).BeginInit();
		this.panel1.SuspendLayout();
		base.SuspendLayout();
		this.label1.AutoSize = true;
		this.label1.Location = new System.Drawing.Point(11, 13);
		this.label1.Name = "label1";
		this.label1.Size = new System.Drawing.Size(42, 13);
		this.label1.TabIndex = 0;
		this.label1.Text = "Ligne : ";
		this.cbLigne.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
		this.cbLigne.FormattingEnabled = true;
		this.cbLigne.Location = new System.Drawing.Point(59, 10);
		this.cbLigne.Name = "cbLigne";
		this.cbLigne.Size = new System.Drawing.Size(89, 21);
		this.cbLigne.TabIndex = 1;
		this.cbNonRenseigne.AutoSize = true;
		this.cbNonRenseigne.Checked = true;
		this.cbNonRenseigne.CheckState = System.Windows.Forms.CheckState.Checked;
		this.cbNonRenseigne.Location = new System.Drawing.Point(14, 37);
		this.cbNonRenseigne.Name = "cbNonRenseigne";
		this.cbNonRenseigne.Size = new System.Drawing.Size(153, 17);
		this.cbNonRenseigne.TabIndex = 2;
		this.cbNonRenseigne.Text = "Non renseigné uniquement";
		this.cbNonRenseigne.UseVisualStyleBackColor = true;
		this.dgvTimon.AllowUserToAddRows = false;
		this.dgvTimon.AllowUserToDeleteRows = false;
		this.dgvTimon.AllowUserToResizeRows = false;
		this.dgvTimon.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.dgvTimon.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
		this.dgvTimon.Columns.AddRange(this.cID, this.cLigne, this.cVoie, this.cPKD, this.cPKF, this.cTimon);
		this.dgvTimon.Location = new System.Drawing.Point(14, 60);
		this.dgvTimon.Name = "dgvTimon";
		this.dgvTimon.RowHeadersVisible = false;
		this.dgvTimon.Size = new System.Drawing.Size(377, 230);
		this.dgvTimon.TabIndex = 3;
		this.cID.HeaderText = "ID";
		this.cID.Name = "cID";
		this.cID.ReadOnly = true;
		this.cID.Width = 40;
		this.cLigne.HeaderText = "Ligne";
		this.cLigne.Name = "cLigne";
		this.cLigne.ReadOnly = true;
		this.cLigne.Width = 60;
		this.cVoie.HeaderText = "Voie";
		this.cVoie.Name = "cVoie";
		this.cVoie.ReadOnly = true;
		this.cVoie.Width = 60;
		this.cPKD.HeaderText = "PK Début";
		this.cPKD.Name = "cPKD";
		this.cPKD.ReadOnly = true;
		this.cPKD.Width = 60;
		this.cPKF.HeaderText = "PK Fin";
		this.cPKF.Name = "cPKF";
		this.cPKF.ReadOnly = true;
		this.cPKF.Width = 70;
		this.cTimon.HeaderText = "ID Timon";
		this.cTimon.Name = "cTimon";
		this.cTimon.Width = 60;
		this.panel1.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.panel1.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
		this.panel1.Controls.Add(this.dgvTimon);
		this.panel1.Controls.Add(this.cbLigne);
		this.panel1.Controls.Add(this.cbNonRenseigne);
		this.panel1.Controls.Add(this.label1);
		this.panel1.Location = new System.Drawing.Point(12, 12);
		this.panel1.Name = "panel1";
		this.panel1.Size = new System.Drawing.Size(404, 302);
		this.panel1.TabIndex = 4;
		this.bValider.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
		this.bValider.Location = new System.Drawing.Point(260, 320);
		this.bValider.Name = "bValider";
		this.bValider.Size = new System.Drawing.Size(75, 23);
		this.bValider.TabIndex = 5;
		this.bValider.Text = "Valider";
		this.bValider.UseVisualStyleBackColor = true;
		this.bFermer.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
		this.bFermer.Location = new System.Drawing.Point(341, 320);
		this.bFermer.Name = "bFermer";
		this.bFermer.Size = new System.Drawing.Size(75, 23);
		this.bFermer.TabIndex = 6;
		this.bFermer.Text = "Fermer";
		this.bFermer.UseVisualStyleBackColor = true;
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.ClientSize = new System.Drawing.Size(424, 352);
		base.Controls.Add(this.bFermer);
		base.Controls.Add(this.bValider);
		base.Controls.Add(this.panel1);
		base.Name = "CorrespondancesTimonDialog";
		base.ShowIcon = false;
		base.ShowInTaskbar = false;
		base.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
		this.Text = "Correspondance ID TIMON";
		((System.ComponentModel.ISupportInitialize)this.dgvTimon).EndInit();
		this.panel1.ResumeLayout(false);
		this.panel1.PerformLayout();
		base.ResumeLayout(false);
	}
}
