using System.Collections.Generic;

namespace EzNet.Utils
{
	/// <summary>
	/// Bi-directional dictionary
	/// </summary>
	public class BiDiDictionary<T1, T2>
	{
		private readonly Dictionary<T1, T2> _dictAB = new Dictionary<T1, T2>();
		private readonly Dictionary<T2, T1> _dictBA = new Dictionary<T2, T1>();
		
		public T2 this[T1 key]
		{
			get => _dictAB[key];
			set
			{
				_dictAB[key] = value;
				_dictBA[value] = key;
			}
		}
		
		public T1 this[T2 key]
		{
			get => _dictBA[key];
			set
			{
				_dictAB[value] = key;
				_dictBA[key] = value;
			}
		}

		public bool TryGetValue(T1 key, out T2 value) => _dictAB.TryGetValue(key, out value);
		public bool TryGetValue(T2 key, out T1 value) => _dictBA.TryGetValue(key, out value);
	}
}
