namespace InfWorld.Utils.Math
{
	class Vector2Int
	{
		public int X, Y;

		public Vector2Int(int x, int y)
		{
			this.X = x;
			this.Y = y;
		}

		public override bool Equals(object obj)
		{
			var @int = obj as Vector2Int;
			return @int != null &&
			       X == @int.X &&
			       Y == @int.Y;
		}

		public override int GetHashCode()
		{
			var hashCode = 1861411795;
			hashCode = hashCode * -1521134295 + X.GetHashCode();
			hashCode = hashCode * -1521134295 + Y.GetHashCode();
			return hashCode;
		}
	}
}
