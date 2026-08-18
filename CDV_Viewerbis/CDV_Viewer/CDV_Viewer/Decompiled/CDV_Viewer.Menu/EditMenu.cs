using System.Windows.Forms;
using CDV_Viewer.CsvBase;
using CDV_Viewer.Data;
using CDV_Viewer.Forms;
using CDV_Viewer.Properties;
using CDV_Viewer.Traitements;

namespace CDV_Viewer.Menu;

public class EditMenu : CdvViewerMenu
{
	private ToolStripMenuItem tsmiAddLigne;

	private ToolStripMenuItem tsmiCorresTIMON;

	private ToolStripMenuItem tsmiEditBase;

	private ToolStripSeparator tss1;

	private ToolStripMenuItem tsmiModeEdition;

	private ToolStripSeparator tss2;

	private ToolStripMenuItem tsmiMajAutoUltraPOT;

	public EditMenu()
	{
		InitializeComponent();
	}

	private void InitializeComponent()
	{
		Text = "Edition";
		AddMenuItem(ref tsmiAddLigne, "Ajouter une ligne...", AddLigneDialog.Open);
		AddMenuItem(ref tsmiCorresTIMON, "Correspondance TIMON...", CorrespondancesTimonDialog.Open);
		AddMenuItem(ref tsmiEditBase, "Editer la base...", Resources.Edit, EditBaseDialog.Open);
		AddSeparator(ref tss1);
		AddMenuItem(ref tsmiModeEdition, "Mode Edition", ModeEdition);
		AddSeparator(ref tss2);
		AddMenuItem(ref tsmiMajAutoUltraPOT, "MAJ UltraPOT Automatique", Resources.ultrapot, MajAutoUltraPOT);
	}

	protected override void RefreshItems()
	{
		bool flag = Global.ModeEdition && Archives.IsOpen && Autorisations.Values.Edition;
		base.Visible = Autorisations.Values.Edition;
		tsmiModeEdition.Enabled = Archives.IsOpen;
		tsmiModeEdition.Checked = Global.ModeEdition;
		ToolStripMenuItem toolStripMenuItem = tsmiAddLigne;
		ToolStripMenuItem toolStripMenuItem2 = tsmiCorresTIMON;
		ToolStripMenuItem toolStripMenuItem3 = tsmiEditBase;
		bool flag2 = (tsmiMajAutoUltraPOT.Enabled = flag);
		bool flag4 = (toolStripMenuItem3.Enabled = flag2);
		bool enabled = (toolStripMenuItem2.Enabled = flag4);
		toolStripMenuItem.Enabled = enabled;
	}

	private void ModeEdition()
	{
		if (!Global.ModeEdition)
		{
			if (MessageBox.Show("Vous allez passer en mode édition. Pensez à faire une sauvegarde de la base avant de la modifier.", Resources.APP_NAME, MessageBoxButtons.OKCancel) == DialogResult.OK)
			{
				Global.ModeEdition = true;
			}
		}
		else
		{
			if (!Base.IsSave)
			{
				switch (MessageBox.Show("Voulez-vous sauvegarder l'archive ?", Resources.APP_NAME, MessageBoxButtons.YesNoCancel))
				{
				case DialogResult.Yes:
					ArchiveFonctions.Enregistrer();
					break;
				case DialogResult.Cancel:
					return;
				}
			}
			Global.ModeEdition = false;
		}
		tsmiModeEdition.Checked = Global.ModeEdition;
	}

	private void MajAutoUltraPOT()
	{
		if (Global.ModeEdition)
		{
			MajUltraPOT.Actif = !MajUltraPOT.Actif;
			tsmiMajAutoUltraPOT.Checked = MajUltraPOT.Actif;
		}
	}
}
