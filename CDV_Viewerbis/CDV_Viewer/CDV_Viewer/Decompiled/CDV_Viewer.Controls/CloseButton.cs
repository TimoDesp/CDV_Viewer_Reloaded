using System;
using System.Drawing;
using System.Windows.Forms;
using CDV_Viewer.Properties;

namespace CDV_Viewer.Controls;

public class CloseButton : Button
{
	public CloseButton()
	{
		base.Size = new Size(15, 15);
		Cursor = Cursors.Hand;
	}

	protected override void OnPaint(PaintEventArgs pevent)
	{
		pevent.Graphics.Clear(BackColor);
		pevent.Graphics.DrawImage(Resources.close, 4, 4, 7, 7);
	}

	protected override void OnMouseEnter(EventArgs e)
	{
		BackColor = Color.Gainsboro;
		Invalidate();
	}

	protected override void OnMouseLeave(EventArgs e)
	{
		BackColor = Color.White;
		Invalidate();
	}
}
