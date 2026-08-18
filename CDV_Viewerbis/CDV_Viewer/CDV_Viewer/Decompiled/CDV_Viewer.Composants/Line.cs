using System;
using System.Drawing;

namespace CDV_Viewer.Composants;

public class Line
{
	public readonly Point P0;

	public readonly Point P1;

	public int Y(int x)
	{
		return P0.Y + (x - P0.X) * (P1.Y - P0.Y) / (P1.X - P0.X);
	}

	public int Y(int x, out double angle)
	{
		int num = P1.Y - P0.Y;
		int num2 = P1.X - P0.X;
		angle = ((num == 0) ? 0.0 : (180.0 * Math.Atan2(num, num2) / Math.PI));
		return P0.Y + (x - P0.X) * num / num2;
	}

	public Line(Point p0, Point p1)
	{
		if (p0.X < p1.X)
		{
			P0 = p0;
			P1 = p1;
		}
		else
		{
			P0 = p1;
			P1 = p0;
		}
	}

	public bool Contains(Point p, int ecarty = 4)
	{
		if (p.X < P0.X)
		{
			return false;
		}
		if (p.X > P1.X)
		{
			return false;
		}
		int num = P0.Y;
		if (P1.Y != num)
		{
			if (P1.X - P0.X == 0)
			{
				return Math.Abs((P1.Y + P0.Y) / 2 - p.Y) <= Math.Abs(P1.Y - P0.Y) / 2;
			}
			num = P0.Y + (P1.Y - P0.Y) * (p.X - P0.X) / (P1.X - P0.X);
		}
		_ = p.Y;
		_ = 166;
		if (Math.Abs(p.Y - num) <= ecarty)
		{
			return true;
		}
		return false;
	}

	public void Draw(Graphics g, Pen pen)
	{
		g.DrawLine(pen, P0, P1);
	}

	public Rectangle ToRectangle()
	{
		int x;
		int width;
		if (P1.X < P0.X)
		{
			x = P0.X;
			width = P1.X - x;
		}
		else
		{
			x = P1.X;
			width = P0.X - x;
		}
		if (P1.Y < P0.Y)
		{
			return new Rectangle(x, P1.Y, width, P0.Y - P1.Y);
		}
		return new Rectangle(x, P0.Y, width, P1.Y - P0.Y);
	}
}
