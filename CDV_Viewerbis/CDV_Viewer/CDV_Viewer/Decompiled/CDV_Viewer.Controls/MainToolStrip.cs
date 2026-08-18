using System;
using System.Drawing;
using System.Windows.Forms;
using CDV_Viewer.CsvBase;
using CDV_Viewer.Data;
using CDV_Viewer.Data.SIGObjects;
using CDV_Viewer.DockControls;
using CDV_Viewer.Forms;
using CDV_Viewer.Properties;
using CDV_Viewer.Styles;

namespace CDV_Viewer.Controls;

public class MainToolStrip : ToolStrip
{
	private ToolStripLabel tslLigne;

	private ToolStripComboBox tscbLigne;

	private ToolStripButton tsbPositionVoies;

	private ToolStripSeparator tss1;

	private ToolStripButton tsbActualiser;

	private ToolStripButton tsbZoomIn;

	private ToolStripButton tsbZoomOut;

	private ToolStripButton tsbLeft;

	private ToolStripButton tsbRight;

	private ToolStripButton tsbSensPk;

	private ToolStripSeparator tss2;

	private ToolStripLabel tslMode;

	private ToolStripComboBox tscbMode;

	private ToolStripButton tsbInfobulle;

	private ToolStripButton tsbLegende;

	public MainToolStrip()
	{
		InitializeComponent();
	}

	private void InitializeComponent()
	{
		CDV_Viewer.DockControls.ComposantsViewer viewer = CDV_Viewer.DockControls.ComposantsViewer.Viewer;
		base.BackColor = CDV_Viewer.Styles.Colors.GetColor("PanelToolStrip");
		this.tslLigne = new System.Windows.Forms.ToolStripLabel("Ligne :");
		this.Items.Add(this.tslLigne);
		this.tscbLigne = new System.Windows.Forms.ToolStripComboBox
		{
			AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.Append,
			Width = 5,
			DropDownWidth = 60,
			MaxLength = 6,
			Size = new System.Drawing.Size(60, 25)
		};
		this.tscbLigne.SelectedIndexChanged += delegate
		{
			this.Ligne_SelectedIndexChanged();
		};
		this.tscbLigne.KeyPress += delegate(object o, System.Windows.Forms.KeyPressEventArgs e)
		{
			this.Ligne_KeyPress(e);
		};
		this.Items.Add(this.tscbLigne);
		this.tscbLigne.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.SuggestAppend;
		this.tsbPositionVoies = new System.Windows.Forms.ToolStripButton("Position des voies...", CDV_Viewer.Properties.Resources.PosVoies)
		{
			DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.ImageAndText
		};
		this.tsbPositionVoies.Click += delegate
		{
			CDV_Viewer.Forms.PositionVoiesDialog.Open();
		};
		this.Items.Add(this.tsbPositionVoies);
		this.tss1 = new System.Windows.Forms.ToolStripSeparator();
		this.Items.Add(this.tss1);
		this.tsbActualiser = new System.Windows.Forms.ToolStripButton("Actualiser la ligne", CDV_Viewer.Properties.Resources.refresh)
		{
			DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image
		};
		this.tsbActualiser.Click += delegate
		{
			viewer.RefreshLigne();
		};
		this.Items.Add(this.tsbActualiser);
		this.tsbZoomIn = new System.Windows.Forms.ToolStripButton("Zoom +", CDV_Viewer.Properties.Resources.ZoomIn)
		{
			DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image
		};
		this.tsbZoomIn.Click += delegate
		{
			viewer.ZoomIn();
		};
		this.Items.Add(this.tsbZoomIn);
		this.tsbZoomOut = new System.Windows.Forms.ToolStripButton("Zoom -", CDV_Viewer.Properties.Resources.ZoomOut)
		{
			DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image
		};
		this.tsbZoomOut.Click += delegate
		{
			viewer.ZoomOut();
		};
		this.Items.Add(this.tsbZoomOut);
		this.tsbLeft = new System.Windows.Forms.ToolStripButton("Aller à gauche", CDV_Viewer.Properties.Resources.Left)
		{
			DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image
		};
		this.tsbLeft.Click += delegate
		{
			viewer.MoveLeft();
		};
		this.Items.Add(this.tsbLeft);
		this.tsbRight = new System.Windows.Forms.ToolStripButton("Aller à droite", CDV_Viewer.Properties.Resources.Right)
		{
			DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image
		};
		this.tsbRight.Click += delegate
		{
			viewer.MoveRight();
		};
		this.Items.Add(this.tsbRight);
		this.tsbSensPk = new System.Windows.Forms.ToolStripButton("Sens du PK", CDV_Viewer.Properties.Resources.plus)
		{
			DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image
		};
		this.tsbSensPk.Click += delegate
		{
			this.ToogleSensPk();
		};
		this.Items.Add(this.tsbSensPk);
		this.tss2 = new System.Windows.Forms.ToolStripSeparator();
		this.Items.Add(this.tss2);
		this.tslMode = new System.Windows.Forms.ToolStripLabel("Mode :");
		this.Items.Add(this.tslMode);
		this.tscbMode = new System.Windows.Forms.ToolStripComboBox
		{
			DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList,
			Size = new System.Drawing.Size(90, 25)
		};
		this.tscbMode.Items.Add("Topologie");
		this.tscbMode.Items.Add("Signalisation");
		this.tscbMode.SelectedIndex = (int)viewer.ModeVisualisation;
		this.tscbMode.SelectedIndexChanged += delegate
		{
			this.ChangeModeVisualisation();
		};
		this.Items.Add(this.tscbMode);
		this.tsbInfobulle = new System.Windows.Forms.ToolStripButton("Afficher/Cacher les infobulles", CDV_Viewer.Properties.Resources.infobulle)
		{
			DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image,
			Checked = CDV_Viewer.Data.Preferences.Affichage.InfoBulles
		};
		this.tsbInfobulle.Click += delegate
		{
			this.ToogleInfobulle();
		};
		this.Items.Add(this.tsbInfobulle);
		this.tsbLegende = new System.Windows.Forms.ToolStripButton("Afficher/Cacher la légende", CDV_Viewer.Properties.Resources.info)
		{
			DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image,
			Checked = CDV_Viewer.Data.Preferences.Affichage.Legende
		};
		this.tsbLegende.Click += delegate
		{
			this.ToogleLegendeVisible();
		};
		this.Items.Add(this.tsbLegende);
		CDV_Viewer.Data.Archives.CurrentArchiveChanged += delegate
		{
			this.Base_ListLignesChanged();
		};
		CDV_Viewer.CsvBase.Base.ListLignesChanged += delegate
		{
			this.Base_ListLignesChanged();
		};
		viewer.SelectedLigneChanged += delegate
		{
			this.SetSelectedLigne(viewer.LigneId);
		};
		viewer.SensPkChanged += delegate
		{
			this.SetImageSensPk();
		};
		viewer.ModeVisualisationChanged += delegate
		{
			this.tscbMode.SelectedIndex = (int)viewer.ModeVisualisation;
		};
		viewer.ShowInfobulleChanged += delegate
		{
			this.tsbInfobulle.Checked = viewer.ShowInfobulle;
		};
		viewer.Legende.VisibleChanged += delegate
		{
			this.tsbLegende.Checked = viewer.Legende.Visible;
		};
	}

	protected override void OnParentChanged(EventArgs e)
	{
		RefreshItems();
	}

	public void RefreshItems()
	{
		bool flag = Archives.CurrentArchive != null;
		bool enabled = ComposantsViewer.Viewer.State == ComposantViewerState.Displayed || ComposantsViewer.Viewer.State == ComposantViewerState.Loaded;
		bool edition = Autorisations.Values.Edition;
		bool visible = Global.ModeEdition && flag && edition;
		tslLigne.Enabled = flag;
		tscbLigne.Enabled = flag;
		tsbPositionVoies.Visible = visible;
		tsbActualiser.Enabled = enabled;
		tsbPositionVoies.Enabled = enabled;
		tsbZoomIn.Enabled = enabled;
		tsbZoomOut.Enabled = enabled;
		tsbLeft.Enabled = enabled;
		tsbRight.Enabled = enabled;
		tsbSensPk.Enabled = enabled;
		tslMode.Enabled = enabled;
		tscbMode.Enabled = enabled;
		tsbInfobulle.Enabled = enabled;
		tsbLegende.Enabled = enabled;
	}

	private void Ligne_SelectedIndexChanged()
	{
		if (int.TryParse(tscbLigne.Text, out var result) && result != ComposantsViewer.Viewer.LigneId && tscbLigne.Items.Contains(result))
		{
			ComposantsViewer.Viewer.SetLigne(result);
		}
	}

	private void Ligne_KeyPress(KeyPressEventArgs e)
	{
		e.Handled = !char.IsDigit(e.KeyChar) && e.KeyChar != '\b';
	}

	private void ChangeModeVisualisation()
	{
		ComposantsViewer.Viewer.ModeVisualisation = (ModeVisualisation)tscbMode.SelectedIndex;
	}

	private void ToogleSensPk()
	{
		ComposantsViewer.Viewer.PkCroissant = !ComposantsViewer.Viewer.PkCroissant;
	}

	private void ToogleInfobulle()
	{
		ComposantsViewer.Viewer.ShowInfobulle = !ComposantsViewer.Viewer.ShowInfobulle;
	}

	private void ToogleLegendeVisible()
	{
		ComposantsViewer.Viewer.Legende.Visible = !ComposantsViewer.Viewer.Legende.Visible;
	}

	private void Base_ListLignesChanged()
	{
		tscbLigne.Items.Clear();
		if (Archives.CurrentArchive == null)
		{
			return;
		}
		foreach (SIGLigne ligne in Base.GetLignes())
		{
			tscbLigne.Items.Add(ligne.ID);
		}
	}

	private int SetSelectedLigne(int ligneId)
	{
		if (ligneId == (int)(tscbLigne.SelectedItem ?? ((object)(-1))))
		{
			return tscbLigne.SelectedIndex;
		}
		int num = tscbLigne.Items.IndexOf(ligneId);
		if (num > 0)
		{
			return tscbLigne.SelectedIndex = num;
		}
		if (ComposantsViewer.Viewer.LigneId != ligneId)
		{
			ComposantsViewer.Viewer.SetLigne(ligneId);
		}
		return -1;
	}

	private void SetImageSensPk()
	{
		tsbSensPk.Image = (ComposantsViewer.Viewer.PkCroissant ? Resources.plus : Resources.moins);
	}
}
