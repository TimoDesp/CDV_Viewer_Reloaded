using System;
using System.Drawing;
using System.Windows.Forms;
using CDV_Viewer.Data;

namespace CDV_Viewer.Controls;

public class SimpleListView : Control
{
	private Collection<SimpleListViewItem> _items;

	private int _hoverIndex = -1;

	private int _selectedIndex = -1;

	private Color _hoverColor = Color.FromArgb(230, 230, 230);

	private Color _selectedColor = Color.Gainsboro;

	private int _itemSize = 20;

	private CustomScrollBar scrollBar;

	private ScrollBarOrientation _orientation;

	private ToolStripItemDisplayStyle _displayStyle = ToolStripItemDisplayStyle.Text;

	public Collection<SimpleListViewItem> Items => _items;

	public int SelectedIndex
	{
		get
		{
			return _selectedIndex;
		}
		set
		{
			if (value >= -1 && value < _items.Count)
			{
				_selectedIndex = value;
			}
		}
	}

	public SimpleListViewItem SelectedItem
	{
		get
		{
			if (_selectedIndex >= 0 && _selectedIndex < _items.Count)
			{
				return _items[_selectedIndex];
			}
			return null;
		}
	}

	public Color HoverColor
	{
		get
		{
			return _hoverColor;
		}
		set
		{
			_hoverColor = value;
			Invalidate();
		}
	}

	public Color SelectedColor
	{
		get
		{
			return _selectedColor;
		}
		set
		{
			_selectedColor = value;
			Invalidate();
		}
	}

	public int ItemSize
	{
		get
		{
			return _itemSize;
		}
		set
		{
			_itemSize = value;
			scrollBar.SmallChange = _itemSize;
			RefreshScrollBar();
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
			scrollBar.Orientation = _orientation;
			if (_orientation == ScrollBarOrientation.Horizontal)
			{
				scrollBar.Dock = DockStyle.Bottom;
				scrollBar.Height = 10;
			}
			else
			{
				scrollBar.Dock = DockStyle.Right;
				scrollBar.Width = 10;
			}
		}
	}

	public ToolStripItemDisplayStyle DisplayStyle
	{
		get
		{
			return _displayStyle;
		}
		set
		{
			_displayStyle = value;
			Invalidate();
		}
	}

	public event EventHandler SelectedIndexChanged;

	public SimpleListView()
	{
		InitializeComponent();
	}

	public SimpleListView(ScrollBarOrientation orientation)
	{
		_orientation = orientation;
		InitializeComponent();
	}

	private void InitializeComponent()
	{
		this.DoubleBuffered = true;
		this.Cursor = System.Windows.Forms.Cursors.Hand;
		this.scrollBar = new CDV_Viewer.Controls.CustomScrollBar(this._orientation);
		this.scrollBar.Value = 0;
		this.scrollBar.Margin = System.Windows.Forms.Padding.Empty;
		if (this._orientation == CDV_Viewer.Controls.ScrollBarOrientation.Horizontal)
		{
			this.scrollBar.Dock = System.Windows.Forms.DockStyle.Bottom;
			this.scrollBar.Height = 10;
		}
		else
		{
			this.scrollBar.Dock = System.Windows.Forms.DockStyle.Right;
			this.scrollBar.Width = 10;
		}
		this.scrollBar.BackColor = System.Drawing.Color.FromArgb(240, 240, 240);
		this.scrollBar.BackHoverColor = System.Drawing.Color.FromArgb(230, 230, 230);
		this.scrollBar.ForeColor = System.Drawing.Color.FromArgb(200, 200, 200);
		this.scrollBar.ForeSelectedColor = System.Drawing.Color.FromArgb(180, 180, 180);
		this.scrollBar.Scroll += new System.Windows.Forms.ScrollEventHandler(scrollBar_Scroll);
		base.Controls.Add(this.scrollBar);
		base.MouseWheel += new System.Windows.Forms.MouseEventHandler(SimpleListView_MouseWheel);
		this._items = new CDV_Viewer.Data.Collection<CDV_Viewer.Controls.SimpleListViewItem>();
		this._items.CollectionChanged += new System.EventHandler(Items_CollectionChanged);
	}

	private int GetIndexAt(Point p)
	{
		if (_orientation == ScrollBarOrientation.Horizontal)
		{
			return (p.X + scrollBar.Value) / ItemSize;
		}
		return (p.Y + scrollBar.Value) / ItemSize;
	}

	public SimpleListViewItem GetItemAt(Point p)
	{
		int indexAt = GetIndexAt(p);
		if (indexAt >= 0 && indexAt < _items.Count)
		{
			return _items[indexAt];
		}
		return null;
	}

	private void RefreshScrollBar()
	{
		int num = ((_orientation != ScrollBarOrientation.Horizontal) ? base.Height : base.Width);
		scrollBar.Visible = _items.Count * ItemSize > num;
		if (!scrollBar.Visible)
		{
			scrollBar.Value = 0;
		}
		else
		{
			scrollBar.Maximum = _items.Count * ItemSize - num;
			scrollBar.LargeChange = num;
		}
		Invalidate();
	}

	private void Items_CollectionChanged(object sender, EventArgs e)
	{
		RefreshScrollBar();
	}

	protected override void OnResize(EventArgs e)
	{
		base.OnResize(e);
		RefreshScrollBar();
	}

	protected override void OnPaint(PaintEventArgs e)
	{
		StringFormat stringFormat = new StringFormat();
		stringFormat.LineAlignment = StringAlignment.Center;
		stringFormat.Trimming = StringTrimming.EllipsisCharacter;
		stringFormat.FormatFlags = StringFormatFlags.NoWrap;
		int num;
		if (_orientation == ScrollBarOrientation.Horizontal)
		{
			num = base.Height;
			if (scrollBar.Visible)
			{
				num -= scrollBar.Height;
			}
		}
		else
		{
			num = base.Width;
			if (scrollBar.Visible)
			{
				num -= scrollBar.Width;
			}
		}
		for (int i = 0; i < _items.Count; i++)
		{
			Rectangle rectangle = ((_orientation != ScrollBarOrientation.Horizontal) ? new Rectangle(0, ItemSize * i - scrollBar.Value, num, ItemSize) : new Rectangle(ItemSize * i - scrollBar.Value, 0, ItemSize, num));
			if (i == _selectedIndex)
			{
				e.Graphics.FillRectangle(new SolidBrush(_selectedColor), rectangle);
			}
			else if (i == _hoverIndex && base.Enabled)
			{
				e.Graphics.FillRectangle(new SolidBrush(_hoverColor), rectangle);
			}
			switch (_displayStyle)
			{
			case ToolStripItemDisplayStyle.Image:
				e.Graphics.DrawImageUnscaled(_items[i].Image, rectangle.X + (rectangle.Width - _items[i].Image.Width) / 2, rectangle.Y + (rectangle.Height - _items[i].Image.Height) / 2);
				break;
			case ToolStripItemDisplayStyle.ImageAndText:
				e.Graphics.DrawImageUnscaled(_items[i].Image, rectangle.X + (rectangle.Width - _items[i].Image.Width) / 2, rectangle.Y + (rectangle.Height - _items[i].Image.Height) / 2);
				break;
			case ToolStripItemDisplayStyle.Text:
				if (base.Enabled)
				{
					e.Graphics.DrawString(_items[i].Text, Font, Brushes.Black, rectangle, stringFormat);
				}
				else
				{
					e.Graphics.DrawString(_items[i].Text, Font, Brushes.Gray, rectangle, stringFormat);
				}
				break;
			}
		}
	}

	protected override void OnMouseMove(MouseEventArgs e)
	{
		_hoverIndex = GetIndexAt(e.Location);
		Invalidate();
	}

	protected override void OnMouseLeave(EventArgs e)
	{
		_hoverIndex = -1;
		Invalidate();
	}

	protected override void OnMouseClick(MouseEventArgs e)
	{
		if (base.Enabled)
		{
			Focus();
			_selectedIndex = GetIndexAt(e.Location);
			if (this.SelectedIndexChanged != null)
			{
				this.SelectedIndexChanged(this, new EventArgs());
			}
		}
	}

	private void scrollBar_Scroll(object sender, ScrollEventArgs e)
	{
		scrollBar.Value = Math.Max(scrollBar.Value - scrollBar.Value % _itemSize, 0);
		Invalidate();
	}

	private void SimpleListView_MouseWheel(object sender, MouseEventArgs e)
	{
		if (scrollBar.Visible)
		{
			if (e.Delta > 0)
			{
				scrollBar.Value -= scrollBar.SmallChange;
			}
			else
			{
				scrollBar.Value += scrollBar.SmallChange;
			}
			Invalidate();
		}
	}
}
