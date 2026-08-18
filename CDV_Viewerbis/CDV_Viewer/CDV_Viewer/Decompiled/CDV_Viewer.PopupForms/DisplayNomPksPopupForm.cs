using System.Drawing;
using System.Windows.Forms;
using CDV_Viewer.Controls;
using CDV_Viewer.Traitements;

namespace CDV_Viewer.PopupForms;

public class DisplayNomPksPopupForm : PopupForm
{
	private Label lPkD;

	private Label lPkF;

	private int _pkd;

	private Label lNom;

	private int _pkf;

	public string Nom
	{
		get
		{
			return lNom.Text.Trim();
		}
		set
		{
			lNom.Text = value.Trim();
		}
	}

	public int PkD
	{
		get
		{
			return _pkd;
		}
		set
		{
			_pkd = value;
			lPkD.Text = Chaines.PkToString(_pkd);
		}
	}

	public int PkF
	{
		get
		{
			return _pkf;
		}
		set
		{
			_pkf = value;
			lPkF.Text = Chaines.PkToString(_pkf);
		}
	}

	public DisplayNomPksPopupForm()
	{
		InitializeComponent();
	}

	private void InitializeComponent()
	{
		this.lPkF = new System.Windows.Forms.Label();
		this.lPkD = new System.Windows.Forms.Label();
		this.lNom = new System.Windows.Forms.Label();
		base.SuspendLayout();
		this.lPkF.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
		this.lPkF.AutoSize = true;
		this.lPkF.Location = new System.Drawing.Point(67, 23);
		this.lPkF.Name = "lPkF";
		this.lPkF.Size = new System.Drawing.Size(49, 13);
		this.lPkF.TabIndex = 20;
		this.lPkF.Text = "000+000";
		this.lPkF.TextAlign = System.Drawing.ContentAlignment.TopRight;
		this.lPkD.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.lPkD.Location = new System.Drawing.Point(4, 23);
		this.lPkD.Name = "lPkD";
		this.lPkD.Size = new System.Drawing.Size(60, 13);
		this.lPkD.TabIndex = 19;
		this.lPkD.Text = "000+000";
		this.lNom.Dock = System.Windows.Forms.DockStyle.Top;
		this.lNom.Font = new System.Drawing.Font("Arial", 9.75f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		this.lNom.ForeColor = System.Drawing.Color.FromArgb(224, 82, 6);
		this.lNom.Location = new System.Drawing.Point(0, 0);
		this.lNom.Name = "lNom";
		this.lNom.Size = new System.Drawing.Size(120, 16);
		this.lNom.TabIndex = 22;
		this.lNom.Text = "JOINT";
		this.lNom.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
		base.Controls.Add(this.lNom);
		base.Controls.Add(this.lPkF);
		base.Controls.Add(this.lPkD);
		base.Name = "DisplayNomPksPopupForm";
		base.Size = new System.Drawing.Size(120, 48);
		base.ResumeLayout(false);
		base.PerformLayout();
	}
}
