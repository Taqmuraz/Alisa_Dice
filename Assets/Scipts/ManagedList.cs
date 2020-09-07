using System.Collections;
using System.Collections.Generic;

public sealed class ManagedList<T> : IList<T>
{
	private IList<T> list;

	public ManagedList()
	{
		list = new List<T>();
	}

	public event System.Action ListChanged;

	private void OnListChanged()
	{
		if (ListChanged != null) ListChanged();
	}

	public int IndexOf(T item)
	{
		return list.IndexOf(item);
	}

	public void Insert(int index, T item)
	{
		list.Insert(index, item);
		OnListChanged();
	}

	public void RemoveAt(int index)
	{
		list.RemoveAt(index);
		OnListChanged();
	}

	public T this[int index] { get { return list[index]; } set { list[index] = value; } }

	public void Add(T item)
	{
		list.Add(item);
		OnListChanged();
	}

	public void Clear()
	{
		list.Clear();
		OnListChanged();
	}

	public bool Contains(T item)
	{
		return list.Contains(item);
	}

	public void CopyTo(T[] array, int arrayIndex)
	{
		list.CopyTo(array, arrayIndex);
	}

	public bool Remove(T item)
	{
		bool remove = list.Remove(item);
		OnListChanged();
		return remove;
	}

	public int Count { get { return list.Count; } }

	public bool IsReadOnly { get { return list.IsReadOnly; } }

	public IEnumerator<T> GetEnumerator()
	{
		return list.GetEnumerator();
	}

	IEnumerator IEnumerable.GetEnumerator()
	{
		return ((IEnumerable)list).GetEnumerator();
	}
}
