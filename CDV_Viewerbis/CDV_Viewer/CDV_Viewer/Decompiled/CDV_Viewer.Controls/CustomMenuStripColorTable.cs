using System.Drawing;
using System.Windows.Forms;

namespace CDV_Viewer.Controls;

internal class CustomMenuStripColorTable : ProfessionalColorTable
{
	public override Color MenuBorder => Color.FromArgb(152, 143, 134);

	public override Color MenuItemBorder => MenuBorder;

	public override Color MenuItemSelectedGradientBegin => Color.FromArgb(224, 222, 216);

	public override Color MenuItemSelectedGradientEnd => MenuItemSelectedGradientBegin;

	public override Color MenuItemPressedGradientBegin => Color.FromArgb(244, 242, 236);

	public override Color MenuItemPressedGradientEnd => MenuItemPressedGradientBegin;

	public override Color MenuItemSelected => MenuItemSelectedGradientBegin;

	public override Color ToolStripDropDownBackground => MenuItemPressedGradientBegin;
}
