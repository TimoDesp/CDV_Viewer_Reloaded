using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using CDV_Viewer.Properties;

namespace CDV_Viewer.Controls;

public class CheckControl : UserControl
{
	private bool _checked;

	private IContainer components;

	private PictureBox pictureBox;

	private Label label;

	[DefaultValue("")]
	[EditorBrowsable(EditorBrowsableState.Always)]
	[Browsable(true)]
	public string Texte
	{
		get
		{
			return label.Text;
		}
		set
		{
			label.Text = value;
			base.Width = label.Left + label.Width;
		}
	}

	[DefaultValue(true)]
	[EditorBrowsable(EditorBrowsableState.Always)]
	[Browsable(true)]
	public bool Checked
	{
		get
		{
			return _checked;
		}
		set
		{
			_checked = value;
			RefreshImage();
		}
	}

	public CheckControl()
	{
		InitializeComponent();
	}

	private void RefreshImage()
	{
		if (_checked)
		{
			pictureBox.Image = Resources.Vrai;
		}
		else
		{
			pictureBox.Image = Resources.Faux;
		}
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
		this.pictureBox = new System.Windows.Forms.PictureBox();
		this.label = new System.Windows.Forms.Label();
		((System.ComponentModel.ISupportInitialize)this.pictureBox).BeginInit();
		base.SuspendLayout();
		this.pictureBox.Location = new System.Drawing.Point(0, 0);
		this.pictureBox.Name = "pictureBox";
		this.pictureBox.Size = new System.Drawing.Size(16, 16);
		this.pictureBox.TabIndex = 0;
		this.pictureBox.TabStop = false;
		this.label.AutoSize = true;
		this.label.Location = new System.Drawing.Point(20, 1);
		this.label.Name = "label";
		this.label.Size = new System.Drawing.Size(56, 13);
		this.label.TabIndex = 1;
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.Controls.Add(this.label);
		base.Controls.Add(this.pictureBox);
		base.Name = "SIGCheckControl";
		base.Size = new System.Drawing.Size(80, 16);
		((System.ComponentModel.ISupportInitialize)this.pictureBox).EndInit();
		base.ResumeLayout(false);
		base.PerformLayout();
	}
}
