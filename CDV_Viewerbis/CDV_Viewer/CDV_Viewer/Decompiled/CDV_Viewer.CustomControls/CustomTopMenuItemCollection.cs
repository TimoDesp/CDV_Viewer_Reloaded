using System;
using System.Collections.Generic;

namespace CDV_Viewer.CustomControls;

public class CustomTopMenuItemCollection : List<CustomTopMenuItem>
{
	public event EventHandler CollectionChanged;

	public void Add(string tag, string texte)
	{
		Add(new CustomTopMenuItem(tag, texte));
	}

	public new void Add(CustomTopMenuItem item)
	{
		base.Add(item);
		if (this.CollectionChanged != null)
		{
			this.CollectionChanged(this, new EventArgs());
		}
	}
}
