using System.Drawing;
using System.Windows.Forms;
using CDV_Viewer.Controls;
using CDV_Viewer.CustomControls;
using CDV_Viewer.Data;
using CDV_Viewer.Traitements;

namespace CDV_Viewer.PopupForms;

public class EditNomPksPopupForm : PopupForm
{
	private CustomTextBox tbNom;

	private Label lTextPkD;

	private CustomTextBox tbPkD;

	private Label lTextPkF;

	private CustomTextBox tbPkF;

	public readonly SimpleEditPopupFormStyle Style;

	public string Nom
	{
		get
		{
			return tbNom.Text;
		}
		set
		{
			tbNom.Text = value;
		}
	}

	public int PkD
	{
		get
		{
			if (Chaines.TryParsePk(tbPkD.Text, out var PK))
			{
				if (PK > 999999)
				{
					return -1000;
				}
				if (PK < -1000)
				{
					return -1000;
				}
				return PK;
			}
			return -1000;
		}
		set
		{
			tbPkD.Text = Chaines.PkToString(value);
		}
	}

	public int PkF
	{
		get
		{
			if (Chaines.TryParsePk(tbPkF.Text, out var PK))
			{
				if (PK > 999999)
				{
					return -1000;
				}
				if (PK < -1000)
				{
					return -1000;
				}
				return PK;
			}
			return -1000;
		}
		set
		{
			tbPkF.Text = Chaines.PkToString(value);
		}
	}

	public EditNomPksPopupForm()
	{
		InitializeComponent();
	}

	private void InitializeComponent()
	{
		this.lTextPkF = new System.Windows.Forms.Label();
		this.tbPkF = new CDV_Viewer.CustomControls.CustomTextBox();
		this.lTextPkD = new System.Windows.Forms.Label();
		this.tbPkD = new CDV_Viewer.CustomControls.CustomTextBox();
		this.tbNom = new CDV_Viewer.CustomControls.CustomTextBox();
		base.SuspendLayout();
		this.lTextPkF.AutoSize = true;
		this.lTextPkF.Location = new System.Drawing.Point(141, 38);
		this.lTextPkF.Name = "lTextPkF";
		this.lTextPkF.Size = new System.Drawing.Size(44, 13);
		this.lTextPkF.TabIndex = 18;
		this.lTextPkF.Text = "PK Fin :";
		this.lTextPkF.TextAlign = System.Drawing.ContentAlignment.TopRight;
		this.tbPkF.BackColor = System.Drawing.Color.White;
		this.tbPkF.Location = new System.Drawing.Point(189, 35);
		this.tbPkF.Name = "tbPkF";
		this.tbPkF.Padding = new System.Windows.Forms.Padding(1);
		this.tbPkF.SelectionLength = 0;
		this.tbPkF.SelectionStart = 0;
		this.tbPkF.Size = new System.Drawing.Size(54, 20);
		this.tbPkF.TabIndex = 17;
		this.tbPkF.TextAlign = System.Windows.Forms.HorizontalAlignment.Left;
		this.lTextPkD.AutoSize = true;
		this.lTextPkD.Location = new System.Drawing.Point(4, 38);
		this.lTextPkD.Name = "lTextPkD";
		this.lTextPkD.Size = new System.Drawing.Size(59, 13);
		this.lTextPkD.TabIndex = 16;
		this.lTextPkD.Text = "PK Debut :";
		this.tbPkD.BackColor = System.Drawing.Color.White;
		this.tbPkD.Location = new System.Drawing.Point(64, 35);
		this.tbPkD.Name = "tbPkD";
		this.tbPkD.Padding = new System.Windows.Forms.Padding(1);
		this.tbPkD.SelectionLength = 0;
		this.tbPkD.SelectionStart = 0;
		this.tbPkD.Size = new System.Drawing.Size(54, 20);
		this.tbPkD.TabIndex = 15;
		this.tbPkD.TextAlign = System.Windows.Forms.HorizontalAlignment.Left;
		this.tbNom.BackColor = System.Drawing.Color.White;
		this.tbNom.Location = new System.Drawing.Point(57, 5);
		this.tbNom.Name = "tbNom";
		this.tbNom.Padding = new System.Windows.Forms.Padding(1);
		this.tbNom.SelectionLength = 0;
		this.tbNom.SelectionStart = 0;
		this.tbNom.Size = new System.Drawing.Size(100, 20);
		this.tbNom.TabIndex = 1;
		this.tbNom.TextAlign = System.Windows.Forms.HorizontalAlignment.Left;
		base.Controls.Add(this.lTextPkF);
		base.Controls.Add(this.tbPkF);
		base.Controls.Add(this.lTextPkD);
		base.Controls.Add(this.tbPkD);
		base.Controls.Add(this.tbNom);
		base.Name = "SimpleEditPopupForm";
		base.Size = new System.Drawing.Size(289, 104);
		base.ResumeLayout(false);
		base.PerformLayout();
	}
}
