using System;
using System.Drawing.Text;
using CDV_Viewer.Properties;

namespace CDV_Viewer.Data;

public static class CustomFonts
{
	public static PrivateFontCollection Geo = new PrivateFontCollection();

	public unsafe static void Load()
	{
		byte[] geosansLight = Resources.GeosansLight;
		fixed (byte* ptr = geosansLight)
		{
			Geo.AddMemoryFont((IntPtr)ptr, geosansLight.Length);
		}
	}
}
