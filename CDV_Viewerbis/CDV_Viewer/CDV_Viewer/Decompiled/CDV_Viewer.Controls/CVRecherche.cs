using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;
using CDV_Viewer.Data.SIGObjects;
using CDV_Viewer.DockControls;
using CDV_Viewer.PopupForms;
using CDV_Viewer.Properties;
using CDV_Viewer.Styles;
using CDV_Viewer.Traitements;

namespace CDV_Viewer.Controls;

public class CVRecherche : UserControl
{
	public const int RESULT_HEIGHT = 20;

	public const int MAX_RESULTS = 5;

	public Recherche _recherche;

	private List<object> _results = new List<object>();

	private int _indexSelected = -1;

	private bool _displayResults;

	private SIGCircuit _circuitSelected;

	private IContainer components;

	private TextBox tbTexte;

	private PictureBox pbImage;

	public CVRecherche()
	{
		InitializeComponent();
		base.Visible = false;
		tbTexte.TextChanged += tbTexte_TextChanged;
		tbTexte.GotFocus += tbTexte_GotFocus;
		tbTexte.LostFocus += tbTexte_LostFocus;
	}

	private void EndSearch()
	{
		RefreshSize();
		Invalidate();
	}

	private void RefreshSize()
	{
		if (_displayResults && tbTexte.Text != string.Empty)
		{
			base.Height = tbTexte.Height + (Math.Min(5, _results.Count) + 1) * 20 + 1;
		}
		else
		{
			base.Height = tbTexte.Height;
		}
	}

	private void Execute(object objet)
	{
		if (objet is int)
		{
			ComposantsViewer.Viewer.MoveToPk((int)objet, Animation: true);
		}
		else if (objet is SIGVoie)
		{
			ComposantsViewer.Viewer.LightVoie((SIGVoie)objet);
		}
		else if (objet is SIGCircuit)
		{
			ComposantsViewer.Viewer.LightCircuit((SIGCircuit)objet);
			tbTexte.Text = "";
		}
		else if (objet is CircuitOnLigne)
		{
			ComposantsViewer.Viewer.SelectedLigneChanged += ComposantsViewer_SelectedLigneChanged;
			ComposantsViewer.Viewer.SetLigne(((CircuitOnLigne)objet).Ligne);
			_circuitSelected = ((CircuitOnLigne)objet).Circuit;
		}
	}

	private void ComposantsViewer_SelectedLigneChanged(object sender, EventArgs e)
	{
		ComposantsViewer.Viewer.SelectedLigneChanged -= ComposantsViewer_SelectedLigneChanged;
		ComposantsViewer.Viewer.LightCircuit(_circuitSelected);
		_circuitSelected = null;
	}

	private string GetType(object objet)
	{
		if (objet is int)
		{
			return "Position (PK)";
		}
		if (objet is SIGVoie)
		{
			if (((SIGVoie)objet).Nom.StartsWith("J"))
			{
				return "Jonction";
			}
			return "Voie";
		}
		if (objet is SIGCircuit || objet is CircuitOnLigne)
		{
			return "Circuit de voie";
		}
		return string.Empty;
	}

	private string GetText(object objet)
	{
		if (objet is int)
		{
			return ((int)objet / 1000).ToString("000") + "+" + ((int)objet % 1000).ToString("000");
		}
		if (objet is SIGVoie)
		{
			return ((SIGVoie)objet).Nom;
		}
		if (objet is SIGCircuit)
		{
			return ((SIGCircuit)objet).Nom;
		}
		if (objet is CircuitOnLigne)
		{
			return ((CircuitOnLigne)objet).Circuit.Nom + " (" + ((CircuitOnLigne)objet).Ligne + ")";
		}
		return string.Empty;
	}

	private Color GetColor(object objet)
	{
		if (objet is SIGCircuit)
		{
			return Colors.GetColor("CDV" + ((SIGCircuit)objet).Frequence);
		}
		return Color.Black;
	}

	protected override void OnPaint(PaintEventArgs e)
	{
		if (!_displayResults)
		{
			return;
		}
		int num = tbTexte.Height;
		Font font = new Font("Arial", 8f);
		Font font2 = new Font("Arial", 8f);
		StringFormat stringFormat = new StringFormat();
		stringFormat.Alignment = StringAlignment.Far;
		for (int i = 0; i < Math.Min(5, _results.Count); i++)
		{
			Rectangle rect = new Rectangle(0, num, base.Width - 1, 20);
			if (_indexSelected == i)
			{
				e.Graphics.FillRectangle(Brushes.Gainsboro, rect);
			}
			else
			{
				e.Graphics.FillRectangle(Brushes.White, rect);
			}
			e.Graphics.DrawRectangle(Pens.LightGray, rect);
			e.Graphics.DrawString(GetText(_results[i]), font, new SolidBrush(GetColor(_results[i])), 4f, num + 4);
			e.Graphics.DrawString(GetType(_results[i]).ToUpper(), font2, new SolidBrush(Color.FromArgb(224, 82, 6)), base.Width - 4, num + 4, stringFormat);
			num += 20;
		}
		if (tbTexte.Text != string.Empty)
		{
			string s = ((_results.Count != 0) ? "Cliquez pour plus de résultats..." : "Aucun résultat. Cliquez pour plus de résultats");
			Rectangle rectangle = new Rectangle(0, num, base.Width - 1, 20);
			if (_indexSelected == _results.Count)
			{
				e.Graphics.FillRectangle(Brushes.Gainsboro, rectangle);
			}
			else
			{
				e.Graphics.FillRectangle(Brushes.White, rectangle);
			}
			e.Graphics.DrawRectangle(Pens.LightGray, rectangle);
			StringFormat stringFormat2 = new StringFormat();
			StringAlignment alignment = (stringFormat2.LineAlignment = StringAlignment.Center);
			stringFormat2.Alignment = alignment;
			e.Graphics.DrawString(s, font, Brushes.DimGray, rectangle, stringFormat2);
		}
	}

	protected override void OnKeyUp(KeyEventArgs e)
	{
		if (_results.Count > 0 && e.KeyCode == Keys.Return)
		{
			Execute(_results[0]);
		}
	}

	private void Recherche_EndSearching(object sender, EventArgs e)
	{
		_results = _recherche.Results;
		Invoke(new ThreadStart(EndSearch));
	}

	protected override void OnMouseMove(MouseEventArgs e)
	{
		if (e.Y > tbTexte.Height)
		{
			Cursor = Cursors.Hand;
			_indexSelected = (int)Math.Floor(((double)e.Y - (double)tbTexte.Height) / 20.0);
		}
		else
		{
			Cursor = Cursors.Default;
			_indexSelected = -1;
		}
		Invalidate();
	}

	protected override void OnMouseLeave(EventArgs e)
	{
		_indexSelected = -1;
		Invalidate();
	}

	protected override void OnMouseClick(MouseEventArgs e)
	{
		tbTexte.Focus();
		if (_indexSelected >= 0 && _indexSelected <= _results.Count)
		{
			_displayResults = false;
			if (_indexSelected < _results.Count)
			{
				tbTexte.Text = GetText(_results[_indexSelected]);
				Execute(_results[_indexSelected]);
				ComposantsViewer.Viewer.Focus();
			}
			else
			{
				RecherchePopupForm recherchePopupForm = new RecherchePopupForm();
				recherchePopupForm.Texte = tbTexte.Text;
				ComposantsViewer.Viewer.PopupContainer.Show(recherchePopupForm, "RECHERCHE");
			}
		}
	}

	private void tbTexte_TextChanged(object sender, EventArgs e)
	{
		if (_recherche != null)
		{
			_recherche.Stop();
			_recherche = null;
		}
		_recherche = new Recherche(ComposantsViewer.Viewer.Ligne, tbTexte.Text, 5);
		_recherche.EndSearching += Recherche_EndSearching;
		_recherche.Start();
	}

	private void tbTexte_GotFocus(object sender, EventArgs e)
	{
		if (tbTexte.ForeColor == Color.Gray)
		{
			tbTexte.ForeColor = Color.Black;
			tbTexte.Text = string.Empty;
		}
		_displayResults = true;
	}

	private void tbTexte_LostFocus(object sender, EventArgs e)
	{
		_displayResults = false;
		RefreshSize();
		if (tbTexte.Text == string.Empty)
		{
			tbTexte.TextChanged -= tbTexte_TextChanged;
			tbTexte.Text = "Rechercher...";
			tbTexte.ForeColor = Color.Gray;
			tbTexte.TextChanged += tbTexte_TextChanged;
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
		this.tbTexte = new System.Windows.Forms.TextBox();
		this.pbImage = new System.Windows.Forms.PictureBox();
		((System.ComponentModel.ISupportInitialize)this.pbImage).BeginInit();
		base.SuspendLayout();
		this.tbTexte.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.tbTexte.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
		this.tbTexte.Font = new System.Drawing.Font("Verdana", 9.75f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		this.tbTexte.ForeColor = System.Drawing.Color.Gray;
		this.tbTexte.Location = new System.Drawing.Point(0, 0);
		this.tbTexte.Name = "tbTexte";
		this.tbTexte.Size = new System.Drawing.Size(225, 23);
		this.tbTexte.TabIndex = 0;
		this.tbTexte.Text = "Rechercher...";
		this.pbImage.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
		this.pbImage.BackColor = System.Drawing.Color.White;
		this.pbImage.Image = CDV_Viewer.Properties.Resources.search;
		this.pbImage.Location = new System.Drawing.Point(206, 3);
		this.pbImage.Name = "pbImage";
		this.pbImage.Size = new System.Drawing.Size(16, 16);
		this.pbImage.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage;
		this.pbImage.TabIndex = 1;
		this.pbImage.TabStop = false;
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		this.BackColor = System.Drawing.Color.Transparent;
		base.Controls.Add(this.pbImage);
		base.Controls.Add(this.tbTexte);
		this.DoubleBuffered = true;
		base.Name = "CVRecherche";
		base.Size = new System.Drawing.Size(225, 23);
		((System.ComponentModel.ISupportInitialize)this.pbImage).EndInit();
		base.ResumeLayout(false);
		base.PerformLayout();
	}
}
