using System.Collections;
using System.Collections.Generic;
using InfWorld.Utils.Math;

namespace InfWorld.Utils
{
	public class ArrayChunk<T> : IEnumerable
	{
		/// <summary>
		/// Return a T depending on the vector position
		/// </summary>
		private readonly Dictionary<Vector2Int, T[,]> m_dictionary;
		/// <summary>
		/// 
		/// </summary>
		private T m_default;

		public ArrayChunk()
		{
			m_dictionary = new Dictionary<Vector2Int, T[,]>();
			m_default = default(T);
		}


		public ArrayChunk(T def) : this()
		{
			m_default = def;
		}

		public T this[int x, int y]
		{
			get
			{
				Vector2Int chunkPos = new Vector2Int(x / 150, y / 150);
				if (m_dictionary.ContainsKey(chunkPos))
				{
					return m_dictionary[chunkPos][x % 150, y % 150];
				}
				return m_default;
			}
			set
			{
				Vector2Int chunkPos = new Vector2Int(x / 150, y / 150);
				/*bool isDefault = IsDefault(value);
				if (isDefault)
				{
					if (DoesExist(chunkPos))
					{
						dictionary.Remove(chunkPos);
						return;
					}
				}*/

				if (!DoesExist(chunkPos))
				{
					m_dictionary[chunkPos] = new T[150, 150];
				}
				m_dictionary[chunkPos][x % 150, y % 150] = value;
			}
		}



		private bool IsDefault(T value)
		{
			return (m_default != null && m_default.Equals(value)) || (m_default == null || value == null);
		}

		private bool DoesExist(Vector2Int chunkPos)
		{
			return m_dictionary.ContainsKey(chunkPos);
		}

		public IEnumerator GetEnumerator()
		{
			return m_dictionary.GetEnumerator();
		}
	}
}
