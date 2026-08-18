using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Text;
using System.Windows.Forms;
using CDV_Viewer.Composants;
using CDV_Viewer.Controls;
using CDV_Viewer.CustomControls;
using CDV_Viewer.Data;
using CDV_Viewer.DockControls;
using CDV_Viewer.Properties;
using CDV_Viewer.Traitements;

namespace CDV_Viewer.PopupForms;

public class ExportPopupForm : PopupForm
{
	private CheckBox cbNoeuds;

	private CheckBox cbBalises;

	private CheckBox cbCdVs;

	private Label label3;

	private CheckBox cbJoints;

	private Label label2;

	private Label label4;

	private PictureBox pbImage;

	private CustomTextBox tbPkD;

	private CustomTextBox tbPkF;

	private Label label1;

	public ExportPopupForm()
	{
		InitializeComponent();
		tbPkD.Text = ComposantsViewer.Viewer.PkD.ToString();
		tbPkF.Text = ComposantsViewer.Viewer.PkF.ToString();
	}

	private void InitializeComponent()
	{
		this.cbNoeuds = new System.Windows.Forms.CheckBox();
		this.cbBalises = new System.Windows.Forms.CheckBox();
		this.cbCdVs = new System.Windows.Forms.CheckBox();
		this.label3 = new System.Windows.Forms.Label();
		this.cbJoints = new System.Windows.Forms.CheckBox();
		this.label2 = new System.Windows.Forms.Label();
		this.label1 = new System.Windows.Forms.Label();
		this.label4 = new System.Windows.Forms.Label();
		this.pbImage = new System.Windows.Forms.PictureBox();
		this.tbPkD = new CDV_Viewer.CustomControls.CustomTextBox();
		this.tbPkF = new CDV_Viewer.CustomControls.CustomTextBox();
		((System.ComponentModel.ISupportInitialize)this.pbImage).BeginInit();
		base.SuspendLayout();
		this.cbNoeuds.AutoSize = true;
		this.cbNoeuds.Checked = true;
		this.cbNoeuds.CheckState = System.Windows.Forms.CheckState.Checked;
		this.cbNoeuds.Location = new System.Drawing.Point(10, 59);
		this.cbNoeuds.Name = "cbNoeuds";
		this.cbNoeuds.Size = new System.Drawing.Size(63, 17);
		this.cbNoeuds.TabIndex = 18;
		this.cbNoeuds.Text = "Noeuds";
		this.cbNoeuds.UseVisualStyleBackColor = true;
		this.cbBalises.AutoSize = true;
		this.cbBalises.Checked = true;
		this.cbBalises.CheckState = System.Windows.Forms.CheckState.Checked;
		this.cbBalises.Location = new System.Drawing.Point(80, 59);
		this.cbBalises.Name = "cbBalises";
		this.cbBalises.Size = new System.Drawing.Size(59, 17);
		this.cbBalises.TabIndex = 17;
		this.cbBalises.Text = "Balises";
		this.cbBalises.UseVisualStyleBackColor = true;
		this.cbCdVs.AutoSize = true;
		this.cbCdVs.Checked = true;
		this.cbCdVs.CheckState = System.Windows.Forms.CheckState.Checked;
		this.cbCdVs.Location = new System.Drawing.Point(202, 59);
		this.cbCdVs.Name = "cbCdVs";
		this.cbCdVs.Size = new System.Drawing.Size(98, 17);
		this.cbCdVs.TabIndex = 16;
		this.cbCdVs.Text = "Circuits de voie";
		this.cbCdVs.UseVisualStyleBackColor = true;
		this.label3.AutoSize = true;
		this.label3.Location = new System.Drawing.Point(3, 40);
		this.label3.Name = "label3";
		this.label3.Size = new System.Drawing.Size(49, 13);
		this.label3.TabIndex = 15;
		this.label3.Text = "Afficher :";
		this.cbJoints.AutoSize = true;
		this.cbJoints.Checked = true;
		this.cbJoints.CheckState = System.Windows.Forms.CheckState.Checked;
		this.cbJoints.Location = new System.Drawing.Point(143, 59);
		this.cbJoints.Name = "cbJoints";
		this.cbJoints.Size = new System.Drawing.Size(53, 17);
		this.cbJoints.TabIndex = 14;
		this.cbJoints.Text = "Joints";
		this.cbJoints.UseVisualStyleBackColor = true;
		this.label2.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
		this.label2.AutoSize = true;
		this.label2.Location = new System.Drawing.Point(213, 12);
		this.label2.Name = "label2";
		this.label2.Size = new System.Drawing.Size(44, 13);
		this.label2.TabIndex = 11;
		this.label2.Text = "PK Fin :";
		this.label1.AutoSize = true;
		this.label1.Location = new System.Drawing.Point(12, 12);
		this.label1.Name = "label1";
		this.label1.Size = new System.Drawing.Size(59, 13);
		this.label1.TabIndex = 10;
		this.label1.Text = "PK Début :";
		this.label4.AutoSize = true;
		this.label4.Location = new System.Drawing.Point(7, 124);
		this.label4.Name = "label4";
		this.label4.Size = new System.Drawing.Size(86, 13);
		this.label4.TabIndex = 19;
		this.label4.Text = "Prévisualisation :";
		this.pbImage.BackColor = System.Drawing.Color.White;
		this.pbImage.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
		this.pbImage.Location = new System.Drawing.Point(5, 141);
		this.pbImage.Name = "pbImage";
		this.pbImage.Size = new System.Drawing.Size(340, 125);
		this.pbImage.TabIndex = 20;
		this.pbImage.TabStop = false;
		this.tbPkD.BackColor = System.Drawing.Color.White;
		this.tbPkD.Location = new System.Drawing.Point(77, 9);
		this.tbPkD.Name = "tbPkD";
		this.tbPkD.Padding = new System.Windows.Forms.Padding(1);
		this.tbPkD.SelectionLength = 0;
		this.tbPkD.SelectionStart = 0;
		this.tbPkD.Size = new System.Drawing.Size(60, 20);
		this.tbPkD.TabIndex = 21;
		this.tbPkD.TextAlign = System.Windows.Forms.HorizontalAlignment.Left;
		this.tbPkF.BackColor = System.Drawing.Color.White;
		this.tbPkF.Location = new System.Drawing.Point(260, 9);
		this.tbPkF.Name = "tbPkF";
		this.tbPkF.Padding = new System.Windows.Forms.Padding(1);
		this.tbPkF.SelectionLength = 0;
		this.tbPkF.SelectionStart = 0;
		this.tbPkF.Size = new System.Drawing.Size(60, 20);
		this.tbPkF.TabIndex = 22;
		this.tbPkF.TextAlign = System.Windows.Forms.HorizontalAlignment.Left;
		this.BackColor = System.Drawing.Color.White;
		base.Controls.Add(this.tbPkF);
		base.Controls.Add(this.tbPkD);
		base.Controls.Add(this.pbImage);
		base.Controls.Add(this.label4);
		base.Controls.Add(this.cbNoeuds);
		base.Controls.Add(this.cbBalises);
		base.Controls.Add(this.cbCdVs);
		base.Controls.Add(this.label3);
		base.Controls.Add(this.cbJoints);
		base.Controls.Add(this.label2);
		base.Controls.Add(this.label1);
		base.Name = "ExportPopupForm";
		base.Size = new System.Drawing.Size(350, 272);
		((System.ComponentModel.ISupportInitialize)this.pbImage).EndInit();
		base.ResumeLayout(false);
		base.PerformLayout();
	}

	protected override void OnClosing(PopupFormResultEventArgs e)
	{
		PaintEventArgs paintEventArgs;
		if (e.Result == PopupContainerResult.OK && tryGetPk(tbPkD.Text, out var value, "PK début incorrect") && tryGetPk(tbPkF.Text, out var value2, "PK fin incorrect"))
		{
			ComposantsViewer.SaveLimits();
			int num = Math.Abs(value2 - value) / 5 + 40;
			int num2 = (ComposantsViewer.PosVoieF - ComposantsViewer.PosVoieD) * 40 + 160;
			ComposantsViewer.SetLimits(num - 40, num2 - 160, ComposantsViewer.GraphOffsetX, ComposantsViewer.GraphOffsetY, value, value2, ComposantsViewer.PosVoieD, ComposantsViewer.PosVoieF);
			Bitmap bitmap = new Bitmap(num, num2);
			Graphics graphics = Graphics.FromImage(bitmap);
			Rectangle rectangle = new Rectangle(Point.Empty, bitmap.Size);
			graphics.TextRenderingHint = TextRenderingHint.AntiAliasGridFit;
			graphics.FillRectangle(Brushes.WhiteSmoke, rectangle);
			paintEventArgs = new PaintEventArgs(graphics, rectangle);
			Composants.ReverseForEach(delegate(Composant _composant)
			{
				drawcheckedComposant(_composant);
			});
			Font font = new Font("Arial", 20f);
			string s = $"Ligne {ComposantsViewer.LigneId} - PK {Chaines.PkToString(value)} à {Chaines.PkToString(value2)}";
			graphics.DrawString(s, font, new SolidBrush(Color.FromArgb(224, 82, 6)), 20f, 25f);
			new Bitmap(10, 10);
			ComposantsViewer.Legende.DrawToBitmap(bitmap, new Rectangle(20, num2 - 120, ComposantsViewer.Legende.Width, ComposantsViewer.Legende.Height));
			graphics.DrawImage(Resources.sncf, new Rectangle(num - 105, 20, 85, 46));
			graphics.Dispose();
			ComposantsViewer.RestoreLimits();
			SaveFileDialog saveFileDialog = new SaveFileDialog
			{
				Filter = "Image PNG (*.png)|*.png|Image JPEG (*.jpg)|*.jpg"
			};
			if (saveFileDialog.ShowDialog() == DialogResult.OK)
			{
				bitmap.Save(saveFileDialog.FileName);
				Process.Start(saveFileDialog.FileName);
			}
			base.OnClosing(e);
		}
		void drawcheckedComposant(Composant composant)
		{
			if (composant.IsInGraph() && (cbNoeuds.Checked || !(composant is CNoeud)) && (cbJoints.Checked || !(composant is CJoint)) && (cbCdVs.Checked || !(composant is CCircuit)) && (cbBalises.Checked || !(composant is CBalise)))
			{
				composant.Paint(paintEventArgs);
			}
		}
		bool tryGetPk(string textValue, out int result, string errorMessage)
		{
			if (!int.TryParse(textValue, out result))
			{
				MessageBox.Show(errorMessage, Resources.APP_NAME);
				e.Canceled = true;
				return false;
			}
			return true;
		}
	}
}
