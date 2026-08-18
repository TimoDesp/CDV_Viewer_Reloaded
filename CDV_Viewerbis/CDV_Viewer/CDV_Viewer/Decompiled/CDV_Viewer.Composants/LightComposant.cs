using System.Drawing;
using System.Windows.Forms;

namespace CDV_Viewer.Composants;

public class LightComposant : Composant
{
	public CVoie Voie;

	public int Pk;

	public override Rectangle DisplayRectangle => new Rectangle(GetPoint(Pk), new Size(1, 1));

	public override bool IsComposantSignalisation => false;

	public override Point GetPoint(int Pk)
	{
		return Voie.GetPoint(Pk);
	}

	protected override bool OnPaint(PaintEventArgs e)
	{
		return false;
	}

	public LightComposant(CVoie voie, int pk)
	{
		Voie = voie;
		Pk = pk;
	}
}
