using System;
using System.Drawing;
using System.Windows.Forms;

namespace CDV_Viewer.Composants;

public class CFondParcours : Composant
{
	private CVoieOnLine _support;

	protected int _pkD;

	protected int _pkF;

	public CVoieOnLine Support => _support;

	public int PKD => _pkD;

	public int PKF => _pkF;

	public override bool IsComposantSignalisation => false;

	public CFondParcours(CVoieOnLine support, int pkD, int pkF)
	{
		if (support == null)
		{
			_erreur = true;
			return;
		}
		_support = support;
		_pkD = pkD;
		_pkF = pkF;
	}

	public override bool IsInGraph()
	{
		if (base.ComposantsViewer.PkD < _pkD)
		{
			return base.ComposantsViewer.PkF > _pkF;
		}
		return false;
	}

	protected override bool OnPaint(PaintEventArgs e)
	{
		if (!_support.Visible)
		{
			return false;
		}
		Pen pen = new Pen(new SolidBrush(Color.FromArgb(100, 0, 230, 230)), 6f);
		Point[] path = _support.GetPath(_pkD, _pkF);
		for (int i = 0; i < path.Length - 1; i++)
		{
			e.Graphics.DrawLine(pen, path[i].X, path[i].Y, path[i + 1].X, path[i + 1].Y);
		}
		if (path[0].Y == path[1].Y)
		{
			int num = (path[1].X * 2 + path[0].X) / 3;
			int num2 = (path[1].X + path[0].X * 2) / 3;
			int y = path[0].Y;
			if (Math.Abs(path[1].X - path[0].X) < 100)
			{
				num = (num + num2) / 2;
				num2 = num;
			}
			if (_pkD < _pkF == base.ComposantsViewer.PkCroissant)
			{
				e.Graphics.DrawLine(pen, num - 10, y - 10, num, y);
				e.Graphics.DrawLine(pen, num - 10, y + 10, num, y);
				e.Graphics.DrawLine(pen, num2 - 10, y - 10, num2, y);
				e.Graphics.DrawLine(pen, num2 - 10, y + 10, num2, y);
			}
			else
			{
				e.Graphics.DrawLine(pen, num + 10, y - 10, num, y);
				e.Graphics.DrawLine(pen, num + 10, y + 10, num, y);
				e.Graphics.DrawLine(pen, num2 + 10, y - 10, num2, y);
				e.Graphics.DrawLine(pen, num2 + 10, y + 10, num2, y);
			}
		}
		return true;
	}
}
