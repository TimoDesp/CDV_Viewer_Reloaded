using System;
using System.Collections.Generic;
using System.Threading;
using CDV_Viewer.CsvBase;
using CDV_Viewer.Data.SIGObjects;

namespace CDV_Viewer.Traitements;

public class Recherche
{
	private bool _isSearching;

	private Thread _thread;

	private bool _stopThread;

	private SIGLigne _ligne;

	private string _texte;

	private int _maxResults = 20;

	private List<object> _results = new List<object>();

	public bool IsSearching => _isSearching;

	public List<object> Results => _results;

	public event EventHandler EndSearching;

	public Recherche(SIGLigne ligne, string texte, int maxResults)
	{
		_ligne = ligne;
		_texte = texte.ToUpper().Trim();
		_maxResults = maxResults;
	}

	public void Start()
	{
		_thread = new Thread(Search);
		_thread.Start();
	}

	private void Search()
	{
		_stopThread = false;
		_results = new List<object>();
		if (_texte.Length == 0)
		{
			SendEndSearching();
			return;
		}
		if (_ligne == null)
		{
			foreach (SIGCircuit item in Base.CsvCircuits.SigCircuits())
			{
				if (_results.Count >= _maxResults)
				{
					break;
				}
				if (item.Nom.Contains(_texte))
				{
					foreach (KeyValuePair<int, int> item2 in Base.GetLignesCircuit(item.ID))
					{
						_results.Add(new CircuitOnLigne(item, item2.Key, item2.Value));
					}
				}
				if (_stopThread)
				{
					return;
				}
			}
		}
		else
		{
			int pKDebut = _ligne.PKDebut;
			int pKFin = _ligne.PKFin;
			if (_texte.Contains("+") && int.TryParse(_texte.Remove(_texte.IndexOf('+'), 1), out var result) && result >= pKDebut && result < pKFin)
			{
				_results.Add(result);
				return;
			}
			if (_texte.Contains(".") && int.TryParse(_texte.Remove(_texte.IndexOf('.'), 1), out result) && result >= pKDebut && result < pKFin)
			{
				_results.Add(result);
				return;
			}
			if (int.TryParse(_texte, out result) && result >= pKDebut && result < pKFin)
			{
				_results.Add(result);
			}
			foreach (SIGVoie voie in _ligne.Voies)
			{
				if (_results.Count >= _maxResults)
				{
					break;
				}
				if (voie.Nom.Contains(_texte))
				{
					_results.Add(voie);
				}
				if (_stopThread)
				{
					return;
				}
			}
			foreach (SIGCircuit circuit in _ligne.Circuits)
			{
				if (_results.Count >= _maxResults)
				{
					break;
				}
				if (circuit.Nom.Contains(_texte))
				{
					_results.Add(circuit);
				}
				if (_stopThread)
				{
					return;
				}
			}
		}
		SendEndSearching();
	}

	private void SendEndSearching()
	{
		this.EndSearching?.Invoke(this, new EventArgs());
	}

	public void Stop()
	{
		if (_thread != null)
		{
			_stopThread = true;
			while (_thread.ThreadState == ThreadState.Running)
			{
				_thread.Abort();
				Thread.Sleep(10);
			}
		}
	}
}
