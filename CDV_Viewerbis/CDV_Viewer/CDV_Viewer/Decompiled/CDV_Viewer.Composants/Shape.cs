using System.Drawing;
using System.Drawing.Drawing2D;

namespace CDV_Viewer.Composants;

public class Shape
{
	private Point[] _points;

	private Point[] _tmp;

	private Size _size;

	public Point[] Points => _points;

	public Point[] DisplayedPoints => _tmp;

	public SmoothingMode SmoothingMode { get; set; }

	public Size Size => _size;

	public int Length => _points.Length;

	public Shape(params Point[] points)
	{
		_points = (Point[])points.Clone();
		_tmp = new Point[_points.Length];
		Size empty = Size.Empty;
		if (_points.Length == 0)
		{
			return;
		}
		Point point = points[0];
		int x;
		int num = (x = point.X);
		int y;
		int num2 = (y = point.Y);
		for (int i = 1; i < points.Length; i++)
		{
			point = points[i];
			if (point.X > x)
			{
				x = point.X;
			}
			else if (point.X < num)
			{
				num = point.X;
			}
			if (point.Y > y)
			{
				y = point.Y;
			}
			else if (point.Y < num2)
			{
				num2 = point.Y;
			}
		}
		empty.Width = x - num + 1;
		empty.Height = y - num2 + 1;
	}

	public void Draw(Graphics g, Pen pen, Point location, float scale = 1f)
	{
		if (SetDisplayedPoints(location, scale))
		{
			bool num = SmoothingMode != SmoothingMode.Default;
			if (num)
			{
				g.SmoothingMode = SmoothingMode;
			}
			g.DrawLines(pen, _tmp);
			if (num)
			{
				g.SmoothingMode = SmoothingMode.Default;
			}
		}
	}

	public void DrawCurve(Graphics g, Pen pen, Point location, float scale = 1f)
	{
		if (SetDisplayedPoints(location, scale))
		{
			bool num = SmoothingMode != SmoothingMode.Default;
			if (num)
			{
				g.SmoothingMode = SmoothingMode;
			}
			g.DrawCurve(pen, _tmp);
			if (num)
			{
				g.SmoothingMode = SmoothingMode.Default;
			}
		}
	}

	public void Fill(Graphics g, Brush b, Point location, float scale = 1f)
	{
		if (SetDisplayedPoints(location, scale))
		{
			bool num = SmoothingMode != SmoothingMode.Default;
			if (num)
			{
				g.SmoothingMode = SmoothingMode;
			}
			g.FillPolygon(b, _tmp);
			if (num)
			{
				g.SmoothingMode = SmoothingMode.Default;
			}
		}
	}

	private bool SetDisplayedPoints(Point location, float scale = 1f)
	{
		if (_points.Length < 2)
		{
			return false;
		}
		if (scale == 1f)
		{
			_points.CopyTo(_tmp, 0);
			for (int i = 0; i < _tmp.Length; i++)
			{
				_tmp[i].Offset(location);
			}
		}
		else
		{
			for (int j = 0; j < _points.Length; j++)
			{
				Point point = _points[j];
				_tmp[j] = Point.Round(new PointF((float)point.X * scale, (float)point.Y * scale));
				_tmp[j].Offset(location);
			}
		}
		return true;
	}

	public Size GetSize(float scale = 1f)
	{
		if (scale != 1f)
		{
			return Size.Round(new SizeF((float)_size.Width * scale, (float)_size.Height * scale));
		}
		return _size;
	}
}
