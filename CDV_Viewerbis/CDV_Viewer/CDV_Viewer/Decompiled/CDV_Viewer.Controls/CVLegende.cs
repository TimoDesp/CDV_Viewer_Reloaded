using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.Windows.Forms;
using CDV_Viewer.Data;
using CDV_Viewer.DockControls;
using CDV_Viewer.Properties;
using CDV_Viewer.Styles;

namespace CDV_Viewer.Controls;

public class CVLegende : UserControl
{
	private static readonly Font _font = new Font(Global.DefaultFontName, 9f);

	private IContainer components;

	public CVLegende()
	{
		InitializeComponent();
		base.Enabled = false;
		base.Paint += SIGLegende_Paint;
	}

	protected override bool IsInputKey(Keys keyData)
	{
		if (keyData == Keys.Left || keyData == Keys.Right)
		{
			return true;
		}
		return base.IsInputKey(keyData);
	}

	public new void Refresh()
	{
		if (ComposantsViewer.Viewer.ModeVisualisation == ModeVisualisation.Topologie)
		{
			base.Width = 200;
		}
		else
		{
			base.Width = 400;
		}
		base.Visible = Preferences.Affichage.Legende;
		Invalidate();
	}

	private void Draw(Graphics g, Point point, string composant)
	{
		Font font = Global.DefaultFont;
		switch (composant)
		{
		case "Voie Principale":
		{
			Pen pen = new Pen(Colors.GetColor("Voie"), 2f);
			g.DrawLine(pen, point.X, point.Y + 6, point.X + 12, point.Y + 6);
			break;
		}
		case "Voie Secondaire":
		{
			Pen pen = new Pen(Colors.GetColor("Voie"), 2f);
			g.DrawLine(pen, point.X, point.Y + 9, point.X + 6, point.Y + 3);
			g.DrawLine(pen, point.X + 6, point.Y + 3, point.X + 12, point.Y + 3);
			break;
		}
		case "Jonction":
		{
			Pen pen = new Pen(Colors.GetColor("Jonction"), 2f);
			g.SmoothingMode = SmoothingMode.AntiAlias;
			g.DrawLine(pen, point.X, point.Y + 2, point.X + 12, point.Y + 10);
			g.SmoothingMode = SmoothingMode.Default;
			break;
		}
		case "Voie autre ligne":
		{
			Pen pen = new Pen(Colors.GetColor("Voie"), 2f);
			g.DrawLine(pen, point.X, point.Y + 10, point.X + 12, point.Y + 10);
			g.DrawLine(pen, point.X + 2, point.Y + 10, point.X + 10, point.Y + 2);
			break;
		}
		case "Noeud":
		{
			Brush brush = new SolidBrush(Colors.GetColor("Noeud"));
			g.SmoothingMode = SmoothingMode.AntiAlias;
			g.FillEllipse(brush, point.X + 2, point.Y + 2, 7, 7);
			g.SmoothingMode = SmoothingMode.Default;
			break;
		}
		case "Balise BLGV":
		{
			Pen pen = new Pen(Colors.GetColor("BaliseEnable"), 3f);
			g.DrawLine(pen, point.X, point.Y + 6, point.X + 12, point.Y + 6);
			break;
		}
		case "Balise CRO":
		{
			Pen pen = new Pen(Colors.GetColor("BaliseEnable"), 2f);
			g.SmoothingMode = SmoothingMode.AntiAlias;
			g.DrawLine(pen, point.X, point.Y + 8, point.X + 3, point.Y + 4);
			g.DrawLine(pen, point.X + 3, point.Y + 4, point.X + 6, point.Y + 8);
			g.DrawLine(pen, point.X + 6, point.Y + 8, point.X + 9, point.Y + 4);
			g.DrawLine(pen, point.X + 9, point.Y + 4, point.X + 12, point.Y + 8);
			g.SmoothingMode = SmoothingMode.Default;
			break;
		}
		case "Joint CC":
		{
			Pen pen = new Pen(Colors.GetColor("Joint"), 2f);
			g.DrawLine(pen, point.X + 3, point.Y + 2, point.X + 3, point.Y + 10);
			g.DrawLine(pen, point.X + 6, point.Y + 1, point.X + 6, point.Y + 11);
			g.DrawLine(pen, point.X + 9, point.Y + 2, point.X + 9, point.Y + 10);
			g.DrawLine(pen, point.X + 3, point.Y + 6, point.X + 9, point.Y + 6);
			break;
		}
		case "Joint SVAC":
		{
			Pen pen = new Pen(Colors.GetColor("Joint"), 2f);
			g.DrawLine(pen, point.X + 3, point.Y + 1, point.X + 3, point.Y + 11);
			g.DrawLine(pen, point.X + 9, point.Y + 1, point.X + 9, point.Y + 11);
			g.DrawLine(pen, point.X + 3, point.Y + 6, point.X + 9, point.Y + 6);
			break;
		}
		case "Joint JI":
		{
			Pen pen = new Pen(Colors.GetColor("Joint"), 2f);
			g.DrawLine(pen, point.X + 5, point.Y + 1, point.X + 5, point.Y + 11);
			g.DrawLine(pen, point.X + 7, point.Y + 1, point.X + 7, point.Y + 11);
			break;
		}
		case "CDV":
		{
			Pen pen = new Pen(Colors.GetColor("CDV0"), 2f);
			g.DrawLine(pen, point.X, point.Y + 6, point.X + 12, point.Y + 6);
			break;
		}
		case "CDV 3Hz":
		{
			Pen pen = new Pen(Colors.GetColor("CDV3"), 2f);
			g.DrawLine(pen, point.X, point.Y + 6, point.X + 12, point.Y + 6);
			break;
		}
		case "CDV 1700Hz":
		{
			Pen pen = new Pen(Colors.GetColor("CDV1700"), 2f);
			g.DrawLine(pen, point.X, point.Y + 6, point.X + 12, point.Y + 6);
			break;
		}
		case "CDV 2000Hz":
		{
			Pen pen = new Pen(Colors.GetColor("CDV2000"), 2f);
			g.DrawLine(pen, point.X, point.Y + 6, point.X + 12, point.Y + 6);
			break;
		}
		case "CDV 2300Hz":
		{
			Pen pen = new Pen(Colors.GetColor("CDV2300"), 2f);
			g.DrawLine(pen, point.X, point.Y + 6, point.X + 12, point.Y + 6);
			break;
		}
		case "CDV 2600Hz":
		{
			Pen pen = new Pen(Colors.GetColor("CDV2600"), 2f);
			g.DrawLine(pen, point.X, point.Y + 6, point.X + 12, point.Y + 6);
			break;
		}
		}
		g.TextRenderingHint = TextRenderingHint.AntiAliasGridFit;
		g.DrawString(composant, font, Brushes.Black, point.X + 16, point.Y);
	}

	private void SIGLegende_Paint(object sender, PaintEventArgs e)
	{
		e.Graphics.Clear(BackColor);
		e.Graphics.DrawImage(Resources.info, new Rectangle(4, 4, 16, 16));
		e.Graphics.DrawString("LÉGENDE :", _font, new SolidBrush(Color.FromArgb(224, 82, 6)), 22f, 5f);
		Draw(e.Graphics, new Point(6, 24), "Voie Principale");
		Draw(e.Graphics, new Point(6, 40), "Voie Secondaire");
		Draw(e.Graphics, new Point(6, 56), "Jonction");
		Draw(e.Graphics, new Point(6, 72), "Voie autre ligne");
		Draw(e.Graphics, new Point(106, 24), "Noeud");
		Draw(e.Graphics, new Point(106, 40), "Balise BLGV");
		Draw(e.Graphics, new Point(106, 56), "Balise CRO");
		ComposantsViewer viewer = ComposantsViewer.Viewer;
		if (viewer != null && viewer.ModeVisualisation == ModeVisualisation.Signalisation)
		{
			Draw(e.Graphics, new Point(206, 24), "Joint CC");
			Draw(e.Graphics, new Point(206, 40), "Joint SVAC");
			Draw(e.Graphics, new Point(206, 56), "Joint JI");
			Draw(e.Graphics, new Point(206, 72), "CDV");
			Draw(e.Graphics, new Point(306, 8), "CDV 3Hz");
			Draw(e.Graphics, new Point(306, 24), "CDV 1700Hz");
			Draw(e.Graphics, new Point(306, 40), "CDV 2000Hz");
			Draw(e.Graphics, new Point(306, 56), "CDV 2300Hz");
			Draw(e.Graphics, new Point(306, 72), "CDV 2600Hz");
		}
	}

	protected override void Dispose(bool disposing)
	{
		if (disposing && components != null)
		{
			components.Dispose();
		}
		base.Dispose(disposing);
	}

	private void InitializeComponent()
	{
		base.SuspendLayout();
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
		this.DoubleBuffered = true;
		base.Name = "CVLegende";
		base.Size = new System.Drawing.Size(228, 78);
		base.ResumeLayout(false);
	}
}
