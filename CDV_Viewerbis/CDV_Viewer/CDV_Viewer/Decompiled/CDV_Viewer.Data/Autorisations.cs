using System;
using System.Collections;
using System.IO;
using System.Text;
using CDV_Viewer.Traitements;

namespace CDV_Viewer.Data;

public class Autorisations
{
	private static Autorisations _values;

	public bool CorrectKey = true;

	public bool Edition;

	public bool AccesParcoursControl;

	public bool AccesLiveControl;

	public bool Valeur4;

	public bool Valeur5;

	public bool Valeur6;

	public bool Valeur7;

	public bool Valeur8;

	public bool Valeur9;

	public bool Valeur10;

	public bool Valeur11;

	public bool Valeur12;

	public bool Valeur13;

	public bool Valeur14;

	public bool Valeur15;

	public bool Valeur16;

	public static Autorisations Values
	{
		get
		{
			if (_values == null)
			{
				_values = Load(Paths.Cle);
			}
			return _values;
		}
	}

	private static Autorisations Load(string path)
	{
		Autorisations autorisations = new Autorisations();
		if (!File.Exists(path))
		{
			return autorisations;
		}
		string text = Chaines.DecryptString(File.ReadAllText(path));
		if (text.Length != 8)
		{
			return autorisations;
		}
		byte[] bytes = Encoding.Default.GetBytes(text);
		autorisations.CorrectKey = ES.IsValidMACAdress(BitConverter.ToString(bytes, 0, 6));
		BitArray bitArray = new BitArray(bytes);
		autorisations.Valeur8 = bitArray[48];
		autorisations.Valeur7 = bitArray[49];
		autorisations.Valeur6 = bitArray[50];
		autorisations.Valeur5 = bitArray[51];
		autorisations.Valeur4 = bitArray[52];
		autorisations.AccesLiveControl = bitArray[53];
		autorisations.AccesParcoursControl = bitArray[54];
		autorisations.Edition = bitArray[55];
		autorisations.Valeur16 = bitArray[56];
		autorisations.Valeur15 = bitArray[57];
		autorisations.Valeur14 = bitArray[58];
		autorisations.Valeur13 = bitArray[59];
		autorisations.Valeur12 = bitArray[60];
		autorisations.Valeur11 = bitArray[61];
		autorisations.Valeur10 = bitArray[62];
		autorisations.Valeur9 = bitArray[63];
		return autorisations;
	}
}
