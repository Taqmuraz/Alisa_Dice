public sealed partial class DiceGame
{
	private sealed class Player
	{
		public Player(string name)
		{
			this.name = name;
		}

		public string name { get; private set; }
		public int scores { get; set; }

		public void Reset()
		{
			scores = 0;
		}

		public override string ToString()
		{
			return string.Format("{0} {1}", name, scores);
		}
	}
}
