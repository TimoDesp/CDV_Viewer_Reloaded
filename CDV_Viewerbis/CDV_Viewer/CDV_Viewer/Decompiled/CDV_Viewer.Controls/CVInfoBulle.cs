using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace CDV_Viewer.Controls;

public class CVInfoBulle : UserControl
{
	private IContainer components;

	private Label label;

	public string Texte
	{
		get
		{
			return label.Text;
		}
		set
		{
			label.Text = value;
			base.Width = label.Width + 25;
		}
	}

	public CVInfoBulle()
	{
		InitializeComponent();
		base.Visible = false;
		base.Paint += SIGInfoBulle_Paint;
		base.Enabled = false;
		label.Text = "";
	}

	public void Show(string texte)
	{
		Texte = texte;
		base.Visible = true;
	}

	private void SIGInfoBulle_Paint(object sender, PaintEventArgs e)
	{
		Graphics graphics = e.Graphics;
		graphics.SmoothingMode = SmoothingMode.HighQuality;
		Point[] points = new Point[3]
		{
			new Point(5, base.Height - 5),
			new Point(40, base.Height / 2),
			new Point(20, base.Height / 2)
		};
		Pen pen = new Pen(Color.DarkGray);
		SolidBrush brush = new SolidBrush(Color.White);
		RoundedRectangle roundedRectangle = new RoundedRectangle(10, 10, base.Width - 20, base.Height - 20, RoundedCorner.All, 7f);
		graphics.DrawPolygon(pen, points);
		graphics.FillPath(brush, roundedRectangle.ToGraphicsPath());
		graphics.DrawPath(pen, roundedRectangle.ToGraphicsPath());
		graphics.FillPolygon(brush, points);
		graphics.SmoothingMode = SmoothingMode.Default;
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
		this.label = new System.Windows.Forms.Label();
		base.SuspendLayout();
		this.label.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.label.AutoSize = true;
		this.label.Font = new System.Drawing.Font("Calibri", 8.25f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		this.label.ForeColor = System.Drawing.Color.Black;
		this.label.Location = new System.Drawing.Point(13, 13);
		this.label.Name = "label";
		this.label.Size = new System.Drawing.Size(56, 13);
		this.label.TabIndex = 0;
		this.label.Text = "Test               ";
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		this.BackColor = System.Drawing.Color.Transparent;
		base.Controls.Add(this.label);
		this.DoubleBuffered = true;
		base.Name = "SIGInfoBulle";
		base.Size = new System.Drawing.Size(200, 40);
		base.ResumeLayout(false);
		base.PerformLayout();
	}
}
