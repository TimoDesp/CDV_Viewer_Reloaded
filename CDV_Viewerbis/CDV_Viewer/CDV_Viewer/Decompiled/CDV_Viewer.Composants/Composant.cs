using System;
using System.Drawing;
using System.Windows.Forms;
using CDV_Viewer.Controls;
using CDV_Viewer.Data;
using CDV_Viewer.DockControls;

namespace CDV_Viewer.Composants;

public abstract class Composant
{
	public struct UnscaledLocation
	{
		public int Pk;

		public double PosY;

		public UnscaledLocation(int pk, double posy)
		{
			Pk = pk;
			PosY = posy;
		}

		public PointF ScaleToPointF()
		{
			ComposantsViewer viewer = ComposantsViewer.Viewer;
			float y = (float)viewer.GraphOffsetY + (float)((PosY - (double)viewer.PosVoieD) * (double)viewer.GraphHeight) / (float)(viewer.PosVoieF - viewer.PosVoieD);
			float num = (float)viewer.GraphWidth / (float)viewer.PkWidth;
			float x = ((!viewer.PkCroissant) ? ((float)viewer.GraphOffsetX + (float)(viewer.PkF - Pk) * num) : ((float)viewer.GraphOffsetX + (float)(Pk - viewer.PkD) * num));
			return new PointF(x, y);
		}

		public float ScaledYToFloat()
		{
			ComposantsViewer viewer = ComposantsViewer.Viewer;
			return (float)viewer.GraphOffsetY + (float)((PosY - (double)viewer.PosVoieD) * (double)viewer.GraphHeight) / (float)(viewer.PosVoieF - viewer.PosVoieD);
		}

		public static float ScaledYToFloat(double posy)
		{
			ComposantsViewer viewer = ComposantsViewer.Viewer;
			return (float)viewer.GraphOffsetY + (float)((posy - (double)viewer.PosVoieD) * (double)viewer.GraphHeight) / (float)(viewer.PosVoieF - viewer.PosVoieD);
		}

		public float ScaleXToFloat()
		{
			ComposantsViewer viewer = ComposantsViewer.Viewer;
			float num = (float)viewer.GraphWidth / (float)viewer.PkWidth;
			if (viewer.PkCroissant)
			{
				return (float)viewer.GraphOffsetX + (float)(Pk - viewer.PkD) * num;
			}
			return (float)viewer.GraphOffsetX + (float)(viewer.PkF - Pk) * num;
		}

		public static float ScaleXToFloat(int pk)
		{
			ComposantsViewer viewer = ComposantsViewer.Viewer;
			float num = (float)viewer.GraphWidth / (float)viewer.PkWidth;
			if (viewer.PkCroissant)
			{
				return (float)viewer.GraphOffsetX + (float)(pk - viewer.PkD) * num;
			}
			return (float)viewer.GraphOffsetX + (float)(viewer.PkF - pk) * num;
		}

		public Point Scale()
		{
			ComposantsViewer viewer = ComposantsViewer.Viewer;
			int y = viewer.GraphOffsetY + (int)Math.Round((PosY - (double)viewer.PosVoieD) * (double)viewer.GraphHeight / (double)(viewer.PosVoieF - viewer.PosVoieD));
			int x = ((!viewer.PkCroissant) ? (viewer.GraphOffsetX + (viewer.PkF - Pk) * viewer.GraphWidth / viewer.PkWidth) : (viewer.GraphOffsetX + (Pk - viewer.PkD) * viewer.GraphWidth / viewer.PkWidth));
			return new Point(x, y);
		}

		public int ScaledY()
		{
			ComposantsViewer viewer = ComposantsViewer.Viewer;
			return viewer.GraphOffsetY + (int)Math.Round((PosY - (double)viewer.PosVoieD) * (double)viewer.GraphHeight / (double)(viewer.PosVoieF - viewer.PosVoieD));
		}

		public int ScaledX()
		{
			ComposantsViewer viewer = ComposantsViewer.Viewer;
			if (viewer.PkCroissant)
			{
				return viewer.GraphOffsetX + (Pk - viewer.PkD) * viewer.GraphWidth / viewer.PkWidth;
			}
			return viewer.GraphOffsetX + (viewer.PkF - Pk) * viewer.GraphWidth / viewer.PkWidth;
		}

		public static UnscaledLocation Interpolated(int pk, UnscaledLocation p0, UnscaledLocation p1)
		{
			if (pk == p0.Pk)
			{
				return p0;
			}
			if (pk == p1.Pk)
			{
				return p1;
			}
			UnscaledLocation result = new UnscaledLocation(pk, p0.PosY);
			if (p0.Pk < pk && pk < p1.Pk)
			{
				if (p0.PosY == p1.PosY)
				{
					return result;
				}
				result.PosY = p0.PosY + (double)(pk - p0.Pk) * (p1.PosY - p0.PosY) / (double)(p1.Pk - p0.Pk);
				return result;
			}
			if (pk < p0.Pk)
			{
				result.PosY = p1.PosY - (double)Math.Sign(p1.PosY - p0.PosY);
				return result;
			}
			result.PosY = p1.PosY + (double)Math.Sign(p1.PosY - p0.PosY);
			return result;
		}

		public static Point InterpolatedScaled(int pk, UnscaledLocation p0, UnscaledLocation p1)
		{
			PointF pointF = p0.ScaleToPointF();
			PointF value = pointF;
			if (pk == p0.Pk)
			{
				return Point.Round(pointF);
			}
			PointF value2 = p1.ScaleToPointF();
			if (pk == p1.Pk)
			{
				return Point.Round(value2);
			}
			value.X = ScaleXToFloat(pk);
			if (p0.Pk < pk && pk < p1.Pk)
			{
				if (p0.PosY == p1.PosY)
				{
					return Point.Round(value);
				}
				value.Y = pointF.Y + (value.X - pointF.X) * (value2.Y - pointF.Y) / (value2.X - pointF.X);
				return Point.Round(value);
			}
			if (pk < p0.Pk)
			{
				value.Y = ScaledYToFloat(p0.PosY - (double)Math.Sign(p1.PosY - p0.PosY));
			}
			else
			{
				value.Y = ScaledYToFloat(p1.PosY + (double)Math.Sign(p1.PosY - p0.PosY));
			}
			return Point.Round(value);
		}

		public static Point InterpolatedScaled(int pk, UnscaledLocation p0, UnscaledLocation p1, out double angle)
		{
			angle = 0.0;
			PointF pointF = p0.ScaleToPointF();
			PointF value = pointF;
			if (pk == p0.Pk)
			{
				return Point.Round(pointF);
			}
			PointF value2 = p1.ScaleToPointF();
			if (pk == p1.Pk)
			{
				return Point.Round(value2);
			}
			value.X = ScaleXToFloat(pk);
			if (p0.Pk < pk && pk < p1.Pk)
			{
				if (p0.PosY == p1.PosY)
				{
					return Point.Round(value);
				}
				float num = value2.X - pointF.X;
				float num2 = value2.Y - pointF.Y;
				angle = 180.0 * Math.Atan2(num2, num) / Math.PI;
				value.Y = pointF.Y + (value.X - pointF.X) * num2 / num;
				return Point.Round(value);
			}
			if (pk < p0.Pk)
			{
				value.Y = ScaledYToFloat(p0.PosY - (double)Math.Sign(p1.PosY - p0.PosY));
			}
			else
			{
				value.Y = ScaledYToFloat(p1.PosY + (double)Math.Sign(p1.PosY - p0.PosY));
			}
			return Point.Round(value);
		}
	}

	protected Cursor _cursor = Cursors.Hand;

	protected bool _erreur;

	protected int _ordre;

	protected bool _enabled = true;

	protected bool _mouseOn;

	internal Rectangle _displayRectangle = Rectangle.Empty;

	internal LinePath _displayPath = new LinePath();

	protected int ZoomVisible;

	protected int ZoomTexteVisible;

	protected bool _mouseDown;

	public ComposantsViewer ComposantsViewer { get; protected set; }

	public ComposantsCollection Composants { get; protected set; }

	public abstract bool IsComposantSignalisation { get; }

	public Cursor Cursor => _cursor;

	public bool Erreur => _erreur;

	public int Ordre => _ordre;

	public bool Enabled => _enabled;

	public bool MouseOn => _mouseOn;

	public virtual bool Visible
	{
		get
		{
			if (IsComposantSignalisation && ComposantsViewer.ModeVisualisation != ModeVisualisation.Signalisation)
			{
				return false;
			}
			if (ZoomVisible > 0)
			{
				return ComposantsViewer.RoundedScaleX < ZoomVisible;
			}
			return true;
		}
	}

	public bool TexteVisible
	{
		get
		{
			if (ZoomTexteVisible > 0)
			{
				return ComposantsViewer.RoundedScaleX < ZoomTexteVisible;
			}
			return true;
		}
	}

	public virtual Rectangle DisplayRectangle => _displayRectangle;

	public LinePath DisplayPath => _displayPath;

	public int Id { get; protected set; } = -1;

	internal void OnComponentAddedInCollection(ComposantsCollection composants)
	{
		Composants = composants;
		ComposantsViewer = composants.ComposantsViewer;
		RecalculPKs();
	}

	public Composant()
	{
	}

	public void Select()
	{
		_mouseOn = true;
	}

	public void Deselect()
	{
		_mouseOn = false;
	}

	public virtual void RecalculPKs()
	{
	}

	public virtual bool Contains(Point Pt)
	{
		return _displayPath.Contains(Pt, 5);
	}

	public virtual bool Contains(Point Pt, int pkPoint)
	{
		return false;
	}

	public virtual bool IsInGraph()
	{
		return false;
	}

	public virtual Point GetPoint(int Pk)
	{
		return Point.Empty;
	}

	public virtual Point[] GetPath(int PkD, int PkF)
	{
		return null;
	}

	public ComposantMenu GetMenu()
	{
		ComposantsViewer.PopupContainer.State = PopupState.Hidden;
		ComposantsViewer.Invalidate();
		return GetContextMenu();
	}

	public virtual ComposantMenu GetContextMenu()
	{
		return null;
	}

	public virtual PopupForm GetPropertyWindow()
	{
		return null;
	}

	public virtual void OnComposantsLoaded()
	{
	}

	public bool Paint(PaintEventArgs e)
	{
		return OnPaint(e);
	}

	public void MouseEnter()
	{
		foreach (Composant composant in Composants)
		{
			composant._mouseOn = false;
		}
		_mouseOn = true;
		OnMouseEnter();
	}

	public void MouseLeave()
	{
		_mouseOn = false;
		OnMouseLeave();
	}

	public void MouseMove(MouseEventArgs e)
	{
		OnMouseMove(e);
	}

	public void MouseUp(MouseEventArgs e)
	{
		_mouseDown = false;
		OnMouseUp(e);
	}

	public void MouseDown(MouseEventArgs e)
	{
		_mouseDown = true;
		OnMouseDown(e);
	}

	public void MouseClick(MouseEventArgs e)
	{
		OnMouseClick(e);
	}

	public void MouseDoubleClick(MouseEventArgs e)
	{
		OnMouseDoubleClick(e);
	}

	protected virtual void OnMouseMove(MouseEventArgs e)
	{
	}

	protected virtual void OnMouseDown(MouseEventArgs e)
	{
	}

	protected virtual void OnMouseUp(MouseEventArgs e)
	{
	}

	protected virtual void OnMouseEnter()
	{
	}

	protected virtual void OnMouseLeave()
	{
	}

	protected virtual void OnMouseClick(MouseEventArgs e)
	{
	}

	protected virtual void OnMouseDoubleClick(MouseEventArgs e)
	{
	}

	protected abstract bool OnPaint(PaintEventArgs e);

	public bool EditModeError(bool conditon, string ErrorMessage)
	{
		if (conditon && Global.ModeEdition)
		{
			MessageBox.Show(ErrorMessage);
			return true;
		}
		return false;
	}
}
