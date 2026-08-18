using System.Windows.Forms;
using CDV_Viewer.Data;
using CDV_Viewer.Properties;

namespace CDV_Viewer.Menu;

public class ToolMenu : CdvViewerMenu
{
	private ToolStripMenuItem tsmiLiveControl;

	private ToolStripMenuItem tsmiTourneeViewer;

	public ToolMenu()
	{
		InitializeComponent();
	}

	private void InitializeComponent()
	{
		Text = "Outils";
		AddMenuItem(ref tsmiLiveControl, "Live Control", LiveControl);
		AddMenuItem(ref tsmiTourneeViewer, "Visualisateur de tournée", Resources.ultrapot, TourneeViewer);
	}

	protected override void RefreshItems()
	{
		bool enabled = Archives.CurrentArchive != null;
		tsmiLiveControl.Enabled = enabled;
		tsmiTourneeViewer.Enabled = enabled;
	}

	private void LiveControl()
	{
		if (Global.LiveControl != null)
		{
			Global.LiveControl.Visible = true;
		}
	}

	private void TourneeViewer()
	{
		if (Global.ParcoursControl != null)
		{
			Global.ParcoursControl.Visible = true;
		}
	}
}
