using System;
using System.ComponentModel;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;
using CDV_Viewer.Controls;
using CDV_Viewer.Data.SIGObjects;
using CDV_Viewer.Properties;
using CDV_Viewer.Traitements;

namespace CDV_Viewer.PopupForms;

public class RecherchePopupForm : PopupForm
{
	private SimpleListView lvResults;

	private PictureBox pbImage;

	private TextBox tbRecherche;

	private Recherche _recherche;

	private SIGCircuit _circuitSelected;

	public string Texte
	{
		get
		{
			return tbRecherche.Text;
		}
		set
		{
			tbRecherche.Text = value;
		}
	}

	public RecherchePopupForm()
	{
		InitializeComponent();
		tbRecherche.TextChanged += delegate
		{
			tbRecherche_TextChanged();
		};
		lvResults.MouseDoubleClick += delegate
		{
			lvResults_MouseDoubleClick();
		};
	}

	private void InitializeComponent()
	{
		this.tbRecherche = new System.Windows.Forms.TextBox();
		this.lvResults = new CDV_Viewer.Controls.SimpleListView();
		this.pbImage = new System.Windows.Forms.PictureBox();
		((System.ComponentModel.ISupportInitialize)this.pbImage).BeginInit();
		base.SuspendLayout();
		this.tbRecherche.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.tbRecherche.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
		this.tbRecherche.Font = new System.Drawing.Font("Verdana", 9.75f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		this.tbRecherche.Location = new System.Drawing.Point(0, 1);
		this.tbRecherche.Name = "tbRecherche";
		this.tbRecherche.Size = new System.Drawing.Size(224, 23);
		this.tbRecherche.TabIndex = 0;
		this.lvResults.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.lvResults.BackColor = System.Drawing.Color.White;
		this.lvResults.Cursor = System.Windows.Forms.Cursors.Hand;
		this.lvResults.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
		this.lvResults.HoverColor = System.Drawing.Color.FromArgb(230, 230, 230);
		this.lvResults.ItemSize = 20;
		this.lvResults.Location = new System.Drawing.Point(0, 25);
		this.lvResults.Name = "lvResults";
		this.lvResults.Orientation = CDV_Viewer.Controls.ScrollBarOrientation.Vertical;
		this.lvResults.SelectedColor = System.Drawing.Color.Gainsboro;
		this.lvResults.SelectedIndex = -1;
		this.lvResults.Size = new System.Drawing.Size(224, 170);
		this.lvResults.TabIndex = 1;
		this.lvResults.Text = "simpleListView1";
		this.pbImage.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
		this.pbImage.BackColor = System.Drawing.Color.White;
		this.pbImage.Image = CDV_Viewer.Properties.Resources.search;
		this.pbImage.Location = new System.Drawing.Point(205, 4);
		this.pbImage.Name = "pbImage";
		this.pbImage.Size = new System.Drawing.Size(16, 16);
		this.pbImage.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage;
		this.pbImage.TabIndex = 2;
		this.pbImage.TabStop = false;
		base.Controls.Add(this.pbImage);
		base.Controls.Add(this.lvResults);
		base.Controls.Add(this.tbRecherche);
		base.Name = "RecherchePopupForm";
		base.Size = new System.Drawing.Size(224, 195);
		((System.ComponentModel.ISupportInitialize)this.pbImage).EndInit();
		base.ResumeLayout(false);
		base.PerformLayout();
	}

	private void tbRecherche_TextChanged()
	{
		_recherche?.Stop();
		_recherche = new Recherche(null, tbRecherche.Text, 100);
		_recherche.EndSearching += delegate
		{
			Recherche_EndSearching();
		};
		_recherche.Start();
	}

	private void Recherche_EndSearching()
	{
		Invoke(new ThreadStart(DisplayResults));
	}

	private void DisplayResults()
	{
		lvResults.Items.Clear();
		foreach (object result in _recherche.Results)
		{
			string text = string.Empty;
			if (result is CircuitOnLigne)
			{
				text = ((CircuitOnLigne)result).Circuit.Nom + " (" + ((CircuitOnLigne)result).Ligne + ")";
			}
			lvResults.Items.Add(new SimpleListViewItem(result, text));
		}
	}

	private void lvResults_MouseDoubleClick()
	{
		if (lvResults.SelectedItem != null)
		{
			CircuitOnLigne circuitOnLigne = lvResults.SelectedItem.Tag as CircuitOnLigne;
			ComposantsViewer.SetLigne(circuitOnLigne.Ligne);
			_circuitSelected = circuitOnLigne.Circuit;
			ComposantsViewer.SelectedLigneChanged += ComposantsViewer_SelectedLigneChanged;
			Close(PopupFormResultEventArgs.Ok);
		}
	}

	private void ComposantsViewer_SelectedLigneChanged(object sender, EventArgs e)
	{
		ComposantsViewer.SelectedLigneChanged -= ComposantsViewer_SelectedLigneChanged;
		ComposantsViewer.LightCircuit(_circuitSelected);
		_circuitSelected = null;
	}
}
