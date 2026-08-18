using System.Windows.Forms;
using CDV_Viewer.Composants;
using CDV_Viewer.Data;
using CDV_Viewer.Data.SIGObjects;
using CDV_Viewer.Properties;

namespace CDV_Viewer.Traitements;

public static class Dialogs
{
	public static bool BaseLinkError;

	public static SIGVoie FirstErrorVoie;

	public static int FirstErrorPk;

	public static bool Confirm(string text, AuthorizedMode editMode = AuthorizedMode.Always)
	{
		if (editMode == AuthorizedMode.Edit && !Global.ModeEdition)
		{
			return true;
		}
		if (editMode == AuthorizedMode.Read && Global.ModeEdition)
		{
			return true;
		}
		return MessageBox.Show(text, Resources.APP_NAME, MessageBoxButtons.YesNo) == DialogResult.Yes;
	}

	public static void Message(string text, AuthorizedMode editMode = AuthorizedMode.Always)
	{
		if ((editMode != AuthorizedMode.Edit || Global.ModeEdition) && (editMode != AuthorizedMode.Read || !Global.ModeEdition))
		{
			MessageBox.Show(text, Resources.APP_NAME, MessageBoxButtons.OK);
		}
	}

	public static void BaseError(string text, SIGVoie voie, int pk)
	{
		if (Autorisations.Values.Edition && !BaseLinkError)
		{
			BaseLinkError = true;
			FirstErrorVoie = voie;
			FirstErrorPk = pk;
			text += $"\n sur {voie} au pk {Chaines.PkToString(pk)}";
			MessageBox.Show(text, Resources.APP_NAME, MessageBoxButtons.OK);
		}
	}

	public static void ClearError()
	{
		BaseLinkError = false;
		FirstErrorVoie = null;
		FirstErrorPk = 0;
	}
}
