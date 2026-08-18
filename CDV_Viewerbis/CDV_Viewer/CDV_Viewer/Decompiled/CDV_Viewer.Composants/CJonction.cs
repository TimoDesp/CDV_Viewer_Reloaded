using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using CDV_Viewer.CsvBase;
using CDV_Viewer.Data;
using CDV_Viewer.Data.SIGObjects;
using CDV_Viewer.Properties;
using CDV_Viewer.Traitements;

namespace CDV_Viewer.Composants;

public class CJonction : CVoieOnLine
{
	public CJonction(SIGVoie voie)
	{
		base.Type = TypesVoie.Jonction;
		_erreur = true;
		if ((_voie = voie) != null)
		{
			base.Id = voie.ID;
			_posY = ((_voie.PositionY != int.MinValue) ? _voie.PositionY : 0);
			_ordre = Global.OrdreJonction;
			ZoomTexteVisible = Global.ZoomTexteVisibleJonctions;
			_erreur = false;
		}
	}

	public override void RecalculPKs()
	{
		_pkD = base.Voie.PKDebut;
		_pkF = base.Voie.PKFin;
		UnscaledLocation unscaledLocation = new UnscaledLocation(_pkD, PosYDebut);
		UnscaledLocation unscaledLocation2 = new UnscaledLocation(_pkF, PosYFin);
		if (base.Voie.Noeuds.Count < 3)
		{
			_unscaledfullPath = new UnscaledLocation[2] { unscaledLocation, unscaledLocation2 };
			return;
		}
		List<CNoeud> list = new List<CNoeud>();
		List<CNoeud> list2 = new List<CNoeud>();
		SortedList<int, UnscaledLocation> sortedList = new SortedList<int, UnscaledLocation>();
		sortedList.Add(unscaledLocation.Pk, unscaledLocation);
		foreach (SIGNoeud noeud2 in base.Voie.Noeuds)
		{
			CNoeud noeud = base.Composants.GetNoeud(noeud2);
			if (noeud != null && noeud.Pk > _pkD && noeud.Pk < _pkF)
			{
				CVoie support = noeud.Support;
				if (support == this)
				{
					list2.Add(noeud);
				}
				else if (support is CJonction)
				{
					list.Add(noeud);
				}
				else
				{
					sortedList.Add(noeud.Pk, new UnscaledLocation(noeud.Pk, (support as CVoiePrincipale).PosY));
				}
			}
		}
		sortedList.Add(unscaledLocation2.Pk, unscaledLocation2);
		foreach (CNoeud item in list)
		{
			CJonction cJonction = item.Support as CJonction;
			if (cJonction._unscaledfullPath.Length < 3)
			{
				cJonction.RecalculPKs();
			}
			sortedList.Add(item.Pk, cJonction.GetUnscaledPoint(item.Pk));
		}
		_unscaledfullPath = sortedList.Values.ToArray();
		foreach (CNoeud item2 in list2)
		{
			sortedList.Add(item2.Pk, GetUnscaledPoint(item2.Pk));
		}
		_unscaledfullPath = sortedList.Values.ToArray();
		int num = _unscaledfullPath.Length - 1;
		if (num <= 1)
		{
			return;
		}
		if (_unscaledfullPath[0].PosY == 0.0)
		{
			int num2 = Math.Sign(_unscaledfullPath[2].PosY - _unscaledfullPath[1].PosY);
			if (num2 == 0)
			{
				num2 = 1;
			}
			_unscaledfullPath[0].PosY = _unscaledfullPath[1].PosY - (double)num2;
		}
		if (_unscaledfullPath[num].PosY == 0.0)
		{
			int num3 = Math.Sign(_unscaledfullPath[num - 1].PosY - _unscaledfullPath[num - 2].PosY);
			if (num3 == 0)
			{
				num3 = 1;
			}
			_unscaledfullPath[num].PosY = _unscaledfullPath[num - 1].PosY + (double)num3;
		}
	}

	protected override bool OnPaint(PaintEventArgs e)
	{
		if (!Visible)
		{
			return false;
		}
		Graphics graphics = e.Graphics;
		Point[] path = GetPath(_pkD, _pkF);
		Point point = path[0];
		Point point2 = path[path.Length - 1];
		Pen pen = (_mouseOn ? CVoie.VoiePenSelected : CVoie.VoiePen);
		if (base.TexteVisible)
		{
			int num = -2;
			int num2 = (int)CVoie._fontNomVoie.SizeInPoints;
			if (point.Y > point2.Y)
			{
				num = num2 * base.Voie.Nom.Length + 2;
			}
			num2 = -2 * num2;
			graphics.DrawString(base.Voie.Nom, CVoie._fontNomVoie, pen.Brush, (point2.X + point.X) / 2 - num, (point2.Y + point.Y) / 2 + num2);
		}
		_displayPath.FromPointArray(path);
		_displayPath.Draw(graphics, pen);
		return true;
	}

	public override ComposantMenu GetContextMenu()
	{
		ComposantMenu composantMenu = new ComposantMenu("Jonction " + base.Voie.Nom);
		ToolStripMenuItem toolStripMenuItem = composantMenu.AddMenu("Insérer", AuthorizedMode.Edit);
		composantMenu.AddDropDownItem(toolStripMenuItem, "TJD...", Resources.TJD, base.InsertTJD, AuthorizedMode.Edit);
		AddCdvItems(composantMenu, toolStripMenuItem);
		AddCommonMenu(composantMenu);
		return composantMenu;
	}

	private void InsertSautDePk(object sender, EventArgs e)
	{
		base.Composants.AddOperation(new CInsertSautPk(this, base.PkSelected));
	}

	private void InsertJonction(object sender, EventArgs e)
	{
		base.Composants.AddOperation(new CInsertJonction(this, base.PkSelected));
	}

	private void SetPositionVoie(object sender, EventArgs e)
	{
		if ((string)(sender as ToolStripMenuItem).Tag == "UP")
		{
			base.Voie.PositionY = _posY - 1;
			Base.SetPositionVoie(base.Voie);
		}
		else
		{
			base.Voie.PositionY = _posY + 1;
			Base.SetPositionVoie(base.Voie);
		}
		base.ComposantsViewer.RefreshLigne();
	}

	private void SetPositionVoieUp(object sender, EventArgs e)
	{
		int posYmin = _posY;
		base.Voie.PositionY = _posY - 1;
		Base.SetPositionVoie(base.Voie);
		base.Voie.Ligne.Voies.FindAll((SIGVoie v) => v != base.Voie && v.IsVoiePrincipale() && v.PKDebut > _pkD && v.PKFin < _pkF && v.PositionY > posYmin);
		base.ComposantsViewer.RefreshLigne();
	}

	private void SetPositionVoieDown(object sender, EventArgs e)
	{
		int posYmax = _posY;
		base.Voie.PositionY = _posY + 1;
		Base.SetPositionVoie(base.Voie);
		base.ComposantsViewer.RefreshLigne();
		base.Voie.Ligne.Voies.FindAll((SIGVoie v) => v != base.Voie && v.IsVoiePrincipale() && v.PKDebut > _pkD && v.PKFin < _pkF && v.PositionY < posYmax);
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

	private void InsertBalise(object sender, EventArgs e)
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
