using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyScript
{
	public abstract class Type
	{
		public abstract bool isInstance(object o);
		public abstract bool isSupertypeof(Type o);
		public class AllType : Type
		{
			public override bool isInstance(object o)
			{
				return true;
			}

			public override bool isSupertypeof(Type o)
			{
				return true;
			}
		}

		private static readonly Type all = new AllType();

		public static Type All => all;
	}
	public class TypedDictionary<K,V> : IDictionary<K, V> , IDictionary<K, TypedDictionary<K, V>.TypedValue> where K : notnull 
	{
		public record struct TypedValue(Type Type, V Value);
		Dictionary<K, TypedValue> datas=new();

        public TypedDictionary(TypedDictionary<K, V> from)
        {
            this.datas = new(from.datas);
        }

        public TypedDictionary()
        {
        }

        public V this[K key] { get => datas[key].Value; set => datas[key]=new(Type.All, value); }

		public ICollection<K> Keys => datas.Keys;

		public ICollection<V> Values => datas.Values.ConvertAll(r=>r.Value);

		public int Count => datas.Count;

		public bool IsReadOnly => false;

		ICollection<TypedDictionary<K, V>.TypedValue> IDictionary<K, TypedDictionary<K, V>.TypedValue>.Values => ((IDictionary<K, TypedDictionary<K, V>.TypedValue>)datas).Values;

		TypedDictionary<K, V>.TypedValue IDictionary<K, TypedDictionary<K, V>.TypedValue>.this[K key] { get => ((IDictionary<K, TypedDictionary<K, V>.TypedValue>)datas)[key]; set => ((IDictionary<K, TypedDictionary<K, V>.TypedValue>)datas)[key] = value; }
		public void SetType(K key, Type t)
        {
			datas[key]= datas[key] with { Type = t };
        }
		public void Add(K key, V value)
		{
			if (datas.ContainsKey(key)) datas.Add(key,new());
			this[key]=value;
		}

		public void Add(KeyValuePair<K, V> item)
		{
			if (datas.ContainsKey(item.Key)) datas.Add(item.Key, new());
			this[item.Key] = item.Value;
		}

		public void Clear()
		{
			datas.Clear();
		}

		public bool Contains(KeyValuePair<K, V> item) => datas.Any(kv => kv.Key.Equals(item.Key) && (kv.Value.Value?.Equals(item.Value) ?? item.Value is null));

		public bool ContainsKey(K key) => datas.ContainsKey(key);

		public void CopyTo(KeyValuePair<K, V>[] array, int arrayIndex)
		{
			foreach (var item in this)
			{
				array[arrayIndex++]=item;
			}
		}

		public IEnumerator<KeyValuePair<K, V>> GetEnumerator()
		{
			foreach (var item in datas)
			{
				yield return new(item.Key, item.Value.Value);
			}
		}

		public bool Remove(K key) => datas.Remove(key);

		public bool Remove(KeyValuePair<K, V> item)
		{
			if(!Contains(item)) return false;
			return datas.Remove(item.Key);
		}

		public bool TryGetValue(K key, [MaybeNullWhen(false)] out V value)
		{
			if(datas.TryGetValue(key, out var typedValue))
			{
				value = typedValue.Value;
				return true;
            }
            else
            {
				value = default;
				return false;
            }
		}

		IEnumerator IEnumerable.GetEnumerator()=>GetEnumerator();

		public void Add(K key, TypedDictionary<K, V>.TypedValue value)
		{
			((IDictionary<K, TypedDictionary<K, V>.TypedValue>)datas).Add(key, value);
		}

		public bool TryGetValue(K key, [MaybeNullWhen(false)] out TypedDictionary<K, V>.TypedValue value)
		{
			return ((IDictionary<K, TypedDictionary<K, V>.TypedValue>)datas).TryGetValue(key, out value);
		}

		public void Add(KeyValuePair<K, TypedDictionary<K, V>.TypedValue> item)
		{
			((ICollection<KeyValuePair<K, TypedDictionary<K, V>.TypedValue>>)datas).Add(item);
		}

		public bool Contains(KeyValuePair<K, TypedDictionary<K, V>.TypedValue> item)
		{
			return ((ICollection<KeyValuePair<K, TypedDictionary<K, V>.TypedValue>>)datas).Contains(item);
		}

		public void CopyTo(KeyValuePair<K, TypedDictionary<K, V>.TypedValue>[] array, int arrayIndex)
		{
			((ICollection<KeyValuePair<K, TypedDictionary<K, V>.TypedValue>>)datas).CopyTo(array, arrayIndex);
		}

		public bool Remove(KeyValuePair<K, TypedDictionary<K, V>.TypedValue> item)
		{
			return ((ICollection<KeyValuePair<K, TypedDictionary<K, V>.TypedValue>>)datas).Remove(item);
		}

		IEnumerator<KeyValuePair<K, TypedDictionary<K, V>.TypedValue>> IEnumerable<KeyValuePair<K, TypedDictionary<K, V>.TypedValue>>.GetEnumerator()
		{
			return ((IEnumerable<KeyValuePair<K, TypedDictionary<K, V>.TypedValue>>)datas).GetEnumerator();
		}
	}
}
