using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace CDV_Viewer.CustomControls;

public class CustomVScrollBar : UserControl
{
	private CustomControlColor _couleur = CustomControlColor.Gris;

	private int _value;

	private int _minimum;

	private int _maximum = 100;

	private int _smallChange = 10;

	private int _largeChange = 20;

	private int _buttonHeight = 16;

	private bool _mouseDown;

	private int _yMouse;

	private bool _mouseOnBar;

	private Timer _timer = new Timer();

	public CustomControlColor Couleur
	{
		get
		{
			return _couleur;
		}
		set
		{
			_couleur = value;
			Invalidate();
		}
	}

	[Browsable(true)]
	[Category("Behavior")]
	[DefaultValue(false)]
	[Description("Valeur")]
	[EditorBrowsable(EditorBrowsableState.Always)]
	public int Value
	{
		get
		{
			return _value;
		}
		set
		{
			_value = value;
		}
	}

	[Browsable(true)]
	[Category("Behavior")]
	[DefaultValue(false)]
	[Description("Minimum")]
	[EditorBrowsable(EditorBrowsableState.Always)]
	public int Minimum
	{
		get
		{
			return _minimum;
		}
		set
		{
			_minimum = value;
		}
	}

	[Browsable(true)]
	[Category("Behavior")]
	[DefaultValue(false)]
	[Description("Maximum")]
	[EditorBrowsable(EditorBrowsableState.Always)]
	public int Maximum
	{
		get
		{
			return _maximum;
		}
		set
		{
			_maximum = value;
		}
	}

	[Browsable(true)]
	[Category("Behavior")]
	[DefaultValue(false)]
	[Description("SmallChange")]
	[EditorBrowsable(EditorBrowsableState.Always)]
	public int SmallChange
	{
		get
		{
			return _smallChange;
		}
		set
		{
			_smallChange = value;
		}
	}

	[Browsable(true)]
	[Category("Behavior")]
	[DefaultValue(false)]
	[Description("LargeChange")]
	[EditorBrowsable(EditorBrowsableState.Always)]
	public int LargeChange
	{
		get
		{
			return _largeChange;
		}
		set
		{
			_largeChange = value;
		}
	}

	public new event ScrollEventHandler Scroll;

	public CustomVScrollBar()
	{
		Cursor = Cursors.Hand;
		_timer.Interval = 20;
		if (base.Parent != null)
		{
			base.Parent.MouseWheel += Parent_MouseWheel;
		}
		_timer.Tick += Timer_Tick;
	}

	protected override void OnMouseMove(MouseEventArgs e)
	{
		_yMouse = e.Y;
		if (_mouseDown && _mouseOnBar)
		{
			int heightBar = GetHeightBar();
			int val = (e.Y - heightBar / 2 - _buttonHeight) * (Maximum - Minimum) / (base.Height - _buttonHeight * 2 - 1 - heightBar) - Minimum;
			_value = Math.Max(Minimum, Math.Min(Maximum, val));
			Invalidate();
			if (this.Scroll != null)
			{
				this.Scroll(this, new ScrollEventArgs(ScrollEventType.ThumbPosition, _value));
			}
		}
	}

	protected override void OnMouseDown(MouseEventArgs e)
	{
		if (e.Button != MouseButtons.Left)
		{
			return;
		}
		_mouseDown = true;
		if (e.Y <= _buttonHeight)
		{
			DecreaseSmall();
			_timer.Start();
			return;
		}
		if (e.Y >= base.Height - _buttonHeight)
		{
			IncreaseSmall();
			_timer.Start();
			return;
		}
		int heightBar = GetHeightBar();
		int num = (Value - Minimum) * (base.Height - _buttonHeight * 2 - 1 - heightBar) / (Maximum - Minimum) + _buttonHeight;
		if (e.Y < num)
		{
			DecreaseLarge();
		}
		else if (e.Y > num + heightBar)
		{
			IncreaseLarge();
		}
		else
		{
			_mouseOnBar = true;
		}
	}

	protected override void OnMouseUp(MouseEventArgs e)
	{
		if (e.Button == MouseButtons.Left)
		{
			_mouseDown = false;
			_mouseOnBar = false;
		}
	}

	protected override void OnMouseWheel(MouseEventArgs e)
	{
		if (e.Delta > 0)
		{
			DecreaseSmall();
		}
		if (e.Delta < 0)
		{
			IncreaseSmall();
		}
	}

	protected override void OnPaint(PaintEventArgs e)
	{
		if (base.Height >= 32)
		{
			e.Graphics.DrawRectangle(new Pen(_couleur.DarkColor), new Rectangle(0, 0, base.Width - 1, _buttonHeight));
			Point[] points = new Point[3]
			{
				new Point(base.Width / 2 - 3, _buttonHeight / 2 + 2),
				new Point(base.Width / 2 + 5, _buttonHeight / 2 + 2),
				new Point(base.Width / 2 + 1, _buttonHeight / 2 - 2)
			};
			e.Graphics.FillPolygon(new SolidBrush(_couleur.DarkColor), points);
			if (base.Height >= 42)
			{
				int heightBar = GetHeightBar();
				int val = (Value - Minimum) * (base.Height - _buttonHeight * 2 - 1 - heightBar) / (Maximum - Minimum) + _buttonHeight;
				val = Math.Max(val, _buttonHeight);
				heightBar = Math.Min(heightBar, base.Height - _buttonHeight * 2);
				Rectangle rect = new Rectangle(0, val, base.Width - 1, heightBar);
				LinearGradientBrush brush = new LinearGradientBrush(rect, _couleur.LightColor, _couleur.VeryLightColor, 90f);
				e.Graphics.FillRectangle(brush, rect);
				e.Graphics.DrawRectangle(new Pen(_couleur.DarkColor), rect);
			}
			e.Graphics.DrawRectangle(new Pen(_couleur.DarkColor), new Rectangle(0, base.Height - _buttonHeight - 1, base.Width - 1, _buttonHeight));
			points = new Point[3]
			{
				new Point(base.Width / 2 - 3, base.Height - _buttonHeight / 2 - 2),
				new Point(base.Width / 2 + 5, base.Height - _buttonHeight / 2 - 2),
				new Point(base.Width / 2 + 1, base.Height - _buttonHeight / 2 + 2)
			};
			e.Graphics.FillPolygon(new SolidBrush(_couleur.DarkColor), points);
		}
	}

	protected override void OnResize(EventArgs e)
	{
		base.OnResize(e);
		Invalidate();
	}

	private void DecreaseSmall()
	{
		Value -= Math.Min(SmallChange, Value - Minimum);
		Invalidate();
		if (this.Scroll != null)
		{
			if (Value == Minimum)
			{
				this.Scroll(this, new ScrollEventArgs(ScrollEventType.First, Value));
			}
			else
			{
				this.Scroll(this, new ScrollEventArgs(ScrollEventType.SmallDecrement, Value));
			}
		}
	}

	private void IncreaseSmall()
	{
		Value += Math.Min(SmallChange, Maximum - Value);
		Invalidate();
		if (this.Scroll != null)
		{
			if (Value == Maximum)
			{
				this.Scroll(this, new ScrollEventArgs(ScrollEventType.Last, Value));
			}
			else
			{
				this.Scroll(this, new ScrollEventArgs(ScrollEventType.SmallIncrement, Value));
			}
		}
	}

	private void DecreaseLarge()
	{
		Value -= Math.Min(LargeChange, Value - Minimum);
		Invalidate();
		if (this.Scroll != null)
		{
			if (Value == Minimum)
			{
				this.Scroll(this, new ScrollEventArgs(ScrollEventType.First, Value));
			}
			else
			{
				this.Scroll(this, new ScrollEventArgs(ScrollEventType.LargeDecrement, Value));
			}
		}
	}

	private void IncreaseLarge()
	{
		Value += Math.Min(LargeChange, Maximum - Value);
		Invalidate();
		if (this.Scroll != null)
		{
			if (Value == Maximum)
			{
				this.Scroll(this, new ScrollEventArgs(ScrollEventType.Last, Value));
			}
			else
			{
				this.Scroll(this, new ScrollEventArgs(ScrollEventType.LargeIncrement, Value));
			}
		}
	}

	private int GetHeightBar()
	{
		return Math.Max(LargeChange * (base.Height - _buttonHeight * 2) / (Maximum - Minimum), 10);
	}

	private void Parent_MouseWheel(object sender, MouseEventArgs e)
	{
		OnMouseWheel(e);
	}

	private void Timer_Tick(object sender, EventArgs e)
	{
		if (_mouseDown)
		{
			if (_yMouse <= _buttonHeight)
			{
				DecreaseSmall();
			}
			if (_yMouse >= base.Height - _buttonHeight)
			{
				IncreaseSmall();
			}
		}
		else
		{
			_timer.Stop();
		}
	}
}
