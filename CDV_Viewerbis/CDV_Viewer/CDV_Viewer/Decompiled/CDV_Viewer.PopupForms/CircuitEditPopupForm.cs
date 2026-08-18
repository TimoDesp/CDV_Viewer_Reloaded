using System;
using System.ComponentModel;
using System.Drawing;
using System.Globalization;
using System.Windows.Forms;
using CDV_Viewer.Controls;
using CDV_Viewer.CsvBase;
using CDV_Viewer.CustomControls;
using CDV_Viewer.Data;
using CDV_Viewer.Data.SIGObjects;
using CDV_Viewer.Properties;

namespace CDV_Viewer.PopupForms;

public class CircuitEditPopupForm : PopupForm
{
	private class ListSelector<T> : ComboBox
	{
		public new T SelectedItem
		{
			get
			{
				return (T)(base.SelectedItem ?? ((object)default(T)));
			}
			set
			{
				base.SelectedItem = value;
			}
		}

		public void SetItems()
		{
			foreach (T value in Enum.GetValues(typeof(T)))
			{
				base.Items.Add(value);
			}
		}
	}

	private Label label1;

	private ListSelector<CircuitType> cbType;

	private ComboBox cbFrequence;

	private Label label2;

	private CheckBox cbCompense;

	private GroupBox gbCompensation;

	private Label lPas;

	private CustomTextBox tbPas;

	private ComboBox cbTypeCompensation;

	private Label lTypeCompensation;

	private NumericUpDown nudPts;

	private Label lPts;

	private Label label6;

	private CustomTextBox tbICC;

	private Label label7;

	private CustomTextBox tbIFuite;

	private Label label8;

	private CustomTextBox tbDiaphonie;

	private GroupBox gbSeuils;

	private Label lDemiPas;

	private CustomTextBox tbDemiPas;

	private CustomTextBox tbNom;

	private Button bCalculerPas;

	private Button bCalculerNbPts;

	private Button bCalculerSeuils;

	private CheckBox tbCALCUL_CONFORME;

	private Label label3;

	private CustomTextBox tbN_FUITE_LONG_ARR;

	private CircuitType _oldType;

	public SIGCircuit Circuit { get; }

	public string Repere
	{
		get
		{
			return tbNom.Text.Trim();
		}
		set
		{
			if (value == null)
			{
				tbNom.Text = string.Empty;
			}
			else
			{
				tbNom.Text = value.Trim();
			}
		}
	}

	private CircuitType TypeCdv
	{
		get
		{
			return cbType.SelectedItem;
		}
		set
		{
			if (value != cbType.SelectedItem)
			{
				cbType.SelectedItem = value;
			}
		}
	}

	private CompensationType TypeCompensation
	{
		get
		{
			if (Enum.TryParse<CompensationType>(cbTypeCompensation.Text, out var result))
			{
				return result;
			}
			return CompensationType.NON;
		}
	}

	private int Frequence
	{
		get
		{
			return (int)(cbFrequence.SelectedItem ?? ((object)0));
		}
		set
		{
			if ((int)(cbFrequence.SelectedItem ?? ((object)0)) != value)
			{
				cbFrequence.SelectedItem = value;
			}
		}
	}

	private int NbPointDeCompensation
	{
		get
		{
			return (int)nudPts.Value;
		}
		set
		{
			if (nudPts.Value != (decimal)value)
			{
				nudPts.Value = value;
			}
		}
	}

	private int PasTheorique => CircuitTheorique.PasTheorique(TypeCompensation, Frequence);

	private double PasReel
	{
		get
		{
			if (!double.TryParse(tbPas.Text, NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture, out var result))
			{
				result = CircuitTheorique.PasTheorique(TypeCompensation, Frequence);
				tbPas.Text = result.ToString(CultureInfo.InvariantCulture);
			}
			return result;
		}
		set
		{
			tbPas.Text = value.ToString(CultureInfo.InvariantCulture);
		}
	}

	private double[] DemiPas
	{
		get
		{
			if (tbDemiPas.Text.Contains("-"))
			{
				string[] array = tbDemiPas.Text.Split('-');
				return new double[2]
				{
					Convert.ToDouble(array[0], CultureInfo.InvariantCulture),
					Convert.ToDouble(array[1], CultureInfo.InvariantCulture)
				};
			}
			return new double[1] { Convert.ToDouble(tbDemiPas.Text, CultureInfo.InvariantCulture) };
		}
		set
		{
			if (value == null || value.Length == 0)
			{
				tbDemiPas.Text = "";
			}
			else if (value.Length == 1)
			{
				tbDemiPas.Text = value[0].ToString(CultureInfo.InvariantCulture);
			}
			else if (value[0] == value[1])
			{
				tbDemiPas.Text = value[0].ToString(CultureInfo.InvariantCulture);
			}
			else
			{
				tbDemiPas.Text = value[0].ToString(CultureInfo.InvariantCulture) + "-" + value[1].ToString(CultureInfo.InvariantCulture);
			}
		}
	}

	private double IccMin
	{
		get
		{
			if (int.TryParse(tbICC.Text, out var result))
			{
				return result;
			}
			return 0.0;
		}
		set
		{
			tbICC.Text = value.ToString();
		}
	}

	private double IFuiteMax
	{
		get
		{
			if (int.TryParse(tbIFuite.Text, out var result))
			{
				return result;
			}
			return 0.0;
		}
		set
		{
			tbIFuite.Text = value.ToString();
		}
	}

	private double IfuiteArriereMax
	{
		get
		{
			if (int.TryParse(tbN_FUITE_LONG_ARR.Text, out var result))
			{
				return result;
			}
			return 0.0;
		}
		set
		{
			tbN_FUITE_LONG_ARR.Text = value.ToString();
		}
	}

	private double Diaphonie
	{
		get
		{
			if (int.TryParse(tbDiaphonie.Text, out var result))
			{
				return result;
			}
			return 0.0;
		}
		set
		{
			tbDiaphonie.Text = value.ToString();
		}
	}

	private bool CalculConforme
	{
		get
		{
			return tbCALCUL_CONFORME.Checked;
		}
		set
		{
			if (tbCALCUL_CONFORME.Checked != value)
			{
				tbCALCUL_CONFORME.Checked = value;
			}
		}
	}

	public CircuitEditPopupForm()
	{
		InitializeComponent();
		Circuit = new SIGCircuit();
		cbType.SetItems();
		cbType.SelectedItem = (_oldType = Circuit.Type);
		cbType.SelectedIndexChanged += delegate
		{
			CdvTypeChanged();
		};
		cbCompense.Visible = false;
		gbCompensation.Visible = false;
		gbSeuils.Top = gbCompensation.Top;
		base.Height = gbSeuils.Bottom + 7;
		bCalculerSeuils.Click += delegate
		{
			CalculerSeuils();
		};
		TypeCdv = CircuitType.NC;
		Frequence = 0;
		NbPointDeCompensation = 0;
		PasReel = 0.0;
		IccMin = 0.0;
		IFuiteMax = 0.0;
		Diaphonie = 0.0;
		IfuiteArriereMax = 0.0;
	}

	public CircuitEditPopupForm(SIGCircuit circuit)
	{
		InitializeComponent();
		Circuit = circuit;
		cbType.SetItems();
		cbType.SelectedItem = (_oldType = Circuit.Type);
		cbFrequence.Items.AddRange(CircuitTheorique.ValidFrequencies(Circuit.Type));
		cbType.SelectedIndexChanged += delegate
		{
			CdvTypeChanged();
		};
		cbCompense.Visible = true;
		gbCompensation.Visible = true;
		cbCompense.CheckedChanged += CbCompense_CheckedChanged;
		bCalculerNbPts.Click += delegate
		{
			CalculerNbPointsCompensation();
		};
		bCalculerPas.Click += delegate
		{
			CalculerPasReel();
		};
		bCalculerSeuils.Click += delegate
		{
			CalculerSeuils();
		};
		Repere = Circuit.Nom;
		Frequence = Circuit.Frequence;
		TypeCdv = Circuit.Type;
		NbPointDeCompensation = Circuit.NbPtsCompensation;
		PasReel = Circuit.PasReel;
		DemiPas = new double[2]
		{
			Circuit.DemiJointDebut.DemiPas,
			Circuit.DemiJointFin.DemiPas
		};
		IccMin = Circuit.ICC;
		IFuiteMax = Circuit.IFuite;
		Diaphonie = Circuit.Diaphonie;
		IfuiteArriereMax = Circuit.N_FUITE_LONG_ARR;
		CalculConforme = Circuit.CALCUL_CONFORME;
	}

	private void InitializeComponent()
	{
		this.tbNom = new CDV_Viewer.CustomControls.CustomTextBox();
		this.label1 = new System.Windows.Forms.Label();
		this.cbType = new CDV_Viewer.PopupForms.CircuitEditPopupForm.ListSelector<CDV_Viewer.CsvBase.CircuitType>();
		this.cbFrequence = new System.Windows.Forms.ComboBox();
		this.label2 = new System.Windows.Forms.Label();
		this.cbCompense = new System.Windows.Forms.CheckBox();
		this.gbCompensation = new System.Windows.Forms.GroupBox();
		this.bCalculerPas = new System.Windows.Forms.Button();
		this.bCalculerNbPts = new System.Windows.Forms.Button();
		this.lDemiPas = new System.Windows.Forms.Label();
		this.tbDemiPas = new CDV_Viewer.CustomControls.CustomTextBox();
		this.nudPts = new System.Windows.Forms.NumericUpDown();
		this.lPts = new System.Windows.Forms.Label();
		this.lPas = new System.Windows.Forms.Label();
		this.tbPas = new CDV_Viewer.CustomControls.CustomTextBox();
		this.cbTypeCompensation = new System.Windows.Forms.ComboBox();
		this.lTypeCompensation = new System.Windows.Forms.Label();
		this.label6 = new System.Windows.Forms.Label();
		this.tbICC = new CDV_Viewer.CustomControls.CustomTextBox();
		this.label7 = new System.Windows.Forms.Label();
		this.tbIFuite = new CDV_Viewer.CustomControls.CustomTextBox();
		this.label8 = new System.Windows.Forms.Label();
		this.tbDiaphonie = new CDV_Viewer.CustomControls.CustomTextBox();
		this.gbSeuils = new System.Windows.Forms.GroupBox();
		this.tbCALCUL_CONFORME = new System.Windows.Forms.CheckBox();
		this.label3 = new System.Windows.Forms.Label();
		this.tbN_FUITE_LONG_ARR = new CDV_Viewer.CustomControls.CustomTextBox();
		this.bCalculerSeuils = new System.Windows.Forms.Button();
		this.gbCompensation.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.nudPts).BeginInit();
		this.gbSeuils.SuspendLayout();
		base.SuspendLayout();
		this.tbNom.BackColor = System.Drawing.Color.White;
		this.tbNom.Location = new System.Drawing.Point(111, 6);
		this.tbNom.Name = "tbNom";
		this.tbNom.Padding = new System.Windows.Forms.Padding(1);
		this.tbNom.SelectionLength = 0;
		this.tbNom.SelectionStart = 0;
		this.tbNom.Size = new System.Drawing.Size(100, 20);
		this.tbNom.TabIndex = 0;
		this.tbNom.TextAlign = System.Windows.Forms.HorizontalAlignment.Left;
		this.label1.AutoSize = true;
		this.label1.Location = new System.Drawing.Point(18, 45);
		this.label1.Name = "label1";
		this.label1.Size = new System.Drawing.Size(37, 13);
		this.label1.TabIndex = 1;
		this.label1.Text = "Type :";
		this.cbType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
		this.cbType.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
		this.cbType.FormattingEnabled = true;
		this.cbType.Location = new System.Drawing.Point(61, 42);
		this.cbType.Name = "cbType";
		this.cbType.Size = new System.Drawing.Size(66, 21);
		this.cbType.TabIndex = 2;
		this.cbFrequence.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
		this.cbFrequence.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
		this.cbFrequence.FormattingEnabled = true;
		this.cbFrequence.Location = new System.Drawing.Point(243, 42);
		this.cbFrequence.Name = "cbFrequence";
		this.cbFrequence.Size = new System.Drawing.Size(51, 21);
		this.cbFrequence.TabIndex = 4;
		this.label2.AutoSize = true;
		this.label2.Location = new System.Drawing.Point(173, 45);
		this.label2.Name = "label2";
		this.label2.Size = new System.Drawing.Size(64, 13);
		this.label2.TabIndex = 3;
		this.label2.Text = "Fréquence :";
		this.cbCompense.AutoSize = true;
		this.cbCompense.Location = new System.Drawing.Point(10, 72);
		this.cbCompense.Name = "cbCompense";
		this.cbCompense.Size = new System.Drawing.Size(82, 17);
		this.cbCompense.TabIndex = 5;
		this.cbCompense.Text = "Compensé :";
		this.cbCompense.UseVisualStyleBackColor = true;
		this.gbCompensation.Controls.Add(this.bCalculerPas);
		this.gbCompensation.Controls.Add(this.bCalculerNbPts);
		this.gbCompensation.Controls.Add(this.lDemiPas);
		this.gbCompensation.Controls.Add(this.tbDemiPas);
		this.gbCompensation.Controls.Add(this.nudPts);
		this.gbCompensation.Controls.Add(this.lPts);
		this.gbCompensation.Controls.Add(this.lPas);
		this.gbCompensation.Controls.Add(this.tbPas);
		this.gbCompensation.Controls.Add(this.cbTypeCompensation);
		this.gbCompensation.Controls.Add(this.lTypeCompensation);
		this.gbCompensation.Location = new System.Drawing.Point(7, 86);
		this.gbCompensation.Name = "gbCompensation";
		this.gbCompensation.Size = new System.Drawing.Size(305, 109);
		this.gbCompensation.TabIndex = 6;
		this.gbCompensation.TabStop = false;
		this.bCalculerPas.Location = new System.Drawing.Point(232, 75);
		this.bCalculerPas.Name = "bCalculerPas";
		this.bCalculerPas.Size = new System.Drawing.Size(64, 23);
		this.bCalculerPas.TabIndex = 12;
		this.bCalculerPas.Text = "Calculer";
		this.bCalculerPas.UseVisualStyleBackColor = true;
		this.bCalculerNbPts.Location = new System.Drawing.Point(232, 44);
		this.bCalculerNbPts.Name = "bCalculerNbPts";
		this.bCalculerNbPts.Size = new System.Drawing.Size(64, 23);
		this.bCalculerNbPts.TabIndex = 11;
		this.bCalculerNbPts.Text = "Calculer";
		this.bCalculerNbPts.UseVisualStyleBackColor = true;
		this.lDemiPas.AutoSize = true;
		this.lDemiPas.Location = new System.Drawing.Point(94, 80);
		this.lDemiPas.Name = "lDemiPas";
		this.lDemiPas.Size = new System.Drawing.Size(58, 13);
		this.lDemiPas.TabIndex = 10;
		this.lDemiPas.Text = "Demi-Pas :";
		this.tbDemiPas.BackColor = System.Drawing.Color.White;
		this.tbDemiPas.Enabled = false;
		this.tbDemiPas.Location = new System.Drawing.Point(158, 76);
		this.tbDemiPas.Name = "tbDemiPas";
		this.tbDemiPas.Padding = new System.Windows.Forms.Padding(1);
		this.tbDemiPas.SelectionLength = 0;
		this.tbDemiPas.SelectionStart = 0;
		this.tbDemiPas.Size = new System.Drawing.Size(46, 20);
		this.tbDemiPas.TabIndex = 9;
		this.tbDemiPas.TextAlign = System.Windows.Forms.HorizontalAlignment.Left;
		this.nudPts.Location = new System.Drawing.Point(190, 47);
		this.nudPts.Name = "nudPts";
		this.nudPts.Size = new System.Drawing.Size(35, 20);
		this.nudPts.TabIndex = 8;
		this.lPts.AutoSize = true;
		this.lPts.Location = new System.Drawing.Point(8, 49);
		this.lPts.Name = "lPts";
		this.lPts.Size = new System.Drawing.Size(183, 13);
		this.lPts.TabIndex = 7;
		this.lPts.Text = "Nombre de points de compensation : ";
		this.lPas.AutoSize = true;
		this.lPas.Location = new System.Drawing.Point(15, 80);
		this.lPas.Name = "lPas";
		this.lPas.Size = new System.Drawing.Size(31, 13);
		this.lPas.TabIndex = 6;
		this.lPas.Text = "Pas :";
		this.tbPas.BackColor = System.Drawing.Color.White;
		this.tbPas.Location = new System.Drawing.Point(46, 76);
		this.tbPas.Name = "tbPas";
		this.tbPas.Padding = new System.Windows.Forms.Padding(1);
		this.tbPas.SelectionLength = 0;
		this.tbPas.SelectionStart = 0;
		this.tbPas.Size = new System.Drawing.Size(35, 20);
		this.tbPas.TabIndex = 5;
		this.tbPas.TextAlign = System.Windows.Forms.HorizontalAlignment.Left;
		this.cbTypeCompensation.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
		this.cbTypeCompensation.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
		this.cbTypeCompensation.FormattingEnabled = true;
		this.cbTypeCompensation.Location = new System.Drawing.Point(130, 15);
		this.cbTypeCompensation.Name = "cbTypeCompensation";
		this.cbTypeCompensation.Size = new System.Drawing.Size(101, 21);
		this.cbTypeCompensation.TabIndex = 4;
		this.lTypeCompensation.AutoSize = true;
		this.lTypeCompensation.Location = new System.Drawing.Point(87, 18);
		this.lTypeCompensation.Name = "lTypeCompensation";
		this.lTypeCompensation.Size = new System.Drawing.Size(37, 13);
		this.lTypeCompensation.TabIndex = 3;
		this.lTypeCompensation.Text = "Type :";
		this.label6.AutoSize = true;
		this.label6.Location = new System.Drawing.Point(8, 24);
		this.label6.Name = "label6";
		this.label6.Size = new System.Drawing.Size(48, 13);
		this.label6.TabIndex = 8;
		this.label6.Text = "Icc Min :";
		this.tbICC.BackColor = System.Drawing.Color.White;
		this.tbICC.Location = new System.Drawing.Point(62, 19);
		this.tbICC.Name = "tbICC";
		this.tbICC.Padding = new System.Windows.Forms.Padding(1);
		this.tbICC.SelectionLength = 0;
		this.tbICC.SelectionStart = 0;
		this.tbICC.Size = new System.Drawing.Size(30, 20);
		this.tbICC.TabIndex = 7;
		this.tbICC.TextAlign = System.Windows.Forms.HorizontalAlignment.Left;
		this.label7.AutoSize = true;
		this.label7.Location = new System.Drawing.Point(125, 24);
		this.label7.Name = "label7";
		this.label7.Size = new System.Drawing.Size(59, 13);
		this.label7.TabIndex = 10;
		this.label7.Text = "Ifuite Max :";
		this.tbIFuite.BackColor = System.Drawing.Color.White;
		this.tbIFuite.Location = new System.Drawing.Point(190, 21);
		this.tbIFuite.Name = "tbIFuite";
		this.tbIFuite.Padding = new System.Windows.Forms.Padding(1);
		this.tbIFuite.SelectionLength = 0;
		this.tbIFuite.SelectionStart = 0;
		this.tbIFuite.Size = new System.Drawing.Size(30, 20);
		this.tbIFuite.TabIndex = 9;
		this.tbIFuite.TextAlign = System.Windows.Forms.HorizontalAlignment.Left;
		this.label8.AutoSize = true;
		this.label8.Location = new System.Drawing.Point(8, 88);
		this.label8.Name = "label8";
		this.label8.Size = new System.Drawing.Size(84, 13);
		this.label8.TabIndex = 12;
		this.label8.Text = "Diaphonie Max :";
		this.tbDiaphonie.BackColor = System.Drawing.Color.White;
		this.tbDiaphonie.Location = new System.Drawing.Point(104, 81);
		this.tbDiaphonie.Name = "tbDiaphonie";
		this.tbDiaphonie.Padding = new System.Windows.Forms.Padding(1);
		this.tbDiaphonie.SelectionLength = 0;
		this.tbDiaphonie.SelectionStart = 0;
		this.tbDiaphonie.Size = new System.Drawing.Size(30, 20);
		this.tbDiaphonie.TabIndex = 11;
		this.tbDiaphonie.TextAlign = System.Windows.Forms.HorizontalAlignment.Left;
		this.gbSeuils.Controls.Add(this.tbCALCUL_CONFORME);
		this.gbSeuils.Controls.Add(this.label3);
		this.gbSeuils.Controls.Add(this.tbN_FUITE_LONG_ARR);
		this.gbSeuils.Controls.Add(this.bCalculerSeuils);
		this.gbSeuils.Controls.Add(this.label6);
		this.gbSeuils.Controls.Add(this.label8);
		this.gbSeuils.Controls.Add(this.tbDiaphonie);
		this.gbSeuils.Controls.Add(this.tbICC);
		this.gbSeuils.Controls.Add(this.label7);
		this.gbSeuils.Controls.Add(this.tbIFuite);
		this.gbSeuils.Location = new System.Drawing.Point(7, 201);
		this.gbSeuils.Name = "gbSeuils";
		this.gbSeuils.Size = new System.Drawing.Size(305, 115);
		this.gbSeuils.TabIndex = 13;
		this.gbSeuils.TabStop = false;
		this.gbSeuils.Text = "Seuils";
		this.tbCALCUL_CONFORME.AutoSize = true;
		this.tbCALCUL_CONFORME.Location = new System.Drawing.Point(148, 53);
		this.tbCALCUL_CONFORME.Name = "tbCALCUL_CONFORME";
		this.tbCALCUL_CONFORME.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
		this.tbCALCUL_CONFORME.Size = new System.Drawing.Size(72, 17);
		this.tbCALCUL_CONFORME.TabIndex = 16;
		this.tbCALCUL_CONFORME.Text = "Calc Conf";
		this.tbCALCUL_CONFORME.UseVisualStyleBackColor = true;
		this.label3.AutoSize = true;
		this.label3.Location = new System.Drawing.Point(8, 57);
		this.label3.Name = "label3";
		this.label3.Size = new System.Drawing.Size(93, 13);
		this.label3.TabIndex = 15;
		this.label3.Text = "N Fuite Long. Arr :";
		this.tbN_FUITE_LONG_ARR.BackColor = System.Drawing.Color.White;
		this.tbN_FUITE_LONG_ARR.Location = new System.Drawing.Point(104, 53);
		this.tbN_FUITE_LONG_ARR.Name = "tbN_FUITE_LONG_ARR";
		this.tbN_FUITE_LONG_ARR.Padding = new System.Windows.Forms.Padding(1);
		this.tbN_FUITE_LONG_ARR.SelectionLength = 0;
		this.tbN_FUITE_LONG_ARR.SelectionStart = 0;
		this.tbN_FUITE_LONG_ARR.Size = new System.Drawing.Size(30, 20);
		this.tbN_FUITE_LONG_ARR.TabIndex = 14;
		this.tbN_FUITE_LONG_ARR.TextAlign = System.Windows.Forms.HorizontalAlignment.Left;
		this.bCalculerSeuils.Location = new System.Drawing.Point(232, 18);
		this.bCalculerSeuils.Name = "bCalculerSeuils";
		this.bCalculerSeuils.Size = new System.Drawing.Size(64, 52);
		this.bCalculerSeuils.TabIndex = 13;
		this.bCalculerSeuils.Text = "Calculer";
		this.bCalculerSeuils.UseVisualStyleBackColor = true;
		this.BackColor = System.Drawing.Color.White;
		base.Controls.Add(this.gbSeuils);
		base.Controls.Add(this.gbCompensation);
		base.Controls.Add(this.cbCompense);
		base.Controls.Add(this.cbFrequence);
		base.Controls.Add(this.label2);
		base.Controls.Add(this.cbType);
		base.Controls.Add(this.label1);
		base.Controls.Add(this.tbNom);
		base.Name = "CircuitEditPopupForm";
		base.Size = new System.Drawing.Size(319, 319);
		this.gbCompensation.ResumeLayout(false);
		this.gbCompensation.PerformLayout();
		((System.ComponentModel.ISupportInitialize)this.nudPts).EndInit();
		this.gbSeuils.ResumeLayout(false);
		this.gbSeuils.PerformLayout();
		base.ResumeLayout(false);
		base.PerformLayout();
	}

	private void CdvTypeChanged()
	{
		if (TypeCdv != _oldType)
		{
			if (((_oldType == CircuitType.ITE || _oldType == CircuitType.NC) && (TypeCdv == CircuitType.ITE || TypeCdv == CircuitType.NC)) || !base.IsHandleCreated)
			{
				_changeCdv();
			}
			else if (MessageBox.Show("Attention ! La modification de ce champ va entrainer le recalcul de la fréquence et du type de compensation. Les valeurs actuelles seront perdues. Voulez-vous continuer ?", Resources.APP_NAME, MessageBoxButtons.YesNo) == DialogResult.Yes)
			{
				_changeCdv();
			}
			else
			{
				TypeCdv = _oldType;
			}
		}
		void _changeCdv()
		{
			_oldType = TypeCdv;
			cbFrequence.Items.Clear();
			cbFrequence.Items.AddRange(CircuitTheorique.ValidFrequencies(TypeCdv));
			if (cbFrequence.Items.Contains(Circuit.Frequence))
			{
				Frequence = Circuit.Frequence;
			}
			else
			{
				SIGCircuit circuit = Circuit;
				int frequence = (Frequence = ((cbFrequence.Items.Count > 0) ? ((int)cbFrequence.Items[0]) : 0));
				circuit.Frequence = frequence;
			}
			cbCompense.Enabled = CircuitTheorique.CanBeCompensed(TypeCdv, Frequence);
			cbCompense.Checked = Circuit.Compensation != CompensationType.NON;
		}
	}

	private void CbCompense_CheckedChanged(object sender, EventArgs e)
	{
		gbCompensation.Enabled = true;
		cbTypeCompensation.Items.Clear();
		CompensationType compensationType = Circuit.Compensation;
		if (compensationType == CompensationType.NON)
		{
			compensationType = CircuitTheorique.DefaultCompensation(TypeCdv);
		}
		if (!cbCompense.Checked || compensationType == CompensationType.NON)
		{
			NbPointDeCompensation = 0;
			PasReel = 0.0;
			DemiPas = new double[1];
			cbTypeCompensation.Items.Add(CompensationType.NON.ToString());
			cbTypeCompensation.SelectedIndex = 0;
			return;
		}
		NbPointDeCompensation = Circuit.NbPtsCompensation;
		PasReel = Circuit.PasReel;
		DemiPas = new double[2]
		{
			Circuit.DemiJointDebut?.DemiPas ?? 0.0,
			Circuit.DemiJointFin?.DemiPas ?? 0.0
		};
		cbTypeCompensation.Items.Add(CompensationType.P_CONSTANT.ToString());
		cbTypeCompensation.Items.Add(CompensationType.P_VARIABLE.ToString());
		switch (compensationType)
		{
		case CompensationType.P_CONSTANT:
			cbTypeCompensation.SelectedIndex = 0;
			break;
		case CompensationType.P_VARIABLE:
			cbTypeCompensation.SelectedIndex = 1;
			break;
		}
	}

	private void CalculerNbPointsCompensation()
	{
		if (Circuit.DemiJointDebut?.Joint != null && Circuit.DemiJointFin?.Joint != null)
		{
			NbPointDeCompensation = CircuitTheorique.GetNbPointsCompensation(TypeCompensation, Circuit.GetLongueurUtile(), PasTheorique);
			CalculerPasReel();
		}
	}

	private void CalculerPasReel()
	{
		double longueurUtile = Circuit.GetLongueurUtile();
		int pasTheorique = PasTheorique;
		double num = CircuitTheorique.GetPas(TypeCompensation, longueurUtile, pasTheorique, NbPointDeCompensation);
		double demiPas = CircuitTheorique.GetDemiPas(TypeCompensation, longueurUtile, pasTheorique, NbPointDeCompensation);
		if (Circuit.DemiJointDebut.DemiPas != demiPas || Circuit.DemiJointFin.DemiPas != demiPas)
		{
			if (MessageBox.Show("Attention ! Voulez-vous changer les demi-pas ?", Resources.APP_NAME, MessageBoxButtons.YesNo) == DialogResult.Yes)
			{
				Circuit.DemiJointDebut.DemiPas = demiPas;
				Base.UpdateDemiJoint(Circuit.DemiJointDebut);
				Circuit.DemiJointFin.DemiPas = demiPas;
				Base.UpdateDemiJoint(Circuit.DemiJointFin);
				DemiPas = new double[1] { demiPas };
			}
			else
			{
				num = Circuit.DemiJointDebut.DemiPas + Circuit.DemiJointFin.DemiPas;
				if (NbPointDeCompensation > 1)
				{
					num = (longueurUtile - num) / (double)(NbPointDeCompensation - 1);
				}
			}
		}
		PasReel = Math.Round(num, 2);
	}

	private void CalculerSeuils()
	{
		IccMin = CircuitTheorique.IccMin(TypeCdv, Frequence);
		IfuiteArriereMax = CircuitTheorique.FuiteArriereMax(TypeCdv);
		IFuiteMax = CircuitTheorique.FuiteMax(TypeCdv);
		Diaphonie = CircuitTheorique.DiaphonieMax(TypeCdv);
	}

	protected override void OnClosing(PopupFormResultEventArgs e)
	{
		if (e.Result != PopupContainerResult.OK)
		{
			return;
		}
		if (Repere == string.Empty)
		{
			MessageBox.Show("Nom incorrect");
			e.Canceled = true;
			return;
		}
		string cdvName = Repere;
		if (ComposantsViewer.Ligne.Circuits.Find((SIGCircuit circuit) => circuit.ID != Circuit.ID && circuit.Nom == cdvName) != null)
		{
			MessageBox.Show("Un circuit de voie avec ce nom existe déjà sur cette ligne");
			e.Canceled = true;
			return;
		}
		if (!int.TryParse(cbFrequence.Text, out var result))
		{
			if (TypeCdv != CircuitType.NC)
			{
				MessageBox.Show("Frequence incorrecte");
				e.Canceled = true;
				return;
			}
			result = 0;
		}
		if (!int.TryParse(tbICC.Text, out var result2))
		{
			MessageBox.Show("Valeur de ICC Min incorrecte");
			e.Canceled = true;
			return;
		}
		if (!int.TryParse(tbN_FUITE_LONG_ARR.Text, out var result3))
		{
			MessageBox.Show("Valeur de N Fuite Long. Arr incorrecte");
			e.Canceled = true;
			return;
		}
		if (!int.TryParse(tbIFuite.Text, out var result4))
		{
			MessageBox.Show("Valeur de Ifuite Max incorrecte");
			e.Canceled = true;
			return;
		}
		if (!int.TryParse(tbDiaphonie.Text, out var result5))
		{
			MessageBox.Show("Valeur de Diaphonie Max incorrecte");
			e.Canceled = true;
			return;
		}
		Circuit.Nom = Repere;
		Circuit.Type = TypeCdv;
		Circuit.Frequence = result;
		Circuit.Compensation = TypeCompensation;
		Circuit.NbPtsCompensation = (int)nudPts.Value;
		Circuit.PasReel = Convert.ToDouble(tbPas.Text, CultureInfo.InvariantCulture);
		Circuit.ICC = result2;
		Circuit.N_FUITE_LONG_ARR = result3;
		Circuit.CALCUL_CONFORME = tbCALCUL_CONFORME.Checked;
		Circuit.IFuite = result4;
		Circuit.Diaphonie = result5;
	}
}
