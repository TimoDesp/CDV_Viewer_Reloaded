using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace CDV_Viewer.Controls;

public class CustomScrollBar : Control
{
	private int _value;

	private int _largeChange = 10;

	private int _smallChange = 10;

	private int _minimum = 100;

	private int _maximum = 100;

	private bool _mouseOn;

	private bool _mouseDown;

	private Point _diffOnClick = Point.Empty;

	private ScrollBarOrientation _orientation;

	private Color _backHoverColor = Color.Gainsboro;

	private Color _foreSelectedColor = Color.LightGray;

	public int Value
	{
		get
		{
			return _value;
		}
		set
		{
			if (value <= Maximum && value >= Minimum)
			{
				_value = value;
				Invalidate();
			}
		}
	}

	public int LargeChange
	{
		get
		{
			return _largeChange;
		}
		set
		{
			_largeChange = value;
			Invalidate();
		}
	}

	public int SmallChange
	{
		get
		{
			return _smallChange;
		}
		set
		{
			_smallChange = value;
			Invalidate();
		}
	}

	public int Minimum
	{
		get
		{
			return _minimum;
		}
		set
		{
			if (_value < value)
			{
				_value = value;
			}
			_minimum = value;
			Invalidate();
		}
	}

	public int Maximum
	{
		get
		{
			return _maximum;
		}
		set
		{
			if (_value > value)
			{
				_value = value;
			}
			_maximum = value;
			Invalidate();
		}
	}

	public ScrollBarOrientation Orientation
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

	private Rectangle _scrollBarRect
	{
		get
		{
			if (_orientation == ScrollBarOrientation.Vertical)
			{
				int num = LargeChange * base.Height / Maximum;
				return new Rectangle(base.Margin.Left, Value * (base.Height - num) / Maximum, base.Width - base.Margin.Horizontal - 1, num - 1);
			}
			int num2 = LargeChange * base.Width / Maximum;
			return new Rectangle(Value * (base.Width - num2) / Maximum, base.Margin.Top, num2 - 1, base.Height - base.Margin.Vertical - 1);
		}
	}

	public Color BackHoverColor
	{
		get
		{
			return _backHoverColor;
		}
		set
		{
			_backHoverColor = value;
			Invalidate();
		}
	}

	public Color ForeSelectedColor
	{
		get
		{
			return _foreSelectedColor;
		}
		set
		{
			_foreSelectedColor = value;
			Invalidate();
		}
	}

	public event ScrollEventHandler Scroll;

	public CustomScrollBar()
	{
		InitializeComponent();
		base.ResizeRedraw = true;
		DoubleBuffered = true;
		SetStyle(ControlStyles.UserPaint | ControlStyles.UserMouse | ControlStyles.AllPaintingInWmPaint, value: true);
	}

	public CustomScrollBar(ScrollBarOrientation orientation)
	{
		_orientation = orientation;
		InitializeComponent();
		base.ResizeRedraw = true;
		DoubleBuffered = true;
		SetStyle(ControlStyles.UserPaint | ControlStyles.UserMouse | ControlStyles.AllPaintingInWmPaint, value: true);
		if (_orientation == ScrollBarOrientation.Vertical)
		{
			base.Width = 16;
		}
		else
		{
			base.Height = 16;
		}
	}

	public CustomScrollBar(IContainer container)
	{
		container.Add(this);
		InitializeComponent();
		base.ResizeRedraw = true;
		DoubleBuffered = true;
		SetStyle(ControlStyles.UserPaint | ControlStyles.UserMouse | ControlStyles.AllPaintingInWmPaint, value: true);
	}

	protected override void OnPaint(PaintEventArgs e)
	{
		if (_mouseOn)
		{
			e.Graphics.Clear(_backHoverColor);
		}
		else
		{
			e.Graphics.Clear(BackColor);
		}
		SolidBrush brush = new SolidBrush(ForeColor);
		if (_mouseDown)
		{
			brush = new SolidBrush(_foreSelectedColor);
		}
		e.Graphics.FillRectangle(brush, _scrollBarRect);
	}

	protected override void OnMouseDown(MouseEventArgs e)
	{
		if (_scrollBarRect.Contains(e.Location))
		{
			_mouseDown = true;
			_diffOnClick = new Point(e.X - _scrollBarRect.X, e.Y - _scrollBarRect.Y);
		}
		Invalidate();
	}

	protected override void OnMouseUp(MouseEventArgs e)
	{
		_mouseDown = false;
		Invalidate();
	}

	protected override void OnMouseMove(MouseEventArgs e)
	{
		if (!_mouseDown)
		{
			return;
		}
		int newValue = Value;
		if (_orientation == ScrollBarOrientation.Vertical)
		{
			if (base.Height > _scrollBarRect.Height + 1)
			{
				int num = Math.Min(base.Height - _scrollBarRect.Height, Math.Max(0, e.Y - _diffOnClick.Y));
				newValue = Math.Min(Maximum, num * Maximum / (base.Height - _scrollBarRect.Height));
			}
		}
		else if (base.Width > _scrollBarRect.Width + 1)
		{
			int num2 = Math.Min(base.Width - _scrollBarRect.Width, Math.Max(0, e.X - _diffOnClick.X));
			newValue = Math.Min(Maximum, num2 * Maximum / (base.Width - _scrollBarRect.Width));
		}
		SetValue(newValue, ScrollEventType.ThumbPosition);
	}

	protected override void OnMouseWheel(MouseEventArgs e)
	{
		if (e.Delta > 0)
		{
			SetValue(Value - Math.Min(Value, SmallChange), ScrollEventType.SmallDecrement);
		}
		else
		{
			SetValue(Value + Math.Min(Maximum - LargeChange - Value, SmallChange), ScrollEventType.SmallIncrement);
		}
	}

	protected override void OnMouseClick(MouseEventArgs e)
	{
		if (_orientation == ScrollBarOrientation.Vertical)
		{
			if (e.Y < _scrollBarRect.Top)
			{
				SetValue(Math.Max(0, Value - LargeChange), ScrollEventType.LargeDecrement);
			}
			else if (e.Y > _scrollBarRect.Bottom)
			{
				SetValue(Math.Min(Maximum, Value + LargeChange), ScrollEventType.LargeIncrement);
			}
		}
		else if (e.X < _scrollBarRect.Left)
		{
			SetValue(Math.Max(0, Value - LargeChange), ScrollEventType.LargeDecrement);
		}
		else if (e.X > _scrollBarRect.Right)
		{
			SetValue(Math.Min(Maximum, Value + LargeChange), ScrollEventType.LargeIncrement);
		}
	}

	protected override void OnMouseEnter(EventArgs e)
	{
		_mouseOn = true;
		Invalidate();
	}

	protected override void OnMouseLeave(EventArgs e)
	{
		_mouseOn = false;
		Invalidate();
	}

	private void InitializeComponent()
	{
		this.DoubleBuffered = true;
		this.Cursor = System.Windows.Forms.Cursors.Hand;
	}

	private void SetValue(int newValue, ScrollEventType type)
	{
		if (newValue != Value)
		{
			Value = newValue;
			Refresh();
			this.Scroll?.Invoke(this, new ScrollEventArgs(type, newValue));
		}
	}
}
