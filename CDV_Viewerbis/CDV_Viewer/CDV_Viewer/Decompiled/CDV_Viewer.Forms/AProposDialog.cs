using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using CDV_Viewer.Properties;

namespace CDV_Viewer.Forms;

public class AProposDialog : Form
{
	private IContainer components;

	private Label lVersion;

	private Label lDate;

	private Label lCopyright;

	public AProposDialog()
	{
		InitializeComponent();
		string dATE_APP_VERSION = Resources.DATE_APP_VERSION;
		lDate.Text = "Date : " + dATE_APP_VERSION;
		lVersion.Text = "Version : " + Resources.APP_VERSION;
		lCopyright.Text = "Copyright © SNCF " + dATE_APP_VERSION.Substring(dATE_APP_VERSION.Length - 4, 4);
	}

	protected override void OnPaintBackground(PaintEventArgs e)
	{
		e.Graphics.DrawImage(Resources.apropos, DisplayRectangle);
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
		this.lVersion = new System.Windows.Forms.Label();
		this.lDate = new System.Windows.Forms.Label();
		this.lCopyright = new System.Windows.Forms.Label();
		base.SuspendLayout();
		this.lVersion.AutoSize = true;
		this.lVersion.BackColor = System.Drawing.Color.Transparent;
		this.lVersion.Location = new System.Drawing.Point(176, 121);
		this.lVersion.Name = "lVersion";
		this.lVersion.Size = new System.Drawing.Size(48, 13);
		this.lVersion.TabIndex = 0;
		this.lVersion.Text = "Version :";
		this.lVersion.TextAlign = System.Drawing.ContentAlignment.TopRight;
		this.lDate.AutoSize = true;
		this.lDate.BackColor = System.Drawing.Color.Transparent;
		this.lDate.Location = new System.Drawing.Point(176, 147);
		this.lDate.Name = "lDate";
		this.lDate.Size = new System.Drawing.Size(39, 13);
		this.lDate.TabIndex = 1;
		this.lDate.Text = "Date : ";
		this.lDate.TextAlign = System.Drawing.ContentAlignment.TopRight;
		this.lCopyright.AutoSize = true;
		this.lCopyright.BackColor = System.Drawing.Color.Transparent;
		this.lCopyright.Location = new System.Drawing.Point(176, 171);
		this.lCopyright.Name = "lCopyright";
		this.lCopyright.Size = new System.Drawing.Size(94, 13);
		this.lCopyright.TabIndex = 2;
		this.lCopyright.Text = "Copyright © SNCF";
		this.lCopyright.TextAlign = System.Drawing.ContentAlignment.TopRight;
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.ClientSize = new System.Drawing.Size(326, 199);
		base.Controls.Add(this.lCopyright);
		base.Controls.Add(this.lDate);
		base.Controls.Add(this.lVersion);
		base.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
		base.MaximizeBox = false;
		base.MinimizeBox = false;
		base.Name = "AProposDialog";
		base.ShowInTaskbar = false;
		base.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
		this.Text = "À Propos...";
		base.ResumeLayout(false);
		base.PerformLayout();
	}
}
