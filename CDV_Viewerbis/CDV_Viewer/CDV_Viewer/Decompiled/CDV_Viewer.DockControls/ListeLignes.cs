using System;
using System.Drawing;
using System.Windows.Forms;
using CDV_Viewer.Controls;
using CDV_Viewer.CsvBase;
using CDV_Viewer.Data;
using CDV_Viewer.Data.SIGObjects;

namespace CDV_Viewer.DockControls;

public class ListeLignes : DockChild
{
	private Label lTitre;

	private CloseButton bFermer;

	private Separator separator;

	private SimpleListView lvLignes;

	public int CurrentLigne => (int)(lvLignes.SelectedItem?.Tag ?? ((object)(-1)));

	public ListeLignes()
	{
		InitializeComponent();
	}

	private void InitializeComponent()
	{
		this.MinimumSize = new System.Drawing.Size(75, 100);
		this.lTitre = new System.Windows.Forms.Label();
		this.lTitre.Height = 14;
		this.lTitre.Text = "LIGNES";
		this.lTitre.Font = new System.Drawing.Font("Arial", 9f);
		this.lTitre.ForeColor = System.Drawing.Color.FromArgb(224, 82, 6);
		this.lTitre.Left = 6;
		this.lTitre.Top = 6;
		this.lTitre.Width = 52;
		base.Controls.Add(this.lTitre);
		this.bFermer = new CDV_Viewer.Controls.CloseButton();
		this.bFermer.Left = base.Width - 20;
		this.bFermer.Top = 6;
		this.bFermer.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
		this.bFermer.Click += new System.EventHandler(bFermer_Click);
		base.Controls.Add(this.bFermer);
		this.separator = new CDV_Viewer.Controls.Separator();
		this.separator.Left = 10;
		this.separator.Width = base.Width - 20;
		this.separator.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.separator.ForeColor = System.Drawing.Color.Gainsboro;
		this.separator.Top = 25;
		base.Controls.Add(this.separator);
		this.lvLignes = new CDV_Viewer.Controls.SimpleListView(CDV_Viewer.Controls.ScrollBarOrientation.Vertical);
		this.lvLignes.Top = 30;
		this.lvLignes.Width = base.Width;
		this.lvLignes.Height = base.Height - 20;
		this.lvLignes.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.lvLignes.Font = new System.Drawing.Font("Arial", 9f);
		this.lvLignes.ItemSize = 16;
		base.Controls.Add(this.lvLignes);
		this.Base_ListLignesChanged(this, new System.EventArgs());
		CDV_Viewer.Data.Archives.CurrentArchiveChanged += new System.EventHandler(Base_ListLignesChanged);
		CDV_Viewer.CsvBase.Base.ListLignesChanged += new System.EventHandler(Base_ListLignesChanged);
		CDV_Viewer.DockControls.ComposantsViewer.Viewer.SelectedLigneChanging += new System.EventHandler(ComposantsViewer_SelectedLigneChanging);
		CDV_Viewer.DockControls.ComposantsViewer.Viewer.SelectedLigneChanged += new System.EventHandler(ComposantsViewer_SelectedLigneChanged);
		this.lvLignes.SelectedIndexChanged += new System.EventHandler(lvLignes_SelectedIndexChanged);
	}

	private void bFermer_Click(object sender, EventArgs e)
	{
		base.Visible = false;
	}

	private void Base_ListLignesChanged(object sender, EventArgs e)
	{
		lvLignes.Items.Clear();
		if (Archives.CurrentArchive == null)
		{
			return;
		}
		foreach (SIGLigne ligne in Base.GetLignes())
		{
			lvLignes.Items.Add(new SimpleListViewItem(ligne.ID, ligne.ID + " : " + ligne.Nom));
		}
	}

	private void ComposantsViewer_SelectedLigneChanging(object sender, EventArgs e)
	{
		base.Enabled = false;
	}

	private void ComposantsViewer_SelectedLigneChanged(object sender, EventArgs e)
	{
		base.Enabled = true;
		for (int i = 0; i < lvLignes.Items.Count; i++)
		{
			if ((int)lvLignes.Items[i].Tag == ComposantsViewer.Viewer.LigneId)
			{
				lvLignes.SelectedIndex = i;
				break;
			}
		}
	}

	private void lvLignes_SelectedIndexChanged(object sender, EventArgs e)
	{
		if (lvLignes.SelectedItem != null)
		{
			ComposantsViewer.Viewer.SetLigne((int)lvLignes.SelectedItem.Tag);
		}
	}
}
