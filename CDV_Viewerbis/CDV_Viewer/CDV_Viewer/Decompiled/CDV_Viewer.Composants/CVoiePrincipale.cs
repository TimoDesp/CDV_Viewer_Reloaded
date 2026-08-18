using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Windows.Forms;
using CDV_Viewer.CsvBase;
using CDV_Viewer.Data;
using CDV_Viewer.Data.SIGObjects;
using CDV_Viewer.Properties;
using CDV_Viewer.Traitements;

namespace CDV_Viewer.Composants;

public class CVoiePrincipale : CVoieOnLine
{
	private bool _sautpkFin;

	private bool _sautpkDebut;

	public CVoiePrincipale(SIGVoie voie)
	{
		_erreur = true;
		base.Type = TypesVoie.Principale;
		if ((_voie = voie) != null)
		{
			base.Id = voie.ID;
			_posY = ((_voie.PositionY != int.MinValue) ? _voie.PositionY : 0);
			_ordre = Global.OrdreVoie;
			_erreur = false;
		}
	}

	public override void RecalculPKs()
	{
		if (!_composantsLoaded)
		{
			return;
		}
		_pkD = base.Voie.PKDebut;
		_pkF = base.Voie.PKFin;
		int num = ((_sautpkFin = _pkF != base.NoeudFin?.Pk) ? _posY : PosYFin);
		int num2 = ((_sautpkDebut = _pkD != base.NoeudDebut?.Pk) ? _posY : PosYDebut);
		if (num2 == _posY && num == _posY)
		{
			_unscaledfullPath = new UnscaledLocation[2]
			{
				new UnscaledLocation(_pkD, _posY),
				new UnscaledLocation(_pkF, _posY)
			};
			return;
		}
		int num3 = Math.Min(_pkD + Global.LongueurVoieSecondaire, _pkF - 10);
		int num4 = Math.Max(_pkF - Global.LongueurVoieSecondaire, _pkD + 10);
		int count = base.Voie.Branches.Count;
		if (count > 2)
		{
			int pK = base.Voie.Branches[1].PK;
			int pK2 = base.Voie.Branches[count - 2].PK;
			num3 = Math.Min(pK, num3);
			num4 = Math.Max(pK2, num4);
		}
		if (num == _posY)
		{
			_unscaledfullPath = new UnscaledLocation[3]
			{
				new UnscaledLocation(_pkD, num2),
				new UnscaledLocation(num3, _posY),
				new UnscaledLocation(_pkF, _posY)
			};
			return;
		}
		if (PosYDebut == _posY)
		{
			_unscaledfullPath = new UnscaledLocation[3]
			{
				new UnscaledLocation(_pkD, _posY),
				new UnscaledLocation(num4, _posY),
				new UnscaledLocation(_pkF, num)
			};
			return;
		}
		if (num4 < num3)
		{
			num3 = (_pkF + 2 * _pkD) / 3;
			num4 = (2 * _pkF + _pkD) / 3;
		}
		_unscaledfullPath = new UnscaledLocation[4]
		{
			new UnscaledLocation(_pkD, num2),
			new UnscaledLocation(num3, _posY),
			new UnscaledLocation(num4, _posY),
			new UnscaledLocation(_pkF, num)
		};
	}

	protected override bool OnPaint(PaintEventArgs e)
	{
		if (!Visible)
		{
			return false;
		}
		Graphics graphics = e.Graphics;
		Point[] path = GetPath(_pkD, _pkF);
		Pen pen = (_mouseOn ? CVoie.VoiePenSelected : CVoie.VoiePen);
		if (base.TexteVisible)
		{
			PaintTextVoiePrincipale(graphics, path, pen);
		}
		_displayPath.FromPointArray(path);
		_displayRectangle = _displayPath.MaxBounds();
		_displayPath.Draw(graphics, pen);
		if (_sautpkDebut)
		{
			Point point = base.NoeudDebut.GetPoint(base.NoeudDebut.Pk);
			Point pt = _unscaledfullPath.First().Scale();
			Pen pen2 = new Pen(pen.Brush)
			{
				DashStyle = DashStyle.Dash,
				EndCap = LineCap.Round
			};
			graphics.FillEllipse(new SolidBrush(pen.Color), pt.X - 5, pt.Y - 5, 10, 10);
			graphics.DrawLine(pen2, point, pt);
		}
		if (_sautpkFin)
		{
			Point pt2 = _unscaledfullPath.Last().Scale();
			Point point2 = base.NoeudFin.GetPoint(base.NoeudFin.Pk);
			Pen pen3 = new Pen(pen.Color)
			{
				DashStyle = DashStyle.Dash
			};
			graphics.FillEllipse(new SolidBrush(pen.Color), pt2.X - 5, pt2.Y - 5, 10, 10);
			graphics.DrawLine(pen3, pt2, point2);
		}
		return true;
	}

	private void PaintTextVoiePrincipale(Graphics g, Point[] _Path, Pen _pen)
	{
		int num = 0;
		int num2 = _Path.Length - 1;
		switch (num2)
		{
		case 3:
			num = 1;
			num2 = 2;
			break;
		case 2:
			if (_voieDebut == null)
			{
				num2 = 1;
			}
			else
			{
				num = 1;
			}
			break;
		}
		Point point = _Path[num];
		Point point2 = _Path[num2];
		int num3 = (point.Y + point2.Y) / 2;
		g.DrawString(layoutRectangle: new Rectangle(Math.Min(point.X, point2.X), num3 - 17, Math.Max(Math.Abs(point.X - point2.X), 32), 15), s: base.Voie.Nom, font: CVoie._fontNomVoie, brush: _pen.Brush, format: CVoie._formatTexteNomVoie);
	}

	public override ComposantMenu GetContextMenu()
	{
		ComposantMenu composantMenu = new ComposantMenu("Voie " + base.Voie.Nom);
		ToolStripMenuItem toolStripMenuItem = composantMenu.AddMenu("Insérer", Resources.plus, AuthorizedMode.Edit);
		composantMenu.AddDropDownItem(toolStripMenuItem, "Jonction...", Resources.Jonction, InsertJonction, AuthorizedMode.Edit);
		composantMenu.AddDropDownItem(toolStripMenuItem, "TJD...", Resources.TJD, base.InsertTJD, AuthorizedMode.Edit);
		composantMenu.AddDropDownItem(toolStripMenuItem, "Saut de PK...", Resources.SautPk, InsertSautDePk, AuthorizedMode.Edit);
		composantMenu.AddDropDownItem(toolStripMenuItem, "Balise...", Resources.Balise, InsertBalise, AuthorizedMode.Edit);
		AddCdvItems(composantMenu, toolStripMenuItem);
		composantMenu.AddMenu(() => GetPositionMenu("Position Voie"), AuthorizedMode.Edit);
		AddCommonMenu(composantMenu);
		return composantMenu;
	}

	public ToolStripMenuItem GetPositionMenu(string submenuText)
	{
		Image posVoies = Resources.PosVoies;
		return new ToolStripMenuItem(submenuText, posVoies)
		{
			DropDownItems = 
			{
				{
					"En haut",
					(Image)Resources.TopArrow,
					(EventHandler)SetPositionVoieOnTop
				},
				{
					"Monter",
					(Image)Resources.UpArrow,
					(EventHandler)SetPositionVoieUp
				},
				{
					"Descendre",
					(Image)Resources.DownpArrow,
					(EventHandler)SetPositionVoieDown
				},
				{
					"En bas",
					(Image)Resources.BottomArrow,
					(EventHandler)SetPositionVoieToBottom
				}
			}
		};
	}

	private void InsertSautDePk()
	{
		base.Composants.AddOperation(new CInsertSautPk(this, base.PkSelected));
	}

	private void InsertJonction()
	{
		base.Composants.AddOperation(new CInsertJonction(this, base.PkSelected));
	}

	private void SetPositionVoie(object sender, EventArgs e)
	{
		if ((string)(sender as ToolStripMenuItem).Tag == "UP")
		{
			base.Voie.PositionY = _posY - 1;
			Base.SetPositionVoie(base.Voie);
			base.ComposantsViewer.RefreshLigne();
		}
		else
		{
			base.Voie.PositionY = _posY + 1;
			Base.SetPositionVoie(base.Voie);
			base.ComposantsViewer.RefreshLigne();
		}
	}

	private void SetPositionVoieUp(object sender, EventArgs e)
	{
		_ = _posY;
		int num = 1;
		if ((Control.ModifierKeys & Keys.Control) == Keys.Control)
		{
			num += 2;
		}
		if ((Control.ModifierKeys & Keys.Shift) == Keys.Shift)
		{
			num++;
		}
		base.Voie.PositionY = _posY - num;
		Base.SetPositionVoie(base.Voie);
		base.ComposantsViewer.RefreshLigne();
	}

	private void SetPositionVoieDown(object sender, EventArgs e)
	{
		_ = _posY;
		int num = 1;
		if ((Control.ModifierKeys & Keys.Control) == Keys.Control)
		{
			num += 2;
		}
		if ((Control.ModifierKeys & Keys.Shift) == Keys.Shift)
		{
			num++;
		}
		base.Voie.PositionY = _posY + num;
		Base.SetPositionVoie(base.Voie);
		base.ComposantsViewer.RefreshLigne();
	}

	private void SetPositionVoieOnTop(object sender, EventArgs e)
	{
		List<SIGVoie> list = base.Voie.Ligne.Voies.FindAll((SIGVoie v) => v != base.Voie && v.PKDebut < _pkF && v.PKFin > _pkD);
		int posYmin = 0;
		list.ForEach(delegate(SIGVoie v)
		{
			posYmin = Math.Min(v.PositionY, posYmin);
		});
		base.Voie.PositionY = posYmin - 1;
		Base.SetPositionVoie(base.Voie);
		base.ComposantsViewer.RefreshLigne();
	}

	private void SetPositionVoieToBottom(object sender, EventArgs e)
	{
		List<SIGVoie> list = base.Voie.Ligne.Voies.FindAll((SIGVoie v) => v != base.Voie && v.PKDebut < _pkF && v.PKFin > _pkD);
		int posYmax = 0;
		list.ForEach(delegate(SIGVoie v)
		{
			posYmax = Math.Max(v.PositionY, posYmax);
		});
		base.Voie.PositionY = posYmax + 1;
		Base.SetPositionVoie(base.Voie);
		base.ComposantsViewer.RefreshLigne();
	}

	private void InsertBalise()
	{
		base.Composants.AddOperation(new CInsertBalise(this, base.PkSelected));
	}

	private void ImportCDV()
	{
		Dialogs.Message("Attention ! Le format du fichier a importer est le suivant : NOM;PK_DEBUT;PK_FIN;TYPE;FREQUENCE;JOINT_DEBUT;JOINT_FIN. Seul les champs NOM, PK_DEBUT, et PK_FIN sont obligatoire");
		OpenFileDialog openFileDialog = new OpenFileDialog();
		openFileDialog.FileName = "CDV.csv";
		openFileDialog.Filter = "Tous les fichiers (*.*)|*.*";
		if (openFileDialog.ShowDialog() == DialogResult.OK)
		{
			QuickImport.Import(openFileDialog.FileName, base.Voie.ID);
		}
	}
}
