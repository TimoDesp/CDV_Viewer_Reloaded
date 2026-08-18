using System;

namespace CDV_Viewer.Controls;

[Flags]
public enum RoundedCorner
{
	TopLeft = 2,
	TopRight = 4,
	BottomLeft = 8,
	BottomRight = 0x10,
	All = 0x1E
}
