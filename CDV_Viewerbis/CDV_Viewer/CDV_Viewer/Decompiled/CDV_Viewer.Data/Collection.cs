using System;
using System.Collections.Generic;

namespace CDV_Viewer.Data;

public class Collection<T> : List<T>
{
	public delegate void ItemEventHandler(object sender, ItemEventArgs e);

	public class ItemEventArgs : EventArgs
	{
		public T Item;

		public ItemEventArgs(T item)
		{
			Item = item;
		}
	}

	public event EventHandler CollectionChanged;

	public event ItemEventHandler ItemAdded;

	public event ItemEventHandler ItemRemoved;

	public new void Add(T item)
	{
		base.Add(item);
		SendItemAdded(item);
		SendCollectionChanged();
	}

	public new void AddRange(IEnumerable<T> collection)
	{
		base.AddRange(collection);
		foreach (T item in collection)
		{
			SendItemAdded(item);
		}
		SendCollectionChanged();
	}

	public new void Clear()
	{
		T[] array = ToArray();
		base.Clear();
		T[] array2 = array;
		foreach (T item in array2)
		{
			SendItemRemoved(item);
		}
		SendCollectionChanged();
	}

	public new void Insert(int index, T item)
	{
		base.Insert(index, item);
		SendItemAdded(item);
		SendCollectionChanged();
	}

	public new void InsertRange(int index, IEnumerable<T> collection)
	{
		base.InsertRange(index, collection);
		foreach (T item in collection)
		{
			SendItemAdded(item);
		}
		SendCollectionChanged();
	}

	public new void Remove(T item)
	{
		base.Remove(item);
		SendItemRemoved(item);
		SendCollectionChanged();
	}

	public new void RemoveAt(int index)
	{
		T item = base[index];
		base.RemoveAt(index);
		SendItemRemoved(item);
		SendCollectionChanged();
	}

	public new void RemoveAll(Predicate<T> match)
	{
		foreach (T item in FindAll(match))
		{
			base.Remove(item);
			SendItemRemoved(item);
		}
		SendCollectionChanged();
	}

	public new void RemoveRange(int index, int count)
	{
		List<T> list = new List<T>();
		for (int i = index; i < index + count; i++)
		{
			list.Add(base[i]);
		}
		base.RemoveRange(index, count);
		foreach (T item in list)
		{
			SendItemRemoved(item);
		}
		SendCollectionChanged();
	}

	private void SendCollectionChanged()
	{
		OnCollectionChanged(new EventArgs());
		if (this.CollectionChanged != null)
		{
			this.CollectionChanged(this, new EventArgs());
		}
	}

	private void SendItemAdded(T item)
	{
		ItemEventArgs e = new ItemEventArgs(item);
		OnItemAdded(e);
		if (this.ItemAdded != null)
		{
			this.ItemAdded(this, e);
		}
	}

	private void SendItemRemoved(T item)
	{
		ItemEventArgs e = new ItemEventArgs(item);
		OnItemRemoved(e);
		if (this.ItemRemoved != null)
		{
			this.ItemRemoved(this, e);
		}
	}

	protected virtual void OnCollectionChanged(EventArgs e)
	{
	}

	protected virtual void OnItemAdded(ItemEventArgs e)
	{
	}

	protected virtual void OnItemRemoved(ItemEventArgs e)
	{
	}
}
