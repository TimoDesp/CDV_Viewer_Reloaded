using System;
using System.Drawing;
using System.Windows.Forms;
using CDV_Viewer.Controls;
using CDV_Viewer.CsvBase;
using CDV_Viewer.CustomControls;
using CDV_Viewer.Data.SIGObjects;
using CDV_Viewer.Traitements;

namespace CDV_Viewer.PopupForms;

public class BalisePopupForm : PopupForm
{
	private ComboBox cbType;

	private Label lType;

	private CheckBox cbEnable;

	private CheckControl ccEnable;

	private Label lNom;

	private Label lTextType;

	private PKBox pkBox;

	public readonly SIGVoie Voie;

	public int Pk
	{
		get
		{
			if (Chaines.TryParsePk(pkBox.Text, out var PK))
			{
				return PK;
			}
			return 0;
		}
	}

	public string Type => cbType.Text;

	public bool Actif => cbEnable.Checked;

	public BalisePopupForm(SIGVoie voie, int pk)
	{
		InitializeComponent_write();
		Voie = voie;
		BaliseType baliseType = BaliseType.BLGV;
		ComboBox.ObjectCollection items = cbType.Items;
		object[] names = Enum.GetNames(typeof(BaliseType));
		items.AddRange(names);
		cbType.Text = baliseType.ToString();
		lTextType.Text = "Pk :";
		pkBox.Text = Chaines.PkToString(pk);
		lType.Visible = false;
		cbEnable.Visible = true;
		ccEnable.Visible = false;
	}

	public BalisePopupForm(SIGBalise balise, bool editMode)
	{
		InitializeComponent();
		cbEnable.Checked = (ccEnable.Checked = balise.Actif);
		lTextType.Text = "Pk :";
		pkBox.Text = Chaines.PkToString(balise.PK);
		if (editMode)
		{
			ComboBox.ObjectCollection items = cbType.Items;
			object[] names = Enum.GetNames(typeof(BaliseType));
			items.AddRange(names);
			cbType.Text = balise.Type.ToString();
			lNom.Text = "Balise";
			cbEnable.Visible = true;
			ccEnable.Visible = false;
			lType.Visible = false;
		}
		else
		{
			lNom.Text = "Balise " + balise.Type;
			pkBox.Visible = false;
			cbEnable.Visible = false;
			ccEnable.Visible = true;
			cbType.Visible = false;
			lType.Left = 30;
			lType.Text = Chaines.PkToString(balise.PK);
			base.Width = 95;
			base.Height = lType.Bottom + 5;
		}
	}

	private void InitializeComponent_write()
	{
		CustomControlColor couleur = new CustomControlColor();
		lNom = new Label();
		cbEnable = new CheckBox();
		ccEnable = new CheckControl();
		lType = new Label();
		cbType = new ComboBox();
		lTextType = new Label();
		pkBox = new PKBox();
		SuspendLayout();
		lNom.Font = new Font("Arial", 9.75f, FontStyle.Regular, GraphicsUnit.Point, 0);
		lNom.ForeColor = Color.FromArgb(224, 82, 6);
		lNom.Location = new Point(82, 3);
		lNom.Name = "lNom";
		lNom.Size = new Size(90, 16);
		lNom.TabIndex = 8;
		lNom.Text = "BALISE";
		lNom.TextAlign = ContentAlignment.MiddleCenter;
		cbEnable.AutoSize = true;
		cbEnable.Location = new Point(5, 53);
		cbEnable.Name = "cbEnable";
		cbEnable.Size = new Size(56, 17);
		cbEnable.TabIndex = 6;
		cbEnable.Text = "Active";
		cbEnable.UseVisualStyleBackColor = true;
		ccEnable.Checked = false;
		ccEnable.Location = new Point(5, 53);
		ccEnable.Name = "ccEnable";
		ccEnable.Size = new Size(57, 16);
		ccEnable.TabIndex = 7;
		ccEnable.Texte = "Active";
		lType.AutoSize = true;
		lType.Location = new Point(25, 122);
		lType.Name = "lType";
		lType.Size = new Size(37, 13);
		lType.TabIndex = 5;
		lType.Text = "Type :";
		cbType.FlatStyle = FlatStyle.Popup;
		cbType.FormattingEnabled = true;
		cbType.Location = new Point(74, 50);
		cbType.Name = "cbType";
		cbType.Size = new Size(143, 21);
		cbType.TabIndex = 4;
		lTextType.AutoSize = true;
		lTextType.Location = new Point(10, 88);
		lTextType.Name = "lTextType";
		lTextType.Size = new Size(37, 13);
		lTextType.TabIndex = 3;
		lTextType.Text = "Type :";
		pkBox.BackColor = Color.White;
		pkBox.Couleur = couleur;
		pkBox.Location = new Point(74, 82);
		pkBox.MaximumSize = new Size(55, 19);
		pkBox.Name = "pkBox";
		pkBox.Padding = new Padding(1);
		pkBox.SelectionLength = 0;
		pkBox.SelectionStart = 0;
		pkBox.Size = new Size(55, 19);
		pkBox.TabIndex = 9;
		pkBox.TextAlign = HorizontalAlignment.Left;
		base.Controls.Add(pkBox);
		base.Controls.Add(lNom);
		base.Controls.Add(cbEnable);
		base.Controls.Add(ccEnable);
		base.Controls.Add(lType);
		base.Controls.Add(cbType);
		base.Controls.Add(lTextType);
		base.Name = "BalisePopupForm";
		base.Size = new Size(285, 150);
		ResumeLayout(performLayout: false);
		PerformLayout();
	}

	private void InitializeComponent()
	{
		CDV_Viewer.CustomControls.CustomControlColor couleur = new CDV_Viewer.CustomControls.CustomControlColor();
		this.lNom = new System.Windows.Forms.Label();
		this.cbEnable = new System.Windows.Forms.CheckBox();
		this.ccEnable = new CDV_Viewer.Controls.CheckControl();
		this.lType = new System.Windows.Forms.Label();
		this.cbType = new System.Windows.Forms.ComboBox();
		this.lTextType = new System.Windows.Forms.Label();
		this.pkBox = new CDV_Viewer.Controls.PKBox();
		base.SuspendLayout();
		this.lNom.Font = new System.Drawing.Font("Arial", 9.75f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		this.lNom.ForeColor = System.Drawing.Color.FromArgb(224, 82, 6);
		this.lNom.Location = new System.Drawing.Point(5, 5);
		this.lNom.Name = "lNom";
		this.lNom.Size = new System.Drawing.Size(90, 16);
		this.lNom.TabIndex = 8;
		this.lNom.Text = "BALISE";
		this.lNom.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
		this.cbEnable.AutoSize = true;
		this.cbEnable.Location = new System.Drawing.Point(5, 30);
		this.cbEnable.Name = "cbEnable";
		this.cbEnable.Size = new System.Drawing.Size(56, 17);
		this.cbEnable.TabIndex = 6;
		this.cbEnable.Text = "Active";
		this.cbEnable.UseVisualStyleBackColor = true;
		this.ccEnable.Checked = false;
		this.ccEnable.Location = new System.Drawing.Point(5, 31);
		this.ccEnable.Name = "ccEnable";
		this.ccEnable.Size = new System.Drawing.Size(57, 16);
		this.ccEnable.TabIndex = 7;
		this.ccEnable.Texte = "Active";
		this.lType.AutoSize = true;
		this.lType.Location = new System.Drawing.Point(48, 72);
		this.lType.Name = "lType";
		this.lType.Size = new System.Drawing.Size(37, 13);
		this.lType.TabIndex = 5;
		this.lType.Text = "Type :";
		this.cbType.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
		this.cbType.FormattingEnabled = true;
		this.cbType.Location = new System.Drawing.Point(77, 28);
		this.cbType.Name = "cbType";
		this.cbType.Size = new System.Drawing.Size(78, 21);
		this.cbType.TabIndex = 4;
		this.lTextType.AutoSize = true;
		this.lTextType.Location = new System.Drawing.Point(5, 72);
		this.lTextType.Name = "lTextType";
		this.lTextType.Size = new System.Drawing.Size(37, 13);
		this.lTextType.TabIndex = 3;
		this.lTextType.Text = "Type :";
		this.pkBox.BackColor = System.Drawing.Color.White;
		this.pkBox.Couleur = couleur;
		this.pkBox.Location = new System.Drawing.Point(57, 70);
		this.pkBox.Name = "pkBox";
		this.pkBox.Padding = new System.Windows.Forms.Padding(1);
		this.pkBox.SelectionLength = 0;
		this.pkBox.SelectionStart = 0;
		this.pkBox.Size = new System.Drawing.Size(55, 19);
		this.pkBox.TabIndex = 9;
		this.pkBox.TextAlign = System.Windows.Forms.HorizontalAlignment.Left;
		base.Controls.Add(this.pkBox);
		base.Controls.Add(this.lNom);
		base.Controls.Add(this.cbEnable);
		base.Controls.Add(this.ccEnable);
		base.Controls.Add(this.lType);
		base.Controls.Add(this.cbType);
		base.Controls.Add(this.lTextType);
		base.Name = "BalisePopupForm";
		base.Size = new System.Drawing.Size(176, 114);
		base.ResumeLayout(false);
		base.PerformLayout();
	}
}
