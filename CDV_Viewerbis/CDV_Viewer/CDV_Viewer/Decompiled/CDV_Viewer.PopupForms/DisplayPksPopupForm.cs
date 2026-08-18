using System.Drawing;
using System.Windows.Forms;
using CDV_Viewer.Controls;
using CDV_Viewer.Traitements;

namespace CDV_Viewer.PopupForms;

public class DisplayPksPopupForm : PopupForm
{
	private Label lPkD;

	private Label lPkF;

	private int _pkd;

	private int _pkf;

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
			lPkD.Text = Chaines.PkToString(_pkf);
		}
	}

	public DisplayPksPopupForm()
	{
		InitializeComponent();
	}

	private void InitializeComponent()
	{
		this.lPkF = new System.Windows.Forms.Label();
		this.lPkD = new System.Windows.Forms.Label();
		base.SuspendLayout();
		this.lPkF.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
		this.lPkF.AutoSize = true;
		this.lPkF.Location = new System.Drawing.Point(73, 7);
		this.lPkF.Name = "lPkF";
		this.lPkF.Size = new System.Drawing.Size(49, 13);
		this.lPkF.TabIndex = 20;
		this.lPkF.Text = "000+000";
		this.lPkF.TextAlign = System.Drawing.ContentAlignment.TopRight;
		this.lPkD.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.lPkD.Location = new System.Drawing.Point(4, 7);
		this.lPkD.Name = "lPkD";
		this.lPkD.Size = new System.Drawing.Size(60, 13);
		this.lPkD.TabIndex = 19;
		this.lPkD.Text = "000+000";
		base.Controls.Add(this.lPkF);
		base.Controls.Add(this.lPkD);
		base.Name = "DisplayPksPopupForm";
		base.Size = new System.Drawing.Size(126, 25);
		base.ResumeLayout(false);
		base.PerformLayout();
	}
}
