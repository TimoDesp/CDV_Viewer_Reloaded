using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Resources;
using System.Runtime.CompilerServices;

namespace CDV_Viewer.Styles;

[GeneratedCode("System.Resources.Tools.StronglyTypedResourceBuilder", "15.0.0.0")]
[DebuggerNonUserCode]
[CompilerGenerated]
internal class Couleurs
{
	private static ResourceManager resourceMan;

	private static CultureInfo resourceCulture;

	[EditorBrowsable(EditorBrowsableState.Advanced)]
	internal static ResourceManager ResourceManager
	{
		get
		{
			if (resourceMan == null)
			{
				resourceMan = new ResourceManager("CDV_Viewer.Styles.Couleurs", typeof(Couleurs).Assembly);
			}
			return resourceMan;
		}
	}

	[EditorBrowsable(EditorBrowsableState.Advanced)]
	internal static CultureInfo Culture
	{
		get
		{
			return resourceCulture;
		}
		set
		{
			resourceCulture = value;
		}
	}

	internal static string AutreLigne => ResourceManager.GetString("AutreLigne", resourceCulture);

	internal static string BaliseDisable => ResourceManager.GetString("BaliseDisable", resourceCulture);

	internal static string BaliseEnable => ResourceManager.GetString("BaliseEnable", resourceCulture);

	internal static string CDV0 => ResourceManager.GetString("CDV0", resourceCulture);

	internal static string CDV1700 => ResourceManager.GetString("CDV1700", resourceCulture);

	internal static string CDV2000 => ResourceManager.GetString("CDV2000", resourceCulture);

	internal static string CDV2300 => ResourceManager.GetString("CDV2300", resourceCulture);

	internal static string CDV2600 => ResourceManager.GetString("CDV2600", resourceCulture);

	internal static string CDV3 => ResourceManager.GetString("CDV3", resourceCulture);

	internal static string ContextMenuItem => ResourceManager.GetString("ContextMenuItem", resourceCulture);

	internal static string ContextMenuTitre => ResourceManager.GetString("ContextMenuTitre", resourceCulture);

	internal static string Echelle => ResourceManager.GetString("Echelle", resourceCulture);

	internal static string FormBack => ResourceManager.GetString("FormBack", resourceCulture);

	internal static string FormMenu => ResourceManager.GetString("FormMenu", resourceCulture);

	internal static string FormTopMenu => ResourceManager.GetString("FormTopMenu", resourceCulture);

	internal static string GraphTitre => ResourceManager.GetString("GraphTitre", resourceCulture);

	internal static string Joint => ResourceManager.GetString("Joint", resourceCulture);

	internal static string Jonction => ResourceManager.GetString("Jonction", resourceCulture);

	internal static string LegendeBack => ResourceManager.GetString("LegendeBack", resourceCulture);

	internal static string MainFormBottom => ResourceManager.GetString("MainFormBottom", resourceCulture);

	internal static string MainFormTop => ResourceManager.GetString("MainFormTop", resourceCulture);

	internal static string Noeud => ResourceManager.GetString("Noeud", resourceCulture);

	internal static string PanelToolStrip => ResourceManager.GetString("PanelToolStrip", resourceCulture);

	internal static string PropertyBoutonBack => ResourceManager.GetString("PropertyBoutonBack", resourceCulture);

	internal static string PropertyBoutonFore => ResourceManager.GetString("PropertyBoutonFore", resourceCulture);

	internal static string PropertyWindow => ResourceManager.GetString("PropertyWindow", resourceCulture);

	internal static string PropertyWindowText => ResourceManager.GetString("PropertyWindowText", resourceCulture);

	internal static string Voie => ResourceManager.GetString("Voie", resourceCulture);

	internal Couleurs()
	{
	}
}
