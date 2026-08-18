using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using CDV_Viewer.Composants;
using CDV_Viewer.Controls;
using CDV_Viewer.CsvBase;
using CDV_Viewer.Data;
using CDV_Viewer.Data.SIGObjects;
using CDV_Viewer.Forms;
using CDV_Viewer.PopupForms;
using CDV_Viewer.Properties;
using CDV_Viewer.Traitements;

namespace CDV_Viewer.DockControls;

public class ComposantsViewer : DockChild
{
	public static ComposantsViewer Viewer;

	private int _graphWidth;

	private int _graphHeight;

	private int _graphOffsetX;

	private int _graphOffsetY;

	private SIGLigne _ligne;

	private bool _sensPK = true;

	private int _pkD;

	private int _pkF;

	private int _pkDWhenReady = int.MinValue;

	private int _pkFWhenReady = int.MinValue;

	private int _nbVoiesDiplay = Global.DefaultNbVoies;

	private ComposantViewerState _state;

	private ModeVisualisation _modeVisualisation = ModeVisualisation.Signalisation;

	private bool _showInfobulle = true;

	private bool _showEmetteurs = true;

	private ComposantsCollection _composants;

	private Composant _lightComposant;

	private int _lightPk;

	private bool _isZooming;

	private Point _mouseMoveLocation;

	private int _mouseZoom = -1;

	private bool _mouseMove;

	private int _mousePkInit;

	private int _mouseVitesse;

	private int _mouseXPrec;

	private bool _stopMove;

	private Process _locNG;

	private Point _mouseClickLocation;

	private bool _fromInvalidate;

	private int[] _savedgraphLimits = new int[8];

	private IContainer components;

	private CVEchelle echelle;

	private Label lIntro;

	private PictureBox pictureBox1;

	private CVScrollBar hScrollBar;

	private CVLegende legende;

	private CVRecherche cvRecherche;

	private Label lNomLigne;

	public PopupContainer PopupContainer { get; }

	public CVInfoBulle InfoBulle { get; }

	public CVLegende Legende => legende;

	public CVScrollBar ScrollBar => hScrollBar;

	public int GraphWidth => _graphWidth;

	public int GraphHeight => _graphHeight;

	public int GraphOffsetX => _graphOffsetX;

	public int GraphOffsetY => _graphOffsetY;

	public SIGLigne Ligne => _ligne;

	public int LigneId => _ligne?.ID ?? 0;

	public int PkDLigne { get; private set; }

	public int PkFLigne { get; private set; }

	public int LongueurLigne => PkFLigne - PkDLigne;

	public int CurrentMargin { get; private set; }

	public bool PkCroissant
	{
		get
		{
			return _sensPK;
		}
		set
		{
			if (_sensPK != value)
			{
				_sensPK = value;
				Recadrer();
				this.SensPkChanged?.Invoke(null, new EventArgs());
			}
		}
	}

	public int PosVoieD { get; private set; }

	public int PosVoieF { get; private set; }

	public int PkD => _pkD;

	public int PkF => _pkF;

	public int PkWidth => _pkF - _pkD;

	public int RoundedScaleX => Math.Abs(_pkF - _pkD) / base.Width;

	public double ScaleX => (double)(_pkF - _pkD) / (double)base.Width;

	public ComposantViewerState State => _state;

	public ModeVisualisation ModeVisualisation
	{
		get
		{
			return _modeVisualisation;
		}
		set
		{
			if (_modeVisualisation != value)
			{
				_modeVisualisation = value;
				legende.Refresh();
				Invalidate();
				this.ModeVisualisationChanged?.Invoke(this, new EventArgs());
			}
		}
	}

	public bool ShowInfobulle
	{
		get
		{
			return _showInfobulle;
		}
		set
		{
			if (_showInfobulle != value)
			{
				_showInfobulle = value;
				this.ShowInfobulleChanged?.Invoke(this, new EventArgs());
			}
		}
	}

	public bool ShowEmetteurs
	{
		get
		{
			return _showEmetteurs;
		}
		set
		{
			if (_showEmetteurs != value)
			{
				_showEmetteurs = value;
				Refresh();
				this.ShowEmetteursChanged?.Invoke(this, new EventArgs());
			}
		}
	}

	public ComposantsCollection Composants => _composants;

	private int _xMouse => _mouseMoveLocation.X;

	public bool MustDrawComponent { get; set; }

	public event EventHandler SelectedLigneChanging;

	public event EventHandler SelectedLigneChanged;

	public event EventHandler PkChanged;

	public event EventHandler SensPkChanged;

	public event EventHandler StateChanged;

	public event EventHandler ModeVisualisationChanged;

	public event EventHandler ShowInfobulleChanged;

	public event EventHandler ShowEmetteursChanged;

	public void Invoke(Action action)
	{
		Invoke((Delegate)action);
	}

	public void ToogleModeVisualisation()
	{
		if (_modeVisualisation == ModeVisualisation.Topologie)
		{
			ModeVisualisation = ModeVisualisation.Signalisation;
		}
		else
		{
			ModeVisualisation = ModeVisualisation.Topologie;
		}
	}

	public ComposantsViewer()
	{
		Viewer = this;
		InitializeComponent();
		cvRecherche.Visible = Archives.CurrentArchive != null;
		base.DockPosition = DockStyle.Fill;
		legende.Visible = Preferences.Affichage.Legende;
		_showInfobulle = Preferences.Affichage.InfoBulles;
		_modeVisualisation = Preferences.Affichage.ModeVisualisation;
		_graphWidth = base.Width;
		PopupContainer = new PopupContainer();
		base.Controls.Add(PopupContainer);
		PopupContainer.BringToFront();
		InfoBulle = new CVInfoBulle();
		base.Controls.Add(InfoBulle);
		InfoBulle.BringToFront();
		OnStateChanged(EventArgs.Empty);
		legende.SizeChanged += Legende_SizeChanged;
		legende.VisibleChanged += Legende_SizeChanged;
		Archives.CurrentArchiveChanged += Archives_CurrentArchiveChanged;
		_composants = new ComposantsCollection(this);
	}

	protected override void OnInvalidated(InvalidateEventArgs e)
	{
		_fromInvalidate = true;
		MustDrawComponent = true;
	}

	public void SaveLimits()
	{
		_savedgraphLimits = new int[8] { GraphWidth, GraphHeight, GraphOffsetX, GraphOffsetY, PkD, PkF, PosVoieD, PosVoieF };
	}

	public void RestoreLimits()
	{
		_graphWidth = _savedgraphLimits[0];
		_graphHeight = _savedgraphLimits[1];
		_graphOffsetX = _savedgraphLimits[2];
		_graphOffsetY = _savedgraphLimits[3];
		_pkD = _savedgraphLimits[4];
		_pkF = _savedgraphLimits[5];
		PosVoieD = _savedgraphLimits[6];
		PosVoieF = _savedgraphLimits[7];
	}

	public void SetLimits(int graphWidth, int graphHeight, int graphOffsetX, int graphOffsetY, int pkD, int pkF, int posVoieDebut, int posVoieFin)
	{
		_graphWidth = graphWidth;
		_graphHeight = graphHeight;
		_graphOffsetX = graphOffsetX;
		_graphOffsetY = graphOffsetY;
		_pkD = pkD;
		_pkF = pkF;
		PosVoieD = posVoieDebut;
		PosVoieF = posVoieFin;
	}

	public void Clear()
	{
		_ligne = null;
		int pkDLigne = (PkFLigne = (_pkD = (_pkF = 0)));
		PkDLigne = pkDLigne;
		_composants.Clear();
		PopupContainer.State = PopupState.Hidden;
		Invalidate();
		SetState(ComposantViewerState.Empty);
	}

	public void SetLigne(int newLigne)
	{
		if (_state != ComposantViewerState.Loading && newLigne != LigneId)
		{
			Focus();
			PopupContainer.State = PopupState.Hidden;
			LoadLigne(newLigne);
			this.SelectedLigneChanging?.Invoke(this, EventArgs.Empty);
		}
	}

	public void SetLignePK(int newLigneId, int pk)
	{
		if (newLigneId != LigneId)
		{
			SetLigne(newLigneId);
			_pkDWhenReady = pk - PkWidth / 2;
			_pkFWhenReady = pk + PkWidth / 2;
		}
		else
		{
			MoveToPkCenter(pk, forceRedraw: true);
		}
	}

	public void UnlightComposant()
	{
		if (_lightComposant != null)
		{
			_lightComposant = null;
			MustDrawComponent = true;
			base.ParentForm.Refresh();
		}
	}

	public void LightCircuit(SIGCircuit circuit)
	{
		CCircuit circuit2 = _composants.GetCircuit(circuit);
		if (circuit2 != null)
		{
			_lightComposant = circuit2;
			int num = circuit2.Circuit.PKDebut;
			if (circuit2.Circuit.DemiJointDebut == null || circuit2.Circuit.DemiJointDebut.Joint.Voie.Ligne != Ligne)
			{
				num = circuit2.Circuit.PKFin - 100;
			}
			int pk = num + 100;
			if (circuit2.Circuit.DemiJointFin != null && circuit2.Circuit.DemiJointFin.Joint.Voie.Ligne == Ligne)
			{
				pk = circuit2.Circuit.PKFin;
			}
			MoveToPkCenter(num, pk, forceMove: true);
		}
	}

	public void LightVoie(SIGVoie voie)
	{
		CVoie cVoie = (CVoie)(_lightComposant = _composants.GetVoie(voie));
		if (cVoie != null)
		{
			if (cVoie is CVoieAdjacente)
			{
				_lightPk = ((CVoieAdjacente)cVoie).Pk;
				MoveToPkCenter(_lightPk, forceRedraw: true);
			}
			else
			{
				_lightPk = (((CVoieOnLine)cVoie).Voie.PKDebut + ((CVoieOnLine)cVoie).Voie.PKFin) / 2;
				MoveToPkCenter(_lightPk, forceRedraw: true);
			}
		}
	}

	public void LightNoeud(SIGNoeud noeud)
	{
		CVoieAdjacente voieAdjacente = _composants.GetVoieAdjacente(noeud);
		if (voieAdjacente != null)
		{
			_lightPk = voieAdjacente.Pk;
			MoveToPkCenter(_lightPk, forceRedraw: true);
			_lightComposant = voieAdjacente;
		}
		_lightComposant = Composants.GetNoeud(noeud);
	}

	public void LightVoie(SIGVoie voie, int pk)
	{
		CVoie voie2 = _composants.GetVoie(voie);
		if (voie2 != null)
		{
			MoveToPkCenter(pk, forceRedraw: true);
		}
		_lightPk = pk;
		_lightComposant = voie2;
	}

	public void RefreshLigne()
	{
		Base.Link();
		if (Dialogs.BaseLinkError)
		{
			SetLignePK(Dialogs.FirstErrorVoie.Ligne.ID, Dialogs.FirstErrorPk);
			LightVoie(Dialogs.FirstErrorVoie, Dialogs.FirstErrorPk);
		}
		else if (_ligne != null)
		{
			_pkDWhenReady = _pkD;
			_pkFWhenReady = _pkF;
			PopupContainer.State = PopupState.Hidden;
			LoadLigne(_ligne.ID);
		}
	}

	private void InitGraph()
	{
		PkDLigne = _ligne.PKDebut;
		PkFLigne = _ligne.PKFin;
		_pkD = PkDLigne;
		_pkF = _pkD + Global.DefaultEchelleX;
		Recadrer();
	}

	private void LoadLigne(int ligne)
	{
		SetState(ComposantViewerState.Loading);
		ThreadLoadLigne(ligne);
	}

	private void ThreadLoadLigne(int ligne)
	{
		_composants.Clear();
		_ligne = Base.GetLigne(ligne);
		InitGraph();
		if (LigneId >= 0)
		{
			LoadTopologie();
			LoadSignalisation();
			Invoke(FinChargement);
		}
	}

	private void LoadTopologie()
	{
		int num = _ligne.Voies.Max((SIGVoie v) => Math.Abs(v.PositionY));
		PosVoieD = -(num + 1);
		PosVoieF = num + 1;
		foreach (SIGVoie voie in _ligne.Voies)
		{
			if (!_composants.AddVoieOnLine(voie, out var cvoie))
			{
				continue;
			}
			foreach (SIGNoeud noeud in voie.Noeuds)
			{
				if (noeud.IsSautPk)
				{
					_composants.AddSautPk(cvoie, noeud);
					continue;
				}
				_composants.AddNoeud(cvoie, noeud);
				foreach (SIGBranche item in noeud.BranchesInOtherLine(voie.Ligne))
				{
					_composants.AddVoieAdjacente(item);
				}
			}
			foreach (SIGBalise balise in voie.Balises)
			{
				_composants.AddBalise(cvoie, balise);
			}
		}
		_composants.OnTopologieLoaded();
	}

	private void LoadSignalisation()
	{
		foreach (CVoieOnLine voiesOnLine in _composants.VoiesOnLines)
		{
			foreach (SIGJoint joint in voiesOnLine.Voie.Joints)
			{
				_composants.AddJoint(voiesOnLine, joint);
			}
		}
		foreach (CVoieAdjacente voiesAdjacente in _composants.VoiesAdjacentes)
		{
			if (voiesAdjacente.Voie.Joints.Count == 0)
			{
				continue;
			}
			foreach (SIGBranche item in voiesAdjacente.Noeud.BranchesInTrack(voiesAdjacente.Voie))
			{
				SIGJoint sIGJoint = null;
				sIGJoint = ((!item.IsAmont) ? (item.IsBrancheFin ? voiesAdjacente.Voie.JointBefore(item.PK) : voiesAdjacente.Voie.JointAfter(item.PK)) : (item.IsBrancheDebut ? voiesAdjacente.Voie.JointAfter(item.PK) : voiesAdjacente.Voie.JointBefore(item.PK)));
				_composants.AddJoint(voiesAdjacente, sIGJoint);
			}
		}
		foreach (SIGCircuit circuit in _ligne.Circuits)
		{
			_composants.AddCircuit(circuit);
		}
	}

	private void FinChargement()
	{
		lNomLigne.Text = _ligne.ID + " : " + _ligne.Nom;
		hScrollBar.Refresh(_composants);
		legende.Refresh();
		SetState(ComposantViewerState.Loaded);
		this.SelectedLigneChanged?.Invoke(this, EventArgs.Empty);
		if (_ligne.Voies.Count == 0)
		{
			if (Dialogs.Confirm("Cette ligne ne contient aucune voie. Veuillez en créer une."))
			{
				EditNomPksPopupForm editNomPksPopupForm = new EditNomPksPopupForm
				{
					PkD = 0,
					PkF = 100,
					Nom = "V???"
				};
				editNomPksPopupForm.Closing += AjoutVoiePopupForm_Closing;
				PopupContainer.Show(editNomPksPopupForm, "Nouvelle Voie", PopupContainerButtons.Valider);
			}
		}
		else if (_composants.Count == 0)
		{
			MessageBox.Show("Aucune voie de cette ligne n'est positionnée. Veuillez en positionner au moins une", Resources.APP_NAME);
			new PositionVoiesDialog().Show();
		}
		if (_pkDWhenReady > int.MinValue && _pkFWhenReady > int.MinValue)
		{
			_pkD = _pkDWhenReady;
			_pkF = _pkFWhenReady;
		}
		MoveToPkCenter(_pkD, _pkF, forceMove: true);
		_pkDWhenReady = int.MinValue;
		_pkFWhenReady = int.MinValue;
	}

	private void SetState(ComposantViewerState newState)
	{
		if (_state != newState)
		{
			_state = newState;
			if (_state != ComposantViewerState.Displayed)
			{
				OnStateChanged(EventArgs.Empty);
			}
		}
	}

	private void Recadrer()
	{
		int pkWidth = PkWidth;
		double num = (double)pkWidth / (double)base.Width;
		int num2 = (CurrentMargin = (int)(100.0 * num));
		int num4 = PkDLigne - num2;
		int num5 = PkFLigne + num2;
		int num6 = Math.Max(num4, _pkD);
		int num7 = Math.Min(num5, _pkF);
		if (num6 == num4)
		{
			num7 = Math.Min(num6 + pkWidth, num5);
		}
		if (num7 == num5)
		{
			num6 = Math.Max(num7 - pkWidth, num4);
		}
		_pkD = num6;
		_pkF = num7;
		hScrollBar.Invalidate();
		echelle.Invalidate();
		Invalidate();
	}

	public int PkToLocation(int pk)
	{
		if (PkCroissant)
		{
			return (int)(((double)pk - (double)_pkD) * (double)base.Width) / (_pkF - _pkD);
		}
		return (int)(((double)_pkF - (double)pk) * (double)base.Width) / (_pkF - _pkD);
	}

	public int LocationToPk(int xLocation)
	{
		if (PkCroissant)
		{
			return _pkD + xLocation * (_pkF - _pkD) / base.Width;
		}
		return _pkF - xLocation * (_pkF - _pkD) / base.Width;
	}

	public int WidthToDistance(double w)
	{
		return (int)(w * (double)(_pkF - _pkD) / (double)base.Width);
	}

	public int DistanceToWidth(double d)
	{
		return (int)(d * (double)base.Width / (double)(_pkF - _pkD));
	}

	public int LocationToPosY(int yLocation)
	{
		return PosVoieD + (int)Math.Round((double)((yLocation - GraphOffsetY) * (PosVoieF - PosVoieD)) / (double)GraphHeight);
	}

	public int PosyToLocation(float positionVoie)
	{
		return GraphOffsetY + (int)Math.Round((positionVoie - (float)PosVoieD) * (float)GraphHeight / (float)(PosVoieF - PosVoieD));
	}

	public int VitesseToOffset(int vitesse)
	{
		if (vitesse <= 0)
		{
			return 0;
		}
		return (int)Math.Round((double)(_pkF - _pkD) * ((double)vitesse / 10.0));
	}

	public void ZoomIn()
	{
		ZoomIn(5, (_pkF + _pkD) / 2);
	}

	public void ZoomOut()
	{
		ZoomOut(5, (_pkF + _pkD) / 2);
	}

	public void ZoomIn(int Vitesse, int PkMouse)
	{
		if (_state != ComposantViewerState.Empty)
		{
			Task.Factory.StartNew(delegate
			{
				Zoom(VitesseToOffset(Vitesse) / 2, PkMouse);
			});
			OnPkChanged(new EventArgs());
		}
	}

	public void ZoomOut(int Vitesse, int PkMouse)
	{
		if (_state != ComposantViewerState.Empty)
		{
			Task.Factory.StartNew(delegate
			{
				Zoom(-VitesseToOffset(Vitesse) / 2, PkMouse);
			});
			OnPkChanged(new EventArgs());
		}
	}

	public void ZoomMin()
	{
		if (_state != ComposantViewerState.Empty)
		{
			int offset = (int)((double)Math.Abs(_pkF - _pkD) - Global.ZoomMin * (double)base.Width) / 2;
			int pkMouse = (_pkF + _pkD) / 2;
			Task.Factory.StartNew(delegate
			{
				Zoom(offset, pkMouse);
			});
			OnPkChanged(new EventArgs());
		}
	}

	public void Zoom(int offset, int pkMouse)
	{
		if (!_isZooming)
		{
			double num = 2.0 * (double)(pkMouse - _pkD) / (double)(_pkF - _pkD);
			_isZooming = true;
			double num2 = (double)Math.Abs(_pkF - _pkD - 2 * offset) / (double)base.Width;
			if (num2 < Global.ZoomMax)
			{
				offset = (int)((double)Math.Abs(_pkF - _pkD) - Global.ZoomMax * (double)base.Width) / 2;
				pkMouse = (_pkF + _pkD) / 2;
			}
			if (num2 > Global.ZoomMin)
			{
				offset = (int)((double)Math.Abs(_pkF - _pkD) - Global.ZoomMin * (double)base.Width) / 2;
				pkMouse = (_pkF + _pkD) / 2;
			}
			int num3 = offset / 10;
			for (int i = 0; i < 10; i++)
			{
				_pkD += (int)((double)num3 * num);
				_pkF -= (int)((double)num3 * (2.0 - num));
				Invoke(Recadrer);
				Thread.Sleep(20);
			}
			Invalidate();
			_isZooming = false;
		}
	}

	public void MoveLeft()
	{
		MoveLeft(4);
	}

	public void MoveRight()
	{
		MoveRight(4);
	}

	public void MoveLeft(int Vitesse)
	{
		MoveToPk((_pkD + _pkF) / 2 - VitesseToOffset(Vitesse), Animation: true);
	}

	public void MoveRight(int Vitesse)
	{
		MoveToPk((_pkD + _pkF) / 2 + VitesseToOffset(Vitesse), Animation: true);
	}

	public void MoveToCDV(SIGCircuit circuit)
	{
		foreach (Composant composant in _composants)
		{
			if (composant is CCircuit && ((CCircuit)composant).Circuit == circuit)
			{
				MoveToPk((((CCircuit)composant).Circuit.PKFin + ((CCircuit)composant).Circuit.PKDebut) / 2, Animation: true);
				break;
			}
		}
	}

	public void MoveToPk(int Pk, bool Animation)
	{
		if (_state == ComposantViewerState.Empty || PopupContainer.State == PopupState.Maximized)
		{
			return;
		}
		_stopMove = true;
		if (Animation)
		{
			Thread.Sleep(5);
			Task.Factory.StartNew(delegate
			{
				ThreadMoveToPk(Pk);
			});
		}
		else
		{
			if (PkCroissant)
			{
				_pkF = Pk + PkWidth;
				_pkD = Pk;
			}
			else
			{
				_pkD = Pk + PkWidth;
				_pkF = Pk;
			}
			Recadrer();
		}
		OnPkChanged(EventArgs.Empty);
	}

	public void MoveToPkCenter(int pk1, int pk2, bool forceMove)
	{
		MoveToPkCenter((pk1 + pk2) / 2, forceMove);
	}

	public void MoveToPkCenter(int Pk, bool forceRedraw = false)
	{
		if (_state != ComposantViewerState.Empty && PopupContainer.State != PopupState.Maximized)
		{
			int pkWidth = PkWidth;
			int num = Pk - pkWidth / 2;
			int num2 = num + pkWidth;
			if (forceRedraw || num != _pkD || num2 != _pkF)
			{
				_pkD = num;
				_pkF = num2;
				Recadrer();
				OnPkChanged(EventArgs.Empty);
			}
		}
	}

	private void ThreadMoveWithVitesse(int vitesse)
	{
		int currentMargin = CurrentMargin;
		_stopMove = false;
		for (int i = 0; i < Math.Abs(vitesse); i++)
		{
			int num = PkWidth / 20 * (Math.Abs(vitesse) - i) / vitesse;
			_pkD -= num;
			_pkF -= num;
			if (_pkD < PkDLigne - currentMargin || _pkF > PkFLigne + currentMargin)
			{
				_stopMove = true;
			}
			Invoke(Recadrer);
			if (!_stopMove)
			{
				Thread.Sleep(10);
				continue;
			}
			break;
		}
	}

	private void ThreadMoveToPk(int pk)
	{
		_stopMove = false;
		pk -= PkWidth / 2;
		pk = Math.Max(pk, PkDLigne - CurrentMargin);
		pk = Math.Min(pk, PkFLigne + CurrentMargin - PkWidth);
		int num = (pk - _pkD) / 50;
		for (int i = 0; i < 50; i++)
		{
			_pkD += num;
			_pkF += num;
			Invoke(Recadrer);
			Thread.Sleep((int)(25.0 * (1.0 - Math.Sin(Math.PI * (double)i / 50.0))));
			if (_stopMove)
			{
				break;
			}
		}
	}

	protected override void OnPaintBackground(PaintEventArgs e)
	{
		base.OnPaintBackground(e);
	}

	protected override void OnPaint(PaintEventArgs e)
	{
		if ((_state == ComposantViewerState.Loaded || _state == ComposantViewerState.Displayed) && MustDrawComponent)
		{
			if (MustDrawComponent)
			{
				e.Graphics.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;
				_composants.Draw(e);
				_state = ComposantViewerState.Displayed;
				MustDrawComponent = false;
			}
			if (!DrawLightComposant(e))
			{
				DrawMouseZoom(e);
			}
		}
	}

	private void DrawMouseZoom(PaintEventArgs e)
	{
		if (_mouseZoom >= 0)
		{
			MustDrawComponent = true;
			SolidBrush brush = new SolidBrush(Color.FromArgb(160, Color.Black));
			int num = Math.Min(_mouseZoom, _xMouse);
			int num2 = Math.Max(_mouseZoom, _xMouse);
			e.Graphics.FillRectangle(brush, 0, 0, num, base.Height);
			e.Graphics.DrawLine(new Pen(Color.Black), num, 0, num, base.Height);
			e.Graphics.DrawLine(new Pen(Color.Black), num2, 0, num2, base.Height);
			e.Graphics.FillRectangle(brush, num2, 0, base.Width - num2, base.Height);
		}
	}

	private bool DrawLightComposant(PaintEventArgs e)
	{
		if (_lightComposant == null)
		{
			return false;
		}
		MustDrawComponent = true;
		e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
		SolidBrush brush = new SolidBrush(Color.FromArgb(160, Color.Black));
		Region region = new Region(DisplayRectangle);
		GraphicsPath graphicsPath = new GraphicsPath();
		Rectangle rectangle = _lightComposant.DisplayRectangle;
		if (_lightComposant is CVoie)
		{
			rectangle = new Rectangle(_lightComposant.GetPoint(_lightPk), Size.Empty);
		}
		Rectangle rect = new Rectangle(rectangle.X - 50, rectangle.Y - 50, rectangle.Width + 100, rectangle.Height + 100);
		graphicsPath.AddEllipse(rect);
		region.Exclude(graphicsPath);
		e.Graphics.FillRegion(brush, region);
		e.Graphics.DrawEllipse(Pens.Black, rect);
		e.Graphics.SmoothingMode = SmoothingMode.Default;
		return true;
	}

	protected override void OnSizeChanged(EventArgs e)
	{
		if (base.IsHandleCreated && base.Width > 50)
		{
			if (_state != ComposantViewerState.Empty)
			{
				_pkF += (base.Width - _graphWidth) * PkWidth / _graphWidth;
				Recadrer();
			}
			_graphWidth = base.Width;
			_graphHeight = base.Height - echelle.Height - legende.Height - 20 - base.Height / 6;
			_graphOffsetX = 0;
			_graphOffsetY = 20 + base.Height / 12;
			Invalidate();
			PerformLayout();
		}
	}

	protected void OnStateChanged(EventArgs e)
	{
		bool flag = _state == ComposantViewerState.Loaded;
		lIntro.Visible = !flag;
		lNomLigne.Visible = flag;
		echelle.Visible = flag;
		legende.Visible = flag;
		hScrollBar.Visible = flag;
		Cursor = (flag ? Cursors.Hand : Cursors.Default);
		switch (_state)
		{
		case ComposantViewerState.Empty:
			lIntro.Text = "SÉLECTIONNEZ UNE LIGNE...";
			PopupContainer.State = PopupState.Hidden;
			break;
		case ComposantViewerState.Loading:
			lIntro.Text = "CHARGEMENT...";
			break;
		}
		Invalidate();
		this.StateChanged?.Invoke(this, e);
	}

	protected void OnPkChanged(EventArgs e)
	{
		this.PkChanged?.Invoke(this, new EventArgs());
	}

	protected override void OnKeyDown(KeyEventArgs e)
	{
		if ((_state == ComposantViewerState.Loaded || _state == ComposantViewerState.Displayed) && e.KeyCode == Keys.ShiftKey)
		{
			Cursor = Cursors.VSplit;
		}
		switch (e.KeyCode)
		{
		case Keys.Add:
			ZoomIn();
			break;
		case Keys.Subtract:
			ZoomOut();
			break;
		case Keys.Left:
			MoveLeft();
			break;
		case Keys.Right:
			MoveRight();
			break;
		case Keys.I:
			ShowInfobulle = !ShowInfobulle;
			break;
		case Keys.L:
			legende.Visible = !legende.Visible;
			break;
		case Keys.M:
			ToogleModeVisualisation();
			break;
		case Keys.P:
			if (e.Control)
			{
				new PositionVoiesDialog().ShowDialog();
			}
			break;
		case Keys.R:
			CommandStart.Refresh();
			break;
		case Keys.X:
			ZoomMin();
			break;
		case Keys.F5:
			RefreshLigne();
			break;
		case Keys.Escape:
			_composants.RemoveAll((Composant c) => c is COperation);
			Invalidate();
			break;
		}
	}

	protected override void OnKeyUp(KeyEventArgs e)
	{
		if ((_state == ComposantViewerState.Loaded || _state == ComposantViewerState.Displayed) && e.KeyCode == Keys.ShiftKey)
		{
			Cursor = Cursors.Hand;
		}
	}

	protected override bool IsInputKey(Keys keyData)
	{
		if (keyData == Keys.Left || keyData == Keys.Right || keyData == Keys.Up || keyData == Keys.Down)
		{
			return true;
		}
		return base.IsInputKey(keyData);
	}

	protected override void OnMouseDown(MouseEventArgs e)
	{
		if (_state != ComposantViewerState.Empty && e.Button == MouseButtons.Left)
		{
			_mouseMoveLocation = e.Location;
			UnlightComposant();
			if (ES.GetStateKey(Keys.ShiftKey))
			{
				_mouseZoom = e.X;
				Invalidate();
			}
			else if (_composants.SelectedComposant == null && _composants.CurrentOperation == null)
			{
				_mouseMove = true;
				_mousePkInit = LocationToPk(e.X);
			}
			else if (_state == ComposantViewerState.Displayed)
			{
				_composants.OnMouseDown(e);
			}
		}
	}

	protected override void OnMouseClick(MouseEventArgs e)
	{
		if (_state != ComposantViewerState.Displayed)
		{
			return;
		}
		_mouseClickLocation = e.Location;
		if (_composants.SelectedComposant != null || _composants.CurrentOperation != null)
		{
			_composants.OnMouseClick(this, e);
		}
		else if (e.Button == MouseButtons.Right)
		{
			SIGContextMenuStrip sIGContextMenuStrip = new SIGContextMenuStrip();
			if (Global.ModeEdition)
			{
				sIGContextMenuStrip.Items.Add("Ajouter une voie...", null, Menu_AjouterVoie);
				sIGContextMenuStrip.Items.Add("-");
			}
			sIGContextMenuStrip.Items.Add("Screenshot", Resources.screenshot, Menu_Screenshot);
			sIGContextMenuStrip.Items.Add("Export rapide", Resources.export_img, Menu_ExportRapide);
			sIGContextMenuStrip.Items.Add("Export avancé...", Resources.export_param, Menu_Export);
			sIGContextMenuStrip.Items.Add("-");
			sIGContextMenuStrip.Items.Add("Ouvrir dans le LOC NG", Resources.loc, Menu_OpenLOCNG);
			sIGContextMenuStrip.Show(this, e.Location);
		}
	}

	protected override void OnMouseMove(MouseEventArgs e)
	{
		if (_state == ComposantViewerState.Empty || _state != ComposantViewerState.Displayed || _mouseMoveLocation == e.Location)
		{
			return;
		}
		_mouseMoveLocation = e.Location;
		if (_mouseZoom >= 0)
		{
			Invalidate();
		}
		else if (_mouseMove)
		{
			int num = _xMouse * PkWidth / base.Width;
			if (_sensPK)
			{
				MoveToPk(_mousePkInit - num, Animation: false);
				_mouseVitesse = _xMouse - _mouseXPrec;
			}
			else
			{
				MoveToPk(_mousePkInit + num, Animation: false);
				_mouseVitesse = _mouseXPrec - _xMouse;
			}
		}
		else
		{
			if (InfoBulle.Visible)
			{
				SuspendLayout();
				InfoBulle.Left = _xMouse;
				InfoBulle.Top = e.Y - InfoBulle.Height;
				ResumeLayout();
			}
			_composants.OnMouseMove(e);
		}
		if (PopupContainer.State == PopupState.Info)
		{
			PopupContainer.SetPosition(e.Location);
		}
		_mouseXPrec = e.X;
	}

	protected override void OnMouseUp(MouseEventArgs e)
	{
		if (_state == ComposantViewerState.Empty)
		{
			return;
		}
		if (_mouseZoom >= 0)
		{
			int num = LocationToPk(e.X);
			int num2 = LocationToPk(_mouseZoom);
			int offset = (_pkF - _pkD - Math.Abs(num - num2)) / 2;
			int center = (num2 + num) / 2;
			_mouseZoom = -1;
			Invalidate();
			Task.Factory.StartNew(delegate
			{
				Zoom(offset, center);
			});
		}
		else if (_mouseMove)
		{
			if (Math.Abs(_mouseVitesse) > 10 && Math.Abs(_mouseVitesse) < 200 && Math.Abs(_mousePkInit - e.X) > 20)
			{
				Task.Factory.StartNew(delegate
				{
					ThreadMoveWithVitesse(_mouseVitesse);
				});
				_mouseVitesse = 0;
			}
			_mouseMove = false;
		}
		else if (_state == ComposantViewerState.Displayed)
		{
			_composants.OnMouseUp(e);
		}
	}

	protected override void OnMouseDoubleClick(MouseEventArgs e)
	{
		if (_state == ComposantViewerState.Displayed && _composants.SelectedComposant != null)
		{
			_composants.OnMouseDoubleClick(this, e);
		}
	}

	protected override void OnMouseWheel(MouseEventArgs e)
	{
		if (e.Delta > 0)
		{
			ZoomIn(4, LocationToPk(e.X));
		}
		else
		{
			ZoomOut(4, LocationToPk(e.X));
		}
	}

	protected override void OnMouseLeave(EventArgs e)
	{
		if (PopupContainer.State == PopupState.Info)
		{
			PopupContainer.State = PopupState.Hidden;
		}
	}

	private void Archives_CurrentArchiveChanged(object sender, EventArgs e)
	{
		if (Ligne != null && Ligne.ID != 0)
		{
			SetLignePK(Ligne.ID, (PkD + PkF) / 2);
		}
		else
		{
			Clear();
		}
		cvRecherche.Visible = Archives.CurrentArchive != null;
	}

	private void Menu_AjouterVoie(object sender, EventArgs e)
	{
		int num = LocationToPk(_xMouse);
		int pkF = num + 500;
		EditNomPksPopupForm editNomPksPopupForm = new EditNomPksPopupForm
		{
			PkD = num,
			PkF = pkF,
			Nom = "V???"
		};
		editNomPksPopupForm.Closing += AjoutVoiePopupForm_Closing;
		PopupContainer.Show(editNomPksPopupForm, "Nouvelle Voie", PopupContainerButtons.Valider);
	}

	private void AjoutVoiePopupForm_Closing(object sender, PopupFormResultEventArgs e)
	{
		if (e.Result != PopupContainerResult.OK)
		{
			return;
		}
		e.Canceled = true;
		EditNomPksPopupForm _popupForm = (EditNomPksPopupForm)sender;
		if (_popupForm.PkD == -1000 || _popupForm.PkF == -1000)
		{
			MessageBox.Show("PKs mal definis", Resources.APP_NAME);
			return;
		}
		int pkDebut = _popupForm.PkD;
		int pkFin = _popupForm.PkF;
		if (Ligne.Voies.Find((SIGVoie voie) => voie.Nom == _popupForm.Nom && voie.PKFin >= pkDebut && voie.PKDebut <= pkFin) != null)
		{
			MessageBox.Show("Une voie portant ce nom existe déjà entre ces 2 PKs", Resources.APP_NAME);
			return;
		}
		e.Canceled = false;
		SIGVoie sIGVoie = Base.CreateVoie(Ligne, _popupForm.Nom, _popupForm.PkD, _popupForm.PkF);
		sIGVoie.PositionY = LocationToPosY(_mouseClickLocation.Y);
		Base.SetPositionVoie(sIGVoie);
		RefreshLigne();
	}

	private void Menu_Screenshot(object sender, EventArgs e)
	{
		Bitmap bitmap = new Bitmap(base.Width, base.Height);
		bool visible = cvRecherche.Visible;
		bool visible2 = ScrollBar.Visible;
		bool visible3 = echelle.Visible;
		echelle.Visible = false;
		ScrollBar.Visible = false;
		cvRecherche.Visible = false;
		MustDrawComponent = true;
		DrawToBitmap(bitmap, DisplayRectangle);
		echelle.DrawToBitmap(bitmap, echelle.DisplayRectangle);
		Clipboard.SetImage(bitmap);
		echelle.Visible = visible3;
		cvRecherche.Visible = visible;
		ScrollBar.Visible = visible2;
		Invalidate(invalidateChildren: true);
	}

	private void Menu_ExportRapide(object sender, EventArgs e)
	{
		new ExportPopupForm().Close(PopupFormResultEventArgs.Ok);
	}

	private void Menu_Export(object sender, EventArgs e)
	{
		ExportPopupForm form = new ExportPopupForm();
		PopupContainer.Show(form, "EXPORTER", PopupContainerButtons.Valider);
	}

	private void Menu_OpenLOCNG(object sender, EventArgs e)
	{
		if (_locNG != null && !_locNG.HasExited)
		{
			_locNG.Kill();
		}
		if (!File.Exists(Paths.LocExe))
		{
			MessageBox.Show("Le LOC NG est introuvable", Resources.APP_NAME);
			return;
		}
		string[] contents = new string[6]
		{
			_ligne.ID.ToString(),
			"4439",
			LocationToPk(_xMouse).ToString(),
			(!_sensPK) ? "+" : "-",
			"+",
			"V1"
		};
		File.WriteAllLines(Paths.LocLastPos, contents);
		_locNG = Process.Start(Paths.LocExe, " /w");
	}

	private void AddFirstVoie_Closed(object sender, PopupFormResultEventArgs e)
	{
		if (e.Result == PopupContainerResult.OK)
		{
			RefreshLigne();
		}
		else
		{
			Clear();
		}
	}

	private void Legende_SizeChanged(object sender, EventArgs e)
	{
		hScrollBar.Left = (legende.Visible ? (legende.Right + 15) : 15);
		hScrollBar.Top = pictureBox1.Top;
		hScrollBar.Width = pictureBox1.Left - 15 - hScrollBar.Left;
		if (_ligne != null)
		{
			lNomLigne.Left = hScrollBar.Left;
			lNomLigne.Width = pictureBox1.Right - lIntro.Left;
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
		this.lIntro = new System.Windows.Forms.Label();
		this.pictureBox1 = new System.Windows.Forms.PictureBox();
		this.legende = new CDV_Viewer.Controls.CVLegende();
		this.echelle = new CDV_Viewer.Controls.CVEchelle();
		this.hScrollBar = new CDV_Viewer.Controls.CVScrollBar();
		this.cvRecherche = new CDV_Viewer.Controls.CVRecherche();
		this.lNomLigne = new System.Windows.Forms.Label();
		((System.ComponentModel.ISupportInitialize)this.pictureBox1).BeginInit();
		base.SuspendLayout();
		this.lIntro.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.lIntro.BackColor = System.Drawing.Color.Transparent;
		this.lIntro.Enabled = false;
		this.lIntro.Font = new System.Drawing.Font("Arial", 18f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		this.lIntro.ForeColor = System.Drawing.Color.FromArgb(64, 64, 64);
		this.lIntro.Location = new System.Drawing.Point(-3, 0);
		this.lIntro.Name = "lIntro";
		this.lIntro.Size = new System.Drawing.Size(834, 308);
		this.lIntro.TabIndex = 2;
		this.lIntro.Text = "SÉLECTIONNEZ UNE LIGNE...";
		this.lIntro.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
		this.pictureBox1.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
		this.pictureBox1.BackColor = System.Drawing.Color.Transparent;
		this.pictureBox1.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
		this.pictureBox1.Enabled = false;
		this.pictureBox1.Image = CDV_Viewer.Properties.Resources.sncf;
		this.pictureBox1.Location = new System.Drawing.Point(743, 279);
		this.pictureBox1.Name = "pictureBox1";
		this.pictureBox1.Size = new System.Drawing.Size(85, 46);
		this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
		this.pictureBox1.TabIndex = 5;
		this.pictureBox1.TabStop = false;
		this.legende.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.legende.BackColor = System.Drawing.Color.FromArgb(230, 230, 230);
		this.legende.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
		this.legende.Enabled = false;
		this.legende.Location = new System.Drawing.Point(10, 233);
		this.legende.Name = "legende";
		this.legende.Size = new System.Drawing.Size(400, 92);
		this.legende.TabIndex = 7;
		this.legende.Visible = false;
		this.echelle.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.echelle.BackColor = System.Drawing.Color.Transparent;
		this.echelle.Location = new System.Drawing.Point(0, 331);
		this.echelle.Name = "echelle";
		this.echelle.Size = new System.Drawing.Size(831, 25);
		this.echelle.TabIndex = 1;
		this.hScrollBar.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.hScrollBar.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
		this.hScrollBar.Location = new System.Drawing.Point(416, 279);
		this.hScrollBar.Name = "hScrollBar";
		this.hScrollBar.Size = new System.Drawing.Size(321, 46);
		this.hScrollBar.TabIndex = 6;
		this.hScrollBar.TabStop = false;
		this.hScrollBar.Visible = false;
		this.cvRecherche.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
		this.cvRecherche.BackColor = System.Drawing.Color.Transparent;
		this.cvRecherche.Location = new System.Drawing.Point(567, 13);
		this.cvRecherche.Name = "cvRecherche";
		this.cvRecherche.Size = new System.Drawing.Size(250, 23);
		this.cvRecherche.TabIndex = 8;
		this.cvRecherche.Visible = false;
		this.lNomLigne.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.lNomLigne.BackColor = System.Drawing.Color.Transparent;
		this.lNomLigne.Enabled = false;
		this.lNomLigne.Font = new System.Drawing.Font("Arial", 18f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		this.lNomLigne.ForeColor = System.Drawing.Color.FromArgb(64, 64, 64);
		this.lNomLigne.Location = new System.Drawing.Point(416, 250);
		this.lNomLigne.Name = "lNomLigne";
		this.lNomLigne.Size = new System.Drawing.Size(412, 26);
		this.lNomLigne.TabIndex = 9;
		this.lNomLigne.Text = "LIGNE";
		this.lNomLigne.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
		this.BackColor = System.Drawing.Color.White;
		base.Controls.Add(this.lNomLigne);
		base.Controls.Add(this.cvRecherche);
		base.Controls.Add(this.legende);
		base.Controls.Add(this.echelle);
		base.Controls.Add(this.hScrollBar);
		base.Controls.Add(this.pictureBox1);
		base.Controls.Add(this.lIntro);
		this.Cursor = System.Windows.Forms.Cursors.Default;
		this.DoubleBuffered = true;
		base.Name = "ComposantsViewer";
		base.Size = new System.Drawing.Size(831, 356);
		base.Controls.SetChildIndex(this.lIntro, 0);
		base.Controls.SetChildIndex(this.pictureBox1, 0);
		base.Controls.SetChildIndex(this.hScrollBar, 0);
		base.Controls.SetChildIndex(this.echelle, 0);
		base.Controls.SetChildIndex(this.legende, 0);
		base.Controls.SetChildIndex(this.cvRecherche, 0);
		base.Controls.SetChildIndex(this.lNomLigne, 0);
		((System.ComponentModel.ISupportInitialize)this.pictureBox1).EndInit();
		base.ResumeLayout(false);
	}
}
