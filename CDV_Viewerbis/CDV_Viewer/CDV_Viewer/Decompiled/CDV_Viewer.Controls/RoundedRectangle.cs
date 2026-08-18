using System;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace CDV_Viewer.Controls;

public struct RoundedRectangle
{
	private int x;

	private int y;

	private int width;

	private int height;

	private float radius;

	private RoundedCorner roundedCorners;

	public static readonly RoundedRectangle Empty;

	public RoundedCorner RoundedCorners
	{
		get
		{
			return roundedCorners;
		}
		set
		{
			roundedCorners = value;
		}
	}

	public Size Size
	{
		get
		{
			return new Size(width, height);
		}
		set
		{
			width = value.Width;
			height = value.Height;
		}
	}

	public int X
	{
		get
		{
			return x;
		}
		set
		{
			x = value;
		}
	}

	public int Y
	{
		get
		{
			return y;
		}
		set
		{
			y = value;
		}
	}

	public int Width
	{
		get
		{
			return width;
		}
		set
		{
			width = value;
		}
	}

	public int Height
	{
		get
		{
			return height;
		}
		set
		{
			height = value;
		}
	}

	public int Left => x;

	public int Right => x + width;

	public int Top => y;

	public int Bottom => y + height;

	public Point Location
	{
		get
		{
			return new Point(x, y);
		}
		set
		{
			x = value.X;
			y = value.Y;
		}
	}

	public float Radius
	{
		get
		{
			return radius;
		}
		set
		{
			radius = value;
		}
	}

	public RoundedRectangle(int x, int y, int width, int height, RoundedCorner rc, float radius)
	{
		this.x = x;
		this.y = y;
		this.width = width;
		this.height = height;
		this.radius = radius;
		roundedCorners = rc;
	}

	public RoundedRectangle(Rectangle rect, RoundedCorner rc, float radius)
	{
		x = rect.X;
		y = rect.Y;
		width = rect.Width;
		height = rect.Height;
		this.radius = radius;
		roundedCorners = rc;
	}

	public RoundedRectangle(Point location, Size size, RoundedCorner rc, float radius)
	{
		x = location.X;
		y = location.Y;
		width = size.Width;
		height = size.Height;
		this.radius = radius;
		roundedCorners = rc;
	}

	public void Inflate(int width, int height)
	{
		x -= width;
		y -= height;
		this.width += 2 * width;
		this.height += 2 * height;
	}

	public void Inflate(Size size)
	{
		Inflate(size.Width, size.Height);
	}

	public static RoundedRectangle Inflate(RoundedRectangle rrect, int width, int height)
	{
		RoundedRectangle result = rrect;
		result.Inflate(width, height);
		return result;
	}

	public static RoundedRectangle Intersect(RoundedRectangle a, RoundedRectangle b, RoundedCorner rc, float radius)
	{
		int num = Math.Max(a.Left, b.Left);
		int num2 = Math.Min(a.Right, b.Right);
		int num3 = Math.Max(a.Top, b.Top);
		int num4 = Math.Min(a.Bottom, b.Bottom);
		if (num <= num2 && num3 <= num4)
		{
			return new RoundedRectangle(num, num2 - num, num3, num4 - num3, rc, radius);
		}
		return Empty;
	}

	public void Intersect(RoundedRectangle rrect)
	{
		RoundedRectangle roundedRectangle = Intersect(rrect, this, roundedCorners, radius);
		x = roundedRectangle.x;
		y = roundedRectangle.y;
		width = roundedRectangle.width;
		height = roundedRectangle.height;
	}

	public bool IntersectWith(RoundedRectangle rrect)
	{
		if (rrect.Left < Right && Left < rrect.Right)
		{
			if (rrect.Top < Bottom)
			{
				return Top < rrect.Bottom;
			}
			return false;
		}
		return false;
	}

	public static RoundedRectangle Union(RoundedRectangle a, RoundedRectangle b, RoundedCorner rc, float radius)
	{
		int num = Math.Min(a.Left, b.Left);
		int num2 = Math.Max(a.Right, b.Right);
		int num3 = Math.Min(a.Top, b.Top);
		int num4 = Math.Max(a.Bottom, b.Bottom);
		return new RoundedRectangle(num, num2 - num, num3, num4 - num3, rc, radius);
	}

	public void Offset(int x, int y)
	{
		this.x += x;
		this.y += y;
	}

	public void Offset(Point point)
	{
		x += point.X;
		y += point.Y;
	}

	public bool Contains(int x, int y)
	{
		if (this.x <= x && x <= Right && this.y <= y)
		{
			return y <= Bottom;
		}
		return false;
	}

	public bool Contains(Point pt)
	{
		return Contains(pt.X, pt.Y);
	}

	public GraphicsPath ToGraphicsPath()
	{
		GraphicsPath graphicsPath = new GraphicsPath();
		if (radius <= 0f)
		{
			graphicsPath.AddRectangle(new Rectangle(Location, Size));
			graphicsPath.CloseFigure();
			return graphicsPath;
		}
		float num = radius * 2f;
		if (height <= width)
		{
			if (num >= (float)height && ((roundedCorners & (RoundedCorner.TopLeft | RoundedCorner.BottomLeft)) == (RoundedCorner.TopLeft | RoundedCorner.BottomLeft) || (roundedCorners & (RoundedCorner.TopRight | RoundedCorner.BottomRight)) == (RoundedCorner.TopRight | RoundedCorner.BottomRight)))
			{
				num = height;
			}
		}
		else if (num >= (float)width && ((roundedCorners & (RoundedCorner.BottomLeft | RoundedCorner.BottomRight)) == (RoundedCorner.BottomLeft | RoundedCorner.BottomRight) || (roundedCorners & (RoundedCorner.TopLeft | RoundedCorner.TopRight)) == (RoundedCorner.TopLeft | RoundedCorner.TopRight)))
		{
			num = width;
		}
		RectangleF rect = new RectangleF(size: new SizeF(num, num), location: Location);
		if ((roundedCorners & RoundedCorner.TopLeft) == RoundedCorner.TopLeft)
		{
			graphicsPath.AddArc(rect, 180f, 90f);
		}
		else
		{
			graphicsPath.AddLine(new PointF(rect.Left, rect.Top), new PointF(rect.Left, rect.Top));
		}
		rect.X = (float)Right - num;
		if ((roundedCorners & RoundedCorner.TopRight) == RoundedCorner.TopRight)
		{
			graphicsPath.AddArc(rect, 270f, 90f);
		}
		else
		{
			graphicsPath.AddLine(new PointF(rect.Right, rect.Top), new PointF(rect.Right, rect.Top));
		}
		rect.Y = (float)Bottom - num;
		if ((roundedCorners & RoundedCorner.BottomRight) == RoundedCorner.BottomRight)
		{
			graphicsPath.AddArc(rect, 0f, 90f);
		}
		else
		{
			graphicsPath.AddLine(new PointF(rect.Right, rect.Bottom), new PointF(rect.Right, rect.Bottom));
		}
		rect.X = Left;
		if ((roundedCorners & RoundedCorner.BottomLeft) == RoundedCorner.BottomLeft)
		{
			graphicsPath.AddArc(rect, 90f, 90f);
		}
		else
		{
			graphicsPath.AddLine(new PointF(rect.Left, rect.Bottom), new PointF(rect.Left, rect.Bottom));
		}
		graphicsPath.CloseFigure();
		return graphicsPath;
	}

	public Rectangle ToRectangle()
	{
		return new Rectangle(Location, Size);
	}

	public override string ToString()
	{
		return $"{{X={x},Y={y},Width={width},Height={height},Radius={radius}}}";
	}

	public static bool operator ==(RoundedRectangle a, RoundedRectangle b)
	{
		if (a.x == b.x && a.y == b.y && a.width == b.width)
		{
			return a.height == b.height;
		}
		return false;
	}

	public static bool operator !=(RoundedRectangle a, RoundedRectangle b)
	{
		return !(a == b);
	}

	public override bool Equals(object obj)
	{
		bool result = false;
		if (obj is RoundedRectangle roundedRectangle)
		{
			result = roundedRectangle.x == x && roundedRectangle.y == y && roundedRectangle.width == width && roundedRectangle.height == height;
		}
		return result;
	}

	public override int GetHashCode()
	{
		return base.GetHashCode();
	}
}
