using System.Drawing;

namespace CDV_Viewer.CustomControls;

public class CustomTopMenuItem
{
	public string Tag = string.Empty;

	public string Text = string.Empty;

	public Image Image = new Bitmap(16, 16);

	public CustomTopMenuItem()
	{
	}

	public CustomTopMenuItem(string tag, string texte, Image image)
	{
		Tag = tag;
		Text = texte;
		Image = image;
	}

	public CustomTopMenuItem(string tag, string texte)
	{
		Tag = tag;
		Text = texte;
	}
}
