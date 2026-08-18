using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using CDV_Viewer.CsvBase;

namespace CDV_Viewer.Data.SIGObjects;

public class SigDictionary<T> : IEnumerable<T>, IEnumerable where T : ISigId
{
	private Dictionary<int, T> _items = new Dictionary<int, T>();

	private T _currentItem;

	private int _currentId => _currentItem?.ID ?? (-1);

	public T this[int id]
	{
		get
		{
			if (_currentId == id)
			{
				return _currentItem;
			}
			return _currentItem = _items[id];
		}
		set
		{
			if (value.ID != id)
			{
				throw new ArgumentException("ID doesn't match");
			}
			_items[id] = value;
		}
	}

	public SigDictionary()
	{
	}

	public SigDictionary(IEnumerable<T> data, bool overwrite = true)
	{
		if (!overwrite)
		{
			data.ToDictionary((T d) => d.ID);
			return;
		}
		foreach (T datum in data)
		{
			if (datum.ID >= 0)
			{
				_items[datum.ID] = datum;
			}
		}
	}

	public static SigDictionary<T> FromRowsTable<TSource>(IEnumerable<TSource> table, Func<TSource, int> keySelector, Func<IEnumerable<TSource>, T> converter) where TSource : BaseRow
	{
		Dictionary<int, T> dictionary = new Dictionary<int, T>();
		foreach (IGrouping<int, TSource> item in table.GroupBy(keySelector))
		{
			dictionary.Add(item.Key, converter(item));
		}
		return new SigDictionary<T>
		{
			_items = dictionary
		};
	}

	public static SigDictionary<T> FromSigTable<TSource>(IEnumerable<IGrouping<int, TSource>> group, Func<IEnumerable<TSource>, T> converter)
	{
		Dictionary<int, T> dictionary = new Dictionary<int, T>();
		foreach (IGrouping<int, TSource> item in group)
		{
			dictionary.Add(item.Key, converter(item));
		}
		return new SigDictionary<T>
		{
			_items = dictionary
		};
	}

	public bool TryGetValue(int id, out T value)
	{
		if (_currentId == id)
		{
			value = _currentItem;
			return true;
		}
		if (_items.TryGetValue(id, out value))
		{
			_currentItem = value;
			return true;
		}
		return false;
	}

	public void Clear()
	{
		_currentItem = default(T);
		_items.Clear();
	}

	public void Add(T item)
	{
		if (item != null && item.ID >= 0)
		{
			_items.Add(item.ID, item);
			_currentItem = item;
		}
	}

	public void Remove(T item)
	{
		if (item == null)
		{
			return;
		}
		int iD = item.ID;
		if (item.ID >= 0)
		{
			if (_items.ContainsKey(iD))
			{
				_items.Remove(iD);
			}
			if (_currentId == iD)
			{
				_currentItem = default(T);
			}
		}
	}

	public int FreeId()
	{
		int num = -1;
		using (IEnumerator<T> enumerator = _items.Values.OrderBy((T n) => n.ID).GetEnumerator())
		{
			while (enumerator.MoveNext() && enumerator.Current.ID == ++num)
			{
			}
		}
		return num;
	}

	public T[] ToArray()
	{
		T[] array = new T[_items.Count];
		_items.Values.CopyTo(array, 0);
		return array;
	}

	public List<T> ToList()
	{
		T[] array = new T[_items.Count];
		_items.Values.CopyTo(array, 0);
		return new List<T>(array);
	}

	public IEnumerator<T> GetEnumerator()
	{
		return ((IEnumerable<T>)_items.Values).GetEnumerator();
	}

	IEnumerator IEnumerable.GetEnumerator()
	{
		return _items.Values.GetEnumerator();
	}
}
