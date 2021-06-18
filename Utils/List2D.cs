using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Security.Permissions;
using InfWorld.Utils.Math;

namespace InfWorld.Utils
{
	/// <summary>
	/// A 2-Dimensional List
	/// </summary>
	/// <typeparam name="T">The type to store in this list</typeparam>
	public class List2D<T> : IEnumerable<KeyValuePair<Position2I, T>>, ISerializable
	{
		/// <summary>
		/// The inner dictionary, which contains all the values in this list
		/// </summary>
		private readonly Dictionary<Position2I, T> m_list;

		/// <summary>
		/// The default value to return if there is nothing at the specified index.
		/// </summary>
		public T Default { get; set; }

		/// <summary>
		/// The amount of values in this list
		/// </summary>
		public int Count => m_list.Count;

		/// <summary>
		/// Initializes a new instance of this class
		/// </summary>
		public List2D()
		{
			m_list = new Dictionary<Position2I, T>();
            Default = default(T);
		}

		/// <summary>
		/// Initializes a new instance of this class with a specified default value
		/// </summary>
		/// <param name="def">default value</param>
		public List2D(T def) : this()
		{
			Default = def;
		}

		/// <summary>
		/// Get the value at the specified position
		/// </summary>
		/// <param name="pos">Position of the value</param>
		/// <returns>The value at the specified position</returns>
		//public T this[Vector2 pos] => this[new Position2I(pos)];

		/// <summary>
		/// Get the value at the specified position
		/// </summary>
		/// <param name="pos">Position of the value</param>
		/// <returns>The value at the specified position</returns>
		public T this[Position2I pos]
        {
            get => this[pos.X, pos.Y];
            set => this[pos.X, pos.Y] = value;
        }

        /// <summary>
		/// Get the value at the specified position
		/// </summary>
		/// <param name="x">X coordinate of the value</param>
		/// <param name="y">Y coordinate of the value</param>
		/// <returns>The value at the specified position</returns>
		public T this[int x, int y]
		{
			get
			{
				var key = new Position2I(x, y);
				if (!m_list.ContainsKey(key))
					return Default;
				return m_list[key];
			}
			set
			{
				var key = new Position2I(x, y);
				bool isDefault = IsDefault(value);
				if (isDefault)
				{
					if (m_list.ContainsKey(key))
						m_list.Remove(key);
				}
				else
					m_list[key] = value;
			}
		}

		/// <summary>
		/// Checks if the specified value is equivalent to the default value
		/// </summary>
		/// <param name="value">Value to compare to the default</param>
		/// <returns>True if the value matches the default, otherwise false</returns>
		private bool IsDefault(T value)
		{
			return (Default != null && Default.Equals(value)) || (Default == null && value == null);
		}

		/// <summary>
		/// Gets the value at the specified index
		/// </summary>
		/// <param name="index">Internal index of value</param>
		/// <returns>The value at the specified index</returns>
		public KeyValuePair<Position2I, T> GetAtIndex(int index)
		{
			return new KeyValuePair<Position2I, T>(GetPositionArray()[index], GetValueArray()[index]);
		}

		/// <summary>
		/// Gets the internal value array
		/// </summary>
		/// <returns>The internal value array</returns>
		public T[] GetValueArray()
		{
			return m_list.Values.ToArray();
		}

		/// <summary>
		/// Gets the internal position array
		/// </summary>
		/// <returns>The internal position array</returns>
		public Position2I[] GetPositionArray()
		{
			return m_list.Keys.ToArray();
		}

		/// <inheritdoc />
		public IEnumerator<KeyValuePair<Position2I, T>> GetEnumerator()
		{
			// Manual Way
			/*foreach (var value in _list)
			{
				Position2I position = value.Key;
				T val = value.Value;
				yield return new KeyValuePair<Position2I, T>(position, val);
			}*/

			return m_list.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}

		[SecurityPermission(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.SerializationFormatter)]
		public void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			if (info == null)
			{
				throw new ArgumentNullException("info");
			}
			info.AddValue("list", m_list);
		}
	}
}
