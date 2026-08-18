using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace CDV_Viewer.CustomControls;

public class CustomButton : Button
{
	private bool _mouseDown;

	private CustomControlColor _couleur = CustomControlColor.Gris;

	public CustomControlColor Couleur
	{
		get
		{
			return _couleur;
		}
		set
		{
			_couleur = value;
			Invalidate();
		}
	}

	public CustomButton()
	{
		Cursor = Cursors.Hand;
		base.MouseUp += Bouton_MouseUp;
		base.MouseDown += Bouton_MouseDown;
	}

	private void Bouton_MouseUp(object sender, MouseEventArgs e)
	{
		_mouseDown = false;
		Invalidate();
	}

	private void Bouton_MouseDown(object sender, MouseEventArgs e)
	{
		_mouseDown = true;
		Invalidate();
	}

	protected override void OnPaint(PaintEventArgs e)
	{
		Brush brush = new LinearGradientBrush(DisplayRectangle, Couleur.LightColor, Couleur.DarkColor, 90f);
		if (_mouseDown)
		{
			brush = new SolidBrush(Couleur.DarkColor);
		}
		e.Graphics.FillRectangle(brush, DisplayRectangle);
		e.Graphics.DrawRectangle(Pens.White, 1, 1, base.Width - 3, base.Height - 3);
		StringFormat stringFormat = new StringFormat();
		stringFormat.Alignment = StringAlignment.Center;
		stringFormat.LineAlignment = StringAlignment.Center;
		e.Graphics.DrawString(Text, Font, Brushes.White, DisplayRectangle, stringFormat);
	}
}
