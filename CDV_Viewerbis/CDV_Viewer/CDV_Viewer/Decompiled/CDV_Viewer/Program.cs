using System;
using System.Windows.Forms;
using CDV_Viewer.Forms;
using CDV_Viewer.Traitements;

namespace CDV_Viewer;

internal static class Program
{
	[STAThread]
	private static void Main(string[] args)
	{
		if (args.Length != 0 && int.TryParse(args[0], out var result))
		{
			UltraPOTReceiver.Create((IntPtr)result);
		}
		if (args.Length == 3)
		{
			UltraPOTReceiver.XmlFile = args[1];
			UltraPOTReceiver.NomTournee = args[2];
		}
		Application.EnableVisualStyles();
		Application.SetCompatibleTextRenderingDefault(defaultValue: false);
		Application.Run(new InitForm());
	}
}
