using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace CDV_Viewer.Forms;

public sealed class LoggerForm : Form
{
	private static LoggerForm _uniqueInstance;

	private Action _onshown;

	private bool _visible;

	private IContainer components;

	private TextBox logText;

	static LoggerForm()
	{
		_uniqueInstance = new LoggerForm();
	}

	public LoggerForm()
	{
		InitializeComponent();
	}

	public new static void Show()
	{
		if (!_uniqueInstance._visible)
		{
			_uniqueInstance._visible = true;
			((Control)_uniqueInstance).Show();
		}
	}

	public new static void Close()
	{
		if (_uniqueInstance._visible)
		{
			((Form)_uniqueInstance).Close();
		}
	}

	public static void ShowDialog(Action onshown)
	{
		if (_uniqueInstance.Visible)
		{
			_uniqueInstance.Visible = false;
		}
		_uniqueInstance._visible = true;
		_uniqueInstance._onshown = onshown;
		_uniqueInstance.ShowDialog();
	}

	protected override void OnShown(EventArgs e)
	{
		base.OnShown(e);
		_onshown?.Invoke();
	}

	protected override void OnClosed(EventArgs e)
	{
		_uniqueInstance._visible = false;
		logText.Text = "";
		base.OnClosed(e);
	}

	public static void Write(string s)
	{
		if (_uniqueInstance._visible)
		{
			_uniqueInstance.logText.Text += s;
		}
	}

	public static void WriteLine(string s)
	{
		Write(s + Environment.NewLine);
	}

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
		this.logText = new System.Windows.Forms.TextBox();
		base.SuspendLayout();
		this.logText.BackColor = System.Drawing.Color.WhiteSmoke;
		this.logText.Dock = System.Windows.Forms.DockStyle.Fill;
		this.logText.Location = new System.Drawing.Point(0, 0);
		this.logText.Multiline = true;
		this.logText.Name = "logText";
		this.logText.ReadOnly = true;
		this.logText.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
		this.logText.Size = new System.Drawing.Size(912, 1067);
		this.logText.TabIndex = 0;
		base.AutoScaleDimensions = new System.Drawing.SizeF(11f, 24f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.ClientSize = new System.Drawing.Size(912, 1067);
		base.Controls.Add(this.logText);
		base.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;
		base.Name = "LoggerForm";
		this.Text = "LoggerForm";
		base.ResumeLayout(false);
		base.PerformLayout();
	}
}
