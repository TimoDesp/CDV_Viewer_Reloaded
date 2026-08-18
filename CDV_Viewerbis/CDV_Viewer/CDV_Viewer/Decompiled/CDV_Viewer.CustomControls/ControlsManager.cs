using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Drawing;
using System.Windows.Forms;

namespace CDV_Viewer.CustomControls;

[Designer("System.Windows.Forms.Design.ParentControlDesigner, System.Design", typeof(IDesigner))]
public class ControlsManager : UserControl
{
	private IContainer components;

	private Timer timer;

	private bool _useAnimation;

	private Dictionary<string, int> _visibleSize = new Dictionary<string, int>();

	private bool _show;

	private Control _animationControl;

	private bool _mouseDown;

	private Control _resizeControl;

	private int RESIZE_REGION_SIZE = 6;

	private int CONTROLS_MIN_WIDTH = 20;

	private int CONTROLS_MIN_HEIGHT = 20;

	public bool UseAnimation
	{
		get
		{
			return _useAnimation;
		}
		set
		{
			_useAnimation = value;
		}
	}

	public event VisibleEventHandler ControlVisibleChanged;

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
		base.SuspendLayout();
		this.timer.Interval = 20;
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.Name = "ControlsManager";
		base.ResumeLayout(false);
	}

	public ControlsManager()
	{
		InitializeComponent();
		base.ControlAdded += ControlsManager_ControlAdded;
		base.MouseDown += ControlsManager_MouseDown;
		base.MouseUp += ControlsManager_MouseUp;
		base.MouseMove += ControlsManager_MouseMove;
		base.Paint += ControlsManager_Paint;
		timer.Tick += timer_Tick;
	}

	public void AddControl(Control control, string name, DockStyle dock, bool visible)
	{
		control.Tag = name;
		control.Dock = dock;
		if (!visible && IsResizableControl(control))
		{
			if (IsVariableWidth(control))
			{
				AddVisibleSize(name, control.Width);
				control.Width = 0;
			}
			else
			{
				AddVisibleSize(name, control.Height);
				control.Height = 0;
			}
			control.Enabled = false;
		}
		base.Controls.Add(control);
	}

	public void SetControlVisible(string name, bool visible)
	{
		if (visible)
		{
			ShowControl(name);
		}
		else
		{
			HideControl(name);
		}
	}

	public void InvertControlVisible(string name)
	{
		Control control = GetControl(name);
		if (control != null)
		{
			if (control.Enabled)
			{
				HideControl(name);
			}
			else
			{
				ShowControl(name);
			}
		}
	}

	public void ShowControl(string name)
	{
		if (_animationControl != null)
		{
			return;
		}
		Control control = GetControl(name);
		if (control == null || !IsResizableControl(control) || control.Enabled)
		{
			return;
		}
		if (_useAnimation)
		{
			_show = true;
			_animationControl = control;
			timer.Start();
			return;
		}
		int visibleSize = GetVisibleSize(name);
		if (IsVariableWidth(control))
		{
			control.Width = visibleSize;
		}
		else
		{
			control.Height = visibleSize;
		}
		_animationControl = control;
		EndOperation(show: true);
	}

	public void HideControl(string name)
	{
		if (_animationControl != null)
		{
			return;
		}
		Control control = GetControl(name);
		if (control == null || !IsResizableControl(control) || !control.Enabled)
		{
			return;
		}
		if (IsVariableWidth(control))
		{
			AddVisibleSize(name, control.Width);
		}
		else
		{
			AddVisibleSize(name, control.Height);
		}
		if (_useAnimation)
		{
			_show = false;
			_animationControl = control;
			timer.Start();
			return;
		}
		if (IsVariableWidth(control))
		{
			control.Width = 0;
		}
		else
		{
			control.Height = 0;
		}
		_animationControl = control;
		EndOperation(show: false);
	}

	private Control GetControl(string name)
	{
		foreach (Control control in base.Controls)
		{
			if ((string)control.Tag == name)
			{
				return control;
			}
		}
		return null;
	}

	private int GetVisibleSize(string name)
	{
		int value = -1;
		if (_visibleSize.TryGetValue(name, out value))
		{
			return value;
		}
		return -1;
	}

	private bool IsVariableWidth(Control control)
	{
		if (control.Dock != DockStyle.Left)
		{
			return control.Dock == DockStyle.Right;
		}
		return true;
	}

	private void EndOperation(bool show)
	{
		if (_animationControl != null)
		{
			timer.Stop();
			_animationControl.Enabled = show;
			if (this.ControlVisibleChanged != null)
			{
				this.ControlVisibleChanged(this, new VisibleEventArgs((string)_animationControl.Tag, show));
			}
			_animationControl = null;
		}
	}

	private Control GetResizeControlAt(Point location)
	{
		foreach (Control control in base.Controls)
		{
			if (control.Enabled)
			{
				Rectangle rectangle = new Rectangle(0, 0, 0, 0);
				switch (control.Dock)
				{
				case DockStyle.Top:
					rectangle = new Rectangle(control.Left, control.Bottom - RESIZE_REGION_SIZE / 2, control.Width, RESIZE_REGION_SIZE);
					break;
				case DockStyle.Bottom:
					rectangle = new Rectangle(control.Left, control.Top - RESIZE_REGION_SIZE / 2, control.Width, RESIZE_REGION_SIZE);
					break;
				case DockStyle.Left:
					rectangle = new Rectangle(control.Right - RESIZE_REGION_SIZE / 2, control.Top, RESIZE_REGION_SIZE, control.Height);
					break;
				case DockStyle.Right:
					rectangle = new Rectangle(control.Left - RESIZE_REGION_SIZE / 2, control.Top, RESIZE_REGION_SIZE, control.Height);
					break;
				}
				if (rectangle.Contains(location))
				{
					return control;
				}
			}
		}
		return null;
	}

	private void AddVisibleSize(string name, int size)
	{
		if (_visibleSize.ContainsKey(name))
		{
			_visibleSize.Remove(name);
		}
		_visibleSize.Add(name, size);
	}

	private bool IsResizableControl(Control control)
	{
		if (control.Dock != DockStyle.Top && control.Dock != DockStyle.Right && control.Dock != DockStyle.Bottom)
		{
			return control.Dock == DockStyle.Left;
		}
		return true;
	}

	private void ControlsManager_ControlAdded(object sender, ControlEventArgs e)
	{
		e.Control.MouseDown += ChildControl_MouseDown;
		e.Control.MouseUp += ChildControl_MouseUp;
		e.Control.MouseMove += ChildControl_MouseMove;
	}

	private void ControlsManager_MouseDown(object sender, MouseEventArgs e)
	{
		if (e.Button == MouseButtons.Left)
		{
			_mouseDown = true;
			_resizeControl = GetResizeControlAt(e.Location);
		}
	}

	private void ControlsManager_MouseUp(object sender, MouseEventArgs e)
	{
		if (e.Button == MouseButtons.Left)
		{
			_mouseDown = false;
			_resizeControl = null;
		}
	}

	private void ControlsManager_MouseMove(object sender, MouseEventArgs e)
	{
		if (_mouseDown)
		{
			if (_resizeControl != null)
			{
				int num = -1;
				int num2 = -1;
				switch (_resizeControl.Dock)
				{
				case DockStyle.Top:
					num2 = e.Y - _resizeControl.Top;
					break;
				case DockStyle.Bottom:
					num2 = _resizeControl.Bottom - e.Y;
					break;
				case DockStyle.Left:
					num = e.X - _resizeControl.Left;
					break;
				case DockStyle.Right:
					num = _resizeControl.Right - e.X;
					break;
				}
				if (num >= CONTROLS_MIN_WIDTH && num <= base.Width - CONTROLS_MIN_WIDTH)
				{
					_resizeControl.Width = num;
				}
				if (num2 >= CONTROLS_MIN_HEIGHT && num2 <= base.Height - CONTROLS_MIN_HEIGHT)
				{
					_resizeControl.Height = num2;
				}
			}
			return;
		}
		Control resizeControlAt = GetResizeControlAt(e.Location);
		if (resizeControlAt != null)
		{
			if (IsVariableWidth(resizeControlAt))
			{
				Cursor = Cursors.VSplit;
			}
			else
			{
				Cursor = Cursors.HSplit;
			}
		}
		else if (Cursor == Cursors.VSplit || Cursor == Cursors.HSplit)
		{
			Cursor = Cursors.Default;
		}
	}

	private void ControlsManager_Paint(object sender, PaintEventArgs e)
	{
		if (_resizeControl != null)
		{
			switch (_resizeControl.Dock)
			{
			case DockStyle.Top:
				e.Graphics.DrawLine(new Pen(Color.Black, 2f), _resizeControl.Left, _resizeControl.Bottom - 1, _resizeControl.Right, _resizeControl.Bottom - 1);
				break;
			case DockStyle.Bottom:
				e.Graphics.DrawLine(new Pen(Color.Black, 2f), _resizeControl.Left, _resizeControl.Top - 1, _resizeControl.Right, _resizeControl.Top - 1);
				break;
			case DockStyle.Left:
				e.Graphics.DrawLine(new Pen(Color.Black, 2f), _resizeControl.Right - 1, _resizeControl.Top, _resizeControl.Right - 1, _resizeControl.Bottom);
				break;
			case DockStyle.Right:
				e.Graphics.DrawLine(new Pen(Color.Black, 2f), _resizeControl.Left - 1, _resizeControl.Top, _resizeControl.Left - 1, _resizeControl.Bottom);
				break;
			}
		}
	}

	private void timer_Tick(object sender, EventArgs e)
	{
		if (!_useAnimation || _animationControl == null)
		{
			return;
		}
		int visibleSize = GetVisibleSize((string)_animationControl.Tag);
		if (_show)
		{
			if (IsVariableWidth(_animationControl))
			{
				if (_animationControl.Width < visibleSize)
				{
					_animationControl.Width += Math.Min(visibleSize / 5, visibleSize - _animationControl.Width);
				}
				else
				{
					EndOperation(show: true);
				}
			}
			else if (_animationControl.Height < visibleSize)
			{
				_animationControl.Height += Math.Min(visibleSize / 5, visibleSize - _animationControl.Height);
			}
			else
			{
				EndOperation(show: true);
			}
		}
		else if (IsVariableWidth(_animationControl))
		{
			if (_animationControl.Width > 0)
			{
				_animationControl.Width -= Math.Min(visibleSize / 5, _animationControl.Width);
			}
			else
			{
				EndOperation(show: false);
			}
		}
		else if (_animationControl.Height > 0)
		{
			_animationControl.Height -= Math.Min(visibleSize / 5, _animationControl.Height);
		}
		else
		{
			EndOperation(show: false);
		}
	}

	private void ChildControl_MouseDown(object sender, MouseEventArgs e)
	{
		ControlsManager_MouseDown(this, new MouseEventArgs(e.Button, e.Clicks, e.X + ((Control)sender).Left, e.Y + ((Control)sender).Top, e.Delta));
	}

	private void ChildControl_MouseUp(object sender, MouseEventArgs e)
	{
		ControlsManager_MouseUp(this, new MouseEventArgs(e.Button, e.Clicks, e.X + ((Control)sender).Left, e.Y + ((Control)sender).Top, e.Delta));
	}

	private void ChildControl_MouseMove(object sender, MouseEventArgs e)
	{
		ControlsManager_MouseMove(this, new MouseEventArgs(e.Button, e.Clicks, e.X + ((Control)sender).Left, e.Y + ((Control)sender).Top, e.Delta));
	}
}
