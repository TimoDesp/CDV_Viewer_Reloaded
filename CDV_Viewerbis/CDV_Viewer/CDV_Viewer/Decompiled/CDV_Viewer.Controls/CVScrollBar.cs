using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using CDV_Viewer.Composants;
using CDV_Viewer.DockControls;
using CDV_Viewer.Styles;

namespace CDV_Viewer.Controls;

public class CVScrollBar : UserControl
{
	protected readonly ComposantsViewer ComposantsViewer = ComposantsViewer.Viewer;

	private SolidBrush _maskBrush = new SolidBrush(Color.FromArgb(60, Color.Black));

	private Pen _trackPen = new Pen(Colors.Voie, 2f);

	private ComposantsCollection _composants;

	private bool _mouseDown;

	private IContainer components;

	private void Invoke(Action action)
	{
		Invoke((Delegate)action);
	}

	public CVScrollBar()
	{
		InitializeComponent();
		base.Load += delegate
		{
			InitializeExternEvent();
		};
	}

	public void Refresh(ComposantsCollection composants)
	{
		_composants = composants;
		Invalidate();
	}

	private int PkToLocation(int pk)
	{
		int num = ComposantsViewer.PkDLigne - ComposantsViewer.CurrentMargin;
		int num2 = ComposantsViewer.PkFLigne + ComposantsViewer.CurrentMargin;
		return (ComposantsViewer.PkCroissant ? (pk - num) : (num2 - pk)) * base.Width / (num2 - num);
	}

	private void PksToBounds(int pkd, int pkf, out int xd, out int xf)
	{
		int num = ComposantsViewer.PkDLigne - ComposantsViewer.CurrentMargin;
		int num2 = ComposantsViewer.PkFLigne + ComposantsViewer.CurrentMargin;
		if (ComposantsViewer.PkCroissant)
		{
			xd = (pkd - num) * base.Width / (num2 - num);
			xf = (pkf - num) * base.Width / (num2 - num);
		}
		else
		{
			xd = (num2 - pkf) * base.Width / (num2 - num);
			xf = (num2 - pkd) * base.Width / (num2 - num);
		}
	}

	private int LocationToPk(int x)
	{
		int num = ComposantsViewer.PkDLigne - ComposantsViewer.CurrentMargin;
		int num2 = ComposantsViewer.PkFLigne + ComposantsViewer.CurrentMargin;
		int num3 = x * (num2 - num) / base.Width;
		if (!ComposantsViewer.PkCroissant)
		{
			return num2 - num3;
		}
		return num + num3;
	}

	private int PosVoieToLocation(double posy)
	{
		return (int)((posy - (double)ComposantsViewer.PosVoieD) / (double)(ComposantsViewer.PosVoieF - ComposantsViewer.PosVoieD) * (double)base.Height);
	}

	private Point Scale(Composant.UnscaledLocation p)
	{
		return new Point
		{
			X = PkToLocation(p.Pk),
			Y = PosVoieToLocation(p.PosY)
		};
	}

	private void InitializeExternEvent()
	{
		if (ComposantsViewer != null)
		{
			ComposantsViewer.SensPkChanged += delegate
			{
				Invalidate();
			};
		}
	}

	protected override void OnPaint(PaintEventArgs e)
	{
		if (_composants == null || ComposantsViewer.CurrentMargin == 0 || ComposantsViewer.LongueurLigne <= 2 * ComposantsViewer.CurrentMargin)
		{
			return;
		}
		Graphics graphics = e.Graphics;
		LinearGradientBrush brush = new LinearGradientBrush(DisplayRectangle, Color.Gainsboro, Color.White, 90f);
		graphics.FillRectangle(brush, DisplayRectangle);
		if (_composants.Count == 0)
		{
			StringFormat stringFormat = new StringFormat();
			stringFormat.Alignment = StringAlignment.Center;
			stringFormat.LineAlignment = StringAlignment.Center;
			graphics.DrawString("Aucune voie", new Font("Calibri", 16f, FontStyle.Bold), Brushes.DimGray, DisplayRectangle, stringFormat);
			return;
		}
		foreach (CVoieOnLine voiesOnLine in _composants.VoiesOnLines)
		{
			graphics.DrawLines(_trackPen, voiesOnLine.GetFullPath(Scale));
		}
		PksToBounds(ComposantsViewer.PkD, ComposantsViewer.PkF, out var xd, out var xf);
		graphics.FillRectangle(_maskBrush, 0, 0, xd, base.Height);
		graphics.FillRectangle(_maskBrush, xf, 0, base.Width - xf, base.Height);
		graphics.DrawLine(Pens.Black, xd - 1, 0, xd - 1, base.Height);
		graphics.DrawLine(Pens.Black, xf, 0, xf, base.Height);
	}

	protected override void OnResize(EventArgs e)
	{
		if (_composants != null && _composants.Count != 0)
		{
			Invalidate();
		}
	}

	protected override bool IsInputKey(Keys keyData)
	{
		if (keyData == Keys.Left || keyData == Keys.Right)
		{
			return true;
		}
		return base.IsInputKey(keyData);
	}

	protected override void OnMouseMove(MouseEventArgs e)
	{
		if (_composants != null && _composants.Count != 0 && _mouseDown && e.X >= -1 && e.X <= base.Width + 1 && e.Y >= -1 && e.Y <= base.Height + 1)
		{
			ComposantsViewer.MoveToPkCenter(LocationToPk(e.X));
		}
	}

	protected override void OnMouseDown(MouseEventArgs e)
	{
		if (_composants != null && _composants.Count != 0)
		{
			_mouseDown = true;
			ComposantsViewer.MoveToPkCenter(LocationToPk(e.X));
		}
	}

	protected override void OnMouseUp(MouseEventArgs e)
	{
		if (_composants != null && _composants.Count > 0)
		{
			_mouseDown = false;
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
		base.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
		this.DoubleBuffered = true;
		base.Name = "SIGScrollBar";
		base.Size = new System.Drawing.Size(514, 99);
		base.ResumeLayout(false);
	}
}
