using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.IO;
using System.Windows.Forms;
using CDV_Viewer.Data.SIGObjects;
using CDV_Viewer.DockControls;
using CDV_Viewer.Styles;
using CDV_Viewer.Traitements;

namespace CDV_Viewer.Controls;

public class SIGModeleViewer : UserControl
{
	private static StringFormat _textCenterFormat = new StringFormat
	{
		Alignment = StringAlignment.Center,
		LineAlignment = StringAlignment.Center
	};

	private static StringFormat _textLeftFormat = new StringFormat
	{
		LineAlignment = StringAlignment.Center
	};

	private static StringFormat _textRightFormat = new StringFormat
	{
		Alignment = StringAlignment.Far,
		LineAlignment = StringAlignment.Center
	};

	private static Pen _dashPen = new Pen(Color.Gray, 1f)
	{
		DashStyle = DashStyle.Dash
	};

	public int LineWidth = 2;

	private bool _statique;

	private SIGModele _modele;

	private int _xMouse;

	private int _yMouse;

	private int MARGIN = 5;

	private int ECHELLE_X_WIDTH = 30;

	private int ECHELLE_Y_HEIGHT = 15;

	private int ECHELLE_Y = 500;

	private IContainer components;

	public bool Statique
	{
		get
		{
			return _statique;
		}
		set
		{
			_statique = value;
		}
	}

	public SIGModele Modele
	{
		get
		{
			return _modele;
		}
		set
		{
			_modele = value;
			Invalidate();
		}
	}

	public SIGModeleViewer()
	{
		InitializeComponent();
		base.MouseMove += SIGModeleViewer_MouseMove;
		base.MouseUp += SIGModeleViewer_MouseUp;
		base.Paint += SIGModeleViewer_Paint;
		base.Resize += SIGModeleViewer_Resize;
		ComposantsViewer.Viewer.SensPkChanged += ComposantsViewer_SensPkChanged;
	}

	private void SIGModeleViewer_MouseMove(object sender, MouseEventArgs e)
	{
		_xMouse = e.X;
		_yMouse = e.Y;
		if (!_statique)
		{
			Invalidate();
		}
	}

	private void SIGModeleViewer_MouseUp(object sender, MouseEventArgs e)
	{
		if (e.Button == MouseButtons.Right && !_statique && Modele != null)
		{
			SIGContextMenuStrip sIGContextMenuStrip = new SIGContextMenuStrip();
			sIGContextMenuStrip.Items.Add("Exporter...", null, Menu_Exporter);
			sIGContextMenuStrip.Show(this, e.Location);
		}
		else if (Control.ModifierKeys == Keys.Control)
		{
			int count = _modele.Condos.Count;
			int num = 0;
			if (_modele.Condos.Count > 0)
			{
				num = _modele.Condos[0] - _modele.Points[0].X;
			}
			double num2 = 0.0;
			if (_modele.Condos.Count > 1)
			{
				num2 = (double)(_modele.Condos[count - 1] - _modele.Condos[0]) / ((double)count - 1.0);
			}
			int num3 = 0;
			if (_modele.Condos.Count > 1)
			{
				num3 = _modele.Condos[_modele.Condos.Count - 1] - _modele.Condos[0] + 2 * num;
			}
			MessageBox.Show("NbCondos : " + count + "\nDemi-pas : " + num + "\nPas : " + num2 + "\nLongueur : " + num3);
		}
	}

	private void DrawPk(Graphics g, Pen pen, Font font, int x, int pk, bool drawLabel, bool drawCondo = false)
	{
		g.DrawLine(pen, x, MARGIN, x, base.Height - MARGIN - ECHELLE_Y_HEIGHT);
		if (drawLabel)
		{
			g.DrawString(Chaines.PkToString(pk), font, Brushes.DimGray, new Point(x, base.Height - MARGIN - ECHELLE_Y_HEIGHT / 2), _textCenterFormat);
		}
		if (drawCondo)
		{
			Pen pen2 = new Pen(Color.Black, 2f);
			g.DrawLine(Pens.Black, x, MARGIN, x, MARGIN + 4);
			g.DrawLine(Pens.Black, x, MARGIN + 10, x, MARGIN + 14);
			g.DrawLine(pen2, x - 4, MARGIN + 5, x + 4, MARGIN + 5);
			g.DrawLine(pen2, x - 4, MARGIN + 9, x + 4, MARGIN + 9);
		}
	}

	private void SIGModeleViewer_Paint(object sender, PaintEventArgs e)
	{
		SIGModele modele = Modele;
		if (modele == null)
		{
			e.Graphics.DrawString("MODÈLE NON DISPONIBLE", new Font("Arial", 12f), Brushes.Gray, DisplayRectangle, _textCenterFormat);
			return;
		}
		e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
		e.Graphics.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;
		Font font = new Font(Font.FontFamily, 7f);
		bool pkCroissant = ComposantsViewer.Viewer.PkCroissant;
		bool flag = modele.DemiJointE.Joint.PK < modele.DemiJointS.Joint.PK;
		int num = 0;
		int num2 = 0;
		int num3 = 0;
		_ = MARGIN;
		int num4 = MARGIN + ECHELLE_X_WIDTH;
		int num5 = MARGIN + 10;
		foreach (Point point4 in modele.Points)
		{
			if (point4.X > num)
			{
				num = point4.X;
			}
			if (point4.Y > num3)
			{
				num3 = point4.Y;
			}
			if (point4.Y < num2)
			{
				num2 = point4.Y;
			}
		}
		num3 = (int)(Math.Ceiling((double)num3 / (double)ECHELLE_Y) + 1.0) * ECHELLE_Y;
		double num6 = (double)(base.Width - num4 - num5) / (double)(num + 1);
		double num7 = (double)(base.Height - MARGIN * 2 - ECHELLE_Y_HEIGHT) / (double)num3;
		int num8 = 0;
		int num9 = (int)Math.Ceiling(15.0 * (double)modele.Condos.Count / (double)base.Height);
		Calculs.MinMax(modele.DemiJointE.Joint.PK, modele.DemiJointS.Joint.PK, out var min, out var max);
		if (!pkCroissant)
		{
			Calculs.Swap(ref min, ref max);
		}
		int num10 = num4;
		DrawPk(e.Graphics, _dashPen, font, num10, min, drawLabel: true);
		foreach (int condo in modele.Condos)
		{
			num10 = (int)((double)condo * num6) + num4;
			if (flag != pkCroissant)
			{
				num10 = base.Width - (int)((double)condo * num6) - num5;
			}
			int pk = condo + min;
			if (!pkCroissant)
			{
				pk = max - condo;
			}
			bool drawLabel = (num8 - 2) % num9 == 0;
			DrawPk(e.Graphics, Pens.Gray, font, num10, pk, drawLabel, !_statique);
			num8++;
		}
		num10 = base.Width - num5;
		DrawPk(e.Graphics, _dashPen, font, num10, max, drawLabel: true);
		for (int i = 0; i < num3; i += ECHELLE_Y)
		{
			int num11 = base.Height - MARGIN - ECHELLE_Y_HEIGHT - (int)((double)i * num7);
			e.Graphics.DrawString(i.ToString(), font, Brushes.DimGray, new Rectangle(MARGIN, num11 - 6, ECHELLE_X_WIDTH, 12), _textRightFormat);
			e.Graphics.DrawLine(Pens.Gray, MARGIN + ECHELLE_X_WIDTH, num11, base.Width - num5, num11);
		}
		Point point = Point.Empty;
		List<Point> list = new List<Point>();
		for (int j = 0; j < modele.Points.Count - 2; j += 2)
		{
			int num12 = modele.Points[j].X;
			double num13 = modele.Points[j + 1].X - num12;
			double num14 = modele.Points[j + 2].X - num12;
			double num15 = num13 * num13;
			double num16 = num14 * num14;
			double num17 = modele.Points[j + 1].Y - modele.Points[j].Y;
			double num18 = modele.Points[j + 2].Y - modele.Points[j].Y;
			double num19 = num13 * num16 - num15 * num14;
			double num20 = (num18 * num13 - num17 * num14) / num19;
			double num21 = (num17 * num16 - num18 * num15) / num19;
			double num22 = modele.Points[j].Y;
			Point point2 = Point.Empty;
			for (double num23 = 0.0; num23 <= num14; num23 += 1.0)
			{
				double num24 = num23 * (num20 * num23 + num21) + num22;
				num10 = (int)((num23 + (double)num12) * num6) + num4;
				int num25 = base.Height - MARGIN - ECHELLE_Y_HEIGHT - (int)(num24 * num7);
				if (flag != pkCroissant)
				{
					num10 = base.Width - (int)((num23 + (double)num12) * num6) - num5;
				}
				Point point3 = new Point(num10, num25);
				list.Add(point3);
				if (point2 != Point.Empty)
				{
					e.Graphics.DrawLine(new Pen(Colors.GetColor("CDV" + modele.Circuit.Frequence), LineWidth), point2, point3);
				}
				if (!_statique && Math.Abs(point3.X - _xMouse) < 10 && Math.Abs(point3.Y - _yMouse) < 50 && Math.Abs(point3.X - _xMouse) < Math.Abs(point.X - _xMouse))
				{
					point = new Point((int)((double)num12 + num23), (int)Math.Round(num24));
				}
				point2 = point3;
			}
		}
		if (point != Point.Empty)
		{
			Cursor = Cursors.Hand;
			num10 = (int)((double)point.X * num6) + num4;
			int num26 = base.Height - MARGIN - ECHELLE_Y_HEIGHT - (int)((double)point.Y * num7);
			if (flag != pkCroissant)
			{
				num10 = base.Width - (int)((double)point.X * num6) - num5;
			}
			e.Graphics.FillEllipse(Brushes.LightGray, num10 - 5, num26 - 5, 10, 10);
			e.Graphics.DrawEllipse(new Pen(Color.DimGray, 2f), num10 - 5, num26 - 5, 10, 10);
			string s = "PK : " + Chaines.PkToString(min + point.X);
			string s2 = "ICC : " + point.Y + "mA";
			font = new Font("Arial", 8f);
			int num27 = Math.Max(e.Graphics.MeasureString(s, font).ToSize().Width, e.Graphics.MeasureString(s2, font).ToSize().Width) + 4;
			Rectangle rect = new Rectangle(num10 + 10, num26 + 10, num27, 32);
			if (rect.Right > base.Width)
			{
				rect.Offset(-num27 - 20, 0);
			}
			e.Graphics.FillRectangle(new SolidBrush(Color.FromArgb(200, Color.White)), rect);
			e.Graphics.DrawRectangle(Pens.DimGray, rect);
			e.Graphics.DrawString(s, font, Brushes.DimGray, rect.X + 2, rect.Y + 2);
			e.Graphics.DrawString(s2, font, Brushes.DimGray, rect.X + 2, rect.Y + rect.Height / 2);
		}
		else
		{
			Cursor = Cursors.Default;
		}
		e.Graphics.SmoothingMode = SmoothingMode.Default;
	}

	private void SIGModeleViewer_Resize(object sender, EventArgs e)
	{
		Invalidate();
	}

	private void ComposantsViewer_SensPkChanged(object sender, EventArgs e)
	{
		Invalidate();
	}

	private void Menu_Exporter(object sender, EventArgs e)
	{
		SaveFileDialog saveFileDialog = new SaveFileDialog();
		saveFileDialog.Filter = "Image PNG (*.png)|*.png|Image JPG (*.jpg)|*.jpg";
		saveFileDialog.FileName = "Modele.png";
		if (saveFileDialog.ShowDialog() != DialogResult.OK)
		{
			return;
		}
		Bitmap bitmap = new Bitmap(base.Width, base.Height);
		_xMouse = (_yMouse = -1);
		Invalidate();
		DrawToBitmap(bitmap, DisplayRectangle);
		string text = Path.GetExtension(saveFileDialog.FileName).ToLower();
		if (!(text == ".png"))
		{
			if (text == ".jpg")
			{
				bitmap.Save(saveFileDialog.FileName);
			}
		}
		else
		{
			bitmap.Save(saveFileDialog.FileName);
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
		base.SuspendLayout();
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		this.DoubleBuffered = true;
		this.MinimumSize = new System.Drawing.Size(50, 50);
		base.Name = "SIGModeleViewer";
		base.Size = new System.Drawing.Size(248, 182);
		base.ResumeLayout(false);
	}
}
