using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using CDV_Viewer.Data;

namespace CDV_Viewer.Controls;

public class DockContainer : UserControl
{
	private const int SHADOW_SIZE = 5;

	private const int MARGIN = 9;

	private bool _disabledListEvents;

	private DockChild _resizedChild;

	private DockChildCollection _dockChilds;

	public DockChildCollection DockChilds => _dockChilds;

	public DockContainer()
	{
		DoubleBuffered = true;
		_dockChilds = new DockChildCollection();
		_dockChilds.ItemAdded += DockChilds_ItemAdded;
		_dockChilds.ItemRemoved += DockChilds_ItemRemoved;
	}

	public void RefreshChilds()
	{
		int num = 0;
		int num2 = 0;
		int num3 = 0;
		int num4 = 0;
		for (int i = 0; i < _dockChilds.Count; i++)
		{
			DockChild dockChild = _dockChilds[i];
			if (!dockChild.Visible)
			{
				continue;
			}
			num = (num2 = (num3 = (num4 = 0)));
			for (int j = 0; j < i; j++)
			{
				if (_dockChilds[j].Visible)
				{
					switch (_dockChilds[j].DockPosition)
					{
					case DockStyle.Top:
						num += _dockChilds[j].Height + 18;
						break;
					case DockStyle.Bottom:
						num2 += _dockChilds[j].Height + 18;
						break;
					case DockStyle.Left:
						num3 += _dockChilds[j].Width + 18;
						break;
					case DockStyle.Right:
						num4 += _dockChilds[j].Width + 18;
						break;
					}
				}
			}
			switch (dockChild.DockPosition)
			{
			case DockStyle.Top:
				dockChild.Top = num + 9;
				dockChild.Left = num3 + 9;
				dockChild.Width = base.Width - num3 - num4 - 18;
				break;
			case DockStyle.Bottom:
				dockChild.Top = base.Height - dockChild.Height - num2 - 9;
				dockChild.Left = num3 + 9;
				dockChild.Width = base.Width - num3 - num4 - 18;
				break;
			case DockStyle.Left:
				dockChild.Left = num3 + 9;
				dockChild.Top = num + 9;
				dockChild.Height = base.Height - num - num2 - 18;
				break;
			case DockStyle.Right:
				dockChild.Left = base.Width - dockChild.Width - num4 - 9;
				dockChild.Top = num + 9;
				dockChild.Height = base.Height - num - num2 - 18;
				break;
			case DockStyle.Fill:
				dockChild.Top = num;
				dockChild.Left = num3;
				dockChild.Height = base.Height - num - num2;
				dockChild.Width = base.Width - num3 - num4;
				break;
			}
			dockChild.BackColor = ((dockChild.DockPosition == DockStyle.Fill) ? Color.Transparent : Color.White);
		}
		MinimumSize = new Size(num3 + num4, num + num2);
		Invalidate();
	}

	public DockChild GetResizedChildAt(Point pt)
	{
		foreach (DockChild dockChild in _dockChilds)
		{
			switch (dockChild.DockPosition)
			{
			case DockStyle.Top:
				if (new Rectangle(dockChild.Left, dockChild.Bottom, dockChild.Width, 9).Contains(pt))
				{
					return dockChild;
				}
				break;
			case DockStyle.Bottom:
				if (new Rectangle(dockChild.Left, dockChild.Top - 9, dockChild.Width, 9).Contains(pt))
				{
					return dockChild;
				}
				break;
			case DockStyle.Left:
				if (new Rectangle(dockChild.Right, dockChild.Top, 9, dockChild.Height).Contains(pt))
				{
					return dockChild;
				}
				break;
			case DockStyle.Right:
				if (new Rectangle(dockChild.Left - 9, dockChild.Top, 9, dockChild.Height).Contains(pt))
				{
					return dockChild;
				}
				break;
			}
		}
		return null;
	}

	protected override void OnControlAdded(ControlEventArgs e)
	{
		if (!_disabledListEvents && e.Control is DockChild)
		{
			_dockChilds.Add((DockChild)e.Control);
		}
	}

	protected override void OnControlRemoved(ControlEventArgs e)
	{
		if (!_disabledListEvents && e.Control is DockChild)
		{
			_dockChilds.Remove((DockChild)e.Control);
		}
	}

	private void DockChilds_ItemAdded(object sender, Collection<DockChild>.ItemEventArgs e)
	{
		if (!_disabledListEvents)
		{
			_disabledListEvents = true;
			base.Controls.Add(e.Item);
			_disabledListEvents = false;
		}
		e.Item.BackColor = Color.White;
		e.Item.VisibleChanged += delegate
		{
			RefreshChilds();
		};
		e.Item.DockPositionChanged += delegate
		{
			RefreshChilds();
		};
		RefreshChilds();
	}

	private void DockChilds_ItemRemoved(object sender, Collection<DockChild>.ItemEventArgs e)
	{
		if (!_disabledListEvents)
		{
			_disabledListEvents = true;
			base.Controls.Remove(e.Item);
			_disabledListEvents = false;
		}
		e.Item.VisibleChanged -= delegate
		{
			RefreshChilds();
		};
		e.Item.DockPositionChanged -= delegate
		{
			RefreshChilds();
		};
		RefreshChilds();
	}

	protected override void OnResize(EventArgs e)
	{
		RefreshChilds();
	}

	protected override void OnPaint(PaintEventArgs e)
	{
		foreach (DockChild dockChild in _dockChilds)
		{
			if (dockChild.Visible && dockChild.DockPosition != DockStyle.None && dockChild.DockPosition != DockStyle.Fill)
			{
				Rectangle rect = new Rectangle(dockChild.Right + 1, dockChild.Top, 5, dockChild.Height);
				Rectangle rect2 = new Rectangle(dockChild.Left - 1 - 5, dockChild.Top, 5, dockChild.Height);
				Rectangle rect3 = new Rectangle(dockChild.Left, dockChild.Top - 1 - 5, dockChild.Width - 1, 5);
				Rectangle rect4 = new Rectangle(dockChild.Left, dockChild.Bottom + 1, dockChild.Width, 5);
				TextureBrush textureBrush = new TextureBrush(Degrade.right, WrapMode.Tile);
				textureBrush.TranslateTransform(rect.X, 0f);
				TextureBrush textureBrush2 = new TextureBrush(Degrade.left, WrapMode.Tile);
				textureBrush2.TranslateTransform(rect2.X, 0f);
				TextureBrush textureBrush3 = new TextureBrush(Degrade.top, WrapMode.Tile);
				textureBrush3.TranslateTransform(0f, rect3.Y);
				TextureBrush textureBrush4 = new TextureBrush(Degrade.bottom, WrapMode.Tile);
				textureBrush4.TranslateTransform(0f, rect4.Y);
				e.Graphics.DrawRectangle(Pens.Gray, new Rectangle(dockChild.Left - 1, dockChild.Top - 1, dockChild.Width + 1, dockChild.Height + 1));
				e.Graphics.FillRectangle(textureBrush, rect);
				e.Graphics.FillRectangle(textureBrush2, rect2);
				e.Graphics.FillRectangle(textureBrush3, rect3);
				e.Graphics.FillRectangle(textureBrush4, rect4);
				e.Graphics.DrawImage(Degrade.topleft, dockChild.Left - 1 - 5, dockChild.Top - 1 - 5);
				e.Graphics.DrawImage(Degrade.topright, dockChild.Right, dockChild.Top - 1 - 5);
				e.Graphics.DrawImage(Degrade.bottomleft, dockChild.Left - 1 - 5, dockChild.Bottom);
				e.Graphics.DrawImage(Degrade.bottomright, dockChild.Right, dockChild.Bottom);
			}
		}
	}

	protected override void OnMouseMove(MouseEventArgs e)
	{
		if (_resizedChild == null)
		{
			Cursor cursor = Cursors.Default;
			DockChild resizedChildAt = GetResizedChildAt(e.Location);
			if (resizedChildAt != null)
			{
				cursor = ((resizedChildAt.DockPosition != DockStyle.Top && resizedChildAt.DockPosition != DockStyle.Bottom) ? Cursors.VSplit : Cursors.HSplit);
			}
			if (cursor != Cursor)
			{
				Cursor = cursor;
			}
			return;
		}
		int min = _resizedChild.MinimumSize.Height;
		int min2 = _resizedChild.MinimumSize.Width;
		int num = base.Height;
		int num2 = base.Width;
		foreach (DockChild dockChild in _dockChilds)
		{
			if (dockChild != _resizedChild && dockChild.Visible)
			{
				switch (dockChild.DockPosition)
				{
				case DockStyle.Top:
				case DockStyle.Bottom:
					num -= dockChild.Height + 18;
					break;
				case DockStyle.Left:
				case DockStyle.Right:
					num2 -= dockChild.Width + 18;
					break;
				}
			}
		}
		switch (_resizedChild.DockPosition)
		{
		case DockStyle.Top:
			_resizedChild.Height = Bound(e.Y - _resizedChild.Top, min, num);
			break;
		case DockStyle.Bottom:
			_resizedChild.Height = Bound(_resizedChild.Bottom - e.Y, min, num);
			break;
		case DockStyle.Left:
			_resizedChild.Width = Bound(e.X - _resizedChild.Left, min2, num2);
			break;
		case DockStyle.Right:
			_resizedChild.Width = Bound(_resizedChild.Right - e.X, min2, num2);
			break;
		}
		RefreshChilds();
		Invalidate();
	}

	private int Bound(int value, int min, int max)
	{
		return Math.Min(Math.Max(value, min), max);
	}

	protected override void OnMouseDown(MouseEventArgs e)
	{
		_resizedChild = GetResizedChildAt(e.Location);
	}

	protected override void OnMouseUp(MouseEventArgs e)
	{
		_resizedChild = null;
	}

	protected override void OnMouseLeave(EventArgs e)
	{
		Cursor = Cursors.Default;
	}
}
