using System.Collections.Generic;
using System.Linq;

namespace CDV_Viewer.Data.SIGObjects;

public class SIGLigne : ISigId
{
	public string Nom;

	public List<SIGVoie> Voies = new List<SIGVoie>();

	public List<SIGVoie> VoiesAdjacentes = new List<SIGVoie>();

	public List<SIGCircuit> Circuits = new List<SIGCircuit>();

	private int? _pkDebut;

	private int? _pkFin;

	public int ID { get; private set; }

	public int PKDebut
	{
		get
		{
			if (!_pkDebut.HasValue)
			{
				_pkDebut = GetPKDebut();
			}
			if (!_pkDebut.HasValue)
			{
				return int.MaxValue;
			}
			return _pkDebut.Value;
		}
	}

	public int PKFin
	{
		get
		{
			if (!_pkFin.HasValue)
			{
				_pkFin = GetPKFin();
			}
			if (!_pkFin.HasValue)
			{
				return 0;
			}
			return _pkFin.Value;
		}
	}

	public List<SIGVoie> VoiesWithoutJonctions => GetSortedVoiesWithoutJonctions();

	public SIGLigne(int id, string nom)
	{
		ID = id;
		Nom = nom;
	}

	internal void Unlink()
	{
		Voies.Clear();
		VoiesAdjacentes.Clear();
		Circuits.Clear();
		_pkFin = (_pkDebut = null);
	}

	private int? GetPKDebut()
	{
		int num = int.MaxValue;
		foreach (SIGVoie voie in Voies)
		{
			if (voie.PKDebut < num)
			{
				num = voie.PKDebut;
			}
		}
		if (num < int.MaxValue)
		{
			return num;
		}
		return null;
	}

	private int? GetPKFin()
	{
		int num = 0;
		foreach (SIGVoie voie in Voies)
		{
			if (voie.PKFin > num)
			{
				num = voie.PKFin;
			}
		}
		if (num > 0)
		{
			return num;
		}
		return null;
	}

	private List<SIGVoie> GetSortedVoiesWithoutJonctions()
	{
		List<SIGVoie> list = Voies.FindAll((SIGVoie v) => v.IsVoiePrincipale());
		list.Sort();
		return list;
	}

	public Dictionary<string, List<SIGVoie>> VoieByName()
	{
		return (from v in Voies
			group v by v.Nom).ToDictionary((IGrouping<string, SIGVoie> g) => g.Key, (IGrouping<string, SIGVoie> g) => g.ToList());
	}

	public override string ToString()
	{
		return "Ligne " + ID + " (" + Nom + ")";
	}
}
