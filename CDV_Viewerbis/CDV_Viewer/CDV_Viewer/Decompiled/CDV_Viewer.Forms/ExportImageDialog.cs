using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Text;
using System.IO;
using System.Windows.Forms;
using CDV_Viewer.Composants;
using CDV_Viewer.Controls;
using CDV_Viewer.Data;
using CDV_Viewer.DockControls;
using CDV_Viewer.Properties;
using CDV_Viewer.Traitements;
using PdfSharp;
using PdfSharp.Drawing;
using PdfSharp.Pdf;

namespace CDV_Viewer.Forms;

public class ExportImageDialog : Form
{
	private readonly ComposantsViewer ComposantsViewer = ComposantsViewer.Viewer;

	private int _idLigne;

	private ComposantsCollection _composants;

	private IContainer components;

	private Label label1;

	private Panel panel1;

	private PKBox pbPkD;

	private Label label2;

	private Button bValider;

	private PKBox pbPkF;

	private Button bAnnuler;

	private CheckBox cbNoeuds;

	private CheckBox cbBalises;

	private CheckBox cbCdVs;

	private Label label3;

	private CheckBox cbJoints;

	public ExportImageDialog(int idLigne, ComposantsCollection composants)
	{
		InitializeComponent();
		base.Icon = Icon.FromHandle(Resources.image.GetHicon());
		_idLigne = idLigne;
		_composants = composants;
		bValider.Click += bValider_Click;
		bAnnuler.Click += bAnnuler_Click;
	}

	private Bitmap GetImage(int ligne, int pkD, int pkF)
	{
		Global.MainForm.Enabled = false;
		ComposantsViewer.SaveLimits();
		int num = Math.Abs(pkF - pkD) / 5 + 40;
		int num2 = (ComposantsViewer.PosVoieF - ComposantsViewer.PosVoieD) * 40 + 160;
		ComposantsViewer.SetLimits(num - 40, num2 - 160, ComposantsViewer.GraphOffsetX, ComposantsViewer.GraphOffsetY, pkD, pkF, ComposantsViewer.PosVoieD, ComposantsViewer.PosVoieF);
		num = Math.Max(num, 600);
		Bitmap bitmap = new Bitmap(num, num2);
		Graphics graphics = Graphics.FromImage(bitmap);
		graphics.TextRenderingHint = TextRenderingHint.AntiAliasGridFit;
		graphics.TranslateTransform(20f, 60f);
		foreach (Composant composant in _composants)
		{
			if (composant.IsInGraph() && (cbNoeuds.Checked || !(composant is CNoeud)) && (cbJoints.Checked || !(composant is CJoint)) && (cbCdVs.Checked || !(composant is CCircuit)) && (cbBalises.Checked || !(composant is CBalise)))
			{
				composant.Paint(new PaintEventArgs(graphics, new Rectangle(Point.Empty, bitmap.Size)));
			}
		}
		graphics.TranslateTransform(0f, -60f);
		DrawEchelle(graphics, num, num2, pkD, pkF);
		graphics.TranslateTransform(-20f, 0f);
		Font font = new Font("Arial", 20f, FontStyle.Bold);
		string s = "Ligne " + _idLigne + " - Pk " + Chaines.PkToString(pkD) + " à " + Chaines.PkToString(pkF);
		graphics.DrawString(s, font, Brushes.Black, 20f, 25f);
		new Bitmap(10, 10);
		ComposantsViewer.Legende.DrawToBitmap(bitmap, new Rectangle(20, num2 - 120, ComposantsViewer.Legende.Width, ComposantsViewer.Legende.Height));
		graphics.DrawImage(Resources.sncf, new Rectangle(num - 105, 20, 85, 46));
		graphics.Dispose();
		ComposantsViewer.RestoreLimits();
		Global.MainForm.Enabled = true;
		return bitmap;
	}

	private void DrawEchelle(Graphics g, int width, int height, int pkD, int pkF)
	{
		Font font = Global.DefaultFont;
		Font font2 = new Font(font, FontStyle.Bold);
		StringFormat stringFormat = new StringFormat();
		stringFormat.Alignment = StringAlignment.Center;
		for (int i = 0; i < pkF - pkD; i += 50)
		{
			g.DrawLine(Pens.Black, i / 5, height - 5, i / 5, height);
			if (i % 250 == 0)
			{
				g.DrawLine(Pens.Black, i / 5, height - 8, i / 5, height);
				if (i % 1000 == 0)
				{
					g.DrawString(Chaines.PkToString(i + pkD), font2, Brushes.Black, new Rectangle(i / 5 - 20, height - 20, 40, 12), stringFormat);
				}
				else
				{
					g.DrawString(Chaines.PkToString(i + pkD).ToString(), font, Brushes.Black, new Rectangle(i / 5 - 20, height - 20, 40, 12), stringFormat);
				}
			}
		}
	}

	private void bValider_Click(object sender, EventArgs e)
	{
		if (!pbPkD.CorrectPK)
		{
			MessageBox.Show("PK Début inccorect", Resources.APP_NAME);
			return;
		}
		if (!pbPkF.CorrectPK)
		{
			MessageBox.Show("PK Fin inccorect", Resources.APP_NAME);
			return;
		}
		SaveFileDialog saveFileDialog = new SaveFileDialog();
		saveFileDialog.Filter = "Image PNG (*.png)|*.png|Image JPG (*.jpg)|*.jpg|Document PDF (*.pdf)|*.pdf|Tous les fichiers (*.*)|*.*";
		if (saveFileDialog.ShowDialog() != DialogResult.OK)
		{
			return;
		}
		Bitmap image = GetImage(_idLigne, pbPkD.GetPk(), pbPkF.GetPk());
		if (Path.GetExtension(saveFileDialog.FileName).ToLower() == ".pdf")
		{
			PdfDocument pdfDocument = new PdfDocument();
			for (int i = 0; i < image.Width; i += 1000)
			{
				PdfPage pdfPage = pdfDocument.Pages.Add();
				pdfPage.Orientation = PageOrientation.Landscape;
				XGraphics xGraphics = XGraphics.FromPdfPage(pdfPage);
				xGraphics.DrawImage(image, -i, 0);
				xGraphics.Dispose();
			}
			pdfDocument.Save(saveFileDialog.FileName);
		}
		else
		{
			image.Save(saveFileDialog.FileName);
		}
		MessageBox.Show("Export terminé !", Resources.APP_NAME);
		Close();
	}

	private void bAnnuler_Click(object sender, EventArgs e)
	{
		Close();
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
		this.label1 = new System.Windows.Forms.Label();
		this.panel1 = new System.Windows.Forms.Panel();
		this.cbNoeuds = new System.Windows.Forms.CheckBox();
		this.cbBalises = new System.Windows.Forms.CheckBox();
		this.cbCdVs = new System.Windows.Forms.CheckBox();
		this.label3 = new System.Windows.Forms.Label();
		this.cbJoints = new System.Windows.Forms.CheckBox();
		this.pbPkF = new CDV_Viewer.Controls.PKBox();
		this.pbPkD = new CDV_Viewer.Controls.PKBox();
		this.label2 = new System.Windows.Forms.Label();
		this.bValider = new System.Windows.Forms.Button();
		this.bAnnuler = new System.Windows.Forms.Button();
		this.panel1.SuspendLayout();
		base.SuspendLayout();
		this.label1.AutoSize = true;
		this.label1.Location = new System.Drawing.Point(12, 14);
		this.label1.Name = "label1";
		this.label1.Size = new System.Drawing.Size(59, 13);
		this.label1.TabIndex = 0;
		this.label1.Text = "PK Début :";
		this.panel1.BackColor = System.Drawing.SystemColors.Control;
		this.panel1.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
		this.panel1.Controls.Add(this.cbNoeuds);
		this.panel1.Controls.Add(this.cbBalises);
		this.panel1.Controls.Add(this.cbCdVs);
		this.panel1.Controls.Add(this.label3);
		this.panel1.Controls.Add(this.cbJoints);
		this.panel1.Controls.Add(this.pbPkF);
		this.panel1.Controls.Add(this.pbPkD);
		this.panel1.Controls.Add(this.label2);
		this.panel1.Controls.Add(this.label1);
		this.panel1.Location = new System.Drawing.Point(12, 12);
		this.panel1.Name = "panel1";
		this.panel1.Size = new System.Drawing.Size(177, 205);
		this.panel1.TabIndex = 1;
		this.cbNoeuds.AutoSize = true;
		this.cbNoeuds.Checked = true;
		this.cbNoeuds.CheckState = System.Windows.Forms.CheckState.Checked;
		this.cbNoeuds.Location = new System.Drawing.Point(24, 103);
		this.cbNoeuds.Name = "cbNoeuds";
		this.cbNoeuds.Size = new System.Drawing.Size(63, 17);
		this.cbNoeuds.TabIndex = 9;
		this.cbNoeuds.Text = "Noeuds";
		this.cbNoeuds.UseVisualStyleBackColor = true;
		this.cbBalises.AutoSize = true;
		this.cbBalises.Checked = true;
		this.cbBalises.CheckState = System.Windows.Forms.CheckState.Checked;
		this.cbBalises.Location = new System.Drawing.Point(24, 122);
		this.cbBalises.Name = "cbBalises";
		this.cbBalises.Size = new System.Drawing.Size(59, 17);
		this.cbBalises.TabIndex = 8;
		this.cbBalises.Text = "Balises";
		this.cbBalises.UseVisualStyleBackColor = true;
		this.cbCdVs.AutoSize = true;
		this.cbCdVs.Checked = true;
		this.cbCdVs.CheckState = System.Windows.Forms.CheckState.Checked;
		this.cbCdVs.Location = new System.Drawing.Point(24, 162);
		this.cbCdVs.Name = "cbCdVs";
		this.cbCdVs.Size = new System.Drawing.Size(98, 17);
		this.cbCdVs.TabIndex = 7;
		this.cbCdVs.Text = "Circuits de voie";
		this.cbCdVs.UseVisualStyleBackColor = true;
		this.label3.AutoSize = true;
		this.label3.Location = new System.Drawing.Point(12, 80);
		this.label3.Name = "label3";
		this.label3.Size = new System.Drawing.Size(49, 13);
		this.label3.TabIndex = 6;
		this.label3.Text = "Afficher :";
		this.cbJoints.AutoSize = true;
		this.cbJoints.Checked = true;
		this.cbJoints.CheckState = System.Windows.Forms.CheckState.Checked;
		this.cbJoints.Location = new System.Drawing.Point(24, 142);
		this.cbJoints.Name = "cbJoints";
		this.cbJoints.Size = new System.Drawing.Size(53, 17);
		this.cbJoints.TabIndex = 5;
		this.cbJoints.Text = "Joints";
		this.cbJoints.UseVisualStyleBackColor = true;
		this.pbPkF.BackColor = System.Drawing.Color.White;
		this.pbPkF.Location = new System.Drawing.Point(77, 40);
		this.pbPkF.Name = "pbPkF";
		this.pbPkF.Padding = new System.Windows.Forms.Padding(1);
		this.pbPkF.SelectionLength = 0;
		this.pbPkF.SelectionStart = 0;
		this.pbPkF.Size = new System.Drawing.Size(67, 24);
		this.pbPkF.TabIndex = 4;
		this.pbPkF.TextAlign = System.Windows.Forms.HorizontalAlignment.Left;
		this.pbPkD.BackColor = System.Drawing.Color.White;
		this.pbPkD.Location = new System.Drawing.Point(77, 10);
		this.pbPkD.Name = "pbPkD";
		this.pbPkD.Padding = new System.Windows.Forms.Padding(1);
		this.pbPkD.SelectionLength = 0;
		this.pbPkD.SelectionStart = 0;
		this.pbPkD.Size = new System.Drawing.Size(67, 24);
		this.pbPkD.TabIndex = 3;
		this.pbPkD.TextAlign = System.Windows.Forms.HorizontalAlignment.Left;
		this.label2.AutoSize = true;
		this.label2.Location = new System.Drawing.Point(12, 40);
		this.label2.Name = "label2";
		this.label2.Size = new System.Drawing.Size(44, 13);
		this.label2.TabIndex = 2;
		this.label2.Text = "PK Fin :";
		this.bValider.Location = new System.Drawing.Point(33, 223);
		this.bValider.Name = "bValider";
		this.bValider.Size = new System.Drawing.Size(75, 23);
		this.bValider.TabIndex = 2;
		this.bValider.Text = "Valider";
		this.bValider.UseVisualStyleBackColor = true;
		this.bAnnuler.Location = new System.Drawing.Point(114, 223);
		this.bAnnuler.Name = "bAnnuler";
		this.bAnnuler.Size = new System.Drawing.Size(75, 23);
		this.bAnnuler.TabIndex = 3;
		this.bAnnuler.Text = "Annuler";
		this.bAnnuler.UseVisualStyleBackColor = true;
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.ClientSize = new System.Drawing.Size(199, 258);
		base.Controls.Add(this.bAnnuler);
		base.Controls.Add(this.bValider);
		base.Controls.Add(this.panel1);
		base.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Fixed3D;
		base.MaximizeBox = false;
		base.MinimizeBox = false;
		base.Name = "ExportImageDialog";
		base.ShowInTaskbar = false;
		base.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
		this.Text = "Exporter...";
		this.panel1.ResumeLayout(false);
		this.panel1.PerformLayout();
		base.ResumeLayout(false);
	}
}
