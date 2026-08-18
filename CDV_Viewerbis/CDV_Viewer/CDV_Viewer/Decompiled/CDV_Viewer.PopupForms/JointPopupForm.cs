using System;
using System.Drawing;
using System.Globalization;
using System.Windows.Forms;
using CDV_Viewer.Controls;
using CDV_Viewer.CsvBase;
using CDV_Viewer.CustomControls;
using CDV_Viewer.Data;
using CDV_Viewer.Data.SIGObjects;
using CDV_Viewer.Styles;
using CDV_Viewer.Traitements;

namespace CDV_Viewer.PopupForms;

public class JointPopupForm : PopupForm
{
	private ComboBox _typeSelector;

	private Label _typeLabel;

	private Label _demiLongueurVisu;

	private Label lTextDemiLongueur;

	private Separator separator1;

	private CheckBox _emetteurAmontEditor;

	private CheckControl _emetteurDebutVisu;

	private CheckBox _dbAmontEditor;

	private CheckControl _dbDebutVisu;

	private Label lTextDemiPasD;

	private CustomTextBox _demiPasAmontEditor;

	private CustomTextBox _demiLongueurEditor;

	private Label lTextDemiPasF;

	private CustomTextBox _demiPasAvalEditor;

	private CheckBox _dbAvalEditor;

	private CheckControl _dbFinVisu;

	private CheckBox _emetteurAvalEditor;

	private CheckControl _emetteurFinVisu;

	private Separator separator2;

	private Label lCdvD;

	private Label lCdvF;

	private Label _demiPasAmontVisu;

	private Label _demiPasAvalVisu;

	private Label _pkVisu;

	private Label lTextPk;

	private CheckControl _jointDebutPrincipalVisu;

	private CheckControl _jointFinPrincipalVisu;

	private Label lNom;

	private CustomTextBox _pkEditor;

	public bool _editMode;

	private double? _demiLongueur;

	private double? _demiPasAmont;

	private double? _demiPasAval;

	private int? _pk;

	private SIGDemiJoint _demiJointAmont;

	private SIGDemiJoint _demiJointAval;

	private SIGJoint _joint;

	public bool EditMode
	{
		get
		{
			return _editMode;
		}
		private set
		{
			CheckControl jointDebutPrincipalVisu = _jointDebutPrincipalVisu;
			bool visible = (_jointFinPrincipalVisu.Visible = true);
			jointDebutPrincipalVisu.Visible = visible;
			_editMode = value;
			bool flag2 = !_editMode;
			CustomTextBox pkEditor = _pkEditor;
			visible = (_pkEditor.Enabled = _editMode);
			pkEditor.Visible = visible;
			_pkVisu.Visible = flag2;
			_demiLongueurVisu.Visible = flag2;
			_dbDebutVisu.Visible = flag2;
			CheckControl emetteurDebutVisu = _emetteurDebutVisu;
			visible = (_demiPasAmontVisu.Visible = flag2);
			emetteurDebutVisu.Visible = visible;
			_dbFinVisu.Visible = flag2;
			CheckControl emetteurFinVisu = _emetteurFinVisu;
			visible = (_demiPasAvalVisu.Visible = flag2);
			emetteurFinVisu.Visible = visible;
			Label typeLabel = _typeLabel;
			visible = (_typeSelector.Visible = _editMode);
			typeLabel.Visible = visible;
			_demiLongueurEditor.Visible = _editMode;
			_dbAmontEditor.Visible = _editMode;
			CheckBox emetteurAmontEditor = _emetteurAmontEditor;
			visible = (_demiPasAmontEditor.Visible = _editMode);
			emetteurAmontEditor.Visible = visible;
			_dbAvalEditor.Visible = _editMode;
			CheckBox emetteurAvalEditor = _emetteurAvalEditor;
			visible = (_demiPasAvalEditor.Visible = _editMode);
			emetteurAvalEditor.Visible = visible;
		}
	}

	public JointType JointType
	{
		get
		{
			if (!Enum.TryParse<JointType>(_typeSelector.Text, out var result))
			{
				ComboBox typeSelector = _typeSelector;
				JointType jointType = (result = JointType.INC);
				typeSelector.Text = jointType.ToString();
			}
			return result;
		}
		private set
		{
			_typeSelector.Text = value.ToString();
		}
	}

	public double? DemiLongueur
	{
		get
		{
			if (_demiLongueur.HasValue)
			{
				return _demiLongueur;
			}
			if (!double.TryParse(_demiLongueurEditor.Text, NumberStyles.Any, CultureInfo.InvariantCulture, out var result))
			{
				return _demiLongueur = null;
			}
			return _demiLongueur = result;
		}
		private set
		{
			Label demiLongueurVisu = _demiLongueurVisu;
			string text = (_demiLongueurEditor.Text = value.ToString());
			demiLongueurVisu.Text = text;
		}
	}

	public double? DemiPasAmont
	{
		get
		{
			if (_demiPasAmont.HasValue)
			{
				return _demiPasAmont;
			}
			if (!double.TryParse(_demiPasAmontEditor.Text, NumberStyles.Any, CultureInfo.InvariantCulture, out var result))
			{
				return _demiPasAmont = null;
			}
			_demiPasAmontVisu.Text = _demiPasAmontEditor.Text;
			return _demiPasAmont = result;
		}
		private set
		{
			CustomTextBox demiPasAmontEditor = _demiPasAmontEditor;
			string text = (_demiPasAmontVisu.Text = value.ToString());
			demiPasAmontEditor.Text = text;
		}
	}

	public double? DemiPasAval
	{
		get
		{
			if (_demiPasAval.HasValue)
			{
				return _demiPasAval;
			}
			if (!double.TryParse(_demiPasAvalEditor.Text, NumberStyles.Any, CultureInfo.InvariantCulture, out var result))
			{
				return _demiPasAval = null;
			}
			_demiPasAvalVisu.Text = _demiPasAvalEditor.Text;
			return _demiPasAval = result;
		}
		private set
		{
			CustomTextBox demiPasAvalEditor = _demiPasAvalEditor;
			string text = (_demiPasAvalVisu.Text = value.ToString());
			demiPasAvalEditor.Text = text;
		}
	}

	public int? Pk
	{
		get
		{
			if (_pk.HasValue)
			{
				return _pk;
			}
			if (!Chaines.TryParsePk(_pkEditor.Text, out var PK))
			{
				return _pk = null;
			}
			return _pk = PK;
		}
		private set
		{
			Label pkVisu = _pkVisu;
			string text = (_pkEditor.Text = Chaines.PkToString(value.Value));
			pkVisu.Text = text;
		}
	}

	public bool PkChanged { get; private set; }

	public bool JointDebutPrincipalChanged { get; private set; }

	public bool JointFinPrincipalChanged { get; private set; }

	public SIGJoint Joint
	{
		get
		{
			return _joint;
		}
		private set
		{
			_joint = value;
			if (value != null)
			{
				Pk = _joint.PK;
				JointType = _joint.Type;
				DemiLongueur = _joint.DemiLongueur;
				_demiJointAmont = _joint.DemiJointAmont;
				if (_demiJointAmont == null)
				{
					_jointDebutPrincipalVisu.Enabled = false;
					CheckBox dbAmontEditor = _dbAmontEditor;
					bool enabled = (_dbDebutVisu.Enabled = false);
					dbAmontEditor.Enabled = enabled;
					CheckBox emetteurAmontEditor = _emetteurAmontEditor;
					enabled = (_emetteurDebutVisu.Enabled = false);
					emetteurAmontEditor.Enabled = enabled;
					Label demiPasAmontVisu = _demiPasAmontVisu;
					enabled = (_demiPasAmontEditor.Enabled = false);
					demiPasAmontVisu.Enabled = enabled;
					lCdvD.Text = "Non relié";
				}
				else
				{
					lCdvD.Text = $"VERS {_demiJointAmont.Circuit.Nom}";
					lCdvD.ForeColor = Colors.Cdv(_demiJointAmont.Circuit.Frequence);
					_jointDebutPrincipalVisu.Checked = _demiJointAmont.Principal;
					CheckBox dbAmontEditor2 = _dbAmontEditor;
					bool enabled = (_dbDebutVisu.Checked = _demiJointAmont.DB);
					dbAmontEditor2.Checked = enabled;
					CheckBox emetteurAmontEditor2 = _emetteurAmontEditor;
					enabled = (_emetteurDebutVisu.Checked = _demiJointAmont.Emetteur);
					emetteurAmontEditor2.Checked = enabled;
					DemiPasAmont = _demiJointAmont.DemiPas;
				}
				_demiJointAval = _joint.DemiJointAval;
				if (_demiJointAval == null)
				{
					_jointFinPrincipalVisu.Enabled = false;
					CheckBox dbAvalEditor = _dbAvalEditor;
					bool enabled = (_dbFinVisu.Enabled = false);
					dbAvalEditor.Enabled = enabled;
					CheckBox emetteurAvalEditor = _emetteurAvalEditor;
					enabled = (_emetteurFinVisu.Enabled = false);
					emetteurAvalEditor.Enabled = enabled;
					Label demiPasAvalVisu = _demiPasAvalVisu;
					enabled = (_demiPasAvalEditor.Enabled = false);
					demiPasAvalVisu.Enabled = enabled;
					lCdvF.Text = "Non relié";
				}
				else
				{
					lCdvF.Text = $"VERS {_demiJointAval.Circuit.Nom}";
					lCdvF.ForeColor = Colors.Cdv(_demiJointAval.Circuit.Frequence);
					_jointFinPrincipalVisu.Checked = _demiJointAval.Principal;
					CheckBox dbAvalEditor2 = _dbAvalEditor;
					bool enabled = (_dbFinVisu.Checked = _demiJointAval.DB);
					dbAvalEditor2.Checked = enabled;
					CheckBox emetteurAvalEditor2 = _emetteurAvalEditor;
					enabled = (_emetteurFinVisu.Checked = _demiJointAval.Emetteur);
					emetteurAvalEditor2.Checked = enabled;
					DemiPasAval = _demiJointAval.DemiPas;
				}
			}
		}
	}

	public JointPopupForm(SIGVoie voie, int pk)
	{
		InitializeComponent();
		int[] array = new int[5];
		foreach (SIGJoint joint in voie.Joints)
		{
			array[(int)joint.Type]++;
		}
		JointType type = JointType.JI;
		int num = 0;
		for (int i = 2; i < 5; i++)
		{
			if (array[i] > num)
			{
				num = array[i];
				type = (JointType)i;
			}
		}
		EditMode = true;
		Joint = new SIGJoint(-1)
		{
			Voie = voie,
			PK = pk,
			Type = type
		};
		ComboBox.ObjectCollection items = _typeSelector.Items;
		object[] names = Enum.GetNames(typeof(JointType));
		items.AddRange(names);
		_typeSelector.SelectedIndexChanged += delegate
		{
			JointTypeChanged();
		};
		_typeSelector.SelectedItem = type.ToString();
	}

	public JointPopupForm(SIGJoint joint, bool editMode)
	{
		InitializeComponent();
		ComboBox.ObjectCollection items = _typeSelector.Items;
		object[] names = Enum.GetNames(typeof(JointType));
		items.AddRange(names);
		EditMode = editMode;
		Joint = joint;
		_typeSelector.SelectedIndexChanged += delegate
		{
			JointTypeChanged();
		};
		if (!editMode)
		{
			lNom.Text = "JOINT " + joint.Type;
			if (Global.ModeEdition)
			{
				Label label = lNom;
				label.Text = label.Text + " (" + joint.ID + ")";
			}
			lTextPk.Top = (_pkVisu.Top = lTextDemiLongueur.Top + 10);
			lTextDemiLongueur.Top = (_demiLongueurVisu.Top = _typeLabel.Top + 10);
		}
	}

	private void InitializeComponent()
	{
		CDV_Viewer.CustomControls.CustomControlColor couleur = new CDV_Viewer.CustomControls.CustomControlColor();
		this._pkVisu = new System.Windows.Forms.Label();
		this.lTextPk = new System.Windows.Forms.Label();
		this._demiPasAvalVisu = new System.Windows.Forms.Label();
		this._demiPasAmontVisu = new System.Windows.Forms.Label();
		this.lCdvF = new System.Windows.Forms.Label();
		this.lCdvD = new System.Windows.Forms.Label();
		this.lTextDemiPasF = new System.Windows.Forms.Label();
		this._demiPasAvalEditor = new CDV_Viewer.CustomControls.CustomTextBox();
		this._dbAvalEditor = new System.Windows.Forms.CheckBox();
		this._emetteurAvalEditor = new System.Windows.Forms.CheckBox();
		this._demiLongueurVisu = new System.Windows.Forms.Label();
		this.lTextDemiPasD = new System.Windows.Forms.Label();
		this._demiPasAmontEditor = new CDV_Viewer.CustomControls.CustomTextBox();
		this._dbAmontEditor = new System.Windows.Forms.CheckBox();
		this._emetteurAmontEditor = new System.Windows.Forms.CheckBox();
		this.lTextDemiLongueur = new System.Windows.Forms.Label();
		this._typeSelector = new System.Windows.Forms.ComboBox();
		this._typeLabel = new System.Windows.Forms.Label();
		this.lNom = new System.Windows.Forms.Label();
		this._demiLongueurEditor = new CDV_Viewer.CustomControls.CustomTextBox();
		this.separator2 = new CDV_Viewer.Controls.Separator();
		this._dbFinVisu = new CDV_Viewer.Controls.CheckControl();
		this._emetteurFinVisu = new CDV_Viewer.Controls.CheckControl();
		this._dbDebutVisu = new CDV_Viewer.Controls.CheckControl();
		this._emetteurDebutVisu = new CDV_Viewer.Controls.CheckControl();
		this.separator1 = new CDV_Viewer.Controls.Separator();
		this._jointDebutPrincipalVisu = new CDV_Viewer.Controls.CheckControl();
		this._jointFinPrincipalVisu = new CDV_Viewer.Controls.CheckControl();
		this._pkEditor = new CDV_Viewer.CustomControls.CustomTextBox();
		base.SuspendLayout();
		this._pkVisu.AutoSize = true;
		this._pkVisu.Location = new System.Drawing.Point(133, 92);
		this._pkVisu.Name = "lPK";
		this._pkVisu.Size = new System.Drawing.Size(13, 13);
		this._pkVisu.TabIndex = 35;
		this._pkVisu.Text = "0";
		this.lTextPk.AutoSize = true;
		this.lTextPk.Location = new System.Drawing.Point(104, 92);
		this.lTextPk.Name = "lTextPk";
		this.lTextPk.Size = new System.Drawing.Size(27, 13);
		this.lTextPk.TabIndex = 34;
		this.lTextPk.Text = "PK :";
		this._demiPasAvalVisu.Location = new System.Drawing.Point(281, 99);
		this._demiPasAvalVisu.Name = "lDemiPasF";
		this._demiPasAvalVisu.Size = new System.Drawing.Size(30, 13);
		this._demiPasAvalVisu.TabIndex = 33;
		this._demiPasAvalVisu.Text = "0";
		this._demiPasAmontVisu.Location = new System.Drawing.Point(64, 99);
		this._demiPasAmontVisu.Name = "lDemiPasD";
		this._demiPasAmontVisu.Size = new System.Drawing.Size(30, 13);
		this._demiPasAmontVisu.TabIndex = 32;
		this._demiPasAmontVisu.Text = "0";
		this.lCdvF.Font = new System.Drawing.Font("Arial", 8.25f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		this.lCdvF.ForeColor = System.Drawing.Color.Gray;
		this.lCdvF.Location = new System.Drawing.Point(222, 8);
		this.lCdvF.Name = "lCdvF";
		this.lCdvF.Size = new System.Drawing.Size(88, 14);
		this.lCdvF.TabIndex = 31;
		this.lCdvF.Text = "CDV";
		this.lCdvF.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
		this.lCdvD.Font = new System.Drawing.Font("Arial", 8.25f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		this.lCdvD.ForeColor = System.Drawing.Color.Gray;
		this.lCdvD.Location = new System.Drawing.Point(1, 8);
		this.lCdvD.Name = "lCdvD";
		this.lCdvD.Size = new System.Drawing.Size(88, 14);
		this.lCdvD.TabIndex = 30;
		this.lCdvD.Text = "CDV";
		this.lCdvD.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
		this.lTextDemiPasF.AutoSize = true;
		this.lTextDemiPasF.Location = new System.Drawing.Point(223, 99);
		this.lTextDemiPasF.Name = "lTextDemiPasF";
		this.lTextDemiPasF.Size = new System.Drawing.Size(58, 13);
		this.lTextDemiPasF.TabIndex = 28;
		this.lTextDemiPasF.Text = "Demi-Pas :";
		this._demiPasAvalEditor.BackColor = System.Drawing.Color.White;
		this._demiPasAvalEditor.Couleur = couleur;
		this._demiPasAvalEditor.Location = new System.Drawing.Point(281, 96);
		this._demiPasAvalEditor.Name = "tbDemiPasF";
		this._demiPasAvalEditor.Padding = new System.Windows.Forms.Padding(1);
		this._demiPasAvalEditor.SelectionLength = 0;
		this._demiPasAvalEditor.SelectionStart = 0;
		this._demiPasAvalEditor.Size = new System.Drawing.Size(30, 20);
		this._demiPasAvalEditor.TabIndex = 27;
		this._demiPasAvalEditor.TextAlign = System.Windows.Forms.HorizontalAlignment.Left;
		this._dbAvalEditor.AutoSize = true;
		this._dbAvalEditor.Location = new System.Drawing.Point(226, 75);
		this._dbAvalEditor.Name = "cbDBF";
		this._dbAvalEditor.Size = new System.Drawing.Size(41, 17);
		this._dbAvalEditor.TabIndex = 25;
		this._dbAvalEditor.Text = "DB";
		this._dbAvalEditor.UseVisualStyleBackColor = true;
		this._emetteurAvalEditor.AutoSize = true;
		this._emetteurAvalEditor.Location = new System.Drawing.Point(226, 52);
		this._emetteurAvalEditor.Name = "cbEmetteurF";
		this._emetteurAvalEditor.Size = new System.Drawing.Size(68, 17);
		this._emetteurAvalEditor.TabIndex = 23;
		this._emetteurAvalEditor.Text = "Emetteur";
		this._emetteurAvalEditor.UseVisualStyleBackColor = true;
		this._demiLongueurVisu.Location = new System.Drawing.Point(186, 64);
		this._demiLongueurVisu.Name = "lDemiLongueur";
		this._demiLongueurVisu.Size = new System.Drawing.Size(24, 13);
		this._demiLongueurVisu.TabIndex = 14;
		this._demiLongueurVisu.Text = "0";
		this.lTextDemiPasD.AutoSize = true;
		this.lTextDemiPasD.Location = new System.Drawing.Point(6, 99);
		this.lTextDemiPasD.Name = "lTextDemiPasD";
		this.lTextDemiPasD.Size = new System.Drawing.Size(58, 13);
		this.lTextDemiPasD.TabIndex = 21;
		this.lTextDemiPasD.Text = "Demi-Pas :";
		this._demiPasAmontEditor.BackColor = System.Drawing.Color.White;
		this._demiPasAmontEditor.Couleur = couleur;
		this._demiPasAmontEditor.Location = new System.Drawing.Point(64, 96);
		this._demiPasAmontEditor.Name = "tbDemiPasD";
		this._demiPasAmontEditor.Padding = new System.Windows.Forms.Padding(1);
		this._demiPasAmontEditor.SelectionLength = 0;
		this._demiPasAmontEditor.SelectionStart = 0;
		this._demiPasAmontEditor.Size = new System.Drawing.Size(30, 20);
		this._demiPasAmontEditor.TabIndex = 20;
		this._demiPasAmontEditor.TextAlign = System.Windows.Forms.HorizontalAlignment.Left;
		this._dbAmontEditor.AutoSize = true;
		this._dbAmontEditor.Location = new System.Drawing.Point(9, 75);
		this._dbAmontEditor.Name = "cbDBD";
		this._dbAmontEditor.Size = new System.Drawing.Size(41, 17);
		this._dbAmontEditor.TabIndex = 18;
		this._dbAmontEditor.Text = "DB";
		this._dbAmontEditor.UseVisualStyleBackColor = true;
		this._emetteurAmontEditor.AutoSize = true;
		this._emetteurAmontEditor.Location = new System.Drawing.Point(9, 52);
		this._emetteurAmontEditor.Name = "cbEmetteurD";
		this._emetteurAmontEditor.Size = new System.Drawing.Size(68, 17);
		this._emetteurAmontEditor.TabIndex = 16;
		this._emetteurAmontEditor.Text = "Emetteur";
		this._emetteurAmontEditor.UseVisualStyleBackColor = true;
		this.lTextDemiLongueur.AutoSize = true;
		this.lTextDemiLongueur.Location = new System.Drawing.Point(104, 64);
		this.lTextDemiLongueur.Name = "lTextDemiLongueur";
		this.lTextDemiLongueur.Size = new System.Drawing.Size(81, 13);
		this.lTextDemiLongueur.TabIndex = 13;
		this.lTextDemiLongueur.Text = "Demi-longueur :";
		this._typeSelector.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
		this._typeSelector.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
		this._typeSelector.FormattingEnabled = true;
		this._typeSelector.Location = new System.Drawing.Point(140, 31);
		this._typeSelector.Name = "_typeSelector";
		this._typeSelector.Size = new System.Drawing.Size(52, 21);
		this._typeSelector.TabIndex = 11;
		this._typeLabel.AutoSize = true;
		this._typeLabel.Location = new System.Drawing.Point(104, 34);
		this._typeLabel.Name = "lTextType";
		this._typeLabel.Size = new System.Drawing.Size(37, 13);
		this._typeLabel.TabIndex = 10;
		this._typeLabel.Text = "Type :";
		this.lNom.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.lNom.Font = new System.Drawing.Font("Arial", 9.75f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		this.lNom.ForeColor = System.Drawing.Color.FromArgb(224, 82, 6);
		this.lNom.Location = new System.Drawing.Point(107, 4);
		this.lNom.Name = "lNom";
		this.lNom.Size = new System.Drawing.Size(111, 16);
		this.lNom.TabIndex = 9;
		this.lNom.Text = "JOINT";
		this.lNom.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
		this._demiLongueurEditor.BackColor = System.Drawing.Color.White;
		this._demiLongueurEditor.Couleur = couleur;
		this._demiLongueurEditor.Location = new System.Drawing.Point(186, 61);
		this._demiLongueurEditor.Name = "_demiLongueurEditor";
		this._demiLongueurEditor.Padding = new System.Windows.Forms.Padding(1);
		this._demiLongueurEditor.SelectionLength = 0;
		this._demiLongueurEditor.SelectionStart = 0;
		this._demiLongueurEditor.Size = new System.Drawing.Size(24, 20);
		this._demiLongueurEditor.TabIndex = 22;
		this._demiLongueurEditor.TextAlign = System.Windows.Forms.HorizontalAlignment.Left;
		this.separator2.ForeColor = System.Drawing.Color.Gainsboro;
		this.separator2.Location = new System.Drawing.Point(216, 8);
		this.separator2.Name = "separator2";
		this.separator2.Orientation = System.Windows.Forms.Orientation.Vertical;
		this.separator2.Size = new System.Drawing.Size(3, 105);
		this.separator2.TabIndex = 29;
		this.separator2.Text = "separator2";
		this._dbFinVisu.Checked = false;
		this._dbFinVisu.Location = new System.Drawing.Point(226, 76);
		this._dbFinVisu.Name = "ccDBF";
		this._dbFinVisu.Size = new System.Drawing.Size(42, 16);
		this._dbFinVisu.TabIndex = 26;
		this._dbFinVisu.Texte = "DB";
		this._emetteurFinVisu.Checked = false;
		this._emetteurFinVisu.Location = new System.Drawing.Point(226, 53);
		this._emetteurFinVisu.Name = "ccEmetteurF";
		this._emetteurFinVisu.Size = new System.Drawing.Size(69, 16);
		this._emetteurFinVisu.TabIndex = 24;
		this._emetteurFinVisu.Texte = "Emetteur";
		this._dbDebutVisu.Checked = false;
		this._dbDebutVisu.Location = new System.Drawing.Point(9, 76);
		this._dbDebutVisu.Name = "ccDBD";
		this._dbDebutVisu.Size = new System.Drawing.Size(42, 16);
		this._dbDebutVisu.TabIndex = 19;
		this._dbDebutVisu.Texte = "DB";
		this._emetteurDebutVisu.Checked = false;
		this._emetteurDebutVisu.Location = new System.Drawing.Point(9, 53);
		this._emetteurDebutVisu.Name = "ccEmetteurD";
		this._emetteurDebutVisu.Size = new System.Drawing.Size(69, 16);
		this._emetteurDebutVisu.TabIndex = 17;
		this._emetteurDebutVisu.Texte = "Emetteur";
		this.separator1.ForeColor = System.Drawing.Color.Gainsboro;
		this.separator1.Location = new System.Drawing.Point(98, 8);
		this.separator1.Name = "separator1";
		this.separator1.Orientation = System.Windows.Forms.Orientation.Vertical;
		this.separator1.Size = new System.Drawing.Size(3, 105);
		this.separator1.TabIndex = 15;
		this.separator1.Text = "separator1";
		this._jointDebutPrincipalVisu.Checked = false;
		this._jointDebutPrincipalVisu.Location = new System.Drawing.Point(9, 29);
		this._jointDebutPrincipalVisu.Name = "ccPrincipalD";
		this._jointDebutPrincipalVisu.Size = new System.Drawing.Size(67, 16);
		this._jointDebutPrincipalVisu.TabIndex = 37;
		this._jointDebutPrincipalVisu.Texte = "Principal";
		this._jointFinPrincipalVisu.Checked = false;
		this._jointFinPrincipalVisu.Location = new System.Drawing.Point(225, 29);
		this._jointFinPrincipalVisu.Name = "ccPrincipalF";
		this._jointFinPrincipalVisu.Size = new System.Drawing.Size(67, 16);
		this._jointFinPrincipalVisu.TabIndex = 39;
		this._jointFinPrincipalVisu.Texte = "Principal";
		this._pkEditor.BackColor = System.Drawing.Color.White;
		this._pkEditor.Couleur = couleur;
		this._pkEditor.Location = new System.Drawing.Point(133, 89);
		this._pkEditor.Name = "_pkEditor";
		this._pkEditor.Padding = new System.Windows.Forms.Padding(1);
		this._pkEditor.SelectionLength = 0;
		this._pkEditor.SelectionStart = 0;
		this._pkEditor.Size = new System.Drawing.Size(50, 20);
		this._pkEditor.TabIndex = 40;
		this._pkEditor.TextAlign = System.Windows.Forms.HorizontalAlignment.Left;
		this.BackColor = System.Drawing.Color.White;
		base.Controls.Add(this._pkVisu);
		base.Controls.Add(this._pkEditor);
		base.Controls.Add(this._jointFinPrincipalVisu);
		base.Controls.Add(this._jointDebutPrincipalVisu);
		base.Controls.Add(this.lTextPk);
		base.Controls.Add(this._demiPasAvalVisu);
		base.Controls.Add(this._demiPasAmontVisu);
		base.Controls.Add(this.lCdvF);
		base.Controls.Add(this.lCdvD);
		base.Controls.Add(this.separator2);
		base.Controls.Add(this.lTextDemiPasF);
		base.Controls.Add(this._demiPasAvalEditor);
		base.Controls.Add(this._dbAvalEditor);
		base.Controls.Add(this._dbFinVisu);
		base.Controls.Add(this._emetteurAvalEditor);
		base.Controls.Add(this._emetteurFinVisu);
		base.Controls.Add(this._demiLongueurVisu);
		base.Controls.Add(this.lTextDemiPasD);
		base.Controls.Add(this._demiPasAmontEditor);
		base.Controls.Add(this._dbAmontEditor);
		base.Controls.Add(this._dbDebutVisu);
		base.Controls.Add(this._emetteurAmontEditor);
		base.Controls.Add(this._emetteurDebutVisu);
		base.Controls.Add(this.separator1);
		base.Controls.Add(this.lTextDemiLongueur);
		base.Controls.Add(this._typeSelector);
		base.Controls.Add(this._typeLabel);
		base.Controls.Add(this.lNom);
		base.Controls.Add(this._demiLongueurEditor);
		base.Name = "JointPopupForm";
		base.Size = new System.Drawing.Size(316, 123);
		base.ResumeLayout(false);
		base.PerformLayout();
	}

	private void JointTypeChanged()
	{
		switch ((JointType)Enum.Parse(typeof(JointType), _typeSelector.Text))
		{
		case JointType.CC:
			_demiLongueurEditor.Text = "15";
			break;
		case JointType.JI:
			_demiLongueurEditor.Text = "0";
			break;
		case JointType.SV:
			_demiLongueurEditor.Text = "12";
			break;
		case JointType.SVAC:
			_demiLongueurEditor.Text = "10";
			break;
		default:
			_typeSelector.Text = _joint.Type.ToString();
			break;
		}
	}

	protected override void OnClosing(PopupFormResultEventArgs e)
	{
		if (e.Result != PopupContainerResult.OK)
		{
			return;
		}
		if (!Pk.HasValue)
		{
			Dialogs.Message("Pk incorrecte");
			e.Canceled = true;
			return;
		}
		if (!DemiLongueur.HasValue)
		{
			Dialogs.Message("Demi-longueur incorrecte");
			e.Canceled = true;
			return;
		}
		if (_joint.DemiJointAmont != null && !DemiPasAmont.HasValue)
		{
			Dialogs.Message("Demi-pas amont incorrect");
			e.Canceled = true;
			return;
		}
		if (_joint.DemiJointAval != null && !DemiPasAval.HasValue)
		{
			Dialogs.Message("Demi-pas aval incorrect");
			e.Canceled = true;
			return;
		}
		JointType type = (JointType)Enum.Parse(typeof(JointType), _typeSelector.Text);
		_joint.Type = type;
		_joint.DemiLongueur = DemiLongueur.Value;
		if (_pkEditor.Visible && Chaines.TryParsePk(_pkEditor.Text, out var PK) && _joint.PK != PK)
		{
			_joint.PK = PK;
			PkChanged = true;
		}
		if (_joint.DemiJointAmont != null)
		{
			_joint.DemiJointAmont.DB = _dbAmontEditor.Checked;
			_joint.DemiJointAmont.Emetteur = _emetteurAmontEditor.Checked;
			_joint.DemiJointAmont.DemiPas = DemiPasAmont.Value;
		}
		if (_joint.DemiJointAval != null)
		{
			_joint.DemiJointAval.DB = _dbAvalEditor.Checked;
			_joint.DemiJointAval.Emetteur = _emetteurAvalEditor.Checked;
			_joint.DemiJointAval.DemiPas = DemiPasAval.Value;
		}
	}
}
