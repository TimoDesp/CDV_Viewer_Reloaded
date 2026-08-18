using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace CDV_Viewer.Controls;

public class SIGContextMenuStrip : ContextMenuStrip
{
	public SIGContextMenuStrip()
	{
		base.RenderMode = ToolStripRenderMode.Professional;
		base.Renderer = new ToolStripProfessionalRenderer(new CustomMenuStripColorTable());
		base.ItemAdded += SIGContextMenuStrip_ItemAdded;
		base.Paint += SIGContextMenuStrip_Paint;
	}

	private void SIGContextMenuStrip_ItemAdded(object sender, ToolStripItemEventArgs e)
	{
		if (sender is ToolStripMenuItem)
		{
			((ToolStripMenuItem)e.Item).DropDown.Paint += DropDown_Paint;
		}
	}

	private void SIGContextMenuStrip_Paint(object sender, PaintEventArgs e)
	{
		Rectangle rect = new Rectangle(0, 0, base.Width, base.Height);
		LinearGradientBrush brush = new LinearGradientBrush(rect, Color.FromArgb(244, 242, 236), Color.FromArgb(224, 222, 216), 90f);
		e.Graphics.FillRectangle(brush, rect);
	}

	private void DropDown_Paint(object sender, PaintEventArgs e)
	{
		Control control = (Control)sender;
		Rectangle rect = new Rectangle(0, 0, control.Width, control.Height);
		LinearGradientBrush brush = new LinearGradientBrush(rect, Color.FromArgb(244, 242, 236), Color.FromArgb(224, 222, 216), 90f);
		e.Graphics.FillRectangle(brush, rect);
	}
}
