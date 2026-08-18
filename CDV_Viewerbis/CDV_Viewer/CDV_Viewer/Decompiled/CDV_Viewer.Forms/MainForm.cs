using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using CDV_Viewer.Controls;
using CDV_Viewer.CsvBase;
using CDV_Viewer.Data;
using CDV_Viewer.DockControls;
using CDV_Viewer.Properties;
using CDV_Viewer.Styles;
using CDV_Viewer.Traitements;

namespace CDV_Viewer.Forms;

public class MainForm : Form
{
	private readonly ComposantsViewer ComposantsViewer;

	private Color _topGradientColor;

	private Color _bottomGradientColor;

	private IContainer components;

	private MainMenuStrip menuStrip;

	private MainToolStrip toolStrip;

	private DockContainer dockContainer;

	public void Invoke(Action method)
	{
		Invoke((Delegate)method);
	}

	public string Invoke(Func<string> method)
	{
		return (string)Invoke((Delegate)method);
	}

	public MainForm()
	{
		Global.MainForm = this;
		UltraPOTReceiver.Accept(base.Handle);
		ComposantsViewer = new ComposantsViewer
		{
			DockPosition = DockStyle.Fill,
			BackColor = Color.Transparent
		};
		Global.LiveControl = new LiveControl
		{
			DockPosition = DockStyle.Top,
			Visible = false
		};
		Global.ListeLignes = new ListeLignes
		{
			DockPosition = DockStyle.Left,
			Visible = false
		};
		Global.HelpControl = new HelpControl
		{
			DockPosition = DockStyle.Right,
			Visible = false
		};
		Global.ModeleViewer = new CVModele
		{
			DockPosition = DockStyle.Bottom,
			Visible = false
		};
		Global.ParcoursControl = new TourneeViewer
		{
			DockPosition = DockStyle.Bottom,
			Visible = false
		};
		InitializeComponent();
		BackColor = Colors.GetColor("FormBack");
		_topGradientColor = Colors.GetColor("FormBackTop");
		_bottomGradientColor = Colors.GetColor("FormBackBottom");
		dockContainer.DockChilds.Add(Global.LiveControl);
		dockContainer.DockChilds.Add(Global.ParcoursControl);
		dockContainer.DockChilds.Add(Global.ListeLignes);
		dockContainer.DockChilds.Add(Global.HelpControl);
		dockContainer.DockChilds.Add(Global.ModeleViewer);
		dockContainer.DockChilds.Add(ComposantsViewer);
		InitializeEvents();
	}

	private void InitializeEvents()
	{
		Base.SaveStateChanged += delegate
		{
			RefreshArchiveInfo();
		};
		Archives.CurrentArchiveChanged += delegate
		{
			RefreshArchiveInfo();
		};
		Global.ModeEditionChanged += delegate
		{
			Global_ModeEditionChanged();
		};
		ComposantsViewer.StateChanged += delegate
		{
			MainFormToolStripRefresh();
		};
		base.Shown += delegate
		{
			FormShown();
		};
		base.Load += delegate
		{
			CommandStart.Refresh();
		};
	}

	private void FormShown()
	{
		if (!Autorisations.Values.Edition)
		{
			Archive lastArchive = Archives.GetLastArchive();
			if (lastArchive == null)
			{
				MessageBox.Show("Aucune donnée présente");
				return;
			}
			if (!lastArchive.Load())
			{
				MessageBox.Show("Impossible de charger la base de donnée");
				return;
			}
		}
		MainFormToolStripRefresh();
	}

	private void MainFormToolStripRefresh()
	{
		toolStrip.RefreshItems();
	}

	protected override void OnFormClosing(FormClosingEventArgs e)
	{
		if (Archives.CurrentArchive != null && !Base.IsSave)
		{
			switch (MessageBox.Show("Voulez-vous sauvegarder l'archive avant de quitter ?", Resources.APP_NAME, MessageBoxButtons.YesNoCancel))
			{
			case DialogResult.Yes:
				ArchiveFonctions.Enregistrer();
				ArchiveFonctions.Fermer();
				break;
			case DialogResult.No:
				ArchiveFonctions.Fermer();
				break;
			default:
				e.Cancel = true;
				break;
			}
		}
		if (e.Cancel)
		{
			Preferences.Affichage.InfoBulles = ComposantsViewer.Viewer.ShowInfobulle;
			Preferences.Affichage.Legende = ComposantsViewer.Viewer.Legende.Visible;
			Preferences.Affichage.ModeVisualisation = ComposantsViewer.Viewer.ModeVisualisation;
		}
	}

	protected override void OnPaintBackground(PaintEventArgs e)
	{
		if (base.Width > 0 && base.Height > 0)
		{
			Rectangle rect = new Rectangle(0, 0, base.Width, base.Height);
			LinearGradientBrush brush = new LinearGradientBrush(rect, _topGradientColor, _bottomGradientColor, 90f);
			e.Graphics.FillRectangle(brush, rect);
		}
	}

	protected override void WndProc(ref Message m)
	{
		base.WndProc(ref m);
		UltraPOTReceiver.OnReceiveMessage(m);
	}

	private void Global_ModeEditionChanged()
	{
		MainFormToolStripRefresh();
		RefreshArchiveInfo();
		_topGradientColor = Colors.FormBackTop(Global.ModeEdition);
		_bottomGradientColor = Colors.FormBackBottom(Global.ModeEdition);
		Invalidate();
	}

	private void RefreshArchiveInfo()
	{
		if (!base.Enabled)
		{
			return;
		}
		Text = "CDV Viewer";
		if (Archives.CurrentArchive != null)
		{
			Text += $" : Archive du {Archives.CurrentArchive.Date:dd/MM/yyyy} ";
			if (Global.ModeEdition)
			{
				Text += $"( {Archives.CurrentArchive.ShortName} )";
			}
			if (Archives.CurrentArchive.Description != "")
			{
				Text += $"( {Archives.CurrentArchive.Description} )";
			}
			if (!Base.IsSave)
			{
				Text += " *";
			}
			MainFormToolStripRefresh();
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
		System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(CDV_Viewer.Forms.MainForm));
		this.dockContainer = new CDV_Viewer.Controls.DockContainer();
		this.menuStrip = new CDV_Viewer.Controls.MainMenuStrip();
		this.toolStrip = new CDV_Viewer.Controls.MainToolStrip();
		base.SuspendLayout();
		this.menuStrip.Dock = System.Windows.Forms.DockStyle.Top;
		this.menuStrip.Name = "menuStrip";
		this.toolStrip.Dock = System.Windows.Forms.DockStyle.Top;
		this.toolStrip.Name = "toolStrip";
		this.dockContainer.Dock = System.Windows.Forms.DockStyle.Fill;
		this.dockContainer.Location = new System.Drawing.Point(0, 49);
		this.dockContainer.Name = "dockContainer";
		this.dockContainer.Size = new System.Drawing.Size(1084, 513);
		this.dockContainer.TabIndex = 5;
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		this.BackColor = System.Drawing.Color.FromArgb(240, 237, 232);
		base.ClientSize = new System.Drawing.Size(1084, 562);
		base.Controls.Add(this.dockContainer);
		base.Controls.Add(this.toolStrip);
		base.Controls.Add(this.menuStrip);
		this.DoubleBuffered = true;
		base.Icon = (System.Drawing.Icon)resources.GetObject("$this.Icon");
		this.MinimumSize = new System.Drawing.Size(800, 600);
		base.Name = "MainForm";
		this.Text = "CDV Viewer";
		base.ResumeLayout(false);
		base.PerformLayout();
	}
}
