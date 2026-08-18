using System;
using System.Drawing;
using System.Windows.Forms;
using CDV_Viewer.Data;

namespace CDV_Viewer.Controls;

public class PopupContainer : DockChild
{
	public static PopupContainer Popup;

	private readonly Brush _backGroundBrush = new SolidBrush(Color.White);

	private readonly Brush _obscureBrush = new SolidBrush(Color.FromArgb(100, Color.Black));

	private readonly Pen _borderPen = new Pen(Brushes.Gray, 1f);

	private PopupState _state;

	private Separator separator;

	private CloseButton bFermer;

	private Label lTitre;

	protected Button bValider;

	protected Button bOui;

	protected Button bNon;

	private Bitmap _formBitmap;

	private PopupForm _form;

	public PopupState State
	{
		get
		{
			return _state;
		}
		set
		{
			_state = value;
			RefreshFormPosition();
			switch (_state)
			{
			case PopupState.Maximized:
			{
				base.Visible = true;
				if (_form != null)
				{
					_form.Visible = true;
				}
				Dock = DockStyle.Fill;
				Label label3 = lTitre;
				CloseButton closeButton3 = bFermer;
				bool flag = (separator.Visible = true);
				bool visible = (closeButton3.Visible = flag);
				label3.Visible = visible;
				break;
			}
			case PopupState.Hidden:
				base.Visible = false;
				CloseForm(PopupContainerResult.Cancel);
				break;
			case PopupState.Info:
			{
				base.Visible = true;
				if (_form != null)
				{
					_form.Visible = false;
				}
				Label label2 = lTitre;
				CloseButton closeButton2 = bFermer;
				bool flag = (separator.Visible = false);
				bool visible = (closeButton2.Visible = flag);
				label2.Visible = visible;
				Dock = DockStyle.None;
				break;
			}
			case PopupState.Edit:
			{
				base.Visible = true;
				Dock = DockStyle.None;
				Label label = lTitre;
				CloseButton closeButton = bFermer;
				bool flag = (separator.Visible = false);
				bool visible = (closeButton.Visible = flag);
				label.Visible = visible;
				break;
			}
			}
			Invalidate();
		}
	}

	public bool HasButton
	{
		get
		{
			if (!bValider.Visible && !bOui.Visible)
			{
				return bNon.Visible;
			}
			return true;
		}
	}

	public PopupContainer()
	{
		InitializeComponent();
		Popup = this;
		base.Visible = false;
	}

	private void InitializeComponent()
	{
		this.lTitre = new System.Windows.Forms.Label();
		this.lTitre.AutoSize = false;
		this.lTitre.Font = new System.Drawing.Font("Arial", 9f);
		this.lTitre.ForeColor = System.Drawing.Color.FromArgb(224, 82, 6);
		this.lTitre.Text = "PROPRIÉTÉS";
		base.Controls.Add(this.lTitre);
		this.bFermer = new CDV_Viewer.Controls.CloseButton();
		this.bFermer.BackColor = System.Drawing.Color.White;
		this.bFermer.Click += new System.EventHandler(bFermer_Click);
		base.Controls.Add(this.bFermer);
		this.separator = new CDV_Viewer.Controls.Separator();
		this.separator.ForeColor = System.Drawing.Color.Gainsboro;
		this.separator.Text = "separator1";
		base.Controls.Add(this.separator);
		this.bValider = new System.Windows.Forms.Button();
		this.bValider.Visible = false;
		this.bValider.Text = "Valider";
		this.bValider.Click += new System.EventHandler(bValider_Click);
		base.Controls.Add(this.bValider);
		this.bOui = new System.Windows.Forms.Button();
		this.bOui.Visible = false;
		this.bOui.Text = "Oui";
		this.bOui.Click += new System.EventHandler(bOui_Click);
		base.Controls.Add(this.bOui);
		this.bNon = new System.Windows.Forms.Button();
		this.bNon.Visible = false;
		this.bNon.Text = "Non";
		this.bNon.Click += new System.EventHandler(bNon_Click);
		base.Controls.Add(this.bNon);
		this.BackColor = System.Drawing.Color.Transparent;
		this.DoubleBuffered = true;
	}

	private void RefreshFormPosition()
	{
		if (_form == null)
		{
			return;
		}
		switch (_state)
		{
		case PopupState.Maximized:
			_form.Left = (base.Width - _form.Width) / 2;
			_form.Top = (base.Height - _form.Height - 30) / 2;
			lTitre.Left = _form.Left + 6;
			lTitre.Top = _form.Top + 6;
			bFermer.Left = _form.Right - 22;
			bFermer.Top = _form.Top + 7;
			separator.Left = _form.Left + 10;
			separator.Top = _form.Top + 28;
			separator.Width = _form.Width - 20;
			if (bValider.Visible || bOui.Visible || bNon.Visible)
			{
				lTitre.Top -= (bValider.Height + 10) / 2;
				bFermer.Top -= (bValider.Height + 10) / 2;
				separator.Top -= (bValider.Height + 10) / 2;
				_form.Top -= (bValider.Height + 10) / 2;
				Button button = bValider;
				Button button2 = bOui;
				int num2 = (bNon.Top = _form.Bottom + 30);
				int left = (button2.Top = num2);
				button.Top = left;
				bValider.Left = _form.Left + (_form.Width - bValider.Width) / 2;
				bOui.Left = _form.Left + _form.Width / 3 - bOui.Width / 2;
				bNon.Left = _form.Left + 2 * _form.Width / 3 - bNon.Width / 2;
			}
			_form.Top += 30;
			break;
		case PopupState.Info:
		case PopupState.Edit:
		{
			PopupForm form = _form;
			int left = (_form.Top = 1);
			form.Left = left;
			base.Width = _form.Width + 2;
			base.Height = _form.Height + 2;
			break;
		}
		}
	}

	private bool CloseForm(PopupContainerResult result)
	{
		if (_form == null)
		{
			return false;
		}
		PopupFormResultEventArgs e = new PopupFormResultEventArgs(result);
		_form.Close(e);
		if (e.Canceled)
		{
			return false;
		}
		return true;
	}

	public void CloseForm()
	{
		if (_form != null)
		{
			State = PopupState.Hidden;
			_form = null;
		}
	}

	public void Show(PopupForm form)
	{
		Show(form, string.Empty, PopupContainerButtons.None, PopupState.Info);
	}

	public void Show(PopupForm form, PopupState state)
	{
		Show(form, string.Empty, PopupContainerButtons.None, state);
	}

	public void Show(PopupForm form, string caption)
	{
		Show(form, caption, PopupContainerButtons.None, PopupState.Maximized);
	}

	public void Show(PopupForm form, string caption, PopupContainerButtons buttons)
	{
		Show(form, caption, buttons, PopupState.Maximized);
	}

	private void Show(PopupForm form, string caption, PopupContainerButtons buttons, PopupState state)
	{
		if (_state == PopupState.Maximized || _state == PopupState.Edit)
		{
			return;
		}
		CloseForm(PopupContainerResult.Cancel);
		BringToFront();
		if (form == null || form.Width <= 0 || form.Height <= 0)
		{
			State = PopupState.Hidden;
			return;
		}
		_form = form;
		_form.Disposed += Form_Disposed;
		_form.BackColor = Color.White;
		_formBitmap = new Bitmap(_form.Width, _form.Height);
		_form.DrawToBitmap(_formBitmap, _form.Bounds);
		base.Controls.Add(_form);
		base.Visible = state != PopupState.Hidden;
		lTitre.Text = caption.ToUpper();
		lTitre.AutoSize = true;
		Button button = bOui;
		bool visible = (bNon.Visible = buttons == PopupContainerButtons.OuiNon);
		button.Visible = visible;
		bValider.Visible = buttons == PopupContainerButtons.Valider;
		State = state;
		Focus();
		if (state != PopupState.Info)
		{
			_form.ComposantsViewer.MustDrawComponent = true;
		}
	}

	public void SetPosition(Point pt)
	{
		if (!(pt == base.Location))
		{
			Point empty = Point.Empty;
			int num = base.Width + 20;
			int num2 = base.Height + 20;
			if (pt.X + num <= _form.ComposantsViewer.Width || pt.X - num <= 0)
			{
				empty.X = pt.X + 20;
			}
			else
			{
				empty.X = pt.X - num;
			}
			if (pt.Y + num2 <= _form.ComposantsViewer.Height || pt.Y - num2 <= 0)
			{
				empty.Y = pt.Y + 20;
			}
			else
			{
				empty.Y = pt.Y - num2;
			}
			base.Location = empty;
		}
	}

	protected override void OnPaint(PaintEventArgs e)
	{
		Graphics graphics = e.Graphics;
		switch (_state)
		{
		case PopupState.Maximized:
		{
			graphics.FillRectangle(_obscureBrush, DisplayRectangle);
			Rectangle rect = new Rectangle(_form.Left - 1, _form.Top - 31, _form.Width + 1, _form.Height + 31);
			if (HasButton)
			{
				rect.Height += bValider.Height + 10;
			}
			graphics.FillRectangle(_backGroundBrush, rect);
			graphics.DrawRectangle(_borderPen, rect);
			break;
		}
		case PopupState.Info:
			graphics.DrawRectangle(_borderPen, 0, 0, base.Width - 1, base.Height - 1);
			if (_formBitmap != null)
			{
				graphics.DrawImage(_formBitmap, _form.Location);
			}
			break;
		case PopupState.Edit:
			graphics.DrawRectangle(_borderPen, 0, 0, base.Width - 1, base.Height - 1);
			break;
		}
	}

	protected override void OnKeyUp(KeyEventArgs e)
	{
		if (e.KeyCode == Keys.Escape)
		{
			bFermer.PerformClick();
		}
	}

	protected override void OnResize(EventArgs e)
	{
		RefreshFormPosition();
	}

	private void Form_Disposed(object sender, EventArgs e)
	{
		base.Controls.Remove(_form);
		_form.Disposed -= Form_Disposed;
		_form = null;
		State = PopupState.Hidden;
	}

	private void bFermer_Click(object sender, EventArgs e)
	{
		if (CloseForm(PopupContainerResult.Cancel))
		{
			State = PopupState.Hidden;
		}
	}

	private void bValider_Click(object sender, EventArgs e)
	{
		if (CloseForm(PopupContainerResult.OK))
		{
			State = PopupState.Hidden;
		}
	}

	private void bOui_Click(object sender, EventArgs e)
	{
		if (CloseForm(PopupContainerResult.Non))
		{
			State = PopupState.Hidden;
		}
	}

	private void bNon_Click(object sender, EventArgs e)
	{
		if (CloseForm(PopupContainerResult.Oui))
		{
			State = PopupState.Hidden;
		}
	}
}
