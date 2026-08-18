using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace CDV_Viewer.CustomControls;

public class CustomTextBox : UserControl
{
	private CustomControlColor _couleur = CustomControlColor.Gris;

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

	[Browsable(true)]
	[EditorBrowsable(EditorBrowsableState.Always)]
	public new string Text
	{
		get
		{
			return _textBox.Text;
		}
		set
		{
			_textBox.Text = value;
		}
	}

	public int SelectionStart
	{
		get
		{
			return _textBox.SelectionStart;
		}
		set
		{
			_textBox.SelectionStart = value;
		}
	}

	public int SelectionLength
	{
		get
		{
			return _textBox.SelectionLength;
		}
		set
		{
			_textBox.SelectionLength = value;
		}
	}

	public HorizontalAlignment TextAlign
	{
		get
		{
			return _textBox.TextAlign;
		}
		set
		{
			_textBox.TextAlign = value;
		}
	}

	public new event EventHandler TextChanged;

	public CustomTextBox()
	{
		BackColor = Color.White;
		base.BorderStyle = BorderStyle.None;
		_textBox = new TextBox();
		base.Width = _textBox.Width + 4;
		base.Height = _textBox.Height + 4;
		_textBox.AutoSize = false;
		_textBox.BorderStyle = BorderStyle.None;
		_textBox.Left = (_textBox.Top = 2);
		_textBox.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
		_textBox.Enter += TextBox_Refresh;
		_textBox.Leave += TextBox_Refresh;
		_textBox.Resize += TextBox_Refresh;
		_textBox.TextChanged += TextBox_TextChanged;
		base.Controls.Add(_textBox);
		base.Padding = new Padding(1);
	}

	private void TextBox_Refresh(object sender, EventArgs e)
	{
		Invalidate();
	}

	private void TextBox_TextChanged(object sender, EventArgs e)
	{
		if (this.TextChanged != null)
		{
			this.TextChanged(this, e);
		}
	}

	protected override void OnPaint(PaintEventArgs e)
	{
		base.OnPaint(e);
		if (_textBox.Focused)
		{
			e.Graphics.DrawRectangle(new Pen(Couleur.DarkColor), 1, 1, base.Width - 3, base.Height - 3);
			e.Graphics.DrawRectangle(new Pen(Couleur.LightColor), 0, 0, base.Width - 1, base.Height - 1);
		}
		else
		{
			e.Graphics.DrawRectangle(new Pen(Couleur.LightColor), 0, 0, base.Width - 1, base.Height - 1);
		}
	}
}
