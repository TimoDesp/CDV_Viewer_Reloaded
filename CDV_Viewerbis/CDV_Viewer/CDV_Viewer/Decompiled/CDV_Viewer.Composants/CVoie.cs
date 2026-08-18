using System;
using System.Drawing;
using CDV_Viewer.Data.SIGObjects;
using CDV_Viewer.Styles;

namespace CDV_Viewer.Composants;

public abstract class CVoie : Composant
{
	public static Font _fontNomVoie = new Font("Tahoma", 10f, FontStyle.Bold);

	public static StringFormat _formatTexteNomVoie = new StringFormat
	{
		Alignment = StringAlignment.Center
	};

	public static Pen VoiePen = new Pen(Colors.Voie, 2f);

	public static Pen JonctionPen = new Pen(Colors.Jonction, 2f);

	public static Pen VoiePenSelected = new Pen(Colors.VoieSelected, 2f);

	public static Pen JonctionPenSelected = new Pen(Colors.JonctionSelected, 2f);

	public static Pen HideVoiePen = new Pen(SystemColors.Control, 2f);

	protected SIGVoie _voie;

	public override bool IsComposantSignalisation => false;

	public SIGVoie Voie => _voie;

	public abstract int PosYDebut { get; }

	public abstract int PosYFin { get; }

	public int MaxPosY => Math.Max(PosYDebut, PosYFin);

	public int MinPosY => Math.Min(PosYDebut, PosYFin);

	public override string ToString()
	{
		return $"Composant {_voie}";
	}
}
