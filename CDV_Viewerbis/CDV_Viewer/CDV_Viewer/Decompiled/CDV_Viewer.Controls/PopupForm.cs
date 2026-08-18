using System.Windows.Forms;
using CDV_Viewer.Composants;
using CDV_Viewer.DockControls;

namespace CDV_Viewer.Controls;

public class PopupForm : UserControl
{
	public readonly ComposantsViewer ComposantsViewer = ComposantsViewer.Viewer;

	public readonly ComposantsCollection Composants = ComposantsViewer.Viewer?.Composants;

	public event PopupFormResultEventHandler Closing;

	public event PopupFormResultEventHandler Closed;

	protected virtual void OnClosing(PopupFormResultEventArgs e)
	{
	}

	protected virtual void OnClosed(PopupFormResultEventArgs e)
	{
	}

	public void Close(PopupFormResultEventArgs e)
	{
		OnClosing(e);
		if (!e.Canceled)
		{
			this.Closing?.Invoke(this, e);
			if (!e.Canceled)
			{
				(base.Parent as PopupContainer)?.Controls?.Remove(this);
				Dispose();
				OnClosed(e);
				this.Closed?.Invoke(this, e);
			}
		}
	}
}
