using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using CDV_Viewer.Data;
using CDV_Viewer.DockControls;
using CDV_Viewer.Traitements;

namespace CDV_Viewer.Controls;

public class CVEchelle : UserControl
{
	private StringFormat _labelFormat = new StringFormat
	{
		Alignment = StringAlignment.Center
	};

	private Font _labelFont = new Font(Global.DefaultFont.FontFamily, 8f, FontStyle.Bold);

	private Brush _labelBrush = new SolidBrush(Color.Black);

	private IContainer components;

	public bool IsOnTop { get; set; }

	public CVEchelle()
	{
		InitializeComponent();
		base.Load += SIGEchelle_Load;
		base.Paint += SIGEchelle_Paint;
	}

	private void SIGEchelle_Load(object sender, EventArgs e)
	{
	}

	private void SIGEchelle_Paint(object sender, PaintEventArgs e)
	{
		base.Enabled = true;
		ComposantsViewer viewer = ComposantsViewer.Viewer;
		if (viewer == null || viewer.CurrentMargin == 0 || viewer.PkWidth < viewer.CurrentMargin * 2)
		{
			return;
		}
		_ = IsOnTop;
		int num = ComposantsViewer.Viewer.PkD;
		Graphics graphics = e.Graphics;
		int num2 = (int)Math.Round((double)viewer.PkWidth / 10.0);
		int num3 = 1;
		while (num2 / num3 >= 10)
		{
			num3 *= 10;
		}
		num2 = (int)Math.Round(10.0 * (double)num2 / (double)num3);
		if (num2 > 50)
		{
			num2 = 50;
		}
		else if (num2 > 20)
		{
			num2 = 20;
		}
		else if (num2 > 10)
		{
			num2 = 10;
		}
		num2 = num3 * num2;
		int num4 = num2 / 10;
		int num5 = num4 * (int)Math.Ceiling(10.0 * (double)viewer.PkD / (double)num4);
		while (num <= viewer.PkF)
		{
			num = num5 / 10;
			int num6 = ComposantsViewer.Viewer.PkToLocation(num);
			if (num5 % num2 == 0)
			{
				graphics.DrawLine(new Pen(new SolidBrush(Color.Black)), num6, 12, num6, base.Height);
				string s = Chaines.PkToString(num);
				graphics.DrawString(s, _labelFont, _labelBrush, new Rectangle(num6 - 25, 0, 50, 15), _labelFormat);
			}
			else
			{
				graphics.DrawLine(Pens.Black, num6, 17, num6, base.Height);
			}
			num5 += num4;
		}
	}

	protected override void OnMouseClick(MouseEventArgs e)
	{
		int pk = ComposantsViewer.Viewer.LocationToPk(e.X);
		ComposantsViewer.Viewer.MoveToPkCenter(pk);
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
		base.SuspendLayout();
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		this.BackColor = System.Drawing.Color.Transparent;
		this.DoubleBuffered = true;
		base.Name = "SIGEchelle";
		base.Size = new System.Drawing.Size(150, 25);
		base.ResumeLayout(false);
	}
}
