using System.Drawing;
using System.Windows.Forms;
using CDV_Viewer.Controls;
using CDV_Viewer.CsvBase;
using CDV_Viewer.Data.SIGObjects;
using CDV_Viewer.Styles;

namespace CDV_Viewer.PopupForms;

public class CircuitPopupForm : PopupForm
{
	private Label label1;

	private Label lType;

	private Label lFrequence;

	private Label label3;

	private GroupBox gbCompensation;

	private Label lTypeCompensation;

	private Label lTextTypeCompensation;

	private Label lPas;

	private Label lTextPas;

	private Label lNbPtsCompensation;

	private Label lTextNbPointsCompensation;

	private Label lICC;

	private Label label6;

	private Label lDiaphonie;

	private Label label9;

	private Label lIFuite;

	private Label label11;

	private SIGModeleViewer modeleViewer;

	private Label lLongueur;

	private Label label4;

	private Label lN_FUITE_LONG_ARR;

	private Label label5;

	private Label lCALCUL_CONFORME;

	private Label label7;

	private Label lNom;

	public CircuitPopupForm(SIGCircuit circuit)
	{
		InitializeComponent();
		lNom.Text = "CIRCUIT DE VOIE " + circuit.Nom;
		lNom.ForeColor = Colors.GetColor("CDV" + circuit.Frequence);
		lType.Text = circuit.Type.ToString();
		lFrequence.Text = circuit.Frequence.ToString();
		lLongueur.Text = circuit.GetLongueur() + "m";
		if (circuit.Compensation == CompensationType.NON)
		{
			lTypeCompensation.Visible = (lTextNbPointsCompensation.Visible = (lNbPtsCompensation.Visible = (lTextPas.Visible = (lPas.Visible = false))));
			lTextTypeCompensation.Text = "Aucune";
			lTextTypeCompensation.ForeColor = Color.Gray;
			lTextTypeCompensation.AutoSize = false;
			lTextTypeCompensation.Width = gbCompensation.Width - 14;
			lTextTypeCompensation.TextAlign = ContentAlignment.MiddleCenter;
		}
		else
		{
			lTypeCompensation.Text = circuit.Compensation.ToString();
			lNbPtsCompensation.Text = circuit.NbPtsCompensation.ToString();
			lPas.Text = circuit.PasReel.ToString();
		}
		lIFuite.Text = circuit.IFuite.ToString();
		lDiaphonie.Text = circuit.Diaphonie.ToString();
		lICC.Text = circuit.ICC.ToString();
		lN_FUITE_LONG_ARR.Text = circuit.N_FUITE_LONG_ARR.ToString();
		lCALCUL_CONFORME.Text = (circuit.CALCUL_CONFORME ? "Oui" : "Non");
		SIGModele sIGModele = ((!ComposantsViewer.PkCroissant) ? circuit.Modeles.Find((SIGModele modele) => modele.DemiJointS == circuit.DemiJointDebut && modele.DemiJointE == circuit.DemiJointFin) : circuit.Modeles.Find((SIGModele modele) => modele.DemiJointE == circuit.DemiJointDebut && modele.DemiJointS == circuit.DemiJointFin));
		if (sIGModele == null && circuit.Modeles.Count > 0)
		{
			sIGModele = circuit.Modeles[0];
		}
		if (sIGModele == null)
		{
			base.Height = modeleViewer.Top - 5;
		}
		modeleViewer.Modele = sIGModele;
	}

	private void InitializeComponent()
	{
		this.lICC = new System.Windows.Forms.Label();
		this.label6 = new System.Windows.Forms.Label();
		this.lDiaphonie = new System.Windows.Forms.Label();
		this.label9 = new System.Windows.Forms.Label();
		this.lIFuite = new System.Windows.Forms.Label();
		this.label11 = new System.Windows.Forms.Label();
		this.gbCompensation = new System.Windows.Forms.GroupBox();
		this.lPas = new System.Windows.Forms.Label();
		this.lTextPas = new System.Windows.Forms.Label();
		this.lNbPtsCompensation = new System.Windows.Forms.Label();
		this.lTextNbPointsCompensation = new System.Windows.Forms.Label();
		this.lTypeCompensation = new System.Windows.Forms.Label();
		this.lTextTypeCompensation = new System.Windows.Forms.Label();
		this.lFrequence = new System.Windows.Forms.Label();
		this.label3 = new System.Windows.Forms.Label();
		this.lType = new System.Windows.Forms.Label();
		this.label1 = new System.Windows.Forms.Label();
		this.lNom = new System.Windows.Forms.Label();
		this.modeleViewer = new CDV_Viewer.Controls.SIGModeleViewer();
		this.lLongueur = new System.Windows.Forms.Label();
		this.label4 = new System.Windows.Forms.Label();
		this.lN_FUITE_LONG_ARR = new System.Windows.Forms.Label();
		this.label5 = new System.Windows.Forms.Label();
		this.lCALCUL_CONFORME = new System.Windows.Forms.Label();
		this.label7 = new System.Windows.Forms.Label();
		this.gbCompensation.SuspendLayout();
		base.SuspendLayout();
		this.lICC.AutoSize = true;
		this.lICC.Location = new System.Drawing.Point(59, 99);
		this.lICC.Name = "lICC";
		this.lICC.Size = new System.Drawing.Size(13, 13);
		this.lICC.TabIndex = 24;
		this.lICC.Text = "0";
		this.label6.AutoSize = true;
		this.label6.Location = new System.Drawing.Point(10, 99);
		this.label6.Name = "label6";
		this.label6.Size = new System.Drawing.Size(50, 13);
		this.label6.TabIndex = 23;
		this.label6.Text = "ICC Min :";
		this.lDiaphonie.AutoSize = true;
		this.lDiaphonie.Location = new System.Drawing.Point(356, 99);
		this.lDiaphonie.Name = "lDiaphonie";
		this.lDiaphonie.Size = new System.Drawing.Size(13, 13);
		this.lDiaphonie.TabIndex = 22;
		this.lDiaphonie.Text = "0";
		this.label9.AutoSize = true;
		this.label9.Location = new System.Drawing.Point(272, 99);
		this.label9.Name = "label9";
		this.label9.Size = new System.Drawing.Size(84, 13);
		this.label9.TabIndex = 21;
		this.label9.Text = "Diaphonie Max :";
		this.lIFuite.AutoSize = true;
		this.lIFuite.Location = new System.Drawing.Point(213, 99);
		this.lIFuite.Name = "lIFuite";
		this.lIFuite.Size = new System.Drawing.Size(13, 13);
		this.lIFuite.TabIndex = 20;
		this.lIFuite.Text = "0";
		this.label11.AutoSize = true;
		this.label11.Location = new System.Drawing.Point(151, 99);
		this.label11.Name = "label11";
		this.label11.Size = new System.Drawing.Size(62, 13);
		this.label11.TabIndex = 19;
		this.label11.Text = "IFuite Max :";
		this.gbCompensation.Controls.Add(this.lPas);
		this.gbCompensation.Controls.Add(this.lTextPas);
		this.gbCompensation.Controls.Add(this.lNbPtsCompensation);
		this.gbCompensation.Controls.Add(this.lTextNbPointsCompensation);
		this.gbCompensation.Controls.Add(this.lTypeCompensation);
		this.gbCompensation.Controls.Add(this.lTextTypeCompensation);
		this.gbCompensation.Location = new System.Drawing.Point(3, 52);
		this.gbCompensation.Name = "gbCompensation";
		this.gbCompensation.Size = new System.Drawing.Size(379, 41);
		this.gbCompensation.TabIndex = 15;
		this.gbCompensation.TabStop = false;
		this.gbCompensation.Text = "Compensation";
		this.lPas.AutoSize = true;
		this.lPas.Location = new System.Drawing.Point(337, 18);
		this.lPas.Name = "lPas";
		this.lPas.Size = new System.Drawing.Size(13, 13);
		this.lPas.TabIndex = 18;
		this.lPas.Text = "0";
		this.lTextPas.AutoSize = true;
		this.lTextPas.Location = new System.Drawing.Point(306, 18);
		this.lTextPas.Name = "lTextPas";
		this.lTextPas.Size = new System.Drawing.Size(31, 13);
		this.lTextPas.TabIndex = 17;
		this.lTextPas.Text = "Pas :";
		this.lNbPtsCompensation.AutoSize = true;
		this.lNbPtsCompensation.Location = new System.Drawing.Point(252, 18);
		this.lNbPtsCompensation.Name = "lNbPtsCompensation";
		this.lNbPtsCompensation.Size = new System.Drawing.Size(13, 13);
		this.lNbPtsCompensation.TabIndex = 16;
		this.lNbPtsCompensation.Text = "0";
		this.lTextNbPointsCompensation.AutoSize = true;
		this.lTextNbPointsCompensation.Location = new System.Drawing.Point(125, 18);
		this.lTextNbPointsCompensation.Name = "lTextNbPointsCompensation";
		this.lTextNbPointsCompensation.Size = new System.Drawing.Size(126, 13);
		this.lTextNbPointsCompensation.TabIndex = 15;
		this.lTextNbPointsCompensation.Text = "Points de compensation :";
		this.lTypeCompensation.AutoSize = true;
		this.lTypeCompensation.Location = new System.Drawing.Point(43, 18);
		this.lTypeCompensation.Name = "lTypeCompensation";
		this.lTypeCompensation.Size = new System.Drawing.Size(31, 13);
		this.lTypeCompensation.TabIndex = 14;
		this.lTypeCompensation.Text = "Type";
		this.lTextTypeCompensation.AutoSize = true;
		this.lTextTypeCompensation.Location = new System.Drawing.Point(7, 18);
		this.lTextTypeCompensation.Name = "lTextTypeCompensation";
		this.lTextTypeCompensation.Size = new System.Drawing.Size(37, 13);
		this.lTextTypeCompensation.TabIndex = 13;
		this.lTextTypeCompensation.Text = "Type :";
		this.lFrequence.AutoSize = true;
		this.lFrequence.Location = new System.Drawing.Point(221, 31);
		this.lFrequence.Name = "lFrequence";
		this.lFrequence.Size = new System.Drawing.Size(13, 13);
		this.lFrequence.TabIndex = 14;
		this.lFrequence.Text = "0";
		this.label3.AutoSize = true;
		this.label3.Location = new System.Drawing.Point(154, 31);
		this.label3.Name = "label3";
		this.label3.Size = new System.Drawing.Size(64, 13);
		this.label3.TabIndex = 13;
		this.label3.Text = "Fréquence :";
		this.lType.AutoSize = true;
		this.lType.Location = new System.Drawing.Point(71, 31);
		this.lType.Name = "lType";
		this.lType.Size = new System.Drawing.Size(22, 13);
		this.lType.TabIndex = 12;
		this.lType.Text = "NC";
		this.label1.AutoSize = true;
		this.label1.Location = new System.Drawing.Point(31, 31);
		this.label1.Name = "label1";
		this.label1.Size = new System.Drawing.Size(37, 13);
		this.label1.TabIndex = 11;
		this.label1.Text = "Type :";
		this.lNom.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.lNom.Font = new System.Drawing.Font("Arial", 9.75f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		this.lNom.ForeColor = System.Drawing.Color.FromArgb(224, 82, 6);
		this.lNom.Location = new System.Drawing.Point(3, 4);
		this.lNom.Name = "lNom";
		this.lNom.Size = new System.Drawing.Size(379, 16);
		this.lNom.TabIndex = 10;
		this.lNom.Text = "NOM";
		this.lNom.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
		this.modeleViewer.Location = new System.Drawing.Point(3, 146);
		this.modeleViewer.MinimumSize = new System.Drawing.Size(50, 50);
		this.modeleViewer.Modele = null;
		this.modeleViewer.Name = "modeleViewer";
		this.modeleViewer.Size = new System.Drawing.Size(379, 151);
		this.modeleViewer.Statique = true;
		this.modeleViewer.TabIndex = 25;
		this.lLongueur.AutoSize = true;
		this.lLongueur.Location = new System.Drawing.Point(339, 31);
		this.lLongueur.Name = "lLongueur";
		this.lLongueur.Size = new System.Drawing.Size(13, 13);
		this.lLongueur.TabIndex = 27;
		this.lLongueur.Text = "0";
		this.label4.AutoSize = true;
		this.label4.Location = new System.Drawing.Point(281, 31);
		this.label4.Name = "label4";
		this.label4.Size = new System.Drawing.Size(58, 13);
		this.label4.TabIndex = 26;
		this.label4.Text = "Longueur :";
		this.lN_FUITE_LONG_ARR.AutoSize = true;
		this.lN_FUITE_LONG_ARR.Location = new System.Drawing.Point(80, 121);
		this.lN_FUITE_LONG_ARR.Name = "lN_FUITE_LONG_ARR";
		this.lN_FUITE_LONG_ARR.Size = new System.Drawing.Size(13, 13);
		this.lN_FUITE_LONG_ARR.TabIndex = 29;
		this.lN_FUITE_LONG_ARR.Text = "0";
		this.label5.AutoSize = true;
		this.label5.Location = new System.Drawing.Point(10, 121);
		this.label5.Name = "label5";
		this.label5.Size = new System.Drawing.Size(69, 13);
		this.label5.TabIndex = 28;
		this.label5.Text = "N Fuite Long. Arr:";
		this.lCALCUL_CONFORME.AutoSize = true;
		this.lCALCUL_CONFORME.Location = new System.Drawing.Point(213, 121);
		this.lCALCUL_CONFORME.Name = "lCALCUL_CONFORME";
		this.lCALCUL_CONFORME.Size = new System.Drawing.Size(13, 13);
		this.lCALCUL_CONFORME.TabIndex = 31;
		this.lCALCUL_CONFORME.Text = "0";
		this.label7.AutoSize = true;
		this.label7.Location = new System.Drawing.Point(149, 121);
		this.label7.Name = "label7";
		this.label7.Size = new System.Drawing.Size(64, 13);
		this.label7.TabIndex = 30;
		this.label7.Text = "Calcul Conf:";
		this.BackColor = System.Drawing.Color.White;
		base.Controls.Add(this.lCALCUL_CONFORME);
		base.Controls.Add(this.label7);
		base.Controls.Add(this.lN_FUITE_LONG_ARR);
		base.Controls.Add(this.label5);
		base.Controls.Add(this.lLongueur);
		base.Controls.Add(this.label4);
		base.Controls.Add(this.modeleViewer);
		base.Controls.Add(this.lICC);
		base.Controls.Add(this.label6);
		base.Controls.Add(this.lDiaphonie);
		base.Controls.Add(this.label9);
		base.Controls.Add(this.lIFuite);
		base.Controls.Add(this.label11);
		base.Controls.Add(this.gbCompensation);
		base.Controls.Add(this.lFrequence);
		base.Controls.Add(this.label3);
		base.Controls.Add(this.lType);
		base.Controls.Add(this.label1);
		base.Controls.Add(this.lNom);
		base.Name = "CircuitPopupForm";
		base.Size = new System.Drawing.Size(385, 300);
		this.gbCompensation.ResumeLayout(false);
		this.gbCompensation.PerformLayout();
		base.ResumeLayout(false);
		base.PerformLayout();
	}
}
