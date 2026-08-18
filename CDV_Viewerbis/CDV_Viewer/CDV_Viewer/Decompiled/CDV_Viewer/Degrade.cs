using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.Resources;
using System.Runtime.CompilerServices;

namespace CDV_Viewer;

[GeneratedCode("System.Resources.Tools.StronglyTypedResourceBuilder", "15.0.0.0")]
[DebuggerNonUserCode]
[CompilerGenerated]
internal class Degrade
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
				resourceMan = new ResourceManager("CDV_Viewer.Degrade", typeof(Degrade).Assembly);
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

	internal static Bitmap bottom => (Bitmap)ResourceManager.GetObject("bottom", resourceCulture);

	internal static Bitmap bottomleft => (Bitmap)ResourceManager.GetObject("bottomleft", resourceCulture);

	internal static Bitmap bottomright => (Bitmap)ResourceManager.GetObject("bottomright", resourceCulture);

	internal static Bitmap left => (Bitmap)ResourceManager.GetObject("left", resourceCulture);

	internal static Bitmap right => (Bitmap)ResourceManager.GetObject("right", resourceCulture);

	internal static Bitmap top => (Bitmap)ResourceManager.GetObject("top", resourceCulture);

	internal static Bitmap topleft => (Bitmap)ResourceManager.GetObject("topleft", resourceCulture);

	internal static Bitmap topright => (Bitmap)ResourceManager.GetObject("topright", resourceCulture);

	internal Degrade()
	{
	}
}
