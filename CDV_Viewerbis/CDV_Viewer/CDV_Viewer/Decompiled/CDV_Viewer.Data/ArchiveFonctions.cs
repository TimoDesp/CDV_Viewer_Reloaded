using System.Windows.Forms;
using CDV_Viewer.CsvBase;
using CDV_Viewer.Forms;
using CDV_Viewer.Properties;

namespace CDV_Viewer.Data;

public static class ArchiveFonctions
{
	public static void Enregistrer()
	{
		switch (MessageBox.Show("Voulez-vous créer une nouvelle archive ?", Resources.APP_NAME, MessageBoxButtons.YesNoCancel))
		{
		default:
			return;
		case DialogResult.Yes:
		{
			SaveDialog saveDialog = new SaveDialog();
			if (saveDialog.ShowDialog() != DialogResult.OK)
			{
				return;
			}
			Base.SaveToTempFolder();
			Archives.CreateArchiveFromTempFolder(saveDialog.Description);
			Base.SaveToBinary();
			break;
		}
		case DialogResult.No:
			Base.SaveToTempFolder();
			Archives.CurrentArchive.Save();
			Base.SaveToBinary();
			break;
		}
		MessageBox.Show("Archive enregistrée !", Resources.APP_NAME);
	}

	public static void Fermer()
	{
		if (Archives.CurrentArchive != null)
		{
			Base.Close();
			Archives.CurrentArchive.Close();
		}
	}
}
