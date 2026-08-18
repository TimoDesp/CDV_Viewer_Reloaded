using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using CDV_Viewer.Menu;
using CDV_Viewer.Styles;

namespace CDV_Viewer.Controls;

public class MainMenuStrip : MenuStrip
{
	private FileMenu FileMenu;

	private EditMenu EditMenu;

	private DisplayMenu DisplayMenu;

	private ToolMenu ToolMenu;

	private TourneeMenu TourneeMenu;

	private HelpMenu HelpMenu;

	public MainMenuStrip()
	{
		InitializeComponent();
		base.ItemAdded += SIGMenu_ItemAdded;
	}

	private void InitializeComponent()
	{
		base.BackColor = CDV_Viewer.Styles.Colors.GetColor("FormMenu");
		base.RenderMode = System.Windows.Forms.ToolStripRenderMode.Professional;
		base.Renderer = new System.Windows.Forms.ToolStripProfessionalRenderer(new CDV_Viewer.Controls.CustomMenuStripColorTable());
		this.FileMenu = new CDV_Viewer.Menu.FileMenu();
		this.Items.Add(this.FileMenu);
		this.EditMenu = new CDV_Viewer.Menu.EditMenu();
		this.Items.Add(this.EditMenu);
		this.DisplayMenu = new CDV_Viewer.Menu.DisplayMenu();
		this.Items.Add(this.DisplayMenu);
		this.ToolMenu = new CDV_Viewer.Menu.ToolMenu();
		this.Items.Add(this.ToolMenu);
		this.TourneeMenu = new CDV_Viewer.Menu.TourneeMenu();
		this.Items.Add(this.TourneeMenu);
		this.HelpMenu = new CDV_Viewer.Menu.HelpMenu();
		this.Items.Add(this.HelpMenu);
	}

	private void SIGMenu_ItemAdded(object sender, ToolStripItemEventArgs e)
	{
		try
		{
			((ToolStripMenuItem)e.Item).DropDown.Paint += DropDown_Paint;
		}
		catch
		{
		}
	}

	private void DropDown_Paint(object sender, PaintEventArgs e)
	{
		Control control = (Control)sender;
		LinearGradientBrush brush = new LinearGradientBrush(control.DisplayRectangle, Color.FromArgb(244, 242, 236), Color.FromArgb(234, 232, 226), 90f);
		e.Graphics.FillRectangle(brush, control.DisplayRectangle);
	}

	protected override void OnPaint(PaintEventArgs e)
	{
		Rectangle rect = new Rectangle(0, 0, base.Width, base.Height);
		LinearGradientBrush brush = new LinearGradientBrush(rect, Colors.GetColor("FormTopMenu"), Colors.GetColor("FormMenu"), 90f);
		e.Graphics.FillRectangle(brush, rect);
		base.OnPaint(e);
	}
}
