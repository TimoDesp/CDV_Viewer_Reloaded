using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace CDV_Viewer.Controls;

public class CVProgressBar : ProgressBar
{
	protected override void OnPaintBackground(PaintEventArgs pevent)
	{
		Graphics graphics = pevent.Graphics;
		graphics.SmoothingMode = SmoothingMode.HighQuality;
		Pen pen = new Pen(Color.LightGray);
		SolidBrush brush = new SolidBrush(Color.LightGray);
		RoundedRectangle roundedRectangle = new RoundedRectangle(0, 0, base.Width * base.Maximum / base.Value, base.Height, RoundedCorner.All, 7f);
		graphics.FillPath(brush, roundedRectangle.ToGraphicsPath());
		graphics.DrawPath(pen, roundedRectangle.ToGraphicsPath());
		graphics.SmoothingMode = SmoothingMode.Default;
	}

	protected override void OnPaint(PaintEventArgs e)
	{
	}
}
