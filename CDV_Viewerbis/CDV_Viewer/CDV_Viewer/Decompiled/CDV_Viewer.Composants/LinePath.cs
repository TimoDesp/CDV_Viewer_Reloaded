using System;
using System.Collections.Generic;
using System.Drawing;

namespace CDV_Viewer.Composants;

public class LinePath
{
	private List<Line> _items = new List<Line>();

	private bool _isEmpty = true;

	private Point _first = Point.Empty;

	private Point _last = Point.Empty;

	private int _xmin = int.MaxValue;

	private int _xmax = int.MinValue;

	private int _ymin = int.MaxValue;

	private int _ymax = int.MinValue;

	public Point First => _first;

	public Point Last => _last;

	public bool IsEmpty => _isEmpty;

	public int FirstY => _first.Y;

	public int LastY => _last.Y;

	public List<Line> Lines => _items;

	public int Y(int x)
	{
		if (_isEmpty)
		{
			return 0;
		}
		if (x < _xmin || x > _xmax)
		{
			return 0;
		}
		if (x == _first.X)
		{
			return _first.Y;
		}
		if (x == _last.X)
		{
			return _last.Y;
		}
		if (_items.Count == 1)
		{
			return _first.Y + (x - _first.X) * (_last.Y - _first.Y) / (_last.X - _first.X);
		}
		foreach (Line item in _items)
		{
			if (item.P0.X <= x && x < item.P1.X)
			{
				return item.Y(x);
			}
		}
		return 0;
	}

	public int Y(int x, out double angle)
	{
		angle = 0.0;
		if (_isEmpty)
		{
			return 0;
		}
		if (x < _xmin || x > _xmax)
		{
			return 0;
		}
		if (_items.Count == 1)
		{
			return _items[0].Y(x, out angle);
		}
		foreach (Line item in _items)
		{
			if (item.P0.X <= x && x <= item.P1.X)
			{
				return item.Y(x, out angle);
			}
		}
		return 0;
	}

	public bool Contains(Point p, int ecarty = 4)
	{
		if (_isEmpty)
		{
			return false;
		}
		if (p.X < _xmin || p.X > _xmax)
		{
			return false;
		}
		return _items.Exists((Line l) => l.Contains(p, ecarty));
	}

	public void Draw(Graphics g, Pen pen)
	{
		_items.ForEach(delegate(Line l)
		{
			g.DrawLine(pen, l.P0, l.P1);
		});
	}

	public void Fill(Graphics g, Brush b)
	{
		if (_items == null || _items.Count == 0)
		{
			return;
		}
		List<Point> list = new List<Point>(_items.Count + 1);
		Point p = _items[0].P0;
		list.Add(p);
		for (int i = 1; i < _items.Count; i++)
		{
			Point p2 = _items[i - 1].P1;
			Point p3 = _items[i].P0;
			if (p3 != p2)
			{
				list.Add(p2);
			}
			list.Add(p3);
			p = p3;
		}
		g.FillPolygon(b, list.ToArray());
	}

	public void FromShape(Shape shape)
	{
		Point[] displayedPoints = shape.DisplayedPoints;
		if (displayedPoints.Length == 0)
		{
			Clear();
			return;
		}
		_items.Clear();
		_isEmpty = false;
		_first = (_last = displayedPoints[0]);
		_xmin = (_xmax = _first.X);
		_ymin = (_ymax = _first.Y);
		if (displayedPoints.Length == 1)
		{
			_items.Add(new Line(_first, _first));
			return;
		}
		Point p = _first;
		for (int i = 1; i < displayedPoints.Length; i++)
		{
			_last = displayedPoints[i];
			if (_last.X > _xmax)
			{
				_xmax = _last.X;
			}
			else if (_last.X < _xmin)
			{
				_xmin = _last.X;
			}
			if (_last.Y > _ymax)
			{
				_ymax = _last.Y;
			}
			else if (_last.Y < _ymin)
			{
				_ymin = _last.Y;
			}
			_items.Add(new Line(p, _last));
			p = _last;
		}
	}

	public void FromPointArray(params Point[] array)
	{
		if (array.Length == 0)
		{
			Clear();
			return;
		}
		_items.Clear();
		_isEmpty = false;
		_first = (_last = array[0]);
		_xmin = (_xmax = _first.X);
		_ymin = (_ymax = _first.Y);
		if (array.Length == 1)
		{
			_items.Add(new Line(_first, _first));
			return;
		}
		Point p = _first;
		for (int i = 1; i < array.Length; i++)
		{
			_last = array[i];
			if (_last.X > _xmax)
			{
				_xmax = _last.X;
			}
			else if (_last.X < _xmin)
			{
				_xmin = _last.X;
			}
			if (_last.Y > _ymax)
			{
				_ymax = _last.Y;
			}
			else if (_last.Y < _ymin)
			{
				_ymin = _last.Y;
			}
			_items.Add(new Line(p, _last));
			p = _last;
		}
	}

	public void FromCSegments(IList<CSegment> segments)
	{
		int count = segments.Count;
		if (count == 0)
		{
			Clear();
			return;
		}
		Point[] path = segments[0].Path;
		FromPointArray(path);
		if (count == 1)
		{
			return;
		}
		for (int i = 1; i < count; i++)
		{
			Point[] path2 = segments[i].Path;
			if (path2.Length != 0)
			{
				Point p = path2[0];
				for (int j = 1; j < path2.Length; j++)
				{
					Point point = path2[j];
					Add(new Line(p, point));
					p = point;
				}
			}
		}
	}

	public void FromPoints(Point p0, Point p1)
	{
		FromLine(new Line(p0, p1));
	}

	public void FromLine(Line l)
	{
		_items.Clear();
		_isEmpty = false;
		_first = l.P0;
		_last = l.P1;
		_xmin = (_xmax = _first.X);
		_ymin = (_ymax = _first.Y);
		if (_last.X > _xmax)
		{
			_xmax = _last.X;
		}
		else if (_last.X < _xmin)
		{
			_xmin = _last.X;
		}
		if (_last.Y > _ymax)
		{
			_ymax = _last.Y;
		}
		else if (_last.Y < _ymin)
		{
			_ymin = _last.Y;
		}
		_items.Add(l);
	}

	public void Add(Line l)
	{
		if (_isEmpty)
		{
			FromLine(l);
			return;
		}
		Point p = l.P0;
		if (p.X > _xmax)
		{
			_xmax = p.X;
		}
		else if (p.X < _xmin)
		{
			_xmin = p.X;
		}
		if (p.Y > _ymax)
		{
			_ymax = p.Y;
		}
		else if (p.Y < _ymin)
		{
			_ymin = p.Y;
		}
		_last = l.P1;
		if (_last.X > _xmax)
		{
			_xmax = _last.X;
		}
		else if (_last.X < _xmin)
		{
			_xmin = _last.X;
		}
		if (_last.Y > _ymax)
		{
			_ymax = _last.Y;
		}
		else if (_last.Y < _ymin)
		{
			_ymin = _last.Y;
		}
		_items.Add(l);
	}

	public void FromRectangle(Rectangle r)
	{
		_items.Clear();
		_isEmpty = false;
		_xmin = Math.Min(r.Left, r.Right);
		_ymin = Math.Min(r.Top, r.Bottom);
		_xmax = _xmin + Math.Abs(r.Width);
		_ymax = _ymin + Math.Abs(r.Height);
		Point point = new Point(_xmin, _ymin);
		Point point2 = new Point(_xmin, _ymax);
		Point point3 = new Point(_xmax, _ymin);
		Point point4 = new Point(_xmax, _ymax);
		_items.Add(new Line(point, point2));
		_items.Add(new Line(point, point3));
		_items.Add(new Line(point2, point4));
		_items.Add(new Line(point3, point4));
		_first = point;
		_last = point4;
	}

	public Rectangle MaxBounds()
	{
		return new Rectangle(_xmin, _ymin, _xmax - _xmin, _ymax - _ymin);
	}

	public Rectangle MaxBounds(int minWidth, int minHeight)
	{
		int x = _xmin;
		int y = _ymin;
		int num = _xmax - _xmin;
		int num2 = _ymax - _ymin;
		if (num < minWidth)
		{
			x = (_xmax + _xmin - (num = minWidth)) / 2;
		}
		if (num2 < minHeight)
		{
			y = (_ymax + _ymin - (num2 = minHeight)) / 2;
		}
		return new Rectangle(x, y, num, num2);
	}

	public void Clear()
	{
		_isEmpty = true;
		_xmin = int.MaxValue;
		_xmax = int.MinValue;
		_ymin = int.MaxValue;
		_ymax = int.MinValue;
		_first = Point.Empty;
		_last = Point.Empty;
		_items.Clear();
	}

	internal Point[] ToPoints(int x0, int x1)
	{
		List<Point> list = new List<Point>();
		Point item = new Point(x0, Y(x0));
		list.Add(item);
		foreach (Line item3 in _items)
		{
			if (item3.P0.X > x0 && item3.P0.X < x1)
			{
				list.Add(item3.P0);
			}
		}
		if (_last.X < x1)
		{
			list.Add(_last);
		}
		Point item2 = new Point(x1, Y(x1));
		list.Add(item2);
		return list.ToArray();
	}

	public static Rectangle MaxBounds(Point[] t)
	{
		if (t == null || t.Length == 0)
		{
			return Rectangle.Empty;
		}
		Point point = t[0];
		int x = point.X;
		int num = x;
		int num2 = point.Y;
		int num3 = num2;
		for (int i = 1; i < t.Length; i++)
		{
			point = t[i];
			if (point.X < x)
			{
				x = point.X;
			}
			else if (point.X > num)
			{
				num = point.X;
			}
			if (point.Y < num2)
			{
				num2 = point.X;
			}
			else if (point.Y > num3)
			{
				num3 = point.X;
			}
		}
		return new Rectangle(x, x, num - x, num3 - num2);
	}

	public static Rectangle MaxBounds(Point[] t, int minWidth, int minHeight)
	{
		if (t == null || t.Length == 0)
		{
			return Rectangle.Empty;
		}
		Point point = t[0];
		int num = point.X;
		int num2 = num;
		int num3 = point.Y;
		int num4 = num3;
		for (int i = 1; i < t.Length; i++)
		{
			point = t[i];
			if (point.X < num)
			{
				num = point.X;
			}
			else if (point.X > num2)
			{
				num2 = point.X;
			}
			if (point.Y < num3)
			{
				num3 = point.X;
			}
			else if (point.Y > num4)
			{
				num4 = point.X;
			}
		}
		int num5 = num2 - num;
		int num6 = num4 - num3;
		if (num5 < minWidth)
		{
			num = (num2 + num - (num5 = minWidth)) / 2;
		}
		if (num6 < minHeight)
		{
			num3 = (num4 + num3 - (num6 = minHeight)) / 2;
		}
		return new Rectangle(num, num, num5, num6);
	}
}
