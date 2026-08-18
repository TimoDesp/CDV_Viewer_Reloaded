using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using CDV_Viewer.Properties;

namespace CDV_Viewer.Forms;

public class OpenTourneeDialog : Form
{
	private IContainer components;

	private DataGridView dgvTournees;

	private Button bOuvrir;

	private Button bFermer;

	private DataGridViewImageColumn cChecked;

	private DataGridViewTextBoxColumn cDate;

	public string SelectedTournee { get; private set; }

	public bool Erreur { get; private set; }

	public OpenTourneeDialog(string xmlFile)
	{
		InitializeComponent();
		SelectedTournee = string.Empty;
		base.Icon = Icon.FromHandle(Resources.open.GetHicon());
		cChecked.DefaultCellStyle.NullValue = null;
		LoadTournees(xmlFile);
		if (dgvTournees.Rows.Count == 0)
		{
			Erreur = true;
		}
		dgvTournees.CellClick += dgvTournees_CellClick;
		dgvTournees.CellDoubleClick += dgvTournees_CellDoubleClick;
		bFermer.Click += bFermer_Click;
		bOuvrir.Click += bOuvrir_Click;
	}

	private void LoadTournees(string xmlFile)
	{
		try
		{
			string path = Path.GetDirectoryName(Path.GetDirectoryName(xmlFile)) + "\\data_out";
			_ = string.Empty;
			string[] directories = Directory.GetDirectories(path);
			for (int i = 0; i < directories.Length; i++)
			{
				string fileName = Path.GetFileName(directories[i]);
				fileName = fileName.Substring(fileName.IndexOf("-") + 1);
				DataGridViewRow dataGridViewRow = new DataGridViewRow();
				dataGridViewRow.Cells.Add(new DataGridViewImageCell());
				dataGridViewRow.Cells.Add(new DataGridViewTextBoxCell());
				dataGridViewRow.Cells[1].Value = fileName;
				dgvTournees.Rows.Add(dataGridViewRow);
			}
		}
		catch
		{
		}
	}

	private void dgvTournees_CellClick(object sender, DataGridViewCellEventArgs e)
	{
		if (e.ColumnIndex != 0)
		{
			return;
		}
		foreach (DataGridViewRow item in (IEnumerable)dgvTournees.Rows)
		{
			if (item.Index == e.RowIndex)
			{
				((DataGridViewImageCell)item.Cells[0]).Value = Resources.Vrai;
			}
			else
			{
				((DataGridViewImageCell)item.Cells[0]).Value = null;
			}
		}
	}

	private void dgvTournees_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
	{
		SelectedTournee = (string)dgvTournees.Rows[e.RowIndex].Cells[1].Value;
		if (SelectedTournee != string.Empty)
		{
			base.DialogResult = DialogResult.OK;
			Close();
		}
	}

	private void bFermer_Click(object sender, EventArgs e)
	{
		base.DialogResult = DialogResult.Cancel;
		Close();
	}

	private void bOuvrir_Click(object sender, EventArgs e)
	{
		foreach (DataGridViewRow item in (IEnumerable)dgvTournees.Rows)
		{
			if (item.Cells[0].Value != null)
			{
				SelectedTournee = (string)item.Cells[1].Value;
			}
		}
		if (SelectedTournee == string.Empty)
		{
			MessageBox.Show("Veuillez sélectionner une tournée", Resources.APP_NAME);
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
		this.dgvTournees = new System.Windows.Forms.DataGridView();
		this.cChecked = new System.Windows.Forms.DataGridViewImageColumn();
		this.cDate = new System.Windows.Forms.DataGridViewTextBoxColumn();
		this.bOuvrir = new System.Windows.Forms.Button();
		this.bFermer = new System.Windows.Forms.Button();
		((System.ComponentModel.ISupportInitialize)this.dgvTournees).BeginInit();
		base.SuspendLayout();
		this.dgvTournees.AllowUserToAddRows = false;
		this.dgvTournees.AllowUserToDeleteRows = false;
		this.dgvTournees.AllowUserToResizeRows = false;
		this.dgvTournees.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.dgvTournees.BackgroundColor = System.Drawing.Color.White;
		this.dgvTournees.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
		this.dgvTournees.Columns.AddRange(this.cChecked, this.cDate);
		this.dgvTournees.Location = new System.Drawing.Point(12, 11);
		this.dgvTournees.Name = "dgvTournees";
		this.dgvTournees.RowHeadersVisible = false;
		this.dgvTournees.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
		this.dgvTournees.Size = new System.Drawing.Size(279, 216);
		this.dgvTournees.TabIndex = 3;
		this.cChecked.HeaderText = "";
		this.cChecked.Name = "cChecked";
		this.cChecked.ReadOnly = true;
		this.cChecked.Resizable = System.Windows.Forms.DataGridViewTriState.True;
		this.cChecked.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Automatic;
		this.cChecked.Width = 25;
		this.cDate.HeaderText = "Tournee";
		this.cDate.Name = "cDate";
		this.cDate.ReadOnly = true;
		this.cDate.Width = 250;
		this.bOuvrir.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
		this.bOuvrir.Location = new System.Drawing.Point(135, 233);
		this.bOuvrir.Name = "bOuvrir";
		this.bOuvrir.Size = new System.Drawing.Size(75, 23);
		this.bOuvrir.TabIndex = 5;
		this.bOuvrir.Text = "Ouvrir";
		this.bOuvrir.UseVisualStyleBackColor = true;
		this.bFermer.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
		this.bFermer.Location = new System.Drawing.Point(216, 233);
		this.bFermer.Name = "bFermer";
		this.bFermer.Size = new System.Drawing.Size(75, 23);
		this.bFermer.TabIndex = 4;
		this.bFermer.Text = "Fermer";
		this.bFermer.UseVisualStyleBackColor = true;
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.ClientSize = new System.Drawing.Size(304, 267);
		base.Controls.Add(this.dgvTournees);
		base.Controls.Add(this.bOuvrir);
		base.Controls.Add(this.bFermer);
		base.Name = "OpenTourneeDialog";
		base.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
		this.Text = "Ouvrir une tournée";
		((System.ComponentModel.ISupportInitialize)this.dgvTournees).EndInit();
		base.ResumeLayout(false);
	}
}
