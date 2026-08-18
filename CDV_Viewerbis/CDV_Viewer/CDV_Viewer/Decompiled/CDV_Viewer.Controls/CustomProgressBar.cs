using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace CDV_Viewer.Controls;

public class CustomProgressBar : ProgressBar
{
	private IContainer components;

	public CustomProgressBar()
	{
		InitializeComponent();
		SetStyle(ControlStyles.UserPaint | ControlStyles.OptimizedDoubleBuffer, value: true);
	}

	public CustomProgressBar(IContainer container)
	{
		container.Add(this);
		InitializeComponent();
		SetStyle(ControlStyles.UserPaint | ControlStyles.OptimizedDoubleBuffer, value: true);
	}

	protected override void OnPaint(PaintEventArgs e)
	{
		if (base.Maximum != 0)
		{
			Rectangle rect = new Rectangle(2, 2, base.Value * (base.Width - 4) / base.Maximum, base.Height - 4);
			e.Graphics.FillRectangle(new SolidBrush(ForeColor), rect);
			e.Graphics.DrawRectangle(new Pen(ForeColor), new Rectangle(0, 0, base.Width - 1, base.Height - 1));
		}
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
		this.components = new System.ComponentModel.Container();
	}
}
