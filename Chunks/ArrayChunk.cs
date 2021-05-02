using System.Collections;
using System.Collections.Generic;

namespace InfWorld.Chunks
{
	public class ArrayChunk<T> : IEnumerable
	{
		/// <summary>
		/// Return a T depending on the vector position
		/// </summary>
		private readonly Dictionary<Vector2Int, T[,]> dictionary;
		/// <summary>
		/// 
		/// </summary>
		private T Default;

		public ArrayChunk()
		{
			dictionary = new Dictionary<Vector2Int, T[,]>();
			Default = default(T);
		}


		public ArrayChunk(T def) : this()
		{
			Default = def;
		}

		public T this[int x, int y]
		{
			get
			{
				Vector2Int chunkPos = new Vector2Int(x / 150, y / 150);
				if (dictionary.ContainsKey(chunkPos))
				{
					return dictionary[chunkPos][x % 150, y % 150];
				}
				return Default;
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
					dictionary[chunkPos] = new T[150, 150];
				}
				dictionary[chunkPos][x % 150, y % 150] = value;
			}
		}



		private bool IsDefault(T value)
		{
			return (Default != null && Default.Equals(value)) || (Default == null || value == null);
		}

		private bool DoesExist(Vector2Int chunkPos)
		{
			return dictionary.ContainsKey(chunkPos);
		}

		public IEnumerator GetEnumerator()
		{
			return dictionary.GetEnumerator();
		}
	}
}
