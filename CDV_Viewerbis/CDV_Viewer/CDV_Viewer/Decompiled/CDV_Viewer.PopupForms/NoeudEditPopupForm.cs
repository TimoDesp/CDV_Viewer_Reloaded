using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using CDV_Viewer.Controls;
using CDV_Viewer.CsvBase;
using CDV_Viewer.CustomControls;
using CDV_Viewer.Data;
using CDV_Viewer.Data.SIGObjects;
using CDV_Viewer.Properties;
using CDV_Viewer.Traitements;

namespace CDV_Viewer.PopupForms;

public class NoeudEditPopupForm : PopupForm
{
	private class Position
	{
		public SIGVoie Voie;

		public List<SIGBranche> Branches = new List<SIGBranche>();

		public int PK;

		public bool Amont;

		public bool Aval;

		public Position(SIGBranche branche)
		{
			Voie = branche.Voie;
			PK = branche.PK;
			Update(branche);
		}

		public void Update(SIGBranche branche)
		{
			Branches.Add(branche);
			if (branche.IsAmont)
			{
				Amont = true;
			}
			if (branche.IsAval)
			{
				Aval = true;
			}
		}
	}

	private class Positions
	{
		private List<Position> _items = new List<Position>();

		public int Count => _items.Count;

		public Position this[SIGBranche branche] => CreateOrUpdate(branche);

		public Position this[int i] => _items[i];

		public Position CreateOrUpdate(SIGBranche branche)
		{
			foreach (Position item in _items)
			{
				if (item.Branches.Contains(branche))
				{
					return item;
				}
				if (!branche.Noeud.IsSautPk && item.Voie.ID == branche.Voie.ID && item.PK == branche.PK)
				{
					item.Update(branche);
					return item;
				}
			}
			Position position = new Position(branche);
			_items.Add(position);
			return position;
		}

		public int Index(Position pos)
		{
			return _items.IndexOf(pos);
		}
	}

	private Panel branche1;

	private PictureBox pbType1;

	private Label lLigne1;

	private Label lVoie1;

	private Label lPk1;

	private CustomTextBox tbPk1;

	private TextBox cbVoie1;

	private TextBox cbLigne1;

	private Panel branche2;

	private PictureBox pbType2;

	private Label lLigne2;

	private Label lVoie2;

	private Label lPk2;

	private CustomTextBox tbPk2;

	private TextBox cbVoie2;

	private TextBox cbLigne2;

	private Panel branche3;

	private PictureBox pbType3;

	private Label lLigne3;

	private Label lVoie3;

	private Label lPk3;

	private CustomTextBox tbPk3;

	private TextBox cbVoie3;

	private TextBox cbLigne3;

	private Positions _positions = new Positions();

	private SIGNoeud _noeud;

	public NoeudEditPopupForm(SIGNoeud noeud)
	{
		InitializeComponent();
		_noeud = noeud;
		noeud.Branches.ForEach(delegate(SIGBranche b)
		{
			_positions.CreateOrUpdate(b);
		});
		branche1.Visible = false;
		branche2.Visible = false;
		branche3.Visible = false;
		if (_positions.Count >= 1)
		{
			pbType1.Image = GetImage(_positions[0]);
			cbLigne1.Text = _positions[0].Voie.Ligne.ID.ToString();
			cbVoie1.Text = _positions[0].Voie.Nom;
			tbPk1.Text = Chaines.PkToString(_positions[0].PK);
			branche1.Visible = true;
		}
		if (_positions.Count >= 2)
		{
			pbType2.Image = GetImage(_positions[1]);
			cbLigne2.Text = _positions[1].Voie.Ligne.ID.ToString();
			cbVoie2.Text = _positions[1].Voie.Nom;
			tbPk2.Text = Chaines.PkToString(_positions[1].PK);
			branche2.Visible = true;
		}
		if (_positions.Count >= 3)
		{
			pbType3.Image = GetImage(_positions[2]);
			cbLigne3.Text = _positions[2].Voie.Ligne.ID.ToString();
			cbVoie3.Text = _positions[2].Voie.Nom;
			tbPk3.Text = Chaines.PkToString(_positions[2].PK);
			branche3.Visible = true;
		}
		pbType1.Click += delegate
		{
			pbType_Click(pbType1, 0);
		};
		pbType2.Click += delegate
		{
			pbType_Click(pbType2, 1);
		};
		pbType3.Click += delegate
		{
			pbType_Click(pbType3, 2);
		};
	}

	private Image GetImage(Position pos)
	{
		if (pos.Amont && pos.Aval)
		{
			return Resources.amontaval;
		}
		if (pos.Amont)
		{
			return Resources.amont;
		}
		if (pos.Aval)
		{
			return Resources.aval;
		}
		return null;
	}

	private void pbType_Click(PictureBox picture, int index)
	{
		Position position = _positions[index];
		if (!position.Amont || !position.Aval)
		{
			if (position.Amont)
			{
				position.Aval = true;
				position.Amont = false;
			}
			else if (position.Aval)
			{
				position.Amont = true;
				position.Aval = false;
			}
			picture.Image = GetImage(position);
		}
	}

	private void InitializeComponent()
	{
		this.lPk3 = new System.Windows.Forms.Label();
		this.lVoie3 = new System.Windows.Forms.Label();
		this.lLigne3 = new System.Windows.Forms.Label();
		this.lPk2 = new System.Windows.Forms.Label();
		this.lVoie2 = new System.Windows.Forms.Label();
		this.lLigne2 = new System.Windows.Forms.Label();
		this.lPk1 = new System.Windows.Forms.Label();
		this.lVoie1 = new System.Windows.Forms.Label();
		this.lLigne1 = new System.Windows.Forms.Label();
		this.pbType3 = new System.Windows.Forms.PictureBox();
		this.pbType2 = new System.Windows.Forms.PictureBox();
		this.pbType1 = new System.Windows.Forms.PictureBox();
		this.branche1 = new System.Windows.Forms.Panel();
		this.tbPk1 = new CDV_Viewer.CustomControls.CustomTextBox();
		this.cbVoie1 = new System.Windows.Forms.TextBox();
		this.cbLigne1 = new System.Windows.Forms.TextBox();
		this.branche2 = new System.Windows.Forms.Panel();
		this.tbPk2 = new CDV_Viewer.CustomControls.CustomTextBox();
		this.cbVoie2 = new System.Windows.Forms.TextBox();
		this.cbLigne2 = new System.Windows.Forms.TextBox();
		this.branche3 = new System.Windows.Forms.Panel();
		this.tbPk3 = new CDV_Viewer.CustomControls.CustomTextBox();
		this.cbVoie3 = new System.Windows.Forms.TextBox();
		this.cbLigne3 = new System.Windows.Forms.TextBox();
		((System.ComponentModel.ISupportInitialize)this.pbType3).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.pbType2).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.pbType1).BeginInit();
		this.branche1.SuspendLayout();
		this.branche2.SuspendLayout();
		this.branche3.SuspendLayout();
		base.SuspendLayout();
		this.lPk3.AutoSize = true;
		this.lPk3.Location = new System.Drawing.Point(256, 6);
		this.lPk3.Name = "lPk3";
		this.lPk3.Size = new System.Drawing.Size(27, 13);
		this.lPk3.TabIndex = 53;
		this.lPk3.Text = "PK :";
		this.lVoie3.AutoSize = true;
		this.lVoie3.Location = new System.Drawing.Point(142, 6);
		this.lVoie3.Name = "lVoie3";
		this.lVoie3.Size = new System.Drawing.Size(34, 13);
		this.lVoie3.TabIndex = 50;
		this.lVoie3.Text = "Voie :";
		this.lLigne3.AutoSize = true;
		this.lLigne3.Location = new System.Drawing.Point(24, 6);
		this.lLigne3.Name = "lLigne3";
		this.lLigne3.Size = new System.Drawing.Size(39, 13);
		this.lLigne3.TabIndex = 48;
		this.lLigne3.Text = "Ligne :";
		this.lPk2.AutoSize = true;
		this.lPk2.Location = new System.Drawing.Point(256, 6);
		this.lPk2.Name = "lPk2";
		this.lPk2.Size = new System.Drawing.Size(27, 13);
		this.lPk2.TabIndex = 47;
		this.lPk2.Text = "PK :";
		this.lVoie2.AutoSize = true;
		this.lVoie2.Location = new System.Drawing.Point(142, 6);
		this.lVoie2.Name = "lVoie2";
		this.lVoie2.Size = new System.Drawing.Size(34, 13);
		this.lVoie2.TabIndex = 44;
		this.lVoie2.Text = "Voie :";
		this.lLigne2.AutoSize = true;
		this.lLigne2.Location = new System.Drawing.Point(24, 6);
		this.lLigne2.Name = "lLigne2";
		this.lLigne2.Size = new System.Drawing.Size(39, 13);
		this.lLigne2.TabIndex = 42;
		this.lLigne2.Text = "Ligne :";
		this.lPk1.AutoSize = true;
		this.lPk1.Location = new System.Drawing.Point(256, 8);
		this.lPk1.Name = "lPk1";
		this.lPk1.Size = new System.Drawing.Size(27, 13);
		this.lPk1.TabIndex = 41;
		this.lPk1.Text = "PK :";
		this.lVoie1.AutoSize = true;
		this.lVoie1.Location = new System.Drawing.Point(142, 8);
		this.lVoie1.Name = "lVoie1";
		this.lVoie1.Size = new System.Drawing.Size(34, 13);
		this.lVoie1.TabIndex = 38;
		this.lVoie1.Text = "Voie :";
		this.lLigne1.AutoSize = true;
		this.lLigne1.Location = new System.Drawing.Point(24, 8);
		this.lLigne1.Name = "lLigne1";
		this.lLigne1.Size = new System.Drawing.Size(39, 13);
		this.lLigne1.TabIndex = 36;
		this.lLigne1.Text = "Ligne :";
		this.pbType3.Cursor = System.Windows.Forms.Cursors.Hand;
		this.pbType3.Location = new System.Drawing.Point(4, 4);
		this.pbType3.Name = "pbType3";
		this.pbType3.Size = new System.Drawing.Size(16, 16);
		this.pbType3.TabIndex = 35;
		this.pbType3.TabStop = false;
		this.pbType2.Cursor = System.Windows.Forms.Cursors.Hand;
		this.pbType2.Location = new System.Drawing.Point(4, 4);
		this.pbType2.Name = "pbType2";
		this.pbType2.Size = new System.Drawing.Size(16, 16);
		this.pbType2.TabIndex = 34;
		this.pbType2.TabStop = false;
		this.pbType1.Cursor = System.Windows.Forms.Cursors.Hand;
		this.pbType1.Location = new System.Drawing.Point(4, 4);
		this.pbType1.Name = "pbType1";
		this.pbType1.Size = new System.Drawing.Size(16, 16);
		this.pbType1.TabIndex = 33;
		this.pbType1.TabStop = false;
		this.branche1.Controls.Add(this.tbPk1);
		this.branche1.Controls.Add(this.cbVoie1);
		this.branche1.Controls.Add(this.cbLigne1);
		this.branche1.Controls.Add(this.lPk1);
		this.branche1.Controls.Add(this.lVoie1);
		this.branche1.Controls.Add(this.lLigne1);
		this.branche1.Controls.Add(this.pbType1);
		this.branche1.Location = new System.Drawing.Point(0, 0);
		this.branche1.Name = "branche1";
		this.branche1.Size = new System.Drawing.Size(375, 28);
		this.branche1.TabIndex = 54;
		this.tbPk1.BackColor = System.Drawing.SystemColors.ButtonHighlight;
		this.tbPk1.Location = new System.Drawing.Point(286, 3);
		this.tbPk1.Name = "tbPk1";
		this.tbPk1.Padding = new System.Windows.Forms.Padding(1);
		this.tbPk1.SelectionLength = 0;
		this.tbPk1.SelectionStart = 0;
		this.tbPk1.Size = new System.Drawing.Size(76, 20);
		this.tbPk1.TabIndex = 42;
		this.tbPk1.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
		this.cbVoie1.BackColor = System.Drawing.SystemColors.ButtonHighlight;
		this.cbVoie1.Location = new System.Drawing.Point(182, 3);
		this.cbVoie1.Name = "cbVoie1";
		this.cbVoie1.ReadOnly = true;
		this.cbVoie1.Size = new System.Drawing.Size(66, 20);
		this.cbVoie1.TabIndex = 42;
		this.cbLigne1.BackColor = System.Drawing.SystemColors.ButtonHighlight;
		this.cbLigne1.Location = new System.Drawing.Point(67, 3);
		this.cbLigne1.Name = "cbLigne1";
		this.cbLigne1.ReadOnly = true;
		this.cbLigne1.Size = new System.Drawing.Size(66, 20);
		this.cbLigne1.TabIndex = 42;
		this.branche2.Controls.Add(this.tbPk2);
		this.branche2.Controls.Add(this.cbVoie2);
		this.branche2.Controls.Add(this.cbLigne2);
		this.branche2.Controls.Add(this.lPk2);
		this.branche2.Controls.Add(this.lVoie2);
		this.branche2.Controls.Add(this.lLigne2);
		this.branche2.Controls.Add(this.pbType2);
		this.branche2.Location = new System.Drawing.Point(0, 27);
		this.branche2.Name = "branche2";
		this.branche2.Size = new System.Drawing.Size(375, 26);
		this.branche2.TabIndex = 55;
		this.tbPk2.BackColor = System.Drawing.SystemColors.ButtonHighlight;
		this.tbPk2.Location = new System.Drawing.Point(286, 3);
		this.tbPk2.Name = "tbPk2";
		this.tbPk2.Padding = new System.Windows.Forms.Padding(1);
		this.tbPk2.SelectionLength = 0;
		this.tbPk2.SelectionStart = 0;
		this.tbPk2.Size = new System.Drawing.Size(76, 20);
		this.tbPk2.TabIndex = 42;
		this.tbPk2.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
		this.cbVoie2.BackColor = System.Drawing.SystemColors.ButtonHighlight;
		this.cbVoie2.Location = new System.Drawing.Point(182, 3);
		this.cbVoie2.Name = "cbVoie2";
		this.cbVoie2.ReadOnly = true;
		this.cbVoie2.Size = new System.Drawing.Size(66, 20);
		this.cbVoie2.TabIndex = 42;
		this.cbLigne2.BackColor = System.Drawing.SystemColors.ButtonHighlight;
		this.cbLigne2.Location = new System.Drawing.Point(67, 3);
		this.cbLigne2.Name = "cbLigne2";
		this.cbLigne2.ReadOnly = true;
		this.cbLigne2.Size = new System.Drawing.Size(66, 20);
		this.cbLigne2.TabIndex = 42;
		this.branche3.Controls.Add(this.tbPk3);
		this.branche3.Controls.Add(this.cbVoie3);
		this.branche3.Controls.Add(this.cbLigne3);
		this.branche3.Controls.Add(this.lPk3);
		this.branche3.Controls.Add(this.lVoie3);
		this.branche3.Controls.Add(this.lLigne3);
		this.branche3.Controls.Add(this.pbType3);
		this.branche3.Location = new System.Drawing.Point(0, 52);
		this.branche3.Name = "branche3";
		this.branche3.Size = new System.Drawing.Size(375, 26);
		this.branche3.TabIndex = 56;
		this.tbPk3.BackColor = System.Drawing.SystemColors.ButtonHighlight;
		this.tbPk3.Location = new System.Drawing.Point(286, 3);
		this.tbPk3.Name = "tbPk3";
		this.tbPk3.Padding = new System.Windows.Forms.Padding(1);
		this.tbPk3.SelectionLength = 0;
		this.tbPk3.SelectionStart = 0;
		this.tbPk3.Size = new System.Drawing.Size(76, 20);
		this.tbPk3.TabIndex = 42;
		this.tbPk3.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
		this.cbVoie3.BackColor = System.Drawing.SystemColors.ButtonHighlight;
		this.cbVoie3.Location = new System.Drawing.Point(182, 3);
		this.cbVoie3.Name = "cbVoie3";
		this.cbVoie3.ReadOnly = true;
		this.cbVoie3.Size = new System.Drawing.Size(66, 20);
		this.cbVoie3.TabIndex = 42;
		this.cbLigne3.BackColor = System.Drawing.SystemColors.ButtonHighlight;
		this.cbLigne3.Location = new System.Drawing.Point(67, 3);
		this.cbLigne3.Name = "cbLigne3";
		this.cbLigne3.ReadOnly = true;
		this.cbLigne3.Size = new System.Drawing.Size(66, 20);
		this.cbLigne3.TabIndex = 42;
		base.Controls.Add(this.branche3);
		base.Controls.Add(this.branche2);
		base.Controls.Add(this.branche1);
		base.Name = "NoeudEditPopupForm";
		base.Size = new System.Drawing.Size(378, 82);
		((System.ComponentModel.ISupportInitialize)this.pbType3).EndInit();
		((System.ComponentModel.ISupportInitialize)this.pbType2).EndInit();
		((System.ComponentModel.ISupportInitialize)this.pbType1).EndInit();
		this.branche1.ResumeLayout(false);
		this.branche1.PerformLayout();
		this.branche2.ResumeLayout(false);
		this.branche2.PerformLayout();
		this.branche3.ResumeLayout(false);
		this.branche3.PerformLayout();
		base.ResumeLayout(false);
	}

	protected override void OnClosing(PopupFormResultEventArgs e)
	{
		if (e.Result != PopupContainerResult.OK)
		{
			return;
		}
		foreach (SIGBranche branch in _noeud.Branches)
		{
			Position position = _positions[branch];
			if (position == null)
			{
				continue;
			}
			if (branch.IsAmont && !position.Amont)
			{
				branch.Type = BrancheType.Aval;
			}
			else if (branch.IsAval && !position.Aval)
			{
				branch.Type = BrancheType.Amont;
			}
			int PK;
			switch (_positions.Index(position))
			{
			case 0:
				if (Chaines.TryParsePk(tbPk1.Text, out PK))
				{
					branch.PK = PK;
				}
				break;
			case 1:
				if (Chaines.TryParsePk(tbPk2.Text, out PK))
				{
					branch.PK = PK;
				}
				break;
			case 2:
				if (Chaines.TryParsePk(tbPk3.Text, out PK))
				{
					branch.PK = PK;
				}
				break;
			}
		}
	}
}
