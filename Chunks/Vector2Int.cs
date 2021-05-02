namespace InfWorld.Chunks
{
	class Vector2Int
	{
		public int X, Y;

		public Vector2Int(int X, int Y)
		{
			this.X = X;
			this.Y = Y;
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
