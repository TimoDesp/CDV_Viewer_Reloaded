using System;
using System.Drawing;
using System.Windows.Forms;
using CDV_Viewer.Data;
using CDV_Viewer.Styles;

namespace CDV_Viewer.Composants;

public class ComposantMenu : ContextMenuStrip
{
	private string _nom = string.Empty;

	public string Nom => _nom;

	public ComposantMenu()
	{
	}

	public ComposantMenu(string nom)
	{
		_nom = nom;
		if (Nom != string.Empty)
		{
			Items.Add(Nom);
			Items[0].Font = new Font(Font, FontStyle.Bold);
			Items[0].TextAlign = ContentAlignment.MiddleCenter;
			Items[0].BackColor = Colors.GetColor("ContextMenuTitre");
			Items[0].ForeColor = Color.Black;
		}
	}

	public ToolStripMenuItem AddMenu(string texte, AuthorizedMode editMode)
	{
		return AddMenu(texte, null, editMode);
	}

	public ToolStripMenuItem AddMenu(string texte, Image image, AuthorizedMode editMode)
	{
		if (editMode == AuthorizedMode.Edit && !Global.ModeEdition)
		{
			return null;
		}
		if (editMode == AuthorizedMode.Read && Global.ModeEdition)
		{
			return null;
		}
		if (image != null)
		{
			return (ToolStripMenuItem)Items.Add(texte, image);
		}
		return (ToolStripMenuItem)Items.Add(texte);
	}

	public ToolStripMenuItem AddMenu(ToolStripMenuItem menuItem, AuthorizedMode editMode)
	{
		if (menuItem == null)
		{
			return null;
		}
		if (editMode == AuthorizedMode.Edit && !Global.ModeEdition)
		{
			return null;
		}
		if (editMode == AuthorizedMode.Read && Global.ModeEdition)
		{
			return null;
		}
		Items.Add(menuItem);
		return menuItem;
	}

	public ToolStripMenuItem AddMenu(Func<ToolStripMenuItem> getMenu, AuthorizedMode editMode)
	{
		if (editMode == AuthorizedMode.Edit && !Global.ModeEdition)
		{
			return null;
		}
		if (editMode == AuthorizedMode.Read && Global.ModeEdition)
		{
			return null;
		}
		ToolStripMenuItem toolStripMenuItem = getMenu();
		if (toolStripMenuItem == null)
		{
			return null;
		}
		Items.Add(toolStripMenuItem);
		return toolStripMenuItem;
	}

	public ToolStripItem AddItem(string texte, AuthorizedMode editMode)
	{
		if (editMode == AuthorizedMode.Edit && !Global.ModeEdition)
		{
			return null;
		}
		if (editMode == AuthorizedMode.Read && Global.ModeEdition)
		{
			return null;
		}
		return Items.Add(texte);
	}

	public ToolStripItem AddItem(string texte, EventHandler fonction, AuthorizedMode editMode)
	{
		return AddItem(null, texte, null, fonction, editMode);
	}

	public ToolStripItem AddItem(object tag, string texte, EventHandler fonction, AuthorizedMode editMode)
	{
		return AddItem(tag, texte, null, fonction, editMode);
	}

	public ToolStripItem AddItem(string texte, Image image, EventHandler fonction, AuthorizedMode editMode)
	{
		return AddItem(null, texte, image, fonction, editMode);
	}

	public ToolStripItem AddItem(object tag, string texte, Image image, EventHandler fonction, AuthorizedMode editMode)
	{
		if (editMode == AuthorizedMode.Edit && !Global.ModeEdition)
		{
			return null;
		}
		if (editMode == AuthorizedMode.Read && Global.ModeEdition)
		{
			return null;
		}
		ToolStripItem toolStripItem = Items.Add(texte, image, fonction);
		toolStripItem.Tag = tag;
		return toolStripItem;
	}

	public ToolStripItem AddItem(string texte, Action action, AuthorizedMode editMode)
	{
		return AddItem(null, texte, null, action, editMode);
	}

	public ToolStripItem AddItem(object tag, string texte, Action action, AuthorizedMode editMode)
	{
		return AddItem(tag, texte, null, action, editMode);
	}

	public ToolStripItem AddItem(string texte, Image image, Action action, AuthorizedMode editMode)
	{
		return AddItem(null, texte, image, action, editMode);
	}

	public ToolStripItem AddItem(object tag, string texte, Image image, Action action, AuthorizedMode editMode)
	{
		if (editMode == AuthorizedMode.Edit && !Global.ModeEdition)
		{
			return null;
		}
		if (editMode == AuthorizedMode.Read && Global.ModeEdition)
		{
			return null;
		}
		ToolStripItem toolStripItem = Items.Add(texte, image, delegate
		{
			action();
		});
		toolStripItem.Tag = tag;
		return toolStripItem;
	}

	public ToolStripItem[] AddItems(ToolStripItem[] items, AuthorizedMode editMode)
	{
		if (editMode == AuthorizedMode.Edit && !Global.ModeEdition)
		{
			return null;
		}
		if (editMode == AuthorizedMode.Read && Global.ModeEdition)
		{
			return null;
		}
		Items.AddRange(items);
		return items;
	}

	public ToolStripItem AddDropDownItem(ToolStripMenuItem subMenu, string texte, AuthorizedMode editMode)
	{
		return AddDropDownItem(subMenu, null, texte, null, null, editMode, enabled: true);
	}

	public ToolStripItem AddDropDownItem(ToolStripMenuItem subMenu, string texte, AuthorizedMode editMode, bool enabled)
	{
		return AddDropDownItem(subMenu, null, texte, null, null, editMode, enabled);
	}

	public ToolStripItem AddDropDownItem(ToolStripMenuItem subMenu, string texte, Action action, AuthorizedMode editMode)
	{
		return AddDropDownItem(subMenu, null, texte, null, action, editMode, enabled: true);
	}

	public ToolStripItem AddDropDownItem(ToolStripMenuItem subMenu, string texte, Action action, AuthorizedMode editMode, bool enabled)
	{
		return AddDropDownItem(subMenu, null, texte, null, action, editMode, enabled);
	}

	public ToolStripItem AddDropDownItem(ToolStripMenuItem subMenu, string texte, Image image, Action action, AuthorizedMode editMode)
	{
		return AddDropDownItem(subMenu, null, texte, image, action, editMode, enabled: true);
	}

	public ToolStripItem AddDropDownItem(ToolStripMenuItem subMenu, string texte, Image image, Action action, AuthorizedMode editMode, bool enabled)
	{
		return AddDropDownItem(subMenu, null, texte, image, action, editMode, enabled);
	}

	public ToolStripItem AddDropDownItem(ToolStripMenuItem subMenu, object tag, string texte, Image image, Action action, AuthorizedMode editMode, bool enabled)
	{
		if (subMenu == null)
		{
			return null;
		}
		if (editMode == AuthorizedMode.Edit && !Global.ModeEdition)
		{
			return null;
		}
		if (editMode == AuthorizedMode.Read && Global.ModeEdition)
		{
			return null;
		}
		ToolStripItem toolStripItem = subMenu.DropDownItems.Add(texte, image, delegate
		{
			action();
		});
		toolStripItem.Tag = tag;
		toolStripItem.Enabled = enabled;
		return toolStripItem;
	}

	public ToolStripItem[] AddDropDownItems(ToolStripMenuItem subMenu, ToolStripItem[] items, AuthorizedMode editMode)
	{
		if (items == null || items.Length < 1)
		{
			return null;
		}
		if (editMode == AuthorizedMode.Edit && !Global.ModeEdition)
		{
			return null;
		}
		if (editMode == AuthorizedMode.Read && Global.ModeEdition)
		{
			return null;
		}
		subMenu.DropDownItems.AddRange(items);
		return items;
	}
}
