using System;
using System.Drawing;
using System.Windows.Forms;

namespace CDV_Viewer.CustomControls;

public class CustomComboBox : UserControl
{
	private CustomControlColor _couleur = CustomControlColor.Gris;

	public object[] Items;

	public ComboBoxStyle Style = ComboBoxStyle.DropDown;

	private TextBox _textBox;

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

	public CustomComboBox()
	{
		BackColor = Color.White;
		base.Enter += ComboBox_Refresh;
		base.Leave += ComboBox_Refresh;
		base.Resize += ComboBox_Refresh;
		_textBox = new TextBox();
		base.Width = _textBox.Width + 24;
		base.Height = _textBox.Height + 4;
		_textBox.AutoSize = false;
		_textBox.BorderStyle = BorderStyle.None;
		_textBox.Top = (_textBox.Left = 2);
		_textBox.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
		_textBox.Enter += ComboBox_Refresh;
		_textBox.Leave += ComboBox_Refresh;
		_textBox.Resize += ComboBox_Refresh;
		base.Controls.Add(_textBox);
	}

	private void ComboBox_Refresh(object sender, EventArgs e)
	{
		Invalidate();
	}

	protected override void OnPaint(PaintEventArgs e)
	{
		base.OnPaint(e);
		if (Focused || _textBox.Focused)
		{
			e.Graphics.DrawRectangle(new Pen(Couleur.DarkColor), 1, 1, base.Width - 3, base.Height - 3);
			e.Graphics.DrawRectangle(new Pen(Couleur.LightColor), 0, 0, base.Width - 1, base.Height - 1);
		}
		else
		{
			e.Graphics.DrawRectangle(new Pen(Couleur.LightColor), 0, 0, base.Width - 1, base.Height - 1);
		}
		e.Graphics.DrawLine(new Pen(Couleur.LightColor), base.Width - 20, 0, base.Width - 20, base.Height);
		Point[] points = new Point[3]
		{
			new Point(base.Width - 14, base.Height / 2 - 2),
			new Point(base.Width - 6, base.Height / 2 - 2),
			new Point(base.Width - 10, base.Height / 2 + 2)
		};
		e.Graphics.FillPolygon(new SolidBrush(Couleur.DarkColor), points);
	}

	protected override void OnMouseUp(MouseEventArgs e)
	{
		if (e.X > base.Width - 20)
		{
			_ = e.Button;
			_ = 1048576;
		}
		base.OnMouseUp(e);
	}

	protected override void OnMouseMove(MouseEventArgs e)
	{
		if (e.X > base.Width - 20)
		{
			Cursor = Cursors.Hand;
		}
		else
		{
			Cursor = Cursors.Default;
		}
		base.OnMouseMove(e);
	}
}
