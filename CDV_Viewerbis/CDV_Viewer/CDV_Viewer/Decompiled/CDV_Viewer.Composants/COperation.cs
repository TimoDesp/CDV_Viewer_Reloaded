using System.Drawing;

namespace CDV_Viewer.Composants;

public abstract class COperation : Composant
{
	public override bool Visible => true;

	public COperation()
	{
		_ordre = 0;
	}

	public override bool Contains(Point Pt, int pkPoint)
	{
		return true;
	}

	public override bool Contains(Point Pt)
	{
		return true;
	}

	public override bool IsInGraph()
	{
		return true;
	}
}
