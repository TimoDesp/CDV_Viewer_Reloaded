using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;
using CDV_Viewer.CsvBase;
using CDV_Viewer.Data;
using CDV_Viewer.Data.SIGObjects;
using CDV_Viewer.DockControls;
using CDV_Viewer.Properties;

namespace CDV_Viewer.Forms;

public class PositionVoiesDialog : Form
{
	private List<SIGVoie> _voies;

	private Dictionary<int, int> _voiePosition = new Dictionary<int, int>();

	private Image _imgPositions;

	private const int NB_VOIES_MAX = 20;

	private Color[] _colors = new Color[6]
	{
		Color.FromArgb(224, 82, 0),
		Color.FromArgb(255, 182, 18),
		Color.FromArgb(204, 202, 0),
		Color.FromArgb(122, 184, 0),
		Color.FromArgb(0, 154, 166),
		Color.FromArgb(0, 136, 206)
	};

	private IContainer components;

	private Button bValider;

	private Button bFermer;

	private PictureBox pbPositions;

	private DataGridView dgVoies;

	private Label label2;

	private System.Windows.Forms.Timer timer;

	private DataGridViewTextBoxColumn voie;

	private DataGridViewTextBoxColumn position;

	private DataGridViewTextBoxColumn couleur;

	public static void Open()
	{
		new PositionVoiesDialog().ShowDialog();
	}

	public PositionVoiesDialog()
	{
		InitializeComponent();
		base.Icon = Icon.FromHandle(Resources.PosVoies.GetHicon());
		dgVoies.Columns[1].ValueType = typeof(int);
		base.Resize += SIGPositionVoies_Resize;
		dgVoies.CellEndEdit += dgVoies_CellEndEdit;
		dgVoies.CellDoubleClick += dgVoies_CellDoubleClick;
		timer.Tick += timer_Tick;
		bFermer.Click += bFermer_Click;
		bValider.Click += bValider_Click;
	}

	private void SIGPositionVoies_Resize(object sender, EventArgs e)
	{
		new Thread(Dessiner).Start();
	}

	private void dgVoies_CellEndEdit(object sender, DataGridViewCellEventArgs e)
	{
		if (!(dgVoies.Rows[e.RowIndex].Cells[1].Value is int))
		{
			dgVoies.Rows[e.RowIndex].Cells[1].Value = 0;
		}
		else if (dgVoies.Rows[e.RowIndex].Tag == null)
		{
			foreach (DataGridViewRow item in (IEnumerable)dgVoies.Rows)
			{
				if (GetNomVoie(item) == (string)dgVoies.Rows[e.RowIndex].Cells[0].Value)
				{
					item.Cells[e.ColumnIndex].Value = dgVoies.Rows[e.RowIndex].Cells[1].Value;
				}
			}
		}
		ActualiserListe();
	}

	private void dgVoies_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
	{
		if (e.ColumnIndex != 0)
		{
			return;
		}
		string text = (string)dgVoies.Rows[e.RowIndex].Cells[0].Value;
		bool visible = !IsOpen(text);
		foreach (DataGridViewRow item in (IEnumerable)dgVoies.Rows)
		{
			if (GetNomVoie(item) == text)
			{
				item.Visible = visible;
			}
		}
	}

	private void timer_Tick(object sender, EventArgs e)
	{
		pbPositions.Image = _imgPositions;
	}

	private void bFermer_Click(object sender, EventArgs e)
	{
		base.DialogResult = DialogResult.Cancel;
		Close();
	}

	private void bValider_Click(object sender, EventArgs e)
	{
		foreach (DataGridViewRow _row in (IEnumerable)dgVoies.Rows)
		{
			if (_row.Tag != null && _row.Tag is int && _row.Cells[1].Value is int)
			{
				SIGVoie sIGVoie = _voies.Find((SIGVoie voie) => voie.ID == (int)_row.Tag);
				if (sIGVoie != null)
				{
					sIGVoie.PositionY = (int)_row.Cells[1].Value;
					Base.SetPositionVoie(sIGVoie);
				}
			}
		}
		MessageBox.Show("Positions mises à jour !", Resources.APP_NAME);
		base.DialogResult = DialogResult.OK;
		Close();
	}

	public new DialogResult ShowDialog()
	{
		if (!Global.ModeEdition || ComposantsViewer.Viewer.Ligne == null)
		{
			return DialogResult.Cancel;
		}
		_voies = Base.GetLigne(ComposantsViewer.Viewer.Ligne.ID).VoiesWithoutJonctions;
		_voies.Sort(delegate(SIGVoie v1, SIGVoie v2)
		{
			int num3 = v1.Nom.CompareTo(v2.Nom);
			return (num3 != 0) ? num3 : v1.PKDebut.CompareTo(v2.PKDebut);
		});
		int num = 0;
		foreach (SIGVoie voie in _voies)
		{
			int positionY = voie.PositionY;
			if (!ContainsVoie(voie.Nom))
			{
				num++;
				AddRow(voie.Nom, positionY, _colors[num % _colors.Length]);
			}
			AddRow(voie.ID, "  PK " + voie.PKDebut + " - " + voie.PKFin, positionY, _colors[num % _colors.Length]);
		}
		ActualiserListe();
		timer.Start();
		DialogResult num2 = base.ShowDialog();
		if (num2 == DialogResult.OK)
		{
			ComposantsViewer.Viewer.RefreshLigne();
		}
		return num2;
	}

	private void ActualiserListe()
	{
		foreach (DataGridViewRow item in (IEnumerable)dgVoies.Rows)
		{
			if (item.Cells[1].Value == null)
			{
				item.Cells[1].Style.BackColor = Color.Red;
			}
			if (item.Tag != null)
			{
				continue;
			}
			string text = (string)item.Cells[0].Value;
			bool flag = true;
			int num = int.MinValue;
			foreach (DataGridViewRow item2 in (IEnumerable)dgVoies.Rows)
			{
				if (GetNomVoie(item2) == text)
				{
					if (num == int.MinValue)
					{
						num = (int)item2.Cells[1].Value;
					}
					else if ((int)item2.Cells[1].Value != num)
					{
						flag = false;
					}
				}
			}
			if (flag)
			{
				item.Cells[1].Value = num;
				item.Cells[1].Style.BackColor = Color.White;
			}
			else
			{
				item.Cells[1].Value = "";
				item.Cells[1].Style.BackColor = Color.Gainsboro;
			}
		}
		new Thread(Dessiner).Start();
	}

	private void Dessiner()
	{
		ComposantsViewer viewer = ComposantsViewer.Viewer;
		Image image = new Bitmap(pbPositions.Width, pbPositions.Height);
		Graphics graphics = Graphics.FromImage(image);
		double num = (viewer.PkFLigne - viewer.PkDLigne) / (pbPositions.Width - 20);
		int num2 = 1;
		foreach (DataGridViewRow item in (IEnumerable)dgVoies.Rows)
		{
			if (item.Cells[1].Value is int && (int)item.Cells[1].Value > int.MinValue)
			{
				num2 = Math.Max(Math.Abs((int)item.Cells[1].Value), num2);
			}
		}
		for (int i = -num2; i <= num2; i++)
		{
			int num3 = 10 + (i + num2) * (pbPositions.Height - 20) / (num2 * 2 + 1);
			graphics.DrawString(i.ToString(), Font, new SolidBrush(Color.Black), 2f, (float)num3 - Font.Size / 2f);
			graphics.DrawLine(new Pen(Color.FromArgb(180, 180, 180)), 20, num3, base.Width - 10, num3);
			foreach (DataGridViewRow _row in (IEnumerable)dgVoies.Rows)
			{
				if (!(_row.Cells[1].Value is int) || _row.Tag == null)
				{
					continue;
				}
				int num4 = (int)_row.Cells[1].Value;
				if (i == num4)
				{
					SIGVoie sIGVoie = _voies.Find((SIGVoie voie) => voie.ID == (int)_row.Tag);
					if (sIGVoie != null)
					{
						graphics.DrawLine(new Pen(Color.FromArgb(150, _row.Cells[2].Style.BackColor), 3f), (int)(20.0 + (double)(sIGVoie.PKDebut - viewer.PkDLigne) / num), num3, (int)(20.0 + (double)(sIGVoie.PKFin - viewer.PkDLigne) / num), num3);
					}
				}
			}
		}
		_imgPositions = image;
	}

	private bool ContainsVoie(string voie)
	{
		foreach (DataGridViewRow item in (IEnumerable)dgVoies.Rows)
		{
			if ((string)item.Cells[0].Value == voie)
			{
				return true;
			}
		}
		return false;
	}

	private bool IsOpen(string voie)
	{
		foreach (DataGridViewRow item in (IEnumerable)dgVoies.Rows)
		{
			if (GetNomVoie(item) == voie && item.Visible)
			{
				return true;
			}
		}
		return false;
	}

	private string GetNomVoie(DataGridViewRow row)
	{
		if (row.Tag != null && row.Tag is int)
		{
			SIGVoie sIGVoie = _voies.Find((SIGVoie voie) => voie.ID == (int)row.Tag);
			if (sIGVoie != null)
			{
				return sIGVoie.Nom;
			}
		}
		return string.Empty;
	}

	private void AddRow(string nomVoie, int pos, Color backColor)
	{
		DataGridViewRow dataGridViewRow = new DataGridViewRow();
		DataGridViewTextBoxCell dataGridViewTextBoxCell = new DataGridViewTextBoxCell();
		dataGridViewTextBoxCell.Value = nomVoie;
		dataGridViewRow.Cells.Add(dataGridViewTextBoxCell);
		dataGridViewTextBoxCell = new DataGridViewTextBoxCell();
		if (pos == int.MinValue)
		{
			dataGridViewTextBoxCell.Value = "";
		}
		else
		{
			dataGridViewTextBoxCell.Value = pos;
		}
		dataGridViewRow.Cells.Add(dataGridViewTextBoxCell);
		dataGridViewTextBoxCell = new DataGridViewTextBoxCell();
		dataGridViewTextBoxCell.Style.BackColor = backColor;
		dataGridViewRow.Cells.Add(dataGridViewTextBoxCell);
		dataGridViewRow.Height = 25;
		dgVoies.Rows.Add(dataGridViewRow);
	}

	private void AddRow(int idVoie, string texte, int pos, Color backColor)
	{
		DataGridViewRow dataGridViewRow = new DataGridViewRow();
		DataGridViewTextBoxCell dataGridViewTextBoxCell = new DataGridViewTextBoxCell();
		dataGridViewTextBoxCell.Value = texte;
		dataGridViewRow.Cells.Add(dataGridViewTextBoxCell);
		dataGridViewTextBoxCell = new DataGridViewTextBoxCell();
		dataGridViewTextBoxCell.Value = pos;
		dataGridViewRow.Cells.Add(dataGridViewTextBoxCell);
		dataGridViewTextBoxCell = new DataGridViewTextBoxCell();
		dataGridViewTextBoxCell.Style.BackColor = backColor;
		dataGridViewRow.Cells.Add(dataGridViewTextBoxCell);
		dataGridViewRow.Height = 20;
		dataGridViewRow.Tag = idVoie;
		dataGridViewRow.Visible = false;
		dgVoies.Rows.Add(dataGridViewRow);
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
		this.components = new System.ComponentModel.Container();
		this.bValider = new System.Windows.Forms.Button();
		this.bFermer = new System.Windows.Forms.Button();
		this.pbPositions = new System.Windows.Forms.PictureBox();
		this.dgVoies = new System.Windows.Forms.DataGridView();
		this.voie = new System.Windows.Forms.DataGridViewTextBoxColumn();
		this.position = new System.Windows.Forms.DataGridViewTextBoxColumn();
		this.couleur = new System.Windows.Forms.DataGridViewTextBoxColumn();
		this.label2 = new System.Windows.Forms.Label();
		this.timer = new System.Windows.Forms.Timer(this.components);
		((System.ComponentModel.ISupportInitialize)this.pbPositions).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.dgVoies).BeginInit();
		base.SuspendLayout();
		this.bValider.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
		this.bValider.Location = new System.Drawing.Point(632, 358);
		this.bValider.Name = "bValider";
		this.bValider.Size = new System.Drawing.Size(75, 23);
		this.bValider.TabIndex = 0;
		this.bValider.Text = "Valider";
		this.bValider.UseVisualStyleBackColor = true;
		this.bFermer.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
		this.bFermer.Location = new System.Drawing.Point(713, 358);
		this.bFermer.Name = "bFermer";
		this.bFermer.Size = new System.Drawing.Size(75, 23);
		this.bFermer.TabIndex = 1;
		this.bFermer.Text = "Fermer";
		this.bFermer.UseVisualStyleBackColor = true;
		this.pbPositions.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.pbPositions.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
		this.pbPositions.Location = new System.Drawing.Point(255, 35);
		this.pbPositions.MinimumSize = new System.Drawing.Size(400, 300);
		this.pbPositions.Name = "pbPositions";
		this.pbPositions.Size = new System.Drawing.Size(533, 317);
		this.pbPositions.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
		this.pbPositions.TabIndex = 3;
		this.pbPositions.TabStop = false;
		this.dgVoies.AllowUserToAddRows = false;
		this.dgVoies.AllowUserToDeleteRows = false;
		this.dgVoies.AllowUserToResizeRows = false;
		this.dgVoies.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.dgVoies.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
		this.dgVoies.Columns.AddRange(this.voie, this.position, this.couleur);
		this.dgVoies.Location = new System.Drawing.Point(12, 12);
		this.dgVoies.Name = "dgVoies";
		this.dgVoies.RowHeadersVisible = false;
		this.dgVoies.Size = new System.Drawing.Size(237, 340);
		this.dgVoies.TabIndex = 4;
		this.voie.HeaderText = "Voie";
		this.voie.Name = "voie";
		this.voie.ReadOnly = true;
		this.voie.Width = 115;
		this.position.HeaderText = "Position";
		this.position.Name = "position";
		this.position.Resizable = System.Windows.Forms.DataGridViewTriState.True;
		this.position.Width = 50;
		this.couleur.HeaderText = "Couleur";
		this.couleur.Name = "couleur";
		this.couleur.ReadOnly = true;
		this.couleur.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
		this.couleur.Width = 50;
		this.label2.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75f, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, 0);
		this.label2.Location = new System.Drawing.Point(255, 12);
		this.label2.Name = "label2";
		this.label2.Size = new System.Drawing.Size(533, 16);
		this.label2.TabIndex = 6;
		this.label2.Text = "Apercu";
		this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
		this.timer.Interval = 200;
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.ClientSize = new System.Drawing.Size(796, 392);
		base.Controls.Add(this.label2);
		base.Controls.Add(this.dgVoies);
		base.Controls.Add(this.pbPositions);
		base.Controls.Add(this.bFermer);
		base.Controls.Add(this.bValider);
		base.Name = "PositionVoiesDialog";
		base.ShowInTaskbar = false;
		base.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
		this.Text = "Positions des voies";
		((System.ComponentModel.ISupportInitialize)this.pbPositions).EndInit();
		((System.ComponentModel.ISupportInitialize)this.dgVoies).EndInit();
		base.ResumeLayout(false);
	}
}
