using System.Drawing;
using System.Windows.Forms;
using CDV_Viewer.Controls;
using CDV_Viewer.CsvBase;
using CDV_Viewer.CustomControls;
using CDV_Viewer.Data.SIGObjects;
using CDV_Viewer.Properties;
using CDV_Viewer.Traitements;

namespace CDV_Viewer.PopupForms;

public class RelierNoeudPopupForm : PopupForm
{
	private ComboBox cbLigne;

	private ComboBox cbVoie;

	private Label label1;

	private Label label6;

	private CustomTextBox tbPk;

	private ImageButton bValider;

	private Label label3;

	private SIGExtremite _extremiteOrigine;

	private int _pkOrigine;

	public SIGLigne LigneOrigine => _extremiteOrigine.Voie.Ligne;

	public SIGVoie VoieOrigine => _extremiteOrigine.Voie;

	public int PkOrigine => _pkOrigine;

	public SIGLigne LigneDestination
	{
		get
		{
			if (cbLigne.SelectedIndex >= 0)
			{
				return (SIGLigne)cbLigne.Items[cbLigne.SelectedIndex];
			}
			return null;
		}
		set
		{
			cbLigne.SelectedItem = value;
		}
	}

	public SIGVoie VoieDestination
	{
		get
		{
			if (cbVoie.SelectedIndex >= 0)
			{
				return cbVoie.SelectedItem as SIGVoie;
			}
			return null;
		}
		set
		{
			cbVoie.SelectedItem = VoieDestination;
		}
	}

	public int PkDestination
	{
		get
		{
			if (Chaines.TryParsePk(tbPk.Text, out var PK))
			{
				if (PK > 999999 || PK < -1000)
				{
					return -1000;
				}
				return PK;
			}
			return -1000;
		}
		set
		{
			tbPk.Text = Chaines.PkToString(value);
		}
	}

	public RelierNoeudPopupForm(SIGExtremite extremiteOrigine)
	{
		InitializeComponent();
		_extremiteOrigine = extremiteOrigine;
		_pkOrigine = extremiteOrigine.PK;
		foreach (SIGLigne ligne in Base.GetLignes())
		{
			cbLigne.Items.Add(ligne);
		}
		cbLigne.DisplayMember = "ID";
		cbLigne.SelectedIndexChanged += delegate
		{
			cbLigne_SelectedIndexChanged();
		};
		cbVoie.SelectedIndexChanged += delegate
		{
			cbVoie_SelectedIndexChanged();
		};
		bValider.Click += delegate
		{
			bValider_Click();
		};
		LigneDestination = _extremiteOrigine.Voie.Ligne;
		PkDestination = _pkOrigine;
	}

	private void InitializeComponent()
	{
		this.cbLigne = new System.Windows.Forms.ComboBox();
		this.label3 = new System.Windows.Forms.Label();
		this.cbVoie = new System.Windows.Forms.ComboBox();
		this.label1 = new System.Windows.Forms.Label();
		this.label6 = new System.Windows.Forms.Label();
		this.tbPk = new CDV_Viewer.CustomControls.CustomTextBox();
		this.bValider = new CDV_Viewer.Controls.ImageButton();
		base.SuspendLayout();
		this.cbLigne.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
		this.cbLigne.FormattingEnabled = true;
		this.cbLigne.Location = new System.Drawing.Point(48, 6);
		this.cbLigne.Name = "cbLigne";
		this.cbLigne.Size = new System.Drawing.Size(66, 21);
		this.cbLigne.TabIndex = 6;
		this.label3.AutoSize = true;
		this.label3.Location = new System.Drawing.Point(5, 9);
		this.label3.Name = "label3";
		this.label3.Size = new System.Drawing.Size(39, 13);
		this.label3.TabIndex = 5;
		this.label3.Text = "Ligne :";
		this.cbVoie.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
		this.cbVoie.FormattingEnabled = true;
		this.cbVoie.Location = new System.Drawing.Point(160, 6);
		this.cbVoie.Name = "cbVoie";
		this.cbVoie.Size = new System.Drawing.Size(66, 21);
		this.cbVoie.TabIndex = 12;
		this.label1.AutoSize = true;
		this.label1.Location = new System.Drawing.Point(123, 9);
		this.label1.Name = "label1";
		this.label1.Size = new System.Drawing.Size(34, 13);
		this.label1.TabIndex = 11;
		this.label1.Text = "Voie :";
		this.label6.AutoSize = true;
		this.label6.Location = new System.Drawing.Point(237, 9);
		this.label6.Name = "label6";
		this.label6.Size = new System.Drawing.Size(27, 13);
		this.label6.TabIndex = 14;
		this.label6.Text = "PK :";
		this.tbPk.BackColor = System.Drawing.Color.White;
		this.tbPk.Location = new System.Drawing.Point(262, 6);
		this.tbPk.Name = "tbPk";
		this.tbPk.Padding = new System.Windows.Forms.Padding(1);
		this.tbPk.SelectionLength = 0;
		this.tbPk.SelectionStart = 0;
		this.tbPk.Size = new System.Drawing.Size(70, 20);
		this.tbPk.TabIndex = 13;
		this.tbPk.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
		this.bValider.Cursor = System.Windows.Forms.Cursors.Hand;
		this.bValider.Image = CDV_Viewer.Properties.Resources.OkButton;
		this.bValider.Location = new System.Drawing.Point(338, 6);
		this.bValider.Name = "bValider";
		this.bValider.Size = new System.Drawing.Size(20, 20);
		this.bValider.TabIndex = 18;
		this.bValider.Text = "validButton1";
		this.bValider.UseVisualStyleBackColor = true;
		base.Controls.Add(this.bValider);
		base.Controls.Add(this.label6);
		base.Controls.Add(this.tbPk);
		base.Controls.Add(this.cbVoie);
		base.Controls.Add(this.label1);
		base.Controls.Add(this.cbLigne);
		base.Controls.Add(this.label3);
		base.Name = "LVPKPopupForm";
		base.Size = new System.Drawing.Size(366, 31);
		base.ResumeLayout(false);
		base.PerformLayout();
	}

	private void cbLigne_SelectedIndexChanged()
	{
		cbVoie.Items.Clear();
		if (cbLigne.SelectedIndex == -1)
		{
			return;
		}
		SIGVoie voie = _extremiteOrigine.Voie;
		if (LigneDestination == voie.Ligne)
		{
			foreach (SIGVoie voie2 in LigneDestination.Voies)
			{
				if (voie2 != voie && voie2.PKDebut <= _pkOrigine && _pkOrigine <= voie2.PKFin)
				{
					cbVoie.Items.Add(voie2);
				}
			}
		}
		else
		{
			foreach (SIGVoie voie3 in LigneDestination.Voies)
			{
				cbVoie.Items.Add(voie3);
			}
		}
		cbVoie.DisplayMember = "Nom";
		if (cbVoie.Items.Count > 0)
		{
			cbVoie.SelectedIndex = 0;
		}
	}

	private void cbVoie_SelectedIndexChanged()
	{
		int pkOrigine = _pkOrigine;
		if (cbVoie.SelectedItem is SIGVoie sIGVoie)
		{
			if (pkOrigine < sIGVoie.PKDebut)
			{
				PkDestination = sIGVoie.PKDebut;
			}
			else if (pkOrigine > sIGVoie.PKFin)
			{
				PkDestination = sIGVoie.PKFin;
			}
		}
	}

	private void bValider_Click()
	{
		if (!(cbVoie.SelectedItem is SIGVoie))
		{
			MessageBox.Show("Erreur ... voie inconnue", Resources.APP_NAME);
		}
		else
		{
			Close(PopupFormResultEventArgs.Ok);
		}
	}
}
