using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using CDV_Viewer.Data;
using CDV_Viewer.DockControls;

namespace CDV_Viewer.Traitements;

public static class UltraPOTReceiver
{
	public enum HostWindowMessages
	{
		HOST_ACCEPT = 1224,
		GO_TO,
		EXIT
	}

	private const int WM_USER = 1024;

	private const int WM_CDV_VIEWER_MIN = 1224;

	private const int WM_CDV_VIEWER_MAX = 1226;

	private static IntPtr _ultraPotHandle = IntPtr.Zero;

	public static string XmlFile;

	public static string NomTournee;

	public static bool Connected => _ultraPotHandle != IntPtr.Zero;

	public static void Create(IntPtr handle)
	{
		_ultraPotHandle = handle;
	}

	public static void OnReceiveMessage(Message message)
	{
		if (message.Msg >= 1224 && message.Msg <= 1226)
		{
			switch (message.Msg)
			{
			case 1225:
				GoTo((int)message.WParam, (int)message.LParam);
				break;
			case 1226:
				Global.MainForm.Close();
				break;
			}
		}
	}

	private static void GoTo(int ligne, int pk)
	{
		if (Archives.CurrentArchive == null)
		{
			Archives.GetLastArchive().Load();
		}
		if (!Global.Parcours.IsOpen && XmlFile != string.Empty && NomTournee != string.Empty)
		{
			MessageBox.Show(XmlFile + " - " + NomTournee);
			Global.Parcours.Load(XmlFile, NomTournee);
		}
		if (ComposantsViewer.Viewer != null)
		{
			ComposantsViewer.Viewer.SetLignePK(Math.Abs(ligne), pk);
			ComposantsViewer.Viewer.PkCroissant = ligne > 0;
		}
	}

	public static void Exit()
	{
		if (Connected)
		{
			uint msg = 1226u;
			PostMessage(_ultraPotHandle, msg, 0, 0);
		}
	}

	public static void Accept(IntPtr handle)
	{
		if (Connected)
		{
			uint msg = 1224u;
			PostMessage(_ultraPotHandle, msg, (int)handle, 0);
		}
	}

	[DllImport("user32.dll")]
	private static extern bool PostMessage(IntPtr hWnd, uint Msg, int wParam, int lParam);
}
