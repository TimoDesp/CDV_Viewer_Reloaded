using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using CDV_Viewer.Data;
using CDV_Viewer.Properties;

namespace CDV_Viewer.Forms;

public class OpenArchiveDialog : Form
{
	public Archive SelectedArchive;

	private IContainer components;

	private DataGridView dgvArchives;

	private Button bFermer;

	private Button bOuvrir;

	private DataGridViewImageColumn cChecked;

	private DataGridViewTextBoxColumn cDate;

	private DataGridViewTextBoxColumn cDescription;

	public OpenArchiveDialog()
	{
		InitializeComponent();
		base.Icon = Icon.FromHandle(Resources.open.GetHicon());
		cChecked.DefaultCellStyle.NullValue = null;
		base.Load += ArchivesForm_Load;
		dgvArchives.Resize += dgvArchives_Resize;
		dgvArchives.CellClick += dgvArchives_CellClick;
		dgvArchives.CellDoubleClick += dgvArchives_CellDoubleClick;
		bFermer.Click += bFermer_Click;
		bOuvrir.Click += bOuvrir_Click;
	}

	private void ArchivesForm_Load(object sender, EventArgs e)
	{
		foreach (Archive item in Archives.GetHistorique())
		{
			DataGridViewRow dataGridViewRow = new DataGridViewRow();
			dataGridViewRow.Cells.Add(new DataGridViewImageCell());
			dataGridViewRow.Cells.Add(new DataGridViewTextBoxCell());
			dataGridViewRow.Cells[1].Value = item.Date;
			dataGridViewRow.Cells.Add(new DataGridViewTextBoxCell());
			dataGridViewRow.Cells[2].Value = item.Description;
			dgvArchives.Rows.Add(dataGridViewRow);
		}
		dgvArchives.Sort(cDate, ListSortDirection.Descending);
	}

	private void dgvArchives_Resize(object sender, EventArgs e)
	{
		cDescription.Width = dgvArchives.Width - cChecked.Width - cDate.Width - 3;
	}

	private void dgvArchives_CellClick(object sender, DataGridViewCellEventArgs e)
	{
		foreach (DataGridViewRow item in (IEnumerable)dgvArchives.Rows)
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

	private void dgvArchives_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
	{
		Archive archive = Archives.GetArchive((DateTime)dgvArchives.Rows[e.RowIndex].Cells[1].Value);
		if (archive != null)
		{
			SelectedArchive = archive;
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
		foreach (DataGridViewRow item in (IEnumerable)dgvArchives.Rows)
		{
			if (item.Cells[0].Value != null)
			{
				Archive archive = Archives.GetArchive((DateTime)item.Cells[1].Value);
				if (archive != null)
				{
					SelectedArchive = archive;
				}
			}
		}
		if (SelectedArchive == null)
		{
			MessageBox.Show("Veuillez sélectionner une archive", Resources.APP_NAME);
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
		this.dgvArchives = new System.Windows.Forms.DataGridView();
		this.cChecked = new System.Windows.Forms.DataGridViewImageColumn();
		this.cDate = new System.Windows.Forms.DataGridViewTextBoxColumn();
		this.cDescription = new System.Windows.Forms.DataGridViewTextBoxColumn();
		this.bFermer = new System.Windows.Forms.Button();
		this.bOuvrir = new System.Windows.Forms.Button();
		((System.ComponentModel.ISupportInitialize)this.dgvArchives).BeginInit();
		base.SuspendLayout();
		this.dgvArchives.AllowUserToAddRows = false;
		this.dgvArchives.AllowUserToDeleteRows = false;
		this.dgvArchives.AllowUserToResizeRows = false;
		this.dgvArchives.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.dgvArchives.BackgroundColor = System.Drawing.Color.White;
		this.dgvArchives.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
		this.dgvArchives.Columns.AddRange(this.cChecked, this.cDate, this.cDescription);
		this.dgvArchives.Location = new System.Drawing.Point(13, 13);
		this.dgvArchives.Name = "dgvArchives";
		this.dgvArchives.RowHeadersVisible = false;
		this.dgvArchives.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
		this.dgvArchives.Size = new System.Drawing.Size(468, 283);
		this.dgvArchives.TabIndex = 0;
		this.cChecked.HeaderText = "";
		this.cChecked.Name = "cChecked";
		this.cChecked.ReadOnly = true;
		this.cChecked.Resizable = System.Windows.Forms.DataGridViewTriState.True;
		this.cChecked.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Automatic;
		this.cChecked.Width = 25;
		this.cDate.HeaderText = "Date";
		this.cDate.Name = "cDate";
		this.cDate.ReadOnly = true;
		this.cDate.Width = 120;
		this.cDescription.HeaderText = "Commentaire";
		this.cDescription.Name = "cDescription";
		this.cDescription.ReadOnly = true;
		this.cDescription.Width = 320;
		this.bFermer.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
		this.bFermer.Location = new System.Drawing.Point(406, 302);
		this.bFermer.Name = "bFermer";
		this.bFermer.Size = new System.Drawing.Size(75, 23);
		this.bFermer.TabIndex = 1;
		this.bFermer.Text = "Fermer";
		this.bFermer.UseVisualStyleBackColor = true;
		this.bOuvrir.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
		this.bOuvrir.Location = new System.Drawing.Point(325, 302);
		this.bOuvrir.Name = "bOuvrir";
		this.bOuvrir.Size = new System.Drawing.Size(75, 23);
		this.bOuvrir.TabIndex = 2;
		this.bOuvrir.Text = "Ouvrir";
		this.bOuvrir.UseVisualStyleBackColor = true;
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.ClientSize = new System.Drawing.Size(493, 334);
		base.Controls.Add(this.bOuvrir);
		base.Controls.Add(this.bFermer);
		base.Controls.Add(this.dgvArchives);
		base.MaximizeBox = false;
		base.MinimizeBox = false;
		this.MinimumSize = new System.Drawing.Size(300, 200);
		base.Name = "OpenArchiveDialog";
		base.ShowInTaskbar = false;
		base.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
		this.Text = "Ouvrir une archive";
		((System.ComponentModel.ISupportInitialize)this.dgvArchives).EndInit();
		base.ResumeLayout(false);
	}
}
