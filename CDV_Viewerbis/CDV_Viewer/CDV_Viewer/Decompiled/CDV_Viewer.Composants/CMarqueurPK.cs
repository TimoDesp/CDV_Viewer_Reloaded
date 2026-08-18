using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using CDV_Viewer.Data;
using CDV_Viewer.Traitements;

namespace CDV_Viewer.Composants;

public class CMarqueurPK : Composant
{
	private int _pk;

	private bool _yPos;

	public int PK => _pk;

	public override bool IsComposantSignalisation => false;

	public CMarqueurPK(int pk, bool yPos)
	{
		_pk = pk;
		_yPos = yPos;
	}

	public override bool IsInGraph()
	{
		if (base.ComposantsViewer.PkD < _pk)
		{
			return base.ComposantsViewer.PkF > _pk;
		}
		return false;
	}

	protected override bool OnPaint(PaintEventArgs e)
	{
		int num = base.ComposantsViewer.PkToLocation(_pk);
		Pen pen = new Pen(Color.Gray);
		pen.DashStyle = DashStyle.Dash;
		StringFormat stringFormat = new StringFormat();
		stringFormat.Alignment = StringAlignment.Center;
		Font defaultFont = Global.DefaultFont;
		int num2 = base.ComposantsViewer.GraphHeight;
		if (_yPos)
		{
			num2 -= 15;
		}
		e.Graphics.DrawLine(pen, num, 50, num, num2);
		e.Graphics.DrawString(Chaines.PkToString(_pk), defaultFont, Brushes.Gray, new Rectangle(num - 25, num2, 50, 20), stringFormat);
		return true;
	}

	public void SetPK(int newPk)
	{
		_pk = newPk;
	}
}
