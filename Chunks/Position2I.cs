using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Xna.Framework;

namespace InfWorld.Chunks
{
	/// <summary>
	/// A class that holds an X and Y position
	/// </summary>
	[Serializable]
	public class Position2I : ISerializable
	{
		/// <summary>
		/// X Position
		/// </summary>
		public readonly int X;

		/// <summary>
		/// Y Position
		/// </summary>
		public readonly int Y;

		/// <summary>
		/// Create a new instance of this class by specifying an X and Y position
		/// </summary>
		/// <param name="x">The X position</param>
		/// <param name="y">The Y position</param>
		public Position2I(int x, int y)
		{
			X = x;
			Y = y;
		}

		/// <summary>
		/// Create a new instance of this class from a vector
		/// </summary>
		/// <param name="vector">The vector to copy the position from</param>
		public Position2I(double v, Vector2 vector) : this((int)Math.Round(vector.X), (int)Math.Round(vector.Y)) { }

		/// <summary>
		/// Convert this instance to a Vector2
		/// </summary>
		/// <returns>A new Vector2 with the position of this instance</returns>
		public Vector2 ToVector2()
		{
			return new Vector2(X, Y);
		}

		/// <inheritdoc />
		public override string ToString()
		{
			return "{" + X + ", " + Y + "}";
		}

		/// <inheritdoc />
		public override bool Equals(object obj)
		{
			return obj is Position2I i && X == i.X && Y == i.Y;
		}

		/// <inheritdoc />
		public override int GetHashCode()
		{
			var hashCode = 1861411795;
			hashCode = hashCode * -1521134295 + X.GetHashCode();
			hashCode = hashCode * -1521134295 + Y.GetHashCode();
			return hashCode;
		}

		[SecurityPermission(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.SerializationFormatter)]
		public void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			
			if (info == null)
			{
				throw new ArgumentNullException("info");
			}
			info.AddValue("x", X);
			info.AddValue("y", Y);
		}
	}
}
