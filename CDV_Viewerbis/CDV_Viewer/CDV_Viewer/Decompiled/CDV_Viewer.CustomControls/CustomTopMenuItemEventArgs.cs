using System;

namespace CDV_Viewer.CustomControls;

public class CustomTopMenuItemEventArgs : EventArgs
{
	public CustomTopMenuItem Item;

	public CustomTopMenuItemEventArgs(CustomTopMenuItem item)
	{
		Item = item;
	}
}
