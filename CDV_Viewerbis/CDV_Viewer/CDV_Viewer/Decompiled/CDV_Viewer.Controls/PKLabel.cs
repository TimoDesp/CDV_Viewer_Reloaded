using System.Windows.Forms;

namespace CDV_Viewer.Controls;

public class PKLabel : Label
{
	public int GetPk()
	{
		int result = 0;
		if (int.TryParse(Text.Replace("+", ""), out result))
		{
			return result;
		}
		return 0;
	}

	public void SetPk(int pk)
	{
		string text = pk.ToString();
		if (text.Length <= 3)
		{
			for (int i = 0; i < 3 - text.Length; i++)
			{
				text = text.Insert(0, "0");
			}
			text = text.Insert(0, "0+");
		}
		else
		{
			text = text.Insert(text.Length - 3, "+");
		}
		Text = text;
	}
}
