using System;
using System.Collections.Generic;
using System.Drawing;
using CDV_Viewer.Composants;
using CDV_Viewer.DockControls;
using CDV_Viewer.Forms;
using CDV_Viewer.Properties;

namespace CDV_Viewer.Data;

public static class Global
{
	public static readonly int DefaultEchelleX = Settings.Default.EchelleX;

	public static readonly int DefaultNbVoies = Settings.Default.NbVoies;

	public static readonly int SizeVoieAdjacente = Settings.Default.SizeVoieAdjacente;

	public static readonly int LongueurVoieSecondaire = Settings.Default.LongueurVoieSecondaire;

	public static readonly int OrdreVoie = Settings.Default.OrdreVoie;

	public static readonly int OrdreJonction = Settings.Default.OrdreJonction;

	public static readonly int OrdreBalise = Settings.Default.OrdreBalise;

	public static readonly int OrdreCircuit = Settings.Default.OrdreCircuit;

	public static readonly int OrdreNoeud = Settings.Default.OrdreNoeud;

	public static readonly int OrdreJoint = Settings.Default.OrdreJoint;

	public static readonly double ZoomMax = Settings.Default.ZoomMax;

	public static readonly double ZoomMin = Settings.Default.ZoomMin;

	public static readonly int ZoomTexteVisibleJonctions = Settings.Default.ZoomTexteVisibleJonctions;

	public static readonly int ZoomVisibleBalises = Settings.Default.ZoomVisibleBalises;

	public static readonly int ZoomVisibleCircuits = Settings.Default.ZoomVisibleCircuits;

	public static readonly int ZoomTexteVisibleCircuits = Settings.Default.ZoomTexteVisibleCircuits;

	public static readonly int ZoomVisibleJoints = Settings.Default.ZoomVisibleJoints;

	public static readonly int ZoomTexteVisibleJoints = Settings.Default.ZoomTexteVisibleJoints;

	public static readonly bool CdvDrawingModeCenter = Settings.Default.CdVDrawingMode == "Center";

	public static readonly int DefaultFontSize = Settings.Default.FontSize;

	public static readonly string DefaultFontName = Settings.Default.Font;

	public static readonly Font DefaultFont = new Font(Settings.Default.Font, Settings.Default.FontSize);

	public static readonly Font DefaultBoldFont = new Font(Settings.Default.Font, Settings.Default.FontSize, FontStyle.Bold);

	public static MainForm MainForm;

	private static bool _modeEdition = false;

	public static CVModele ModeleViewer;

	public static Parcours Parcours = new Parcours();

	public static TourneeViewer ParcoursControl;

	public static LiveControl LiveControl;

	public static HelpControl HelpControl;

	public static ListeLignes ListeLignes;

	public static Dictionary<int, ComposantsCollection> ComposantsParcours = new Dictionary<int, ComposantsCollection>();

	public static bool ModeEdition
	{
		get
		{
			return _modeEdition;
		}
		set
		{
			if (Autorisations.Values.Edition && value != _modeEdition)
			{
				_modeEdition = value;
				Global.ModeEditionChanged?.Invoke(null, new EventArgs());
			}
		}
	}

	public static event EventHandler ModeEditionChanged;
}
