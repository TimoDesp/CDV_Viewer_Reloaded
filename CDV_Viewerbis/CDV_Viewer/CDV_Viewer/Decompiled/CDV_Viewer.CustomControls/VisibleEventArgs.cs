using System;

namespace CDV_Viewer.CustomControls;

public class VisibleEventArgs : EventArgs
{
	public string Text;

	public bool Visible;

	public VisibleEventArgs(string text, bool visible)
	{
		Text = text;
		Visible = visible;
	}
}
