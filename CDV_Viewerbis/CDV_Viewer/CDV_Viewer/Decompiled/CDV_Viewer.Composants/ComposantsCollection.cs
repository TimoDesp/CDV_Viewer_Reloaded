using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using CDV_Viewer.Data;
using CDV_Viewer.Data.SIGObjects;
using CDV_Viewer.DockControls;
using CDV_Viewer.Traitements;

namespace CDV_Viewer.Composants;

public class ComposantsCollection : List<Composant>
{
	internal interface IComposantDictionary
	{
		void Clear();
	}

	internal class ComposantDictionary<T> : Dictionary<int, T>, IComposantDictionary where T : Composant
	{
		internal void Add(T item)
		{
			Add(item.Id, item);
		}
	}

	public readonly ComposantsViewer ComposantsViewer;

	private bool _sorted;

	public readonly List<Composant> DisplayedComposants = new List<Composant>();

	private ComposantDictionary<CVoieOnLine> _voiesOnLines = new ComposantDictionary<CVoieOnLine>();

	private ComposantDictionary<CVoieAdjacente> _voiesAdjacentes = new ComposantDictionary<CVoieAdjacente>();

	private ComposantDictionary<CNoeud> _noeuds = new ComposantDictionary<CNoeud>();

	private ComposantDictionary<CSautPK> _sautsPk = new ComposantDictionary<CSautPK>();

	private ComposantDictionary<CBalise> _balises = new ComposantDictionary<CBalise>();

	private ComposantDictionary<CJoint> _joints = new ComposantDictionary<CJoint>();

	private ComposantDictionary<CCircuit> _circuits = new ComposantDictionary<CCircuit>();

	public bool HasSelectedComposant
	{
		get
		{
			if (SelectedComposant != null)
			{
				return CurrentOperation != null;
			}
			return false;
		}
	}

	public Composant SelectedComposant { get; private set; }

	public COperation CurrentOperation { get; private set; }

	public IEnumerable<CVoieOnLine> VoiesOnLines => _voiesOnLines.Values;

	public IEnumerable<CVoieAdjacente> VoiesAdjacentes => _voiesAdjacentes.Values;

	public IEnumerable<CJoint> Joints => _joints.Values;

	public IEnumerable<CNoeud> Noeuds => _noeuds.Values;

	public IEnumerable<CSautPK> SautsPk => _sautsPk.Values;

	public IEnumerable<CCircuit> Circuits => _circuits.Values;

	public event EventHandler SelectedComposantChanged;

	public void UnSelectComposant()
	{
		SelectedComposant = null;
	}

	private void ClearComposantDictionaries()
	{
		_voiesOnLines.Clear();
		_voiesAdjacentes.Clear();
		_noeuds.Clear();
		_sautsPk.Clear();
		_balises.Clear();
		_joints.Clear();
		_circuits.Clear();
	}

	public ComposantsCollection(ComposantsViewer composantviewer)
	{
		ComposantsViewer = composantviewer;
	}

	public new bool Add(Composant composant)
	{
		if (composant.Erreur)
		{
			return false;
		}
		base.Add(composant);
		composant.OnComponentAddedInCollection(this);
		_sorted = false;
		return true;
	}

	public void RemoveOperation()
	{
		CurrentOperation = null;
		ComposantsViewer.Invalidate();
	}

	public void AddOperation(COperation operation)
	{
		if (SelectedComposant != null)
		{
			SelectedComposant.MouseLeave();
			SelectedComposant = null;
		}
		CurrentOperation = operation;
		operation.OnComponentAddedInCollection(this);
	}

	public bool AddJoint(CVoie cvoie, SIGJoint joint)
	{
		if (joint == null)
		{
			return false;
		}
		if (_joints.ContainsKey(joint.ID))
		{
			return true;
		}
		CJoint cJoint = new CJoint(cvoie, joint);
		if (Add(cJoint))
		{
			_joints.Add(cJoint);
			return true;
		}
		return false;
	}

	public bool AddBalise(CVoieOnLine cvoie, SIGBalise balise)
	{
		if (_balises.ContainsKey(balise.ID))
		{
			return true;
		}
		CBalise cBalise = new CBalise(cvoie, balise);
		if (Add(cBalise))
		{
			_balises.Add(cBalise);
			return true;
		}
		return false;
	}

	public bool AddSautPk(CVoieOnLine cvoie, SIGNoeud noeud)
	{
		if (_sautsPk.ContainsKey(noeud.ID))
		{
			return true;
		}
		CSautPK cSautPK = new CSautPK(cvoie, noeud);
		if (Add(cSautPK))
		{
			_sautsPk.Add(cSautPK);
			return true;
		}
		return false;
	}

	public bool AddNoeud(CVoieOnLine cvoie, SIGNoeud noeud)
	{
		if (_noeuds.ContainsKey(noeud.ID))
		{
			return true;
		}
		CNoeud cNoeud = new CNoeud(cvoie, noeud);
		if (Add(cNoeud))
		{
			_noeuds.Add(cNoeud);
			return true;
		}
		return false;
	}

	public bool AddVoieOnLine(SIGVoie voie, out CVoieOnLine cvoie)
	{
		if (_voiesOnLines.TryGetValue(voie.ID, out cvoie))
		{
			return false;
		}
		cvoie = CVoieOnLine.Create(voie);
		if (Add(cvoie))
		{
			_voiesOnLines.Add(cvoie);
			return true;
		}
		return false;
	}

	public bool AddVoieAdjacente(SIGBranche branche)
	{
		SIGNoeud sIGNoeud = branche?.Noeud;
		if (sIGNoeud == null)
		{
			return false;
		}
		if (_voiesAdjacentes.TryGetValue(sIGNoeud.ID, out var value))
		{
			return false;
		}
		value = new CVoieAdjacente(branche.Voie, sIGNoeud);
		if (Add(value))
		{
			_voiesAdjacentes.Add(value);
			return true;
		}
		return false;
	}

	public bool AddCircuit(SIGCircuit circuit)
	{
		if (circuit == null)
		{
			return false;
		}
		CCircuit cCircuit = new CCircuit(this, circuit);
		if (Add(cCircuit))
		{
			_circuits.Add(cCircuit);
			return true;
		}
		return false;
	}

	public new void Clear()
	{
		base.Clear();
		ClearComposantDictionaries();
		CurrentOperation = null;
	}

	public void Draw(PaintEventArgs e)
	{
		SortCollection();
		DisplayedComposants.Clear();
		for (int num = base.Count - 1; num >= 0; num--)
		{
			DrawComposant(e, base[num]);
		}
		DisplayedComposants.Reverse();
		CurrentOperation?.Paint(e);
		void SortCollection()
		{
			if (!_sorted)
			{
				Sort((Composant x, Composant y) => x.Ordre.CompareTo(y.Ordre));
			}
			_sorted = true;
		}
	}

	private void DrawComposant(PaintEventArgs e, Composant composant)
	{
		composant._displayPath.Clear();
		composant._displayRectangle = Rectangle.Empty;
		try
		{
			if (composant.IsInGraph() && composant.Paint(e))
			{
				DisplayedComposants.Add(composant);
			}
		}
		catch (Exception)
		{
		}
	}

	public void ReverseForEach(Action<Composant> action)
	{
		for (int num = base.Count - 1; num >= 0; num--)
		{
			action(base[num]);
		}
	}

	public void OnMouseMove(MouseEventArgs e)
	{
		if (CurrentOperation != null)
		{
			CurrentOperation.MouseMove(e);
			return;
		}
		Composant composant = DisplayedComposants.Find((Composant c) => c.Contains(e.Location));
		if (composant != null && ES.GetStateKey(Keys.ControlKey))
		{
			composant = DisplayedComposants.FindLast((Composant c) => c.Contains(e.Location));
		}
		if (composant == null)
		{
			if (SelectedComposant != null)
			{
				SelectedComposant.MouseLeave();
				SelectedComposant = null;
				if (ComposantsViewer.PopupContainer.State == PopupState.Info)
				{
					ComposantsViewer.PopupContainer.State = PopupState.Hidden;
				}
				this.SelectedComposantChanged?.Invoke(this, EventArgs.Empty);
				ComposantsViewer.Invalidate();
			}
		}
		else if (composant == SelectedComposant)
		{
			if (ComposantsViewer.PopupContainer.State == PopupState.Info)
			{
				ComposantsViewer.Invalidate();
			}
		}
		else
		{
			SelectedComposant = composant;
			bool showInfobulle = ComposantsViewer.ShowInfobulle;
			bool flag = ComposantsViewer.PopupContainer.State == PopupState.Edit;
			SelectedComposant.MouseEnter();
			this.SelectedComposantChanged?.Invoke(this, EventArgs.Empty);
			if (showInfobulle && !flag)
			{
				ComposantsViewer.PopupContainer.Show(SelectedComposant.GetPropertyWindow());
			}
			ComposantsViewer.Invalidate();
		}
	}

	public void OnMouseDown(MouseEventArgs e)
	{
		if (CurrentOperation != null)
		{
			CurrentOperation.MouseDown(e);
		}
		else
		{
			SelectedComposant?.MouseDown(e);
		}
	}

	public void OnMouseUp(MouseEventArgs e)
	{
		if (CurrentOperation != null)
		{
			CurrentOperation.MouseUp(e);
		}
		else
		{
			SelectedComposant?.MouseUp(e);
		}
	}

	public void OnMouseClick(ComposantsViewer graph, MouseEventArgs e)
	{
		if (CurrentOperation != null)
		{
			CurrentOperation.MouseClick(e);
		}
		else if (SelectedComposant != null)
		{
			SelectedComposant.MouseClick(e);
			if (e.Button == MouseButtons.Right)
			{
				SelectedComposant.GetMenu()?.Show(graph, e.Location);
			}
		}
	}

	public void OnMouseDoubleClick(ComposantsViewer graph, MouseEventArgs e)
	{
		SelectedComposant?.MouseDoubleClick(e);
	}

	public void OnTopologieLoaded()
	{
		foreach (CNoeud noeud in Noeuds)
		{
			noeud.SetSupport();
		}
		using Enumerator enumerator2 = GetEnumerator();
		while (enumerator2.MoveNext())
		{
			enumerator2.Current.OnComposantsLoaded();
		}
	}

	public CVoieOnLine GetVoie(SIGVoie voie)
	{
		_voiesOnLines.TryGetValue(voie.ID, out var value);
		return value;
	}

	public CVoie GetVoie(SIGBranche branche)
	{
		CVoieOnLine voie = GetVoie(branche.Voie);
		if (voie != null)
		{
			return voie;
		}
		return GetVoieAdjacente(branche.Noeud);
	}

	public CVoiePrincipale DisplayedVoieOnLine(Point location, int ecarty = 10)
	{
		foreach (Composant displayedComposant in DisplayedComposants)
		{
			if (displayedComposant is CVoiePrincipale cVoiePrincipale && cVoiePrincipale.DisplayPath.Contains(location, ecarty))
			{
				return cVoiePrincipale;
			}
		}
		return null;
	}

	public CVoieAdjacente GetVoieAdjacente(SIGBranche branche)
	{
		return GetVoieAdjacente(branche?.Noeud);
	}

	public CVoieAdjacente GetVoieAdjacente(SIGNoeud noeud)
	{
		if (noeud == null)
		{
			return null;
		}
		_voiesAdjacentes.TryGetValue(noeud.ID, out var value);
		return value;
	}

	public CVoieAdjacente GetVoieAdjacente(SIGVoie V, int pk1, int pk2)
	{
		Calculs.OrderPk(ref pk1, ref pk2);
		foreach (IGrouping<int, SIGBranche> item in (from b in V.Branches.FindAll((SIGBranche b) => b.PK >= pk1 && b.PK <= pk2)
			group b by b.Noeud.ID).ToList())
		{
			int key = item.Key;
			if (_voiesAdjacentes.TryGetValue(key, out var value))
			{
				return value;
			}
		}
		return null;
	}

	public CJoint GetJoint(SIGJoint joint)
	{
		_joints.TryGetValue(joint.ID, out var value);
		return value;
	}

	public CNoeud GetNoeud(SIGNoeud noeud)
	{
		_noeuds.TryGetValue(noeud.ID, out var value);
		return value;
	}

	public CSautPK GetSautPk(SIGNoeud noeud)
	{
		_sautsPk.TryGetValue(noeud.ID, out var value);
		return value;
	}

	public T GetComposant<T>(Point pt, int pkPoint) where T : Composant
	{
		using (Enumerator enumerator = GetEnumerator())
		{
			while (enumerator.MoveNext())
			{
				Composant current = enumerator.Current;
				if (current is T && current.Contains(pt, pkPoint))
				{
					return current as T;
				}
			}
		}
		return null;
	}

	public CCircuit GetCircuit(SIGCircuit Circuit)
	{
		if (Circuit == null)
		{
			return null;
		}
		_circuits.TryGetValue(Circuit.ID, out var value);
		return value;
	}
}
