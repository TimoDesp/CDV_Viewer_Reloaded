using System.Windows.Forms;
using CDV_Viewer.CsvBase;
using CDV_Viewer.Data;
using CDV_Viewer.Forms;
using CDV_Viewer.Properties;
using CDV_Viewer.Traitements;

namespace CDV_Viewer.Menu;

public class FileMenu : CdvViewerMenu
{
	private ToolStripMenuItem tsmiOuvrir;

	private ToolStripMenuItem tsmiOuvrirDerniere;

	private ToolStripMenuItem tsmiEnregistrer;

	private ToolStripMenuItem tsmiFermer;

	private ToolStripSeparator tss1;

	private ToolStripMenuItem tsmiImporter;

	private ToolStripMenuItem tsmiImporterModele;

	private ToolStripMenuItem tsmiImporterRefMgv;

	private ToolStripMenuItem tsmiExporter;

	private ToolStripMenuItem tsmiVerifier;

	private ToolStripSeparator tss2;

	private ToolStripMenuItem tsmiQuitter;

	public FileMenu()
	{
		InitializeComponent();
	}

	private void InitializeComponent()
	{
		Text = "Fichier";
		AddMenuItem(ref tsmiOuvrir, "Ouvrir...", Resources.open, OuvrirArchive, Keys.O | Keys.Control);
		AddMenuItem(ref tsmiOuvrirDerniere, "Ouvrir la dernière archive", Resources.recent_file, OuvrirDerniereArchive, Keys.D | Keys.Control);
		AddMenuItem(ref tsmiEnregistrer, "Enregistrer...", Resources.save, ArchiveFonctions.Enregistrer, Keys.S | Keys.Control);
		AddMenuItem(ref tsmiFermer, "Fermer", Resources.close_file, Fermer, Keys.F | Keys.Control);
		AddSeparator(ref tss1);
		tsmiImporterModele = CreateMenuItem("Fichier Modele", Resources.Graph, ImportModeles.Import);
		tsmiImporterRefMgv = CreateMenuItem("Fichiers REF_MGV", null, ImportMGV.Import);
		AddMenuItem(ref tsmiImporter, "Importer...", Resources.Import, new ToolStripItem[2] { tsmiImporterModele, tsmiImporterRefMgv });
		AddMenuItem(ref tsmiExporter, "Exporter...", Resources.Export, Exporter, Keys.E | Keys.Control);
		AddMenuItem(ref tsmiVerifier, "Vérifier la base...", Resources.Check_base, Verifier, Keys.V | Keys.Control);
		AddSeparator(ref tss2);
		AddMenuItem(ref tsmiQuitter, "Quitter", null, Close);
	}

	protected override void RefreshItems()
	{
		_ = Archives.IsOpen;
		bool edition = Autorisations.Values.Edition;
		bool enabled = Global.ModeEdition && Archives.IsOpen && Autorisations.Values.Edition;
		tsmiOuvrir.Visible = edition;
		tsmiOuvrirDerniere.Visible = edition && Archives.GetLastArchive() != null;
		tsmiEnregistrer.Visible = edition;
		tsmiEnregistrer.Enabled = enabled;
		tsmiFermer.Visible = edition;
		tsmiFermer.Enabled = Archives.IsOpen;
		tss1.Visible = edition;
		tsmiImporter.Visible = edition;
		tsmiImporter.Enabled = enabled;
		tsmiExporter.Visible = edition;
		tsmiExporter.Enabled = Archives.IsOpen;
		ToolStripMenuItem toolStripMenuItem = tsmiImporterModele;
		bool visible = (tsmiImporterRefMgv.Visible = edition);
		toolStripMenuItem.Visible = visible;
		tsmiImporterModele.Enabled = enabled;
		tsmiImporterRefMgv.Enabled = !Archives.IsOpen && Global.ModeEdition;
		tss2.Visible = edition;
		tsmiVerifier.Visible = edition;
		tsmiVerifier.Enabled = Archives.IsOpen;
	}

	private void Close()
	{
		Global.MainForm?.Close();
	}

	private void OuvrirArchive()
	{
		OpenArchiveDialog openArchiveDialog = new OpenArchiveDialog();
		if (openArchiveDialog.ShowDialog() == DialogResult.OK)
		{
			openArchiveDialog.SelectedArchive.Load();
		}
	}

	private void OuvrirDerniereArchive()
	{
		Archives.GetLastArchive()?.Load();
	}

	private void Fermer()
	{
		if (!Base.IsSave)
		{
			switch (MessageBox.Show("Voulez-vous sauvegarder l'archive avant de la fermer ?", Resources.APP_NAME, MessageBoxButtons.YesNoCancel))
			{
			case DialogResult.Yes:
				ArchiveFonctions.Enregistrer();
				break;
			case DialogResult.Cancel:
				return;
			}
		}
		ArchiveFonctions.Fermer();
	}

	private void Exporter()
	{
		ExportDialog exportDialog = new ExportDialog();
		if (exportDialog.ShowDialog() == DialogResult.OK)
		{
			if (exportDialog.ToutesLignes)
			{
				ExportMGV.Export(exportDialog.OutPath, exportDialog.Archive, exportDialog.Loc, exportDialog.MGV, exportDialog.UltraPot, exportDialog.Timon);
			}
			else
			{
				ExportMGV.Export(exportDialog.OutPath, exportDialog.Lignes, exportDialog.Archive, exportDialog.Loc, exportDialog.MGV, exportDialog.UltraPot, exportDialog.Timon);
			}
		}
	}

	private void Verifier()
	{
		new VerifBaseForm().Show();
	}
}
