using System;
using System.Drawing;
using System.Windows.Forms;

namespace CDV_Viewer.Menu;

public class CdvViewerMenu : ToolStripMenuItem
{
	protected void AddSeparator(ref ToolStripSeparator separator)
	{
		separator = new ToolStripSeparator();
		base.DropDownItems.Add(separator);
	}

	protected void AddMenuItem(ref ToolStripMenuItem menuItem, string texte, Image image, Action onClick, bool menuChecked)
	{
		menuItem = new ToolStripMenuItem(texte, image, delegate
		{
			onClick();
		});
		menuItem.Checked = menuChecked;
		base.DropDownItems.Add(menuItem);
	}

	protected void AddMenuItem(ref ToolStripMenuItem menuItem, string texte, Action onClick, bool menuChecked)
	{
		AddMenuItem(ref menuItem, texte, null, onClick, menuChecked);
	}

	protected void AddMenuItem(ref ToolStripMenuItem menuItem, string texte, Image image, Action onClick, Keys shortcutKey, bool menuChecked)
	{
		menuItem = new ToolStripMenuItem(texte, image, delegate
		{
			onClick();
		}, shortcutKey);
		menuItem.Checked = menuChecked;
		base.DropDownItems.Add(menuItem);
	}

	protected void AddMenuItem(ref ToolStripMenuItem menuItem, string texte, Image image, Action onClick, Keys shortcutKey)
	{
		menuItem = new ToolStripMenuItem(texte, image, delegate
		{
			onClick();
		}, shortcutKey);
		base.DropDownItems.Add(menuItem);
	}

	protected void AddMenuItem(ref ToolStripMenuItem menuItem, string texte, Image image, Action onClick)
	{
		menuItem = new ToolStripMenuItem(texte, image, delegate
		{
			onClick();
		});
		base.DropDownItems.Add(menuItem);
	}

	protected void AddMenuItem(ref ToolStripMenuItem menuItem, string texte, Action onClick)
	{
		AddMenuItem(ref menuItem, texte, null, onClick);
	}

	protected void AddMenuItem(ref ToolStripMenuItem menuItem, string texte, Image image, ToolStripItem[] dropDownItems)
	{
		menuItem = new ToolStripMenuItem(texte, image);
		menuItem.DropDown.Items.AddRange(dropDownItems);
		base.DropDownItems.Add(menuItem);
	}

	protected ToolStripMenuItem CreateMenuItem(string texte, Image image, Action onClick)
	{
		return new ToolStripMenuItem(texte, image, delegate
		{
			onClick();
		});
	}

	protected ToolStripMenuItem CreateMenuItem(string texte, Action onClick, bool menuChecked)
	{
		return new ToolStripMenuItem(texte, null, delegate
		{
			onClick();
		})
		{
			Checked = menuChecked
		};
	}

	protected override void OnDropDownOpened(EventArgs e)
	{
		RefreshItems();
	}

	protected virtual void RefreshItems()
	{
	}
}
