using System.IO;
using System.Windows.Forms;
using CDV_Viewer.Data;
using CDV_Viewer.Forms;

namespace CDV_Viewer.Menu;

public class TourneeMenu : CdvViewerMenu
{
	private ToolStripMenuItem tsmiOuvrir;

	private ToolStripMenuItem tsmiFermer;

	public TourneeMenu()
	{
		InitializeComponent();
	}

	private void InitializeComponent()
	{
		Text = "Tournée";
		AddMenuItem(ref tsmiOuvrir, "Ouvrir...", Ouvrir);
		AddMenuItem(ref tsmiFermer, "Fermer", Fermer);
	}

	protected override void RefreshItems()
	{
		bool flag = Archives.CurrentArchive != null;
		ToolStripMenuItem toolStripMenuItem = tsmiOuvrir;
		bool enabled = (tsmiFermer.Enabled = flag);
		toolStripMenuItem.Enabled = enabled;
		if (Global.Parcours.IsOpen)
		{
			Text = Global.Parcours.NomTournee;
			tsmiFermer.Visible = true;
		}
		else
		{
			Text = "Tournée";
			tsmiFermer.Visible = false;
		}
	}

	private void Ouvrir()
	{
		OpenFileDialog openFileDialog = new OpenFileDialog();
		if (openFileDialog.ShowDialog() != DialogResult.OK)
		{
			return;
		}
		string text = Path.GetExtension(openFileDialog.FileName).ToLower();
		if (text == ".xml")
		{
			OpenTourneeDialog openTourneeDialog = new OpenTourneeDialog(openFileDialog.FileName);
			if (!openTourneeDialog.Erreur && openTourneeDialog.ShowDialog() == DialogResult.OK)
			{
				Global.Parcours.Load(openFileDialog.FileName, openTourneeDialog.SelectedTournee);
			}
		}
		else
		{
			_ = text == ".csv";
		}
	}

	private void Fermer()
	{
		Global.Parcours?.Close();
	}
}
