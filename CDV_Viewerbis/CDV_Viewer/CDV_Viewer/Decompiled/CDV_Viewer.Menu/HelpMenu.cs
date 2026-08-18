using System.Windows.Forms;
using CDV_Viewer.Data;
using CDV_Viewer.Forms;
using CDV_Viewer.Properties;

namespace CDV_Viewer.Menu;

public class HelpMenu : CdvViewerMenu
{
	private ToolStripMenuItem tsmiAide;

	private ToolStripMenuItem tsmiRaccourcis;

	private ToolStripMenuItem tsmiAPropos;

	public HelpMenu()
	{
		InitializeComponent();
	}

	private void InitializeComponent()
	{
		Text = "?";
		AddMenuItem(ref tsmiAide, "Aide...", Resources.help, Aide);
		AddMenuItem(ref tsmiRaccourcis, "Raccourcis...", null, Raccourcis);
		AddMenuItem(ref tsmiAPropos, "À propos...", null, APropos);
	}

	private void Aide()
	{
		if (Global.HelpControl != null)
		{
			Global.HelpControl.Visible = true;
		}
	}

	private void Raccourcis()
	{
		new RaccourcisDialog().ShowDialog();
	}

	private void APropos()
	{
		new AProposDialog().ShowDialog();
	}
}
