using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using CDV_Viewer.Data;

namespace CDV_Viewer.Controls;

public class FlecheButton : Button
{
	private MoveOrientation _orientation;

	private const int FLECHE_SIZE = 16;

	public MoveOrientation Orientation
	{
		get
		{
			return _orientation;
		}
		set
		{
			_orientation = value;
			Invalidate();
		}
	}

	public FlecheButton()
	{
		Cursor = Cursors.Hand;
	}

	protected override void OnPaint(PaintEventArgs pevent)
	{
		pevent.Graphics.Clear(BackColor);
		Pen pen = ((!base.Enabled) ? new Pen(Color.Gainsboro, 3f) : new Pen(Color.Gray, 3f));
		pevent.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
		Point point = new Point(base.Width / 2, base.Height / 2);
		Point point2 = Point.Empty;
		Point point3 = Point.Empty;
		Point point4 = Point.Empty;
		switch (Orientation)
		{
		case MoveOrientation.E:
			point2 = new Point(point.X - 4, point.Y - 8);
			point3 = new Point(point.X + 4, point.Y);
			point4 = new Point(point.X - 4, point.Y + 8);
			break;
		case MoveOrientation.NE:
			point2 = new Point(point.X - 5, point.Y - 5);
			point3 = new Point(point.X + 5, point.Y - 5);
			point4 = new Point(point.X + 5, point.Y + 5);
			break;
		case MoveOrientation.NW:
			point2 = new Point(point.X + 5, point.Y - 5);
			point3 = new Point(point.X - 5, point.Y - 5);
			point4 = new Point(point.X - 5, point.Y + 5);
			break;
		case MoveOrientation.SE:
			point2 = new Point(point.X - 5, point.Y + 5);
			point3 = new Point(point.X + 5, point.Y + 5);
			point4 = new Point(point.X + 5, point.Y - 5);
			break;
		case MoveOrientation.SW:
			point2 = new Point(point.X + 5, point.Y + 5);
			point3 = new Point(point.X - 5, point.Y + 5);
			point4 = new Point(point.X - 5, point.Y - 5);
			break;
		case MoveOrientation.W:
			point2 = new Point(point.X + 4, point.Y - 8);
			point3 = new Point(point.X - 4, point.Y);
			point4 = new Point(point.X + 4, point.Y + 8);
			break;
		}
		pevent.Graphics.DrawCurve(pen, new Point[3] { point2, point3, point4 }, 0f);
		pevent.Graphics.SmoothingMode = SmoothingMode.Default;
	}

	protected override void OnMouseEnter(EventArgs e)
	{
		if (base.Enabled)
		{
			BackColor = Color.Gainsboro;
			Invalidate();
		}
	}

	protected override void OnMouseLeave(EventArgs e)
	{
		BackColor = Color.White;
		Invalidate();
	}
}
