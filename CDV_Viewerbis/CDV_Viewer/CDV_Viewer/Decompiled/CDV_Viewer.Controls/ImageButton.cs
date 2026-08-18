using System;
using System.Drawing;
using System.Windows.Forms;

namespace CDV_Viewer.Controls;

public class ImageButton : Button
{
	public ImageButton()
	{
		Cursor = Cursors.Hand;
	}

	protected override void OnPaint(PaintEventArgs pevent)
	{
		pevent.Graphics.Clear(BackColor);
		if (base.Image != null)
		{
			pevent.Graphics.DrawImage(base.Image, (base.Width - base.Image.Width) / 2, (base.Height - base.Image.Height) / 2, base.Image.Width, base.Image.Height);
		}
		pevent.Graphics.DrawRectangle(new Pen(Color.LightGray, 1f), 0, 0, base.Width - 1, base.Height - 1);
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
