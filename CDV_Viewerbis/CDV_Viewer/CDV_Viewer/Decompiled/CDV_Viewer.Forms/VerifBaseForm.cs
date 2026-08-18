using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Threading;
using System.Windows.Forms;
using CDV_Viewer.CsvBase;
using CDV_Viewer.Data;
using CDV_Viewer.Data.SIGObjects;
using CDV_Viewer.DockControls;
using CDV_Viewer.Properties;
using CDV_Viewer.Traitements;

namespace CDV_Viewer.Forms;

public class VerifBaseForm : Form
{
	private class ListViewItemComparer : IComparer
	{
		private int col;

		public ListViewItemComparer()
		{
			col = 0;
		}

		public ListViewItemComparer(int column)
		{
			col = column;
		}

		public int Compare(object x, object y)
		{
			return string.Compare(((ListViewItem)x).SubItems[col].Text, ((ListViewItem)y).SubItems[col].Text);
		}
	}

	private Thread _thread;

	private InterfaceProgressBar _interface = new InterfaceProgressBar();

	private IContainer components;

	private Label lTexte;

	private ProgressBar progressBar;

	private ListView lvResults;

	private Panel panel1;

	private Button bFermer;

	private Button bArreter;

	private Button bLancer;

	private ColumnHeader chImg;

	private ColumnHeader chErreur;

	private System.Windows.Forms.Timer timer;

	private ColumnHeader chPosition;

	private ColumnHeader chObjet;

	public VerifBaseForm()
	{
		InitializeComponent();
		base.Icon = Icon.FromHandle(Resources.Check_base.GetHicon());
		base.Load += VerifForm_Load;
		timer.Tick += timer_Tick;
		bFermer.Click += bFermer_Click;
		bArreter.Click += bArreter_Click;
		bLancer.Click += bLancer_Click;
		lvResults.ColumnClick += lvResults_ColumnClick;
		lvResults.MouseClick += lvResults_MouseClick;
		timer.Start();
	}

	private void ShowResults()
	{
		List<ErreurVerif> obj = (List<ErreurVerif>)_interface.Result;
		obj.Sort();
		foreach (ErreurVerif item in obj)
		{
			string text = string.Empty;
			string text2 = string.Empty;
			if (item.Objet is SIGVoie)
			{
				text = "Voie " + ((SIGVoie)item.Objet).ID;
				text2 = ((SIGVoie)item.Objet).Ligne.ID + " " + ((SIGVoie)item.Objet).Nom;
			}
			else if (item.Objet is SIGNoeud)
			{
				text = "Noeud " + ((SIGNoeud)item.Objet).ID;
				text2 = ((SIGNoeud)item.Objet).BrancheAmont.Voie.Ligne.ID + " " + ((SIGNoeud)item.Objet).BrancheAmont.Voie.Nom + " " + Chaines.PkToString(((SIGNoeud)item.Objet).BrancheAmont.PK);
			}
			else if (item.Objet is SIGJoint)
			{
				text = "Joint " + ((SIGJoint)item.Objet).ID;
				text2 = ((SIGJoint)item.Objet).Voie.Ligne.ID + " " + ((SIGJoint)item.Objet).Voie.Nom + " " + Chaines.PkToString(((SIGJoint)item.Objet).PK);
			}
			else if (item.Objet is SIGCircuit)
			{
				text = "Circuit " + ((SIGCircuit)item.Objet).ID + " (" + ((SIGCircuit)item.Objet).Nom + ")";
				if (((SIGCircuit)item.Objet).DemiJoints.Count > 0 && ((SIGCircuit)item.Objet).DemiJoints[0].Joint != null)
				{
					text2 = ((SIGCircuit)item.Objet).DemiJoints[0].Joint.Voie.Ligne.ID + " " + ((SIGCircuit)item.Objet).DemiJoints[0].Joint.Voie.Nom + " " + Chaines.PkToString(((SIGCircuit)item.Objet).DemiJoints[0].Joint.PK);
				}
			}
			else if (item.Objet is SIGModele)
			{
				SIGModele sIGModele = (SIGModele)item.Objet;
				if (sIGModele.Circuit == null)
				{
					text = "Modèle " + sIGModele.ID;
				}
				else
				{
					text = "Modèle du circuit " + sIGModele.Circuit.ID + " (" + sIGModele.Circuit.Nom + ")";
					if (sIGModele.Circuit.DemiJoints.Count > 0 && sIGModele.Circuit.DemiJoints[0].Joint != null)
					{
						text2 = sIGModele.Circuit.DemiJoints[0].Joint.Voie.Ligne.ID + " " + sIGModele.Circuit.DemiJoints[0].Joint.Voie.Nom + " " + Chaines.PkToString(sIGModele.Circuit.DemiJoints[0].Joint.PK);
					}
				}
			}
			string[] obj2 = new string[4] { "", text, text2, null };
			TypesErreurVerif type = item.Type;
			obj2[3] = type.ToString();
			ListViewItem listViewItem = new ListViewItem(obj2);
			listViewItem.Tag = item;
			lvResults.Items.Add(listViewItem);
		}
		if (lvResults.Items.Count == 0)
		{
			lvResults.Items.Add(new ListViewItem(new string[2] { "", "Aucune erreur" }));
		}
	}

	private ErreurVerif GetSelectedErreur()
	{
		return (ErreurVerif)lvResults.SelectedItems[0].Tag;
	}

	private void VerifForm_Load(object sender, EventArgs e)
	{
		progressBar.Enabled = false;
		bArreter.Enabled = false;
		lvResults.Enabled = false;
	}

	private void bFermer_Click(object sender, EventArgs e)
	{
		if (_thread != null && _thread.ThreadState == ThreadState.Running)
		{
			_thread.Abort();
			_interface.Maximum = -1;
			Global.MainForm.Enabled = true;
		}
		Close();
	}

	private void bArreter_Click(object sender, EventArgs e)
	{
		_thread.Abort();
		_interface.Maximum = -1;
		progressBar.Value = 0;
		lTexte.Text = "Vérification arrétée !";
		Global.MainForm.Enabled = true;
		progressBar.Enabled = false;
		bLancer.Enabled = true;
		bArreter.Enabled = false;
	}

	private void bLancer_Click(object sender, EventArgs e)
	{
		progressBar.Enabled = true;
		bLancer.Enabled = false;
		bArreter.Enabled = true;
		Global.MainForm.Enabled = false;
		_thread = new Thread(Base.VerifierBase);
		_thread.Start(_interface);
	}

	private void timer_Tick(object sender, EventArgs e)
	{
		if (_interface.Maximum > 0)
		{
			if (_interface.Result == null)
			{
				progressBar.Maximum = _interface.Maximum;
				progressBar.Value = _interface.Avancement;
				lTexte.Text = _interface.Texte;
				return;
			}
			_interface.Maximum = -1;
			progressBar.Value = 0;
			lTexte.Text = "Vérification terminée !";
			lvResults.Items.Clear();
			ShowResults();
			Global.MainForm.Enabled = true;
			progressBar.Enabled = false;
			bLancer.Enabled = true;
			bArreter.Enabled = false;
			lvResults.Enabled = true;
		}
	}

	private void lvResults_ColumnClick(object sender, ColumnClickEventArgs e)
	{
		lvResults.ListViewItemSorter = new ListViewItemComparer(e.Column);
		lvResults.ListViewItemSorter = null;
	}

	private void lvResults_MouseClick(object sender, MouseEventArgs e)
	{
		if (e.Button != MouseButtons.Right)
		{
			return;
		}
		ContextMenu contextMenu = new ContextMenu();
		if (((List<ErreurVerif>)_interface.Result).Count > 0 && lvResults.SelectedItems.Count > 0)
		{
			int index = lvResults.SelectedItems[0].Index;
			if (lvResults.SelectedItems[0].SubItems[1].Text != string.Empty)
			{
				contextMenu.MenuItems.Add("Aller à " + lvResults.SelectedItems[0].SubItems[1].Text, AllerA);
			}
			contextMenu.MenuItems.Add("Traitée", Traitee);
			if (lvResults.SelectedItems[0].ForeColor == Color.LightGray)
			{
				contextMenu.MenuItems[contextMenu.MenuItems.Count - 1].Checked = true;
			}
			int count = contextMenu.MenuItems.Count;
			switch (((List<ErreurVerif>)_interface.Result)[index].Type)
			{
			case TypesErreurVerif.VoieSansNoeuds:
				contextMenu.MenuItems.Add("Définir les limites de la voie...", SetDebutFinVoie);
				break;
			case TypesErreurVerif.VoieSansDebut:
				contextMenu.MenuItems.Add("Définir le début de la voie...", EditDebutVoie);
				break;
			case TypesErreurVerif.VoieSansFin:
				contextMenu.MenuItems.Add("Définir la fin de la voie...", EditFinVoie);
				break;
			case TypesErreurVerif.JointSurAucuneVoie:
				contextMenu.MenuItems.Add("Supprimer ce joint...", SupprimerJoint);
				break;
			case TypesErreurVerif.JointAvantDebutVoie:
				contextMenu.MenuItems.Add("Supprimer ce joint...", SupprimerJoint);
				contextMenu.MenuItems.Add("Ajuster les limites de la voie...", AllongerVoie);
				break;
			case TypesErreurVerif.JointApresFinVoie:
				contextMenu.MenuItems.Add("Supprimer ce joint...", SupprimerJoint);
				contextMenu.MenuItems.Add("Ajuster les limites de la voie...", AllongerVoie);
				break;
			case TypesErreurVerif.JointRelieAucunCircuit:
				contextMenu.MenuItems.Add("Supprimer ce joint...", SupprimerJoint);
				break;
			case TypesErreurVerif.CircuitMoins2Joints:
				contextMenu.MenuItems.Add("Supprimer ce circuit...", SupprimerCircuit);
				break;
			case TypesErreurVerif.ModeleIdJointIncorrect:
				contextMenu.MenuItems.Add("Supprimer ce modèle...", SupprimerModele);
				break;
			case TypesErreurVerif.ModeleDemiPasDifferentDeDemiPasDemiJoint:
				contextMenu.MenuItems.Add("Utiliser la valeur du modèle", UseModeleDemiPasValue);
				break;
			case TypesErreurVerif.ModeleNbCondosDifferentNbCondosCircuit:
				contextMenu.MenuItems.Add("Utiliser la valeur du modèle", UseModeleNbCondosValue);
				break;
			case TypesErreurVerif.ModeleSansCircuit:
				contextMenu.MenuItems.Add("Supprimer ce modèle...", SupprimerModele);
				break;
			}
			if (!Global.ModeEdition)
			{
				for (int i = count; i < contextMenu.MenuItems.Count; i++)
				{
					contextMenu.MenuItems[i].Enabled = false;
				}
			}
			contextMenu.MenuItems.Add("-");
		}
		contextMenu.MenuItems.Add("Exporter...", Exporter);
		contextMenu.Show(this, new Point(e.X + lvResults.Left + panel1.Left, e.Y + lvResults.Top + panel1.Top));
	}

	private void AllerA(object sender, EventArgs e)
	{
		if (!(lvResults.SelectedItems[0].SubItems[2].Text != string.Empty))
		{
			return;
		}
		object objet = GetSelectedErreur().Objet;
		int num = int.MinValue;
		int num2 = int.MinValue;
		if (objet is SIGVoie)
		{
			num = ((SIGVoie)objet).Ligne.ID;
			num2 = (((SIGVoie)objet).PKDebut + ((SIGVoie)objet).PKFin) / 2;
		}
		else if (objet is SIGNoeud)
		{
			num = ((SIGNoeud)objet).BrancheAmont.Voie.Ligne.ID;
			num2 = ((SIGNoeud)objet).BrancheAmont.PK;
		}
		else if (objet is SIGJoint)
		{
			num = ((SIGJoint)objet).Voie.Ligne.ID;
			num2 = ((SIGJoint)objet).PK;
			ComposantsViewer.Viewer.LightVoie(((SIGJoint)objet).Voie, num2);
		}
		else if (objet is SIGCircuit)
		{
			foreach (SIGDemiJoint demiJoint in ((SIGCircuit)objet).DemiJoints)
			{
				if (demiJoint != null && demiJoint.Joint != null)
				{
					num = demiJoint.Joint.Voie.Ligne.ID;
					num2 = demiJoint.Joint.PK;
					break;
				}
			}
		}
		else if (objet is SIGModele)
		{
			foreach (SIGDemiJoint demiJoint2 in ((SIGModele)objet).Circuit.DemiJoints)
			{
				if (demiJoint2 != null && demiJoint2.Joint != null)
				{
					num = demiJoint2.Joint.Voie.Ligne.ID;
					num2 = demiJoint2.Joint.PK;
					break;
				}
			}
		}
		if (num != int.MinValue)
		{
			if (num2 == int.MinValue)
			{
				ComposantsViewer.Viewer.SetLigne(num);
			}
			else
			{
				ComposantsViewer.Viewer.SetLignePK(num, num2);
			}
			if (objet is SIGCircuit)
			{
				ComposantsViewer.Viewer.LightCircuit((SIGCircuit)objet);
			}
			if (objet is SIGModele)
			{
				ComposantsViewer.Viewer.LightCircuit(((SIGModele)objet).Circuit);
			}
			if (objet is SIGJoint)
			{
				ComposantsViewer.Viewer.LightVoie(((SIGJoint)objet).Voie, num2);
			}
		}
	}

	private void Traitee(object sender, EventArgs e)
	{
		if (lvResults.SelectedItems[0].ForeColor == Color.Black)
		{
			lvResults.SelectedItems[0].ForeColor = Color.LightGray;
		}
		else
		{
			lvResults.SelectedItems[0].ForeColor = Color.Black;
		}
	}

	public void AllongerVoie(object sender, EventArgs e)
	{
		SIGJoint sIGJoint = (SIGJoint)GetSelectedErreur().Objet;
		if (sIGJoint.Voie.PKDebut > sIGJoint.PK)
		{
			SIGBranche brancheAval = sIGJoint.Voie.NoeudDebut.BrancheAval;
			brancheAval.PK = sIGJoint.PK - 5;
			Base.UpdateBranche(brancheAval);
			MessageBox.Show("Le PK début de la voie a été correctement ajusté !", Resources.APP_NAME);
		}
		if (sIGJoint.Voie.PKFin < sIGJoint.PK)
		{
			SIGBranche brancheAmont = sIGJoint.Voie.NoeudFin.BrancheAmont;
			brancheAmont.PK = sIGJoint.PK + 5;
			Base.UpdateBranche(brancheAmont);
			MessageBox.Show("Le PK fin de la voie a été correctement ajusté !", Resources.APP_NAME);
		}
	}

	public void SetDebutFinVoie(object sender, EventArgs e)
	{
		SIGVoie sIGVoie = (SIGVoie)GetSelectedErreur().Objet;
		EditDebutFinVoieDialog editDebutFinVoieDialog = new EditDebutFinVoieDialog();
		if (editDebutFinVoieDialog.ShowDialog() == DialogResult.OK)
		{
			SIGBranche brancheDebut = sIGVoie.GetBrancheDebut();
			SIGBranche brancheFin = sIGVoie.GetBrancheFin();
			brancheDebut.PK = editDebutFinVoieDialog.PkDebut;
			brancheFin.PK = editDebutFinVoieDialog.PkFin;
			Base.UpdateBranche(brancheDebut);
			Base.UpdateBranche(brancheFin);
		}
	}

	public void EditDebutVoie(object sender, EventArgs e)
	{
		SIGVoie sIGVoie = (SIGVoie)GetSelectedErreur().Objet;
		int num = int.MaxValue;
		bool flag = false;
		foreach (SIGNoeud noeud in sIGVoie.Noeuds)
		{
			foreach (SIGBranche item in noeud.BranchesInTrack(sIGVoie))
			{
				if (item.PK < num)
				{
					num = item.PK;
				}
				flag = true;
			}
		}
		foreach (SIGJoint joint in sIGVoie.Joints)
		{
			if (joint.PK < num)
			{
				num = joint.PK;
				flag = false;
			}
		}
		foreach (SIGBalise balise in sIGVoie.Balises)
		{
			if (balise.PK < num)
			{
				num = balise.PK;
				flag = false;
			}
		}
		if (num != int.MaxValue)
		{
			if (!flag)
			{
				num -= 100;
				if (MessageBox.Show("Le PK de début de voie va être définit 100m avant premier élement (joint, balise, etc...) - 100m => PK " + Chaines.PkToString(num) + ". Voulez-vous continuer ?", Resources.APP_NAME, MessageBoxButtons.YesNo) == DialogResult.Yes)
				{
					Base.CreateNoeud(sIGVoie, num, BrancheType.Aval);
				}
			}
		}
		else
		{
			MessageBox.Show("Erreur", Resources.APP_NAME);
		}
	}

	public void EditFinVoie(object sender, EventArgs e)
	{
		SIGVoie sIGVoie = (SIGVoie)GetSelectedErreur().Objet;
		int num = int.MinValue;
		bool flag = false;
		foreach (SIGNoeud noeud in sIGVoie.Noeuds)
		{
			foreach (SIGBranche item in noeud.BranchesInTrack(sIGVoie))
			{
				if (item.PK > num)
				{
					num = item.PK;
					flag = true;
				}
			}
		}
		foreach (SIGJoint joint in sIGVoie.Joints)
		{
			if (joint.PK > num)
			{
				num = joint.PK;
				flag = false;
			}
		}
		foreach (SIGBalise balise in sIGVoie.Balises)
		{
			if (balise.PK > num)
			{
				num = balise.PK;
				flag = false;
			}
		}
		if (num != int.MinValue)
		{
			if (flag)
			{
				num += 100;
				if (MessageBox.Show("Le PK de fin de voie va être définit au PK du dernier élement (joint, noeud, balise, etc...) + 100m => PK " + Chaines.PkToString(num) + ". Voulez-vous continuer ?", Resources.APP_NAME, MessageBoxButtons.YesNo) == DialogResult.Yes)
				{
					Base.CreateNoeud(sIGVoie, num, BrancheType.Amont);
				}
			}
		}
		else
		{
			MessageBox.Show("Erreur", Resources.APP_NAME);
		}
	}

	public void SupprimerJoint(object sender, EventArgs e)
	{
		if (MessageBox.Show("Êtes-vous sûr de vouloir supprimer ce joint ?", Resources.APP_NAME, MessageBoxButtons.YesNo) == DialogResult.Yes)
		{
			Base.DeleteJoint((SIGJoint)GetSelectedErreur().Objet);
		}
	}

	public void SupprimerCircuit(object sender, EventArgs e)
	{
		if (MessageBox.Show("Êtes-vous sûr de vouloir supprimer ce circuit ?", Resources.APP_NAME, MessageBoxButtons.YesNo) == DialogResult.Yes)
		{
			Base.DeleteCircuit(((SIGCircuit)GetSelectedErreur().Objet).ID);
		}
	}

	public void SupprimerModele(object sender, EventArgs e)
	{
		if (MessageBox.Show("Êtes-vous sûr de vouloir supprimer ce modèle ?", Resources.APP_NAME, MessageBoxButtons.YesNo) == DialogResult.Yes)
		{
			Base.DeleteModele(((SIGModele)GetSelectedErreur().Objet).ID);
		}
	}

	public void UseModeleDemiPasValue(object sender, EventArgs e)
	{
		SIGModele sIGModele = (SIGModele)GetSelectedErreur().Objet;
		sIGModele.DemiJointE.DemiPas = sIGModele.Points[0].X;
		Base.UpdateDemiJoint(sIGModele.DemiJointE);
	}

	public void UseModeleNbCondosValue(object sender, EventArgs e)
	{
		SIGModele sIGModele = (SIGModele)GetSelectedErreur().Objet;
		sIGModele.Circuit.NbPtsCompensation = sIGModele.Condos.Count;
		if (sIGModele.Condos.Count > 1)
		{
			sIGModele.Circuit.PasReel = sIGModele.Condos[1] - sIGModele.Condos[0];
		}
		else
		{
			sIGModele.Circuit.PasReel = 0.0;
		}
		Base.UpdateCircuit(sIGModele.Circuit);
	}

	public void Exporter(object sender, EventArgs e)
	{
		SaveFileDialog saveFileDialog = new SaveFileDialog();
		saveFileDialog.Filter = "Fichier CSV (*.csv)|*.csv";
		if (saveFileDialog.ShowDialog() != DialogResult.OK)
		{
			return;
		}
		List<string> list = new List<string>();
		list.Add("OBJET;POSITION;ERREUR");
		foreach (ListViewItem item in lvResults.Items)
		{
			list.Add(item.SubItems[1].Text + ";" + item.SubItems[2].Text + ";" + item.SubItems[3].Text);
		}
		File.WriteAllLines(saveFileDialog.FileName, list.ToArray());
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
		this.lTexte = new System.Windows.Forms.Label();
		this.progressBar = new System.Windows.Forms.ProgressBar();
		this.lvResults = new System.Windows.Forms.ListView();
		this.chImg = new System.Windows.Forms.ColumnHeader();
		this.chObjet = new System.Windows.Forms.ColumnHeader();
		this.chPosition = new System.Windows.Forms.ColumnHeader();
		this.chErreur = new System.Windows.Forms.ColumnHeader();
		this.panel1 = new System.Windows.Forms.Panel();
		this.bArreter = new System.Windows.Forms.Button();
		this.bLancer = new System.Windows.Forms.Button();
		this.bFermer = new System.Windows.Forms.Button();
		this.timer = new System.Windows.Forms.Timer(this.components);
		this.panel1.SuspendLayout();
		base.SuspendLayout();
		this.lTexte.AutoSize = true;
		this.lTexte.Location = new System.Drawing.Point(13, 16);
		this.lTexte.Name = "lTexte";
		this.lTexte.Size = new System.Drawing.Size(16, 13);
		this.lTexte.TabIndex = 0;
		this.lTexte.Text = "   ";
		this.progressBar.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.progressBar.Location = new System.Drawing.Point(16, 40);
		this.progressBar.Name = "progressBar";
		this.progressBar.Size = new System.Drawing.Size(575, 13);
		this.progressBar.TabIndex = 1;
		this.lvResults.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.lvResults.Columns.AddRange(new System.Windows.Forms.ColumnHeader[4] { this.chImg, this.chObjet, this.chPosition, this.chErreur });
		this.lvResults.FullRowSelect = true;
		this.lvResults.Location = new System.Drawing.Point(16, 59);
		this.lvResults.MultiSelect = false;
		this.lvResults.Name = "lvResults";
		this.lvResults.Size = new System.Drawing.Size(656, 333);
		this.lvResults.TabIndex = 7;
		this.lvResults.UseCompatibleStateImageBehavior = false;
		this.lvResults.View = System.Windows.Forms.View.Details;
		this.chImg.Text = "";
		this.chImg.Width = 25;
		this.chObjet.Text = "Objet";
		this.chObjet.Width = 70;
		this.chPosition.Text = "Position";
		this.chPosition.Width = 140;
		this.chErreur.Text = "Erreur";
		this.chErreur.Width = 342;
		this.panel1.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.panel1.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
		this.panel1.Controls.Add(this.bArreter);
		this.panel1.Controls.Add(this.bLancer);
		this.panel1.Controls.Add(this.lTexte);
		this.panel1.Controls.Add(this.lvResults);
		this.panel1.Controls.Add(this.progressBar);
		this.panel1.Location = new System.Drawing.Point(12, 12);
		this.panel1.Name = "panel1";
		this.panel1.Size = new System.Drawing.Size(679, 404);
		this.panel1.TabIndex = 3;
		this.bArreter.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
		this.bArreter.Enabled = false;
		this.bArreter.Location = new System.Drawing.Point(597, 35);
		this.bArreter.Name = "bArreter";
		this.bArreter.Size = new System.Drawing.Size(75, 22);
		this.bArreter.TabIndex = 6;
		this.bArreter.Text = "Arreter";
		this.bArreter.UseVisualStyleBackColor = true;
		this.bLancer.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
		this.bLancer.Location = new System.Drawing.Point(597, 11);
		this.bLancer.Name = "bLancer";
		this.bLancer.Size = new System.Drawing.Size(75, 22);
		this.bLancer.TabIndex = 5;
		this.bLancer.Text = "Lancer...";
		this.bLancer.UseVisualStyleBackColor = true;
		this.bFermer.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
		this.bFermer.Location = new System.Drawing.Point(616, 422);
		this.bFermer.Name = "bFermer";
		this.bFermer.Size = new System.Drawing.Size(75, 23);
		this.bFermer.TabIndex = 8;
		this.bFermer.Text = "Fermer";
		this.bFermer.UseVisualStyleBackColor = true;
		this.timer.Interval = 200;
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.ClientSize = new System.Drawing.Size(703, 457);
		base.Controls.Add(this.bFermer);
		base.Controls.Add(this.panel1);
		this.MinimumSize = new System.Drawing.Size(450, 300);
		base.Name = "VerifBaseForm";
		base.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
		this.Text = "Vérifier la base...";
		this.panel1.ResumeLayout(false);
		this.panel1.PerformLayout();
		base.ResumeLayout(false);
	}
}
