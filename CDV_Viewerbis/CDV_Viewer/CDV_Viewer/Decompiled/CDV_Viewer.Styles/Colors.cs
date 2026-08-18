using System.Collections.Generic;
using System.Drawing;
using System.Reflection;
using System.Resources;

namespace CDV_Viewer.Styles;

public static class Colors
{
	private static ResourceManager _ResourceManager;

	private static Color _formBackTopEdit = GetColor("FormBackTopEdit");

	private static Color _formBackTop = GetColor("FormBackTop");

	private static Color _formBackBottomEdit = GetColor("FormBackBottomEdit");

	private static Color _formBackBottom = GetColor("FormBackBottom");

	public static Color BaliseEnabled = GetColor("BaliseEnable");

	public static Color BaliseDisabled = GetColor("BaliseDisable");

	public static Color Noeud = GetColor("Noeud");

	public static Color NoeudSelected = GetSelectedColor(GetColor("Noeud"));

	public static Color Voie = GetColor("Voie");

	public static Color VoieSelected = GetSelectedColor(GetColor("Voie"));

	public static Color Joint = GetColor("Joint");

	public static Color Jonction = GetColor("Jonction");

	public static Color JonctionSelected = GetSelectedColor(GetColor("Jonction"));

	public static Color AutreLigne = GetColor("AutreLigne");

	public static Color AutreLigneSelected = GetSelectedColor(GetColor("Voie"));

	private static Dictionary<int, Color> _cdv = new Dictionary<int, Color>
	{
		{
			0,
			GetColor("CDV0")
		},
		{
			1700,
			GetColor("CDV1700")
		},
		{
			2000,
			GetColor("CDV2000")
		},
		{
			2300,
			GetColor("CDV2300")
		},
		{
			2600,
			GetColor("CDV2600")
		},
		{
			3,
			GetColor("CDV3")
		}
	};

	public static Color FormBackTop(bool editMode)
	{
		if (!editMode)
		{
			return _formBackTop;
		}
		return _formBackTopEdit;
	}

	public static Color FormBackBottom(bool editMode)
	{
		if (!editMode)
		{
			return _formBackBottom;
		}
		return _formBackBottom;
	}

	public static Color Cdv(int frequence)
	{
		if (_cdv.TryGetValue(frequence, out var value))
		{
			return value;
		}
		return Color.YellowGreen;
	}

	public static Color GetCdvSelectedColor(Color color)
	{
		return Color.FromArgb(80, color);
	}

	public static Color GetSelectedColor(Color color)
	{
		return Color.FromArgb(100, color);
	}

	public static Color GetColor(string Chaine)
	{
		if (_ResourceManager == null)
		{
			_ResourceManager = new ResourceManager("CDV_Viewer.Styles.Couleurs", Assembly.GetExecutingAssembly());
		}
		return ColorTranslator.FromHtml(_ResourceManager.GetString(Chaine) ?? string.Empty);
	}
}
