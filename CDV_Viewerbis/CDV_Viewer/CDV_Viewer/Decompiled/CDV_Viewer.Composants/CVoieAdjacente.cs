using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using CDV_Viewer.Controls;
using CDV_Viewer.Data;
using CDV_Viewer.Data.SIGObjects;
using CDV_Viewer.PopupForms;
using CDV_Viewer.Styles;

namespace CDV_Viewer.Composants;

public class CVoieAdjacente : CVoie
{
	private class DrawPath
	{
		private const int Width = 20;

		private const int Heigth = 24;

		private const int HalfWidth = 10;

		private const int HalfHeigth = 12;

		private const int Shifty = 5;

		private const int Shiftx = 5;

		private Size[] _offset;

		private Size _startArrow;

		private Size _endArrow;

		private Size _NameAreaOffset;

		private Size _NameAreaSize;

		private string _label = "";

		public DrawPath(CVoieAdjacente voie)
		{
			Graphics graphics = voie.ComposantsViewer.CreateGraphics();
			_label = $"{voie.Voie.Ligne.ID} {voie.Voie.Nom}";
			_NameAreaSize = Size.Ceiling(graphics.MeasureString(_label, CVoie._fontNomVoie));
			bool pkCroissant = voie.ComposantsViewer.PkCroissant;
			CVoieOnLine support = voie.Support;
			support.Voie.IsJonction();
			SIGNoeud noeud = voie.Noeud;
			bool changementSens = voie._changementSens;
			bool flag = false;
			bool flag2 = false;
			if (pkCroissant)
			{
				flag = noeud == support.Voie.NoeudDebut;
				flag2 = noeud == support.Voie.NoeudFin;
			}
			else
			{
				flag2 = noeud == support.Voie.NoeudDebut;
				flag = noeud == support.Voie.NoeudFin;
			}
			if (noeud.Type == SIGNoeud.NoeudType.ChangementLigne || noeud.Branches.Count > 3)
			{
				SetChangementLigne(flag, changementSens);
			}
			else if (!flag && !flag2)
			{
				if (noeud.Type == SIGNoeud.NoeudType.BranchementLignePointe)
				{
					SetBranchementPointe(support, changementSens);
				}
				else if (noeud.Type == SIGNoeud.NoeudType.BranchementLigneTalon)
				{
					SetBranchementTalon(support, changementSens);
				}
				else
				{
					SetBranchementError();
				}
			}
			else if (flag)
			{
				SetBranchementDebut(changementSens);
			}
			else if (flag2)
			{
				SetBranchementFin(changementSens);
			}
			else
			{
				SetBranchementError();
			}
		}

		private void SetChangementLigne(bool isDebut, bool changementSensPK)
		{
			if (isDebut)
			{
				SetChangementLigneDebut(changementSensPK);
			}
			else
			{
				SetChangementLigneFin(changementSensPK);
			}
		}

		private void SetChangementLigneDebut(bool changementSensPK)
		{
			Size empty = Size.Empty;
			Size size = new Size(-40, 0);
			_offset = new Size[2] { empty, size };
			empty.Width = -20;
			int height = (size.Height = -5);
			empty.Height = height;
			if (changementSensPK)
			{
				_startArrow = size;
				_endArrow = empty;
			}
			else
			{
				_startArrow = empty;
				_endArrow = size;
			}
			_NameAreaOffset = new Size(-(_NameAreaSize.Width + 10), 2);
		}

		private void SetChangementLigneFin(bool changementSensPK)
		{
			Size empty = Size.Empty;
			Size size = new Size(40, 0);
			_offset = new Size[2] { empty, size };
			empty.Width = 20;
			int height = (size.Height = -5);
			empty.Height = height;
			if (changementSensPK)
			{
				_startArrow = size;
				_endArrow = empty;
			}
			else
			{
				_startArrow = empty;
				_endArrow = size;
			}
			_NameAreaOffset = new Size(10, 2);
		}

		private void SetBranchementPointe(CVoieOnLine support, bool changementSensPK)
		{
			int num = ((support.PosY >= 0) ? 24 : (-24));
			int height = ((num > 0) ? num : (num - _NameAreaSize.Height));
			Size size = new Size(20, num);
			_offset = new Size[2]
			{
				size,
				Size.Empty
			};
			Size size2 = new Size(5, num / 2);
			size = new Size(15, num);
			if (changementSensPK)
			{
				_startArrow = size;
				_endArrow = size2;
			}
			else
			{
				_startArrow = size2;
				_endArrow = size;
			}
			_NameAreaOffset = new Size(25, height);
		}

		private void SetBranchementTalon(CVoieOnLine support, bool changementSensPK)
		{
			int num = ((support.PosY >= 0) ? 24 : (-24));
			int height = ((num > 0) ? num : (num - _NameAreaSize.Height));
			Size size = new Size(-20, num);
			_offset = new Size[2]
			{
				size,
				Size.Empty
			};
			Size size2 = new Size(-15, num / 2);
			size = new Size(-25, num);
			if (changementSensPK)
			{
				_startArrow = size;
				_endArrow = size2;
			}
			else
			{
				_startArrow = size2;
				_endArrow = size;
			}
			_NameAreaOffset = new Size(-(20 + _NameAreaSize.Width + 5), height);
		}

		private void SetBranchementDebut(bool changementSensPK)
		{
			Size size = new Size(-20, 24);
			Size size2 = new Size(20, -24);
			_offset = new Size[3]
			{
				size,
				Size.Empty,
				size2
			};
			size = new Size(-15, 12);
			size2 = new Size(5, -12);
			if (changementSensPK)
			{
				_startArrow = size2;
				_endArrow = size;
			}
			else
			{
				_startArrow = size;
				_endArrow = size2;
			}
			_NameAreaOffset = new Size(-(_NameAreaSize.Width + 10), -_NameAreaSize.Height / 2);
		}

		private void SetBranchementFin(bool changementSensPK)
		{
			Size size = new Size(-20, 24);
			Size size2 = new Size(20, -24);
			_offset = new Size[3]
			{
				size,
				Size.Empty,
				size2
			};
			size = new Size(-5, 12);
			size2 = new Size(15, -12);
			if (changementSensPK)
			{
				_startArrow = size2;
				_endArrow = size;
			}
			else
			{
				_startArrow = size;
				_endArrow = size2;
			}
			_NameAreaOffset = new Size(10, -_NameAreaSize.Height / 2);
		}

		private void SetBranchementError()
		{
			_offset = new Size[3]
			{
				new Size(0, 24),
				Size.Empty,
				new Size(0, -24)
			};
			_startArrow = (_endArrow = Size.Empty);
			_NameAreaOffset = new Size(-_NameAreaSize.Width / 2, -(_NameAreaSize.Height + 5));
		}

		public Point[] GetPoints(Point center)
		{
			return Array.ConvertAll(_offset, (Size s) => Point.Add(center, s));
		}

		public Rectangle GetTextArea(Point center)
		{
			return new Rectangle(Point.Add(center, _NameAreaOffset), _NameAreaSize);
		}

		public Rectangle DrawLabel(Graphics g, Pen pen, Point center)
		{
			Rectangle rectangle = new Rectangle(Point.Add(center, _NameAreaOffset), _NameAreaSize);
			g.DrawString(_label, CVoie._fontNomVoie, pen.Brush, rectangle, CVoie._formatTexteNomVoie);
			return rectangle;
		}

		public void DrawArrow(Graphics g, Pen pen, Point center)
		{
			Point pt = Point.Add(center, _startArrow);
			Point pt2 = Point.Add(center, _endArrow);
			g.DrawLine(pen, pt, pt2);
		}
	}

	private static readonly Pen VoieAdjacentePen = new Pen(Colors.AutreLigne, 2f);

	private static readonly Pen VoieAdjacentePenSelected = new Pen(Colors.AutreLigneSelected, 2f);

	private DrawPath _pathData;

	private bool _changementSens;

	private Rectangle nomVoieRectangle = Rectangle.Empty;

	public SIGNoeud Noeud { get; }

	public int Pk { get; private set; }

	public int PkDestination { get; private set; }

	public CVoieOnLine Support { get; private set; }

	public override int PosYDebut => Support?.PosYDebut ?? 0;

	public override int PosYFin => Support?.PosYFin ?? 0;

	public CVoieAdjacente(SIGVoie voie, SIGNoeud noeud)
	{
		_voie = voie;
		Noeud = noeud;
		base.Id = noeud.ID;
		_ordre = Global.OrdreVoie;
		if (base.Voie == null)
		{
			_erreur = true;
		}
		else if (noeud.BranchesInTrack(voie).Count == 0)
		{
			_erreur = true;
		}
		else
		{
			nomVoieRectangle = default(Rectangle);
		}
	}

	public override void RecalculPKs()
	{
		if (Support != null)
		{
			_changementSens = Noeud.ChangementDeSens;
			Pk = Noeud.PkOnLine(base.ComposantsViewer.Ligne);
			PkDestination = Noeud.PkOnTrack(_voie);
			_pathData = new DrawPath(this);
		}
	}

	public override bool Contains(Point Pt)
	{
		if (base.TexteVisible && nomVoieRectangle.Contains(Pt))
		{
			return true;
		}
		return _displayPath.Contains(Pt);
	}

	public override bool Contains(Point Pt, int pkPoint)
	{
		if (Math.Abs(pkPoint - Pk) > 2000)
		{
			return false;
		}
		if (base.TexteVisible && nomVoieRectangle.Contains(Pt))
		{
			return true;
		}
		return _displayPath.Contains(Pt);
	}

	public override bool IsInGraph()
	{
		if (base.ComposantsViewer.PkD > Pk + Global.SizeVoieAdjacente)
		{
			return false;
		}
		if (base.ComposantsViewer.PkF < Pk - Global.SizeVoieAdjacente)
		{
			return false;
		}
		return true;
	}

	public override Point GetPoint(int Pk)
	{
		return Support.GetPoint(this.Pk);
	}

	public override Point[] GetPath(int PkD, int PkF)
	{
		Point center = Support.internalGetPoint(Pk);
		return _pathData.GetPoints(center);
	}

	protected override bool OnPaint(PaintEventArgs e)
	{
		nomVoieRectangle = Rectangle.Empty;
		if (!Visible)
		{
			return false;
		}
		Point center = Support.internalGetPoint(Pk);
		Point[] points = _pathData.GetPoints(center);
		if (points.Length < 1)
		{
			return false;
		}
		_displayPath.FromPointArray(points);
		Pen pen = (_mouseOn ? VoieAdjacentePenSelected : VoieAdjacentePen);
		if (base.TexteVisible)
		{
			nomVoieRectangle = _pathData.DrawLabel(e.Graphics, pen, center);
		}
		_displayPath.Draw(e.Graphics, pen);
		if (_changementSens)
		{
			Pen pen2 = new Pen(pen.Color, 6f)
			{
				EndCap = LineCap.ArrowAnchor
			};
			_pathData.DrawArrow(e.Graphics, pen2, center);
		}
		_displayRectangle = _displayPath.MaxBounds();
		return true;
	}

	public override ComposantMenu GetContextMenu()
	{
		ComposantMenu composantMenu = new ComposantMenu();
		composantMenu.AddItem($"Aller à {base.Voie.Ligne.ID} {base.Voie.Nom}", GoToLigne, AuthorizedMode.Always);
		return composantMenu;
	}

	public override PopupForm GetPropertyWindow()
	{
		return new DisplayNomPksPopupForm
		{
			Nom = _voie.Ligne.ID + " " + _voie.Nom,
			PkD = _voie.PKDebut,
			PkF = _voie.PKFin
		};
	}

	public override void OnComposantsLoaded()
	{
		SIGBranche sIGBranche = Noeud.BranchesInOtherTrack(_voie).Find((SIGBranche b) => b.Voie.Ligne == base.ComposantsViewer.Ligne);
		Support = base.Composants.GetVoie(sIGBranche.Voie);
		RecalculPKs();
	}

	protected override void OnMouseDoubleClick(MouseEventArgs e)
	{
		GoToLigne(this, new EventArgs());
	}

	private void GoToLigne(object sender, EventArgs e)
	{
		base.ComposantsViewer.SelectedLigneChanged += ComposantsViewer_SelectedLigneChanged;
		base.ComposantsViewer.SetLigne(base.Voie.Ligne.ID);
	}

	private void ComposantsViewer_SelectedLigneChanged(object sender, EventArgs e)
	{
		base.ComposantsViewer.SelectedLigneChanged -= ComposantsViewer_SelectedLigneChanged;
		base.ComposantsViewer.LightNoeud(Noeud);
	}

	public override string ToString()
	{
		return $"Voie Adjacente {_voie.FullName} ({Pk})";
	}
}
