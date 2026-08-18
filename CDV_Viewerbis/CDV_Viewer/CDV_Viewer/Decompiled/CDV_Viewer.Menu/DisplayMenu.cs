using System.Windows.Forms;
using CDV_Viewer.Data;
using CDV_Viewer.DockControls;
using CDV_Viewer.Properties;

namespace CDV_Viewer.Menu;

public class DisplayMenu : CdvViewerMenu
{
	private ToolStripMenuItem tsmiModeAffichage;

	private ToolStripMenuItem tsmiModeTopologie;

	private ToolStripMenuItem tsmiModeSignalisation;

	private ToolStripSeparator tss1;

	private ToolStripMenuItem tsmiListeLignes;

	private ToolStripSeparator tss2;

	private ToolStripMenuItem tsmiInfobulles;

	private ToolStripMenuItem tsmiLegende;

	private ToolStripMenuItem tsmiShowEmetteurs;

	public DisplayMenu()
	{
		InitializeComponent();
	}

	protected override void RefreshItems()
	{
		ComposantsViewer viewer = ComposantsViewer.Viewer;
		tsmiInfobulles.Checked = viewer.ShowInfobulle;
		tsmiListeLignes.Checked = Global.ListeLignes.Visible;
		tsmiLegende.Checked = viewer.Legende.Visible;
		tsmiModeTopologie.Checked = viewer.ModeVisualisation == ModeVisualisation.Topologie;
		tsmiModeSignalisation.Checked = viewer.ModeVisualisation == ModeVisualisation.Signalisation;
		tsmiShowEmetteurs.Visible = viewer.ModeVisualisation == ModeVisualisation.Signalisation;
		tsmiShowEmetteurs.Checked = viewer.ShowEmetteurs;
	}

	private void InitializeComponent()
	{
		ComposantsViewer viewer = ComposantsViewer.Viewer;
		Text = "Affichage";
		tsmiModeTopologie = CreateMenuItem("Topologie", ModeTopologie, viewer.ModeVisualisation == ModeVisualisation.Topologie);
		tsmiModeSignalisation = CreateMenuItem("Signalisation", ModeSignalisation, viewer.ModeVisualisation != ModeVisualisation.Topologie);
		AddMenuItem(ref tsmiModeAffichage, "Mode d'Affichage", null, new ToolStripItem[2] { tsmiModeTopologie, tsmiModeSignalisation });
		AddSeparator(ref tss1);
		AddMenuItem(ref tsmiListeLignes, "Afficher la liste des lignes", ListeLignes, Global.ListeLignes.Visible);
		AddSeparator(ref tss2);
		AddMenuItem(ref tsmiInfobulles, "Afficher les infobulles", Resources.infobulle, Infobulles, Preferences.Affichage.InfoBulles);
		AddMenuItem(ref tsmiLegende, "Afficher la légende", Resources.info, Legende, Keys.E | Keys.Control, Preferences.Affichage.Legende);
		AddMenuItem(ref tsmiShowEmetteurs, "Afficher les Emetteurs", null, delegate
		{
			viewer.ShowEmetteurs = !viewer.ShowEmetteurs;
		});
	}

	private void ModeTopologie()
	{
		ComposantsViewer.Viewer.ModeVisualisation = ModeVisualisation.Topologie;
	}

	private void ModeSignalisation()
	{
		ComposantsViewer.Viewer.ModeVisualisation = ModeVisualisation.Signalisation;
	}

	private void ListeLignes()
	{
		Global.ListeLignes.Visible = !Global.ListeLignes.Visible;
	}

	private void Infobulles()
	{
		ComposantsViewer.Viewer.ShowInfobulle = !ComposantsViewer.Viewer.ShowInfobulle;
	}

	private void Legende()
	{
		ComposantsViewer.Viewer.Legende.Visible = !ComposantsViewer.Viewer.Legende.Visible;
	}
}
