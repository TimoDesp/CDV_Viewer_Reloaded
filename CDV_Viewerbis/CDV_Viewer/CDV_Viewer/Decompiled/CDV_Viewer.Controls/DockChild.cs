using System;
using System.Drawing;
using System.Windows.Forms;

namespace CDV_Viewer.Controls;

public class DockChild : UserControl
{
	private DockStyle _dockPosition;

	private FocusGetter _fg;

	public DockStyle DockPosition
	{
		get
		{
			return _dockPosition;
		}
		set
		{
			_dockPosition = value;
			this.DockPositionChanged?.Invoke(this, new EventArgs());
		}
	}

	public event EventHandler DockPositionChanged;

	public DockChild()
	{
		_fg = new FocusGetter
		{
			Location = Point.Empty,
			Size = Size.Empty
		};
		base.Controls.Add(_fg);
		base.Click += delegate
		{
			_fg.Focus();
		};
		_fg.KeyUp += delegate(object sender, KeyEventArgs e)
		{
			OnKeyUp(e);
		};
		_fg.KeyDown += delegate(object sender, KeyEventArgs e)
		{
			OnKeyDown(e);
		};
		_fg.KeyPress += delegate(object sender, KeyPressEventArgs e)
		{
			OnKeyPress(e);
		};
		_fg.MouseWheel += delegate(object sender, MouseEventArgs e)
		{
			OnMouseWheel(e);
		};
	}

	public new void Focus()
	{
		_fg.Focus();
	}
}
