using System;
using System.Drawing;
using System.Windows.Forms;

namespace CDV_Viewer.Controls;

public class Separator : Control
{
	private Orientation _orientation;

	public Orientation Orientation
	{
		get
		{
			return _orientation;
		}
		set
		{
			_orientation = value;
		}
	}

	public Separator()
	{
		base.Height = 3;
	}

	protected override void OnPaint(PaintEventArgs e)
	{
		if (_orientation == Orientation.Horizontal)
		{
			e.Graphics.DrawLine(new Pen(ForeColor), 0, 1, base.Width, 1);
		}
		else
		{
			e.Graphics.DrawLine(new Pen(ForeColor), 1, 0, 1, base.Height);
		}
	}

	protected override void OnSizeChanged(EventArgs e)
	{
		if (_orientation == Orientation.Horizontal)
		{
			base.Height = 3;
		}
		else
		{
			base.Width = 3;
		}
	}
}
