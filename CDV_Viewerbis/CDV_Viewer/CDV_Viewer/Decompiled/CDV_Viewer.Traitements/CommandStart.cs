using System.IO;
using CDV_Viewer.Data;
using CDV_Viewer.DockControls;

namespace CDV_Viewer.Traitements;

public static class CommandStart
{
	public static void Refresh()
	{
		if (!File.Exists(Paths.CStart))
		{
			return;
		}
		string[] array = File.ReadAllText(Paths.CStart).Split(';');
		if (array.Length == 3 && int.TryParse(array[0], out var result) && int.TryParse(array[1], out var result2))
		{
			ComposantsViewer.Viewer.SetLignePK(result, result2);
			if (array[2] == "-")
			{
				ComposantsViewer.Viewer.PkCroissant = false;
			}
		}
		File.Delete(Paths.CStart);
	}
}
