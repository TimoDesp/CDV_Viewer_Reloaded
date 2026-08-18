using System.Drawing;

namespace CDV_Viewer.Controls;

public class SimpleListViewItem
{
	public object Tag;

	public string Text = string.Empty;

	public Image Image;

	public SimpleListViewItem(object tag, string text)
	{
		Tag = tag;
		Text = text;
	}

	public SimpleListViewItem(object tag, Image image)
	{
		Tag = tag;
		Image = image;
	}

	public SimpleListViewItem(object tag, string text, Image image)
	{
		Tag = tag;
		Text = text;
		Image = image;
	}
}
