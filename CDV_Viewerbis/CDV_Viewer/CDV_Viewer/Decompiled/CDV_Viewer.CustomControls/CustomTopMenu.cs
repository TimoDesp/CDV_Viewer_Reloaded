using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace CDV_Viewer.CustomControls;

public class CustomTopMenu : UserControl
{
	private IContainer components;

	private Timer timer;

	private CustomVScrollBar scrollBar;

	private CustomControlColor _couleur = CustomControlColor.Gris;

	private CustomTopMenuItemCollection _items = new CustomTopMenuItemCollection();

	public ToolStripItemDisplayStyle DisplayStyle = ToolStripItemDisplayStyle.Text;

	public int MinimizeHeight = 25;

	public int ItemHeight = 20;

	private int _nextHeight;

	private int _contentMargin = 5;

	private int _imageMargin = 5;

	private bool _isSlideUp;

	private bool _isSlideDown;

	private bool _isMinimize;

	private int _selectedIndex = -1;

	private bool _mouseDown;

	private int _alpha;

	public CustomControlColor Couleur
	{
		get
		{
			return _couleur;
		}
		set
		{
			_couleur = value;
			scrollBar.Couleur = _couleur;
			Invalidate();
		}
	}

	public CustomTopMenuItemCollection Items => _items;

	public int NextHeight
	{
		get
		{
			return _nextHeight;
		}
		set
		{
			_nextHeight = value;
		}
	}

	public event CustomTopMenuItemEventHandler SelectItem;

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
		this.components = new System.ComponentModel.Container();
		this.timer = new System.Windows.Forms.Timer(this.components);
		this.scrollBar = new CDV_Viewer.CustomControls.CustomVScrollBar();
		base.SuspendLayout();
		this.timer.Interval = 20;
		this.scrollBar.Cursor = System.Windows.Forms.Cursors.Hand;
		this.scrollBar.Dock = System.Windows.Forms.DockStyle.Right;
		this.scrollBar.LargeChange = 20;
		this.scrollBar.Location = new System.Drawing.Point(133, 0);
		this.scrollBar.Maximum = 100;
		this.scrollBar.Minimum = 0;
		this.scrollBar.Name = "scrollBar";
		this.scrollBar.Size = new System.Drawing.Size(17, 150);
		this.scrollBar.SmallChange = 2;
		this.scrollBar.TabIndex = 0;
		this.scrollBar.Value = 0;
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		this.BackColor = System.Drawing.Color.White;
		base.Controls.Add(this.scrollBar);
		this.Cursor = System.Windows.Forms.Cursors.Hand;
		this.DoubleBuffered = true;
		base.Name = "CustomTopMenu";
		base.ResumeLayout(false);
	}

	public CustomTopMenu()
	{
		InitializeComponent();
		base.MouseMove += CustomListBox_MouseMove;
		base.MouseDown += CustomListBox_MouseDown;
		base.MouseUp += CustomListBox_MouseUp;
		base.MouseLeave += CustomListBox_MouseLeave;
		base.Resize += CustomListBox_Resize;
		Items.CollectionChanged += CustomListBox_Resize;
		scrollBar.Scroll += scrollBar_Scroll;
		timer.Tick += timer_Tick;
	}

	protected override void OnPaint(PaintEventArgs e)
	{
		if (_isMinimize)
		{
			int num = (int)((double)MinimizeHeight / 1.5);
			int num2 = num / 2;
			Point[] array = new Point[3]
			{
				new Point((base.Width - num) / 2, (base.Height - num2) / 2),
				new Point((base.Width + num) / 2, (base.Height - num2) / 2),
				new Point(base.Width / 2, (base.Height + num2) / 2)
			};
			LinearGradientBrush brush = new LinearGradientBrush(new Rectangle(array[0].X, array[0].Y, num, num2), _couleur.VeryLightColor, _couleur.LightColor, 90f);
			e.Graphics.FillPolygon(brush, array);
			e.Graphics.DrawPolygon(new Pen(_couleur.DarkColor), array);
			return;
		}
		int num3 = 0;
		if (scrollBar.Visible)
		{
			num3 = -scrollBar.Value;
		}
		LinearGradientBrush brush2;
		for (int i = 0; i < _items.Count; i++)
		{
			if (num3 + ItemHeight > base.Height - MinimizeHeight)
			{
				break;
			}
			Rectangle rect = new Rectangle(0, num3, base.Width - 3, ItemHeight);
			if (scrollBar.Visible)
			{
				rect = new Rectangle(0, num3, base.Width - scrollBar.Width - 5, ItemHeight);
			}
			if (_selectedIndex == i)
			{
				if (_mouseDown)
				{
					e.Graphics.FillRectangle(new SolidBrush(_couleur.LightColor), rect);
					e.Graphics.DrawRectangle(new Pen(_couleur.DarkColor), rect);
				}
				else
				{
					brush2 = new LinearGradientBrush(rect, Color.FromArgb(_alpha, _couleur.VeryLightColor), Color.FromArgb(_alpha, _couleur.LightColor), 90f);
					e.Graphics.FillRectangle(brush2, rect);
					e.Graphics.DrawRectangle(new Pen(_couleur.DarkColor), rect);
				}
			}
			StringFormat stringFormat = new StringFormat();
			stringFormat.LineAlignment = StringAlignment.Center;
			switch (DisplayStyle)
			{
			case ToolStripItemDisplayStyle.Text:
				rect = new Rectangle(_contentMargin, num3, base.Width - scrollBar.Width - _contentMargin, ItemHeight);
				e.Graphics.DrawString(_items[i].Text, Font, Brushes.Black, rect, stringFormat);
				break;
			case ToolStripItemDisplayStyle.Image:
			{
				Point point2 = new Point(_contentMargin + _imageMargin, (ItemHeight - 16) / 2);
				e.Graphics.DrawImage(_items[i].Image, point2);
				break;
			}
			case ToolStripItemDisplayStyle.ImageAndText:
			{
				Point point = new Point(_contentMargin + _imageMargin, (ItemHeight - 16) / 2);
				e.Graphics.DrawImage(_items[i].Image, point);
				rect = new Rectangle(_imageMargin * 2, num3, base.Width - scrollBar.Width - _imageMargin * 2, ItemHeight);
				e.Graphics.DrawString(_items[i].Text, Font, Brushes.Black, rect, stringFormat);
				break;
			}
			}
			num3 += ItemHeight;
		}
		int num4 = (int)((double)MinimizeHeight / 1.5);
		int num5 = num4 / 2;
		Point[] array2 = new Point[3]
		{
			new Point((base.Width - num4) / 2, base.Height - (MinimizeHeight - num5) / 2),
			new Point((base.Width + num4) / 2, base.Height - (MinimizeHeight - num5) / 2),
			new Point(base.Width / 2, base.Height - (MinimizeHeight + num5) / 2)
		};
		brush2 = new LinearGradientBrush(new Rectangle(array2[0].X, array2[0].Y, num4, num5), _couleur.VeryLightColor, _couleur.LightColor, 90f);
		e.Graphics.FillPolygon(brush2, array2);
		e.Graphics.DrawPolygon(new Pen(_couleur.DarkColor), array2);
	}

	public void SlideUp()
	{
		_nextHeight = base.Height;
		_isSlideUp = true;
		timer.Start();
	}

	public void SlideDown()
	{
		_isSlideDown = true;
		_isMinimize = false;
		_nextHeight = _items.Count * ItemHeight + MinimizeHeight;
		timer.Start();
	}

	public void SlideDown(int height)
	{
		_nextHeight = height;
		_isMinimize = false;
		_isSlideDown = true;
		timer.Start();
	}

	private void CustomListBox_MouseMove(object sender, MouseEventArgs e)
	{
		Focus();
		int num = (e.Y + scrollBar.Value) / ItemHeight;
		if (e.Y > base.Height - MinimizeHeight)
		{
			num = -1;
		}
		if (_selectedIndex != num)
		{
			if ((num >= 0 && num < _items.Count) || e.Y > base.Height - MinimizeHeight)
			{
				Cursor = Cursors.Hand;
			}
			else
			{
				Cursor = Cursors.Default;
			}
			_selectedIndex = num;
			_mouseDown = false;
			_alpha = 0;
			timer.Start();
		}
	}

	private void CustomListBox_MouseDown(object sender, MouseEventArgs e)
	{
		_mouseDown = true;
		Focus();
		Invalidate();
	}

	private void CustomListBox_MouseUp(object sender, MouseEventArgs e)
	{
		if (e.Button != MouseButtons.Left)
		{
			return;
		}
		bool mouseDown = _mouseDown;
		Focus();
		_mouseDown = false;
		if (_isMinimize)
		{
			SlideDown();
			return;
		}
		Invalidate();
		if (mouseDown)
		{
			if (e.Y > base.Height - MinimizeHeight)
			{
				SlideUp();
			}
			else if (_selectedIndex >= 0 && _selectedIndex < _items.Count && this.SelectItem != null)
			{
				this.SelectItem(this, new CustomTopMenuItemEventArgs(_items[_selectedIndex]));
			}
		}
	}

	private void CustomListBox_MouseLeave(object sender, EventArgs e)
	{
		_selectedIndex = -1;
		_mouseDown = false;
		Invalidate();
	}

	private void CustomListBox_Resize(object sender, EventArgs e)
	{
		if (_items.Count * ItemHeight > base.Height - MinimizeHeight && !_isMinimize)
		{
			scrollBar.Visible = true;
			scrollBar.Maximum = _items.Count * ItemHeight - base.Height + MinimizeHeight;
		}
		else
		{
			scrollBar.Visible = false;
			scrollBar.Value = 0;
		}
		Invalidate();
	}

	private void timer_Tick(object sender, EventArgs e)
	{
		if (_isSlideUp)
		{
			if (base.Height > MinimizeHeight)
			{
				base.Height -= Math.Min((_nextHeight - MinimizeHeight) / 5, base.Height - MinimizeHeight);
			}
			else
			{
				_isSlideUp = false;
				_isMinimize = true;
				scrollBar.Visible = false;
				timer.Stop();
			}
		}
		else if (_isSlideDown)
		{
			if (base.Height < _nextHeight)
			{
				base.Height += Math.Min((_nextHeight - MinimizeHeight) / 5, _nextHeight - base.Height);
			}
			else
			{
				_isSlideDown = false;
				timer.Stop();
			}
		}
		else if (_alpha < 255)
		{
			_alpha += Math.Min(50, 255 - _alpha);
		}
		else
		{
			timer.Stop();
		}
		Invalidate();
	}

	private void scrollBar_Scroll(object sender, ScrollEventArgs e)
	{
		Invalidate();
	}
}
