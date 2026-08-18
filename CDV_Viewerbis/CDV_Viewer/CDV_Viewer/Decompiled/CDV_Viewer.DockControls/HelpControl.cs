using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using CDV_Viewer.Controls;
using CDV_Viewer.Data;

namespace CDV_Viewer.DockControls;

public class HelpControl : DockChild
{
	private IContainer components;

	private WebBrowser webBrowser;

	private CloseButton bFermer;

	private Separator separator1;

	private Label label4;

	private SimpleListView lvPages;

	private SplitContainer splitContainer1;

	public HelpControl()
	{
		InitializeComponent();
		MinimumSize = new Size(160, 100);
		HelpXml helpXml = null;
		try
		{
			helpXml = HelpXml.Load();
		}
		catch
		{
		}
		if (helpXml != null)
		{
			foreach (HelpPage page in helpXml.Pages)
			{
				lvPages.Items.Add(new SimpleListViewItem(page.Path, page.Nom));
			}
		}
		splitContainer1.SplitterDistance = lvPages.Items.Count * lvPages.ItemSize;
		bFermer.Click += bFermer_Click;
		lvPages.SelectedIndexChanged += lvPages_SelectedIndexChanged;
	}

	private void bFermer_Click(object sender, EventArgs e)
	{
		base.Visible = false;
	}

	private void lvPages_SelectedIndexChanged(object sender, EventArgs e)
	{
		if (lvPages.SelectedItem != null)
		{
			webBrowser.Url = new Uri(Paths.HelpFolder + "\\" + lvPages.SelectedItem.Tag);
			splitContainer1.Panel2Collapsed = false;
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
		this.splitContainer1 = new System.Windows.Forms.SplitContainer();
		this.lvPages = new CDV_Viewer.Controls.SimpleListView(CDV_Viewer.Controls.ScrollBarOrientation.Vertical);
		this.webBrowser = new System.Windows.Forms.WebBrowser();
		this.separator1 = new CDV_Viewer.Controls.Separator();
		this.label4 = new System.Windows.Forms.Label();
		this.bFermer = new CDV_Viewer.Controls.CloseButton();
		this.splitContainer1.Panel1.SuspendLayout();
		this.splitContainer1.Panel2.SuspendLayout();
		this.splitContainer1.SuspendLayout();
		base.SuspendLayout();
		this.splitContainer1.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.splitContainer1.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
		this.splitContainer1.Location = new System.Drawing.Point(0, 34);
		this.splitContainer1.Name = "splitContainer1";
		this.splitContainer1.Orientation = System.Windows.Forms.Orientation.Horizontal;
		this.splitContainer1.Panel1.Controls.Add(this.lvPages);
		this.splitContainer1.Panel2.Controls.Add(this.webBrowser);
		this.splitContainer1.Size = new System.Drawing.Size(383, 300);
		this.splitContainer1.SplitterDistance = 95;
		this.splitContainer1.TabIndex = 18;
		this.lvPages.Cursor = System.Windows.Forms.Cursors.Hand;
		this.lvPages.Dock = System.Windows.Forms.DockStyle.Fill;
		this.lvPages.HoverColor = System.Drawing.Color.FromArgb(230, 230, 230);
		this.lvPages.ItemSize = 20;
		this.lvPages.Location = new System.Drawing.Point(0, 0);
		this.lvPages.Name = "lvPages";
		this.lvPages.SelectedColor = System.Drawing.Color.Gainsboro;
		this.lvPages.SelectedIndex = -1;
		this.lvPages.Size = new System.Drawing.Size(383, 95);
		this.lvPages.TabIndex = 17;
		this.lvPages.Text = "simpleListView1";
		this.webBrowser.Dock = System.Windows.Forms.DockStyle.Fill;
		this.webBrowser.Location = new System.Drawing.Point(0, 0);
		this.webBrowser.MinimumSize = new System.Drawing.Size(20, 20);
		this.webBrowser.Name = "webBrowser";
		this.webBrowser.Size = new System.Drawing.Size(383, 201);
		this.webBrowser.TabIndex = 1;
		this.separator1.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.separator1.ForeColor = System.Drawing.Color.Gainsboro;
		this.separator1.Location = new System.Drawing.Point(10, 25);
		this.separator1.Name = "separator1";
		this.separator1.Size = new System.Drawing.Size(363, 3);
		this.separator1.TabIndex = 16;
		this.separator1.Text = "separator1";
		this.label4.AutoSize = true;
		this.label4.Font = new System.Drawing.Font("Arial", 9f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		this.label4.ForeColor = System.Drawing.Color.FromArgb(224, 82, 6);
		this.label4.Location = new System.Drawing.Point(6, 6);
		this.label4.Name = "label4";
		this.label4.Size = new System.Drawing.Size(34, 15);
		this.label4.TabIndex = 15;
		this.label4.Text = "AIDE";
		this.bFermer.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
		this.bFermer.Cursor = System.Windows.Forms.Cursors.Hand;
		this.bFermer.Location = new System.Drawing.Point(365, 4);
		this.bFermer.Name = "bFermer";
		this.bFermer.Size = new System.Drawing.Size(15, 15);
		this.bFermer.TabIndex = 4;
		this.bFermer.Text = "closeButton1";
		this.bFermer.UseVisualStyleBackColor = true;
		this.BackColor = System.Drawing.Color.White;
		base.Controls.Add(this.splitContainer1);
		base.Controls.Add(this.separator1);
		base.Controls.Add(this.label4);
		base.Controls.Add(this.bFermer);
		base.Name = "HelpControl";
		base.Size = new System.Drawing.Size(383, 320);
		this.splitContainer1.Panel1.ResumeLayout(false);
		this.splitContainer1.Panel2.ResumeLayout(false);
		this.splitContainer1.ResumeLayout(false);
		base.ResumeLayout(false);
		base.PerformLayout();
	}
}
