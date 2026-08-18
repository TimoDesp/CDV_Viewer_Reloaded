using System;
using CDV_Viewer.Data;

namespace CDV_Viewer.Controls;

public class PopupFormResultEventArgs : EventArgs
{
	public PopupContainerResult Result { get; internal set; }

	public bool Canceled { get; set; }

	public static PopupFormResultEventArgs Ok => new PopupFormResultEventArgs(PopupContainerResult.OK);

	public static PopupFormResultEventArgs Yes => new PopupFormResultEventArgs(PopupContainerResult.Oui);

	public static PopupFormResultEventArgs No => new PopupFormResultEventArgs(PopupContainerResult.Non);

	public static PopupFormResultEventArgs Cancel => new PopupFormResultEventArgs();

	public PopupFormResultEventArgs()
	{
	}

	public PopupFormResultEventArgs(PopupContainerResult result)
	{
		Result = result;
	}
}
