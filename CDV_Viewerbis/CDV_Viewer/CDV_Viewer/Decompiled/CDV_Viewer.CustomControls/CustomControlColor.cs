using System.Drawing;

namespace CDV_Viewer.CustomControls;

public class CustomControlColor
{
	private Color _veryLightColor = Color.White;

	private Color _lightColor = Color.LightGray;

	private Color _darkColor = Color.DarkGray;

	public static CustomControlColor Gris = new CustomControlColor(Color.Gainsboro, Color.LightGray, Color.DarkGray);

	public static CustomControlColor Blue = new CustomControlColor(Color.FromArgb(180, 240, 255), Color.FromArgb(140, 220, 255), Color.FromArgb(0, 200, 255));

	public static CustomControlColor Green = new CustomControlColor(Color.FromArgb(180, 255, 140), Color.FromArgb(110, 244, 83), Color.FromArgb(73, 210, 48));

	public static CustomControlColor Orange = new CustomControlColor(Color.FromArgb(255, 200, 150), Color.FromArgb(255, 171, 106), Color.FromArgb(255, 132, 33));

	public static CustomControlColor Purple = new CustomControlColor(Color.FromArgb(210, 170, 255), Color.FromArgb(194, 135, 255), Color.FromArgb(192, 82, 255));

	public Color VeryLightColor => _veryLightColor;

	public Color LightColor => _lightColor;

	public Color DarkColor => _darkColor;

	public CustomControlColor()
	{
	}

	public CustomControlColor(Color veryLightColor, Color lightColor, Color darkColor)
	{
		_veryLightColor = veryLightColor;
		_lightColor = lightColor;
		_darkColor = darkColor;
	}

	public void SetColors(Color veryLightColor, Color lightColor, Color darkColor)
	{
		_veryLightColor = veryLightColor;
		_lightColor = lightColor;
		_darkColor = darkColor;
	}
}
