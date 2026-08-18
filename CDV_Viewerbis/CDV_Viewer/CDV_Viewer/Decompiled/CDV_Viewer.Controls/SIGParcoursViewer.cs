using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.Windows.Forms;
using CDV_Viewer.Data;
using CDV_Viewer.DockControls;

namespace CDV_Viewer.Controls;

public class SIGParcoursViewer : UserControl
{
	private class LigneEtape
	{
		public int Ligne;

		public int X;

		public int PkD;

		public int PkF;

		public int PkMin => Math.Min(PkD, PkF);

		public int PkMax => Math.Max(PkD, PkF);

		public LigneEtape(int ligne, int x, int pkD, int pkF)
		{
			Ligne = ligne;
			X = x;
			PkD = pkD;
			PkF = pkF;
		}

		public LigneEtape(EtapeParcours etape, int x)
		{
			Ligne = etape.Ligne;
			PkD = etape.PkD;
			PkF = etape.PkF;
			X = x;
		}
	}

	private ComposantsViewer ComposantsViewer = ComposantsViewer.Viewer;

	private int _deltaXMin;

	private int _rapport;

	private List<LigneEtape> _etapes = new List<LigneEtape>();

	private int _iMouse = -1;

	private int _marginH = 10;

	private IContainer components;

	private Timer timer;

	public SIGParcoursViewer()
	{
		InitializeComponent();
		base.EnabledChanged += SIGParcoursViewer_EnabledChanged;
		base.Paint += SIGParcoursViewer_Paint;
		base.Resize += SIGParcoursViewer_Resize;
		base.MouseMove += SIGParcoursViewer_MouseMove;
		base.MouseLeave += SIGParcoursViewer_MouseLeave;
		base.MouseUp += SIGParcoursViewer_MouseUp;
		timer.Tick += timer_Tick;
		Global.Parcours.ParcoursChanged += Parcours_ParcoursChanged;
	}

	private void SIGParcoursViewer_EnabledChanged(object sender, EventArgs e)
	{
		if (base.Enabled)
		{
			Initialize();
			Cursor = Cursors.Hand;
			timer.Start();
		}
		else
		{
			timer.Stop();
			_deltaXMin = -1;
			_rapport = -1;
		}
	}

	private void SIGParcoursViewer_Paint(object sender, PaintEventArgs e)
	{
		StringFormat stringFormat = new StringFormat();
		stringFormat.Alignment = StringAlignment.Center;
		stringFormat.LineAlignment = StringAlignment.Center;
		if (!Global.Parcours.IsOpen)
		{
			e.Graphics.DrawString("AUCUN PARCOURS", new Font("Arial", 16f), Brushes.Gray, DisplayRectangle, stringFormat);
			return;
		}
		e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
		e.Graphics.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;
		if (_deltaXMin >= 0 && _rapport > 0)
		{
			Graphics graphics = e.Graphics;
			Font font = new Font(SystemFonts.DefaultFont.FontFamily, 8f);
			Font font2 = new Font(SystemFonts.DefaultFont.FontFamily, 7f);
			bool flag = false;
			for (int i = 0; i < _etapes.Count; i++)
			{
				Color color = Color.White;
				Color color2 = Color.Gainsboro;
				if (flag)
				{
					color2 = Color.LightGray;
				}
				flag = !flag;
				Rectangle rectangle = new Rectangle(_etapes[i].X, 0, base.Width - _etapes[i].X, base.Height);
				if (i < _etapes.Count - 1)
				{
					rectangle = new Rectangle(_etapes[i].X, 0, _etapes[i + 1].X - _etapes[i].X, base.Height);
				}
				if (i == _iMouse)
				{
					color = color2;
				}
				LinearGradientBrush brush = new LinearGradientBrush(rectangle, color2, color, 90f);
				graphics.FillRectangle(brush, rectangle);
				int num = rectangle.Width / 40;
				if (num > 1)
				{
					int num2 = rectangle.X + 20;
					for (int j = 0; j < num; j++)
					{
						int num3 = _etapes[i].PkD + (num2 - rectangle.X) * (_etapes[i].PkF - _etapes[i].PkD) / rectangle.Width;
						graphics.DrawString(num3.ToString(), font2, new SolidBrush(Color.Gray), num2 - num3.ToString().Length * 3, base.Height - 14);
						Pen pen = new Pen(Color.Gray);
						pen.DashStyle = DashStyle.Dash;
						graphics.DrawLine(pen, num2, 0, num2, base.Height - 14);
						num2 += 40;
					}
				}
				graphics.DrawString(_etapes[i].Ligne.ToString(), font, new SolidBrush(Color.Black), rectangle, stringFormat);
				if (i > 0)
				{
					graphics.DrawLine(Pens.DimGray, rectangle.X, rectangle.Y, rectangle.X, rectangle.Bottom);
				}
			}
			ComposantsViewer viewer = ComposantsViewer.Viewer;
			int num4 = 0;
			if (num4 == int.MinValue)
			{
				num4 = (viewer.PkD + viewer.PkF) / 2;
			}
			int delta = Global.Parcours.GetDelta(ComposantsViewer.LigneId, num4);
			if (delta > int.MinValue)
			{
				int num5 = (delta - _deltaXMin) / _rapport;
				graphics.DrawLine(new Pen(Color.Red, 2f), num5, 0, num5, base.Height);
			}
		}
		e.Graphics.SmoothingMode = SmoothingMode.Default;
	}

	private void SIGParcoursViewer_Resize(object sender, EventArgs e)
	{
		Initialize();
	}

	private void SIGParcoursViewer_MouseMove(object sender, MouseEventArgs e)
	{
		_iMouse = -1;
		for (int i = 0; i < _etapes.Count; i++)
		{
			Rectangle rectangle = new Rectangle(_etapes[i].X, 0, base.Width - _etapes[i].X, base.Height);
			if (i < _etapes.Count - 1)
			{
				rectangle = new Rectangle(_etapes[i].X, 0, _etapes[i + 1].X - _etapes[i].X, base.Height);
			}
			if (rectangle.Contains(e.Location))
			{
				_iMouse = i;
			}
		}
		Invalidate();
	}

	private void SIGParcoursViewer_MouseLeave(object sender, EventArgs e)
	{
		_iMouse = -1;
		Invalidate();
	}

	private void SIGParcoursViewer_MouseUp(object sender, MouseEventArgs e)
	{
		for (int i = 0; i < _etapes.Count; i++)
		{
			int num = base.Width;
			if (i < _etapes.Count - 1)
			{
				num = _etapes[i + 1].X;
			}
			if (_etapes[i].X < e.X && num > e.X)
			{
				int pk = _etapes[i].PkD + (e.X - _etapes[i].X) * (_etapes[i].PkF - _etapes[i].PkD) / (num - _etapes[i].X);
				ComposantsViewer.SetLignePK(_etapes[i].Ligne, pk);
				ComposantsViewer.PkCroissant = _etapes[i].PkD < _etapes[i].PkF;
				break;
			}
		}
	}

	private void timer_Tick(object sender, EventArgs e)
	{
		if (base.Enabled && Global.Parcours.Etapes.Count > 0)
		{
			Invalidate();
		}
	}

	private void Parcours_ParcoursChanged(object sender, EventArgs e)
	{
		Initialize();
		SIGParcoursViewer_MouseUp(this, new MouseEventArgs(MouseButtons.Left, 1, 1, 0, 0));
	}

	private void Initialize()
	{
		if (!Global.Parcours.IsOpen)
		{
			return;
		}
		_deltaXMin = Global.Parcours.Etapes[0].DeltaXD;
		int deltaXF = Global.Parcours.Etapes[Global.Parcours.Etapes.Count - 1].DeltaXF;
		_rapport = (deltaXF - _deltaXMin) / (base.Width - _marginH * 2);
		int num = 0;
		_etapes.Clear();
		foreach (EtapeParcours etape in Global.Parcours.Etapes)
		{
			if (num != etape.Ligne)
			{
				int num2 = (etape.DeltaXD - _deltaXMin) / _rapport;
				if (_etapes.Count == 0 || _etapes[_etapes.Count - 1].X != num2)
				{
					_etapes.Add(new LigneEtape(etape, num2));
					num = etape.Ligne;
				}
			}
			else
			{
				_etapes[_etapes.Count - 1].PkF = etape.PkF;
			}
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
		this.timer = new System.Windows.Forms.Timer(this.components);
		base.SuspendLayout();
		this.timer.Interval = 500;
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		this.BackColor = System.Drawing.Color.White;
		this.DoubleBuffered = true;
		base.Name = "SIGParcoursViewer";
		base.Size = new System.Drawing.Size(399, 89);
		base.ResumeLayout(false);
	}
}
