using System;
using System.Drawing;
using System.Windows.Forms;
using CDV_Viewer.Controls;
using CDV_Viewer.CustomControls;
using CDV_Viewer.Properties;

namespace CDV_Viewer.PopupForms;

public class PkPopupForm : PopupForm
{
	private Label label6;

	private ImageButton bValider;

	private ImageButton bAnnuler;

	private CustomTextBox tbPk;

	public int Pk
	{
		get
		{
			if (!int.TryParse(tbPk.Text, out var result))
			{
				return 0;
			}
			if (result > 999999)
			{
				return -1;
			}
			return result;
		}
		set
		{
			tbPk.Text = value.ToString();
		}
	}

	public PkPopupForm()
	{
		InitializeComponent();
		bAnnuler.Click += bAnnuler_Click;
		bValider.Click += bValider_Click;
	}

	private void InitializeComponent()
	{
		this.label6 = new System.Windows.Forms.Label();
		this.tbPk = new CDV_Viewer.CustomControls.CustomTextBox();
		this.bValider = new CDV_Viewer.Controls.ImageButton();
		this.bAnnuler = new CDV_Viewer.Controls.ImageButton();
		base.SuspendLayout();
		this.label6.AutoSize = true;
		this.label6.Location = new System.Drawing.Point(4, 9);
		this.label6.Name = "label6";
		this.label6.Size = new System.Drawing.Size(27, 13);
		this.label6.TabIndex = 16;
		this.label6.Text = "PK :";
		this.tbPk.BackColor = System.Drawing.Color.White;
		this.tbPk.Location = new System.Drawing.Point(33, 6);
		this.tbPk.Name = "tbPk";
		this.tbPk.Padding = new System.Windows.Forms.Padding(1);
		this.tbPk.SelectionLength = 0;
		this.tbPk.SelectionStart = 0;
		this.tbPk.Size = new System.Drawing.Size(44, 20);
		this.tbPk.TabIndex = 15;
		this.tbPk.TextAlign = System.Windows.Forms.HorizontalAlignment.Left;
		this.bValider.Cursor = System.Windows.Forms.Cursors.Hand;
		this.bValider.Image = CDV_Viewer.Properties.Resources.OkButton;
		this.bValider.Location = new System.Drawing.Point(86, 6);
		this.bValider.Name = "bValider";
		this.bValider.Size = new System.Drawing.Size(20, 20);
		this.bValider.TabIndex = 17;
		this.bValider.UseVisualStyleBackColor = true;
		this.bAnnuler.Cursor = System.Windows.Forms.Cursors.Hand;
		this.bAnnuler.Image = CDV_Viewer.Properties.Resources.close;
		this.bAnnuler.Location = new System.Drawing.Point(112, 6);
		this.bAnnuler.Name = "bAnnuler";
		this.bAnnuler.Size = new System.Drawing.Size(20, 20);
		this.bAnnuler.TabIndex = 18;
		this.bAnnuler.UseVisualStyleBackColor = true;
		base.Controls.Add(this.bAnnuler);
		base.Controls.Add(this.bValider);
		base.Controls.Add(this.label6);
		base.Controls.Add(this.tbPk);
		base.Name = "PkPopupForm";
		base.Size = new System.Drawing.Size(138, 32);
		base.ResumeLayout(false);
		base.PerformLayout();
	}

	private void bAnnuler_Click(object sender, EventArgs e)
	{
		Close(PopupFormResultEventArgs.Cancel);
	}

	private void bValider_Click(object sender, EventArgs e)
	{
		if (Pk >= 0)
		{
			Close(PopupFormResultEventArgs.Ok);
		}
	}
}
