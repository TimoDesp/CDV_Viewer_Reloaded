using System;

namespace CDV_Viewer;

public struct ErreurVerif : IComparable
{
	public TypesErreurVerif Type;

	public object Objet;

	public ErreurVerif(TypesErreurVerif type, object objet)
	{
		Type = type;
		Objet = objet;
	}

	public int CompareTo(object y)
	{
		return Type.CompareTo(((ErreurVerif)y).Type);
	}
}
