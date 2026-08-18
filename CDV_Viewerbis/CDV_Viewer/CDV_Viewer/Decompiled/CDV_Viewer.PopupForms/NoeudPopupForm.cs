using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using CDV_Viewer.Controls;
using CDV_Viewer.Data.SIGObjects;
using CDV_Viewer.Properties;
using CDV_Viewer.Traitements;

namespace CDV_Viewer.PopupForms;

public class NoeudPopupForm : PopupForm
{
	private class Position
	{
		public SIGVoie Voie;

		public int PK;

		public bool Amont;

		public bool Aval;

		public Position(SIGVoie voie, int pk)
		{
			Voie = voie;
			PK = pk;
		}
	}

	private PictureBox pbType3;

	private Label lPk3;

	private Label lVoie3;

	private Label lLigne3;

	private PictureBox pbType2;

	private Label lPk2;

	private Label lVoie2;

	private Label lLigne2;

	private PictureBox pbType1;

	private Label lPk1;

	private Label lVoie1;

	private Label lLigne1;

	public NoeudPopupForm(SIGNoeud noeud)
	{
		InitializeComponent();
		List<Position> list = new List<Position>();
		foreach (SIGBranche _branche in noeud.Branches)
		{
			Position position = list.Find((Position position2) => position2.Voie == _branche.Voie && position2.PK == _branche.PK);
			if (position == null)
			{
				position = new Position(_branche.Voie, _branche.PK);
				list.Add(position);
			}
			if (_branche.IsAmont)
			{
				position.Amont = true;
			}
			if (_branche.IsAval)
			{
				position.Aval = true;
			}
		}
		base.Height = Math.Min(base.Height, 25 * list.Count);
		if (list.Count >= 1)
		{
			pbType1.Image = GetImage(list[0]);
			lLigne1.Text = list[0].Voie.Ligne.ID.ToString();
			lVoie1.Text = list[0].Voie.Nom;
			lPk1.Text = Chaines.PkToString(list[0].PK);
		}
		if (list.Count >= 2)
		{
			pbType2.Image = GetImage(list[1]);
			lLigne2.Text = list[1].Voie.Ligne.ID.ToString();
			lVoie2.Text = list[1].Voie.Nom;
			lPk2.Text = Chaines.PkToString(list[1].PK);
		}
		if (list.Count >= 3)
		{
			pbType3.Image = GetImage(list[2]);
			lLigne3.Text = list[2].Voie.Ligne.ID.ToString();
			lVoie3.Text = list[2].Voie.Nom;
			lPk3.Text = Chaines.PkToString(list[2].PK);
		}
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

	private void InitializeComponent()
	{
		this.pbType3 = new System.Windows.Forms.PictureBox();
		this.lPk3 = new System.Windows.Forms.Label();
		this.lVoie3 = new System.Windows.Forms.Label();
		this.lLigne3 = new System.Windows.Forms.Label();
		this.pbType2 = new System.Windows.Forms.PictureBox();
		this.lPk2 = new System.Windows.Forms.Label();
		this.lVoie2 = new System.Windows.Forms.Label();
		this.lLigne2 = new System.Windows.Forms.Label();
		this.pbType1 = new System.Windows.Forms.PictureBox();
		this.lPk1 = new System.Windows.Forms.Label();
		this.lVoie1 = new System.Windows.Forms.Label();
		this.lLigne1 = new System.Windows.Forms.Label();
		((System.ComponentModel.ISupportInitialize)this.pbType3).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.pbType2).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.pbType1).BeginInit();
		base.SuspendLayout();
		this.pbType3.Location = new System.Drawing.Point(5, 52);
		this.pbType3.Name = "pbType3";
		this.pbType3.Size = new System.Drawing.Size(16, 16);
		this.pbType3.TabIndex = 35;
		this.pbType3.TabStop = false;
		this.lPk3.AutoSize = true;
		this.lPk3.Location = new System.Drawing.Point(115, 54);
		this.lPk3.Name = "lPk3";
		this.lPk3.Size = new System.Drawing.Size(49, 13);
		this.lPk3.TabIndex = 34;
		this.lPk3.Text = "000+000";
		this.lVoie3.AutoSize = true;
		this.lVoie3.Location = new System.Drawing.Point(69, 54);
		this.lVoie3.Name = "lVoie3";
		this.lVoie3.Size = new System.Drawing.Size(28, 13);
		this.lVoie3.TabIndex = 33;
		this.lVoie3.Text = "Voie";
		this.lLigne3.AutoSize = true;
		this.lLigne3.Location = new System.Drawing.Point(25, 54);
		this.lLigne3.Name = "lLigne3";
		this.lLigne3.Size = new System.Drawing.Size(43, 13);
		this.lLigne3.TabIndex = 32;
		this.lLigne3.Text = "000000";
		this.pbType2.Location = new System.Drawing.Point(5, 29);
		this.pbType2.Name = "pbType2";
		this.pbType2.Size = new System.Drawing.Size(16, 16);
		this.pbType2.TabIndex = 31;
		this.pbType2.TabStop = false;
		this.lPk2.AutoSize = true;
		this.lPk2.Location = new System.Drawing.Point(115, 31);
		this.lPk2.Name = "lPk2";
		this.lPk2.Size = new System.Drawing.Size(49, 13);
		this.lPk2.TabIndex = 30;
		this.lPk2.Text = "000+000";
		this.lVoie2.AutoSize = true;
		this.lVoie2.Location = new System.Drawing.Point(69, 31);
		this.lVoie2.Name = "lVoie2";
		this.lVoie2.Size = new System.Drawing.Size(28, 13);
		this.lVoie2.TabIndex = 29;
		this.lVoie2.Text = "Voie";
		this.lLigne2.AutoSize = true;
		this.lLigne2.Location = new System.Drawing.Point(25, 31);
		this.lLigne2.Name = "lLigne2";
		this.lLigne2.Size = new System.Drawing.Size(43, 13);
		this.lLigne2.TabIndex = 28;
		this.lLigne2.Text = "000000";
		this.pbType1.Location = new System.Drawing.Point(5, 6);
		this.pbType1.Name = "pbType1";
		this.pbType1.Size = new System.Drawing.Size(16, 16);
		this.pbType1.TabIndex = 27;
		this.pbType1.TabStop = false;
		this.lPk1.AutoSize = true;
		this.lPk1.Location = new System.Drawing.Point(115, 8);
		this.lPk1.Name = "lPk1";
		this.lPk1.Size = new System.Drawing.Size(49, 13);
		this.lPk1.TabIndex = 26;
		this.lPk1.Text = "000+000";
		this.lVoie1.AutoSize = true;
		this.lVoie1.Location = new System.Drawing.Point(69, 8);
		this.lVoie1.Name = "lVoie1";
		this.lVoie1.Size = new System.Drawing.Size(28, 13);
		this.lVoie1.TabIndex = 25;
		this.lVoie1.Text = "Voie";
		this.lLigne1.AutoSize = true;
		this.lLigne1.Location = new System.Drawing.Point(25, 8);
		this.lLigne1.Name = "lLigne1";
		this.lLigne1.Size = new System.Drawing.Size(43, 13);
		this.lLigne1.TabIndex = 24;
		this.lLigne1.Text = "000000";
		base.Controls.Add(this.pbType3);
		base.Controls.Add(this.lPk3);
		base.Controls.Add(this.lVoie3);
		base.Controls.Add(this.lLigne3);
		base.Controls.Add(this.pbType2);
		base.Controls.Add(this.lPk2);
		base.Controls.Add(this.lVoie2);
		base.Controls.Add(this.lLigne2);
		base.Controls.Add(this.pbType1);
		base.Controls.Add(this.lPk1);
		base.Controls.Add(this.lVoie1);
		base.Controls.Add(this.lLigne1);
		base.Name = "NoeudPopupForm";
		base.Size = new System.Drawing.Size(169, 75);
		((System.ComponentModel.ISupportInitialize)this.pbType3).EndInit();
		((System.ComponentModel.ISupportInitialize)this.pbType2).EndInit();
		((System.ComponentModel.ISupportInitialize)this.pbType1).EndInit();
		base.ResumeLayout(false);
		base.PerformLayout();
	}
}
