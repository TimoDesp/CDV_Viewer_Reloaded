using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace CDV_Viewer.CsvBase;

public class BaseTable<T> : IEnumerable<T>, IEnumerable, ICsvTable where T : BaseRow
{
	internal const char CSV_SEPARATOR = ';';

	internal string TempBaseDirectory = "";

	internal string fullPath = "";

	protected bool _isSorted;

	protected SortedDictionary<int, T> _items = new SortedDictionary<int, T>();

	public virtual string Path { get; protected set; } = "";

	public virtual string Header => "";

	public int Count => _items.Count;

	public T this[int id] => _items[id];

	public List<T> this[List<int> ids]
	{
		get
		{
			List<T> list = new List<T>(ids.Count);
			foreach (int id in ids)
			{
				list.Add(this[id]);
			}
			return list;
		}
	}

	public SortedDictionary<int, T> Dictionary => _items;

	public int FreeId()
	{
		int num = 0;
		using (SortedDictionary<int, T>.ValueCollection.Enumerator enumerator = _items.Values.GetEnumerator())
		{
			while (enumerator.MoveNext() && enumerator.Current.ID == num)
			{
				num++;
			}
		}
		return num;
	}

	protected bool TryGetValue(int id, out T row)
	{
		return _items.TryGetValue(id, out row);
	}

	protected bool ContainsKey(int id)
	{
		return _items.ContainsKey(id);
	}

	public void Load(string tempBaseDirectory)
	{
		TempBaseDirectory = tempBaseDirectory;
		fullPath = System.IO.Path.Combine(tempBaseDirectory, Path);
		string[] array = File.ReadAllLines(fullPath);
		List<T> list = new List<T>(array.Length - 1);
		for (int i = 1; i < array.Length; i++)
		{
			string[] csvFields = array[i].Split(';');
			T item = RowFromCsv(csvFields);
			list.Add(item);
		}
		list.Sort((T r1, T r2) => r1.ID.CompareTo(r2.ID));
		_items = new SortedDictionary<int, T>(list.ToDictionary((T val) => val.ID));
		Base.NeedLink();
	}

	public void LoadBinary(BinaryReader reader)
	{
		int num = reader.ReadInt32();
		List<T> list = new List<T>();
		for (int i = 0; i < num; i++)
		{
			T item = RowFromBinary(reader);
			list.Add(item);
		}
		list.Sort((T r1, T r2) => r1.ID.CompareTo(r2.ID));
		_items = new SortedDictionary<int, T>(list.ToDictionary((T val) => val.ID));
		Base.NeedLink();
	}

	public virtual void Clear()
	{
		_items.Clear();
		Base.NeedLink();
	}

	public virtual T RowFromCsv(string[] csvFields)
	{
		return null;
	}

	public virtual T RowFromBinary(BinaryReader reader)
	{
		return null;
	}

	public virtual void Link()
	{
	}

	public void Remove(int id)
	{
		if (_items.ContainsKey(id))
		{
			_items.Remove(id);
		}
		Base.NeedLink();
	}

	public void RemoveAll(IEnumerable<int> keys)
	{
		foreach (int key in keys)
		{
			if (_items.ContainsKey(key))
			{
				_items.Remove(key);
			}
		}
		Base.NeedLink();
	}

	public void RemoveAll(Func<T, bool> match)
	{
		foreach (T item in FindAll(match))
		{
			_items.Remove(item.ID);
		}
		Base.NeedLink();
	}

	public void Add(T row)
	{
		_items.Add(row.ID, row);
		Base.NeedLink();
	}

	public void Add(List<T> list)
	{
		foreach (T item in list)
		{
			_items.Add(item.ID, item);
		}
		Base.NeedLink();
	}

	public List<T> FindAll(Func<T, bool> match)
	{
		return _items.Values.Where(match).ToList();
	}

	public T Find(Func<T, bool> match)
	{
		return _items.Values.First(match);
	}

	public void Save()
	{
		string directoryName = System.IO.Path.GetDirectoryName(fullPath);
		if (!Directory.Exists(directoryName))
		{
			Directory.CreateDirectory(directoryName);
		}
		IOrderedEnumerable<T> orderedEnumerable = _items.Values.OrderBy((T item) => item.ID);
		StreamWriter streamWriter = new StreamWriter(fullPath, append: false, Encoding.GetEncoding(1252));
		streamWriter.WriteLine(Header);
		foreach (T item in orderedEnumerable)
		{
			streamWriter.WriteLine(item.ToCsv());
		}
		streamWriter.Close();
		streamWriter.Dispose();
	}

	public void BinarySave(BinaryWriter writer)
	{
		IOrderedEnumerable<T> orderedEnumerable = _items.Values.OrderBy((T item) => item.ID);
		writer.Write(orderedEnumerable.Count());
		foreach (T item in orderedEnumerable)
		{
			item.ToBinaryStream(writer);
		}
	}

	public IEnumerator<T> GetEnumerator()
	{
		return _items.Values.GetEnumerator();
	}

	IEnumerator IEnumerable.GetEnumerator()
	{
		return _items.Values.GetEnumerator();
	}
}
