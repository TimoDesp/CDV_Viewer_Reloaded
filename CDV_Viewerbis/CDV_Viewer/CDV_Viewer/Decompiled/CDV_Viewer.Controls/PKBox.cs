using System.Windows.Forms;
using CDV_Viewer.CustomControls;
using CDV_Viewer.Traitements;

namespace CDV_Viewer.Controls;

public class PKBox : CustomTextBox
{
	public bool CorrectPK
	{
		get
		{
			int result = 0;
			return int.TryParse(base.Text.Replace("+", ""), out result);
		}
	}

	public int GetPk()
	{
		int PK = 0;
		if (!Chaines.TryParsePk(base.Text, out PK))
		{
			return 0;
		}
		return PK;
	}

	public void SetPk(int pk)
	{
		base.Text = Chaines.PkToString(pk);
	}

	protected override void OnKeyDown(KeyEventArgs e)
	{
		e.Handled = true;
		if (char.IsDigit((char)e.KeyValue) || (ushort)e.KeyValue == 43 || (ushort)e.KeyValue == 45)
		{
			if (base.SelectionStart < base.Text.Length)
			{
				base.Text = base.Text.Remove(base.SelectionStart, 1);
			}
			base.Text = base.Text.Insert(base.SelectionStart, ((char)e.KeyValue).ToString());
		}
	}
}
