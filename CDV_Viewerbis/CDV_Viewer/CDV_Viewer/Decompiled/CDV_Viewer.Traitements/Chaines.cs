using System;
using System.Globalization;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace CDV_Viewer.Traitements;

public static class Chaines
{
	public class SubStr
	{
		public static SubStr Empty = new SubStr();

		public string Fullstring { get; } = "";

		public int Start { get; } = -1;

		public int Length { get; } = -1;

		public string Value
		{
			get
			{
				if (Start < 0 || Length == 0)
				{
					return "";
				}
				return Fullstring.Substring(Start, Length);
			}
		}

		private SubStr()
		{
		}

		public SubStr(string s, int p, int l)
		{
			Fullstring = s;
			if (!(s == "") && p <= s.Length)
			{
				Start = p;
				if (Start + l > s.Length)
				{
					Length = s.Length - Start;
				}
				else
				{
					Length = l;
				}
			}
		}
	}

	public static int GetFirstNombre(string chaine, out int position)
	{
		position = -1;
		if (chaine == "")
		{
			return -1;
		}
		char c = '0';
		int num = 0;
		while (num < chaine.Length && !char.IsDigit(c = chaine[num++]))
		{
		}
		if (num >= chaine.Length)
		{
			return -1;
		}
		position = num;
		int num2 = c - 48;
		while (num < chaine.Length && char.IsDigit(c = chaine[num++]))
		{
			num2 = num2 * 10 + (c - 48);
		}
		return num2;
	}

	public static string PkToString(int pk)
	{
		bool num = pk < 0;
		pk = Math.Abs(pk);
		int num2 = pk / 1000;
		int num3 = pk - num2 * 1000;
		if (num)
		{
			if (pk >= 1000)
			{
				return $"-{num2:##0}+{num3:000}";
			}
			return $"000-{pk:000}";
		}
		return $"{num2:##0}+{num3:000}";
	}

	public static bool TryParsePk(string pk_str, out int PK)
	{
		PK = 0;
		if (string.IsNullOrEmpty(pk_str))
		{
			return false;
		}
		int num = 1;
		if (pk_str.Contains("-"))
		{
			num = -1;
		}
		pk_str = pk_str.TrimStart('+', '-').Replace('+', '.').Replace('-', '.');
		if (!double.TryParse(pk_str, NumberStyles.Any, CultureInfo.InvariantCulture, out var result))
		{
			return false;
		}
		PK = num * (int)Math.Round(result * 1000.0);
		return true;
	}

	public static string DecryptString(string texte)
	{
		try
		{
			byte[] array = Convert.FromBase64String(texte);
			byte[] bytes = Encoding.UTF8.GetBytes("sK8Sviap12q0q86S");
			byte[] bytes2 = Encoding.UTF8.GetBytes("sIva6S2b5e4QS3o2");
			ICryptoTransform transform = new RijndaelManaged
			{
				Mode = CipherMode.CBC
			}.CreateDecryptor(bytes, bytes2);
			MemoryStream memoryStream = new MemoryStream(array);
			CryptoStream cryptoStream = new CryptoStream(memoryStream, transform, CryptoStreamMode.Read);
			byte[] array2 = new byte[array.Length];
			int count = cryptoStream.Read(array2, 0, array2.Length);
			memoryStream.Close();
			cryptoStream.Close();
			return Encoding.UTF8.GetString(array2, 0, count);
		}
		catch
		{
			return string.Empty;
		}
	}
}
